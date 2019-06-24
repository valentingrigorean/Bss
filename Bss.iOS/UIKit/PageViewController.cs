using System;
using UIKit;
using System.Collections.Generic;
using Bss.iOS.Extensions;

namespace Bss.iOS.UIKit
{
    public abstract class PageViewController : UIPageViewController
    {

        protected PageViewController(IntPtr handle) : base(handle)
        {
        }

        public IList<UIViewController> Pages { get; private set; } = new List<UIViewController>();

        public PageViewSource Source { get; private set; }

        public int CurrentItem { get; private set; } = -1;

        public bool CanMoveForward
        {
            get
            {
                if (Pages.IsNullOrEmpty()) return false;
                return CurrentItem + 1 < Pages.Count;
            }
        }

        public bool CanMoveBackward
        {
            get
            {
                if (Pages.IsNullOrEmpty()) return false;
                return CurrentItem - 1 >= 0;
            }
        }

        public event EventHandler<PageChangeEventArgs> PageChanged;

        public class PageChangeEventArgs : EventArgs
        {
            public PageChangeEventArgs(int prevIndex, int currentIndex,
                                       UIViewController prevController, UIViewController nextController)
            {
                PreviewsItem = prevIndex;
                CurrentItem = currentIndex;
                PreviewsController = prevController;
                CurrentController = nextController;
            }

            public int PreviewsItem { get; }
            public int CurrentItem { get; }
            public UIViewController PreviewsController { get; }
            public UIViewController CurrentController { get; }
        }

        public void MoveBackward(bool animated = true)
        {
            if (CanMoveBackward)
                SetCurrentItem(CurrentItem - 1, animated);
        }

        public void MoveForward(bool animated = true)
        {
            if (CanMoveForward)
                SetCurrentItem(CurrentItem + 1, animated);
        }


        public void SetCurrentItem(int newPostion, bool animated = true)
        {
            var animDirection = newPostion > CurrentItem ? UIPageViewControllerNavigationDirection.Forward :
                                                    UIPageViewControllerNavigationDirection.Reverse;

            var prevPage = TryGetPage(CurrentItem);
            var nextPage = Pages[newPostion];

            SetViewControllers(new[] { Pages[newPostion] }, animDirection, animated, null);
            var currentPosition = CurrentItem;
            CurrentItem = newPostion;
            PageChanged?.Invoke(this, new PageChangeEventArgs(currentPosition, CurrentItem, prevPage, nextPage));

        }


        public void SetPages(IList<UIViewController> pages)
        {
            Pages = pages;
            Source = new PageViewSource(Pages);
            Source.PageChanged += (sender, e) =>
            {
                var prevPage = TryGetPage(CurrentItem);
                var prevPageIndex = CurrentItem;
                CurrentItem = e.Index;
                PageChanged?.Invoke(this, new PageChangeEventArgs(prevPageIndex, e.Index,
                                                                  prevPage, e.ViewController));

            };

            this.SetSource(Source);
            if (Source.Count == 0) return;
            SetCurrentItem(CurrentItem < 0 ? 0 : Math.Min(CurrentItem, Source.Count - 1));
        }

        protected void NotifyEmptyPage()
        {
            PageChanged?.Invoke(this, new PageChangeEventArgs(0, 0, null, null));
        }

        private UIViewController TryGetPage(int page)
        {
            if (page < 0 || page > Pages.Count - 1)
                return null;
            return Pages[page];
        }
    }
}
