//
// GlobalLayoutLisener.cs
//
// Author:
//       Valentin Grigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 
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
using Android.Util;
using Android.Views;

namespace Bss.Droid.Views
{
    /// <summary>
    /// Global layout lisener.
    /// A class that is aim to help to lisen when a view get size/ or size change
    /// </summary>
    public class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private const string Tag = "GlobalLayoutLisener";
        private EventHandler _handler;
        private bool _wasRaise;
        private ViewTreeObserver _viewTreeObserver;
        private bool _wasRemoved;
        private View _view;

        public GlobalLayoutListener(View view, bool oneTime = false)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view), "view == null");
            _view = view;
            OneTime = oneTime;
            Init();
        }


        public View View => _view;

        public bool OneTime { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            RemoveLisener();
        }

        public event EventHandler GlobalLayout
        {
            add
            {
                if (_wasRaise)
                {
                    value(this, null);
                    return;
                }
                _handler += value;
            }

            remove
            {
                var temp = value;
                if (temp == null) return;
                _handler -= temp;
            }
        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            _handler?.Invoke(this, EventArgs.Empty);
            if (OneTime)
            {
                if (_wasRaise)
                    return;
                _wasRaise = true;
                RemoveLisener();
            }
            else
            {
                if (!_viewTreeObserver.IsAlive)
                {
                    _viewTreeObserver = _view.ViewTreeObserver;
                    _viewTreeObserver.AddOnGlobalLayoutListener(this);
                }
            }
        }

        private void RemoveLisener()
        {
            if (_wasRemoved)
                return;
            if (_view == null || _viewTreeObserver == null || !_viewTreeObserver.IsAlive)
            {
                NullEverything();
                return;
            }
            try
            {
                if ((int)Android.OS.Build.VERSION.SdkInt >= 16)
                    _viewTreeObserver.RemoveOnGlobalLayoutListener(this);
                else
#pragma warning disable CS0618 // Type or member is obsolete
                    _viewTreeObserver.RemoveGlobalOnLayoutListener(this);
#pragma warning restore CS0618 // Type or member is obsolete
                _wasRemoved = true;
                NullEverything();
            }
            catch (Exception ex)
            {
                Log.Warn(Tag, $"Failed to remove lisenere:{ex.Message}");
            }
        }

        private void NullEverything()
        {
            _view = null;
            _handler = null;
            _viewTreeObserver = null;
        }

        private void Init()
        {
            _viewTreeObserver = _view.ViewTreeObserver;
            if (_viewTreeObserver.IsAlive)
                _viewTreeObserver.AddOnGlobalLayoutListener(this);

        }
    }
}