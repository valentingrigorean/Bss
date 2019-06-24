//
// ButtonExtensions.cs
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
using Android.Widget;
using System.Linq;
using Java.Lang;
using Android.Graphics.Drawables;

namespace Bss.Droid.Extensions
{
    public static class ButtonExtensions
    {
        public static void SetTintDrawable(this Button This, int selectorID)
        {
            if ((int)Android.OS.Build.VERSION.SdkInt >= 23)
                return;

            var drawables = This.GetCompoundDrawables();
            var newDrawables = new Drawable[4];

            for (var i = 0; i < drawables.Length; i++)
            {
                var drawable = drawables[i];
                if (drawable == null)
                    continue;

                newDrawables[i] = DrawableUtils.GetTintedDrawable(This.Context, drawable, selectorID);
            }

            This.SetCompoundDrawables(newDrawables[0], newDrawables[1], newDrawables[2], newDrawables[3]);
        }
    }
}
