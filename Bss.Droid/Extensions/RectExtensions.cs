//
// RectExtensions.cs
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
using Android.Graphics;
namespace Bss.Droid.Extensions
{
    public static class RectExtensions
    {
        public static Rect SetPosition(this Rect This, int x, int y)
        {
            This.Left = x;
            This.Top = y;
            This.Right = x + This.Right;
            This.Bottom = y + This.Bottom;

            return This;
        }

        public static Rect SetSize(this Rect This, int width, int height)
        {
            This.Right = This.Left + width;
            This.Bottom = This.Top + height;
            return This;
        }

        public static Rect SetFrame(this Rect This, int x, int y, int width, int height)
        {
            This.Left = x;
            This.Top = y;
            This.Right = x + width;
            This.Bottom = y + height;
            return This;
        }

        public static RectF SetPosition(this RectF This, float x, float y)
        {
            This.Left = x;
            This.Top = y;
            This.Right = x + This.Right;
            This.Bottom = y + This.Bottom;
            return This;
        }

        public static RectF SetSize(this RectF This, float width, float height)
        {
            This.Right = This.Left + width;
            This.Bottom = This.Top + height;
            return This;
        }


        public static RectF SetFrame(this RectF This, float x, float y, float width, float height)
        {
            This.Left = x;
            This.Top = y;
            This.Right = x + width;
            This.Bottom = y + height;
            return This;
        }
    }
}
