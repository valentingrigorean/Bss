﻿//
// RxDialog.cs
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
using System.Threading.Tasks;
using Android.App;
using Rx.Core.Views;
using Splat;
using Android.OS;

namespace Rx.Droid.App
{
    internal class RxDialog : IDialog
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public string OkButtonText { get; set; } = "Ok";
        public string CancelButtonText { get; set; }

        public Task DismissAsync()
        {
            return Task.FromResult(true);
        }

        public Task<bool> ShowAsync()
        {
            var tcs = new TaskCompletionSource<bool>();           

            var handler = new Handler(Looper.MainLooper);

            handler.Post(()=>CreateAndShowDialog(tcs));

            return tcs.Task;
        }

        private void CreateAndShowDialog(TaskCompletionSource<bool> tcs)
        {
            var currentActivityProvider = Locator.Current.GetService<ICurrentActivityProvider>();

            if (currentActivityProvider == null || currentActivityProvider.CurrentActivity == null)
                throw new Exception("ICurrentActivityProvider not injected or current activity is null");

            var builder = new AlertDialog.Builder(currentActivityProvider.CurrentActivity);

            builder.SetTitle(Title)
                   .SetMessage(Message)
                   .SetPositiveButton(OkButtonText, (sender, e) => tcs.TrySetResult(true));

            if (!string.IsNullOrEmpty(CancelButtonText))
                builder.SetNegativeButton(CancelButtonText, (sender, e) => tcs.TrySetResult(false));

            var dialog = builder.Create();

            dialog.Show();
        }
    }
}
