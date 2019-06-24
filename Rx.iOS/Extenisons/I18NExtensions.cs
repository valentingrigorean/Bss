//
// I18NExtensions.cs
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
using System.Reactive.Disposables;
using Bss.iOS.UIKit;

namespace Rx.iOS.Extensions
{
    public static class I18NExtensions
    {
        public static void SetText(this UILabel This, IObservable<string> observable, CompositeDisposable disp, bool isUpper = false)
        {
            observable.Subscribe(text =>
            {
                if (isUpper)
                {
                    This.Text = text.ToUpperInvariant();
                }
                else
                {
                    This.Text = text;
                }

            }).DisposeWith(disp);
        }

        public static void SetText(this UIButton This, IObservable<string> observable, CompositeDisposable disp, bool isUpper = false)
        {
            observable.Subscribe(text =>
            {
                if (isUpper)
                {
                    This.SetTitle(text.ToUpperInvariant(), UIControlState.Normal);
                }
                else
                {
                    This.SetTitle(text, UIControlState.Normal);
                }

            }).DisposeWith(disp);
        }

        public static void SetText(this UIButtonView This, IObservable<string> observable, CompositeDisposable disp, bool isUpper = false)
        {
            observable.Subscribe(text =>
            {
                if (isUpper)
                {
                    This.SetTitle(text.ToUpperInvariant());
                }
                else
                {
                    This.SetTitle(text);
                }

            })
                      .DisposeWith(disp);
        }

        public static void SetText(this UITextField This, IObservable<string> observable, CompositeDisposable disp)
        {
            observable.Subscribe(text => This.Text = text)
                      .DisposeWith(disp);
        }

        public static void SetText(this UITextView This, IObservable<string> observable, CompositeDisposable disp)
        {
            observable.Subscribe(text => This.Text = text)
                      .DisposeWith(disp);
        }

        public static void SetPlaceholder(this UITextField This, IObservable<string> observable, CompositeDisposable disp, UIColor color = null)
        {
            observable.Subscribe(text =>
            {
                if (color != null)
                    This.SetPlaceholderColor(text, color);
                else
                    This.Placeholder = text;
            }).DisposeWith(disp);
        }
    }
}