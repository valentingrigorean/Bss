﻿//
// RxCollectionViewCell.cs
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
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using Rx.Core;
using Rx.iOS.Extenisons;
using CoreGraphics;

namespace Rx.iOS.RxViews
{
    public class RxCollectionViewCell<T> : ReactiveCollectionViewCell<T>, ISupportTheme
        where T : class
    {
        private readonly List<ISupportRxSubscription> _rxSubscriptions = new List<ISupportRxSubscription>();
        private bool _isActivated;

        protected RxCollectionViewCell()
        {

        }

        protected RxCollectionViewCell(IntPtr handle)
            : base(handle)
        {          
        }

        protected RxCollectionViewCell(CGRect frame) : base(frame)
        {

        }

        public bool IsActivated { get; private set; }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            _rxSubscriptions.AddRange(this.GetAllRxViews());

            SetupUserInterface();
            SetupReactiveObservables();

            this.WhenActivated((CompositeDisposable disp) =>
            {
                IsActivated = false;
                SetupReactiveTranslation(disp);
                SetupReactiveSubscriptions(disp);
                IsActivated = true;

                if (_isActivated)
                    return;
                _isActivated = true;
                var activated = ViewModel as ISupportsActivation;
                activated?.Activator.Activate();
            });
        }

        public virtual void SetupUserInterface()
        {
            SetupTheme();
        }

        public virtual void SetupReactiveObservables() { }

        /// <summary>
        /// Setups the reactive translation.
        /// Used to can change at runtime translation
        /// </summary>
        /// <param name="disp">Disp.</param>
        public virtual void SetupReactiveTranslation(CompositeDisposable disp) { }

        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            AppTheme.Instance.NotifyWhenThemeChange(this)
                    .DisposeWith(disp);

            foreach (var rxView in _rxSubscriptions)
                rxView.SetupReactiveSubscriptions(disp);
        }

        public virtual void SetupTheme()
        {

        }
    }
}