//
// BRadioGroup.cs
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

using Android.Views;
using System.Collections.Generic;
using Android.Widget;
using System.Linq;
using System;

namespace Bss.Droid.Widgets
{
    public class BRadioGroup : Java.Lang.Object, IBOnCheckedChangeListener
    {
        private readonly IList<ICompoundButton> Items = new List<ICompoundButton>();
        private readonly IList<ICompoundButton> PreviusItems = new List<ICompoundButton>();

        public BRadioGroup(ViewGroup rootView)
        {
            RootView = rootView;
            Initialize();
        }

        public BRadioGroup(IEnumerable<View> views)
        {
            InitializeHandlers(views);
        }

        public ViewGroup RootView { get; }

        public int MinSelected { get; set; } = -1;

        public int MaxSelection { get; set; } = 1;

        public bool AutoDeselect { get; set; } = false;

        public int Count => Items.Count;

        public IList<ICompoundButton> Selected => Items.Where(_ => _.Checked).ToArray();

        public IList<int> SelectedIndexs => Selected.Select(_ => Items.IndexOf(_)).ToArray();

        public Func<ICompoundButton, bool> ShouldChangeCheck { get; set; } = (arg) => true;

        protected override void Dispose(bool disposing)
        {
            foreach (var item in Items)
                item.SetOnCheckedChangeListener(null);
            base.Dispose(disposing);
        }

        public void ClearAll()
        {
            PreviusItems.Clear();
            foreach (var item in Items)
                item.Checked = false;
        }

        public void SetItemChecked(int position, bool chck)
        {
            var item = Items[position];
            item.Checked = chck;
        }

        public void OnCheckedChanged(ICompoundButton buttonView, bool isChecked)
        {
            if (isChecked && PreviusItems.Count == MaxSelection)
            {
                if (!AutoDeselect)
                {
                    buttonView.Checked = false;
                    return;
                }
                var prevItem = PreviusItems[0];
                prevItem.Checked = false;
            }
            if (isChecked)
                PreviusItems.Add(buttonView);
            else
            {
                if (PreviusItems.Count < MinSelected)
                {
                    buttonView.Checked = true;
                    return;
                }
                PreviusItems.Remove(buttonView);
            }
        }

        private void InitializeHandlers(IEnumerable<View> views)
        {
            foreach (var view in views)
            {
                var btn = view as ICompoundButton;
                if (view is CompoundButton)
                    btn = new CompoundButtonAdapter((CompoundButton)view);
                if (btn == null) continue;
                Items.Add(btn);
                if (btn.Checked)
                    PreviusItems.Add(btn);
            }

            foreach (var item in Items)
                item.SetOnCheckedChangeListener(this);
        }

        private void Initialize()
        {
            var list = new List<View>();

            for (var i = 0; i < RootView.ChildCount; i++)
                list.Add(RootView.GetChildAt(i));

            InitializeHandlers(list);
        }
    }
}
