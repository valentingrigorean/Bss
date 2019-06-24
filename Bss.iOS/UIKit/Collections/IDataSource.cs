using System;
using System.Collections.Generic;

namespace Bss.iOS.UIKit.Collections
{
    public enum TransitionType
    {
        Add,
        Remove
    }

    public class DataSetChangeEventArgs : EventArgs
    {
        public DataSetChangeEventArgs(int[] indexs, TransitionType type)
        {
            Indexs = indexs;
            Type = type;
        }
        public TransitionType Type { get; }
        public int[] Indexs { get; }
    }

    public interface IDataSource<T>
    {
        /// <summary>
        /// Occurs when data set changed.
        /// </summary>
        event EventHandler<DataSetChangeEventArgs> DataSetChanged;

		IList<T> Items { get; }

        int Count { get; }

        /// <summary>
        /// Add the specified item.
        /// Will reloadData
        /// </summary>
        /// <param name="item">Item.</param>      
        void Add(T item);

        /// <summary>
        /// Adds the range.
        /// Will reloadData
        /// </summary>
        /// <param name="col">Col.</param>
        void AddRange(IEnumerable<T> col);

        /// <summary>
        /// Remove the specified item.
        /// Will reloadData
        /// </summary>
        /// <param name="item">Item.</param>
        void Remove(T item);

        /// <summary>
        /// Updates the data source.
        /// Will reloadData
        /// </summary>
        /// <param name="newDataSource">New data source.</param>
        void UpdateDataSource(IList<T> newDataSource);

        void Clear();

        T GetItem(int pos);

        int IndexOf(T item);

        /// <summary>
        /// Filters the by.
        /// Will reloadData
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <param name="autoReset">If set to <c>true</c> auto reset.</param>
        void FilterBy(Func<T, bool> predicate, bool autoReset = true);

        /// <summary>
        /// Clears the filter.
        /// Will reloadData
        /// </summary>
        void ClearFilter();

        void ReloadData();

        void ReloadData(int[] indexcs, TransitionType type);
    }
}
