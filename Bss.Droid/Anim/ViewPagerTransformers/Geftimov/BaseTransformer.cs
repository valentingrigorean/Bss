//
// BaseTransformer.cs
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
    /// port from https://github.com/geftimov/android-viewpager-transformers
    /// </summary>
    public abstract class BaseTransformer : Java.Lang.Object, ViewPager.IPageTransformer
    {
        protected virtual bool HideOffscreenPages { get; } = true;

        /// <summary>
        /// Indicates if the default animations of the view pager should be used.
        /// </summary>
        /// <value><c>true</c> if is paging enabled; otherwise, <c>false</c>.</value>
        protected virtual bool IsPagingEnabled { get; }

        protected abstract void OnTransform(View view, float position);

        public void TransformPage(View view, float position)
        {
            OnPreTransform(view, position);
            OnTransform(view, position);
            OnPostTransform(view, position);
        }

        protected virtual void OnPreTransform(View view, float position)
        {
            var width = view.Width;

            view.RotationX = 0;
            view.RotationY = 0;
            view.Rotation = 0;
            view.ScaleX = 1;
            view.ScaleY = 1;
            view.PivotX = 0;
            view.PivotY = 0;
            view.TranslationY = 0;
            view.TranslationX = IsPagingEnabled ? 0f : -width * position;

            if (HideOffscreenPages)
            {
                view.Alpha = position <= -1f || position >= 1f ? 0f : 1f;
            }
            else 
			{
                view.Alpha = 1f;
            }
        }


        /// <summary>
        /// Called each {@link #transformPage(android.view.View, float)} call after {@link #onTransform(android.view.View, float)} is finished.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="position">Position.</param>
        protected virtual void OnPostTransform(View view, float position)
        {
        }

    }
}
