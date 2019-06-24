//
// StringExtension.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 valentingrigorean
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
using Bss.iOS.Utils;
using Foundation;

// Analysis disable once CheckNamespace
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;

public static class StringExtension
{

    public static IEnumerable<int> GetUnicodeCodePoints(this string s)
    {
        for (var i = 0; i < s.Length; i++)
        {
            var unicodeCodePoint = char.ConvertToUtf32(s, i);
            if (unicodeCodePoint > 0xffff)
            {
                i++;
            }
            yield return unicodeCodePoint;
        }
    }

    public static NSAttributedString GetHtmlAttributedString(this string This, UITextAlignment? textAlignment = null)
    {
        var err = new NSError();
        var atts = new NSAttributedStringDocumentAttributes
        {
            DocumentType = NSDocumentType.HTML,
            StringEncoding = NSStringEncoding.UTF8
        };


        if (textAlignment.HasValue)
        {
            var aligment = "";
            switch (textAlignment.Value)
            {
                case UITextAlignment.Center:
                    aligment = "center";
                    break;
                case UITextAlignment.Left:
                    aligment = "left";
                    break;
                case UITextAlignment.Right:
                    aligment = "right";
                    break;
                default:
                    throw new NotSupportedException($"TextAlignment {textAlignment.Value} not supported");

            }
            This = $"<div style='text-align:{aligment};'>{This}</div>";
        }

        return new NSAttributedString(NSData.FromString(This), atts, ref err);
    }

    /// <summary>
    /// Gets the translation.
    /// Will use LanguageConfiguration.DefaultBundle 
    /// </summary>
    /// <returns>The translation.</returns>
    /// <param name="str">String.</param>
    public static string GetTranslation(this string str)
    {
        return GetTranslation(str, LanguageConfiguration.DefaultBundle);
    }

    public static string GetTranslation(this string str, NSBundle bundle)
    {
        return bundle.LocalizedString(str, "", "");
    }

    public enum ReturnType
    {
        None,
        /// <summary>
        /// Will return back to app after call ends
        /// </summary>
        Back
    }

    public static void CallNumber(this string number, ReturnType returnType = ReturnType.Back)
    {
        var havePrefix = number.StartsWith("tel://", StringComparison.CurrentCulture) ||
                               number.StartsWith("telprompt://", StringComparison.CurrentCulture);
        if (!havePrefix)
            switch (returnType)
            {
                case ReturnType.Back:
                    if (!number.Contains("telprompt://"))
                        number = "telprompt://" + number;
                    break;
                case ReturnType.None:
                    if (!number.Contains("tel://"))
                        number = "tel://" + number;
                    break;
            }
        UIApplication.SharedApplication.OpenUrl(new NSUrl(number));
    }

    public static nfloat GetWidthFromText(this string str, float fontSize = 17f)
    {
        var lbl = new UILabel { Text = str };
        lbl.Font = lbl.Font.WithSize(fontSize);
        lbl.SizeToFit();
        return lbl.Bounds.Width;
    }

    public static nfloat GetHeightFromText(this string str, UIFont font, nfloat width)
    {
        var ns = new NSString(str);
        var attr = new UIStringAttributes();
        attr.Font = font;
        var rect = ns.GetBoundingRect(new CGSize(width, nfloat.MaxValue),
                                      NSStringDrawingOptions.UsesLineFragmentOrigin,
                                      attr, null);
        return (nfloat)Math.Ceiling(rect.Height);
    }
}


