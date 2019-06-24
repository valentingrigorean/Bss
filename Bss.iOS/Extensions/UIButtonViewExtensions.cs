//
// UIButtonViewExtensions.cs
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

using Bss.iOS.UIKit;
using UIKit;

namespace Bss.iOS.Extensions
{
	public static class UIButtonViewExtensions
	{

		public static void SetAlphaDrawable(this UIButtonView btn, float alpha = 0.8f)
		{
			btn.Delegate = new AlphaButtonDelegate { Alpha = alpha };
		}


		private class AlphaButtonDelegate : IButtonDelegate
		{
			public float Alpha { get; set; }

			public void HandleState(UIView view, TState state)
			{
				switch (state)
				{
					case TState.Began:
					case TState.MoveIn:
						view.Alpha = Alpha;
						break;
					case TState.Ended:
					case TState.MoveOut:
						view.Alpha = 1f;
						break;
				}
			}

		}
	}
}
