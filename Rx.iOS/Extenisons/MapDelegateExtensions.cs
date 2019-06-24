using System;
using MapKit;
using Bss.iOS.CoreLocation;
using System.Reactive.Linq;

namespace Rx.iOS.Extenisons
{
    public static class MapDelegateExtensions
    {
        public static IObservable<MKAnnotationViewEventArgs> WhenAnnotationSelected(this MapDelegate This)
        {
            return Observable.FromEventPattern<MKAnnotationViewEventArgs>(
                e => This.SelectedAnnotationView += e, e => This.SelectedAnnotationView -= e)
                             .Select(args => args.EventArgs);
        }

        public static IObservable<MKAnnotationViewEventArgs> WhenAnnotationDeselected(this MapDelegate This)
        {
            return Observable.FromEventPattern<MKAnnotationViewEventArgs>(
                e => This.DeselectedAnnotationView += e, e => This.DeselectedAnnotationView -= e)
                             .Select(args => args.EventArgs);
        }
    }
}