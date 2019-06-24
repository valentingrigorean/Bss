//
// TextExtensions.cs
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
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using Bss.Droid.Utils;

namespace Bss.Droid.Extensions
{
    public static class TextExtensions
    {
        public static void SetFont(this TextView textView, int fontType)
        {
            textView.Typeface = FontManager.Get(fontType);
        }

        public static void SetFont(this TextView textView, int fontType, float size)
        {
            textView.TextSize = size;
            SetFont(textView, fontType);
        }

        public static void SetFonts(this View view, int type,
                                    ICollection<View> ignoreList = null)
        {
            var grp = view as ViewGroup;
            if (grp != null)
                grp.SetFonts(type, ignoreList);
        }

        public static void SetFonts(this ViewGroup grp, int type, ICollection<View> ignoreList = null)
        {
            for (int i = 0; i < grp.ChildCount; i++)
            {
                var view = grp.GetChildAt(i);
                if (ignoreList != null &&
                    (ignoreList.Select(_ => ReferenceEquals(view, _)).
                        ToList().Count == 0))
                    continue;
                var viewGroup = view as ViewGroup;
                if (viewGroup != null)
                {
                    SetFonts(viewGroup, type, ignoreList);
                    continue;
                }
                var textView = view as TextView;
                if (textView != null)
                {
                    textView.SetFont(type);
                    continue;
                }
                var buttonView = view as Button;
                if (buttonView != null)
                {
                    buttonView.SetFont(type);
                }
            }
        }

        public enum IgnoreType
        {
            Button,
            TextView
        }

        public static void SetFonts(this View view, int type,IgnoreType ignoreType)
        {
            var grp = view as ViewGroup;
            if (grp != null)
                grp.SetFonts(type, ignoreType);
        }


        public static void SetFonts(this ViewGroup grp, int type, IgnoreType ignoreType)
        {
            for (int i = 0; i < grp.ChildCount; i++)
            {
                var view = grp.GetChildAt(i);

                var viewGroup = view as ViewGroup;
                if (viewGroup != null)
                {
                    SetFonts(viewGroup, type, ignoreType);
                    continue;
                }
                var textView = view as TextView;
                if (textView != null && ignoreType != IgnoreType.Button)
                {
                    textView.SetFont(type);
                    continue;
                }
                var buttonView = view as Button;
                if (buttonView != null && ignoreType != IgnoreType.Button)
                {
                    buttonView.SetFont(type);
                }
            }
        }
    }
}
