//
// DividerDecoration.cs
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
using Android.Support.V7.Widget;
using Android.Graphics;
using Bss.Droid.Extensions;
using Android.Views;
using Android.Content.Res;
using Android.Content;
using Android.Util;

namespace Bss.Droid.Widgets
{
    public class DividerDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _height;
        private readonly int _leftPadding;
        private readonly int _rightPadding;
        private readonly Paint _paint;

        public DividerDecoration(int height, int lPadding, int rPadding, Color colour)
        {
            _height = height;
            _leftPadding = lPadding;
            _rightPadding = rPadding;
            _paint = new Paint();
            _paint.Color = colour;
        }

        public override void OnDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            base.OnDrawOver(c, parent, state);

            foreach (var child in parent.GetViews())
            {
                var top = child.Bottom;
                var bottom = top + _height;

                var left = child.Left + _leftPadding;
                var right = child.Right + _rightPadding;

                c.Save();
                c.DrawRect(left, top, right, bottom, _paint);
                c.Restore();
            }
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Set(0, 0, 0, _height);
        }

        /// <summary>
        /// A basic builder for divider decorations. The default builder creates a 1px thick black
        /// divider decoration.
        /// </summary>
        public class Builder
        {
            private Resources _resources;
            private int _height;
            private int _lPadding;
            private int _rPadding;
            private Color _color;

            public Builder(Context context)
            {
                _resources = context.Resources;
                _height = (int)TypedValue.ApplyDimension(ComplexUnitType.Px, 1f, _resources.DisplayMetrics);
                _lPadding = 0;
                _rPadding = 0;
                _color = Color.Black;
            }

            public Builder SetHeight(float pixels)
            {
                _height = (int)TypedValue.ApplyDimension(ComplexUnitType.Px, pixels, _resources.DisplayMetrics);
                return this;
            }

            public Builder SetPadding(float pixels)
            {
                SetLeftPadding(pixels);
                SetRightPadding(pixels);
                return this;
            }

            public Builder SetLeftPadding(float pixels)
            {
                _lPadding = (int)TypedValue.ApplyDimension(ComplexUnitType.Px, pixels, _resources.DisplayMetrics);
                return this;
            }

            public Builder SetRightPadding(float pixels)
            {
                _rPadding = (int)TypedValue.ApplyDimension(ComplexUnitType.Px, pixels, _resources.DisplayMetrics);
                return this;
            }

            public Builder SetColor(Color color)
            {
                _color = color;
                return this;
            }

            public DividerDecoration Build()
            {
                return new DividerDecoration(_height, _lPadding, _rPadding, _color);
            }
        }
    }
}
