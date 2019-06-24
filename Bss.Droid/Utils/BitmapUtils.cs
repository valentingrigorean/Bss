//
// BitmapUtils.cs
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
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Bss.Droid.Extensions;

namespace Bss.Droid.Utils
{
    public static class BitmapUtils
    {
        public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, int reqWidth, int reqHeight)
        {
            var options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeResource(res, resId, options);

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeResource(res, resId, options);
        }

        public static Bitmap DecodeSampledBitmapFromUri(Context context, Android.Net.Uri uri, int reqWidth, int reqHeight)
        {
            var options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            using (var imageStream = context.ContentResolver.OpenInputStream(uri))
            {
                BitmapFactory.DecodeStream(imageStream, new Rect(0, 0, 0, 0), options);

                // Calculate inSampleSize
                options.InSampleSize = CalculateInSampleSizeEqualOrSmaller(options, reqWidth, reqHeight);

                // Decode bitmap with inSampleSize set
                options.InJustDecodeBounds = false;      
            }

            using (var imageStream = context.ContentResolver.OpenInputStream(uri))
            {
                var tempBitmap = BitmapFactory.DecodeStream(imageStream, new Rect(0, 0, 0, 0), options);

                if (tempBitmap.Width > reqWidth || tempBitmap.Height > reqHeight)
                {
                    double newWidth = 0;
                    double newHeight = 0;
                    double ratio = (double)tempBitmap.Width / (double)tempBitmap.Height;

                    if (ratio < 1)
                    {
                        newHeight = reqHeight;
                        newWidth = ratio * newHeight;
                    }
                    else
                    {
                        newWidth = reqWidth;
                        newHeight = newWidth / ratio;
                    }

                    return tempBitmap.Resize((int)newWidth, (int)newHeight);
                }

                return tempBitmap;
            }
        }

        private static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = options.OutHeight;
            var width = options.OutWidth;
            var inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {

                var halfHeight = height / 2;
                var halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) >= reqHeight
                        && (halfWidth / inSampleSize) >= reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        private static int CalculateInSampleSizeEqualOrSmaller(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = options.OutHeight;
            var width = options.OutWidth;
            var inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int stretch_width = (int)Math.Round((double) width / (double)reqWidth);
                int stretch_height = (int)Math.Round((double)height / (double)reqHeight);

                if (stretch_width <= stretch_height)
                    inSampleSize = stretch_height;
                else
                    inSampleSize = stretch_width;
            }

            return inSampleSize;
        }

        public class ClusterFactory
        {
            private Paint TextPaint { get; } = new Paint
            {
                AntiAlias = true
            };

            private Paint BorderPaint { get; } = new Paint
            {
                AntiAlias = true
            };

            private Paint BackgroundPaint { get; } = new Paint
            {
                AntiAlias = true
            };

            private Rect RectForText { get; } = new Rect();

            public ClusterFactory()
            {
                BackgroundPaint.SetStyle(Paint.Style.Fill);
                BackgroundColor = Color.CadetBlue;

                BorderPaint.SetStyle(Paint.Style.Stroke);
                BorderPaint.StrokeWidth = 2.DpToPixel();
                BorderColor = Color.White;

                TextPaint.SetStyle(Paint.Style.Fill);
                TextSize = 20.DpToPixel();
                TextColor = Color.White;
            }

            public Color BackgroundColor
            {
                get { return BackgroundPaint.Color; }
                set { BackgroundPaint.Color = value; }
            }

            public Color BorderColor
            {
                get { return BorderPaint.Color; }
                set { BorderPaint.Color = value; }
            }

            public float BorderWidth
            {
                get { return BorderPaint.StrokeWidth; }
                set { BorderPaint.StrokeWidth = value; }
            }

            public Typeface Typeface
            {
                get { return TextPaint.Typeface; }
                set { TextPaint.SetTypeface(value); }
            }

            public float TextSize
            {
                get { return TextPaint.TextSize; }
                set { TextPaint.TextSize = value; }
            }

            public Color TextColor
            {
                get { return TextPaint.Color; }
                set { TextPaint.Color = value; }
            }

            public enum Shape
            {
                Circle,
            }

            public Bitmap Create(string text, int padding = 0, Shape shape = Shape.Circle)
            {
                RectForText.SetEmpty();
                TextPaint.GetTextBounds(text, 0, text.Length, RectForText);
                RectForText.Right += padding * 2;
                if (shape == Shape.Circle)
                    return DrawCircle(RectForText, padding, text);
                throw new NotSupportedException();
            }

            private Bitmap DrawCircle(Rect rect, int padding, string text)
            {
                var max = Math.Max(rect.Width(), rect.Height());
                var bmp = Bitmap.CreateBitmap(max, max, Bitmap.Config.Argb8888);
                var canvas = new Canvas(bmp);
                var center = max / 2f;
                canvas.DrawCircle(center, center, center, BackgroundPaint);
                canvas.DrawCircle(center, center, center - BorderWidth, BorderPaint);
                canvas.DrawText(text, padding, center - (TextPaint.Ascent() + TextPaint.Descent()) / 2f, TextPaint);
                return bmp;
            }

        }
    }
}
