//
// ReadMoreTextView.cs
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
using System.ComponentModel;
using Foundation;

namespace Bss.iOS.UIKit
{
    /// <summary>
    /// Port from swift(https://github.com/ilyapuchka/ReadMoreTextView)
    /// </summary>
    [Register("ReadMoreTextView")]
    public class ReadMoreTextView : UITextView
    {
        private int _maxiumNumberOfLines;

        private string _trimText;
        private NSAttributedString _attributedTrimText;
        private bool _shouldTrim;
        private bool _shouldTrimInternal;

        private string _originalText;
        private NSAttributedString _origianlAttributedText;

        public ReadMoreTextView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public ReadMoreTextView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public ReadMoreTextView(CGRect frame, NSTextContainer textContainer) : base(frame, textContainer)
        {
            Initialize();
        }

        public UIEdgeInsets TrimTextRangePadding { get; set; } = UIEdgeInsets.Zero;

        public bool AppendTrimTextPrefix { get; set; } = true;

        public string TrimTextPrefix { get; set; } = "...";

        public Func<UITextView, bool> ShouldExpend { get; set; } = (arg) => true;

        [Export("maxiumNumberOfLines"), Browsable(true)]
        public int MaxiumNumberOfLines
        {
            get { return _maxiumNumberOfLines; }
            set
            {
                _maxiumNumberOfLines = value;
                _shouldTrim |= (_maxiumNumberOfLines > 0 && _shouldTrimInternal);
                SetNeedsLayout();
            }
        }

        [Export("trimText"), Browsable(true)]
        public string TrimText
        {
            get { return _trimText; }
            set
            {
                _trimText = value;
                SetNeedsLayout();
            }
        }

        public NSAttributedString AttributedTrimText
        {
            get { return _attributedTrimText; }
            set
            {
                _attributedTrimText = value;
                SetNeedsLayout();
            }
        }

        [Export("shouldTrim"), Browsable(true)]
        public bool ShouldTrim
        {
            get { return _shouldTrim; }
            set
            {
                _shouldTrim = value;
                _shouldTrimInternal = true;
                SetNeedsLayout();
            }
        }

        public override NSAttributedString AttributedText
        {
            get
            {
                return base.AttributedText;
            }
            set
            {
                base.AttributedText = value;
                _originalText = null;
                _origianlAttributedText = value;
                if (NeedsTrim()) UpdateText();
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
                _originalText = Text;
                _origianlAttributedText = null;
                if (NeedsTrim()) UpdateText();
            }
        }

        public void UpdateText()
        {
            TextContainer.MaximumNumberOfLines = (nuint)MaxiumNumberOfLines;
            TextContainer.Size = new CGSize(Bounds.Size.Width, nfloat.MaxValue);

            var range = RangeToReplaceWithTrimText();
            if (range.Location != NSRange.NotFound)
            {
                var prefix = AppendTrimTextPrefix ? TrimTextPrefix : "";
                if (!string.IsNullOrEmpty(TrimText))
                {
                    var text = TrimTextInternal.Insert(0, prefix);
                    TextStorage.Replace(range, text);
                }
                else if (AttributedTrimText != null)
                {
                    var mutable = AttributedTrimText?.MutableCopy() as NSMutableAttributedString;
                    if (mutable != null)
                    {
                        mutable.Insert(new NSAttributedString(prefix), 0);
                        TextStorage.Replace(range, mutable);
                    }
                }
            }
            InvalidateIntrinsicContentSize();
        }

        public void ResetText()
        {
            TextContainer.MaximumNumberOfLines = 0;
            if (_originalText != null)
                TextStorage.Replace(new NSRange(0, Text.Length), _originalText);
            else if (_origianlAttributedText != null)
                TextStorage.Replace(new NSRange(0, Text.Length), _origianlAttributedText);
            InvalidateIntrinsicContentSize();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (NeedsTrim())
                UpdateText();
            else
                ResetText();
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var handler = ShouldExpend;
            if (NeedsTrim() && PointIntrimTextRange(point))
            {
                if (handler != null && handler(this))
                {
                    ShouldTrim = false;
                    _maxiumNumberOfLines = 0;
                }
            }
            return base.HitTest(point, uievent);
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                TextContainer.Size = new CGSize(Bounds.Width, nfloat.MaxValue);
                var intrinsicContentSize = LayoutManager.BoundingRectForGlyphRange(
                    LayoutManager.GetGlyphRange(TextContainer), TextContainer).Size;
                intrinsicContentSize.Width = NoIntrinsicMetric;
                intrinsicContentSize.Height += (TextContainerInset.Top + TextContainerInset.Bottom);
                return intrinsicContentSize;
            }
        }

        private string TrimTextInternal => string.IsNullOrEmpty(TrimText) ?
                                                 AttributedTrimText?.Value : TrimText;

        private int OriginalTextLength
        {
            get
            {
                if (!string.IsNullOrEmpty(_originalText))
                    return _originalText.Length;
                if (_origianlAttributedText != null)
                    return (int)_origianlAttributedText.Length;
                return 0;
            }
        }

        private int TrimtextPrefixLength => AppendTrimTextPrefix ? TrimTextPrefix.Length + 1 : 1;

        private bool NeedsTrim()
        {
            return ShouldTrim && !string.IsNullOrEmpty(TrimTextInternal);
        }

        private NSRange RangeToReplaceWithTrimText()
        {
            var emptyRange = new NSRange(NSRange.NotFound, 0);
            var rangeToReplace = LayoutManager.CharacterRangeThatFits(TextContainer);

            if (rangeToReplace.NSMaxRange() == OriginalTextLength)
                rangeToReplace = emptyRange;
            else
            {
                rangeToReplace.Location = rangeToReplace.NSMaxRange() -
                    TrimTextInternal.Length - TrimtextPrefixLength;
                if (rangeToReplace.Location < 0)
                    rangeToReplace = emptyRange;
                else
                    rangeToReplace.Length = TextStorage.Length - rangeToReplace.Location;
            }
            return rangeToReplace;
        }

        private NSRange TrimTextRange()
        {
            var trimTextRange = RangeToReplaceWithTrimText();
            if (trimTextRange.Location != NSRange.NotFound)
                trimTextRange.Length = TrimtextPrefixLength + TrimTextInternal.Length;
            return trimTextRange;
        }

        private bool PointIntrimTextRange(CGPoint point)
        {
            var offset = new CGPoint(TextContainerInset.Left, TextContainerInset.Top);
            var boundingRect = LayoutManager.BoundingRectForCharacterRange(TrimTextRange(), TextContainer, offset);
            boundingRect.Offset(TextContainerInset.Left, TextContainerInset.Top);
            boundingRect.Inset(-(TrimTextRangePadding.Left + TrimTextRangePadding.Right),
                               -(TrimTextRangePadding.Top + TrimTextRangePadding.Bottom));

            return boundingRect.Contains(point);
        }

        private void Initialize()
        {
            ScrollEnabled = false;
            Editable = false;
        }
    }
}