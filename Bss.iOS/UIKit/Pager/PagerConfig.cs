//
// GalleryConfig.cs
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
using System.ComponentModel;
using UIKit;

namespace Bss.iOS.UIKit.Pager
{
    public class PagerConfig : INotifyPropertyChanged
    {
        private bool _showPager;
        private UIColor _backgroundColor = UIColor.Clear;       
        private nfloat _cornerRadius = 0f;
        private nfloat _boderWidth = 0f;
        private UIColor _boderColor = UIColor.Black;
        private UIColor _dotSelectedColor = UIColor.FromRGB(66, 97, 155);
        private UIColor _dotDefaultColor = UIColor.White;
        private float _spaceBetweenPage = 20;

        public bool AllowSwipe { get; set; } = true;

        public float SpaceBetweenPage
        {
            get { return _spaceBetweenPage; }
            set
            {
                _spaceBetweenPage = value;
                NotifyPropertyChanged(nameof(SpaceBetweenPage));
            }
        }

        public nfloat CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = value;
                NotifyPropertyChanged("CornerRadius");
            }
        }

        public nfloat BorderWidth
        {
            get { return _boderWidth; }
            set
            {
                _boderWidth = value;
                NotifyPropertyChanged("BorderWidth");
            }
        }

        public UIColor BorderColor
        {
            get { return _boderColor; }
            set
            {
                _boderColor = value;
                NotifyPropertyChanged("BorderColor");
            }
        }

        public bool ShowDots
        {
            get { return _showPager; }
            set
            {
                _showPager = value;
                NotifyPropertyChanged("ShowDots");
            }
        }

        public UIColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        public UIColor DotSelectedColor
        {
            get { return _dotSelectedColor; }
            set
            {
                _dotSelectedColor = value;
                NotifyPropertyChanged("DotSelectedColor");
            }
        }
        public UIColor DotDefaultColor
        {
            get { return _dotDefaultColor; }
            set
            {
                _dotDefaultColor = value;
                NotifyPropertyChanged("DotDefaultColor");
            }
        }

        public UIPageViewControllerNavigationOrientation ScrollType { get; set; } =
            UIPageViewControllerNavigationOrientation.Horizontal;

        public UIPageViewControllerTransitionStyle TransitionStyle { get; set; } =
            UIPageViewControllerTransitionStyle.Scroll;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
