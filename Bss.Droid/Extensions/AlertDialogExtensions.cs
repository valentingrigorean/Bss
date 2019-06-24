//
// AlertDialogExtensions.cs
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
using System.Runtime.CompilerServices;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.App;

namespace Bss.Droid.Extensions
{
    public static class AlertDialogExtensions
    {
        private static ConditionalWeakTable<AlertDialog, ShowDialogDelegate> _weakTable = new ConditionalWeakTable<AlertDialog, ShowDialogDelegate>();


        public static AlertDialog SetButtonCallback(this AlertDialog This, DialogButtonType btn, Action<AlertDialog> callback)
        {
            if (This.IsShowing)
                throw new InvalidOperationException("Dialog is already show.");
            if (!_weakTable.TryGetValue(This, out ShowDialogDelegate del))
            {
                del = new ShowDialogDelegate();
                This.SetOnShowListener(del);
                _weakTable.Add(This, del);
            }
            del.AddButton(btn, callback);
            return This;
        }

        private class ShowDialogDelegate : Java.Lang.Object, IDialogInterfaceOnShowListener
        {
            private IDictionary<int, Action<AlertDialog>> _buttons = new Dictionary<int, Action<AlertDialog>>();

            public void AddButton(DialogButtonType btn, Action<AlertDialog> callback)
            {
                var key = (int)btn;
                if (_buttons.ContainsKey(key))
                    _buttons[key] = callback;
                else
                    _buttons.Add(key, callback);
            }

            public void OnShow(IDialogInterface dialog)
            {
                var alertDialog = dialog as AlertDialog;

                foreach (var btn in _buttons)
                {
                    var view = alertDialog.GetButton(btn.Key);
                    view.SetOnClickListener(new OnClickDelegate(alertDialog, btn.Value));
                }
            }
        }

        private class OnClickDelegate : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action<AlertDialog> _action;
            private readonly AlertDialog _dialog;

            public OnClickDelegate(AlertDialog dialog, Action<AlertDialog> action)
            {
                _action = action;
                _dialog = dialog;
            }


            public void OnClick(View view)
            {
                _action?.Invoke(_dialog);
            }
        }
    }
}
