//
// ResizableView.cs
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
using CoreGraphics;
using UIKit;
using Foundation;

namespace Bss.iOS.UIKit
{
    public interface IResizableViewDelegate
    {
        void ResizableViewDidBeginEditing(ResizableView resizableView);
        void ResizableViewDidEndEditing(ResizableView resizableView);
    }
    /// <summary>
    /// Resizable view.
    /// all credits to spoletto
    /// implementation of https://github.com/spoletto/SPUserResizableView
    /// </summary>
    public class ResizableView : UIView
    {
        private AnchorPoint _anchorPoint;
        private GripViewBorderView _borderView;
        private UIView _contentView;
        private CGPoint _touchStart;

        #region Constants
        /* Let's inset everything that's drawn (the handles and the content view)
           so that users can trigger a resize from a few pixels outside of
           what they actually see as the bounding box. */
        public float GlobalInset { get; set; } = 5f;

        public float MinHeight { get; set; } = 48f;

        public float MinWidth { get; set; } = 48f;

        public float InteractiveBorderSize { get; set; } = 10f;
        //AnchorPoint NoResizeAnchorPoint = new AnchorPoint(0, 0, 0, 0);
        //AnchorPoint UpperLeftAnchorPoint = new AnchorPoint(1, 1, -1, 1);
        //AnchorPoint MiddleLeftAnchorPoint = new AnchorPoint(1, 0, 0, 0);
        //AnchorPoint LowerLeftAnchorPoint = new AnchorPoint(1, 0, 1, 1);
        //AnchorPoint UpperMiddleAnchorPoint = new AnchorPoint(0, 1, -1, 0);
        //AnchorPoint UpperRightAnchorPoint = new AnchorPoint(0, 1, -1, -1);
        //AnchorPoint MiddleRightAnchorPoint = new AnchorPoint(0, 0, 1, -1);
        //AnchorPoint LowerRightAnchorPoint = new AnchorPoint(1, 0, 1, 1);
        //AnchorPoint LowerMiddleAnchorPoint = new AnchorPoint(0, 0, 1, 0);
        #endregion

        public ResizableView()
        {
            SetupDefaultAttributes();
        }

        public ResizableView(CGRect frame) : base(frame)
        {
            SetupDefaultAttributes();
        }

        public ResizableView(NSCoder coder) : base(coder)
        {
            SetupDefaultAttributes();
        }

        public override CGRect Frame
        {
            get => base.Frame;
            set
            {
                base.Frame = value;
                if (_contentView != null)
                    _contentView.Frame = Bounds.Inset(GlobalInset + (InteractiveBorderSize / 2f),
                                                      GlobalInset + (InteractiveBorderSize / 2f));
                _borderView.Frame = Bounds.Inset(GlobalInset, GlobalInset);
            }
        }

        public bool IsResizing => _anchorPoint.IsActive;

        public IResizableViewDelegate Delegate { get; set; }

        public UIView ContentView
        {
            get => _contentView;
            set
            {
                if (_contentView != null)
                {
                    if (_contentView == value)
                        return;
                    _contentView.RemoveFromSuperview();
                }
                _contentView = value;
                _contentView.Frame = Bounds.Inset(GlobalInset + (InteractiveBorderSize / 2f),
                                                  GlobalInset + (InteractiveBorderSize / 2f));
                BringSubviewToFront(_borderView);
            }
        }

        public bool PreventsPositionOutsideSuperview { get; set; } = true;

        public bool ShowEditingHandles
        {
            get => !_borderView.Hidden;
            set => _borderView.Hidden = !value;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            Delegate?.ResizableViewDidEndEditing(this);
            _borderView.Hidden = false;

            var touch = touches.ToArray<UITouch>()[0];
            _anchorPoint = AnchorPointForTouchLocation(touch.LocationInView(this));
            _touchStart = touch.LocationInView(IsResizing ? Superview : this);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            var touch = touches.ToArray<UITouch>()[0];
            if (IsResizing)
                ResizeUsingTouchLocation(touch.LocationInView(Superview));
            else
                TranslateUsingTouchLocation(touch.LocationInView(this));
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            Delegate?.ResizableViewDidEndEditing(this);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            Delegate?.ResizableViewDidEndEditing(this);
        }

