using UIKit;
using Foundation;

namespace Bss.iOS.UIKit
{
    public interface ITextView
    {
        NSAttributedString AttributedText { get; set; }
        UIColor TextColor { get; set; }
        UITextAlignment TextAlignment { get; set; }
        string Text { get; set; }
        UIFont Font { get; set; }
        bool UserInteractionEnabled { get; set; }
        UIView Source { get; }
    }

    public interface IInputTextView:ITextView
    {
        string Placeholder { get; set; }
        NSAttributedString AttributedPlaceholder { get; set; }
    }
}
