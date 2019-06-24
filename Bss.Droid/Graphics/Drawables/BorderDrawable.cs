//
// RoundDrawable.cs
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

using Android.Graphics;
using Android.Graphics.Drawables;

namespace Bss.Graphics.Drawables
{
	public class BorderDrawable : Drawable
	{
		private Rect _rect = new Rect();
		private RectF _rectF = new RectF();

		private Paint _fillPaint = new Paint(PaintFlags.AntiAlias | PaintFlags.Dither | PaintFlags.FilterBitmap);
		private Paint _linePaint = new Paint(PaintFlags.AntiAlias | PaintFlags.Dither | PaintFlags.FilterBitmap);

		public BorderDrawable()
		{
			_fillPaint.SetStyle(Paint.Style.Fill);
			_linePaint.StrokeWidth = 1.DpToPixel();
			_linePaint.SetStyle(Paint.Style.Stroke);
			FillColor = Color.Transparent;
		}

		public override int Opacity => 1;

		public float Radius { get; set; } = 0;

		public float LineWidth
		{
			get { return _linePaint.StrokeWidth; }
			set { _linePaint.StrokeWidth = value; }
		}


		public Color FillColor
		{
			get { return _fillPaint.Color; }
			set
			{
				_fillPaint.Color = value;
				InvalidateSelf();
			}
		}

		public Color LineColor
		{
			get { return _linePaint.Color; }
			set
			{
				_linePaint.Color = value;
				InvalidateSelf();
			}
		}

		public bool IsCircle { get; set; } = false;

		public override void Draw(Canvas canvas)
		{
			canvas.GetClipBounds(_rect);
			_rectF.Set(_rect);
			var radius = Radius;
			if (IsCircle)
				radius = _rect.Width() / 2 - LineWidth;

			if (radius > 0)
			{
				var lineWidth = LineWidth / 2;
				_rectF.Left += lineWidth;
				_rectF.Right -= lineWidth;
				_rectF.Top += lineWidth;
				_rectF.Bottom -= lineWidth;
			}
			canvas.DrawRoundRect(_rectF, radius, radius, _fillPaint);
			canvas.DrawRoundRect(_rectF, radius, radius, _linePaint);
		}

		public override void SetAlpha(int alpha)
		{

		}

		public override void SetColorFilter(ColorFilter colorFilter)
		{

		}
	}
}