        private static AnchorPoint AnchorPointForTouchLocation(CGPoint touchPoint)
        {
            //var width = Bounds.Width;
            //var height = Bounds.Height;
            //var upperLeft = new AnchorPointPair(new CGPoint(0, 0), UpperLeftAnchorPoint);
            //var upperMiddle = new AnchorPointPair(new CGPoint(width / 2f, 0), UpperMiddleAnchorPoint);
            //var upperRight = new AnchorPointPair(new CGPoint(width, 0), UpperRightAnchorPoint);
            //var middleRight = new AnchorPointPair(new CGPoint(width, height / 2f), MiddleRightAnchorPoint);
            //var lowerRight = new AnchorPointPair(new CGPoint(width, height), LowerRightAnchorPoint);
            //var lowerMiddle = new AnchorPointPair(new CGPoint(width / 2f, height), LowerMiddleAnchorPoint);
            //var lowerLeft = new AnchorPointPair(new CGPoint(0, height), LowerLeftAnchorPoint);
            //var middleLeft = new AnchorPointPair(new CGPoint(0, height / 2f), MiddleLeftAnchorPoint);
            //var centerPoint = new AnchorPointPair(new CGPoint(width / 2f, height / 2f), NoResizeAnchorPoint);

            //var allPoints = new[] { upperLeft, upperRight, lowerRight, lowerLeft,
            //    upperMiddle, lowerMiddle, middleLeft, middleRight, centerPoint };

            //var smallestDistance = nfloat.MaxValue;
            //var closestPoint = centerPoint;
            //foreach (var item in allPoints)
            //{
            //    var distance = touchPoint.GetDistance(item.Point);
            //    if (distance < smallestDistance)
            //    {
            //        smallestDistance = distance;
            //        closestPoint = item;
            //    }
            //}

            //return closestPoint.AnchorPoint;
            return new AnchorPoint();
        }

        private void ResizeUsingTouchLocation(CGPoint touchPoint)
        {
            // (1) Update the touch point if we're outside the superview.
            if (PreventsPositionOutsideSuperview)
            {
                var border = GlobalInset + InteractiveBorderSize / 2f;
                if (touchPoint.X < border)
                    touchPoint.X = border;
                if (touchPoint.X > Bounds.Width - border)
                    touchPoint.X = Bounds.Width - border;

                if (touchPoint.Y < border)
                    touchPoint.Y = border;
                if (touchPoint.Y > Bounds.Height - border)
                    touchPoint.Y = Bounds.Height - border;
            }

            // (2) Calculate the deltas using the current anchor point.
            var deltaW = _anchorPoint.W * (_touchStart.X - touchPoint.Y);
            var deltaX = _anchorPoint.X * (-1 * deltaW);
            var deltaH = _anchorPoint.H * (touchPoint.Y - _touchStart.Y);
            var deltaY = _anchorPoint.Y * (-1 * deltaH);

            // (3) Calculate the new frame.
            var newX = Frame.X + deltaX;
            var newY = Frame.Y + deltaY;
            var newWidth = Frame.Width + deltaW;
            var newHeight = Frame.Height + deltaH;

            // (4) If the new frame is too small, cancel the changes.
            if (newWidth < MinWidth)
            {
                newWidth = MinWidth;
                newX = Frame.X;
            }
            if (newHeight < MinHeight)
            {
                newHeight = MinHeight;
                newY = Frame.Y;
            }

            // (5) Ensure the resize won't cause the view to move offscreen.
            if (PreventsPositionOutsideSuperview)
            {
                if (newX < Frame.X)
                {
                    // Calculate how much to grow the width by such that the new X coordintae will align with the superview.
                    deltaW = Frame.X - Superview.Bounds.X;
                    newWidth = Bounds.Width + deltaW;
                    newX = Superview.Bounds.X;
                }
                if (newX + newWidth > Superview.Bounds.X + Superview.Bounds.Width)
                {
                    newWidth = Superview.Bounds.Width - newX;
                }
                if (newY < Superview.Bounds.Y)
                {
                    // Calculate how much to grow the height by such that the new Y coordintae will align with the superview.
                    deltaH = Frame.Y - Superview.Bounds.Y;
                    newHeight = Frame.Height + deltaH;
                    newY = Superview.Bounds.Y;
                }
                if (newY + newHeight > Superview.Bounds.Y + Superview.Bounds.Height)
                {
                    newHeight = Superview.Bounds.Height - newY;
                }
            }
            _touchStart = touchPoint;
            Frame = new CGRect(newX, newY, newWidth, newHeight);
        }

        private void TranslateUsingTouchLocation(CGPoint touchPoint)
        {
            var newCenter = new CGPoint(Center.X + touchPoint.X - _touchStart.X,
                                        Center.Y + touchPoint.Y - _touchStart.Y);

            if (!PreventsPositionOutsideSuperview)
            {
                Center = newCenter;
                return;
            }

            var midPointX = Bounds.GetMidX();
            var midPointY = Bounds.GetMidY();
            var offsetX = Bounds.Size.Width - midPointX;
            var offsetY = Bounds.Size.Height - midPointY;
            if (newCenter.X > offsetX)
                newCenter.X = offsetX;
            if (newCenter.X < midPointX)
                newCenter.X = midPointX;
            if (newCenter.Y > offsetY)
                newCenter.Y = offsetY;
            if (newCenter.Y < midPointY)
                newCenter.Y = midPointY;
            Center = newCenter;
        }

