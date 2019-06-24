//
// RecyclerViewExtensions.cs
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
using Android.Support.V7.Widget;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Rx.Droid.Extensions
{
    public static class RecyclerViewExtensions
    {

        public static IObservable<Unit> WhenScrolled(this RecyclerView This)
        {
            return Observable.Create<Unit>(obser =>
            {
                var listener = new RecyclerViewOnScrollListener(obser);
                This.AddOnScrollListener(listener);
                return Disposable.Create(() => This.RemoveOnScrollListener(listener));
            });
        }

        private class RecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            private readonly IObserver<Unit> _scrolled;

            public RecyclerViewOnScrollListener(IObserver<Unit> scrolled)
            {
                _scrolled = scrolled;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);
                _scrolled.OnNext(Unit.Default);
            }
        }
    }
}
