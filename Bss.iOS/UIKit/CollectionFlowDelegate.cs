//
// CollectionFlowDelegate.cs
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
    public class CollectionFlowDelegate : UICollectionViewDelegateFlowLayout
    {
        private readonly CGSize Size;

        public CollectionFlowDelegate(CGSize size)
        {
            Size = size;
        }

        public event EventHandler<NSIndexPath> ItemClicked;

        public Func<UICollectionViewCell, int, bool> CanFocusItemCallBack { get; set; } = (arg, index) => true;

        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout,
                                                NSIndexPath indexPath)
        {
            return Size;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemClicked?.Invoke(this, indexPath);
            collectionView.SelectItem(indexPath, true, UICollectionViewScrollPosition.None);
        }

        public override bool CanFocusItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return CanFocusItemCallBack(collectionView.CellForItem(indexPath), indexPath.Row);
        }
    }
}
