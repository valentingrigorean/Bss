//
// CGAffineTransformExtension.cs
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
namespace CoreGraphics
{
    public static class CgAffineTransformExtension
    {
        public static float GetAngle(this CGAffineTransform transform)
        {
            var radians = Math.Atan2(transform.yx, transform.xx);
            return (float)(radians * (180 / Math.PI));
        }

        public static float GetRadians(this CGAffineTransform transform)
        {
            return (float)Math.Atan2(transform.yx, transform.xx);
        }

        public static float GetScaleX(this CGAffineTransform transform)
        {
            var x = transform.xx * transform.xx;
            var xy = transform.xy * transform.xy;
            var sum = x + xy;
            var sqrt = Math.Sqrt(sum);
            return (float)sqrt;
        }

        public static float GetScaleY(this CGAffineTransform transform)
        {
            var y = transform.yy * transform.yy;
            var yx = transform.yx * transform.yx;
            var sum = y + yx;
            var sqrt = Math.Sqrt(sum);
            return (float)sqrt;
        }

        public static CGAffineTransform RotationByDegree(this CGAffineTransform transform, nfloat degree)
        {
            return CGAffineTransform.Rotate(transform, (nfloat)(degree * Math.PI / 180f));
        }
    }
}



