//
// Locale.cs
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
using System.Linq;
using System.Reflection;

namespace Bss.Core.Lang
{
    [Serializable]
    public class Locale : IEquatable<Locale>
    {
        public Locale()
        {
        }

        public Locale(string shortName)
        {
            ShortName = shortName;
        }

        public Locale(string shortName, string fullName) : this(shortName)
        {
            FullName = fullName;
        }

        public string ShortName { get; set; }
        public string FullName { get; set; }

        public static Locale Default
        {
            get
            {
#if __ANDROID__
                //TODO check if this valid?
                return new Locale(Java.Util.Locale.Default.Language, Java.Util.Locale.Default.DisplayLanguage);
#elif __IOS__
                var devLang = Foundation.NSBundle.MainBundle.PreferredLocalizations.FirstOrDefault();
                if (string.IsNullOrEmpty(devLang))
                    devLang = "en";
                else
                {
                    var split = devLang.Split('-');
                    devLang = split.Length > 0 ? split[0] : "en";
                }
                return FromShortName(devLang) ?? new Locale(devLang);
#else
                return new Locale("en");
#endif
            }
        }


        public static Locale Catala => new Locale("ca", "Catalan");
        public static Locale English => new Locale("en", "English");
        public static Locale French => new Locale("fr", "French");
        public static Locale Italian => new Locale("it", "Italia");
        public static Locale Spanish => new Locale("es", "Spanish");
        //public static Locale 한국어 => new Locale("한국어", "한국어");
        //public static Locale Polskie => new Locale("pl", "Polish");
        //public static Locale Svenska => new Locale("sv", "Svenska");
        //public static Locale 中国 => new Locale("中国", "中国");
        //public static Locale German => new Locale("de", "German");
        //public static Locale Bahasa => new Locale("ba", "Bahasa");
        //public static Locale Dutch => new Locale("du", "Dutch");
        //public static Locale Portugues => new Locale("po", "Português");
        //public static Locale ไทย => new Locale("ไทย", "ไทย");
        //public static Locale العربي => new Locale("العربي", "العربي");
        //public static Locale Norvegese => new Locale("no", "Norwegian");
        //public static Locale Pусский => new Locale("py", "Pусский");
        //public static Locale Український => new Locale("yk", "Український");
        //public static Locale České => new Locale("ce", "České");

        public static Locale[] AllLanguages => new[]
        {
            Catala,
            English,
            French,
            Italian,
            Spanish
        };

        public bool Equals(Locale other)
        {
            return Equals((object)other);
        }

        public static Locale FromShortName(string shortName)
        {
            var lowername = shortName.ToLower();
            return AllLanguages.FirstOrDefault(_ => _.ShortName.ToLower() == lowername);
        }

        public static Locale FromFullName(string fullName)
        {
            var lower = fullName.ToLower();
            return AllLanguages.FirstOrDefault(_ => _.FullName.ToLower() == lower);
        }

        public static Locale[] GetAllLanguageEmbedded()
        {
            return GetAllLanguageEmbedded(Assembly.GetExecutingAssembly());
        }

        public static Locale[] GetAllLanguageEmbedded(Assembly assembly)
        {
            var files = assembly.GetManifestResourceNames();
            var languages = files.Where(_ => _.EndsWith(".trans", StringComparison.CurrentCulture)).
                Select(res =>
                {
                    var split = res.Split('.');
                    return FromShortName(split[split.Length - 3]);
                }).Distinct().ToArray();

            return languages;
        }


        public static bool operator ==(Locale l1, Locale l2)
        {
            if (ReferenceEquals(l1, l2)) 
                return true;
            if ((object)l1 == null || (object)l2 == null)
                return false;
            return l1.Equals(l2);
        }

        public static bool operator !=(Locale l1, Locale l2)
        {
            return !(l1 == l2);
        }

        public override bool Equals(object obj)
        {
            var locale = obj as Locale;
            if (locale == null) 
                return false;
            if (ReferenceEquals(obj, this))
                return true;
            return GetHashCode() == locale.GetHashCode();
        }

        public override int GetHashCode()
        {
            var shortName = ShortName?.GetHashCode();
            var fullName = FullName?.GetHashCode();
            if (shortName.HasValue && fullName.HasValue)
                return shortName.Value + fullName.Value;
            if (shortName.HasValue)
                return shortName.Value;
            return fullName.Value;
        }
    }
}