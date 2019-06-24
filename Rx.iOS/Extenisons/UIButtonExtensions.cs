//
// UIButtonExtensions.cs
//
// Author:
//       valentingrigorean <valentin.grigorean1@gmail.com>
//
// Copyright (c) 2017 (c) Grigorean Valentin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UIKit;
using System.Reactive.Linq;
using System;
using System.Reactive;
using Bss.iOS.UIKit;
using System.Reactive.Disposables;

namespace Rx.Extensions
{
	public static class UIButtonViewExtensions
	{
		public static IObservable<EventPattern<object>> WhenClick(this UIButton This)
		{
			return Observable.FromEventPattern(e => This.TouchUpInside += e, e => This.TouchUpInside -= e);
		}

		public static IDisposable WhenClick(this UIButton This, Action<object, object> action)
		{
			return This.WhenClick()
					   .Subscribe(e => action?.Invoke(e.Sender, e.EventArgs));
		}

		public static IDisposable WhenClick(this UIButton This, Action action)
		{
			return This.WhenClick()
					   .Subscribe(e => action?.Invoke());
		}

		public static IObservable<EventPattern<object>> WhenClick(this IClickableView This)
		{
			return Observable.FromEventPattern(e => This.Click += e, e => This.Click -= e);
		}

		public static IDisposable WhenClick(this IClickableView This, Action<object, object> action)
		{
			return This.WhenClick()
					   .Subscribe(e => action?.Invoke(e.Sender, e.EventArgs));
		}

		public static IDisposable WhenClick(this IClickableView This, Action action)
		{
			return This.WhenClick()
					   .Subscribe(e => action?.Invoke());
		}

		public static IObservable<Unit> AddWhenClick(this UIView This)
		{
			return Observable.Create<Unit>(obser =>
			{
				This.UserInteractionEnabled = true;
				var tap = new UITapGestureRecognizer(() => obser.OnNext(Unit.Default))
				{
					NumberOfTapsRequired = (nuint)1
				};
				This.AddGestureRecognizer(tap);
				return Disposable.Create(() => This.RemoveGestureRecognizer(tap));
			});
		}

		public static IDisposable AddClickGesture(this UIView This, Action action, int numberOfTaps = 1)
		{
			This.UserInteractionEnabled = true;
			var tap = new UITapGestureRecognizer(() => action?.Invoke())
			{
				NumberOfTapsRequired = (nuint)numberOfTaps
			};
			This.AddGestureRecognizer(tap);
			return Disposable.Create(() => This.RemoveGestureRecognizer(tap));
		}

	}
}
