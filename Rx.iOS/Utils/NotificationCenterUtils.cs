//
// NotificationCenterUtils.cs
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
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Foundation;
using UIKit;

namespace Rx.iOS.Utils
{
    public class KeyboardEventArgs : UIKeyboardEventArgs
    {
        public KeyboardEventArgs(NSNotification notification, bool isShowing) : base(notification)
        {
            IsShowing = isShowing;
        }

        public bool IsShowing { get; }
    }

    public static class NotificationCenterUtils
    {
        /// <summary>
        /// Whens the keyboard appear.
        /// </summary>
        /// <returns>The true if the keyboard appear.</returns>
        public static IObservable<KeyboardEventArgs> WhenKeyboardAppear()
        {
            return Observable.Create<KeyboardEventArgs>(obser =>
            {
                var disposable = new CompositeDisposable();

                disposable.Add(UIKeyboard.Notifications.ObserveWillHide((sender, e) =>
                {
                    var ev = new KeyboardEventArgs(e.Notification, false);
                    obser.OnNext(ev);
                }));
                disposable.Add(UIKeyboard.Notifications.ObserveDidShow((sender, e) =>
                {
                    if (e.FrameBegin.Equals(e.FrameEnd))
                        return;
                    var ev = new KeyboardEventArgs(e.Notification, true);
                    obser.OnNext(ev);
                }));
                return disposable;
            });
        }
    }
}
