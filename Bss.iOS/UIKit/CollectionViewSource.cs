//
// CollectionViewSource.cs
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
using UIKit;
using System.Collections.Generic;
using Bss.iOS.UIKit.Collections;
using Bss.iOS.Extensions;

namespace Bss.iOS.UIKit
{
    public abstract class CollectionViewSource<T> : UICollectionViewSource, IDataSource<T>
    {
        private readonly IDataSource<T> _dataSource;

        protected UICollectionView CollectionView { get; }

        protected CollectionViewSource(IList<T> dataSource)
        {
            _dataSource = new InternalDataSource<T>(dataSource);
            _dataSource.DataSetChanged += (sender, e) => ReloadData(e.Indexs, e.Type);
        }

        protected CollectionViewSource(IList<T> dataSource, UICollectionView collectionView) : this(dataSource)
        {
            CollectionView = collectionView;
            CollectionView.Source = this;
        }

        public int Count => _dataSource.Count;

        public event RowClickedEventHandler<T> ItemClicked;

        public Func<UICollectionViewCell, int, bool> CanFocusItemCallBack { get; set; } = (arg, index) => true;

		public IList<T> Items => _dataSource.Items;

		public event EventHandler<DataSetChangeEventArgs> DataSetChanged;

        public void Add(T item)
        {
            _dataSource.Add(item);
        }

        public void AddRange(IEnumerable<T> col)
        {
            _dataSource.AddRange(col);
        }

        public void ClearFilter()
        {
            _dataSource.ClearFilter();
        }

        public void FilterBy(Func<T, bool> predicate, bool autoReset = true)
        {
            _dataSource.FilterBy(predicate, autoReset);
        }

        public T GetItem(int pos)
        {
            return _dataSource.GetItem(pos);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Count;
        }

        public int IndexOf(T item)
        {
            return _dataSource.IndexOf(item);
        }

        public void ReloadData()
        {
            ReloadData(null, TransitionType.Add);
        }


        public void ReloadData(int[] indexcs, TransitionType type)
        {
            DataSetChanged?.Invoke(this, new DataSetChangeEventArgs(indexcs, type));
            CheckIfHaveTable();
            if (indexcs != null)
            {
                var items = indexcs.ConvertToRows();
                CollectionView.PerformBatchUpdates(() =>
                {
                    if (type == TransitionType.Add)
                        CollectionView.InsertItems(items);
                    else
                        CollectionView.DeleteItems(items);
                }, null);
            }
            else
                CollectionView.ReloadData();
        }

        public void Remove(T item)
        {
            _dataSource.Remove(item);
        }

        public void UpdateDataSource(IList<T> newDataSource)
        {
            _dataSource.UpdateDataSource(newDataSource);
        }

        private void CheckIfHaveTable()
        {
            if (CollectionView == null)
                throw new Exception("You need to call constructor with " +
                    "const(list,collectionView)");
        }

        public void Clear()
        {
            _dataSource.Clear();
        }

        public override void ItemSelected(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            ItemClicked?.Invoke(this, new RowClickedEventArgs<T>(indexPath, cell, GetItem(indexPath.Row)));
            collectionView.SelectItem(indexPath, true, UICollectionViewScrollPosition.None);
        }

        public override bool CanFocusItem(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
        {
            return CanFocusItemCallBack(collectionView.CellForItem(indexPath), indexPath.Row);
        }

    }
}
