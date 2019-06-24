//
// ModalPickerAnimatedDismissed.cs
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
    public class ModalPickerAnimatedDismissed : UIViewControllerAnimatedTransitioning
    {
        public bool IsPresenting { get; set; }
        private float _transitionDuration = 0.25f;

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return _transitionDuration;
        }

        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {

            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

            var screenBounds = UIScreen.MainScreen.Bounds;
            var fromFrame = fromViewController.View.Frame;

            UIView.AnimateNotify(_transitionDuration,
                                 () =>
            {
                toViewController.View.Alpha = 1.0f;

                switch (fromViewController.InterfaceOrientation)
                {
                    case UIInterfaceOrientation.Portrait:
                        fromViewController.View.Frame = new CGRect(0, screenBounds.Height, fromFrame.Width, fromFrame.Height);
                        break;
                    case UIInterfaceOrientation.LandscapeLeft:
                        fromViewController.View.Frame = new CGRect(screenBounds.Width, 0, fromFrame.Width, fromFrame.Height);
                        break;
                    case UIInterfaceOrientation.LandscapeRight:
                        fromViewController.View.Frame = new CGRect(screenBounds.Width * -1, 0, fromFrame.Width, fromFrame.Height);
                        break;
                }

            },
                                 (finished) => transitionContext.CompleteTransition(true));
        }
    }
}

