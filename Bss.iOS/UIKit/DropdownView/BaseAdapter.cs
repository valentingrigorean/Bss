//
// BaseAdapter.cs
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

namespace Bss.iOS.UIKit.DropdownView
{
    public abstract class BaseAdapter<T> : IAdapter, IDataSource<T>
    {
        private InternalDataSource<T> _dataSource;

     

        protected BaseAdapter() : this(new List<T>())
        {

        }

        protected BaseAdapter(IList<T> dataSource)
        {
            _dataSource = new InternalDataSource<T>(dataSource);
            _dataSource.DataSetChanged += (sender, e) =>
                NotifyDataSetChanged?.Invoke(this, EventArgs.Empty);
        }


		public IList<T> Items => _dataSource.Items;

		public int Count => _dataSource.Count;    

        public event EventHandler<DataSetChangeEventArgs> DataSetChanged;
        public event EventHandler NotifyDataSetChanged;

        public abstract UITableViewCell GetView(int position, UITableView tableView);

        public void Add(T item)
        {
            _dataSource.Add(item);
        }

        public void AddRange(IEnumerable<T> col)
        {
            _dataSource.AddRange(col);
        }

        public void Remove(T item)
        {
            _dataSource.Remove(item);
        }

        public void UpdateDataSource(IList<T> newDataSource)
        {
            _dataSource.UpdateDataSource(newDataSource);
        }

        public void Clear()
        {
            _dataSource.Clear();
        }

        public T GetItem(int pos)
        {
            return _dataSource.GetItem(pos);
        }

        public int IndexOf(T item)
        {
            return _dataSource.IndexOf(item);
        }

        public void FilterBy(Func<T, bool> predicate, bool autoReset = true)
        {
            _dataSource.FilterBy(predicate, autoReset);
        }

        public void ClearFilter()
        {
            _dataSource.ClearFilter();
        }

        public void ReloadData()
        {
            _dataSource.ReloadData();
        }

        public void ReloadData(int[] indexcs, TransitionType type)
        {
            _dataSource.ReloadData(indexcs, type);
        }

        object IAdapter.GetItem(int position)
        {
            return GetItem(position);
        }

        protected void NotifyDataChange()
        {
            InvokeOnMainThread(() => NotifyDataSetChanged?.Invoke(this, EventArgs.Empty));
        }

        private void InvokeOnMainThread(Action action)
        {
            Application.InvokeOnMainThread(action);
        }
    }
}