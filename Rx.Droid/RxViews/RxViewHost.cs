//
// RxViewHost.cs
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
using System.Reactive.Linq;
using Android.Content;
using Android.Views;
using ReactiveUI;
using Rx.Core;
using Rx.Core.Views;
using Rx.Droid.App;
using Rx.Extensions;
using Rx.Core.ViewModels;
using Rx.Core.Extensions;

namespace Rx.Droid.RxViews
{
    public abstract class RxViewHost<T> : ReactiveViewHost<T>, IDisposable, ISupportRxUI, ICanSupportRxSubscription
        where T : ReactiveObject
    {
        private readonly ChangeableLoadingView _loadingView = new ChangeableLoadingView();
        private View _view;
        private IDisposable _activated;

        protected RxViewHost()
        {
            Initialize();
        }

        protected RxViewHost(View view)
        {
            Initialize();
            View = view;
        }

        protected RxViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = false)
        {
            Initialize();
            var inflater = LayoutInflater.FromContext(ctx);
            View = inflater.Inflate(layoutId, parent, attachToRoot);

            if (performAutoWireup)
                this.WireUpControls();
        }

        public CompositeDisposable SubscriptionDisposables { get; } = new CompositeDisposable();

        public new View View
        {
            get => _view;
            set
            {
                _view = value;
                base.View = value;
                RxDroidCompat.InjectAndCallUISetup(this, View);
            }
        }

        public ILoadingView LoadingView
        {
            get => _loadingView.LoadingView;
            set => _loadingView.LoadingView = value;
        }

        public ISupportRxSubscription[] RxSubscriptions { get; set; }

        public void Dispose()
        {
            SubscriptionDisposables.Dispose();
            _activated?.Dispose();
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
        public virtual void SetupReactiveObservables() { }

        /// <summary>
        /// Setups the reactive translation.
        /// Used to can change at runtime translation
        /// </summary>
        /// <param name="disp">Disp.</param>
        public virtual void SetupReactiveTranslation(CompositeDisposable disp) { }

        /// <summary>
        /// Setups the reactive subscriptions.
        /// Used to set events/observable might be called  1 or more times
        /// </summary>
        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            RxController.SetupReactiveSubscriptions(this, disp);
        }

        private void Initialize()
        {
            this.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Subscribe(vm =>
                {
                    SubscriptionDisposables.Clear();
                    SetupReactiveTranslation(SubscriptionDisposables);
                    SetupReactiveSubscriptions(SubscriptionDisposables);

                    _activated?.Dispose();
                    var supportActivation = vm as ISupportsActivation;
                    _activated = supportActivation?.Activator.Activate();

                    var viewModelBase = vm as ViewModelBase;
                    if (viewModelBase != null)
                    {
                        viewModelBase.BindLoadingView(_loadingView)
                                     .DisposeWith(SubscriptionDisposables);
                    }
                });
        }
    }
}