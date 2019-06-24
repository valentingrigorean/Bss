//
// UILabelExtension.cs
//
// Author:
//       Valentin <v.grigorean@software-dep.net>
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
using System.Threading.Tasks;
using Bss.iOS.UIKit;
using Bss.iOS.Utils;
using CoreGraphics;
using CoreText;
using Foundation;

namespace UIKit
{
    public enum Direction
    {
        /// <summary>
        /// Will Expand Width keeping Height
        /// </summary>
        Width,
        /// <summary>
        /// Will Expand Height keeping Width
        /// </summary>
        Height
    }

    public static class UiLabelExtension
    {
        public static ITextView ToTextView(this UILabel This)
        {
            return new LabelTextViewWrapper(This);
        }

        public static void ChangeFontSize(this UILabel This, float size)
        {
            This.Font = This.Font.WithSize(size);
        }

        /// <summary>
        /// Sizes to fit.
        /// </summary>
        /// <param name="lbl">Lbl.</param>
        /// <param name="direction">Direction.</param>
        public static void SizeToFit(this UILabel lbl, Direction direction)
        {
            if (lbl == null)
                return;
            var width = direction == Direction.Width ?
                nfloat.MaxValue : lbl.Bounds.Width;
            var height = direction == Direction.Height ?
                nfloat.MaxValue : lbl.Bounds.Height;

            var size = lbl.SizeThatFits(new CGSize(width, height));
            var frame = lbl.Frame;
            frame.Size = size;
            lbl.Frame = frame;
        }

        public static void SetUnderLine(this UILabel lbl)
        {
            SetUnderLine(lbl, new NSRange(0, lbl.Text.Length), lbl.Text);
        }

        public static void SetUnderLine(this UILabel lbl, string text)
        {
            SetUnderLine(lbl, new NSRange(0, lbl.Text.Length), text);
        }

        public static void SetUnderLine(this UILabel lbl, string text, string textUnderLine)
        {
            var range = new NSRange(text.IndexOf(textUnderLine, StringComparison.CurrentCulture), textUnderLine.Length);
            SetUnderLine(lbl, range, text);
        }

        public static void SetUnderLine(this UILabel lbl, NSRange range, string text)
        {
            var attrText = new NSMutableAttributedString(text, lbl.Font);
            attrText.AddAttribute(UIStringAttributeKey.UnderlineStyle,
                                  NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);
            lbl.AttributedText = attrText;
        }

        public static void SetBold(this UILabel lbl)
        {
            if (lbl.Text == null) return;
            SetBold(lbl, new NSRange(0, lbl.Text.Length));
        }

        public static void SetBold(this UILabel lbl, string subString, UIFont font = null)
        {
            var range = new NSRange(lbl.Text.IndexOf(subString, StringComparison.CurrentCulture), subString.Length);
            SetBold(lbl, range, font);
        }

        public static void SetBold(this UILabel lbl, NSRange range, UIFont font = null)
        {
            var attrs = new NSMutableAttributedString(lbl.Text);
            var _font = font?.GetCtFont() ?? FontManager.GetFontWithTraits(UIFontDescriptorSymbolicTraits.Bold,
                                                                           lbl.Font.PointSize).GetCtFont();
            attrs.AddAttributes(new CTStringAttributes
            {
                Font = _font
            }, range);
            lbl.AttributedText = attrs;
        }

        public static nfloat SizeHeight(this UILabel lable, nfloat viewWidth, string text, nfloat sizeFont)
        {
            var lables = new UILabel
            {
                Font = lable.Font.WithSize(sizeFont),
                Text = text,
                Frame = new CGRect(0, 0, viewWidth, nfloat.MaxValue),
                Lines = 0
            };
            lables.SizeToFit();

            return lables.Bounds.Height;
        }

        public static void SizeToFitWidth(this UILabel self, nfloat width)
        {
            SizeToFitWidth(self, width, 0);
        }

        public static void SizeToFitWidth(this UILabel self, nfloat width, int lines = 0)
        {
            self.Frame = new CGRect(0, 0, width, float.MaxValue);
            self.Lines = lines;
            self.SizeToFit();
        }

        public static nint GetNumberOfLines(this UILabel lbl)
        {

            if (lbl.Lines > 0) return lbl.Lines;
            var paragraphStyle = new NSMutableParagraphStyle { LineBreakMode = lbl.LineBreakMode };


            var maxSize = new CGSize(lbl.Bounds.Width, nfloat.MaxValue);
            var attributes = new UIStringAttributes
            {
                Font = lbl.Font,
                ParagraphStyle = (NSParagraphStyle)paragraphStyle.Copy()
            };
            var nsstring = new NSString(lbl.Text);
            var rect = nsstring.GetBoundingRect(maxSize, NSStringDrawingOptions.
                                                UsesLineFragmentOrigin, attributes, null);
            return (nint)Math.Floor(rect.Height / lbl.Font.LineHeight);
        }

        public static async Task SetTime(this UILabel lbl, UIViewController controller, UIDatePickerMode mode = UIDatePickerMode.Date, string format = "dd.MM.yyyy")
        {
            var modalPicker = new ModalPickerViewController(ModalPickerType.Date, "", controller)
            {
                HeaderBackgroundColor = UIColor.Gray,
                HeaderTextColor = UIColor.White,
                TransitioningDelegate = new ModalPickerTransitionDelegate(),
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
            };

            modalPicker.DatePicker.Mode = mode;

            modalPicker.OnModalPickerDismissed += (s, ea) =>
            {
                var dateFormatter = new NSDateFormatter
                {
                    DateFormat = format
                };

                lbl.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
            };

            await controller.PresentViewControllerAsync(modalPicker, true);
        }

        public static bool SetHtml(this UILabel lbl, string html, bool failBacktoText = true)
        {
            if (string.IsNullOrEmpty(html)) return true;
            var attr = new NSAttributedStringDocumentAttributes();
            var nsError = new NSError();
            attr.DocumentType = NSDocumentType.HTML;
            var unicode = NSData.FromString(html, NSStringEncoding.Unicode);
            var nsAttr = new NSAttributedString(unicode, attr, ref nsError);
            var att = new NSMutableAttributedString(nsAttr);
            att.AddAttribute(UIStringAttributeKey.Font, lbl.Font, new NSRange(0, att.Length));
            lbl.AttributedText = att;
            var flag = lbl.AttributedText.Length > 0;
            if (!flag && failBacktoText)
                lbl.Text = html;
            return flag;
        }
    }
}