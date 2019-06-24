//
// UIViewExtension.cs
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
using Bss.iOS.CoreAnimation;
using Bss.iOS.Extensions;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using System.IO;

namespace UIKit
{
    public static partial class UIViewExtension
    {
        public readonly static CGSize DefaultShadowOffset = new CGSize(0, 2);

        public static void PinToParent(this UIView This, UIView parent, float padding = 0f)
        {
            if ((This.Superview != null && This.Superview != parent) || This.Superview == null)
            {
                This.RemoveFromSuperview();
                parent.AddSubview(This);
            }

            PinToParent(This, padding);
        }

        public static void PinToParent(this UIView This, float padding = 0f)
        {
            if (This.Superview == null)
                return;

            var parent = This.Superview;

            This.TranslatesAutoresizingMaskIntoConstraints = false;

            This.TrailingAnchor.ConstraintEqualTo(parent.TrailingAnchor, padding).Active = true;
            This.LeadingAnchor.ConstraintEqualTo(parent.LeadingAnchor, padding).Active = true;
            This.TopAnchor.ConstraintEqualTo(parent.TopAnchor, padding).Active = true;
            This.BottomAnchor.ConstraintEqualTo(parent.BottomAnchor, padding).Active = true;
        }


        public static void DisenableUITapGesture(this UIView self)
        {
            var tap = self.GestureRecognizers.FirstOrDefault(g => g is UITapGestureRecognizer);
            if (tap != null)
                tap.Enabled = false;
        }

        public static void EnableUITapGesture(this UIView self)
        {
            var tap = self.GestureRecognizers.FirstOrDefault(g => g is UITapGestureRecognizer);
            if (tap != null)
                tap.Enabled = true;
        }

        public static void AddDropShadow(this UIView view, UIColor color, CGSize offset, float radius = 1f, float opacity = 0.8f)
        {
            view.Layer.ShadowColor = color.CGColor;
            view.Layer.ShadowOffset = offset;
            view.Layer.ShadowRadius = radius;
            view.Layer.ShadowOpacity = opacity;
            view.Layer.MasksToBounds = false;
        }

        public static void ChangeFrame(this UIView This, nfloat x, nfloat y)
        {
            var frame = This.Frame;
            frame.X = x;
            frame.Y = y;
            This.Frame = frame;
        }

        public static void ChangeFrameX(this UIView This, nfloat x)
        {
            ChangeFrame(This, x, This.Frame.Y);
        }

        public static void ChangeFrameY(this UIView This, nfloat y)
        {
            ChangeFrame(This, This.Frame.X, y);
        }

        public static PulseEffect AddPulseEffect(this UIView This)
        {
            var layer = new PulseEffect.Builder().Create();
            This.Layer.AddSublayer(layer);
            return layer;
        }

        public static PulseEffect AddPulseEffect(this UIView This, UIColor color)
        {
            var layer = new PulseEffect
                .Builder()
                .SetFillColor(color)
                .Create();
            This.Layer.AddSublayer(layer);
            return layer;
        }

        public static PulseEffect AddPulseEffect(this UIView This, float radius, UIColor color)
        {
            var layer = new PulseEffect
                .Builder()
                .SetRadius(radius)
                .SetFillColor(color)
                .Create();
            This.Layer.AddSublayer(layer);
            return layer;
        }

        public static UIImage GetImageFromView(this UIView view, bool opaque = false)
        {
            UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, opaque, 0.0f);
            view.DrawViewHierarchy(view.Bounds, true);
            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return img;
        }

        public static async System.Threading.Tasks.Task GetImagePathFromView(this UIView view, UIViewController parent, Action<UIImage, string> callback, bool opaque = false, int maxSize = 1024)
        {
            var resizedImage = view.GetImageFromView(opaque).Resize(maxSize, BContentMode.ScaleToFit);

            var documentsDirectory = Environment.GetFolderPath
                     (Environment.SpecialFolder.MyDocuments);
            documentsDirectory = Path.Combine(documentsDirectory, "..", "tmp");

            var jpgFilename = Path.Combine(documentsDirectory, $"graph_{DateTime.Now.ToString("yyyyMd_HHms")}" + ".jpg");

            await resizedImage.SaveToFileAsync(jpgFilename, () =>
            {
                var fileExists = File.Exists(jpgFilename);
                parent.InvokeOnMainThread(() =>
                {
                    callback?.Invoke(resizedImage, fileExists ? jpgFilename : "");
                });
            }, ImageExtension.JPG);
        }

