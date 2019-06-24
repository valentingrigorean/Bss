//
// BulletView.cs
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
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Bss.Droid.Views
{
    [Register("bss.droid.views.BulletView"), DesignTimeVisible(true)]
    public class BulletView : View
    {
        private readonly Path _path = new Path();

        private Color _selectedColor;
        private Color _defaultColor;

        //private Paint _maskPaint;
        //private Bitmap _maskBitmap;
        //private Bitmap _background;

        public BulletView(Context context) :
            base(context)
        {
            Initialize();
        }

        public BulletView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public BulletView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public bool Selectable { get; set; } = true;

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;
                Invalidate();
            }
        }

        public Color DefaultColor
        {
            get { return _defaultColor; }
            set
            {
                _defaultColor = value;
                if (!Selected)
                    Invalidate();
            }
        }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                if (Selected)
                    Invalidate();
            }
        }

        public override void SetBackgroundColor(Color color)
        {
            throw new NotSupportedException("Please set backgroudColor using DefaultColor  or SelectedColor");
        }

        public override void SetBackgroundResource(int resid)
        {
            throw new NotSupportedException("Please set backgroudColor using DefaultColor  or SelectedColor");
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var width = MeasuredWidth;
            var height = MeasuredHeight;
            var size = width > height ? height : width;
            SetMeasuredDimension(size, size);
            InitializeDraw(size);
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.ClipPath(_path);
            canvas.DrawColor(Selected ? SelectedColor : DefaultColor);
        }

        private void InitializeDraw(int size)
        {
            //var maskPaint = new Paint();
            //maskPaint.AntiAlias = true;
            //maskPaint.SetStyle(Paint.Style.FillAndStroke);
            //maskPaint.Color = Color.Black;

            _path.Reset();
            _path.AddCircle(size / 2f, size / 2f, size / 2f, Path.Direction.Ccw);

            //var rectBounds = new RectF();
            //_path.ComputeBounds(rectBounds, true);

            //if (_maskBitmap != null)
            //    _maskBitmap.Recycle();
            //_maskBitmap = Bitmap.CreateBitmap((int)rectBounds.Width(), (int)rectBounds.Height(), Bitmap.Config.Argb8888);

            //var maskCanvas = new Canvas(_maskBitmap);
            //maskCanvas.DrawPath(_path, maskPaint);

            //_maskPaint = new Paint();
            //_maskPaint.AntiAlias = true;
            //_maskPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
        }

        private void Initialize()
        {
            Click += (sender, e) =>
            {
                if (Selectable)
                    Selected = !Selected;
            };
        }
    }
}
