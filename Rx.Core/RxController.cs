//
// RxController.cs
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
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using Rx.Core.ViewModels;
using Rx.Core.Views;
using Splat;

namespace Rx.Core
{
    public static class RxController
    {
        public static Task<bool> DisplayAlert(string title, string message, string okButton, string cancelButton)
        {
            var dialog = Locator.Current.GetService<IDialog>();

            dialog.Title = title;

            dialog.Message = message;
            dialog.OkButtonText = okButton;
            dialog.CancelButtonText = cancelButton;

            return dialog.ShowAsync();
        }

        /// <summary>
        /// Handles the error async.
        /// Set context.SetOutput(true) if u handled the error
        /// </summary>
        /// <returns>The error async.</returns>
        /// <param name="context">Context.</param>
        public static async Task HandleErrorAsync(IRxController controller, InteractionContext<UserError, ErrorRecoveryOption> context)
        {
            if (context.IsHandled)
                return;
            var userError = context.Input;

            var result = true;
            switch (userError.DisplayType)
            {
                case DisplayType.Dialog:
                    if (userError.HaveCancelButton)
                        result = await controller.DisplayAlert(userError.Title, userError.Message, userError.OkButton, userError.CancelButton);
                    else
                        await controller.DisplayAlert(userError.Title, userError.Message, userError.OkButton, "");
                    break;
                case DisplayType.Toast:
                    controller.ShowToast(userError.Message);
                    break;
            }
            context.SetOutput(result ? ErrorRecoveryOption.Retry : ErrorRecoveryOption.Abort);
        }

        public static void ShowToast(string msg, ToastLength duration = ToastLength.Short)
        {
            var toast = Locator.Current.GetService<IToast>();
            toast?.ShowMessage(msg, duration);

            if (toast == null)
                Debug.WriteLine("Warning: IToast was not register");
        }

        public static void SetupReactiveSubscriptions(ISupportRxUI context, CompositeDisposable disp)
        {
            AppTheme.Instance
                    .NotifyWhenThemeChange(context)
                    .DisposeWith(disp);

            if (context is ICanSupportRxSubscription supportRxSubs && supportRxSubs.RxSubscriptions != null)
            {
                foreach (var subs in supportRxSubs.RxSubscriptions)
                    subs.SetupReactiveSubscriptions(disp);
            }

            InitViewModel(context).DisposeWith(disp);
        }

        public static void SetupReactiveSubscriptions(IRxController context, CompositeDisposable disp)
        {
            ViewModelBase.Errors
                         .RegisterHandler(context.HandleErrorAsync)
                         .DisposeWith(disp);

            SetupReactiveSubscriptions((ISupportRxUI)context, disp);
        }

        private static IDisposable InitViewModel(object context)
        {
            var viewFor = context as IViewFor;
            var viewModel = viewFor?.ViewModel as ViewModelBase;
            if (viewModel == null)
                return Disposable.Empty;

            var disp = new CompositeDisposable();

            if (context is ISupportLoadingView loadingContext)
            {
                viewModel.WhenAnyValue(vm => vm.IsBusy)
                         .Subscribe(isBusy =>
                         {
                             if (isBusy)
                                 loadingContext.LoadingView?.Show();
                             else
                                 loadingContext.LoadingView?.Hide();
                         }).DisposeWith(disp);

                Disposable.Create(()=>loadingContext.LoadingView?.Hide())
                          .DisposeWith(disp);
            }

            var backNavigationProvider = Locator.Current.GetService<IBackNavigation>();

            if(backNavigationProvider != null)
            {
                viewModel.GoBack = backNavigationProvider.GetBackAction(context);

                Disposable.Create(() => viewModel.GoBack = null)
                          .DisposeWith(disp);
            }          

            return disp;
        }
    }
}