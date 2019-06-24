//
// RxViewHolder.cs
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
using System.Threading;
using Android.Views;
using ReactiveUI;
using ReactiveUI.Android.Support;
using Rx.Core;
using Rx.Droid.App;
using Rx.Extensions;

namespace Rx.Droid.RxViews
{
    public abstract class RxViewHolder<T> : ReactiveRecyclerViewViewHolder<T>, ISupportRxUI, ICanSupportRxSubscription
        where T : class, IReactiveObject
    {
        private CompositeDisposable _subscriptionDisposables = new CompositeDisposable();
        private IDisposable _inner;
        private IDisposable _activated;

        protected RxViewHolder(View view) : base(view)
        {
            Initialiaze(view);
        }

        protected CompositeDisposable SubscriptionDisposables => _subscriptionDisposables;

        public virtual bool ItemSelected
        {
            get => ItemView.Selected;
            set => ItemView.Selected = value;
        }

        public ISupportRxSubscription[] RxSubscriptions { get; set; }

        protected override void Dispose(bool disposing)
        {
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
            Interlocked.Exchange(ref _subscriptionDisposables, new CompositeDisposable()).Dispose();
            base.Dispose(disposing);
        }

        public virtual void PrepareForReuse()
        {
        }

        public virtual void SetupUserInterface()
        {
            SetupTheme();
        }

        public virtual void SetupTheme()
        {
        }

        public virtual void SetupReactiveObservables()
        {
        }

        /// <summary>
        /// Setups the reactive translation.
        /// Used to can change at runtime translation
        /// </summary>
        /// <param name="disp">Disp.</param>
        public virtual void SetupReactiveTranslation(CompositeDisposable disp) { }

        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            RxController.SetupReactiveSubscriptions(this, disp);
        }

        private void Initialiaze(View view)
        {
            RxDroidCompat.InjectAndCallUISetup(this, view);

            _inner = this
                .WhenAnyValue(v => v.ViewModel)
                .Do(vm => PrepareForReuse())
                .Where(vm => vm != null)
                .Do(vm =>
                {
                    _activated?.Dispose();
                    var supportActivation = vm as ISupportsActivation;
                    _activated = supportActivation?.Activator.Activate();

                    SubscriptionDisposables.Clear();
                    SetupReactiveTranslation(SubscriptionDisposables);
                    SetupReactiveSubscriptions(SubscriptionDisposables);
                }).Subscribe();

        }
    }
}