//
// RxTableViewSource.cs
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
using Foundation;
using ReactiveUI;
using UIKit;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace Rx.iOS.RxViews
{
    public class RxDynamicSizeTableViewSource<T> : ReactiveTableViewSource<T>
    {
        public RxDynamicSizeTableViewSource(UITableView tableView)
            : base(tableView)
        {

        }

        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return UITableView.AutomaticDimension;
        }
    }

    public static class RxTableViewSourceExtensions
    {
        /// <summary>
        /// <para>Extension method that binds an observable of a list of table
        /// sections as the source of a <see cref="UITableView"/>.</para>
        /// <para>If your <see cref="IReadOnlyList"/> is also an instance of
        /// <see cref="IReactiveNotifyCollectionChanged"/>, then this method
        /// will silently update the bindings whenever it changes as well.
        /// Otherwise, it will just log a message.</para>
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sectionsObservable">Sections observable.</param>
        /// <param name="tableView">Table view.</param>      
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReadOnlyList<TableSectionInformation<TSource, TCell>>> sectionsObservable,
            UITableView tableView,
            ReactiveTableViewSource<TSource> source)
            where TCell : UITableViewCell
        {
            var bind = sectionsObservable.BindTo(source, x => x.Data);
            tableView.Source = source;
            return new CompositeDisposable(bind, source);
        }

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UITableView"/>.
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <param name="sizeHint">Size hint.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>       
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UITableView tableView,
            NSString cellKey,
            float sizeHint,
            ReactiveTableViewSource<TSource> source,
            Action<TCell> initializeCellAction = null)
            where TCell : UITableViewCell
        {
            return sourceObservable
                .Select(src => new[] {
                    new TableSectionInformation<TSource, TCell>(
                        src,
                        cellKey,
                        sizeHint,
                        initializeCellAction),
                })
                .BindTo(tableView, source);
        }
        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UITableView"/>.
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <param name="sizeHint">Size hint.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>       
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindToDynamicSize<TSource, TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UITableView tableView,
            NSString cellKey,
            float sizeHint,
            Action<TCell> initializeCellAction = null, Func<ReactiveTableViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UITableViewCell
        {
            var source = new RxDynamicSizeTableViewSource<TSource>(tableView);

            var sourceObservableDisposable = sourceObservable
                .Select(src => new[] {
                    new TableSectionInformation<TSource, TCell>(
                        src,
                        cellKey,
                        sizeHint,
                        initializeCellAction),
                })
                .BindTo(tableView, source);

            var sourceDisposable = initSource?.Invoke(source) ?? Disposable.Empty;

            return new CompositeDisposable(sourceObservableDisposable, sourceDisposable);
        }

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UITableView"/>.  Also registers
        /// the given class with an unspecified cellKey (you should probably
        /// not specify any other cellKeys).
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="sizeHint">Size hint.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>       
        /// <typeparam name="TCell">Type of the <see cref="UITableViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UITableView tableView,
            float sizeHint,
            ReactiveTableViewSource<TSource> source,
            Action<TCell> initializeCellAction = null)
            where TCell : UITableViewCell
        {
            var type = typeof(TCell);
            var cellKey = new NSString(type.ToString());
            tableView.RegisterClassForCellReuse(type, new NSString(cellKey));

            return sourceObservable
                .BindTo(tableView, cellKey, sizeHint, source, initializeCellAction);
        }
    }
}