//
// ViewModelBase.cs
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
using ReactiveUI;
using Rx.Core.Views;
using Splat;
using System.Reactive.Linq;

namespace Rx.Core.ViewModels
{
    public enum ErrorRecoveryOption
    {
        /// <summary>
        /// OkButton
        /// </summary>
        Retry,
        /// <summary>
        /// Cancel
        /// </summary>
        Abort
    }

    public class ViewModelBase : ReactiveObject, ISupportsActivation, IEnableLogger
    {
        private bool _isBusy;
        private bool _isRefreshing;
        private string _urlPathSegment = String.Empty;
        private bool _isInitialize;

        public ViewModelBase() : this(null)
        {
        }

        public ViewModelBase(IObservable<string> reactiveTitle)
        {
            ReactiveTitle = reactiveTitle;

            this.WhenActivated(async (CompositeDisposable disp) =>
            {
                IsActivated = false;
                SetupReactiveSubscriptions(disp);
                IsActivated = true;
                if (!_isInitialize)
                {
                    _isInitialize = true;
                    await InitializeAsync();
                }
            });
        }

        protected bool IsActivated { get; private set; }

        public Action GoBack { get; set; }

        /// <summary>
        /// Gets or sets the reactive title.
        /// Will set UrlPathSegment when this change
        /// </summary>
        /// <value>The reactive title.</value>
        public IObservable<string> ReactiveTitle { get; }

        public static Interaction<UserError, ErrorRecoveryOption> Errors { get; } =
            new Interaction<UserError, ErrorRecoveryOption>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Genesys.Core.ViewModels.ViewModelBase"/> is busy.
        /// </summary>
        /// <value><c>true</c> if is busy; otherwise, <c>false</c>.</value>
        public virtual bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Genesys.Core.ViewModels.ViewModelBase"/> is refreshing.
        /// its used when refreshing items
        /// </summary>
        /// <value><c>true</c> if is refreshing; otherwise, <c>false</c>.</value>
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => this.RaiseAndSetIfChanged(ref _isRefreshing, value);
        }

        /// <summary>
        /// Get or set the Title
        /// </summary>
        /// <value></value>
        public string UrlPathSegment
        {
            get => _urlPathSegment;
            set => this.RaiseAndSetIfChanged(ref _urlPathSegment, value);
        }

        /// <summary>
        /// Used to initialiaze ViewModel its called after SetupReactiveSubscription
        /// Called only once
        /// </summary>
        /// <returns>The async.</returns>
        public virtual Task InitializeAsync()
        {
            return Task.FromResult(true);
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public virtual void SetupReactiveSubscriptions(CompositeDisposable disp)
        {
            ReactiveTitle?.Subscribe(text => UrlPathSegment = text)
                          .DisposeWith(disp);
        }

        /// <summary>
        /// Shows the toast.
        /// </summary>
        /// <param name="message">Message.</param>
        public void ShowToast(string message, ToastLength duration = ToastLength.Short)
        {
            var toast = Locator.Current.GetService<IToast>();
            toast?.ShowMessage(message, duration);
        }

        /// <summary>
        /// Wired the command
        /// Binds the command IsExecuting to IsRefreshing.
        /// </summary>
        /// <returns>The command to refreshing.</returns>
        /// <param name="command">Command.</param>
        public IDisposable BindCommandToIsRefreshing(ReactiveCommand command)
        {
            var compositeDisp = new CompositeDisposable();

            command.IsExecuting
                   .Subscribe(isBusy => IsRefreshing = isBusy)
                   .DisposeWith(compositeDisp);

            return compositeDisp;
        }

        /// <summary>
        /// Wires the command.
        /// Binds the command IsExecuting to IsBusy.
        /// </summary>
        /// <param name="command">command.</param>
        public IDisposable BindCommandToIsBusy(ReactiveCommand command)
        {
            var compositeDisp = new CompositeDisposable();

            command.IsExecuting
                   .Subscribe(isBusy => IsBusy = isBusy)
                   .DisposeWith(compositeDisp);

            return compositeDisp;
        }

        /// <summary>
        /// Wires the command.
        /// Binds the command ThrownExceptions to Errors.
        /// </summary>
        /// <param name="command">command.</param>
        public IDisposable BindCommandToUserError(ReactiveCommand command, UserError error)
        {
            var compositeDisp = new CompositeDisposable();

            command.ThrownExceptions
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .LoggedCatch(this)
                   .Subscribe(ex =>
                   {
                       Errors.Handle(error)
                             .Subscribe()
                             .DisposeWith(compositeDisp);
                   })
                   .DisposeWith(compositeDisp);

            return compositeDisp;
        }

        /// <summary>
        /// Wires the command.
        /// Binds the command ThrownExceptions to Errors.
        /// </summary>
        /// <param name="command">command.</param>
        public IDisposable BindCommandToIsBusyAndUserError(ReactiveCommand command, UserError error)
        {
            return new CompositeDisposable(BindCommandToIsBusy(command),
                                           BindCommandToUserError(command,error));           
        }
    }
}