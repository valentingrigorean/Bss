//
// HyperLabel.cs
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
    /// UILabel with hyperlink support
    /// Might not work proper if used with multiple UIStringAttributes with diff fonts
    /// </summary>
    [Register("HyperLabel"), DesignTimeVisible(true)]
    public class HyperLabel : UILabel
    {
        private const float HighLightAnimationTime = 0.15f;
        private IDictionary<NSRange, LinkHandler> _handlerDictionary;
        private NSAttributedString _backupAttributedText;
        private bool _wasTextCheck;

        private static readonly UIColor HyperLinkColorDefault = UIColor.FromRGB(28, 135, 199);
        private static readonly UIColor HyperLinkColorHighlight = UIColor.FromRGB(242, 183, 73);


        public HyperLabel()
        {
            Initialize();
        }

        public HyperLabel(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public HyperLabel(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                _wasTextCheck = true;
                if (Lines != 0)
                {
                    base.Text = value;
                    return;
                }
                var newAttr = new NSMutableAttributedString(value);
                newAttr.AddAttributes(new UIStringAttributes
                {
                    Font = Font
                }, new NSRange(0, value.Length));
                AttributedText = newAttr;
            }
        }


        public UIStringAttributes LinkAttributeDefault { get; set; } = new UIStringAttributes
        {
            ForegroundColor = HyperLinkColorDefault,
            UnderlineStyle = NSUnderlineStyle.Single
        };

        public UIStringAttributes LinkAttributeHighlight { get; set; } = new UIStringAttributes
        {
            ForegroundColor = HyperLinkColorHighlight,
            UnderlineStyle = NSUnderlineStyle.Single
        };

        public delegate void LinkHandler(HyperLabel label, NSRange range);

        #region  APIs
        public void SetLinkForRange(NSRange range, UIStringAttributes attributes, LinkHandler handler)
        {
            CheckIfShouldDoAttribute();
            var newAttribute = new NSMutableAttributedString(AttributedText);
            newAttribute.AddAttributes(attributes, range);
            if (handler != null)
                _handlerDictionary.Add(range, handler);
            AttributedText = newAttribute;
        }

        public void SetLinkForRange(NSRange range, LinkHandler handler)
        {
            SetLinkForRange(range, LinkAttributeDefault, handler);
        }

        public void SetLinkForSubstring(string substring, UIStringAttributes attributes, LinkHandler handler)
        {
            var range = Text.IndexOf(substring, StringComparison.CurrentCulture);
            if (range < 0) return;
            SetLinkForRange(new NSRange(range, substring.Length), attributes, handler);
        }

        public void SetLinkForSubstring(string substring, LinkHandler handler)
        {
            SetLinkForSubstring(substring, LinkAttributeDefault, handler);
        }

        public void SetLinkForSubstrings(string[] substrings, LinkHandler handler)
        {
            foreach (var item in substrings)
                SetLinkForSubstring(item, handler);
        }

        public void ClearActionDictionary()
        {
            _handlerDictionary.Clear();
        }

        #endregion

        #region Event Handler

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            _backupAttributedText = AttributedText;
            var arr = touches.ToArray<UITouch>();
            foreach (var touch in arr)
            {
                var touchPoint = touch.LocationInView(this);
                var rangeValue = AttributedTextRangeForPoint(touchPoint);
                if (rangeValue == null) continue;

                var range = rangeValue.Value;
                var attributedString = new NSMutableAttributedString(AttributedText);
                attributedString.AddAttributes(LinkAttributeHighlight, range);
                TransitionNotify(this, HighLightAnimationTime, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                    AttributedText = attributedString;
                }, null);

            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            ReverseAnimation();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            ReverseAnimation();
            var arr = touches.ToArray<UITouch>();
            foreach (var touch in arr)
            {
                var touchPoint = touch.LocationInView(this);
                var rangeValue = AttributedTextRangeForPoint(touchPoint);
                if (rangeValue == null) continue;
                var handler = _handlerDictionary[rangeValue.Value];
                handler(this, rangeValue.Value);
            }
        }

        #endregion

        #region Substring Locator

        private NSRange? AttributedTextRangeForPoint(CGPoint point)
        {
            var layoutManager = new NSLayoutManager();
            var textContainer = new NSTextContainer(CGSize.Empty)
            {
                LineFragmentPadding = 0.0f,
                LineBreakMode = LineBreakMode,
                MaximumNumberOfLines = (nuint) Lines,
                Size = Bounds.Size
            };

            layoutManager.AddTextContainer(textContainer);
            var textStorage = new NSTextStorage();
            textStorage.SetString(AttributedText);
            textStorage.AddLayoutManager(layoutManager);

            var textBoundingBox = layoutManager.GetUsedRectForTextContainer(textContainer);
            var textContainerOffset = new CGPoint(
                (Bounds.Width - textBoundingBox.Width) * 0.5f - textBoundingBox.GetMinX(),
                (Bounds.Height - textBoundingBox.Height) * 0.5f - textBoundingBox.GetMinY());

            var locationOfTouchInTextContainer = new CGPoint(point.X - textContainerOffset.X,
                                                             point.Y - textContainerOffset.Y);
            var frac = new nfloat(0.0f);
            var indexOfCharacter = layoutManager.CharacterIndexForPoint(locationOfTouchInTextContainer,
                                                                        textContainer, ref frac);
            foreach (var pair in _handlerDictionary)
            {
                var range = pair.Key;
                if (range.LocationInRange((int)indexOfCharacter))
                    return range;
            }
            return null;
        }

        #endregion

        private void ReverseAnimation()
        {
            TransitionNotify(this, HighLightAnimationTime, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                    AttributedText = _backupAttributedText;
                }, null);
        }

        private void CheckIfShouldDoAttribute()
        {
            if (!_wasTextCheck && Text.Length > 0)
                Text = Text;
        }

        private void Initialize()
        {
            _handlerDictionary = new Dictionary<NSRange, LinkHandler>();
            UserInteractionEnabled = true;
        }
    }
}

