﻿//
// InfiniteBlockHelper.cs
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
using System.Reactive.Linq;
using ReactiveUI;
using System.Diagnostics;

namespace Rx.Core.Utils
{
    public class InfiniteBlockHelper
    {
        private readonly Func<string> _errorMessageProvider;

        private string _errorMessage;

        private IDisposable _timer;

        public InfiniteBlockHelper(Func<string> errorMessageProvider)
        {
            _errorMessageProvider = errorMessageProvider;
        }

        public void Start(TimeSpan timeout)
        {
            _errorMessage = _errorMessageProvider();
            _timer?.Dispose();
            _timer = Observable
                .Timer(timeout, RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    Debug.WriteLine(_errorMessage);
                    throw new Exception(_errorMessage);
                });
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
