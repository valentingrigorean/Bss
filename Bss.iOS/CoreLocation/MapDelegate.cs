using System;
using MapKit;

namespace Bss.iOS.CoreLocation
{
    public class MapDelegate : MKMapViewDelegate
    {
        public event EventHandler<MKAnnotationViewEventArgs> SelectedAnnotationView;
        public event EventHandler<MKAnnotationViewEventArgs> DeselectedAnnotationView;

        public Func<MKMapView, IMKAnnotation, MKAnnotationView> GetViewForAnnotationDelegate { get; set; } = (map, annotation) => null;

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            return GetViewForAnnotationDelegate?.Invoke(mapView, annotation);
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            view.SetSelected(true, true);
            SelectedAnnotationView?.Invoke(this, new MKAnnotationViewEventArgs(view));
        }

        public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            view.SetSelected(false, true);
            DeselectedAnnotationView?.Invoke(this, new MKAnnotationViewEventArgs(view));
        }
    }
}
