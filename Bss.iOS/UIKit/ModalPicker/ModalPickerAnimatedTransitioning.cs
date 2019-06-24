//
// ModalPickerAnimatedTransitioning.cs
//
// Author:
//       Songurov Fiodor <f.songurov@software-dep.net>
//
// Copyright (c) 2016 Songurov
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


using UIKit;
using CoreGraphics;

namespace Bss.iOS.UIKit
{
    public class ModalPickerAnimatedTransitioning : UIViewControllerAnimatedTransitioning
    {
        public bool IsPresenting { get; set; }

        private float _transitionDuration = 0.25f;

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return _transitionDuration;
        }

        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var inView = transitionContext.ContainerView;

            var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);

            inView.AddSubview(toViewController.View);

            toViewController.View.Frame = CGRect.Empty;

            var startingPoint = GetStartingPoint(fromViewController.InterfaceOrientation);
            if (fromViewController.InterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                toViewController.View.Frame = new CGRect(startingPoint.X, startingPoint.Y,
                                                             fromViewController.View.Frame.Width,
                                                             fromViewController.View.Frame.Height);
            }
            else
            {
                toViewController.View.Frame = new CGRect(startingPoint.X, startingPoint.Y,
                                                             fromViewController.View.Frame.Height,
                                                             fromViewController.View.Frame.Width);
            }

            UIView.AnimateNotify(_transitionDuration,
                                 () =>
            {
                var endingPoint = GetEndingPoint(fromViewController.InterfaceOrientation);
                toViewController.View.Frame = new CGRect(endingPoint.X, endingPoint.Y, fromViewController.View.Frame.Width,
                                                                 fromViewController.View.Frame.Height);
                fromViewController.View.Alpha = 0.5f;
            },
                                 (finished) => transitionContext.CompleteTransition(true)
                                );
        }

        private CGPoint GetStartingPoint(UIInterfaceOrientation orientation)
        {
            var screenBounds = UIScreen.MainScreen.Bounds;
            var coordinate = CGPoint.Empty;
            switch (orientation)
            {
                case UIInterfaceOrientation.Portrait:
                    coordinate = new CGPoint(0, screenBounds.Height);
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                    coordinate = new CGPoint(screenBounds.Width, 0);
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    coordinate = new CGPoint(screenBounds.Width * -1, 0);
                    break;
                default:
                    coordinate = new CGPoint(0, screenBounds.Height);
                    break;
            }

            return coordinate;
        }

        private CGPoint GetEndingPoint(UIInterfaceOrientation orientation)
        {
            var screenBounds = UIScreen.MainScreen.Bounds;
            var coordinate = CGPoint.Empty;
            switch (orientation)
            {
                case UIInterfaceOrientation.Portrait:
                    coordinate = new CGPoint(0, 0);
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                    coordinate = new CGPoint(0, 0);
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    coordinate = new CGPoint(0, 0);
                    break;
                default:
                    coordinate = new CGPoint(0, 0);
                    break;
            }

            return coordinate;
        }
    }
}

