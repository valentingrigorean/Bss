//
// MKMapViewExtension.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 valentingrigorean
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MapKit;
using CoreLocation;

// Analysis disable once CheckNamespace
using UIKit;
using ObjCRuntime;
public static class MkMapViewExtension
{
    //private const double MercatorOffset = 268435456;
    //private const double MercatorRadius = 85445659.44705395;
    //private const double Pi = Math.PI;

    public static bool IsUserAnnotation(this MKAnnotationView This)
    {
        if (This == null || This.Annotation == null)
            return false;
        var annotation = Runtime.GetNSObject(This.Annotation.Handle) as MKUserLocation;
        return annotation != null;
    }

    public static int GetZoomLevel(this MKMapView mapView)
    {
        var span = (mapView.Frame.Width / 256) / mapView.Region.Span.LongitudeDelta;
        return (int)Math.Ceiling(Math.Log(360 * span, 2));
    }

    public static void SetZoomLevel(this MKMapView mapView, int zoomLevel, bool animated)
    {
        var span = 360 / Math.Pow(2, zoomLevel) * (mapView.Frame.Width / 256);
        var region = new MKCoordinateSpan(0, span);
        mapView.SetRegion(new MKCoordinateRegion(mapView.CenterCoordinate, region), animated);
    }

    public static void SetCenterCoordinate(this MKMapView mapView, CLLocationCoordinate2D coords,
                                           int zoomLevel, bool animated)
    {
        var span = 360 / Math.Pow(2, zoomLevel) * (mapView.Frame.Width / 256);
        var region = new MKCoordinateSpan(0, span);
        mapView.SetRegion(new MKCoordinateRegion(coords, region), animated);
    }

    [Obsolete("Use mapView.ShowAnnotations() instead.")]
    public static void SetRegionForAnnotations(this MKMapView This, bool animated = true)
    {
        SetRegionForAnnotations(This, UIEdgeInsets.Zero, animated);
    }

    [Obsolete("Use mapView.ShowAnnotations() instead.")]
    public static void SetRegionForAnnotations(this MKMapView This, UIEdgeInsets padding, bool animated = true)
    {
        var zoomRect = MKMapRect.Null;
        foreach (var annotation in This.Annotations)
        {
            var annotationPoint = MKMapPoint.FromCoordinate(annotation.Coordinate);
            var pointRect = new MKMapRect(annotationPoint, new MKMapSize(0, 0));
            if (zoomRect.IsNull)
                zoomRect = pointRect;
            else
                zoomRect = MKMapRect.Union(zoomRect, pointRect);
        }
        This.SetVisibleMapRect(zoomRect, padding, animated);
        //This.SetRegion(This.Annotations.RegionForAnnotations(), true);
    }



    //public static int GetZoomLevel(this MKMapView mapView)
    //{
    //	var longitudeDelta = mapView.Region.Span.LongitudeDelta;
    //	var mapWidthInPixels = mapView.Bounds.Size.Width;
    //	var zoomScale = longitudeDelta * MercatorRadius * Math.PI / (180.0 * mapWidthInPixels);
    //	double zoomer = 20 - Math.Log(zoomScale,2);
    //	if (zoomer < 0) zoomer = 0;
    //	//  zoomer = round(zoomer);
    //	var zoom = (int)Math.Ceiling(zoomer);
    //	return zoom;
    //}

    //public static void SetCenterCoordinate(this MKMapView mapView, CLLocationCoordinate2D coords,
    //									   int zoomLevel, bool animated)
    //{
    //	zoomLevel = Math.Min(zoomLevel, 28);
    //	var span = CoordinateSpanWithMapView(mapView, coords, zoomLevel);
    //	var region = new MKCoordinateRegion(coords, span);
    //	mapView.SetRegion(region, animated);
    //}


    //private static MKCoordinateSpan CoordinateSpanWithMapView(MKMapView mapView,
    //														  CLLocationCoordinate2D centerCoordinate, int zoomLevel)
    //{
    //	// convert center coordiate to pixel space
    //	var centerPixelX = LongitudeToPixelSpaceX(centerCoordinate.Longitude);
    //	var centerPixelY = LatitudeToPixelSpaceY(centerCoordinate.Latitude);

    //	// determine the scale value from the zoom level
    //	var zoomExponent = 20 - zoomLevel;
    //	var zoomScale = Math.Pow(2, zoomExponent);

    //	// scale the map’s size in pixel space
    //	var mapSizeInPixels = mapView.Bounds.Size;
    //	var scaledMapWidth = mapSizeInPixels.Width * zoomScale;
    //	var scaledMapHeight = mapSizeInPixels.Height * zoomScale;

    //	// figure out the position of the top-left pixel
    //	var topLeftPixelX = centerPixelX - (scaledMapWidth / 2);
    //	var topLeftPixelY = centerPixelY - (scaledMapHeight / 2);

    //	// find delta between left and right longitudes
    //	var minLng = PixelSpaceXToLongitude(topLeftPixelX);
    //	var maxLng = PixelSpaceXToLongitude(topLeftPixelX + scaledMapWidth);
    //	var longitudeDelta = maxLng - minLng;

    //	// find delta between top and bottom latitudes
    //	var minLat = PixelSpaceYToLatitude(topLeftPixelY);
    //	var maxLat = PixelSpaceYToLatitude(topLeftPixelY + scaledMapHeight);
    //	var latitudeDelta = -1 * (maxLat - minLat);

    //	// create and return the lat/lng span
    //	var span = new MKCoordinateSpan(latitudeDelta, longitudeDelta);
    //	return span;
    //}


    //private static double LongitudeToPixelSpaceX(double longitude)
    //{
    //	return Math.Round(MercatorOffset + MercatorRadius * longitude * Pi / 180.0);
    //}

    //private static double LatitudeToPixelSpaceY(double latitude)
    //{
    //	return Math.Round(MercatorOffset - MercatorRadius * Math.Log((1 + Math.Sin(latitude * Pi / 180.0)) /
    //			(1 - Math.Sin(latitude * Pi / 180.0))) / 2.0);
    //}

    //private static double PixelSpaceXToLongitude(double pixelX)
    //{
    //	return ((Math.Round(pixelX) - MercatorOffset) / MercatorRadius) * 180.0 / Pi;
    //}

    //private static double PixelSpaceYToLatitude(double pixelY)
    //{
    //	return (Pi / 2.0 - 2.0 * Math.Atan(Math.Exp((Math.Round(pixelY) - MercatorOffset) / MercatorRadius))) * 180.0 / Pi;
    //}
}


