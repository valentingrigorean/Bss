using UIKit;
using Foundation;

namespace Bss.iOS.UIKit
{
    internal class LabelTextViewWrapper : ITextView
    {
        private readonly UILabel _lbl;

        public LabelTextViewWrapper(UILabel lbl)
        {
            _lbl = lbl;
        }

        public string Text 
        { 
            get => _lbl.Text;
            set => _lbl.Text = value;
        }
              
        public UIFont Font 
        {
            get => _lbl.Font;
            set => _lbl.Font = value;
        }

        public bool UserInteractionEnabled
        {
            get => _lbl.UserInteractionEnabled;
            set => _lbl.UserInteractionEnabled = value;
        }

        public NSAttributedString AttributedText 
        { 
            get => _lbl.AttributedText;
            set => _lbl.AttributedText = value;
        }

        public UIColor TextColor 
        {
            get => _lbl.TextColor;
            set => _lbl.TextColor = value;
        }

        public UITextAlignment TextAlignment
        {
            get => _lbl.TextAlignment;
            set => _lbl.TextAlignment = value;
        }

        public UIView Source => _lbl;
    }
}