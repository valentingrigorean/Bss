//
// FontHelper.cs
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
using CoreText;
using UIKit;
using System.Linq;

namespace Bss.iOS.Utils
{
    public static class FontManager
    {
        private static readonly IDictionary<int, Lazy<UIFont>> Fonts =
            new Dictionary<int, Lazy<UIFont>>();

        public static void Add(int type, string name)
        {
            if (Fonts.ContainsKey(type))
                return;
            Fonts.Add(type, new Lazy<UIFont>(() =>
            {
                UIFont font;
                if (!TryCreate(name, out font))
                    throw new Exception($"Font with {name} not found.");
                return font;
            }));
        }

        public static void Add(int type, UIFont font)
        {
            if (Fonts.ContainsKey(type))
                return;
            Fonts.Add(type, new Lazy<UIFont>(() => font));
        }

        public static UIFont Get(int type)
        {
            return Get(type, 16);
        }

        public static UIFont Get(int type, nfloat size)
        {
            return Fonts.ContainsKey(type) ? Fonts[type].Value.WithSize(size) :
                UIFont.FromName("HelveticaNeueInterface-Regular", size);
        }

        public static IList<UIFont> GetFontByFamilyName(string name)
        {
            var family = UIFont.FamilyNames.Where(_ => _.Contains(name)).ToArray();
            return (from fam in family from subFam in UIFont.FontNamesForFamilyName(fam) select UIFont.FromName(subFam, 16f)).ToList();
        }

        public static UIFont GetFontWithTraits(UIFontDescriptorSymbolicTraits traits, nfloat size)
        {
            var desc = new UIFontDescriptor();
            desc = desc.CreateWithTraits(traits);
            return UIFont.FromDescriptor(desc, size);
        }

        public static CTFont GetCtFont(this UIFont font)
        {
            var fda = new CTFontDescriptorAttributes
            {
                FamilyName = font.FamilyName,
                Size = (float)font.PointSize,
                StyleName = font.FontDescriptor.Face
            };
            var fd = new CTFontDescriptor(fda);
            return new CTFont(fd, 0);
        }

        public static bool Exists(string name)
        {
            var font = UIFont.FromName(name, 16f);
            return font != null;
        }

        public static void PrintAllFonts()
        {
            foreach (var font in UIFont.FamilyNames)
            {
                Console.WriteLine(font + ":");
                var subFamilty = UIFont.FontNamesForFamilyName(font);
                foreach (var subfont in subFamilty)
                {
                    Console.WriteLine("   -{0}", subfont);
                }
            }
        }

        private static bool TryCreate(string name, out UIFont font)
        {
            font = UIFont.FromName(name, 16f);
            return font != null;
        }
    }
}


