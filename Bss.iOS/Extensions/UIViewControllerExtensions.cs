
using UIKit;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Bss.iOS.Extensions
{
    public static class UIViewControllerExtensions
    {
        public static UIView GetFirstResponder(this UIViewController This)
        {
            return This.View.GetFirstResponder();
        }

        public static void PresentUsingRootViewController(this UIViewController This, bool animated, Action completionHandler)
        {
            var root = Application.TopViewController;
            root.PresentViewController(This, animated, completionHandler);
        }

        public static Task PresentUsingRootViewControllerAsync(this UIViewController This, bool animated)
        {
            var root = Application.TopViewController;
            return root.PresentViewControllerAsync(This, animated);
        }

        public static void RemoveFromStack(this UIViewController This, Type type)
        {
            if (This == null || This.NavigationController == null) return;

            This.NavigationController.ViewControllers = This.NavigationController
                .ViewControllers.Where(_ => _.GetType() != type).ToArray();
        }


        public static void RemoveFromStack(this UIViewController This)
        {
            if (This == null || This.NavigationController == null) 
                return;

            This.NavigationController.ViewControllers = This.NavigationController
                .ViewControllers.Where(_ => _ != This).ToArray();
        }

        public static void SetFont(this UIViewController This, int fontType, params UIView[] views)
        {
            foreach (var view in views)
                view.SetFonts(fontType);
        }

        /// <summary>
        /// Moves to this controller to another controller
        /// </summary>
        /// <param name="this">This.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="containerView">Container view if is null will put in parent.view.</param>
        public static void MoveTo(this UIViewController @this, UIViewController parent, UIView containerView = null)
        {
            parent.AddChildViewController(@this);
            containerView = containerView ?? parent.View;
            containerView.Layer.MasksToBounds = true;
            @this.View.Frame = containerView.Bounds;
            @this.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            @this.View.TranslatesAutoresizingMaskIntoConstraints = true;
            containerView.AddSubview(@this.View);

            @this.DidMoveToParentViewController(parent);
        }

        public static void RemoveFromParent(this UIViewController @this)
        {
            @this.WillMoveToParentViewController(null);
            @this.View.RemoveFromSuperview();
            @this.RemoveFromParentViewController();
        }


        public static void PopSafetyViewController(this UIViewController This, bool animated = true)
        {
            var navigationController = This.NavigationController;
            if (navigationController.ViewControllers.LastOrDefault() == This)
                navigationController.PopViewController(animated);
        }
    }
}
