//
// CubeOutTransformer.cs
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
using Android.Support.V4.View;
using Android.Views;

namespace Bss.Droid.Anim.ViewPagerTransformers.Geftimov
{

    /// <summary>
    /// Cube out transformer.
    /// https://github.com/geftimov/android-viewpager-transformers/wiki/CubeOutTransformer
    /// </summary>
    public class CubeOutTransformer : BaseTransformer
    {

        protected override bool IsPagingEnabled => true;

        protected override void OnTransform(View view, float position)
        {
            view.PivotX = position < 0f ? view.Width : 0f;
            view.PivotY = view.Height * 0.5f;
            view.RotationY = 90f * position;
        }
    }
}
