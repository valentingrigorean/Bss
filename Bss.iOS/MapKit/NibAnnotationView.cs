using System;
using CoreGraphics;
using Foundation;
using MapKit;
using UIKit;

namespace Bss.iOS.MapKit
{
    public abstract class NibAnnotationView : AnnotationView
    {
        private bool _wasInit;

        protected NibAnnotationView()
        {
            Initialize();
        }

        protected NibAnnotationView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        protected NibAnnotationView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        protected NibAnnotationView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        protected NibAnnotationView(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        protected NibAnnotationView(IMKAnnotation annotation, string reuseIdentifier) : base(annotation, reuseIdentifier)
        {
            Initialize();
        }

        public UIView ContentView { get; private set; }

		public override CGRect Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				base.Bounds = value;
				if (ContentView != null)
					ContentView.Bounds = value;
			}
		}

        public virtual void ViewDidLoad()
        {
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            if (!_wasInit)
            {
                ViewDidLoad();
                _wasInit = true;
            }
        }

        private void LoadNib()
        {
            var arr = NSBundle.MainBundle.LoadNib(GetType().Name, this, null);
            ContentView = arr.GetItem<UIView>(0);
            base.Frame = ContentView.Bounds;
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            ContentView.TranslatesAutoresizingMaskIntoConstraints = true;
            AddSubview(ContentView);
        }

        private void Initialize()
        {
            BackgroundColor = UIColor.Clear;
            LoadNib();
        }
    }
}