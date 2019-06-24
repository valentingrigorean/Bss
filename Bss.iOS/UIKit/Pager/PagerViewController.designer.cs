// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace Bss.iOS.UIKit.Pager
{
    [Register("PagerViewController")]
    partial class PagerViewController
    {
        [Outlet]
        UIView ContainerViewGallery { get; set; }

        [Outlet]
        UIPageControl Pager { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (ContainerViewGallery != null)
            {
                ContainerViewGallery.Dispose();
                ContainerViewGallery = null;
            }

            if (Pager != null)
            {
                Pager.Dispose();
                Pager = null;
            }
        }
    }
}
