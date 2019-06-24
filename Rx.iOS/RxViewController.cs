//
// RxViewController.cs
//
// Author:
//       valentingrigorean <>
//
// Copyright (c) 2017 ${CopyrightHolder}
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
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using Rx.Core;
using Rx.Core.ViewModels;
using Rx.Core.Views;
using Rx.iOS.Extenisons;
using UIKit;

namespace Rx.iOS
{
    public abstract class RxViewController<TViewModel>
        : RxViewController, IRxController, ISupportTheme, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected RxViewController()
        {
        }

        protected RxViewController(IntPtr handle) : base(handle)
        {
        }

        protected RxViewController(string nibName) : base(nibName, null)
        {
        }

        protected RxViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        private TViewModel _viewModel;
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = (TViewModel)value; }
        }
    }

    public abstract class RxViewController : ReactiveViewController, IRxController
    {
        protected RxViewController()
        {
        }

        protected RxViewController(IntPtr handle) : base(handle)
        {
        }

        protected RxViewController(string nibName) : base(nibName, null)
        {
        }

        protected RxViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public bool IsModal { get; private set; }

        public ILoadingView LoadingView { get; set; }

        public CompositeDisposable SubscriptionDisposables { get; private set; } = new CompositeDisposable();

        public CGSize ScreenSize
        {
            get
            {
                var offset = 0;
                if (NavigationController != null)
                    offset = (int)NavigationController.NavigationBar.Bounds.Height;
                if (TabBarController != null)
                    offset += (int)TabBarController.TabBar.Bounds.Height;
                return new CGSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - offset);
            }
        }

        public ISupportRxSubscription[] RxSubscriptions { get; set; }

        public virtual void ShowToast(string message, ToastLength duration = ToastLength.Short)
        {
            RxController.ShowToast(message, duration);
        }

        public virtual Task DisplayAlert(string title, string message, string okButton)
        {
            return DisplayAlert(title, message, okButton, "");
        }

        public virtual Task<bool> DisplayAlert(string title, string message, string okButton, string cancelButton)
        {
            return RxController.DisplayAlert(title, message, okButton, cancelButton);
        }

        /// <summary>
        /// Handles the error async.
        /// Set context.SetOutput(true) if u handled the error
        /// </summary>
        /// <returns>The error async.</returns>
        /// <param name="context">Context.</param>
        public virtual Task HandleErrorAsync(InteractionContext<UserError, ErrorRecoveryOption> context)
        {
            return RxController.HandleErrorAsync(this, context);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RxSubscriptions = View.GetAllRxViews().ToArray();

            SetupUserInterface();

            SetupReactiveObservables();

            this.WhenActivated((CompositeDisposable disp) =>
            {
                SetupReactiveTranslation(disp);
                SetupReactiveSubscriptions(disp);
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (IsBeingPresented)
                IsModal = true;
            else IsModal &= !IsMovingToParentViewController;
        }

        public override void ViewWillDisappear(bool animated)
        {
            SubscriptionDisposables.Clear();

            base.ViewDidDisappear(animated);
        }

        /// <summary>
        /// Dismiss this viewcontroller
        /// Will call DismissViewController(true,null);
        /// </summary>
        public virtual void Dismiss()
        {
            DismissViewController(true, null);
        }

        public virtual void SetupTheme()
        {
        }

        public virtual void SetupUserInterface()
        {
            SetupTheme();
        }

        public virtual void SetupReactiveObservables()
        {
        }

        /// <summary>
        /// Setups the reactive translation.
        /// Used to can change at runtime translation
        /// </summary>
        /// <param name="disp">Disp.</param>
        public virtual void SetupReactiveTranslation(CompositeDisposable disp)
        {
        }

        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            RxController.SetupReactiveSubscriptions(this,disp);
        }
    }
}