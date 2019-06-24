// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace Bss.iOS.UIKit
{
    [Register("DatepickerViewController")]
    partial class DatepickerViewController
    {
        [Outlet]
        UIVisualEffectView BlurView { get; set; }

        [Outlet]
        UIView BottomView { get; set; }

        [Outlet]
        UIButton CancelBtn { get; set; }

        [Outlet]
        UIButton ConfirmBtn { get; set; }

        [Outlet]
        UIView ContentView { get; set; }

        [Outlet]
        UIDatePicker DatePicker { get; set; }

        [Outlet]
        UILabel MonthLbl { get; set; }

        [Outlet]
        UIButton NextBtn { get; set; }

        [Outlet]
        UIButton PrevBtn { get; set; }

        [Outlet]
        UIView SplitView { get; set; }

        [Outlet]
        UIView TopView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (BlurView != null)
            {
                BlurView.Dispose();
                BlurView = null;
            }

            if (BottomView != null)
            {
                BottomView.Dispose();
                BottomView = null;
            }

            if (CancelBtn != null)
            {
                CancelBtn.Dispose();
                CancelBtn = null;
            }

            if (ConfirmBtn != null)
            {
                ConfirmBtn.Dispose();
                ConfirmBtn = null;
            }

            if (ContentView != null)
            {
                ContentView.Dispose();
                ContentView = null;
            }

            if (DatePicker != null)
            {
                DatePicker.Dispose();
                DatePicker = null;
            }

            if (MonthLbl != null)
            {
                MonthLbl.Dispose();
                MonthLbl = null;
            }

            if (NextBtn != null)
            {
                NextBtn.Dispose();
                NextBtn = null;
            }

            if (PrevBtn != null)
            {
                PrevBtn.Dispose();
                PrevBtn = null;
            }

            if (SplitView != null)
            {
                SplitView.Dispose();
                SplitView = null;
            }

            if (TopView != null)
            {
                TopView.Dispose();
                TopView = null;
            }
        }
    }
}
