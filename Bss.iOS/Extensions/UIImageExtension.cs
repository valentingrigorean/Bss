//
// UIImageExtension.cs
//
// Author:
//       Valentin <v.grigorean@bss-one.net>
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
// Analysis disable once CheckNamespace
using CoreGraphics;
using System;
using System.IO;
using CoreImage;
using Foundation;
using System.Threading;
using CoreAnimation;

namespace UIKit
{
    public enum ImageExtension
    {
        PNG,
        JPG
    }

    public enum BContentMode
    {
        ScaleToFill,
        ScaleToFit
    }

    public static class UIImageExtension
    {
        public static UIImage Resize(this UIImage img, int width, int height)
        {
            return Resize(img, new CGSize(width, height));
        }

        public static UIImage Resize(this UIImage img, CGSize newSize)
        {
            var rect = new CGRect(0, 0, newSize.Width, newSize.Height);
            try
            {
                UIGraphics.BeginImageContextWithOptions(newSize, false, 1f);
                img.Draw(rect);
                var newImg = UIGraphics.GetImageFromCurrentImageContext();
                return newImg;
            }
            finally
            {
                UIGraphics.EndImageContext();
            }
        }

        public static UIImage Resize(this UIImage img, nfloat maxSize,
                                     BContentMode contentMode = BContentMode.ScaleToFit)
        {
            var size = img.Size;
            var aspectWidth = maxSize / size.Width;
            var aspectHeight = maxSize / size.Height;
            var aspect = contentMode == BContentMode.ScaleToFit ?
                                                    Math.Min(aspectWidth, aspectHeight) :
                                                    Math.Max(aspectWidth, aspectHeight);
            return Resize(img, new CGSize(size.Width * aspect, size.Height * aspect));
        }

        public static UIImage MaskImage(this UIImage img, UIImage maskImage)
        {
            var maskRef = maskImage.CGImage;
            var mask = CGImage.CreateMask((int)maskRef.Width, (int)maskRef.Height,
                        (int)maskRef.BitsPerComponent, (int)maskRef.BitsPerPixel,
                        (int)maskRef.BytesPerRow, maskRef.DataProvider, null, false);
            var masked = img.CGImage.WithMask(mask);
            return new UIImage(masked);
        }

        public static UIImage MaskImageOnAlpha(this UIImage img, UIImage maskImg, bool inverse = false)
        {
            byte[] originalData, maskData;
            var originalCtx = CreateContext(img.Size, out originalData);
            var maskCtx = CreateContext(maskImg.Size, out maskData);
            originalCtx.DrawImage(new CGRect(0, 0, img.Size.Width, img.Size.Height), img.CGImage);
            maskCtx.DrawImage(new CGRect(0, 0, maskImg.Size.Width, maskImg.Size.Height), maskImg.CGImage);
            for (var i = 0; i < originalData.Length; i += 4)
            {
                var a = maskData[i + 3];
                if (inverse)
                {
                    if (a != 0)
                    {
                        originalData[i] = 0;
                        originalData[i + 1] = 0;
                        originalData[i + 2] = 0;
                        originalData[i + 3] = 0;
                    }
                }
                else
                {
                    if (a == 0)
                    {
                        originalData[i] = 0;
                        originalData[i + 1] = 0;
                        originalData[i + 2] = 0;
                        originalData[i + 3] = 0;
                    }
                }
            }
            var newImg = UIImage.FromImage(originalCtx.ToImage());
            originalCtx.Dispose();
            maskCtx.Dispose();
            return newImg;
        }

        public static UIImage ChangeColor(this UIImage img, UIColor color)
        {
            if (color == null)
                throw new ArgumentNullException(nameof(color));
            UIGraphics.BeginImageContextWithOptions(img.Size, false, img.CurrentScale);
            var ctx = UIGraphics.GetCurrentContext();
            color.SetFill();
            ctx.TranslateCTM(0, img.Size.Height);
            ctx.ScaleCTM(1f, -1f);
            var rect = new CGRect(CGPoint.Empty, img.Size);
            ctx.ClipToMask(rect, img.CGImage);
            ctx.FillRect(rect);
            var coloredImg = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return coloredImg;
        }

        public static string ToBase64(this UIImage img)
        {
            var nsdata = img.AsPNG().ToArray();
            return Convert.ToBase64String(nsdata);
        }

