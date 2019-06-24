using System;
using Bss.iOS.UIKit.DropdownView;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class DropdownExtensions
    {
        public static IObservable<ItemSelectedEventArgs> WhenItemClick(this DropdownListView This)
        {
            return Observable.FromEventPattern<ItemSelectedEventArgs>(
                e => This.ItemSelected += e, e => This.ItemSelected -= e)
                             .Select(args => args.EventArgs);
        }
    }
}