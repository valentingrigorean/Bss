//
// DrawableExtensions.cs
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
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V4.Graphics.Drawable;
namespace Bss.Droid.Extensions
{
    public static class DrawableExtensions
    {
        public static Drawable SetCompatTint(this Drawable self, Color color)
        {
            if ((int)Android.OS.Build.VERSION.SdkInt >= 21)
            {
                self.SetTint(color);
                return self;
            }

            var newDrawable = DrawableCompat.Wrap(self);
            DrawableCompat.SetTint(newDrawable, color);
            return newDrawable;
        }

        public static RotateDrawable ToRateDrawable(this Drawable self)
        {
            return new RotateDrawable
            {
                Drawable = self
            };
        }
    }
}
