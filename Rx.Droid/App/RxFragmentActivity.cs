//
// RxFragmentActivity.cs
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
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using ReactiveUI;
using Rx.Core;
using Rx.Core.ViewModels;
using Rx.Core.Views;

namespace Rx.Droid.App
{   
    public abstract class RxFragmentActivity<TViewModel> : RxActivity, IViewFor<TViewModel>
       where TViewModel : class
    {
        TViewModel _viewModel;
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

    public abstract class RxFragmentActivity
        : ReactiveFragmentActivity, IRxController
    {
        public ISupportRxSubscription[] RxSubscriptions { get; set; }

        public ILoadingView LoadingView { get; set; }

        public CompositeDisposable SubscriptionDisposables { get; private set; } = new CompositeDisposable();

        private IDisposable _whenActivated;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _whenActivated?.Dispose();

            _whenActivated = this.WhenActivated((CompositeDisposable disp) =>
            {
                SetupReactiveTranslation(disp);
                SetupReactiveSubscriptions(disp);
            });
        }

        protected override void OnPause()
        {
            base.OnPause();
            SubscriptionDisposables.Clear();
        }

        public override void SetContentView(int layoutResID)
        {
            base.SetContentView(layoutResID);
            RxDroidCompat.InjectAndCallUISetup(this, Window.DecorView);
        }

        public override void SetContentView(View view)
        {
            base.SetContentView(view);
            RxDroidCompat.InjectAndCallUISetup(this, view);
        }

        public override void SetContentView(View view, ViewGroup.LayoutParams @params)
        {
            base.SetContentView(view, @params);
            RxDroidCompat.InjectAndCallUISetup(this, view);
        }

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

        public Task HandleErrorAsync(InteractionContext<UserError, ErrorRecoveryOption> context)
        {
            return RxController.HandleErrorAsync(this, context);
        }

        /// <summary>
        /// Setups the user interface.
        /// Its called only once on created View
        /// </summary>
        public virtual void SetupUserInterface()
        {
            SetupTheme();
        }

        public virtual void SetupTheme()
        {
        }

        /// <summary>
        /// Setups the reactive observables.
        /// Its called only once on created View
        /// </summary>
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

        /// <summary>
        /// Setups the reactive subscriptions.
        /// Used to set events/observable might be called  1 or more times
        /// </summary>
        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            RxController.SetupReactiveSubscriptions(this, disp);
        }
    }
}
