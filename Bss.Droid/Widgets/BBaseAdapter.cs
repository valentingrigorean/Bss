//
// BBaseAdapter.cs
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
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using System.Linq;
using Android.OS;

namespace Bss.Droid.Widgets
{
    [Serializable]
    public sealed class ItemEventArgs<T> : EventArgs
    {
        public ItemEventArgs(T model)
        {
            Model = model;
        }

        public ItemEventArgs(T model, object tag)
        {
            Model = model;
            Tag = tag;
        }


        public object Tag { get; private set; }

        public T Model { get; private set; }
    }

    public abstract class BBaseAdapter<T> : BaseAdapter<T>
    {
        private IList<T> _originalList;
        private IList<T> _currentList;
        private View _header;
        private View _footer;
        private int? _headerId;
        private int? _footerId;

        protected BBaseAdapter(Context context) : this(context, new List<T>())
        {

        }

        protected BBaseAdapter(Context context, IList<T> dataSource)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), "context == null");
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource), "dataSource == null");
            Context = context;
            LayoutInflater = LayoutInflater.FromContext(context);
            _originalList = dataSource;
            _currentList = new List<T>(_originalList);
        }

        protected Context Context { get; }

        public class ViewHolder : Java.Lang.Object
        {
            public ViewHolder(View contentView)
            {
                ContentView = contentView;
            }

            public View ContentView { get; }
            public int ViewType { get; internal set; }
        }

        public virtual event EventHandler<ItemEventArgs<T>> ItemClick;

        public virtual event EventHandler DataSetChanged;

        public override int Count
        {
            get
            {
                int count = _currentList == null ? 0 : _currentList.Count;
                if (HaveHeader())
                    count++;
                if (HaveFooter())
                    count++;
                return count;
            }
        }

        public override bool IsEnabled(int position)
        {
            if (HaveHeader() && position == 0)
                return false;
            if (HaveFooter() && position == Count - 1)
                return false;
            return true;
        }

        public override int ViewTypeCount => 3;

        public override int GetItemViewType(int position)
        {
            if (HaveHeader() && position == 0)
                return 0;
            if (HaveFooter() && position == Count - 1)
                return 2;
            return 1;
        }

        protected LayoutInflater LayoutInflater { get; }

        protected virtual IList<T> DataSource => _currentList;

        public View Header => _header;

        public sealed override T this[int position] => DataSource[position];

        public void SetHeader(View view)
        {
            _header = view;
            if (view == null)
                _headerId = null;
        }

        public View Footer => _footer;

        public void SetHeader(int resource)
        {
            _headerId = resource;
            if (_header != null || resource == 0)
                _header = null;
        }

        public void SetFooter(View view)
        {
            _footer = view;
            if (view == null)
                _footerId = null;
        }

        public void SetFooter(int resource)
        {
            _footerId = resource;
            if (_footerId != null || resource == 0)
                _footerId = null;
        }

        public void Add(T item)
        {
            DataSource.Add(item);
            NotifyDataSetThreadSafe();
        }

        public void AddRange(IEnumerable<T> range)
        {
            AddRange(range, false);
        }

        public void AddRange(IEnumerable<T> range, bool clearList)
        {
            if (clearList)
                DataSource.Clear();
            foreach (var item in range)
                DataSource.Add(item);
            NotifyDataSetThreadSafe();
        }

        public void Remove(T item)
        {
            DataSource.Remove(item);
            NotifyDataSetThreadSafe();
        }

        public void UpdateDataSource(IList<T> newDataSource)
        {
            _originalList = _currentList = newDataSource;
            NotifyDataSetThreadSafe();
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
            NotifyDataSetThreadSafe();
        }


        public void ClearFilter()
        {
            if (_originalList == null)
                return;
            if (_currentList.Count == _originalList.Count)
                return;
            _currentList = _originalList;
            NotifyDataSetThreadSafe();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        protected virtual void EmitItemClick(T model)
        {
            ItemClick?.Invoke(this, new ItemEventArgs<T>(model));
        }

        protected virtual void EmitItemClick(object sender, T model)
        {
            ItemClick?.Invoke(sender, new ItemEventArgs<T>(model));
        }

        protected abstract void OnBindViewHolder(ViewHolder holder, T model, int position);

        protected abstract ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType);

        public sealed override View GetView(int position, View convertView, ViewGroup parent)
        {


            if (position == 0 && HaveHeader())
            {
                if (convertView != null)
                    return convertView;
                if (_header == null)
                    _header = LayoutInflater.Inflate(_headerId.Value,
                        parent, false);
                return _header;
            }
            if (position == Count - 1 && HaveFooter())
            {
                if (convertView != null)
                    return convertView;
                if (_footerId == null)
                    _footer = LayoutInflater.Inflate(_footerId.Value,
                        parent, false);
                return _footer;
            }
            var viewType = GetItemViewType(position);
            var vh = convertView?.Tag as ViewHolder;
            if (vh == null || vh.ViewType != viewType)
            {
                vh = OnCreateViewHolder(parent, viewType);
                vh.ViewType = viewType;
            }
            OnBindViewHolder(vh, _currentList[position], position);
            return vh.ContentView;
        }

        protected int GetPositionIndex(int position)
        {
            if (HaveHeader()) return position - 1;
            return position;
        }

        //private View CheckConvertView(View convertView)
        //{
        //    if (HaveHeader() && _header == convertView)
        //        return null;
        //    if (HaveFooter() && _footer == convertView)
        //        return null;
        //    return convertView;
        //}

        private bool HaveHeader()
        {
            return _header != null || (_headerId != null && _headerId.Value > 0);
        }

        private bool HaveFooter()
        {
            return _footer != null || (_footerId != null && _footerId.Value > 0);
        }

        private void NotifyDataSetThreadSafe()
        {
            var handler = new Handler(Context.MainLooper);
            handler.Post(() =>
            {
                NotifyDataSetChanged();
                DataSetChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
