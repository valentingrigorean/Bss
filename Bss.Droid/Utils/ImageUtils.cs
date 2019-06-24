//
// ImageUtils.cs
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

using Android.Widget;
using Bss.Droid.Views;

namespace Bss.Droid.Utils
{
    public static class ImageUtils
    {
        public static ImageView.ScaleType ConvertFrom(BScaleType scaletype)
        {
            switch (scaletype)
            {
                case BScaleType.Center:
                    return ImageView.ScaleType.Center;
                case BScaleType.CenterCrop:
                    return ImageView.ScaleType.CenterCrop;
                case BScaleType.CenterInside:
                    return ImageView.ScaleType.CenterInside;
                case BScaleType.FitStart:
                    return ImageView.ScaleType.FitStart;
                case BScaleType.FitEnd:
                    return ImageView.ScaleType.FitEnd;
                case BScaleType.FitXy:
                    return ImageView.ScaleType.FitXy;
                case BScaleType.Matrix:
                    return ImageView.ScaleType.Matrix;
                default:
                    return ImageView.ScaleType.FitCenter;
            }
        }


    }
}
