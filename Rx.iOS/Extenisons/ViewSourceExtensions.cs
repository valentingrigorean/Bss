using System;
using Bss.iOS.UIKit;
using System.Reactive.Linq;
using System.Reactive;

namespace Rx.Extensions
{
    public static class ViewSourceExtensions
    {
		public static IObservable<RowClickedEventArgs<T>> WhenItemClicked<T>(this CollectionViewSource<T> This)
		{
			return Observable.FromEventPattern<RowClickedEventHandler<T>, RowClickedEventArgs<T>>(
				e => This.ItemClicked += e, e => This.ItemClicked -= e)
							 .Select(arg => arg.EventArgs);
		}

        public static IObservable<RowClickedEventArgs<T>> WhenRowClicked<T>(this ViewSource<T> This)
        {
            return Observable.FromEvent<RowClickedEventHandler<T>, RowClickedEventArgs<T>>(
                    handler =>
            {
                RowClickedEventHandler<T> _handler = (sender, e) => handler(e);
                return _handler;
            }, e => This.RowClicked += e, e => This.RowClicked -= e);
        }

        public static IDisposable WhenRowClicked<T>(this ViewSource<T> This, Action<RowClickedEventArgs<T>> callback)
        {
            return This.WhenRowClicked()
                       .Subscribe(callback);
        }

        public static IObservable<Unit> WhenScrolled<T>(this ViewSource<T> This)
        {
            return Observable.FromEventPattern(e => This.ViewDidScroll += e, e => This.ViewDidScroll -= e)
                             .Select(_ => Unit.Default);
        }
    }
}