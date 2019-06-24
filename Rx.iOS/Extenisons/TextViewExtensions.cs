using System;
using Bss.iOS.UIKit;
using UIKit;
using Rx.Extensions;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class TextViewExtensions
    {
        public static IObservable<string> WhenTextChange(this ITextView This)
        {
            if (This.Source is UILabel lbl)
                return lbl.WhenTextChange();
            if (This.Source is UITextView textView)
                return textView.WhenTextChange();
            if (This.Source is UITextField textField)
                return textField.WhenTextChange()
                                .Select(e => e.Text);
            throw new Exception($"Invalid type {This.Source.GetType()}" +
                                " expected one of the UILabel,UITextView,UITextField");
        }     
    }
}