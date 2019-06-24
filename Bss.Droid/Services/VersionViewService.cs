//
// VersionViewService.cs
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Bss.Graphics.Drawables;
using Android.Graphics;
using Java.Interop;

namespace Bss.Droid.Services
{
    [Service]
    public class VersionViewService : Service
    {
        private IWindowManager _windowManager;
        private TextView _textView;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();

            var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);

            _textView = new TextView(this);
            _textView.SetTextColor(Color.White);
            _textView.Text = $"VersionName:{packageInfo.VersionName}\nVersionCode:{packageInfo.VersionCode}";
            _textView.SetPadding(8.DpToPixel(), 8.DpToPixel(), 8.DpToPixel(), 8.DpToPixel());
            _textView.Background = new BorderDrawable
            {
                Radius = 8.DpToPixel(),
                FillColor = Color.Black
            };

            _textView.Alpha = 0.5f;         
           
            var layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent,
                WindowManagerTypes.SystemAlert,
                WindowManagerFlags.NotFocusable | WindowManagerFlags.NotTouchable,
                Format.Translucent);

            layoutParams.Gravity = GravityFlags.Right | GravityFlags.Bottom;

            _windowManager.AddView(_textView, layoutParams);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_textView != null)
            {
                _windowManager.RemoveView(_textView);
                _textView = null;
            }
        }
    }

    public static class ShowVersionHelper
    {
        private static bool _isRunning;

        public static void Start(Context context)
        {
            if (_isRunning)
                return;
            var intent = new Intent(context, typeof(VersionViewService));
            context.StartService(intent);
            _isRunning = true;
        }

        public static void Stop(Context context)
        {
            if (!_isRunning)
                return;
            var intent = new Intent(context, typeof(VersionViewService));
            context.StopService(intent);
            _isRunning = false;
        }
    }
}
