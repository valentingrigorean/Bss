//
// NextResponderTextField.cs
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
using System.ComponentModel;
using Foundation;
using CoreGraphics;


namespace Bss.iOS.UIKit
{
    /// <summary>
    /// Credit to mohamede1945
    /// https://github.com/mohamede1945/NextResponderTextField/blob/master/Pod/Classes/NextResponderTextField.swift
    /// </summary>
    [Register("NextResponderTextField"), DesignTimeVisible(true)]
    public class NextResponderTextField : UITextField, INextResponder
    {
        public NextResponderTextField()
        {
            Initialiaze();
        }

        public NextResponderTextField(IntPtr handle)
            : base(handle)
        {
            Initialiaze();
        }

        public NextResponderTextField(NSCoder coder)
            : base(coder)
        {
            Initialiaze();
        }

        public NextResponderTextField(CGRect frame)
            : base(frame)
        {
            Initialiaze();
        }

        public NextResponderTextField(NSObjectFlag t)
            : base(t)
        {
            Initialiaze();
        }

        [Outlet("nextResponderField")]
        public UIResponder NextResponderField { get; set; }

        private void ActionButtonTapped(object sender, EventArgs e)
        {
            if (NextResponderField == null)
            {
                ResignFirstResponder();
                return;
            }
            var btn = NextResponderField as UIButton;
            if (btn != null)
            {
                if (btn.Enabled)
                    btn.SendActionForControlEvents(UIControlEvent.TouchUpInside);
                else
                    ResignFirstResponder();
                return;
            }
            var clickableView = NextResponderField as IClickableView;
            if (clickableView != null)
            {
                if (clickableView.Enabled)
                    clickableView.SendActionForControlEvent();
                else
                    ResignFirstResponder();
                return;
            }
            NextResponderField.BecomeFirstResponder();
        }

        private void Initialiaze()
        {
            AddTarget(ActionButtonTapped, UIControlEvent.EditingDidEndOnExit);
        }
    }
}

