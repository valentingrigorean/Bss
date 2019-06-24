﻿//
// RxSupportFragment.cs
//
// Author:
//       valentingrigorean <valentin.grigorean1@gmail.com>
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
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using ReactiveUI;
using Rx.Core;
using Rx.Core.ViewModels;
using Rx.Core.Views;

namespace Rx.Droid.App
{
    public abstract class RxSupportFragment<TViewModel> : RxSupportFragment, IViewFor<TViewModel>
        where TViewModel : class
    {
        TViewModel _viewModel;
        public virtual TViewModel ViewModel
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

    public abstract class RxSupportFragment : ReactiveUI.AndroidSupport.ReactiveFragment, IRxController
    {
        public CompositeDisposable SubscriptionDisposables { get; private set; } = new CompositeDisposable();

        public ILoadingView LoadingView { get; set; }

        public ISupportRxSubscription[] RxSubscriptions { get; set; }

        public abstract int ContentId { get; }

        private IDisposable _whenActivated;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _whenActivated?.Dispose();

            _whenActivated = this.WhenActivated(async (CompositeDisposable disp) =>
            {
                SetupReactiveTranslation(disp);
                SetupReactiveSubscriptions(disp);
                await InitialiazeAsync();
            });
        }

        readonly Subject<Tuple<int, Result, Intent>> activityResult = new Subject<Tuple<int, Result, Intent>>();
        public IObservable<Tuple<int, Result, Intent>> ActivityResult
        {
            get { return activityResult.AsObservable(); }
        }

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
            base.OnActivityResult(requestCode, resultCode, data);
            activityResult.OnNext(Tuple.Create(requestCode, (Result)resultCode, data));
		}		

        public Task<Tuple<Result, Intent>> StartActivityForResultAsync(Intent intent, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we
            // call ActivityForResult
            var ret = ActivityResult
                .Where(x => x.Item1 == requestCode)
                .Select(x => Tuple.Create(x.Item2, x.Item3))
                .FirstAsync()
                .ToTask();

            StartActivityForResult(intent, requestCode);
            return ret;
        }       

        public override void OnPause()
        {
            base.OnPause();
            SubscriptionDisposables.Clear();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            SubscriptionDisposables.Clear();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (ContentId == 0)
                throw new Exception("Invalid content id: " + ContentId);
            return inflater.Inflate(ContentId, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            RxDroidCompat.InjectAndCallUISetup(this, view);
        }

        public virtual Task<bool> DisplayAlert(string title, string message, string okButton, string cancelButton)
        {
            return RxController.DisplayAlert(title, message, okButton, cancelButton);
        }

        public Task HandleErrorAsync(InteractionContext<UserError, ErrorRecoveryOption> context)
        {
            return RxController.HandleErrorAsync(this, context);
        }

        public virtual void SetupReactiveObservables()
        {
        }

        public virtual Task InitialiazeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            RxController.SetupReactiveSubscriptions(this, disp);
        }

        public virtual void SetupReactiveTranslation(CompositeDisposable disp)
        {
        }

        public virtual void SetupTheme()
        {
        }

        public virtual void SetupUserInterface()
        {
            SetupTheme();
        }

        public virtual void ShowToast(string message, ToastLength duration = ToastLength.Short)
        {
            RxController.ShowToast(message, duration);
        }
    }
}