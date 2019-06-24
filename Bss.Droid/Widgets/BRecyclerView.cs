//
// BRecyclerView.cs
//
// Author:
//       valentingrigorean <>
//
// Copyright (c) 2017 ${CopyrightHolder}
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


using Android.Content;
using Android.Util;
using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace Bss.Droid.Widgets
{
	public class BRecyclerView : RecyclerView
	{
		private readonly CustomAdapterDataObserver DataObserver;

		private View _emptyView;	

		public BRecyclerView(Context context) :
			base(context)
		{
			DataObserver = new CustomAdapterDataObserver(UpdateEmptyView);
		}

		public BRecyclerView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			DataObserver = new CustomAdapterDataObserver(UpdateEmptyView);
		}

		public BRecyclerView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			DataObserver = new CustomAdapterDataObserver(UpdateEmptyView);
		}

		/// <summary>
		/// Gets or sets the empty view. When the backing adapter has no
		/// data this view will be made visible and the recycler view hidden.
		/// </summary>
		/// <value>The empty view.</value>
		public View EmptyView
		{
			get { return _emptyView;}
			set
			{
				_emptyView = value;
				UpdateEmptyView();
			}
		}

		public bool IsNullOrEmpty => GetAdapter() == null || GetAdapter().ItemCount == 0;

		public override void SetAdapter(Adapter adapter)
		{
			var _adapter = GetAdapter();
			if (_adapter != null)
				adapter.UnregisterAdapterDataObserver(DataObserver);
			if (adapter != null)
				adapter.RegisterAdapterDataObserver(DataObserver);
			base.SetAdapter(adapter);
			UpdateEmptyView();
		}

		private void UpdateEmptyView()
		{
			if (_emptyView == null || GetAdapter() == null) return;
			var showEmptyView = GetAdapter().ItemCount == 0;
			_emptyView.Visibility = showEmptyView ? ViewStates.Visible : ViewStates.Gone;
			Visibility = showEmptyView ? ViewStates.Gone : ViewStates.Visible;
		}

		private class CustomAdapterDataObserver : AdapterDataObserver
		{
			private readonly Action Callback;

			public CustomAdapterDataObserver(Action callback)
			{
				Callback = callback;
			}

			public override void OnChanged()
			{
				base.OnChanged();
				Callback();
			}

            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                base.OnItemRangeInserted(positionStart, itemCount);
                Callback();
            }

            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                base.OnItemRangeRemoved(positionStart, itemCount);
                Callback();
            }
		}
	}
}
