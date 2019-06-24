using System;
using System.Collections.Generic;
using System.Linq;

namespace Bss.iOS.UIKit.Collections
{
    internal class InternalDataSource<T> : IDataSource<T>
    {
        private IList<T> _originalList;
        private IList<T> DataSource { get; set; }


        public InternalDataSource(IList<T> dataSource)
        {
            DataSource = _originalList = dataSource;
        }

        public IList<T> Items => _originalList;

        public event EventHandler<DataSetChangeEventArgs> DataSetChanged;

        public int Count => DataSource.Count;

        public void Add(T item)
        {
            var index = DataSource.Count;
            DataSource.Add(item);
            ReloadData(new[] { index }, TransitionType.Add);
        }

        public void AddRange(IEnumerable<T> col)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));
            var haveElements = false;
            var list = new List<int>();
            foreach (var item in col)
            {
                haveElements = true;
                list.Add(DataSource.Count);
                DataSource.Add(item);
            }
            if (haveElements)
                ReloadData(list.ToArray(), TransitionType.Add);
        }

        public void Remove(T item)
        {
            var indexOf = DataSource.IndexOf(item);
            if (indexOf < 0)
                return;
            DataSource.Remove(item);
            var arr = indexOf >= 0 ? new[] { indexOf } : null;
            ReloadData(arr, TransitionType.Remove);
        }

        public int IndexOf(T item)
        {
            return DataSource.IndexOf(item);
        }

        public T GetItem(int pos)
        {
            return DataSource[pos];
        }


        /// <summary>
        /// Filters the by.
        /// Will not work if you dont initialiaze with tableView
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="autoReset">If set to <c>true</c> auto reset.</param>
        public void FilterBy(Func<T, bool> predicate, bool autoReset = true)
        {
            var flag = DataSource.Count == _originalList.Count;
            if (autoReset)
                DataSource = _originalList;
            var temp = DataSource.Where(predicate).ToList();
            if (temp.Count == _originalList.Count && flag)
                return;
            DataSource = temp;
            ReloadData();
        }


        public void ClearFilter()
        {
            if (_originalList == null)
                return;
            if (DataSource.Count == _originalList.Count)
                return;
            DataSource = _originalList;
            ReloadData();
        }


        public void Clear()
        {
            DataSource.Clear();
            ReloadData();
        }

        public virtual void UpdateDataSource(IList<T> newDataSource)
        {
            _originalList = DataSource = newDataSource;
            ReloadData();
        }

        public void ReloadData()
        {
            ReloadData(null, TransitionType.Add);
        }

        public void ReloadData(int[] indexs, TransitionType type)
        {
            Application.InvokeOnMainThread(() => DataSetChanged?.Invoke(
                this, new DataSetChangeEventArgs(indexs, type)));
        }
    }
}
