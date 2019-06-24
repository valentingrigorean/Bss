//
// PageDataSource.cs
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
using UIKit;
using System.Collections.Generic;
using System;
using Foundation;

namespace Bss.iOS.UIKit
{
    /// <summary>
    /// Page data source.
    /// UIViewController most implement IPageItem for this to work
    /// PageViewSource is delegate also DataSource
    /// </summary>
    public class PageViewSource : NSObject, IUIPageViewControllerDataSource, IUIPageViewControllerDelegate
    {
        private int _afterIndex = -1;
        private UIViewController _afterController;
        private int _beforeIndex = -1;
        private UIViewController _beforeController;

        public PageViewSource(IList<UIViewController> dataSource)
        {
            DataSource = new List<UIViewController>(dataSource);
        }

        public int Count => DataSource.Count;

        public IList<UIViewController> DataSource { get; private set; }

        public EventHandler<int> CountChange;

        public bool InfiniteScrolling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PageControl should be visible or not
        /// </summary>
        /// <value><c>true</c> if page control; otherwise, <c>false</c>.</value>
        public bool ShowPageControl { get; set; }

        public UIViewController CurrentController { get; private set; }

        public int CurrentPosition { get; set; }

        public int PreviewPosition => _beforeIndex;

        public class PageChangedEventArgs : EventArgs
        {
            public PageChangedEventArgs(int index, UIViewController viewController)
            {
                Index = index;
                ViewController = viewController;
            }

            public int Index { get; }
            public UIViewController ViewController { get; set; }
        }

        public event EventHandler<PageChangedEventArgs> PageChanged;

        public void Add(UIViewController controller)
        {
            var page = controller as IPageItem;
            if (page == null)
                NotValidController();
            DataSource.Add(controller);
            CountChange?.Invoke(this, Count);
        }

        public void Remove(UIViewController controller)
        {
            DataSource.Remove(controller);
            CountChange?.Invoke(this, Count);
        }

        public void ChangeDataSource(IList<UIViewController> newDataSource)
        {
            DataSource = newDataSource ?? throw new ArgumentNullException(nameof(newDataSource));
            CountChange?.Invoke(this, Count);
        }

        [Export("pageViewController:willTransitionToViewControllers:")]
        public void WillTransition(UIPageViewController pageViewController, UIViewController[] pendingViewControllers)
        {
            if (pendingViewControllers.Length == 0) return;
            _afterController = pendingViewControllers[0];
            var page = _afterController as IPageItem;
            _afterIndex = page.Index;

        }

        [Export("pageViewController:didFinishAnimating:previousViewControllers:transitionCompleted:")]
        public void DidFinishAnimating(UIPageViewController pageViewController, bool finished,
                                       UIViewController[] previousViewControllers, bool completed)
        {
            if (previousViewControllers.Length == 0 || !finished || !completed) return;
            _beforeController = previousViewControllers[0];
            var page = _beforeController as IPageItem;
            _beforeIndex = page.Index;
            CurrentPosition = _afterIndex;
            CurrentController = _afterController;
            PageChanged?.Invoke(this, new PageChangedEventArgs(CurrentPosition, CurrentController));
        }

        [Export("presentationCountForPageViewController:")]
        public nint GetPresentationCount(UIPageViewController pageViewController)
        {
            if (!ShowPageControl) return 0;
            return DataSource.Count;
        }

        [Export("presentationIndexForPageViewController:")]
        public nint GetPresentationIndex(UIPageViewController pageViewController)
        {
            if (!ShowPageControl) return 0;
            return CurrentPosition;
        }

        public virtual UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            if (Count == 0)
                return null;
            var pageView = referenceViewController as IPageItem;
            if (pageView == null) NotValidController();
            return pageView.Index == 0 ? InfiniteScrolling ? DataSource[DataSource.Count - 1] : null :
                           DataSource[pageView.Index - 1];
        }

        public virtual UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            if (Count == 0)
                return null;
            var pageView = referenceViewController as IPageItem;
            if (pageView == null) NotValidController();
            return pageView != null && pageView.Index + 1 == DataSource.Count ? InfiniteScrolling ? DataSource[0] : null :
                                                      DataSource[pageView.Index + 1];
        }

        private static void NotValidController()
        {
            throw new Exception("UIViewController most implement IPageItem");
        }
    }
}

