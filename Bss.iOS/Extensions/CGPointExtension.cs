//
// CGPointExtension.cs
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
using CoreGraphics;
using System.Drawing;

public static class CgPointExtension
{
    public static nfloat GetDistance(this CGPoint point1, CGPoint point2)
    {
        var dx = point1.X - point2.X;
        var dy = point1.Y - point2.Y;
        return new nfloat(Math.Sqrt(dx * dx + dy * dy));
    }

    public static CGPoint GetMidPoint(this CGPoint point, CGSize size)
    {
        return new CGPoint(point.X - size.Width / 2f, point.Y - size.Height / 2f);
    }

    public static Point ToNetPoint(this CGPoint point)
    {
        return new Point((int)point.X, (int)point.Y);
    }

    public static PointF ToNetPointF(this CGPoint point)
    {
        return new PointF((float)point.X, (float)point.Y);
    }
}


