﻿//
// BoundsProxyAnimator.cs
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

using Android.Views;
using Java.Interop;
using Bss.Droid.Graphics;

namespace Bss.Droid.Anim
{
    public class BoundsProxyAnimator : BaseProxyAnimator
    {
        public BoundsProxyAnimator(View view) : base(view)
        {

        }

        public const string PropertyName = nameof(Bounds);

        public BSize Bounds
        {
            [Export("getBounds")]
            get
            {
                return new BSize(ContentView.LayoutParameters.Width, ContentView.LayoutParameters.Height);
            }
            [Export("setBounds")]
            set
            {
                var lp = ContentView.LayoutParameters;
                lp.Width = value.Width;
                lp.Height = value.Height;
                ContentView.RequestLayout();
            }
        }

    }
}
