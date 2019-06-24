//
// PaddingLabel.cs
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
    [Register("PaddingLabel")]
    public class PaddingLabel : UILabel
    {
        public PaddingLabel()
        {
        }

        public PaddingLabel(IntPtr handle) : base(handle)
        {
        }

        [Export("Padding")]
        public UIEdgeInsets Padding { get; set; } = new UIEdgeInsets(0, 10, 0, 10);


        [Export("paddingTop")]
        public nfloat PaddingTop
        {
            get { return Padding.Top; }
            set
            {
                Padding = new UIEdgeInsets(value, Padding.Left, Padding.Bottom, Padding.Right);
            }
        }

        [Export("paddingLeft")]
        public nfloat PaddingLeft
        {
            get { return Padding.Left; }
            set
            {
                Padding = new UIEdgeInsets(Padding.Top, value, Padding.Bottom, Padding.Right);
            }
        }


        [Export("paddingBottom")]
        public nfloat PaddingBottom
        {
            get { return Padding.Bottom; }
            set
            {
                Padding = new UIEdgeInsets(Padding.Top, Padding.Left, value, Padding.Right);
            }
        }

        [Export("paddingRight")]
        public nfloat PaddingRight
        {
            get { return Padding.Right; }
            set
            {
                Padding = new UIEdgeInsets(Padding.Top, Padding.Left, Padding.Bottom, value);
            }
        }

        public override void DrawText(CGRect rect)
        {
            base.DrawText(Padding.InsetRect(rect));
        }

        public override CGRect TextRectForBounds(CGRect bounds, nint numberOfLines)
        {
            var rect = base.TextRectForBounds(bounds, numberOfLines);

            return new CGRect(0, 0, rect.Width + Padding.Left + Padding.Right,
                              rect.Height + Padding.Top + Padding.Bottom);
        }

        //public override CGSize IntrinsicContentSize
        //{
        //    get
        //    {
        //        var _base = base.IntrinsicContentSize;
        //        var width = _base.Width + Padding.Left + Padding.Right;
        //        var height = _base.Height + Padding.Top + Padding.Bottom;
        //        return new CGSize(width, height);
        //    }
        //}

        //public override CGSize SizeThatFits(CGSize size)
        //{
        //    var _base = base.SizeThatFits(size);
        //    var width = _base.Width + Padding.Left + Padding.Right;
        //    var height = _base.Height + Padding.Top + Padding.Bottom;
        //    return new CGSize(width, height);
        //}
    }
}
