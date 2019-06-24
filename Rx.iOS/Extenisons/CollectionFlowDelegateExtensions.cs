using System;
using Bss.iOS.UIKit;
using Foundation;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class CollectionFlowDelegateExtensions
    {
        public static IObservable<NSIndexPath> WhenItemClick(this CollectionFlowDelegate This)
        {
            return Observable.FromEventPattern<NSIndexPath>(e => This.ItemClicked += e, e => This.ItemClicked -= e)
                             .Select(x => x.EventArgs);
        }
    }
}