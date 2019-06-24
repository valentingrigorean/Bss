//
// Collection.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Bss.Core.Collections
{
    public delegate void ItemRemovedEventHandler<T>(object sender, ItemChangeEventArgs<T> e);
    public delegate void ItemAddedEventHandler<T>(object sender, ItemChangeEventArgs<T> e);

    public enum ItemFlag
    {
        /// <summary>
        /// A single item was added
        /// </summary>
        Single,

        /// <summary>
        /// Added one or more items
        /// </summary>
        Range
    }

    public sealed class ItemChangeEventArgs<T> : EventArgs
    {


        public ItemChangeEventArgs(T item)
        {
            Flag = ItemFlag.Single;
            Item = item;
        }

        public ItemChangeEventArgs(IList<T> items)
        {
            Flag = ItemFlag.Range;
            Range = items;
        }

        public ItemFlag Flag { get; private set; }

        public T Item { get; private set; }

        public IList<T> Range { get; private set; }
    }

    public class Collection<T> : IList<T>
    {
        public Collection()
        {
            Container = new List<T>();
        }

        public Collection(IList<T> collection)
        {
            Container = collection;
            if (collection == null)
                Container = new List<T>();
        }

        public Collection(ICollection<T> collection)
        {
            Container = new List<T>(collection);
            if (collection == null)
                Container = new List<T>();
        }

        public bool EmitEvents { get; set; } = true;

        public virtual event ItemRemovedEventHandler<T> ItemRemoved;
        public virtual event ItemAddedEventHandler<T> ItemAdded;

        public int Count => Container.Count;

        protected IList<T> Container { get; set; }

        public virtual void AddRange(IList<T> items)
        {
            foreach (var item in items)
                Container.Add(item);
            EmitItemAddRange(items);
        }

        public virtual void Add(T item)
        {
            if (Container.Contains(item)) return;
            Container.Add(item);
            EmitItemAdd(item);
        }

        public virtual bool Remove(T item)
        {
            var flag = Container.Remove(item);

            EmitItemRemove(item);
            return flag;
        }

        public virtual bool Contains(T item)
        {
            return Container.Contains(item);
        }

        public virtual void Clear()
        {
            EmitItemRemove(this);
            Container.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Container.Count)
                throw new InvalidOperationException("Array is too small");
            Container.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return Container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Container.GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Container.Count)
                    throw new IndexOutOfRangeException();
                return Container[index];
            }
            set
            {
                if (index < 0 || index > Container.Count)
                    throw new IndexOutOfRangeException();
                Container[index] = value;
            }
        }

        public int IndexOf(T item)
        {
            return Container.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException();
            var item = this[index];
            Container.RemoveAt(index);
            EmitItemRemove(item);
        }

        #region ICloneable implementation

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        protected virtual void EmitItemAdd(T item)
        {
            if (!EmitEvents)
                return;
            ItemAdded?.Invoke(this, new ItemChangeEventArgs<T>(item));
        }

        protected virtual void EmitItemAddRange(IList<T> items)
        {
            if (!EmitEvents)
                return;
            ItemAdded?.Invoke(this, new ItemChangeEventArgs<T>(items));
        }

        protected virtual void EmitItemRemove(T item)
        {
            if (!EmitEvents)
                return;
            ItemRemoved?.Invoke(this, new ItemChangeEventArgs<T>(item));
        }


        protected virtual void EmitItemRemove(IList<T> items)
        {
            if (!EmitEvents)
                return;
            ItemRemoved?.Invoke(this, new ItemChangeEventArgs<T>(items));
        }

        public override string ToString()
        {
            return $"[Collection: EmitEvents={EmitEvents}, Count={Count}, IsReadOnly={IsReadOnly}]";
        }
    }
}
