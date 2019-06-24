//
// PageSourceExtensions.cs
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
using Bss.iOS.UIKit;
using ReactiveUI;
using UIKit;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Reactive.Linq;
namespace Rx.iOS.Extenisons
{
    public interface IRxPageItem<T> : IPageItem, IViewFor<T>
        where T : class
    {

    }

    public static class PageSourceExtensions
    {
        public static IObservable<PageViewSource.PageChangedEventArgs> WhenPageChanged(this PageViewSource This)
        {
            return Observable.FromEventPattern<PageViewSource.PageChangedEventArgs>(
                e => This.PageChanged += e, e => This.PageChanged -= e)
                             .Select(args => args.EventArgs);
        }

		public static IObservable<int> WhenCountChanged(this PageViewSource This)
		{
			return Observable.FromEventPattern<int>(
                e => This.CountChange += e, e => This.CountChange -= e)
							 .Select(args => args.EventArgs);
		}

        public static IDisposable BindTo<TSource, TPage>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable, UIPageViewController pageViewController,
            Func<IRxPageItem<TSource>> pageFactory,
            Func<PageViewSource, IDisposable> initSource = null)
            where TSource : class
            where TPage : IRxPageItem<TSource>
        {
            var disp = new CompositeDisposable();

            sourceObservable.Subscribe(items =>
            {
                var list = items as IList<TSource>;

                var vcs = new UIViewController[list.Count];

                for (var i = 0; i < list.Count; i++)
                {
                    vcs[i] = CreatePageFor<TSource, TPage>(pageFactory, i, list[i]);
                }

                var source = new PageViewSource(vcs);

                pageViewController.SetSource(source);

                items.ItemsAdded
                     .Subscribe(item =>
                     {
                         source.Add(CreatePageFor<TSource, TPage>(pageFactory, source.Count, item));
                         if (source.CurrentController == null && source.Count > 0)
                             pageViewController.SetViewControllers(new[] { source.DataSource[0] }, UIPageViewControllerNavigationDirection.Forward, false, null);
                     })
                     .DisposeWith(disp);

                var sourceDisp = initSource?.Invoke(source);
                if (sourceDisp != null)
                    disp.Add(disp);
            }).DisposeWith(disp);

            return disp;
        }

        private static UIViewController CreatePageFor<TSource, TPage>(Func<IRxPageItem<TSource>> pageFactory, int index, TSource item)
            where TSource : class
            where TPage : IRxPageItem<TSource>
        {
            var vc = pageFactory();
            vc.Index = index;
            vc.ViewModel = item;
            return vc as UIViewController;
        }
    }
}