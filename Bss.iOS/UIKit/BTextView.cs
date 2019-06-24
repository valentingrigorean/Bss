//
// BTextView.cs
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
using UIKit;
using CoreGraphics;
using Foundation;
using System.ComponentModel;
using System.Collections.Generic;

namespace Bss.iOS.UIKit
{

    /// <summary>
    /// UITextView that have highlight text
    /// Don't set delegate as will not work anymore
    /// </summary>
    [Register("BTextView"), DesignTimeVisible(true)]
    public class BTextView : UITextView, INextResponder, IUITextViewDelegate, INotifyPropertyChanged
    {
        private enum CInset
        {
            Left,
            Top,
            Right,
            Bottom
        }
        private enum CState
        {
            Placeholder,
            Editing,
        }

        private readonly Dictionary<CInset, NSLayoutConstraint> _insetMap = new Dictionary<CInset, NSLayoutConstraint>();

        private CState _currentState;
        private UILabel _label;

        private readonly PropertyChangedEventArgs _textChangedArgs = new PropertyChangedEventArgs(nameof(Text));
        public event PropertyChangedEventHandler PropertyChanged;

        public BTextView()
        {
            Initialize();
        }

        public BTextView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public BTextView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override UIEdgeInsets ContentInset
        {
            get
            {
                return base.ContentInset;
            }
            set
            {
                base.ContentInset = value;
                RefreshInset();
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                _currentState = string.IsNullOrEmpty(value) ? CState.Placeholder : CState.Editing;
                HandleState(_currentState);
            }
        }

        [Export("placeholder"), Browsable(true)]
        public string Placeholder
        {
            get { return _label.Text; }
            set
            {
                _label.Text = value;
            }
        }

        [Export("placehodlerColor"), Browsable(true)]
        public UIColor PlaceholderColor
        {
            get { return _label.TextColor; }
            set
            {
                _label.TextColor = value;
            }
        }

        [Outlet("nextResponderField")]
        public UIResponder NextResponderField { get; set; }

        public override UIFont Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                _label.Font = value;
            }
        }

        private void HandleState(CState state)
        {
            _label.Hidden = state == CState.Editing;
        }

        private void RefreshInset()
        {
            foreach (var item in _insetMap)
            {
                var constant = new nfloat(0);
                switch (item.Key)
                {
                    case CInset.Left:
                        constant = ContentInset.Left + 8f;
                        break;
                    case CInset.Top:
                        constant = ContentInset.Top + 8f;
                        break;
                    case CInset.Right:
                        constant = ContentInset.Right;
                        break;
                    case CInset.Bottom:
                        constant = ContentInset.Bottom;
                        break;
                }
                var val = item.Value;
                val.Constant = constant;
            }
            InvalidateIntrinsicContentSize();
            SetNeedsLayout();
        }

        private void Initialize()
        {
            _label = new UILabel(new CGRect(0, 0, 0, 0)) { TextColor = UIColor.LightGray };
            _label.Font = UIFont.PreferredBody;
            _label.Lines = 0;
            AddSubview(_label);

            nfloat offsetX = 8f;
            nfloat offsetY = 8f;

            _label.TranslatesAutoresizingMaskIntoConstraints = false;

            _insetMap.Add(CInset.Left, _label.LeftAnchor.ConstraintEqualTo(LeftAnchor, offsetX));
            _insetMap.Add(CInset.Right, _label.RightAnchor.ConstraintEqualTo(RightAnchor));
            _insetMap.Add(CInset.Top, _label.TopAnchor.ConstraintEqualTo(TopAnchor, offsetY));
            _insetMap.Add(CInset.Bottom, _label.BottomAnchor.ConstraintEqualTo(BottomAnchor));

            _label.WidthAnchor.ConstraintLessThanOrEqualTo(WidthAnchor).Active = true;

            foreach (var pair in _insetMap)
                pair.Value.Active = true;

            Changed += (sender, e) =>
            {
                var text = Text;
                _currentState = text.Length == 0 ? CState.Placeholder : CState.Editing;
                HandleState(_currentState);
                PropertyChanged?.Invoke(this, _textChangedArgs);
            };

            Ended += (sender, e) =>
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
            };

            TextColor = UIColor.Black;
            _currentState = CState.Placeholder;
        }
    }
}

