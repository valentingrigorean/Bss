//
// NSLayoutConstraintExtension.cs
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
using System.Linq;

// Analysis disable once CheckNamespace
namespace UIKit
{
	public static class NsLayoutConstraintExtension
	{
		/// <summary>
		/// Clone  layout changing only the view.
		/// </summary>
		/// <returns>The clone.</returns>
		/// <param name="This">This.</param>
		/// <param name="replaceView">Replace view.</param>
		/// <param name="newView">New view.</param>
		public static NSLayoutConstraint Clone(this NSLayoutConstraint This, UIView replaceView, UIView newView)
		{
			var firstItem = This.FirstItem == replaceView ? newView : This.FirstItem;
			var secondItem = This.SecondItem == replaceView ? newView : This.SecondItem;
			return NSLayoutConstraint.Create(
				firstItem, This.FirstAttribute, This.Relation,
				secondItem, This.SecondAttribute, This.Multiplier, This.Constant);
		}

		public static NSLayoutConstraint ChangeMultiplier(this NSLayoutConstraint layout, nfloat newMultiplier)
		{
			var constraint = NSLayoutConstraint.Create(
								 layout.FirstItem,
								 layout.FirstAttribute,
								 layout.Relation,
								 layout.SecondItem,
								 layout.SecondAttribute,
								 newMultiplier,
								 layout.Constant
							 );
			constraint.Active = layout.Active;
			constraint.SetIdentifier(layout.GetIdentifier());
			return constraint;
		}

		public static NSLayoutConstraint ChangeLayoutRelation(this NSLayoutConstraint layout, NSLayoutRelation rel)
		{

			var constraint = NSLayoutConstraint.Create(
								 layout.FirstItem,
								 layout.FirstAttribute,
								 rel,
								 layout.SecondItem,
								 layout.SecondAttribute,
								 layout.Multiplier,
								 layout.Constant
							 );
			constraint.Active = layout.Active;
			constraint.SetIdentifier(layout.GetIdentifier());
			return constraint;
		}

		public static UIView GetParent(this NSLayoutConstraint layout)
		{
			var view = layout.FirstItem as UIView;
			var view2 = layout.SecondItem as UIView;
			if (view.Subviews.FirstOrDefault(_ => _ == view2) != null)
				return view2;
			if (view2.Subviews.FirstOrDefault(_ => _ == view) != null)
				return view;
			if (view.Superview != null && view2.Superview != null)
				if (view.Superview.Subviews.Contains(view2) && view2.Superview.Subviews.Contains(view))
					return view;
			return null;
		}

		private static UIView GetParentInternal(UIView view, NSLayoutConstraint constraint)
		{
			if (view == null) return null;
			if (view.Constraints.FirstOrDefault(_ => _ == constraint) != null)
				return view;
			foreach (var item in view.Subviews)
			{
				var _view = GetParentInternal(item, constraint);
				if (_view != null)
					return _view;
			}
			return null;
		}

	}
}

