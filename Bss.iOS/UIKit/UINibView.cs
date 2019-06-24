//
// UINibView.cs
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Bss.Core.Utils;
using Bss.iOS.Utils;

namespace Bss.iOS.UIKit
{
    [Register("UINibView")]
    public abstract class UINibView : UIView, INotifyPropertyChanged
    {
        private bool _wasInit;
        private BBooleanDisposable _supressNotifications;

        protected UINibView()
        {
            Initialize();
        }

        protected UINibView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        protected UINibView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public UIView ContentView { get; private set; }

        public override CGRect Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;
                if (ContentView != null)
                    ContentView.Bounds = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable SuppressChangeNotifications()
        {
            if (_supressNotifications == null || _supressNotifications.IsDisposed)
                _supressNotifications = new BBooleanDisposable();
            return _supressNotifications;
        }

        protected void RaisePropertyChanged([CallerMemberName]string caller = "")
        {
            if (_supressNotifications == null || _supressNotifications.IsDisposed)
                PropertyChanged?.Invoke(this, PropertyChangedEventArgsCache.GetArgs(caller));
        }

        protected bool RaiseAndSetIfChanged<T>(ref T backingField, T newValue, [CallerMemberName]string caller = "")
        {
            var areEqaual = EqualityComparer<T>.Default.Equals(backingField, newValue);
            if (areEqaual)
                return false;
            backingField = newValue;
            RaisePropertyChanged(caller);
            return true;
        }


        public virtual void ViewDidLoad()
        {
            BackgroundColor = UIColor.Clear;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            if (!_wasInit)
            {
                ViewDidLoad();
                _wasInit = true;
            }
        }

        protected virtual NSArray LoadViewFromNib()
        {
            return NSBundle.MainBundle.LoadNib(GetType().Name, this, null);
        }

        private void LoadNib()
        {
            var arr = LoadViewFromNib();
            ContentView = arr.GetItem<UIView>(0);
            ContentView.Frame = Bounds;
            ContentView.BackgroundColor = UIColor.Clear;
            ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddSubview(ContentView);
            ContentView.TopAnchor.ConstraintEqualTo(TopAnchor, 0).Active = true;
            ContentView.BottomAnchor.ConstraintEqualTo(BottomAnchor, 0).Active = true;
            ContentView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 0).Active = true;
            ContentView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, 0).Active = true;
        }

        private void Initialize()
        {          
            LoadNib();
        }
    }
}

