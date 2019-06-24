//
// UITableViewExtension.cs
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
using Bss.iOS.UIKit;
using CoreGraphics;
using CoreFoundation;

namespace UIKit
{
    public static class UITableViewExtension
    {
        public static void SetAutoResize(this UITableView tableView, nfloat extimatedRowHeight)
        {
            tableView.EstimatedRowHeight = extimatedRowHeight;
            tableView.RowHeight = UITableView.AutomaticDimension;
        }

        /// <summary>
        /// Sets the default property.
        /// Will clear background color and set TableFooterView with new UIView()
        /// </summary>
        /// <param name="tableView">Table view.</param>
        public static void SetDefaultProp(this UITableView tableView)
        {
            tableView.BackgroundColor = UIColor.Clear;
            tableView.TableFooterView = new UIView();
        }

        public static void SetSource<T>(this UITableView tableView, ViewSource<T> source)
        {
            tableView.Delegate = source;
            tableView.DataSource = source;
        }

        public static void MakeSeparatorLineFullWidth(this UITableView tableView)
        {
            tableView.PreservesSuperviewLayoutMargins = false;
            tableView.SeparatorInset = UIEdgeInsets.Zero;
            tableView.LayoutMargins = UIEdgeInsets.Zero;
        }
        /// <summary>
        /// Remove additional empty rows from the bottom of the table
        /// </summary>
        /// <param name="tableView">Table view.</param>
        public static void RemoveEmptyRows(this UITableView tableView)
        {
            tableView.TableFooterView = new UIView(CGRect.Empty);
        }

        /// <summary>
        /// After put the data source (fill the cells).
        /// Nr cells is fixed
        /// procentScreen = [0,1] - maximum percent of the parent view
        /// </summary>
        /// <param name="tableView">Table view.</param>
        public static void GetHeight(this UITableView tableView, UIView parent, NSLayoutConstraint heightConstraint, int minHeightCell = 50, double procentParent = 1)
        {
            nfloat height = 0;
            tableView.SetAutoResize(minHeightCell);
            tableView.LayoutIfNeeded();

            var cells = tableView.VisibleCells;

            foreach (var cell in cells)
            {
                height += cell.Frame.Height;
            }

            height = (int)Math.Min(parent.Frame.Height * procentParent, height);
            heightConstraint.Constant = height;
        }

        /// <summary>
        /// After put the data source (fill the cells).
        /// Nr of cells it changes
        /// procentScreen = [0,1] - maximum percent of the parent view
        /// </summary>
        /// <param name="tableView">Table view.</param>

        public static void GetHeight(this UITableView tableView, int nrCells, UIView parent, NSLayoutConstraint heightConstraint, int minHeightCell = 50, double procentParent = 1)
        {
            nfloat height = 0;
            tableView.SetAutoResize(minHeightCell);
            tableView.LayoutIfNeeded();

            height = nrCells * minHeightCell;

            height = (int)Math.Min(parent.Frame.Height * procentParent, height);
            heightConstraint.Constant = height;
        }

        public static void ReloadDataWithDelay(this UITableView This, int delayMS = 500, Action callback = null)
        {
            var delay = new DispatchTime(DispatchTime.Now, TimeSpan.FromMilliseconds(delayMS));
            DispatchQueue.MainQueue.DispatchAfter(delay, () =>
            {
                This.ReloadData();
                callback?.Invoke();
            });
        }
    }
}