        public static UIImage ToGrayScale(this UIImage img)
        {
            var imgRect = new CGRect(0, 0, img.Size.Width, img.Size.Height);
            using (var colorSpace = CGColorSpace.CreateDeviceGray())
            {
                using (var ctx = new CGBitmapContext(
                    IntPtr.Zero, (nint)img.Size.Width, (nint)img.Size.Height, 8, 0,
                    colorSpace, CGImageAlphaInfo.None))
                {
                    ctx.DrawImage(imgRect, img.CGImage);
                    return new UIImage(ctx.ToImage());
                }
            }
        }

        public static UIImage ToImage(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            UIImage image;
            try
            {
                image = new UIImage(NSData.FromArray(data));
            }
            catch (Exception)
            {
                return null;
            }
            return image;
        }

        public static UIImage InvertColors(this UIImage img)
        {
            using (var coreImg = new CIImage(img.CGImage))
            {
                var filter = new CIColorInvert
                {
                    Image = coreImg
                };
                var output = filter.OutputImage;
                var ctx = CIContext.FromOptions(null);
                var cgimage = ctx.CreateCGImage(output, output.Extent);
                return UIImage.FromImage(cgimage);
            }
        }

        public static UIImage GetMaskFromAlpha(this UIImage img)
        {
            byte[] data;
            using (var ctx = CreateContext(img.Size, out data))
            {
                ctx.DrawImage(new CGRect(0, 0, img.Size.Width, img.Size.Height), img.CGImage);
                var white = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
                var black = new byte[] { 0xFF, 0x00, 0x00, 0x00 };

                for (var i = (nint)0; i < data.Length; i += 4)
                {
                    var a = data[i + 3];
                    if (a == 0)
                    {
                        white.CopyTo(data, (int)i);
                        continue;
                    }
                    var r = data[i];
                    var g = data[i + 1];
                    var b = data[i + 2];
                    if (r != 0 || g != 0 || b != 0)
                        black.CopyTo(data, i);
                }
                return UIImage.FromImage(ctx.ToImage());
            }
        }

