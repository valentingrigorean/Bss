//
// IRxController.cs
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
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using Rx.Core.ViewModels;
using Rx.Core.Views;
using System;

namespace Rx.Core
{
    public interface ISupportRxSubscription
    {
        /// <summary>
        /// Setups the reactive subscriptions.
        /// </summary>
        /// <returns>The reactive subscriptions.</returns>
        void SetupReactiveSubscriptions(CompositeDisposable disp);
    }

    public interface ICanSupportRxSubscription
    {
        ISupportRxSubscription[] RxSubscriptions { get; set; }
    }

    public interface ISupportRxUI : ISupportTheme, ISupportRxSubscription
    {
        /// <summary>
        /// Setups the user interface.
        /// Its called only once on created View
        /// </summary>
        void SetupUserInterface();

        /// <summary>
        /// Setups the reactive observables.
        /// Its called only once on created View
        /// </summary>
        void SetupReactiveObservables();

        /// <summary>
        /// Setups the reactive translation.
        /// Used to can change at runtime translation
        /// </summary>
        /// <param name="disp">Disp.</param>
        void SetupReactiveTranslation(CompositeDisposable disp);
    }

    public interface ISupportLoadingView
    {
        ILoadingView LoadingView { get; }
    }

    public interface IBackNavigation
    {
        Action GetBackAction(object context);
    }   

    public interface IRxController : ISupportRxUI, ISupportLoadingView, IActivatable, ICanActivate, ICanSupportRxSubscription
    {
        CompositeDisposable SubscriptionDisposables { get; }

        Task<bool> DisplayAlert(string title, string message, string okButton, string cancelButton);

        /// <summary>
        /// Handles the error async.
        /// Set context.SetOutput(true) if u handled the error
        /// </summary>
        /// <returns>The error async.</returns>
        /// <param name="context">Context.</param>
        Task HandleErrorAsync(InteractionContext<UserError, ErrorRecoveryOption> context);

        void ShowToast(string message, ToastLength duration = ToastLength.Short);
    }
}