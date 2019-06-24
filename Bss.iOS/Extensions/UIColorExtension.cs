//
// UIColorExtension.cs
//
// Author:
//       Valentin <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 Valentin Grigorean
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

// Analysis disable once CheckNamespace
using CoreGraphics;
namespace UIKit
{
    public static class UiColorExtension
    {

        public static UIImage GetImageForColor(this UIColor color)
        {
            var imageSize = new CGSize(30, 30);
            var imageSizeRectF = new CGRect(0, 0, 30, 30);

            UIGraphics.BeginImageContextWithOptions(imageSize, false, 0);
            var context = UIGraphics.GetCurrentContext();

            context.SetFillColor(color.CGColor);
            context.FillRect(imageSizeRectF);

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }

        /// <summary>
        /// Gets the inverse.
        /// </summary>
        /// <returns>The inverse.</returns>
        /// <param name="color">Color.</param>
        /// <param name="bw">If set to <c>true</c> bw will return white or black.</param>
        public static UIColor GetInverse(this UIColor color, bool bw = true)
        {
            nfloat r, g, b, a;
            color.GetRGBA(out r, out g, out b, out a);
            if (bw)
                return (r * 0.299 + g * 0.587 + b * 0.114) > 186 ?
                    UIColor.FromRGB(0, 0, 0) : UIColor.FromRGB(0xff, 0xff, 0xff);
            r = 255 - r;
            g = 255 - g;
            b = 255 - b;
            return UIColor.FromRGBA(r, g, b, a);
        }

        public static UIColor FromHex(this UIColor color, string hex, float alpha = 1f)
        {
            var colorString = hex.Replace("#", "");
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }

            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    red = Convert.ToInt32(string.Format("{0}{0}",
                            colorString.Substring(0, 1)), 16) / 255f;
                    green = Convert.ToInt32(string.Format("{0}{0}",
                            colorString.Substring(1, 1)), 16) / 255f;
                    blue = Convert.ToInt32(string.Format("{0}{0}",
                            colorString.Substring(2, 1)), 16) / 255f;
                    return UIColor.FromRGBA(red, green, blue, alpha);
                case 6: // #RRGGBB
                    red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                    green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                    blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                    return UIColor.FromRGBA(red, green, blue, alpha);
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Invalid color value {hex} is invalid. " + "It should be a hex value of the form #RBG," +
                        " #RRGGBB");

            }
        }

        public static bool AreEqual(this UIColor color1, UIColor color2)
        {
            return color1.ToHex() == color2.ToHex();
        }

        public enum AlphaChannel
        {
            First,
            Last,
            None
        }

        public static string ToHex(this UIColor color, AlphaChannel alpha = AlphaChannel.None)
        {
            nfloat r, g, b, a;
            color.GetRGBA(out r, out g, out b, out a);
            var ir = (int)(r * 255);
            var ig = (int)(g * 255);
            var ib = (int)(b * 255);
            var ia = (int)(a * 255);
            var str = $"{ir.ToString("X2")}{ig.ToString("X2")}{ib.ToString("X2")}";
            if (alpha == AlphaChannel.None) return str;
            return alpha == AlphaChannel.First ? $"{ia.ToString("X2")}" + str :
                                        str + $"{ia.ToString("X2")}";
        }

        /// <summary>
        /// Gets the color of the close.
        /// 
        /// </summary>
        /// <returns>The close color.</returns>
        /// <param name="color">Color.</param>
        /// <param name="dev">Dev this will be added to red,blue,green.</param>
        public static UIColor GetCloseColor(this UIColor color, float dev)
        {
            nfloat r, g, b, a;
            color.GetRGBA(out r, out g, out b, out a);
            r = (nfloat)Math.Max(0, Math.Min(r + dev, 255f));
            g = (nfloat)Math.Max(0, Math.Min(g + dev, 255f));
            b = (nfloat)Math.Max(0, Math.Min(b + dev, 255f));
            return UIColor.FromRGBA(r, g, b, a);
        }
    }
}

