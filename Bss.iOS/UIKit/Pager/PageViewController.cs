// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using UIKit;

namespace Bss.iOS.UIKit.Pager
{
    internal class PageViewController : UIPageViewController
    {
        private readonly IList<UIViewController> _pages = new List<UIViewController>();
        private PageViewSource _source;
        private PagerDataSource _gallerySource;


        public PageViewController(PagerConfig config, PagerViewController galleryRef)
            : base(config.TransitionStyle, config.ScrollType,
                   config.TransitionStyle == UIPageViewControllerTransitionStyle.Scroll ?
                   UIPageViewControllerSpineLocation.None : UIPageViewControllerSpineLocation.Min,
                   config.SpaceBetweenPage)
        {
            Config = config;
            GalleryRef = galleryRef;
        }

        public PagerViewController GalleryRef { get; set; }

        public PagerConfig Config { get; }

        public event Action<int> OnNewPage;

        public int CurrentPage { get; private set; } = -1;

        public PagerDataSource GalleryDataSource
        {
            get { return _gallerySource; }
            set
            {
                _gallerySource = value;
                RecreateControllers();
                GotoScreen(CurrentPage, false);
            }
        }

        public void GotoScreen(int screen, bool animated = true)
        {
            if (_pages.Count == 0) 
                return;
            screen = screen.Clamp(0, _pages.Count - 1);
            if (screen == CurrentPage) 
                return;
            CurrentPage = screen;
            OnNewPage?.Invoke(screen);
            SetViewControllers(new[] { _pages[screen] }, GetDirection(screen), animated, null);
            if (_source != null)
                _source.CurrentPosition = screen;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Initialize();
        }

        private UIPageViewControllerNavigationDirection GetDirection(int position)
        {
            if (_source == null)
                return UIPageViewControllerNavigationDirection.Forward;
            if (position < _source.CurrentPosition)
                return UIPageViewControllerNavigationDirection.Reverse;
            return UIPageViewControllerNavigationDirection.Forward;
        }

        private void RecreateControllers()
        {
            if (GalleryDataSource == null) 
                return;
            _pages.Clear();
            for (var i = 0; i < GalleryDataSource.Count; i++)
                _pages.Add(GalleryDataSource.GetPage(GalleryRef, i));
        }

        private void Initialize()
        {
            _source = new PageViewSource(_pages);
            RecreateControllers();
            if (Config.AllowSwipe)
                this.SetSource(_source);
            GotoScreen(0);
			_source.PageChanged += (sender, e) =>
			{
				CurrentPage = e.Index;
				OnNewPage?.Invoke(e.Index);
			};
        }
    }
}
