//
// BaseTableViewCell.cs
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
using UIKit;
using System.Collections.Generic;

namespace Bss.iOS.UIKit
{
    public abstract class TableViewCell<T> : UITableViewCell, IReusableView<T>
    {
        private IList<IDisposable> _disposableContainer = new List<IDisposable>();

        protected TableViewCell(IntPtr ptr)
            : base(ptr)
        {
        }

        public void AddDisposable(IDisposable disposable)
        {
            if (!_disposableContainer.Contains(disposable))
                _disposableContainer.Add(disposable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in _disposableContainer)
                    item.Dispose();
                _disposableContainer.Clear();
            }
            base.Dispose(disposing);
        }
       

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            OnCreate();
        }

        public virtual T Model { get; private set; }


        public int Index { get; private set; }

        public void SetModel(int index, T model)
        {
            BeforeBind();
            Index = index;
            Model = model;
            OnBind();
        }

        public virtual void BeforeBind()
        {

        }

        public virtual void OnCreate()
        {

        }

        public abstract void OnBind();
    }
}