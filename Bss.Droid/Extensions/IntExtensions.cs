//
// IntExtensions.cs
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
using Android.App;
using Android.Content.Res;
using Android.Util;

public static class IntExtensions
{
    public static string GetStringResource(this int id)
    {
        var context = Application.Context;
        if (context == null)
            return "";
        return context.Resources.GetString(id);
    }

    public static int Clamp(this int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public static int FromDp(this int dp)
    {
        return (int)Math.Round(TypedValue.ApplyDimension(
                ComplexUnitType.Px, dp, Resources.System.DisplayMetrics));
    }


    public static int PixelToDp(this int px)
    {
        if (Application.Context == null)
            return px;
        var res = Application.Context.Resources;
        var metrics = res.DisplayMetrics;
        var dp = px / ((int)metrics.DensityDpi / (int)DisplayMetricsDensity.Default);
        return dp;
    }

    public static int DpToPixel(this double dp)
    {
        if (Application.Context == null)
            return (int)dp;
        var res = Application.Context.Resources;
        var metrics = res.DisplayMetrics;
        var px = dp * ((int)metrics.DensityDpi / (int)DisplayMetricsDensity.Default);
        return (int)px;
    }

    public static int DpToPixel(this int dp)
    {
        if (Application.Context == null)
            return dp;
        var res = Application.Context.Resources;
        var metrics = res.DisplayMetrics;
        var px = dp * ((int)metrics.DensityDpi / (int)DisplayMetricsDensity.Default);
        return px;
    }

    public static int DpToPixel(this float dp)
    {
        if (Application.Context == null)
            return (int)dp;
        var res = Application.Context.Resources;
        var metrics = res.DisplayMetrics;
        var px = dp * ((int)metrics.DensityDpi / (int)DisplayMetricsDensity.Default);
        return (int)px;
    }
}

