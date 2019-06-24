﻿//
// ModalPickerDimissedEventHandler.cs
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


using System;
using UIKit;
using CoreGraphics;

namespace Bss.iOS.UIKit
{
    public delegate void ModalPickerDimissedEventHandler(object sender, EventArgs e);

    public class ModalPickerViewController : UIViewController
    {
        public event ModalPickerDimissedEventHandler OnModalPickerDismissed;
        private const float _headerBarHeight = 40;

        public UIColor HeaderBackgroundColor { get; set; }
        public UIColor HeaderTextColor { get; set; }
        public string HeaderText { get; set; }
        public string DoneButtonText { get; set; }
        public string CancelButtonText { get; set; }

        public UIDatePicker DatePicker { get; set; }
        public UIPickerView PickerView { get; set; }
        private ModalPickerType _pickerType;
        public ModalPickerType PickerType
        {
            get { return _pickerType; }
            set
            {
                switch (value)
                {
                    case ModalPickerType.Date:
                        DatePicker = new UIDatePicker(CGRect.Empty);
                        PickerView = null;
                        break;
                    case ModalPickerType.Custom:
                        DatePicker = null;
                        PickerView = new UIPickerView(CGRect.Empty);
                        break;
                }

                _pickerType = value;
            }
        }

        private UILabel _headerLabel;
        private UIButton _doneButton;
        private UIButton _cancelButton;
        private UIViewController _parent;
        private UIView _internalView;

        public ModalPickerViewController(ModalPickerType pickerType, string headerText, UIViewController parent)
        {
            HeaderBackgroundColor = UIColor.White;
            HeaderTextColor = UIColor.Black;
            HeaderText = headerText;
            PickerType = pickerType;
            _parent = parent;
            DoneButtonText = "Done";
            CancelButtonText = "Cancel";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeControls();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            Show();
        }

        private void InitializeControls()
        {
            View.BackgroundColor = UIColor.Clear;
            _internalView = new UIView();

            _headerLabel = new UILabel(new CGRect(0, 0, 320 / 2, 44));
            _headerLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            _headerLabel.BackgroundColor = HeaderBackgroundColor;
            _headerLabel.TextColor = HeaderTextColor;
            _headerLabel.Text = HeaderText;
            _headerLabel.TextAlignment = UITextAlignment.Center;

            _cancelButton = UIButton.FromType(UIButtonType.System);
            _cancelButton.SetTitleColor(HeaderTextColor, UIControlState.Normal);
            _cancelButton.BackgroundColor = UIColor.Clear;
            _cancelButton.SetTitle(CancelButtonText, UIControlState.Normal);
            _cancelButton.TouchUpInside += CancelButtonTapped;

            _doneButton = UIButton.FromType(UIButtonType.System);
            _doneButton.SetTitleColor(HeaderTextColor, UIControlState.Normal);
            _doneButton.BackgroundColor = UIColor.Clear;
            _doneButton.SetTitle(DoneButtonText, UIControlState.Normal);
            _doneButton.TouchUpInside += DoneButtonTapped;

            switch (PickerType)
            {
                case ModalPickerType.Date:
                    DatePicker.BackgroundColor = UIColor.White;
                    _internalView.AddSubview(DatePicker);
                    break;
                case ModalPickerType.Custom:
                    PickerView.BackgroundColor = UIColor.White;
                    _internalView.AddSubview(PickerView);
                    break;
            }
            _internalView.BackgroundColor = HeaderBackgroundColor;

            _internalView.AddSubview(_headerLabel);
            _internalView.AddSubview(_cancelButton);
            _internalView.AddSubview(_doneButton);

            Add(_internalView);
        }

        private void Show(bool onRotate = false)
        {
            var buttonSize = new CGSize(71, 30);

            var width = _parent.View.Frame.Width;

            var internalViewSize = CGSize.Empty;
            switch (_pickerType)
            {
                case ModalPickerType.Date:
                    internalViewSize = new CGSize(width, DatePicker.Frame.Height + _headerBarHeight);
                    break;
                case ModalPickerType.Custom:
                    internalViewSize = new CGSize(width, PickerView.Frame.Height + _headerBarHeight);
                    break;
            }

            var internalViewFrame = CGRect.Empty;
            if (InterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                if (onRotate)
                {
                    internalViewFrame = new CGRect(0, View.Frame.Height - internalViewSize.Height,
                        internalViewSize.Width, internalViewSize.Height);
                }
                else
                {
                    internalViewFrame = new CGRect(0, View.Bounds.Height - internalViewSize.Height,
                        internalViewSize.Width, internalViewSize.Height);
                }
            }
            else
            {
                if (onRotate)
                {
                    internalViewFrame = new CGRect(0, View.Bounds.Height - internalViewSize.Height,
                        internalViewSize.Width, internalViewSize.Height);
                }
                else
                {
                    internalViewFrame = new CGRect(0, View.Frame.Height - internalViewSize.Height,
                        internalViewSize.Width, internalViewSize.Height);
                }
            }
            _internalView.Frame = internalViewFrame;

            switch (_pickerType)
            {
                case ModalPickerType.Date:
                    DatePicker.Frame = new CGRect(DatePicker.Frame.X, _headerBarHeight, _internalView.Frame.Width,
                                                      DatePicker.Frame.Height);
                    break;
                case ModalPickerType.Custom:
                    PickerView.Frame = new CGRect(PickerView.Frame.X, _headerBarHeight, _internalView.Frame.Width,
                                                      PickerView.Frame.Height);
                    break;
            }

            _headerLabel.Frame = new CGRect(20 + buttonSize.Width, 4, _parent.View.Frame.Width - (40 + 2 * buttonSize.Width), 35);
            _doneButton.Frame = new CGRect(internalViewFrame.Width - buttonSize.Width - 10, 7, buttonSize.Width, buttonSize.Height);
            _cancelButton.Frame = new CGRect(10, 7, buttonSize.Width, buttonSize.Height);
        }

        private void DoneButtonTapped(object sender, EventArgs e)
        {
            DismissViewController(true, null);
            if (OnModalPickerDismissed != null)
            {
                OnModalPickerDismissed(sender, e);
            }
        }

        private void CancelButtonTapped(object sender, EventArgs e)
        {
            DismissViewController(true, null);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            if (InterfaceOrientation == UIInterfaceOrientation.Portrait ||
                InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
                InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                Show(true);
                View.SetNeedsDisplay();
            }
        }
    }

    public enum ModalPickerType
    {
        Date = 0,
        Custom = 1
    }
}

