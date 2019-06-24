//
// GradientView.cs
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
using System;
using UIKit;
using CoreAnimation;

namespace Bss.iOS.UIKit
{
    public class GradientView:UIView
    {
        private CAGradientLayer _layer;

        public GradientView()
        {
            Initialiaze();
        }

        public GradientView(IntPtr handle)
            : base(handle)
        {
            Initialiaze();
        }

        public GradientView(UIColor[] colors, float[] locations)
            : this()
        {
            SetGrandient(colors, locations);
        }

        public GradientView(UIColor color1, UIColor color2)
            : this()
        {
            SetGrandient(color1, color2);
        }

        public void SetGrandient(UIColor color1, UIColor color2)
        {
            _layer.SetGradient(color1, color2);
        }

        public void SetGrandient(UIColor[] colors, float[] locations)
        {
            _layer.SetGradient(colors, locations);
        }

        public override CALayer Layer => _layer;

        private void Initialiaze()
        {           
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth |
            UIViewAutoresizing.FlexibleHeight;

            _layer = new CAGradientLayer {Frame = Bounds};
        }
       
    
    }
}

