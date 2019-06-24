//
// LayoutObserver.cs
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
using Foundation;
using UIKit;
using CoreGraphics;

namespace Bss.iOS.UIKit
{

    public class LayoutChangeEventArgs : EventArgs
    {
        public LayoutChangeEventArgs(UIView view, CGRect oldValue, CGRect newValue)
        {
            View = view;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public UIView View { get; }
        public CGRect OldValue { get; }
        public CGRect NewValue { get; }
    }

    public class LayoutObserver : IDisposable
    {
        private readonly UIView _view;
        private readonly Action _action;
        private NSObject _observer;
        private bool _disposed;

        private const string Key = "bounds";

        public LayoutObserver(UIView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            _view = view;
            _observer = _view.AddObserver(Key, NSKeyValueObservingOptions.OldNew, OnNewLayout) as NSObject;
        }

        public LayoutObserver(UIView view, Action action) : this(view)
        {
            _action = action;
        }

        ~LayoutObserver()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            //_view.RemoveObserver(_observer, new NSString(Key), _observer.Handle);
            _observer.Dispose();
        }

        public event EventHandler<LayoutChangeEventArgs> OnLayoutChange;

        private void OnNewLayout(NSObservedChange obj)
        {
            var oldValue = obj.OldValue as NSValue;
            var newValue = obj.NewValue as NSValue;

            OnLayoutChange?.Invoke(this, new LayoutChangeEventArgs(
                _view, oldValue.CGRectValue, newValue.CGRectValue));
            _action?.Invoke();
        }
    }
}
