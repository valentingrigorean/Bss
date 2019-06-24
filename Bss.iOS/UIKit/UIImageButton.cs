//
// ImageButton.cs
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
using UIKit;
using CoreGraphics;
using Foundation;
using System.ComponentModel;

namespace Bss.iOS.UIKit
{
    [Obsolete("Use UIButton")]
    [Register("UIImageButton"), DesignTimeVisible(true)]
    public class UIImageButton : UIImageView, IClickableView
    {
        private UIImage _image;
        private UIImage _highlighted;
        private UIImage _imageTint;
        private UIImage _highlightedTint;
        private bool _dontChange;
        private CTouch _currentState;

        private enum CTouch
        {
            None,
            Begin,
            Ended
        }

        public UIImageButton()
        {
            Initialize();
        }

        public UIImageButton(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public UIImageButton(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override UIImage Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
                if (_dontChange) return;
                _image = value;
                _imageTint = value.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
        }

        public override UIImage HighlightedImage
        {
            get
            {
                return base.HighlightedImage;
            }
            set
            {
                base.HighlightedImage = value;
                if (_dontChange) return;
                _highlighted = value;
                _highlightedTint = value?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
        }


        public bool Enabled { get; set; } = true;

        public event EventHandler Click = delegate
        {

        };

        public void SendActionForControlEvent()
        {
            if (!Enabled) return;
            SetCurrentImage(CTouch.Ended);
            Click?.Invoke(this, EventArgs.Empty);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (!Enabled) return;
#if DEBUG
            LogInfo("TouchBegin");
#endif
            SetCurrentImage(CTouch.Begin);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            if (!Enabled) return;
            if (_currentState == CTouch.Ended) return;
            var arr = touches.ToArray<UITouch>();
            var touch = arr[0];
            var state = PointInside(touch.LocationInView(touch.View), null) ?
                CTouch.Begin : CTouch.Ended;
            SetCurrentImage(state);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (!Enabled) return;
#if DEBUG
            LogInfo("TouchEnded");
#endif
            SetCurrentImage(CTouch.Ended);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            if (!Enabled) return;
#if DEBUG
            LogInfo("TouchEnded");
#endif
            SetCurrentImage(CTouch.Ended);
        }

        private void SetCurrentImage(CTouch cTouch)
        {
            if (cTouch == _currentState) return;
            _currentState = cTouch;
            _dontChange = true;
            CheckImages();
            switch (cTouch)
            {
                case CTouch.Begin:
                    if (Highlighted)
                        HighlightedImage = _highlightedTint;
                    else
                        Image = _imageTint;
                    break;
                case CTouch.Ended:
                    if (Highlighted)
                        HighlightedImage = _highlighted;
                    else
                        Image = _image;
                    break;
            }
#if DEBUG
            LogInfo("NewState " + cTouch);
#endif
            _dontChange = false;
        }

#if DEBUG
        public static bool Debugging { get; set; } = false;
#endif

#if DEBUG
        private void LogInfo(string log)
        {
            if (!Debugging) return;
            Console.WriteLine("\nUIImageButton Info:\n   {0}:{1}", DateTime.Now.ToString("hh:mm:ss.fff"), log);
        }
#endif

        private void CheckImages()
        {
            if (Image != null && _imageTint == null)
            {
                _imageTint = Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                _image = Image;
            }
            if (HighlightedImage != null && _highlightedTint == null)
            {
                _highlighted = HighlightedImage;
                _highlightedTint = HighlightedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
        }

        private void Initialize()
        {
            this.OnClick(SendActionForControlEvent);
            ContentMode = UIViewContentMode.ScaleAspectFit;
        }
    }
}

