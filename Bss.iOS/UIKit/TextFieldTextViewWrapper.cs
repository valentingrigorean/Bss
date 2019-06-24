using Foundation;
using UIKit;

namespace Bss.iOS.UIKit
{
    internal class TextFieldTextViewWrapper : IInputTextView
    {
        private readonly UITextField _textField;

        public TextFieldTextViewWrapper(UITextField textField)
        {
            _textField = textField;
        }

        public string Text
        {
            get => _textField.Text;
            set => _textField.Text = value;
        }

        public UIFont Font
        {
            get => _textField.Font;
            set => _textField.Font = value;
        }

        public bool UserInteractionEnabled
        {
            get => _textField.UserInteractionEnabled;
            set => _textField.UserInteractionEnabled = value;
        }

        public NSAttributedString AttributedText
        {
            get => _textField.AttributedText;
            set => _textField.AttributedText = value;
        }

        public UIColor TextColor
        {
            get => _textField.TextColor;
            set => _textField.TextColor = value;
        }

        public UITextAlignment TextAlignment
        {
            get => _textField.TextAlignment;
            set => _textField.TextAlignment = value;
        }

        public string Placeholder
        {
            get => _textField.Placeholder;
            set => _textField.Placeholder = value;
        }

        public NSAttributedString AttributedPlaceholder
        {
            get => _textField.AttributedPlaceholder;
            set => _textField.AttributedPlaceholder = value;
        }

        public UIView Source => _textField;
    }
}
