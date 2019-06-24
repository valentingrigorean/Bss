using System;
using UIKit;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class UILabelExtensions
    {
        public static IObservable<string> WhenTextChange(this UILabel This)
        {
            return Observable.Create<string>(obser =>
            {
                return This.AddObserver("text", Foundation.NSKeyValueObservingOptions.OldNew, obserFunc =>
                   {
                       obser.OnNext(obserFunc.NewValue.ToString());
                   });
            });
        }
    }
}