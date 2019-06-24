//
// SizeTypeEvaluator.cs
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
using Android.Animation;
using Bss.Droid.Graphics;

namespace Bss.Droid.Anim
{
    public class BSizeTypeEvaluator : Java.Lang.Object, ITypeEvaluator
    {
        /// <summary>
        /// Evaluate the specified fraction, startValue and endValue.
        /// result = x0 + t * (x1 - x0), where x0 is startValue, x1 is endValue, and t is fraction.
        /// </summary>
        /// <param name="fraction">Fraction.</param>
        /// <param name="startValue">Start value.</param>
        /// <param name="endValue">End value.</param>
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
        {
            var fromSize = startValue as BSize;
            var endSize = endValue as BSize;
            if (fromSize == null || endSize == null)
                throw new InvalidCastException("StartValue,endvalue most be BSize");
            var width = fromSize.Width + fraction * (endSize.Width - fromSize.Width);
            var height = fromSize.Height + fraction * (endSize.Height - fromSize.Height);
            var newSize = new BSize((int)width, (int)height);
            return newSize;
        }
    }
}
