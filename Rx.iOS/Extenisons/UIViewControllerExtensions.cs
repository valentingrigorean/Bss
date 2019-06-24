//
// NSNotificationCenterExtensions.cs
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
using UIKit;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Rx.iOS.Utils;

namespace Rx.Extensions
{
    public static class UIViewControllerExtensions
    {       
        public static IObservable<KeyboardEventArgs> WhenKeyboardAppear(this UIViewController This)
        {
            return NotificationCenterUtils.WhenKeyboardAppear();
        }

        public static IDisposable AutoHideKeyboard(this UIViewController This, UISearchBar searchBar)
        {
            var tabGesture = new UITapGestureRecognizer(() => searchBar.ResignFirstResponder());
            var disposable = new CompositeDisposable
            {
                Disposable.Create(() => This.View.RemoveGestureRecognizer(tabGesture)),
                This.WhenKeyboardAppear().Select(e=>e.IsShowing)
                       .Subscribe(isVisible =>
                       {
                           if (isVisible)
                           {
                               This.View.AddGestureRecognizer(tabGesture);
                           }
                           else
                               This.View.RemoveGestureRecognizer(tabGesture);
                       })
            };
            return disposable;
        }

        public static IDisposable AutoHideKeyboard(this UIViewController This, UITextField textField)
        {
            var tabGesture = new UITapGestureRecognizer(() => textField.ResignFirstResponder());
            var disposable = new CompositeDisposable
            {
                Disposable.Create(() => This.View.RemoveGestureRecognizer(tabGesture)),
                This.WhenKeyboardAppear()
                .Select(e=>e.IsShowing)
                       .Subscribe(isVisible =>
                       {
                           if (isVisible)
                               This.View.AddGestureRecognizer(tabGesture);
                           else
                               This.View.RemoveGestureRecognizer(tabGesture);
                       })
            };
            return disposable;
        }

    }
}