//
// TextButton.cs
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
using Foundation;
using System.ComponentModel;
using CoreGraphics;
using UIKit;

namespace Bss.iOS.UIKit
{
    [Register("TextButton"), DesignTimeVisible(true)]
    public class TextButton : UIButtonView
    {
        public TextButton()
        {
            Initialize();
        }

        public TextButton(IntPtr ptr)
            : base(ptr)
        {
            Initialize();
        }

        public TextButton(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        public TextButton(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        public TextButton(NSObjectFlag t)
            : base(t)
        {
            Initialize();
        }

        public new UILabel TitleLabel { get; private set; }
		      

        [Export("FontSize"), Browsable(true)]
        public nfloat FontSize
        {
            get { return TitleLabel.Font.PointSize; }
            set
            {
                TitleLabel.Font = TitleLabel.Font.WithSize(value);
            }
        }

        [Export("TextAlignment"), Browsable(true)]
        public UITextAlignment TextAlignment
        {
            get { return TitleLabel.TextAlignment; }
            set
            {
                TitleLabel.TextAlignment = value;
            }
        }


        [Export("TextColor"), Browsable(true)]
        public UIColor TextColor
        {
            get { return TitleLabel.TextColor; }
            set
            {
                TitleLabel.TextColor = value;
            }
        }

        [Export("HighlightedTextColor"), Browsable(true)]
        public UIColor HighlightedTextColor
        {
            get { return TitleLabel.HighlightedTextColor; }
            set
            {
                TitleLabel.HighlightedTextColor = value;
            }
        }

        private void Initialize()
        {
            TitleLabel = new UILabel
            {
                Text = "Button",
                Lines = 0,
                TextAlignment = UITextAlignment.Center
            };

            TitleLabel.PinToParent(TitleLabel);

            HighlightChanged += (sender, e) => TitleLabel.Highlighted = e.Highlighted;
        }
    }
}

