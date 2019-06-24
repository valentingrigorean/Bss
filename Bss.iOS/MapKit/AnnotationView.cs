using System;
using CoreGraphics;
using Foundation;
using MapKit;
using UIKit;

namespace Bss.iOS.MapKit
{
    public class AnnotationView : MKAnnotationView
    {
        public AnnotationView()
        {
        }

        public AnnotationView(NSCoder coder) : base(coder)
        {
        }

        public AnnotationView(CGRect frame) : base(frame)
        {
        }

        public AnnotationView(IMKAnnotation annotation, string reuseIdentifier) : base(annotation, reuseIdentifier)
        {
        }

        public AnnotationView(NSObjectFlag t) : base(t)
        {
        }

        public AnnotationView(IntPtr handle) : base(handle)
        {
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var hitView = base.HitTest(point, uievent);
            if (hitView != null)
                Superview?.BringSubviewToFront(this);
            return hitView;
        }

        public override bool PointInside(CGPoint point, UIEvent uievent)
        {
            var rect = Bounds;
            var isInside = rect.Contains(point);
            if (!isInside)
            {
                foreach (var view in Subviews)
                {
                    isInside = view.Frame.Contains(point);
                    if (isInside)
                        break;
                }
            }
            return isInside;
        }

        protected void AddSubviewAbove(UIView view)
        {
            if (view.Superview != null)
                view.RemoveFromSuperview();
            AddSubview(view);
            view.Center = new CGPoint(Bounds.Width / 2f, -view.Bounds.Height * 0.5f);
        }
    }
}