        public static void SaveToFile(this UIImage img, string path, Action callback = null, ImageExtension extension = ImageExtension.PNG)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var data = extension == ImageExtension.JPG ? img.AsJPEG() : img.AsPNG();
                if (File.Exists(path))
                    File.Delete(path);
                File.WriteAllBytes(path, data.ToArray());
                callback?.Invoke();
            });
        }

        public static async System.Threading.Tasks.Task SaveToFileAsync(this UIImage img, string path, Action callback = null, ImageExtension extension = ImageExtension.PNG)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                var data = extension == ImageExtension.JPG ? img.AsJPEG() : img.AsPNG();
                File.WriteAllBytes(path, data.ToArray());
                callback?.Invoke();
            });
        }

        public static void SaveToFileScaled(this UIImage img, string path, Action callback = null, int size = 1024, ImageExtension extension = ImageExtension.PNG)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                img = Resize(img, (float)size, BContentMode.ScaleToFit);
                var data = extension == ImageExtension.JPG ? img.AsJPEG() : img.AsPNG();
                if (File.Exists(path))
                    File.Delete(path);
                File.WriteAllBytes(path, data.ToArray());
                callback?.Invoke();
            });
        }

        public static CGRect CropRectForImage(this UIImage img)
        {
            var cgImage = img.CGImage;
            var width = cgImage.Width;
            var height = cgImage.Height;
            var bytesPerRow = width * 4;
            var byteCount = bytesPerRow * height;

            var colorSpace = CGColorSpace.CreateDeviceRGB();

            var data = new byte[byteCount];
            using (var ctx = new CGBitmapContext(
                data, width, height, 8, bytesPerRow, colorSpace,
                CGImageAlphaInfo.PremultipliedLast))
            {
                var rect = new CGRect(0, 0, width, height);

                ctx.DrawImage(rect, cgImage);

                var lowX = width;
                var lowY = height;
                var highX = (nint)0;
                var highY = (nint)0;

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var pixelIndex = ((width * y) + x) * 4;
                        if (data[pixelIndex] != 0)
                        {
                            if (x < lowX) lowX = x;
                            if (x > highX) highX = x;
                            if (y < lowY) lowY = y;
                            if (y > highY) highY = y;
                        }
                    }
                }
                return new CGRect(lowX, lowY, highX - lowX, highY - lowY);
            }
        }

        public static UIImage GetSubImage(this UIImage img, CGRect rect)
        {
            UIGraphics.BeginImageContext(rect.Size);
            var ctx = UIGraphics.GetCurrentContext();

            var drawRect = new CGRect(-rect.X, -rect.Y, img.Size.Width, img.Size.Height);

            ctx.ClipToRect(drawRect);

            img.Draw(drawRect);

            var subImg = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return subImg;
        }

        public static CGBitmapContext CreateContext(CGSize size, out byte[] data)
        {
            return CreateContext(size, out data, CGImageAlphaInfo.PremultipliedFirst);
        }

        public static CGBitmapContext CreateContext(CGSize size, out byte[] data, CGImageAlphaInfo bitmapInfo)
        {
            var width = (int)size.Width;
            var height = (int)size.Height;
            var bytesPerRow = width * 4;
            var byteCount = bytesPerRow * height;

            var colorSpace = CGColorSpace.CreateDeviceRGB();

            data = new byte[byteCount];
            return new CGBitmapContext(data, width, height, 8,
                                       bytesPerRow, colorSpace, bitmapInfo);
        }

        public static UIImage FixOrientation(this UIImage This)
        {
            if (This.Orientation == UIImageOrientation.Up)
                return This;

            UIGraphics.BeginImageContextWithOptions(This.Size, false, This.CurrentScale);

            var rect = new CGRect(0, 0, This.Size.Width, This.Size.Height);

            This.Draw(rect);

            var normalImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return normalImage;
        }

        public static UIImage ScaleAndRotateImage(this UIImage imageIn, UIImageOrientation orIn)
        {
            const int kMaxResolution = 2048;

            var imgRef = imageIn.CGImage;
            float width = imgRef.Width;
            float height = imgRef.Height;

            var bounds = new CGRect(0, 0, width, height);

            if (width > kMaxResolution || height > kMaxResolution)
            {
                var ratio = width / height;

                if (ratio > 1)
                {
                    bounds.Width = kMaxResolution;
                    bounds.Height = bounds.Width / ratio;
                }
                else
                {
                    bounds.Height = kMaxResolution;
                    bounds.Width = bounds.Height * ratio;
                }
            }

            var scaleRatio = bounds.Width / width;
            var orient = orIn;

            var transform = GetTransformation(orIn, bounds);

            UIGraphics.BeginImageContext(bounds.Size);

            var context = UIGraphics.GetCurrentContext();

            if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
            {
                context.ScaleCTM(-scaleRatio, scaleRatio);
                context.TranslateCTM(-height, 0);
            }
            else
            {
                context.ScaleCTM(scaleRatio, -scaleRatio);
                context.TranslateCTM(0, -height);
            }

            context.ConcatCTM(transform);
            context.DrawImage(new CGRect(0, 0, width, height), imgRef);

            var imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }

        public static UIImage ToLayer(this CALayer layer)
        {
            UIGraphics.BeginImageContextWithOptions(layer.Bounds.Size, layer.Opaque, 0.0f);
            layer.RenderInContext(UIGraphics.GetCurrentContext());
            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return img;
        }

        private static CGAffineTransform GetTransformation(UIImageOrientation orientation, CGRect bounds)
        {
            var transform = CGAffineTransform.MakeIdentity();
            var imageSize = bounds.Size;
            nfloat boundHeight = 0;
            switch (orientation)
            {
                case UIImageOrientation.Up:                                        //EXIF = 1
                    transform = CGAffineTransform.MakeIdentity();
                    break;

                case UIImageOrientation.UpMirrored:                                //EXIF = 2
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0f);
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    break;

                case UIImageOrientation.Down:                                      //EXIF = 3
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
                    break;

                case UIImageOrientation.DownMirrored:                              //EXIF = 4
                    transform = CGAffineTransform.MakeTranslation(0f, imageSize.Height);
                    transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
                    break;

                case UIImageOrientation.LeftMirrored:                              //EXIF = 5
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, imageSize.Width);
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Left:                                      //EXIF = 6
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.RightMirrored:                             //EXIF = 7
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Right:                                     //EXIF = 8
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                default:
                    throw new Exception("Invalid image orientation");
            }

            return transform;
        }
    }
}