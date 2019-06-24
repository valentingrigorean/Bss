//
// ViewSourceFactory.cs
//
// Author:
//       Valentin Grigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 
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
using System.Collections.Generic;
using Foundation;
using UIKit;
using Bss.iOS.UIKit.Pager;

namespace Bss.iOS.UIKit
{
    public interface ITableViewSourceDelegate
    {
        void OnCreate(UITableViewCell cell, UITableView tableView);
    }

    public interface ICollectionViewSourceDelegate
    {
        void OnCreate(UICollectionReusableView cell, UICollectionView collectionView);
    }

    public static class ViewSourceFactory
    {
        public static PageImageDataSource CreateForGalleryImage(IList<string> urls,
                                                            UIViewContentMode contentMode = UIViewContentMode.ScaleAspectFit)
        {
            return new PageImageDataSource(urls, contentMode);
        }

        public static ViewSource<T> CreateForTable<T>(string reuseIdentifier,
                                                      ITableViewSourceDelegate del = null)
        {
            return CreateForTable(reuseIdentifier, new List<T>(), null, del);
        }

        public static ViewSource<T> CreateForTable<T>(string reuseIdentifier, IList<T> dataSource,
                                                      ITableViewSourceDelegate del = null)
        {
            return CreateForTable(reuseIdentifier, dataSource, null, del);
        }

        public static ViewSource<T> CreateForTable<T>(string reuseIdentifier, UITableView tableView,
                                                      ITableViewSourceDelegate del = null)
        {
            return new ViewSourceInternal<T>(reuseIdentifier, new List<T>(), tableView, del);
        }

        /// <summary>
        /// Creates ViewSource
        /// </summary>
        /// <returns>The from.</returns>
        /// <param name="reuseIdentifier">Reuse identifier.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="tableView">Table view is need if you more functions like add/remove elements to auto reload data.</param>
        /// <param name="del"></param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static ViewSource<T> CreateForTable<T>(string reuseIdentifier, IList<T> dataSource,
                                                      UITableView tableView, ITableViewSourceDelegate del = null)
        {
            return new ViewSourceInternal<T>(reuseIdentifier, dataSource, tableView, del);
        }

        public static CollectionViewSource<T> CreateForCollection<T>(string reuseIdentifier, ICollectionViewSourceDelegate del = null)
        {
            return CreateForCollection(reuseIdentifier, new List<T>(), null, del);
        }


        public static CollectionViewSource<T> CreateForCollection<T>(string reuseIdentifier, UICollectionView collectionView,
                                                                     ICollectionViewSourceDelegate del = null)
        {
            return CreateForCollection(reuseIdentifier, new List<T>(), collectionView, del);
        }


        public static CollectionViewSource<T> CreateForCollection<T>(string reuseIdentifier, IList<T> dataSource,
                                                                     ICollectionViewSourceDelegate del = null)
        {
            return CreateForCollection(reuseIdentifier, dataSource, null, del);
        }

        public static CollectionViewSource<T> CreateForCollection<T>(string reuseIdentifier, IList<T> dataSource,
                                                                     UICollectionView collectionView,
                                                                     ICollectionViewSourceDelegate del = null)
        {
            return new CollectionViewSourceInternal<T>(reuseIdentifier, dataSource, collectionView, del);
        }

        private class CollectionViewSourceInternal<T> : CollectionViewSource<T>
        {
            private readonly string _reuseIdentifier;
            private readonly ICollectionViewSourceDelegate _del;

            public CollectionViewSourceInternal(string reuseIdentifier, IList<T> dataSource,
                                                UICollectionView collectionView, ICollectionViewSourceDelegate del)
                : base(dataSource, collectionView)
            {
                _reuseIdentifier = reuseIdentifier;
                _del = del;
            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                var cell = collectionView.DequeueReusableCell(_reuseIdentifier, indexPath);
                if (cell == null)
                    throw new Exception("Invalid reuseIdentifier");
                var rCell = cell as IReusableView<T>;
                if (rCell == null)
                    throw new Exception("Your cell most implement IReusableView<T> or " +
                                        "derive from CollectionViewCell");
                _del?.OnCreate(cell, collectionView);
                rCell.SetModel((int)indexPath.Item, GetItem((int)indexPath.Item));
                return (UICollectionViewCell)cell;
            }
        }

        private class ViewSourceInternal<T> : ViewSource<T>
        {
            private readonly string _reuseIdentifier;
            private readonly ITableViewSourceDelegate _del;

            public ViewSourceInternal(string reuseIdentifier, IList<T> dataSource, UITableView tableView,
                                      ITableViewSourceDelegate del)
                : base(dataSource, tableView)
            {
                _reuseIdentifier = reuseIdentifier;
                _del = del;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(_reuseIdentifier);
                if (cell == null)
                    throw new Exception("Invalid reuseIdentifier");
                var rCell = cell as IReusableView<T>;
                if (rCell == null)
                    throw new Exception("Your cell most implement IReusableView<T> or " +
                                        "derive from TableViewCell");
                _del?.OnCreate(cell, tableView);
                rCell.SetModel(indexPath.Row, GetItem(indexPath.Row));
                return cell;
            }
        }
    }
}
