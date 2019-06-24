//
// CustomLayout.cs
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
using UIKit;
using CoreGraphics;
using Foundation;
using System;

namespace Bss.iOS.UIKit
{
    [Register("GridCollectionViewFlowLayout")]
    public partial class GridCollectionViewFlowLayout : UICollectionViewFlowLayout
    {
        public GridCollectionViewFlowLayout()
        {
            Initialize();
        }

        public int NumberOfColumns { get; set; } = 3;

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// To Width
        /// </summary>
        /// <value>The aspect ratio.</value>
        public nfloat? AspectRatio { get; set; }

        public int Height { get; set; } = 150;

        public override CGSize ItemSize
        {
            get
            {
                if (CollectionView == null) return CGSize.Empty;
                var width = (CollectionView.Bounds.Width - NumberOfColumns - 1) / NumberOfColumns;
                if (AspectRatio.HasValue)
                    Height = (int)(width * AspectRatio.Value);
                return new CGSize(width, Height);
            }
        }

        private void Initialize()
        {
            MinimumLineSpacing = 1f;
            MinimumInteritemSpacing = 1f;
            ScrollDirection = UICollectionViewScrollDirection.Vertical;
        }
    }
}

