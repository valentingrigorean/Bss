//
// FontManager.cs
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
using Android.App;
using Android.Graphics;
using Android.Util;
using Bss.Droid.Extensions;

namespace Bss.Droid.Utils
{
    public static class FontManager
    {
        private static readonly IDictionary<int, Typeface> Fonts = new Dictionary<int, Typeface>();


        public static void AddFromAssets(int type, string name)
        {
            if (Fonts.ContainsKey(type))
                return;
            Typeface font;
            if (!TryCreate(name, out font))
                throw new Exception($"Font with {name} not found.");
            Fonts.Add(type, font);
        }

        public static void Add(int type, Typeface font)
        {
            if (Fonts.ContainsKey(type)) return;
            Fonts.Add(type, font);
        }

        public static void Add(int type, string name)
        {
            if (Fonts.ContainsKey(type)) return;
            var tf = Typeface.CreateFromAsset(Application.Context.Assets, name);
            Fonts.Add(type, tf);
        }

        public static Typeface Get(int type)
        {
            if (Fonts.ContainsKey(type))
                return Fonts[type];
            Log.Warn(nameof(FontManager), "No font found with id {0}".Format(type));
            return Typeface.Default;
        }


        private static bool TryCreate(string name, out Typeface font)
        {
            var assets = Application.Context.Assets;
            try
            {
                font = Typeface.CreateFromAsset(assets, name);
            }
            catch
            {
                font = null;
                return false;
            }
            return font != null;
        }
    }
}
