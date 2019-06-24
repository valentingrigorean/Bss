//
// UIViewExtension.Grandient.cs
//
// Author:
//       Valentin <v.grigorean@bss-one.net>
//
// Copyright (c) 2016 Valentin
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
using System.Linq;
using CoreAnimation;
using CoreGraphics;

namespace UIKit
{

    public class BorderGrandientConfig
    {
        public CGColor StartColor { get; set; }

        public CGColor EndColor { get; set; }

        public CGPoint StartPoint { get; set; }

        public CGPoint EndPoint { get; set; }

        public nfloat BorderWidth { get; set; } = 1f;

        public enum Orientation
        {
            Vertical,
            Horizontal
        }

        public static BorderGrandientConfig Create(UIColor color1, UIColor color2,
                                                   Orientation orientation)
        {
            var border = new BorderGrandientConfig
            {
                StartColor = color1.CGColor,
                EndColor = color2.CGColor
            };

            switch (orientation)
            {
                case Orientation.Vertical:
                    border.StartPoint = new CGPoint(0f, 0.0f);
                    border.EndPoint = new CGPoint(1.0f, 1.0f);
                    break;
                case Orientation.Horizontal:
                    border.StartPoint = new CGPoint(0f, 0.5f);
                    border.EndPoint = new CGPoint(1.0f, 0.5f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
            return border;
        }
    }

    public static partial class UIViewExtension
    {
        /// <summary>
        /// Adds the gradient color border.
        /// Should be called after ViewDidLayoutSubviews
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="config">Config.</param>
        public static void AddGradientBorder(this UIView view, BorderGrandientConfig config)
        {
            view.Layer.MasksToBounds = true;
            view.Layer.BorderWidth = 0f;

            var grandientLayer = new CAGradientLayer();
            var bounds = new CGRect(CGPoint.Empty, view.Frame.Size);
            grandientLayer.Frame = bounds;
            grandientLayer.StartPoint = config.StartPoint;
            grandientLayer.EndPoint = config.EndPoint;
            grandientLayer.Colors = new[] { config.StartColor, config.EndColor };

            var shapeLayer = new CAShapeLayer
            {
                LineWidth = config.BorderWidth,
                FillColor = null,
                StrokeColor = UIColor.Black.CGColor,
                Path = UIBezierPath.FromRoundedRect(bounds, view.Layer.CornerRadius).CGPath
            };

            grandientLayer.Mask = shapeLayer;
            view.Layer.AddSublayer(grandientLayer);
        }

        /// <summary>
        /// Adds the gradient. 
        /// Will use view bounds will not resize on orientation change
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="color1">Color1 is top color</param>
        /// <param name="color2">Color2 is bottom color</param>
        public static CAGradientLayer AddGradient(this UIView view, UIColor color1, UIColor color2)
        {
            return AddGradient(view, new[] { color1, color2 }, new[] { 0.0f, 1.0f });
        }


        /// <summary>
        /// Adds the gradient. 
        /// Will use view bounds will not resize on orientation change
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="settings">Settings.</param>
        public static CAGradientLayer AddGradient(this UIView view, IDictionary<UIColor, float> settings)
        {
            var colors = settings.Keys.ToArray();
            var locations = settings.Values.ToArray();
            return AddGradient(view, colors, locations);
        }

        /// <summary>
        /// Adds the gradient. 
        /// Will use view bounds will not resize on orientation change
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="colors">Colors used from top to bottom.</param>
        /// <param name="locations">Locations used from top to bottom</param>
        public static CAGradientLayer AddGradient(this UIView view, UIColor[] colors, float[] locations)
        {
            var gradient = new CAGradientLayer().SetGradient(colors, locations);
            gradient.Frame = view.Bounds;
            view.Layer.InsertSublayer(gradient, 0);
            return gradient;
        }
    }
}

