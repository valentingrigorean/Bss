﻿//
// TintedImageView.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2018 (c) Grigorean Valentin
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
namespace Bss.iOS.UIKit
{
    [Register(nameof(TintedImageView))]
    public class TintedImageView : UIImageView
    {
        private UIImage _tintedImage;

        public TintedImageView()
        {
            Initialiaze();
        }

        public TintedImageView(IntPtr handle) : base(handle)
        {
            Initialiaze();
        }

        public TintedImageView(CGRect frame) : base(frame)
        {
            Initialiaze();
        }

        public override UIColor TintColor
        {
            get => base.TintColor;
            set
            {
                base.TintColor = value;
                SetTintForImage(Image);
            }
        }

        public override UIImage Image
        {
            get => base.Image;
            set => SetTintForImage(value);
        }

        private void SetTintForImage(UIImage image)
        {
            if (image == null || TintColor == null || image == _tintedImage)
                return;
            _tintedImage = image.ChangeColor(TintColor);
            base.Image = _tintedImage;
        }

        private void Initialiaze()
        {
            SetTintForImage(Image);
        }
    }
}
