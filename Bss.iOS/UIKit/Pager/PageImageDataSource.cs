//
// PageImageDataSource.cs
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
using System.Collections.Generic;
using UIKit;

namespace Bss.iOS.UIKit.Pager
{

    public class PageImageDataSource : PagerDataSource
    {
        private readonly IList<string> _images;
        private readonly UIViewContentMode _contentMode;

        public PageImageDataSource(IList<string> images, UIViewContentMode contentMode)
        {
            _images = images;
            _contentMode = contentMode;
        }

        public override int Count => _images.Count;
        public delegate UIImage PlaceHolderForCallback(int pos);
        public PlaceHolderForCallback PlaceHolderFor { get; set; }

        public LoadImageCallback LoadImageCallback { get; set; }

        public override UIViewController GetPage(PagerViewController galleryViewController, int pos)
        {
            var item = new ImageGalleryItemViewController();
            item.ImageMode = _contentMode;
            item.Index = pos;
            item.PlaceHolder = PlaceHolderFor?.Invoke(pos);
            item.LoadImageCallback = LoadImageCallback;
            item.Url = _images[pos];
            return item;
        }
    }
}
