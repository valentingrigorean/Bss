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

namespace Rx.Core
{
    public enum DisplayType
    {
        Toast,
        Dialog
    }

    public class UserError
    {
        public UserError()
        {

        }

        public UserError(Exception error)
            : this(error, "Error", error.Message, "OK", null)
        {

        }

        public UserError(string title, string message, string accept)
            : this(null, title, message, accept, null)
        {

        }

        public UserError(string title, string message, string accept, string cancel)
            : this(null, title, message, accept, cancel)
        {

            Title = title;
            Message = message;
            OkButton = accept;
            CancelButton = cancel;
        }

        public UserError(Exception error, string title, string message, string accept, string cancel)
        {
            Error = error;
            Title = title;
            Message = message;
            OkButton = accept;
            CancelButton = cancel;
        }

        public Exception Error { get; }

        public DisplayType DisplayType { get; set; } = DisplayType.Toast;

        public string Title { get; set; }
        public string Message { get; set; }

        public string OkButton { get; set; }
        public string CancelButton { get; set; }

        public bool HaveCancelButton => !string.IsNullOrEmpty(CancelButton);
    }
}
