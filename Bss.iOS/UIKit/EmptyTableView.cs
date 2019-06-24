//
// EmptyTableView.cs
//
// Author:
//       valentingrigorean <>
//
// Copyright (c) 2017 ${CopyrightHolder}
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
using CoreGraphics;
using UIKit;

namespace Bss.iOS.UIKit
{
	[Register("EmptyTableView")]
	internal partial class EmptyTableView : UINibView
	{
		private string _text;
		private UIImage _image;
		private NSAttributedString _attributedText;

        [Outlet] private UIImageView ImageView { get; set; }

        [Outlet] private UILabel TextLbl { get; set; }

		private void ReleaseDesignerOutlets()
        {
            if (TextLbl != null)
            {
                TextLbl.Dispose();
                TextLbl = null;
            }

            if (ImageView != null)
            {
                ImageView.Dispose();
                ImageView = null;
            }
        }


        public EmptyTableView()
		{
		}

		public EmptyTableView(CGRect frame) : base(frame)
		{
		}

		public EmptyTableView(IntPtr handle) : base(handle)
		{
		}

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				if (TextLbl != null)
					TextLbl.Text = value;
			}
		}

		public UIImage Image
		{
			get { return _image; }
			set
			{
				_image = value;
				if (ImageView != null)
					ImageView.Image = value;
			}
		}

		public NSAttributedString AttributedText
		{
			get { return _attributedText; }
			set
			{
				_attributedText = value;
				if (TextLbl != null)
					TextLbl.AttributedText = value;
			}
		}

		public override void ViewDidLoad()
		{
			ImageView.Image = _image;
			if (_attributedText != null)
				TextLbl.AttributedText = _attributedText;
			else
				TextLbl.Text = _text;

			BackgroundColor = UIColor.Clear;
		}
	}
}
