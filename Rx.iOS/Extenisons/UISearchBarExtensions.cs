//
// UISearchBarExtensions.cs
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
using System.Reactive.Linq;
using UIKit;

namespace Rx.Extensions
{
    public static class UISearchBarExtensions
    {
        public static IObservable<UISearchBarTextChangedEventArgs> WhenTextChange(this UISearchBar This)
        {
            return Observable.FromEventPattern<UISearchBarTextChangedEventArgs>(
                e => This.TextChanged += e, e => This.TextChanged -= e)
                             .Select(args => args.EventArgs);
        }

        public static IDisposable WhenTextChange(this UISearchBar searchBar, Action<string> action)
        {
            return searchBar.WhenTextChange()
                            .Subscribe(e => action?.Invoke(e.SearchText));
        }

        public static IObservable<UISearchBarTextChangedEventArgs> WhenSearch(this UISearchBar This, bool dismissKeyboard = true)
        {
            return Observable.FromEventPattern(e => This.SearchButtonClicked += e, e => This.SearchButtonClicked -= e)
                             .Select(_ =>
            {
                if (dismissKeyboard)
                    This.ResignFirstResponder();
                return new UISearchBarTextChangedEventArgs(This.Text);
            });
        }

        public static IObservable<UISearchBarTextChangedEventArgs> WhenTextChangeOrSearch(this UISearchBar This, bool dismissKeyboard = true)
        {
            return This.WhenTextChange()
                       .Merge(This.WhenSearch(dismissKeyboard))
                       .DistinctUntilChanged(_=>_.SearchText);
        }
    }
}