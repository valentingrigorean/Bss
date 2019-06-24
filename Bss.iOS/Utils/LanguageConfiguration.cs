//
// LanguageConfiguration.cs
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
using Foundation;
namespace Bss.iOS.Utils
{
    public static class LanguageConfiguration
    {
        /// <summary>
        /// Gets or sets the default bundle.
        /// Its used for translation extension
        /// </summary>
        /// <value>The default bundle.</value>
        public static NSBundle DefaultBundle { get; set; } = NSBundle.MainBundle;

        public static NSBundle BundleForLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang) || lang.Length != 2)
                throw new Exception($"Invalid argument {nameof(lang)} should be 'en' format");
            var path = NSBundle.MainBundle.PathForResource(lang, "lproj");
            var bundle = NSBundle.FromPath(path);
            return bundle;
        }
    }
}

