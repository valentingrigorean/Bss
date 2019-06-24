//
// SupportSearchViewExtensionsV7.cs
//
// Author:
//       valen <valentin.grigorean1@gmail.com>
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
using Android.Support.V7.Widget;

namespace Rx.Extensions
{



    public static class SupportSearchViewExtensionsV7
    {
        /// <summary>
        /// TextSubmit + TextChange
        /// </summary>
        /// <returns>The searched.</returns>
        /// <param name="This">This.</param>
        public static IObservable<string> WhenTextChangeOrSearch(this SearchView This)
        {
            return This.WhenQueryTextChange()
                       .Select(e => e.NewText)
                       .Merge(This.WhenQueryTextSubmit()
                                  .Select(e => e.Query))
                       .DistinctUntilChanged();
        }

        public static IObservable<SearchView.QueryTextSubmitEventArgs> WhenQueryTextSubmit(this SearchView This)
        {
            return Observable.FromEventPattern<SearchView.QueryTextSubmitEventArgs>(
                e => This.QueryTextSubmit += e, e => This.QueryTextSubmit -= e)
                             .Select(e => e.EventArgs);
        }

        public static IDisposable WhenQueryTextSubmit(this SearchView This, Action<string> action)
        {
            return This.WhenQueryTextSubmit().
                       Subscribe(e => action?.Invoke(e.Query));
        }

        public static IObservable<SearchView.QueryTextChangeEventArgs> WhenQueryTextChange(this SearchView This)
        {
            return Observable.FromEventPattern<SearchView.QueryTextChangeEventArgs>(
                e => This.QueryTextChange += e, e => This.QueryTextChange -= e)
                             .Select(e => e.EventArgs);
        }

        public static IDisposable WhenQueryTextChange(this SearchView This, Action<string> action)
        {
            return This.WhenQueryTextChange().
                       Subscribe(e => action?.Invoke(e.NewText));
        }
    }
}
