//
// RxNibView.cs
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
using System.Reactive.Disposables;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using Rx.Core;
using UIKit;

namespace Rx.iOS.RxViews
{
    public abstract class RxNibView<T> : RxNibView, IViewFor<T>
        where T : class
    {
        private T _viewModel;

        protected RxNibView()
        {
           
        }

        protected RxNibView(IntPtr handle) : base(handle)
        {
           
        }

        protected RxNibView(CGRect frame) : base(frame)
        {
            
        }

        public T ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        object IViewFor.ViewModel
        {
            get => _viewModel;
            set => _viewModel = (T)value;
        }
    }

    public abstract class RxNibView : ReactiveView, ISupportRxUI, IActivatable
    {
        private bool _wasInit;


        protected RxNibView()
        {
            Initialize();
        }

        protected RxNibView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        protected RxNibView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public UIView ContentView { get; private set; }

        public override CGRect Bounds
        {
            get => base.Bounds;
            set
            {
                base.Bounds = value;
                if (ContentView != null)
                    ContentView.Bounds = value;
            }
        }

        public virtual void SetupUserInterface()
        {
        }

        /// <summary>
        /// Setups the reactive translation.
        /// Used to can change at runtime translation
        /// </summary>
        /// <param name="disp">Disp.</param>
        public virtual void SetupReactiveTranslation(CompositeDisposable disp) { }

        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp) { }

        public void SetupReactiveObservables()
        {
        }

        public void SetupTheme()
        {
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            if (!_wasInit)
            {
                SetupUserInterface();
                _wasInit = true;
            }
        }

        private void LoadNib()
        {
            var arr = NSBundle.MainBundle.LoadNib(GetType().Name, this, null);
            ContentView = arr.GetItem<UIView>(0);
            ContentView.Frame = Bounds;
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            ContentView.TranslatesAutoresizingMaskIntoConstraints = true;
            AddSubview(ContentView);
        }

        private void Initialize()
        {
            BackgroundColor = UIColor.Clear;
            LoadNib();

            this.WhenActivated((CompositeDisposable disp) =>
            {
                SetupReactiveTranslation(disp);
                SetupReactiveSubscriptions(disp);
            });
        }
    }
}