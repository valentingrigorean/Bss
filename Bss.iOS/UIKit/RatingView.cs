//
// FloatingRatingView.cs
//
// Author:
//       Valentin <v.grigorean@software-dep.net>
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

using UIKit;
using CoreGraphics;
using System;
using Foundation;
using System.ComponentModel;
using System.Collections.Generic;
using CoreAnimation;

namespace Bss.iOS.UIKit
{
    [Register("RatingView"), DesignTimeVisible(true)]
    public class RatingView : UIView
    {
        private IList<UIImageView> _emptyImageViews;
        private IList<UIImageView> _fullImageViews;

        private UIImage _emptyImage;
        private UIImage _fullImage;
        private UIViewContentMode _imageContentMode;

        private int _minRating;
        private int _maxRating;
        private nfloat _rating;

        private CGSize _currentSize;

        private FillModeType _fillMode;

        public RatingView()
        {
            Initialize();
        }

        public RatingView(IntPtr ptr)
            : base(ptr)
        {
            Initialize();
        }

        public RatingView(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        public RatingView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        public RatingView(NSObjectFlag t)
            : base(t)
        {
            Initialize();
        }

        public enum FillModeType
        {
            Whole,
            Half,
            Float
        }

        public event EventHandler<float> RatingChanged;

        [Export("EmptyImage"), Browsable(true)]
        public UIImage EmptyImage
        {
            get => _emptyImage;
            set
            {
                _emptyImage = value;
                foreach (var item in _emptyImageViews)
                    item.Image = _emptyImage;
                Refresh();
            }
        }

        [Export("FullImage"), Browsable(true)]
        public UIImage FullImage
        {
            get => _fullImage;
            set
            {
                _fullImage = value;
                foreach (var item in _fullImageViews)
                    item.Image = _fullImage;
                Refresh();
            }
        }

        [Export("MinRating"), Browsable(true)]
        public int MinRating
        {
            get => _minRating;
            set
            {
                if (_minRating <= value)
                    return;
                _minRating = value;
                Refresh();
            }
        }

        [Export("MaxRating"), Browsable(true)]
        public int MaxRating
        {
            get => _maxRating;
            set
            {
                if (value == _maxRating) return;
                _maxRating = value;
                InitView();
            }
        }

        [Export("Rating"), Browsable(true)]
        public nfloat Rating
        {
            get => _rating;
            set
            {
                if (!(Math.Abs(value - _rating) > 0.10)) return;
                _rating = value;
                Refresh();
            }
        }

        [Export("MinImageSize"), Browsable(true)]
        public CGSize MinImageSize { get; set; } = new CGSize(5.0f, 5.0f);

        [Export("Editable"), Browsable(true)]
        public bool Editable { get; set; }

        [Export("FillMode"), Browsable(true)]
        public FillModeType FillMode
        {
            get => _fillMode;
            set
            {
                var needRefresh = _fillMode != value;
                _fillMode = value;
                if (needRefresh)
                    Refresh();
            }
        }

        public override void AwakeFromNib()
        {
            InitView();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            var arr = touches.ToArray<UITouch>();
            if (arr != null && arr.Length > 0)
                HandleTouchAtLocation(arr[0].LocationInView(this));
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            var arr = touches.ToArray<UITouch>();
            if (arr != null && arr.Length > 0)
                HandleTouchAtLocation(arr[0].LocationInView(this));
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            RatingChanged?.Invoke(this, (float)Rating);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (EmptyImage == null)
                return;
            var desiredImageWidth = Bounds.Size.Width / _emptyImageViews.Count;
            var maxImageWidth = Math.Max(MinImageSize.Width, desiredImageWidth);
            var maxImageHeight = Math.Max(MinImageSize.Height, Bounds.Height);
            _currentSize = SizeForImage(EmptyImage,
                new CGSize(maxImageWidth, maxImageHeight));

            var imageXOffset = (Bounds.Width -
                               (_currentSize.Width * _emptyImageViews.Count)) /
                               (_emptyImageViews.Count - 1);
            for (var i = 0; i < MaxRating; i++)
            {
                var frame = new CGRect(i == 0 ? 0 : i * (imageXOffset + _currentSize.Width),
                                0, _currentSize.Width, _currentSize.Height);
                var imageView = _emptyImageViews[i];
                imageView.Frame = frame;

                imageView = _fullImageViews[i];
                imageView.Frame = frame;
            }
            Refresh();
        }

        private static CGSize SizeForImage(UIImage image, CGSize size)
        {
            var imageRatio = image.Size.Width / image.Size.Height;
            var viewRatio = size.Width / size.Height;
            nfloat rescale;
            if (imageRatio < viewRatio)
            {
                rescale = size.Height / image.Size.Height;
                var width = rescale * image.Size.Width;
                return new CGSize(width, size.Height);
            }
            rescale = size.Width / image.Size.Width;
            var height = rescale * image.Size.Height;
            return new CGSize(size.Width, height);
        }

        private void HandleTouchAtLocation(CGPoint touchLocation)
        {
            if (!Editable)
                return;
            nfloat newRating = 0f;
            for (var i = MaxRating - 1; i >= 0; i--)
            {
                var imageView = _emptyImageViews[i];
                if (touchLocation.X <= imageView.Frame.X) continue;
                var newLocation = imageView.
                    ConvertPointFromView(touchLocation, this);
                if (imageView.PointInside(newLocation, null) &&
                    (FillMode == FillModeType.Float || FillMode == FillModeType.Half))
                {
                    var decimalNum = newLocation.X / _currentSize.Width;

                    if (FillMode == FillModeType.Half)
                        newRating = i + (decimalNum > 0.75f ? 1f :
                            (decimalNum > 0.25f ? 0.5f : 0f));
                    else
                        newRating = i + decimalNum;
                }
                else
                    newRating = i + 1;
                break;
            }
            Rating = newRating < MinRating ? MinRating : newRating;
            RatingChanged?.Invoke(this, (float)Rating);
        }

        private void Refresh()
        {
            var rating = Rating;
            for (var i = 0; i < MaxRating; i++)
            {
                var imageView = _fullImageViews[i];

                if (rating >= (i + 1))
                {
                    imageView.Layer.Mask = null;
                    imageView.Hidden = false;
                    continue;
                }
                if ((rating > (float)i) && (rating < (float)(i + 1)))
                {
                    var maskLayer = new CALayer
                    {
                        Frame = new CGRect(0, 0, (rating - i) *
                                                 _currentSize.Width, _currentSize.Height),
                        BackgroundColor = UIColor.Black.CGColor
                    };
                    imageView.Layer.Mask = maskLayer;
                    imageView.Hidden = false;
                    continue;
                }
                imageView.Layer.Mask = null;
                imageView.Hidden = true;
            }
        }

        private void InitImageViews()
        {
            if (_emptyImageViews.Count != 0)
                return;
            for (var i = 0; i < MaxRating; i++)
            {
                var emptyImageView = new UIImageView
                {
                    ContentMode = _imageContentMode,
                    Image = EmptyImage
                };
                _emptyImageViews.Add(emptyImageView);
                AddSubview(emptyImageView);

                var fullImageView = new UIImageView
                {
                    ContentMode = _imageContentMode,
                    Image = FullImage
                };
                _fullImageViews.Add(fullImageView);
                AddSubview(fullImageView);
            }
        }

        private void RemoveImageViews()
        {
            for (var i = 0; i < _emptyImageViews.Count; i++)
            {
                var imageView = _emptyImageViews[i];
                imageView.RemoveFromSuperview();
                imageView = _fullImageViews[i];
                imageView.RemoveFromSuperview();
            }
            _emptyImageViews.Clear();
            _fullImageViews.Clear();
        }

        private void InitView()
        {
            RemoveImageViews();
            InitImageViews();
            SetNeedsLayout();
            Refresh();
        }

        private void Initialize()
        {
            BackgroundColor = UIColor.Clear;
            _imageContentMode = UIViewContentMode.ScaleAspectFit;
            _emptyImageViews = new List<UIImageView>();
            _fullImageViews = new List<UIImageView>();
            _minRating = 0;
            _maxRating = 5;
            InitImageViews();
            _rating = 2.5f;
            FillMode = FillModeType.Half;
            _emptyImage = UIImage.FromBundle("StarEmpty");
            _fullImage = UIImage.FromBundle("StartFull");
        }
    }
}