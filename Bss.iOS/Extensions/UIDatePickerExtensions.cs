//
// UIDatePickerExtensions.cs
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
using Foundation;
using UIKit;

namespace Bss.iOS.Extensions
{
    public static class UIDatePickerExtensions
    {
        public static void SetCurrentDate(this UIDatePicker This, DateTime date)
        {
            This.Date = DateSpecifyKind(date).ToNsDate();
        }

        public static DateTime GetCurrentDate(this UIDatePicker This)
        {
            return DateSpecifyKind(This.Date.ToDateTime()).ToLocalTime();
        }

        public static void SetMaxDate(this UIDatePicker This, DateTime date)
        {
            This.MaximumDate = DateSpecifyKind(date).ToNsDate();
        }

        public static void SetMinDate(this UIDatePicker This, DateTime date)
        {
            This.MinimumDate = DateSpecifyKind(date).ToNsDate();
        }

        private static DateTime DateSpecifyKind(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}
