using System;
using UIKit;
using System.Reactive.Linq;
using WebKit;
using Foundation;
using Bss.iOS.Utils;
using System.Reactive.Disposables;

namespace Rx.Extensions
{
	public enum PageLoadState
	{
		Started,
		Finished,
		Error
	}

	public class PageLoadArgs : EventArgs
	{
		public PageLoadArgs(PageLoadState state)
		{
			State = state;
		}

		public PageLoadArgs(NSError error)
		{
			State = PageLoadState.Error;
			Error = error;
		}

		public PageLoadState State { get; }
		public NSError Error { get; }
	}

	public static class UIWebViewExtensions
	{
		public static IObservable<PageLoadArgs> WhenPageLoad(this UIWebView This)
		{
			var started = Observable.FromEventPattern(e => This.LoadStarted += e, e => This.LoadStarted -= e)
												.Select(_ => new PageLoadArgs(PageLoadState.Started));

			var finished = Observable.FromEventPattern(e => This.LoadFinished += e, e => This.LoadFinished -= e)
									 .Select(_ => new PageLoadArgs(PageLoadState.Finished));

			var err = Observable.FromEventPattern<UIWebErrorArgs>(e => This.LoadError += e, e => This.LoadError -= e)
								.Select(_ => new PageLoadArgs(_.EventArgs.Error));

			return started.Merge(finished)
						  .Merge(err);
		}

		/// <summary>
		/// Warning: Don't set NavigationDelegate or this will not work
		/// </summary>
		/// <returns>The page load.</returns>
		/// <param name="This">This.</param>
		public static IObservable<PageLoadArgs> WhenPageLoad(this WKWebView This)
		{
			return Observable.Create<PageLoadArgs>(obser =>
			{
				This.NavigationDelegate = new WebViewDelegate(obser);
				return Disposable.Create(() => This.NavigationDelegate = null);
			});
		}

		private class WebViewDelegate : WKNavigationDelegate
		{
			private readonly BIWeakReference<IObserver<PageLoadArgs>> _weakRef;

			private IObserver<PageLoadArgs> Observer => _weakRef.Target;

			public WebViewDelegate(IObserver<PageLoadArgs> observer)
			{
				_weakRef = new BIWeakReference<IObserver<PageLoadArgs>>(observer);
			}

			public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
			{
				Observer?.OnNext(new PageLoadArgs(error));
			}

			public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
			{
				Observer?.OnNext(new PageLoadArgs(PageLoadState.Finished));
			}

			public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
			{
				Observer?.OnNext(new PageLoadArgs(PageLoadState.Started));
			}
		}
	}
}