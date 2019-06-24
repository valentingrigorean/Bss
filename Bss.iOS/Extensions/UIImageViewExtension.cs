//
// UIImageViewExtension.cs
//
// Author:
//       Valentin <valentin.grigorean1@gmail.com>
//
// Copyright (c) 2016 Valentin
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
using Bss.iOS.Utils;
using CoreGraphics;
using Foundation;

namespace UIKit
{

    public static class UiImageViewExtension
    {
        public static void LoadImage(this UIImageView imgView, string urlString, UIImage placeHolder = null,
                                     Action callback = null)
        {
            if (ImageLoader.Default == null)
                throw new NullReferenceException("ImageLoder.Default was not initialize");
            if (string.IsNullOrEmpty(urlString))
            {
                imgView.Image = null;
                return;
            }
            var url = CheckIfFileOrHttp(urlString);
            if (url != null)
            {
                imgView.Image = placeHolder;
                ImageLoader.Default.LoadImage(imgView, urlString, (_img, err, _url) =>
                {
                    callback?.Invoke();
                });
            }
            var img = UIImage.FromFile(urlString);
            if (img == null) return;
            imgView.Image = img;
        }

        private static NSUrl CheckIfFileOrHttp(string url)
        {
            try
            {
                if (url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                    return new NSUrl(url);
                if (url.StartsWith("file", StringComparison.CurrentCultureIgnoreCase))
                    return new NSUrl(url);
                return NSBundle.MainBundle.GetUrlForResource(url, "png");
            }
            catch
            {
                return null;
            }
        }


        public static void ChangeImageColor(this UIImageView imgView, UIColor color)
        {
            if (imgView.Image != null)
                imgView.Image = imgView.Image.ChangeColor(color);
        }

        public static void ChangeHighlightedImageColor(this UIImageView imgView, UIColor color)
        {
            var img = imgView.HighlightedImage ?? imgView.Image;
            if (img == null)
                return;
            img = img.ChangeColor(color);
            imgView.HighlightedImage = img;
        }

        /// <summary>
        /// Loads the image view from base64.
        /// Nothing happen if string is null or empty
        /// </summary>
        /// <param name="imgView">Image view.</param>
        /// <param name="base64">Base64.</param>
        public static void LoadFromBase64(this UIImageView imgView, string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                imgView.Image = null;
                return;
            }
            var data = new NSData(base64, NSDataBase64DecodingOptions.None);
            var img = UIImage.LoadFromData(data);
            imgView.Image = img;
        }

        public static void CropImage(this UIImageView imgView)
        {
            var rect = imgView.Image.CropRectForImage();
            using (var cg = imgView.Image.CGImage.WithImageInRect(rect))
            {
                imgView.Image = UIImage.FromImage(cg);
            }
        }

        public static void AnimateHighlight(this UIImageView imgView, bool highlighted, float duration = 0.35f)
        {
            var expandTransform = CGAffineTransform.MakeScale(1.15f, 1.15f);
            var initDur = duration * 0.25f;
            var lastDur = duration * 0.75f;

            UIView.TransitionNotify(imgView, initDur, UIViewAnimationOptions.TransitionCrossDissolve,
                       () =>
               {
                   imgView.Highlighted = highlighted;
                   imgView.Transform = expandTransform;
               }, finished =>
               {
                   UIView.AnimateNotify(lastDur, 0.0f, 0.4f, 0.2f, UIViewAnimationOptions.CurveEaseOut, () =>
                        imgView.Transform = CGAffineTransform.CGAffineTransformInvert(expandTransform), null);
               });
        }

        public static void SetTint(this UIImageView imageView, UIColor color)
        {
            imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            imageView.TintColor = color;
        }

    }
}

