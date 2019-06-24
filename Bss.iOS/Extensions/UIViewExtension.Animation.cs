//
// UIViewExtension.Animation.cs
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
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace UIKit
{

	public enum AnimationType
	{
		In,
		Out
	}

	public enum FlipType
	{
		Horizontal,
		Vertical
	}

	public enum ShakeType
	{
		Horizontal,
		Vertical
	}

	public static partial class UIViewExtension
	{
		private const float MinAlpha = 0.0f;
		private const float MaxAlpha = 1.0f;

		public static void AnimationShake(this UIView view, ShakeType type = ShakeType.Horizontal, int count = 2,
										  double duration = 0.2f, float withTranslation = -5f)
		{
			var animation = new CABasicAnimation
			{
				KeyPath = $"transform.translation.{(type == ShakeType.Horizontal ? "x" : "y")}",
				TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear),
				RepeatCount = count,
				Duration = duration / count,
				AutoReverses = true,
				By = new NSNumber(withTranslation)
			};

			view.Layer.AddAnimation(animation, "shake");
		}

		/// <summary>
		/// Animations the fade.
		/// </summary>
		/// <param name="view">View.</param>
		/// <param name="type">Type.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="onCompletion">On completion if there is anyaction set</param>
		public static void AnimationFade(this UIView view, AnimationType type,
										 double duration = 0.3,
										 Action<bool> onCompletion = null)
		{

			view.Alpha = type == AnimationType.In ? MinAlpha : MaxAlpha;
			view.Transform = CGAffineTransform.MakeIdentity();

			AnimateNotifyInternal(duration,
				() =>
				{
					view.Alpha = type == AnimationType.In ? MaxAlpha : MinAlpha;
				}, onCompletion);
		}

		public static void AnimationFlip(this UIView view,
										 AnimationType type, FlipType flip,
										 double duration = 0.3,
										 Action<bool> onCompletion = null)
		{
			var m34 = (nfloat)(-1 * 0.001);

			view.Alpha = 1.0f;

			var minTransform = CATransform3D.Identity;

			minTransform.m34 = m34;


			var maxTransform = CATransform3D.Identity;
			maxTransform.m34 = m34;

			nfloat x = flip == FlipType.Horizontal ? 1.0f : 0f;
			nfloat y = flip == FlipType.Vertical ? 1.0f : 0f;

			switch (type)
			{
				case AnimationType.In:
					minTransform = minTransform.Rotate((float)(1 * Math.PI * 0.5),
						x, y, 0.0f);
					view.Alpha = MinAlpha;
					view.Layer.Transform = minTransform;
					break;
				case AnimationType.Out:
					minTransform = minTransform.Rotate((float)(-1 * Math.PI * 0.5),
						x, y, 0.0f);
					view.Alpha = MaxAlpha;
					view.Layer.Transform = maxTransform;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

			AnimateNotifyInternal(duration, () =>
			{
				view.Layer.AnchorPoint = new CGPoint(0.5f, 0.5f);

				switch (type)
				{
					case AnimationType.In:
						view.Alpha = MaxAlpha;
						view.Layer.Transform = maxTransform;
						break;
					case AnimationType.Out:
						view.Alpha = MinAlpha;
						view.Layer.Transform = minTransform;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}, onCompletion);
		}

		public static void AnimationRotate(this UIView view, AnimationType type, bool fromLeft = true, double duration = 0.3, Action<bool> onCompletion = null)
		{
			var minTransform = CGAffineTransform.MakeRotation((fromLeft ? -1f : 1) * 720);
			var maxTransform = CGAffineTransform.MakeRotation(0f);

			switch (type)
			{
				case AnimationType.In:
					view.Alpha = MinAlpha;
					view.Transform = minTransform;
					break;
				case AnimationType.Out:
					view.Alpha = MaxAlpha;
					view.Transform = maxTransform;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

			AnimateNotifyInternal(duration, () =>
			{
				switch (type)
				{
					case AnimationType.In:
						view.Alpha = MaxAlpha;
						view.Transform = maxTransform;
						break;
					case AnimationType.Out:
						view.Alpha = MinAlpha;
						view.Transform = minTransform;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}, onCompletion);
		}

		public static void AnimateScale(this UIView view, AnimationType type, double duration = 0.3, Action<bool> onCompletion = null)
		=> AnimateScale(view, type, duration, 0.1f, MinAlpha, MaxAlpha, onCompletion);


		public static void AnimateScale(this UIView view, AnimationType type, double duration, float scale, float minAlpha, float maxAlpha, Action<bool> onCompletion = null)
		{
			var minTransform = CGAffineTransform.MakeScale(scale, scale);
			var maxTransform = CGAffineTransform.MakeRotation(0f);

			switch (type)
			{
				case AnimationType.In:
					view.Alpha = minAlpha;
					view.Transform = minTransform;
					break;
				case AnimationType.Out:
					view.Alpha = maxAlpha;
					view.Transform = maxTransform;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

			AnimateNotifyInternal(duration, () =>
			{
				switch (type)
				{
					case AnimationType.In:
						view.Alpha = maxAlpha;
						view.Transform = maxTransform;
						break;
					case AnimationType.Out:
						view.Alpha = maxAlpha;
						view.Transform = minTransform;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}, onCompletion);
		}

		private static void TransformAnimation()
		{
		}

		private static void AnimateNotifyInternal(double duration, Action animation, Action<bool> onCompletion)
		{
			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, animation, finished =>
			{
				onCompletion?.Invoke(finished);
			});
		}
	}
}

