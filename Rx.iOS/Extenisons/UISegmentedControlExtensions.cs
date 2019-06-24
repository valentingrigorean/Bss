using System;
using UIKit;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class UISegmentedControlExtensions
    {
        public static IObservable<int> WhenTabChange(this UISegmentedControl This)
        {
            return Observable.FromEventPattern(e => This.ValueChanged += e, e => This.ValueChanged -= e)
                      .Select(_ => (int)This.SelectedSegment);          
        }
    }
}