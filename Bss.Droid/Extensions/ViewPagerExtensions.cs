//
// ViewPagerExtensions.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2017 (c) Grigorean Valentin
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
using Android.Animation;

namespace Bss.Droid.Extensions
{
    public static class ViewPagerExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/a/30976741/8644269
        /// </summary>
        public static void SetCurrentItem(this ViewPager This, int item, long animationSpeed = 500)
        {
            if (Math.Abs(This.CurrentItem - item) > 1)
            {
                This.SetCurrentItem(item, true);
                return;
            }

            var forward = This.CurrentItem < item;
            var padding = forward ? This.PaddingStart : This.PaddingEnd;
            var animator = ValueAnimator.OfInt(0, This.Width - padding);
            animator.SetDuration(animationSpeed);

            animator.AnimationCancel += (sender, e) => This.EndFakeDrag();
            animator.AnimationEnd += (sender, e) => This.EndFakeDrag();

            var oldDragPosition = 0;

            animator.Update += (sender, e) =>
            {
                var dragPosition = (int)e.Animation.AnimatedValue;
                var dragOffset = dragPosition - oldDragPosition;
                oldDragPosition = dragPosition;
                This.FakeDragBy(dragOffset * (forward ? -1 : 1));
            };

            This.BeginFakeDrag();
            animator.Start();
        }
    }
}