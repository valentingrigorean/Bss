//
// BulletViewGroup.cs
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
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using Android.Views;
using Android.Runtime;
using System.ComponentModel;
using System;
using Android.Support.V4.View;
using Android.Database;

namespace Bss.Droid.Views
{
    [Register("bss.droid.views.BulletViewGroup"), DesignTimeVisible(true)]
    public class BulletViewGroup : LinearLayout
    {
        private DataSourceChange _lisener;

        private int _numbersOfPages;
        private int _currentPage;

        private int _prevPage = -1;

        private readonly IList<BulletView> _bullets = new List<BulletView>();

        private Rect _itemInset;

        private readonly WeakReference<ViewPager> _weakRefPager = new WeakReference<ViewPager>(null);

        private ViewPager Pager
        {
            get
            {
                ViewPager pager;
                _weakRefPager.TryGetTarget(out pager);
                return pager;
            }
            set { _weakRefPager.SetTarget(value); }
        }

        public static readonly Rect DefaultInset = new Rect(4.DpToPixel(), 0, 4.DpToPixel(), 0);

        private Color _pageSelectedColor;
        private Color _pageDefaultColor;

        public BulletViewGroup(Context context) :
            base(context)
        {
            Initialize();
        }

        public BulletViewGroup(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public BulletViewGroup(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public bool AllowSelection { get; set; } = true;

        public override int MinimumHeight => 15.DpToPixel();
       
        public class PageSelectedEventArgs : EventArgs
        {
            public PageSelectedEventArgs(int prevPage, int currPage)
            {
                PrevPage = prevPage;
                CurrentPage = currPage;
            }

            public int PrevPage { get; }
            public int CurrentPage { get; }
        }

        public event EventHandler<PageSelectedEventArgs> PageSelected;

        public Color PageSelectedColor
        {
            get { return _pageSelectedColor; }
            set
            {
                _pageSelectedColor = value;
                foreach (var item in _bullets)
                    item.SelectedColor = value;
            }
        }

        public Color PageDefaultColor
        {
            get { return _pageDefaultColor; }
            set
            {
                _pageDefaultColor = value;
                foreach (var item in _bullets)
                    item.DefaultColor = value;
            }
        }

        public Rect ItemInset
        {
            get { return _itemInset; }
            set
            {
                _itemInset = value;
                if (value == null)
                    _itemInset = null;
                foreach (var item in _bullets)
                    SetLayoutBullet(item);
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                if (_prevPage >= 0)
                    _bullets[_prevPage].Selected = false;
                _bullets[value].Selected = true;
                PageSelected?.Invoke(this, new PageSelectedEventArgs(_prevPage, value));
                _prevPage = value;
            }
        }

        public int NumberOfPages
        {
            get { return _numbersOfPages; }
            set
            {
                if (value == _numbersOfPages) return;
                _numbersOfPages = value;
                CreateBullets();
            }
        }

        public void SetViewPager(ViewPager pager)
        {
            if (Pager != null)
            {
                pager.AdapterChange -= AdapterChange;
                pager.PageSelected -= PageSelectedHandler;
            }
            Pager = pager;
            pager.AdapterChange += AdapterChange;
            pager.PageSelected += PageSelectedHandler;

            if (pager.Adapter == null) return;
            pager.Adapter.RegisterDataSetObserver(_lisener);
            Refresh();
        }

        private void AdapterChange(object sender, ViewPager.AdapterChangeEventArgs e)
        {
            e.OldAdapter?.UnregisterDataSetObserver(_lisener);
            e.NewAdapter?.RegisterDataSetObserver(_lisener);

            Refresh();
        }

        private void PageSelectedHandler(object sender, ViewPager.PageSelectedEventArgs e)
        {
            CurrentPage = e.Position;
        }

        private void Refresh()
        {
            var adapter = Pager?.Adapter;
            if (adapter == null) return;
            NumberOfPages = adapter.Count;
            if (NumberOfPages > 0)
                CurrentPage = Pager.CurrentItem;
        }

        private void BulletClick(object sender, EventArgs e)
        {
            if (!AllowSelection) return;
            var bullet = sender as BulletView;
            if (bullet == null) return;
            var index = (int)bullet.Tag;
            CurrentPage = index;
        }

        private void SetLayoutBullet(BulletView bulletView)
        {
            var lp = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent)
            {
                TopMargin = ItemInset.Top,
                BottomMargin = ItemInset.Bottom,
                LeftMargin = ItemInset.Left,
                RightMargin = ItemInset.Right,
                Gravity = GravityFlags.Center
            };
            bulletView.LayoutParameters = lp;
        }

        private void CreateBullets()
        {
            foreach (var item in _bullets)
            {
                item.Click -= BulletClick;
                RemoveView(item);
            }

            _bullets.Clear();
            for (var i = 0; i < NumberOfPages; i++)
            {
                var bullet = new BulletView(Context)
                {
                    Tag = i,
                    DefaultColor = PageDefaultColor,
                    SelectedColor = PageSelectedColor
                };
                bullet.Click += BulletClick;
                bullet.Selectable = false;
                SetLayoutBullet(bullet);
                _bullets.Add(bullet);
                AddView(bullet);
            }
        }

        private void Initialize()
        {

            _lisener = new DataSourceChange(Refresh);
            ItemInset = DefaultInset;
            NumberOfPages = 3;
            CurrentPage = 0;
            ItemInset = DefaultInset;
            PageDefaultColor = Color.DimGray;
            PageSelectedColor = Color.DeepSkyBlue;
            Orientation = Orientation.Horizontal;
            SetGravity(GravityFlags.Center);
        }

        private class DataSourceChange : DataSetObserver
        {
            private readonly WeakReference<Action> _weakRefCallback;

            public DataSourceChange(Action callback)
            {
                _weakRefCallback = new WeakReference<Action>(callback);
            }

            public override void OnChanged()
            {
                base.OnChanged();
                Action callback;
                _weakRefCallback.TryGetTarget(out callback);
                callback?.Invoke();
            }
        }
    }
}