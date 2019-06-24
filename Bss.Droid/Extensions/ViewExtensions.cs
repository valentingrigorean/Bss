//
// ViewExtensions.cs
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
using Android.Views;
using Bss.Droid.Views;
using Android.Util;
using Android.Content;
using Bss.Droid.Graphics;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bss.Droid.Extensions
{
    public static class ViewExtensions
    {
        public static IEnumerable<View> GetViews(this ViewGroup self)
        {
            var count = self.ChildCount;
            for (var i = 0; i < count; i++)
                yield return self.GetChildAt(i);
        }

        public static void AddRippleEffect(this View view, bool borderless = false)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                var outValue = new TypedValue();
                view.Context.Theme.ResolveAttribute(borderless ?
                                                    Android.Resource.Attribute.SelectableItemBackground :
                                                    Android.Resource.Attribute.SelectableItemBackgroundBorderless,
                                                    outValue, true);
                view.SetBackgroundResource(outValue.ResourceId);
                return;
            }
        }


        public static void SetDrawableColor(this View This, Color color)
        {
            var drawable = This.Background;
            if (drawable == null)
                return;

            switch (drawable)
            {
                case ColorDrawable colorDrawable:
                    colorDrawable.Color = color;
                    break;
                case StateListDrawable stateListDrawable:
                    stateListDrawable.AddState(new int[] { }, new ColorDrawable(color));
                    break;

                case GradientDrawable gradientDrawable:
                    gradientDrawable.Mutate();
                    gradientDrawable.SetColor(color);
                    break;
            }
        }

        public static void SetDrawable(this View This, params KeyValuePair<int[], Drawable>[] args)
        {
            var drawable = new StateListDrawable();
            foreach (var pair in args)
                drawable.AddState(pair.Key, pair.Value);
            This.Background = drawable;
        }

        public static void SetDrawable(this View This, params KeyValuePair<int, Drawable>[] args)
        {
            var e = args.Select(arg => new KeyValuePair<int[], Drawable>(new[] { arg.Key }, arg.Value)).ToArray();
            SetDrawable(This, e);
        }

        public static void SetDrawableColor(this View This, params KeyValuePair<int[], Color>[] args)
        {
            var e = args.Select(
                arg => new KeyValuePair<int[], Drawable>(arg.Key, new ColorDrawable(arg.Value))).ToArray();
            SetDrawable(This, e);
        }

        public static void SetDrawableColor(this View This, params KeyValuePair<int, Color>[] args)
        {
            SetDrawableColor(This, args.Select(
                arg => new KeyValuePair<int[], Color>(new[] { arg.Key }, arg.Value)).ToArray());
        }

        public static void HideKeyboard(this View view)
        {
            if (view == null || !view.IsFocused)
                return;
            var imm = (Android.Views.InputMethods.InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            //Find the currently focused view, so we can grab the correct window token from it.           
            //If no view currently has focus, create a new one, just so we can grab a window token from it
            imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        public static void OnLayout(this View view, Action action)
        {
            var layoutLisener = new GlobalLayoutListener(view, true);
            layoutLisener.GlobalLayout += (sender, e) =>
            {
                action?.Invoke();
                layoutLisener = null;
            };
        }

        public static BSize GetDefaultSize(this View view)
        {
            view.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
            return new BSize(view.MeasuredWidth, view.MeasuredHeight);
        }

        public static BSize GetSizeWithDensityScaledDown(this View view)
        {
            var width = (int)(view.Width / view.Resources.DisplayMetrics.Density);
            var height = (int)(view.Height / view.Resources.DisplayMetrics.Density);
            return new BSize(width, height);
        }

        public static T GetViewWithType<T>(this View view)
            where T : View
        {
            var viewGrp = view as ViewGroup;
            if (viewGrp == null)
                return default(T);
            for (var i = 0; i < viewGrp.ChildCount; i++)
            {
                var subview = viewGrp.GetChildAt(i);
                if (subview is T)
                    return (T)subview;
                var inView = GetViewWithType<T>(subview);
                if (inView != null && inView is T)
                    return inView;
            }

            return default(T);
        }

        public static void DebugBounds(this View view, string tag)
        {
            Log.Debug(tag, "width:{0} height:{1}".Format(view.Width, view.Height));
        }

        public static View[] GetSubviews(this ViewGroup view)
        {
            var arr = new View[view.ChildCount];
            for (var i = 0; i < view.ChildCount; i++)
                arr[i] = view.GetChildAt(i);
            return arr;
        }

        public static void SetChildVisibility(this View self, ViewStates viewStates)
        {
            if (self is ViewGroup viewGroup)
            {
                for (var i = 0; i < viewGroup.ChildCount; i++)
                {
                    var view = viewGroup.GetChildAt(i);
                    view.Visibility = viewStates;
                }
            }
        }

        public static void SetRoundCorners(this View This, Color background, float radiiTop, float radiiRight, float radiiBottom, float radiiLeft)
        {
            setRoundCorners(This, radiiTop, radiiRight, radiiBottom, radiiLeft, background);
        }

        public static void SetRoundCorners(this View This, float radiiTop, float radiiRight, float radiiBottom, float radiiLeft)
        {
            setRoundCorners(This, radiiTop, radiiRight, radiiBottom, radiiLeft, Color.Transparent);
        }

        public static void SetRoundCorners(this View This, float radius)
        {
            setRoundCorners(This, radius, Color.Transparent);
        }

        public static void SetRoundCorners(this View This, float radius, Color background)
        {
            setRoundCorners(This, radius, background);
        }

        private static void setRoundCorners(View view, float radius, Color background)
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetCornerRadius(radius);
            shape.SetColor(background);
            view.Background = shape;
        }

        private static void setRoundCorners(View view, float radiiTop, float radiiRight, float radiiBottom, float radiiLeft, Color background)
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetCornerRadii(new float[] { radiiTop, radiiTop, radiiRight, radiiRight, radiiBottom, radiiBottom, radiiLeft, radiiLeft });
            shape.SetColor(background);
            view.Background = shape;
        }  
    }
}
