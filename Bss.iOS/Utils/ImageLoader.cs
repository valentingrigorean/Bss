//
// ImageLoader.cs
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
using System.Threading;
using Foundation;

namespace Bss.iOS.Utils
{
    public static class ImageLoader
    {
        private static IImageLoader _default = new ImageLoaderInternal();

        public static IImageLoader Default
        {
            get { return _default; }
            set { _default = value ?? throw new ArgumentNullException(nameof(Default)); }
        }

        private class ImageLoaderInternal : IImageLoader
        {
            public void LoadImage(UIImageView imgView, string url, ImageCompletionHandler completionBlock)
            {
                if (imgView == null) throw new ArgumentNullException(nameof(imgView));
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var nsUrl = new NSUrl(url);
                    NSError err = null;
                    var data = NSData.FromUrl(nsUrl, NSDataReadingOptions.MappedAlways, out err);

                    imgView.InvokeOnMainThread(() =>
                    {
                        if (err != null)
                        {
                            completionBlock?.Invoke(null, err, url);
                            return;
                        }
                        var img = UIImage.LoadFromData(data);
                        imgView.Image = img;
                        completionBlock?.Invoke(img, null, url);
                    });


                });
            }
        }
    }
}
