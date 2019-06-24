using System;
using UIKit;
using Foundation;

namespace Bss.iOS.UIKit.DropdownView
{
    public partial class SimpleCellView : TableViewCell<string>
    {

        public static readonly NSString Key = new NSString("SimpleCellView");
        public static readonly UINib Nib;

        static SimpleCellView()
        {
            Nib = UINib.FromName("SimpleCellView", NSBundle.MainBundle);
        }

        public SimpleCellView(IntPtr handle)
            : base(handle)
        {
        }

        public SimpleCellViewAppearance AppearanceCell { get; set; } = SharedAppearance;

        public static SimpleCellViewAppearance SharedAppearance { get; } = new SimpleCellViewAppearance
        {
            TextAlignment = UITextAlignment.Left
        };

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;
                SetSelectedState(value);
            }
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            SetSelectedState(selected);
        }


        public const string CellId = "SimpleCellView";

        #region implemented abstract members of TableViewCell

        public override void OnCreate()
        {
            TextLbl.TextAlignment = AppearanceCell.TextAlignment;
            if (AppearanceCell.Font != null)
                TextLbl.Font = AppearanceCell.Font;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            TextLbl.TextColor = AppearanceCell.TextColor;
            BackgroundColor = AppearanceCell.BackgroundColor;
            BackgroundView = AppearanceCell.BackgroundView;
            SelectedBackgroundView = AppearanceCell.SelectedBackgroundView;
        }


        public override void OnBind()
        {
            TextLbl.Text = Model;
        }

        #endregion

        private void SetSelectedState(bool val)
        {
            var text = AppearanceCell.TextColor;
            var selectedText = AppearanceCell.SelectedTextColor;
            var background = AppearanceCell.BackgroundColor;
            var selectedBackground = AppearanceCell.SelectedBackgroundColor;
            switch (val)
            {
                case true:
                    if (selectedText != null)
                        TextLbl.TextColor = selectedText;
                    if (selectedBackground != null)
                        BackgroundColor = selectedBackground;
                    break;
                case false:
                    TextLbl.TextColor = text;
                    BackgroundColor = background;
                    break;
            }
        }

        public class SimpleCellViewAppearance
        {
            public UITextAlignment TextAlignment { get; set; }
            public UIFont Font { get; set; }
            public UIColor TextColor { get; set; } = UIColor.Black;
            public UIColor BackgroundColor { get; set; } = UIColor.White;
            public UIColor SelectedBackgroundColor { get; set; }
            public UIColor SelectedTextColor { get; set; }
            public UIView BackgroundView { get; set; }
            public UIView SelectedBackgroundView { get; set; }
        }
    }


}
