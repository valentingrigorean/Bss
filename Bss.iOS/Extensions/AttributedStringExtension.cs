using Foundation;
using UIKit;

namespace Bss.iOS.Extensions
{
    public static class AttributedStringExtension
    {
        public static void AddText(this NSMutableAttributedString attributedString, string text, UIFont font)
        {
            var attrs = new UIStringAttributes();
            attrs.Font = font;

            var attrString = new NSMutableAttributedString(text, attrs);
            attributedString.Append(attrString);
        }
    }
}
