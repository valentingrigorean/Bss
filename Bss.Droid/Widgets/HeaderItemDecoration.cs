//
// HeaderItemDecoration.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2018 (c) Grigorean Valentin
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
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
namespace Bss.Droid.Widgets
{
    public class HeaderItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly IStickyHeaderInterface _listner;

        private Dictionary<int, View> _headerCache = new Dictionary<int, View>();

        public HeaderItemDecoration(IStickyHeaderInterface listner)
        {
            _listner = listner;
        }

        public override void OnDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            base.OnDrawOver(c, parent, state);

            var topChild = parent.GetChildAt(0);
            if (topChild == null)
                return;

            var topChildPosition = parent.GetChildAdapterPosition(topChild);
            if (topChildPosition == RecyclerView.NoPosition)
                return;
            
            var headerPosition = _listner.GetHeaderPositionForItem(topChildPosition);
            if (headerPosition == topChildPosition)
                return;
            
            var currentHeader = GetHeaderForItem(topChildPosition, parent);
            var contactPoint = currentHeader.Bottom;
            var childInContact = GetChildInContact(parent, contactPoint);
            if (childInContact != null)
            {
                if (_listner.IsHeader(parent.GetChildAdapterPosition(childInContact)))
                {
                    MoveHeader(c, currentHeader, childInContact);
                    return;
                }
            }

            DrawHeader(c, currentHeader);
        }

        private View GetChildInContact(RecyclerView parent, int contactPoint)
        {
            View childInContact = null;

            for (var i = 0; i < parent.ChildCount; i++)
            {
                var child = parent.GetChildAt(i);
                if (child.Bottom > contactPoint)
                {
                    if (child.Top <= contactPoint)
                    {
                        childInContact = child;
                        break;
                    }
                }
            }
            return childInContact;
        }

        private void FixLayoutSize(ViewGroup parent, View view)
        {
            var widthSpec = View.MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly);
            var heightSpec = View.MeasureSpec.MakeMeasureSpec(parent.Height, MeasureSpecMode.Unspecified);

            var childWidthSpec = ViewGroup.GetChildMeasureSpec(
                widthSpec, parent.PaddingLeft + parent.PaddingRight,
                view.LayoutParameters.Width);
            var childHeightSpec = ViewGroup.GetChildMeasureSpec(
                widthSpec, parent.PaddingTop + parent.PaddingBottom,
                view.LayoutParameters.Height);

            view.Measure(childWidthSpec, childHeightSpec);
            view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
        }

        private View GetHeaderForItem(int itemPosition, RecyclerView parent)
        {
            var headerPosition = _listner.GetHeaderPositionForItem(itemPosition);

            if (_headerCache.ContainsKey(headerPosition))
                return _headerCache[headerPosition];

            var layoutResId = _listner.GetHeaderLayout(itemPosition);
            var header = LayoutInflater.FromContext(parent.Context).Inflate(layoutResId, parent, false);
            _listner.BindHeaderData(header, headerPosition);
            _headerCache.Add(headerPosition, header);
            FixLayoutSize(parent, header);
            return header;
        }

        private void MoveHeader(Canvas c, View currentHeader, View nextHeader)
        {
            c.Save();
            c.Translate(0, nextHeader.Top - currentHeader.Height);
            currentHeader.Draw(c);
            c.Restore();
        }

        private void DrawHeader(Canvas c, View header)
        {
            c.Save();
            c.Translate(0, 0);
            header.Draw(c);
            c.Restore();
        }
    }
}
