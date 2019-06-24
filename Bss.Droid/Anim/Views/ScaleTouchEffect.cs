//
// RBaseAdapter.cs
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
using Android.Views;
using System;
using Bss.Droid.Extensions;
using Java.Lang;

namespace Bss.Droid.Anim.Views
{

	public class ScaleTouchEffect : IClickableView
	{
		/// <summary>
		/// Gets or sets the scale.
		/// Between 0 - 1
		/// </summary>
		/// <value>The scale.</value>
		public float Scale { get; set; } = 0.9f;

		/// <summary>
		/// Gets or sets the alpha.
		///	Between 0 - 1
		///  </summary>
		/// <value>The alpha.</value>
		public float Alpha { get; set; } = 0.85f;

		/// <summary>
		/// Gets or sets the start duration of the animation in miliseconds.
		/// </summary>
		/// <value>The start duration of the animation.</value>
		public int StartAnimationDuration { get; set; } = 200;

		/// <summary>
		/// Gets or sets the end duration of the animation in mileseconds.
		/// </summary>
		/// <value>The end duration of the animation.</value>
		public int EndAnimationDuration { get; set; } = 100;

		public virtual void OnTouch(View view, TState state, Action clickCallback)
		{
			switch (state)
			{
				case TState.Began:
					if (view.ScaleX.AreEqual(Scale) && view.Alpha.AreEqual(Scale)) return;	
					view.Animate()
						.ScaleX(Scale)
						.ScaleY(Scale)
						.Alpha(Alpha)
						.SetDuration(StartAnimationDuration)
						.Start();
					break;
				case TState.Cancel:	
					if (view.ScaleX.AreEqual(1f) && view.Alpha.AreEqual(1f)) return;				
					view.Animate()
						.ScaleX(1f)
						.ScaleY(1f)
						.Alpha(1f)
						.SetDuration(EndAnimationDuration)
						.Start();
					break;
				case TState.Ended:
					view.Animate()
						.ScaleX(1f)
						.ScaleY(1f)
						.Alpha(1f)
						.SetDuration(EndAnimationDuration)
						.WithEndAction(new Runnable(() => clickCallback?.Invoke()))
						.Start();
					break;
			}
		}
	}

}