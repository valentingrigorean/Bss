//
// BUITableView.cs
//
// Author:
//       valentingrigorean <>
//
// Copyright (c) 2017 ${CopyrightHolder}
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
using System;
using Foundation;
using CoreGraphics;

namespace Bss.iOS.UIKit
{
	[Register("BUITableView")]
	public class BUITableView : UITableView
	{
		private UIView _emptyView;

		public BUITableView()
		{
		}

		public BUITableView(IntPtr handle) : base(handle)
		{
		}

		public BUITableView(CGRect frame) : base(frame)
		{

		}

		public int ItemCount
		{
			get
			{
				var total = 0;
				var sections = base.NumberOfSections();
				for (var i = 0; i < sections; i++)
					total += (int)NumberOfRowsInSection(i);
				return total;
			}
		}

		public override NSObject WeakDataSource
		{
			get
			{
				return base.WeakDataSource;
			}
			set
			{
				base.WeakDataSource = value;
				ShowEmptyView();
			}
		}

		public override void EndUpdates()
		{
			base.EndUpdates();
			ShowEmptyView();
		}


		/// <summary>
		/// Gets or sets the empty view. When the backing datasource has no
		/// data this view will be made visible.
		/// </summary>
		/// <value>The empty view.</value>
		public UIView EmptyView
		{
			get { return _emptyView; }
			set
			{
				if (_emptyView != null)
					_emptyView.RemoveFromSuperview();
				_emptyView = value;
				if (_emptyView != null)
				{
					AddSubview(_emptyView);
					_emptyView.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
					_emptyView.TranslatesAutoresizingMaskIntoConstraints = true;
					_emptyView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
					SendSubviewToBack(_emptyView);
				}
				ShowEmptyView();
			}
		}


		public void SetDefaultEmptyView(string text, UIImage img = null)
		{
			EmptyView = new EmptyTableView
			{
				Text = text,
				Image = img
			};
		}

		public void SetDefaultEmptyView(NSAttributedString attributedText, UIImage img = null)
		{
			EmptyView = new EmptyTableView
			{
				AttributedText = attributedText,
				Image = img
			};
		}

		public override void ReloadData()
		{
			base.ReloadData();
			ShowEmptyView();
		}

		private void ShowEmptyView()
		{
			if (_emptyView == null) return;
			if (ItemCount == 0)
			{
				_emptyView.Hidden = false;
				BringSubviewToFront(_emptyView);
			}
			else
			{
				SendSubviewToBack(_emptyView);
				_emptyView.Hidden = true;
			}
		}
	}
}
