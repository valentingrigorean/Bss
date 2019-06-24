//
// BaseViewSource.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 valentingrigorean
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
using Foundation;
using Bss.iOS.UIKit.Collections;
using Bss.iOS.Extensions;

namespace Bss.iOS.UIKit
{
    [Serializable]
    public sealed class RowClickedEventArgs<T> : EventArgs
    {
        public RowClickedEventArgs(NSIndexPath position, UIView cell, T model)
        {
            Model = model;
            IndexPath = position;
            Cell = cell;
        }

        public int Position => IndexPath.Row;
        public T Model { get; private set; }
        public UIView Cell { get; private set; }
        public NSIndexPath IndexPath { get; private set; }
    }

    public delegate void RowClickedEventHandler<T>(object sender, RowClickedEventArgs<T> e);

    [Serializable]
    public sealed class DisplayCellEventArgs : EventArgs
    {
        public DisplayCellEventArgs(UITableView tableView, UITableViewCell cell, NSIndexPath index)
        {
            TableView = tableView;
            Cell = cell;
            IndexPath = index;
        }
        public UITableView TableView { get; }
        public UITableViewCell Cell { get; }
        public NSIndexPath IndexPath { get; }
    }

    public delegate void DisplayCellEventHandler(object sender, DisplayCellEventArgs e);

    public abstract class ViewSource<T> : NSObject, IUITableViewDelegate, IUITableViewDataSource, IDataSource<T>
    {
        private readonly IDataSource<T> _dataSource;
        protected UITableView TableView { get; }

        protected ViewSource(IList<T> dataSource)
        {
            _dataSource = new InternalDataSource<T>(dataSource);
            _dataSource.DataSetChanged += (sender, e) => ReloadData(e.Indexs, e.Type);
        }

        protected ViewSource(IList<T> dataSource, UITableView tableView) : this(dataSource)
        {
            TableView = tableView;
        }

        public bool AllowHighlighted { get; set; } = true;

        public delegate nfloat HeightForRowCallback(UITableView tableView, NSIndexPath position);

        public int Count => _dataSource.Count;

        public IList<T> Items => _dataSource.Items;

        #region Events

        public event EventHandler<DataSetChangeEventArgs> DataSetChanged;

        public event EventHandler ViewDidScroll;

        public event RowClickedEventHandler<T> RowClicked;

        public event DisplayCellEventHandler DisplayCell;

        public event DisplayCellEventHandler DisplayCellEnded;

        #endregion

        #region IDataSource
        /// <summary>
        /// Add the specified item.
        /// Will not work if you dont initialiaze with tableView
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void Add(T item)
        {
            _dataSource.Add(item);
        }

        public virtual void AddRange(IEnumerable<T> col)
        {
            _dataSource.AddRange(col);
        }

        /// <summary>
        /// Remove the specified item.
        /// Will not work if you dont initialiaze with tableView
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void Remove(T item)
        {
            _dataSource.Remove(item);
        }

        public T GetItem(int pos)
        {
            return _dataSource.GetItem(pos);
        }

        public virtual int IndexOf(T item)
        {
            return _dataSource.IndexOf(item);
        }

        /// <summary>
        /// Filters the by.
        /// Will not work if you dont initialiaze with tableView
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="autoReset">If set to <c>true</c> auto reset.</param>
        public virtual void FilterBy(Func<T, bool> predicate, bool autoReset = true)
        {
            _dataSource.FilterBy(predicate, autoReset);
        }

        /// <summary>
        /// Clears the filter.
        /// Will not work if you dont initialiaze with tableView
        /// </summary>
        public virtual void ClearFilter()
        {
            _dataSource.ClearFilter();
        }

        public virtual void UpdateDataSource(IList<T> newDataSource)
        {
            _dataSource.UpdateDataSource(newDataSource);
        }

        public void ReloadData()
        {
            ReloadData(null, TransitionType.Add);
        }

        public void ReloadData(int[] indexcs, TransitionType type)
        {
            CheckIfHaveTable();
            DataSetChanged?.Invoke(this, new DataSetChangeEventArgs(indexcs, type));
            if (indexcs != null)
            {
                var rows = indexcs.ConvertToRows();
                TableView.BeginUpdates();
                if (type == TransitionType.Add)
                    TableView.InsertRows(rows, UITableViewRowAnimation.Automatic);
                else
                    TableView.DeleteRows(rows, UITableViewRowAnimation.Automatic);
                TableView.EndUpdates();
            }
            else
                TableView.ReloadData();

        }
        #endregion

        [Export("numberOfSectionsInTableView:")]
        public virtual nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public virtual nint RowsInSection(UITableView tableview, nint section)
        {
            return Count;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public virtual void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView.AllowsSelection)
            {
                tableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);
                var cell = tableView.CellAt(indexPath);
                var model = _dataSource.GetItem(indexPath.Row);
                RowClicked?.Invoke(this, new RowClickedEventArgs<T>(indexPath, cell, model));
            }
        }

        [Export("tableView:didDeselectRowAtIndexPath:")]
        public virtual void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView.AllowsSelection)
                tableView.DeselectRow(indexPath, true);
        }

        [Export("tableView:didHighlightRowAtIndexPath:")]
        public virtual void RowHighlighted(UITableView tableView, NSIndexPath rowIndexPath)
        {
            if (!AllowHighlighted) return;
            var cell = tableView.CellAt(rowIndexPath);
            cell.SetHighlighted(true, true);
        }

        [Export("tableView:didUnhighlightRowAtIndexPath:")]
        public virtual void RowUnhighlighted(UITableView tableView, NSIndexPath rowIndexPath)
        {
            if (!AllowHighlighted) return;
            var cell = tableView.CellAt(rowIndexPath);
            cell.SetHighlighted(false, true);
        }

        [Export("tableView:willDisplayCell:forRowAtIndexPath:")]
        public virtual void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            DisplayCell?.Invoke(this, new DisplayCellEventArgs(tableView, cell, indexPath));
        }

        [Export("tableView:didEndDisplayingCell:forRowAtIndexPath:")]
        public virtual void CellDisplayingEnded(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            DisplayCellEnded?.Invoke(this, new DisplayCellEventArgs(tableView, cell, indexPath));
        }

        [Export("scrollViewDidScroll:")]
        public virtual void Scrolled(UIScrollView scrollView)
        {
            ViewDidScroll?.Invoke(this, EventArgs.Empty);
        }

        [Export("tableView:shouldHighlightRowAtIndexPath:")]
        public bool ShouldHighlightRow(UITableView tableView, NSIndexPath rowIndexPath)
        {
            return AllowHighlighted;
        }
        #region virtual funct

        #endregion

        private void CheckIfHaveTable()
        {
            if (TableView == null)
                throw new Exception("You need to call constructor with " +
                    "const(list,tableView)");
        }

        public void Clear()
        {
            _dataSource.Clear();
        }

        public abstract UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath);
    }
}