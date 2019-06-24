//
// RxImageView.cs
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
using UIKit;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foundation;

namespace Rx.iOS.RxViews
{
    [Register("RxImageView")]
    public class RxImageView : UIImageView, INotifyPropertyChanged
    {
        public RxImageView()
        {
        }

        public RxImageView(IntPtr handle)
            : base(handle)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Highlighted
        {
            get => base.Highlighted;
            set
            {
                if (base.Highlighted == value)
                    return;
                base.Highlighted = value;
                RaisePropertyChanged();
            }
        }

        public override bool Hidden
        {
            get => base.Hidden;           
            set
            {
                if (base.Hidden == value)
                    return;
                base.Hidden = value;
				RaisePropertyChanged();
            }
        }

        private void RaisePropertyChanged([CallerMemberName]string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}