        public static void SetCornerRadius(this UIView view, nfloat radius)
        {
            view.Layer.MasksToBounds = true;
            view.Layer.CornerRadius = radius;
        }

        public static void SetCornerRadius(this UIView view, nfloat radius, UIRectCorner corners)
        {
            SetCornerRadius(view, new CGSize(radius, radius), corners);
        }

        public static void SetCornerRadius(this UIView view, CGSize radii, UIRectCorner corners)
        {
            var rounded = UIBezierPath.FromRoundedRect(view.Bounds, corners, radii);
            var shape = new CAShapeLayer { Path = rounded.CGPath };
            view.Layer.Mask = shape;
            view.Layer.MasksToBounds = true;
        }

        public static void AsCircle(this UIView view, bool maskToBounds = true)
        {
            view.LayoutIfNeeded();

            AsCircleInternal(view, maskToBounds);
        }

        private static void AsCircleInternal(UIView view, bool maskToBounds)
        {
            var width = (int)view.Bounds.Width;
            var height = (int)view.Bounds.Height;
            if (width != height)
                throw new Exception("Can't make circle view height != width");
            view.Layer.MasksToBounds = maskToBounds;
            view.Layer.CornerRadius = view.Bounds.Width / 2f;
        }

        public static void ClearSublayer(this UIView view)
        {
            view.Layer.Sublayers = null;
        }

        #region Border

        public static CALayer GetGradientLayer(this UIView view, UIColor colorStart, UIColor colorEnd)
        {
            var gradient = new CAGradientLayer();
            gradient.Frame = view.Bounds;
            gradient.StartPoint = new CGPoint(0, 0.5);
            gradient.EndPoint = new CGPoint(1, 0.5);
            gradient.Colors = new CGColor[2] { colorStart.CGColor, colorEnd.CGColor };
            return gradient;
        }

        public static CALayer GetGradientLayer(this UIView view, UIColor colorStart, UIColor colorEnd, float cornerRadius = 1f)
        {
            var gradient = view.GetGradientLayer(colorStart, colorEnd);
            gradient.CornerRadius = cornerRadius;
            return gradient;
        }

        public static CALayer GetGradientLayer(this UIView view, UIColor colorStart, UIColor colorEnd, float cornerRadius = 1f, CAShapeLayer shape = null)
        {
            var gradient = view.GetGradientLayer(colorStart, colorEnd);
            gradient.Mask = shape;
            return gradient;
        }

        public static CALayer GetGradientLayer(this UIView view, UIColor colorStart, UIColor colorEnd, UIColor colorBorder, float borderWidth = 1f, float cornerRadius = 1f)
        {
            var gradient = view.GetGradientLayer(colorStart, colorEnd, cornerRadius);
            gradient.BorderColor = (colorBorder == null ? UIColor.Clear : colorBorder).CGColor;
            gradient.BorderWidth = borderWidth;
            return gradient;
        }

        public static void SetGradient(this UIView view, CGRect rect, UIColor colorStart, UIColor colorEnd, CAShapeLayer shape = null, float cornerRadius = 1f)
        {
            var gradient = view.GetGradientLayer(colorStart, colorEnd, cornerRadius, shape);
            view.Layer.MasksToBounds = true;
            view.Layer.AddSublayer(gradient);
        }


        private static void SetGradientBorder(this UIView view, CGRect rect, UIColor colorStart, UIColor colorEnd, float cornerRadius = 1f, float line = 3)
        {
            var shape = new CAShapeLayer();
            shape.LineWidth = line;
            shape.Path = UIBezierPath.FromRoundedRect(rect, cornerRadius).CGPath;
            shape.StrokeColor = UIColor.Black.CGColor;
            shape.FillColor = UIColor.Clear.CGColor;
            shape.CornerRadius = cornerRadius;
            SetGradient(view, rect, colorStart, colorEnd, shape, cornerRadius);
        }

        public static void SetBorder(this UIView view, UIColor colorStart, UIColor colorEnd, float cornerRadius = 1f, float line = 3)
        {
            SetGradientBorder(view, view.Bounds, colorStart, colorEnd, cornerRadius, line);
        }

