//
// UIViewExtensions.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
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
using System;
using UIKit;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using Rx.Core;

namespace Rx.iOS.Extenisons
{
    public static class UIViewExtensions
    {
        public static IObservable<UILongPressGestureRecognizer> WhenLongPressed(
            this UIView This, double minimumPressDuration = 0.5d, bool delayBeginTouch = true, bool delayEndTouch = false)
        {
            return Observable.Create<UILongPressGestureRecognizer>(obser =>
            {
                var gesture = new UILongPressGestureRecognizer(_ => obser.OnNext(_))
                {
                    MinimumPressDuration = minimumPressDuration,
                    DelaysTouchesBegan = true,
                    DelaysTouchesEnded = delayEndTouch
                };
                return WireupGesture(This, gesture);
            });
        }

        public static IObservable<UITapGestureRecognizer> WhenTapped(this UIView This, int taps = 1)
        {
            return Observable.Create<UITapGestureRecognizer>(obser =>
            {
                var gesture = new UITapGestureRecognizer(_ => obser.OnNext(_))
                {
                    NumberOfTapsRequired = (nuint)taps
                };
                return WireupGesture(This, gesture);
            });
        }

        public static IDisposable WhenTapped(this UIView This, IObserver<Unit> observer, int taps = 1)
        {
            return WhenTapped(This, taps)
                .Subscribe(_ => observer.OnNext(Unit.Default));
        }

        public static IDisposable WireupGesture(this UIView view,UIGestureRecognizer gesture)
        {
			var userInteraction = view.UserInteractionEnabled;
			view.UserInteractionEnabled = true;

			view.AddGestureRecognizer(gesture);
			return Disposable.Create(() =>
			{
				view.RemoveGestureRecognizer(gesture);
				view.UserInteractionEnabled = userInteraction;
			});
        }

        internal static IEnumerable<ISupportRxSubscription> GetAllRxViews(this UIView This)
        {
            var list = new List<ISupportRxSubscription>();

            FindAllRxViews(This);

            return list;

            void FindAllRxViews(UIView viewGrp)
            {
                if (viewGrp == null)
                    return;
                if (viewGrp is ISupportRxSubscription rxView && !list.Contains(rxView))
                    list.Add(rxView);

                foreach (var view in viewGrp.Subviews)
                {
                    switch (view)
                    {
                        case ISupportRxSubscription rxViewInternal:
                            if (!list.Contains(rxViewInternal))
                                list.Add(rxViewInternal);
                            break;
                    }
                    if (view.Subviews.Any())
                        FindAllRxViews(view);
                }
            }
        }

    }
}
