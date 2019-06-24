//
// Foreground.cs
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
using Android.OS;
using Java.Lang;
using System.Collections.Generic;
using Bss.Core.Utils;

namespace Bss.Droid.Utils
{
    public class Foreground
    {
        private readonly Handler _handler = new Handler();
        private readonly IList<IListener> _listeners = new List<IListener>();

        private Runnable _check;

        private bool _paused;

        public interface IListener
        {
            void OnBecameForeground();
            void OnBecameBackground();
        }

        public bool IsForeground { get; private set; }
        public bool IsBackground => !IsForeground;

        public IDisposable AddListener(IListener listener)
        {
            _listeners.Add(listener);
            return Disposable.Create(() => RemoveListener(listener));
        }

        public void RemoveListener(IListener listener)
        {
            _listeners.Remove(listener);
        }

        public void OnResumed()
        {
            _paused = false;
            var wasBackground = !IsForeground;
            IsForeground = true;
            if (_check != null)
                _handler.RemoveCallbacks(_check);
            if(wasBackground)
            {
                foreach (var listener in _listeners)
                    listener.OnBecameForeground();
            }
        }

        public void OnPaused()
        {
            _paused = true;
            if (_check != null)
                _handler.RemoveCallbacks(_check);

            _check = new Runnable(() =>
            {
                if (IsForeground && _paused)
                {
                    IsForeground = false;
                    foreach (var listener in _listeners)
                        listener.OnBecameBackground();
                }
            });

            _handler.PostDelayed(_check, 500);
        }
    }
}
