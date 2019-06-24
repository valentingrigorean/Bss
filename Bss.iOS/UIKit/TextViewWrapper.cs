using Foundation;
using UIKit;

namespace Bss.iOS.UIKit
{
    internal class TextViewWrapper :ITextView
    {
        private readonly UITextView _textView;

        public TextViewWrapper(UITextView textView)
        {
            _textView = textView;
        }

		public string Text
		{
			get => _textView.Text;
			set => _textView.Text = value;
		}

		public UIFont Font
		{
			get => _textView.Font;
			set => _textView.Font = value;
		}

		public bool UserInteractionEnabled
		{
			get => _textView.UserInteractionEnabled;
			set => _textView.UserInteractionEnabled = value;
		}

		public NSAttributedString AttributedText
		{
			get => _textView.AttributedText;
			set => _textView.AttributedText = value;
		}

		public UIColor TextColor
		{
			get => _textView.TextColor;
			set => _textView.TextColor = value;
		}

		public UITextAlignment TextAlignment
		{
			get => _textView.TextAlignment;
			set => _textView.TextAlignment = value;
		}

        public UIView Source => _textView;
    }
}
