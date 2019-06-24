//
// ScaleOnTouchLisener.cs
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
using Android.Views;

namespace Bss.Droid.Views
{
    public class ScaleOnTouchLisener : Java.Lang.Object, View.IOnTouchListener
    {
        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public float Scale { get; set; } = 0.85f;

        /// <summary>
        /// Gets or sets the duration in msec
        /// </summary>
        /// <value>The duration.</value>
        public long Duration { get; set; } = 150;

        public bool Animated { get; set; } = true;

        public bool OnTouch(View v, MotionEvent e)
        {
            if (!Animated) return false;
            if (e.Action == MotionEventActions.Down)
                v.Animate().ScaleX(Scale).ScaleY(Scale).SetDuration(Duration).Start();
            if (e.Action == MotionEventActions.Up)
                v.Animate().ScaleX(1f).ScaleY(1f).SetDuration(Duration).Start();
            return false;
        }

    }
}
