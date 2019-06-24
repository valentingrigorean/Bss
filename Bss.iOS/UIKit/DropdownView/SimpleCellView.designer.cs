// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace Bss.iOS.UIKit.DropdownView
{
    [Register("SimpleCellView")]
    partial class SimpleCellView
    {
        [Outlet]
        UIImageView RightImage { get; set; }

        [Outlet]
        UILabel TextLbl { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (TextLbl != null)
            {
                TextLbl.Dispose();
                TextLbl = null;
            }

            if (RightImage != null)
            {
                RightImage.Dispose();
                RightImage = null;
            }
        }
    }
}
