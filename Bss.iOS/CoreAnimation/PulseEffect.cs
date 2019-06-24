//
// PulseEffect.cs
//
// Author:
//       valentingrigorean <valentin.grigorean1@gmail.com>
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
using CoreAnimation;
using UIKit;
using CoreGraphics;

namespace Bss.iOS.CoreAnimation
{
	public class PulseEffect : CAShapeLayer
	{
		private readonly CAAnimationGroup _annimation;




		private PulseEffect(CGPoint center, float radius, float scaleFrom,
							float scaleTo, float alphaFrom, float alphaTo,
							UIColor fillColor, double duration, int repet)
		{
			Path = UIBezierPath.FromArc(center, radius, 0, new nfloat(360 * (Math.PI / 180)), true).CGPath;

			var scaleAnimation = CABasicAnimation.FromKeyPath("transform.scale.xy");
			scaleAnimation.From = scaleFrom.ToNative();
			scaleAnimation.To = scaleTo.ToNative();

			var alphaAnimation = CABasicAnimation.FromKeyPath("opacity");
			alphaAnimation.From = alphaFrom.ToNative();
			alphaAnimation.To = alphaTo.ToNative();

			var animationGroup = new CAAnimationGroup();
			animationGroup.Animations = new[] { scaleAnimation, alphaAnimation };
			animationGroup.Duration = duration;

			animationGroup.RepeatCount = repet < 0 ? float.MaxValue : repet;


			FillColor = fillColor.CGColor;

			_annimation = animationGroup;
		}

		public void StartAnimation()
		{
			AddAnimation(_annimation, AnimationKey);
		}

		public void StopAnimation()
		{
			RemoveAnimation(AnimationKey);
		}

		public const string AnimationKey = "pulse";

		public class Builder
		{
			private CGPoint _center = CGPoint.Empty;
			private float _radius = 100;


			private float _scaleFrom = 0f;
			private float _scaleTo = 1f;

			private float _alphaFrom = 0.8f;
			private float _alphaTo = 0f;

			private UIColor _fillColor = UIColor.FromRGB(0.193f, 0.577f, 0.775f);

			private double _duration = 0.3;

			private int _repet = -1;

			/// <summary>
			/// Sets the center.
			/// Default = 0,0
			/// </summary>
			/// <returns>The center.</returns>
			/// <param name="center">center.</param>
			public Builder SetCenter(CGPoint center)
			{
				_center = center;
				return this;
			}

			/// <summary>
			/// Sets the radius.
			/// Default = 100
			/// </summary>
			/// <returns>The radius.</returns>
			/// <param name="radius">Radius.</param>
			public Builder SetRadius(float radius)
			{
				_radius = radius;
				return this;
			}

			public Builder SetScaleFrom(float fromScale)
			{
				_scaleFrom = fromScale;
				return this;
			}

			public Builder SetScaleTo(float toScale)
			{
				_scaleTo = toScale;
				return this;
			}


			public Builder SetAlphaFrom(float fromAlpha)
			{
				_alphaFrom = fromAlpha;
				return this;
			}

			public Builder SetAlphaTo(float toAlpha)
			{

				_alphaTo = toAlpha;
				return this;
			}

			public Builder SetFillColor(UIColor fillColor)
			{
				_fillColor = fillColor;
				return this;
			}

			public Builder SetDuration(double duration)
			{
				_duration = duration;
				return this;
			}

			/// <summary>
			/// Sets the repeat count.
			/// if repet < 0  then will be infinit
			/// </summary>
			/// <returns>The repeat count.</returns>
			/// <param name="repet">Repet.</param>
			public Builder SetRepeatCount(int repet)
			{
				_repet = repet;
				return this;
			}

			public PulseEffect Create()
			{
				return new PulseEffect(_center, _radius, _scaleFrom, _scaleTo, _alphaFrom, _alphaTo, _fillColor, _duration, _repet);
			}
		}
	}
}