        public static void SetBorder(this UIView view, UIColor color, float borderWidth = 1f)
        {
            view.Layer.BorderWidth = borderWidth;
            view.Layer.BorderColor = color.CGColor;
        }

        public static void SetBorderBottom(this UIView view, UIColor color, float borderWidth = 1f)
        {
            var border = new CALayer();
            border.BackgroundColor = color.CGColor;

            // Version from Ioana - Monitora project - added 400
            //border.Frame = new CGRect(x: 0, y: view.Layer.Frame.Size.Height - borderWidth, width: view.Layer.Frame.Size.Width, height: borderWidth);
            border.Frame = new CGRect(x: 0, y: view.Layer.Frame.Size.Height - borderWidth, width: view.Layer.Frame.Size.Width + 400, height: borderWidth);

            view.Layer.AddSublayer(border);
        }

        public static void SetBorderBottom(this UIView view, UIColor colorStart, UIColor colorEnd)
        {
            SetGradient(view, view.Bounds, colorStart, colorEnd);
        }

        public static void SetBorderTop(this UIView view, UIColor color, float borderWidth = 1f)
        {
            var border = new CALayer();
            border.BackgroundColor = color.CGColor;
            border.Frame = new CGRect(x: 0, y: 0, width: view.Layer.Frame.Size.Width, height: borderWidth);

            view.Layer.AddSublayer(border);
        }

        #endregion
        public static void SetShadowRound(this UIView view, UIColor backgroundColor, UIColor shadowColor)
        {
            view.BackgroundColor = backgroundColor;
            view.SetShadow(shadowColor, 0.5f, 4.0f);
        }
        public static void SetShadowRound(this UIView view, UIColor backgroundColor)
        {
            view.BackgroundColor = backgroundColor;
            view.SetShadow(UIColor.DarkGray, 0.5f, 4.0f);
        }

        public static void SetShadowRound(this UIView view)
        {
            // Version from Ioana - Monitora project (removed line)
            //view.BackgroundColor = UIColor.White;
            view.SetShadow(UIColor.DarkGray, 0.5f, 4.0f);
        }

        public static void SetShadowBottom(this UIView view)
        {
            view.SetShadow(UIColor.DarkGray, 1f, 0f);
        }

        private static void SetShadow(this UIView view, UIColor color, float opacity, nfloat radius)
        {
            view.Layer.ShadowColor = color.CGColor;
            view.Layer.ShadowOffset = new System.Drawing.SizeF(0, 2);
            view.Layer.ShadowOpacity = opacity;
            view.Layer.ShadowRadius = radius;
            view.Layer.ShouldRasterize = false;
            view.Layer.MasksToBounds = false;
        }

        /// <summary>
        /// Adds the border.
        /// With black color
        /// </summary>
        /// <returns>The border.</returns>
        /// <param name="view">View.</param>
        /// <param name="borderWidth">Border width.</param>
        public static void SetBorder(this UIView view, float borderWidth = 1f)
        {
            SetBorder(view, UIColor.Black, borderWidth);
        }

        public static float GetRotation(this UIView view)
        {
            var number = view.ValueForKeyPath(new NSString("layer.transform.rotation.z")) as NSNumber;
            if (number != null) return (float)(number.FloatValue * (180 / Math.PI));
            return 0;
        }

        public static CGRect RelativeRectToScreen(this UIView view)
        {
            var globalPoint = view.Superview?.ConvertPointToView(view.Frame.Location, null) ?? view.ConvertPointToView(view.Frame.Location, null);
            return new CGRect(globalPoint, view.Bounds.Size);
        }

        /// <summary>
        /// Raises the click event.
        /// Will set UserInteraction On
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="action">Action.</param>
        /// <param name="tapsRequired">Taps required.</param>
        public static UITapGestureRecognizer OnClick(this UIView view, Action action,
                                   uint tapsRequired = 1)
        {
            view.UserInteractionEnabled = true;
            var tap = new UITapGestureRecognizer(action)
            {
                NumberOfTapsRequired = tapsRequired
            };
            view.AddGestureRecognizer(tap);
            return tap;
        }

        public static UITapGestureRecognizer OnClick(this UIView view, Action<UITapGestureRecognizer> action,
                                   uint tapsRequired = 1)
        {
            view.UserInteractionEnabled = true;
            var tap = new UITapGestureRecognizer(action)
            {
                NumberOfTapsRequired = tapsRequired
            };
            view.AddGestureRecognizer(tap);
            return tap;
        }

