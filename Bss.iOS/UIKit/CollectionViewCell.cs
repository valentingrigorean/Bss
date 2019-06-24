//
// CollectionViewCell.cs
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
using System;
using CoreGraphics;
using Foundation;

namespace Bss.iOS.UIKit
{
    public abstract class CollectionViewCell<T> : UICollectionViewCell, IReusableView<T>
    {       
        protected CollectionViewCell(IntPtr handle) : base(handle)
        {

        }

        [Export("initWithFrame:")]
        protected CollectionViewCell(CGRect frame) : base(frame)
        {

        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            OnCreate();
        }

        public virtual T Model { get; private set; }


        public int Index { get; private set; }


        public virtual void OnCreate()
        {

        }


        public virtual void BeforeBind()
        {

        }

        public abstract void OnBind();

        public void SetModel(int index, T model)
        {
            BeforeBind();
            Index = index;
            Model = model;
            OnBind();
        }
    }
}

