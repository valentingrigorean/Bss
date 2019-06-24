//
// RBaseAdapter.cs
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
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Views;
using System;
using Bss.Droid.Extensions;
using System.Linq;
using Bss.Droid.Anim.Views;

namespace Bss.Droid.Widgets
{

    public class StaticViewHolder : RecyclerView.ViewHolder
    {
        public StaticViewHolder(View view) : base(view)
        {

        }
    }


    public abstract class RVBaseAdapter<T, VH> : RecyclerView.Adapter
         where VH : RecyclerView.ViewHolder
    {
        private IList<T> _originalList;
        private IList<T> _currentList;

        protected IList<T> DataSource => _currentList;


        protected RVBaseAdapter() : this(new List<T>())
        {

        }

        protected RVBaseAdapter(IList<T> dataSource)
        {
            _originalList = dataSource;
            _currentList = new List<T>(dataSource);
        }

        /// <summary>
        /// Gets or set if would use as background attr?selectableItemBackground
        /// </summary>
        /// <value><c>true</c> if default selector; otherwise, <c>false</c>.</value>
        public bool DefaultSelector { get; set; } = false;

        public T this[int index] => _currentList[index];

        public IReadOnlyList<T> Items => _currentList?.ToArray() ?? new T[0];

        public class ItemClickEventArgs : EventArgs
        {
            public ItemClickEventArgs(int pos, T item, VH viewHolder)
            {
                Position = pos;
                Item = item;
                ViewHolder = viewHolder;
            }

            public VH ViewHolder { get; }
            public T Item { get; }
            public int Position { get; }
        }

        public event EventHandler<ItemClickEventArgs> ItemClick;

        public event EventHandler CollectionChange;

        public IClickableView Delegate { get; set; } = new DefaultTouchEffect();

        public override int ItemCount => DataSource.Count;

        public void Add(T item)
        {
            DataSource.Add(item);
            NotifyItemInserted(DataSource.Count - 1);
            InvokeCollectionChange();
        }

        public void Add(T item, int position)
        {
            DataSource.Insert(position, item);
            NotifyItemInserted(position);
            InvokeCollectionChange();
        }

        public void AddRange(IEnumerable<T> items)
        {
            var count = DataSource.Count;
            DataSource.AddRange(items);
            NotifyItemRangeInserted(count == 0 ? 0 : count - 1, DataSource.Count - count);
            InvokeCollectionChange();
        }

        public void Remove(T item)
        {
            var index = DataSource.IndexOf(item);
            if (index < 0)
                return;
            DataSource.RemoveAt(index);
            NotifyItemRemoved(index);
            InvokeCollectionChange();
        }

        public void Reload(T item)
        {
            var index = DataSource.IndexOf(item);
            if (index < 0)
                return;
            NotifyItemChanged(index);
            InvokeCollectionChange();
        }

        public void Reload(IList<T> items)
        {
            var list = new List<int>();
            foreach (var item in items)
            {
                var index = DataSource.IndexOf(item);
                if (index < 0)
                    continue;
                list.Add(index);
            }
            if (list.Count == 0)
                return;
            //TODO update in bulk where you can
            foreach (var item in list)
                NotifyItemChanged(item);
            InvokeCollectionChange();
        }

        public void ClearFilter()
        {
            if (_originalList == null)
                return;
            if (_currentList.Count == _originalList.Count)
                return;
            _currentList = _originalList;
            NotifyDataSetChanged();
            InvokeCollectionChange();
        }

        public void FilterBy(Func<T, bool> predicate, bool autoReset = true)
        {
            var flag = _currentList.Count == _originalList.Count;
            if (autoReset)
                _currentList = _originalList;
            var temp = _currentList.Where(predicate).ToList();
            if (temp.Count == _originalList.Count && flag)
                return;
            _currentList = temp;
            NotifyDataSetChanged();
            InvokeCollectionChange();
        }

        public void Clear()
        {
            DataSource.Clear();
            NotifyDataSetChanged();
            InvokeCollectionChange();
        }

        public void ClearAndReplace(IEnumerable<T> items)
        {
            if (DataSource.Equals(items))
                return;
            DataSource.Clear();
            DataSource.AddRange(items);

            _currentList = new List<T>(items);
            _originalList = DataSource;

            NotifyDataSetChanged();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OnBindViewHolder(holder as VH, position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var vh = CreateHolder(parent, viewType);

            vh.ItemView.Touch += (sender, e) =>
            {
                var view = sender as View;
                var actionMask = e.Event.ActionMasked;

                Action clickAction = () =>
                {
                    if (vh.AdapterPosition >= DataSource.Count)
                        return;
                    if (vh.AdapterPosition < 0)
                        return;
                    var item = DataSource[vh.AdapterPosition];
                    ItemClick?.Invoke(
                        this, new ItemClickEventArgs(vh.AdapterPosition, item, vh));
                };

                switch (actionMask)
                {
                    case MotionEventActions.Down:
                        Delegate.OnTouch(view, TState.Began, clickAction);
                        break;
                    case MotionEventActions.Up:
                        Delegate.OnTouch(view, TState.Ended, clickAction);
                        break;
                    case MotionEventActions.Cancel:
                        Delegate.OnTouch(view, TState.Cancel, clickAction);
                        break;
                }
            };
            if (DefaultSelector)
                vh.ItemView.AddRippleEffect();
            return vh;
        }

        public abstract void OnBindViewHolder(VH holder, int position);

        public abstract VH CreateHolder(ViewGroup parent, int viewType);

        protected void InvokeCollectionChange()
        {
            CollectionChange?.Invoke(this, EventArgs.Empty);
        }
    }
}