        public static UILongPressGestureRecognizer OnLongClick(this UIView view, Action<UILongPressGestureRecognizer> action)
        {
            view.UserInteractionEnabled = true;
            var tap = new UILongPressGestureRecognizer(action)
            {
                NumberOfTapsRequired = 0
            };
            view.AddGestureRecognizer(tap);
            return tap;
        }

        /// <summary>
        /// Removes the on click event create with OnClick
        /// Caution: will remove all UITapGestureRecognizer
        /// </summary>
        /// <param name="view">View.</param>
        public static void RemoveOnClick(this UIView view)
        {
            if (view.GestureRecognizers.Length == 0) return;
            if (view.GestureRecognizers.Length == 1)
                view.UserInteractionEnabled = false;

            view.GestureRecognizers = view.GestureRecognizers.Where(_ =>
            {
                if (_ is UITapGestureRecognizer)
                    return false;
                return true;
            }).ToArray();
        }

        public static void OnSwipe(this UIView view, Action<UISwipeGestureRecognizer> action)
        {
            view.UserInteractionEnabled = true;
            var swipeDirectionR = new UISwipeGestureRecognizer(action) { Direction = UISwipeGestureRecognizerDirection.Right };
            var swipeDirectionL = new UISwipeGestureRecognizer(action) { Direction = UISwipeGestureRecognizerDirection.Left };
            view.AddGestureRecognizer(swipeDirectionR);
            view.AddGestureRecognizer(swipeDirectionL);
        }

        public static void SetAnchorPoint(this UIView view, CGPoint anchorPoint)
        {
            var width = view.Bounds.Size.Width;
            var height = view.Bounds.Size.Height;
            var oldAnchorPoint = view.Layer.AnchorPoint;
            var newPoint = new CGPoint(width * anchorPoint.X, height * anchorPoint.Y);
            var oldPoint = new CGPoint(width * oldAnchorPoint.X, height * oldAnchorPoint.Y);

            var position = view.Layer.Position;

            newPoint = view.Transform.TransformPoint(newPoint);
            oldPoint = view.Transform.TransformPoint(oldPoint);

            position.X -= oldPoint.X;
            position.X += newPoint.X;

            position.Y -= oldPoint.Y;
            position.Y += newPoint.Y;

            view.Layer.Position = position;
            view.Layer.AnchorPoint = anchorPoint;
        }

        public static ICollection<NSLayoutConstraint> GetAllConstraints(this UIView This)
        {
            var list = new List<NSLayoutConstraint>();
            var superView = This.Superview;
            while (superView != null)
            {
                list.AddRange(superView.Constraints.Where(_ => _.FirstItem == This || _.SecondItem == This));
                superView = superView.Superview;
            }
            return list;
        }

        public static IDictionary<UIView, NSLayoutConstraint[]> GetAllConstraintWithParent(this UIView This)
        {
            var list = new Dictionary<UIView, NSLayoutConstraint[]>();
            if (!This.Constraints.IsNullOrEmpty())
                list.Add(This, This.Constraints);
            var superView = This.Superview;
            while (superView != null)
            {
                var constraints = superView.Constraints.Where(_ => _.FirstItem == This || _.SecondItem == This).ToArray();
                if (constraints.Length > 0)
                    list.Add(superView, constraints);
                superView = superView.Superview;
            }
            return list;
        }

        public static T GetViewWithType<T>(this UIView view)
            where T : UIView
        {
            var t = view as T;
            return t ?? view.Subviews.Select(GetViewWithType<T>).FirstOrDefault(res => res != null);
        }

        public static UIView GetFirstResponder(this UIView This)
        {
            return This.Subviews
                       .Select(GetFirstResponder)
                       .FirstOrDefault(v => v != null && v.IsFirstResponder);
        }

        public static void Hide(this UIView view)
        {
            if (view == null)
                return;
            view.HeightAnchor.ConstraintEqualTo(1).Active = true;
            view.Hidden = true;
        }

        public static void ClearSubviews(this UIView superview)
        {
            if (superview == null || superview.Subviews.Length == 0)
                return;

            foreach (UIView subview in superview)
                subview.RemoveFromSuperview();
        }

        public static void AddView(this UIView superview, UIView view)
        {
            if (superview == null || view == null)
                return;
            superview.ClearSubviews();
            superview.AddSubview(view);
        }
    }
}