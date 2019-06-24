using System;
using UIKit;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class UITextViewExtensions
    {
        public static IObservable<string> WhenTextChange(this UITextView This)
        {
            return Observable.FromEventPattern(e => This.Changed += e, e => This.Changed -= e)
                             .Select(_ => This.Text);
        }
    }
}