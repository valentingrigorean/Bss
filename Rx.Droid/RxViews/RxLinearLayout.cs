//
// RxFrameLayout.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2017 (c) Grigorean Valentin
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Rx.Core.Utils;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Rx.Droid.RxViews
{
    public class RxLinearLayout : LinearLayout, INotifyPropertyChanged
    {
        private BooleanDisposable _supressNotifications;
        private Subject<Unit> _activated;
        private Subject<Unit> _deactivated;

        public RxLinearLayout(Context context) :
            base(context)
        {
        }

        public RxLinearLayout(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
        }

        public RxLinearLayout(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                SubscriptionDisposable.Dispose();
            base.Dispose(disposing);
        }

        public CompositeDisposable SubscriptionDisposable { get; private set; } = new CompositeDisposable();       

        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable SuppressChangeNotifications()
        {
            if (_supressNotifications == null || _supressNotifications.IsDisposed)
                _supressNotifications = new BooleanDisposable();
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
    }
}