//
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

using System.Threading.Tasks;
using Rx.Core.Views;
using UIKit;
using Bss.iOS;
using CoreFoundation;

namespace Rx.iOS.RxViews
{
    public class RxDialog : IDialog
    {
        private UIAlertController _alertController;

        public string Message { get; set; }
        public string Title { get; set; }
        public string OkButtonText { get; set; } = "Ok";
        public string CancelButtonText { get; set; }

        public Task DismissAsync()
        {
            if (_alertController == null)
                return Task.FromResult(true);

            var tcs = new TaskCompletionSource<bool>();

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        public Task<bool> ShowAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                _alertController = UIAlertController.Create(
                Title, Message, UIAlertControllerStyle.Alert);

                _alertController.AddAction(UIAlertAction.Create(
                    OkButtonText, UIAlertActionStyle.Default, (obj) => tcs.SetResult(true)));

                if (!string.IsNullOrEmpty(CancelButtonText))
                    _alertController.AddAction(UIAlertAction.Create(
                        CancelButtonText, UIAlertActionStyle.Cancel, (obj) => tcs.SetResult(false)));

                Application.TopViewController.PresentViewController(_alertController, true, null);
            });

            return tcs.Task;
        }
    }
}