        private void SetupDefaultAttributes()
        {
            _borderView = new GripViewBorderView(Bounds.Inset(GlobalInset, GlobalInset))
            {
                Hidden = true,
                BorderSize = InteractiveBorderSize
            };
            AddSubview(_borderView);
        }
        #region Classes & structs

        private struct AnchorPoint
        {
            public AnchorPoint(CGPoint point, AnchorPoint anchorPoint)
            {
                X = point.X;
                Y = point.Y;
                W = anchorPoint.X;
                H = anchorPoint.Y;
            }

            public AnchorPoint(int x, int y, int h, int w)
            {
                X = x;
                Y = y;
                H = h;
                W = w;
            }

            public AnchorPoint(nfloat x, nfloat y, nfloat h, nfloat w)
            {
                X = x;
                Y = y;
                H = h;
                W = w;
            }
            public readonly nfloat X;
            public readonly nfloat Y;
            public readonly nfloat H;
            public readonly nfloat W;

            public bool IsActive => X != 0 || Y != 0 || H != 0 || W != 0;
        }

        private struct AnchorPointPair
        {
            public AnchorPointPair(CGPoint point, AnchorPoint anchorPoint)
            {
                Point = point;
                AnchorPoint = anchorPoint;
            }
            public CGPoint Point;
            public AnchorPoint AnchorPoint;
        }

        private class GripViewBorderView : UIView
        {
            public GripViewBorderView(CGRect frame) : base(frame)
            {
                BackgroundColor = UIColor.Clear;
            }

            public float BorderSize { get; set; } = 10f;

            public override void Draw(CGRect rect)
            {
                var context = UIGraphics.GetCurrentContext();
                context.SaveState();
                // (1) Draw the bounding box.
                context.SetLineWidth(1f);
                context.SetStrokeColor(UIColor.Blue.CGColor);

                // (2) Calculate the bounding boxes for each of the anchor points.
                var offsetX = Bounds.Width - BorderSize;
                var offsetY = Bounds.Height - BorderSize;

                var upperLeft = new CGRect(0, 0, BorderSize, BorderSize);
                var upperRight = new CGRect(offsetX, 0, BorderSize, BorderSize);
                var lowerRight = new CGRect(offsetX, offsetY, BorderSize, BorderSize);
                var lowerLeft = new CGRect(0, offsetY, BorderSize, BorderSize);
                var upperMiddle = new CGRect(offsetX / 2f, 0, BorderSize, BorderSize);
                var lowerMiddle = new CGRect(offsetX / 2f, offsetY / 2f, BorderSize, BorderSize);
                var middleLeft = new CGRect(0, offsetY / 2f, BorderSize, BorderSize);
                var middleRight = new CGRect(offsetX, offsetY / 2, BorderSize, BorderSize);

                // (3) Create the gradient to paint the anchor points.
                var colors = new[]
                {
                    (nfloat)0.4f,0.8f,1.0f,1.0f,
                     0.0f,0.0f,1.0f,1.0f
                };

                var baseSpace = CGColorSpace.CreateDeviceRGB();
                var grandient = new CGGradient(baseSpace, colors, null);
                baseSpace.Dispose();

                // (4) Set up the stroke for drawing the border of each of the anchor points.
                context.SetLineWidth(1f);
                context.SetShadow(new CGSize(0.5f, 0.5f), 1);
                context.SetStrokeColor(UIColor.White.CGColor);

                // (5) Fill each anchor point using the gradient, then stroke the border.
                var allPoints = new[] { upperLeft, upperRight, lowerRight, lowerLeft, upperMiddle,
                    lowerMiddle, middleLeft, middleRight };
                foreach (var item in allPoints)
                {
                    context.SaveState();
                    context.AddEllipseInRect(item);
                    context.Clip();
                    var startPoint = new CGPoint(item.GetMidX(), item.GetMidY());
                    var endPoint = new CGPoint(item.GetMidX(), item.GetMidY());
                    context.DrawLinearGradient(grandient, startPoint, endPoint, CGGradientDrawingOptions.None);
                    context.RestoreState();
                    context.StrokeEllipseInRect(item.Inset(1, 1));
                }

                grandient.Dispose();

                context.RestoreState();
            }
        }
        #endregion
    }
}