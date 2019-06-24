//
// LocationManager.cs
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
using CoreLocation;
using UIKit;
using System;

namespace Bss.iOS.Utils
{
    public class LocationUpdatedEventArgs : EventArgs
    {
        public LocationUpdatedEventArgs(CLLocation location)
        {
            Location = location;
        }

        public CLLocation Location { get; }
    }

    public enum LisingType
    {
        Foreground,
        Background
    }

    /// <summary>
    ///     iOS 8 also requires an entry in the Info.plist file to show the user
    ///  as part of the authorization request. 
    /// Add a key NSLocationAlwaysUsageDescription or NSLocationWhenInUseUsageDescription
    ///  with a string that will be displayed to the user in the alert
    ///  that requests location data access. 
    ///     iOS 9 requires that when using AllowsBackgroundLocationUpdates the 
    /// Info.plist includes the key UIBackgroundModes with the value location. 
    /// If you have completed step 2 of this walkthrough, this should already
    ///  been in your Info.plist file.
    /// </summary>
    public class LocationManager
    {
        private bool _debugInfo;
        private bool _isDebugMode;
        private bool _isUpdating;
        protected CLLocationManager _locMgr;
        private LisingType listeningType;
        public event Action<bool> PermisionRequested;

        public LocationManager(LisingType lisingType = LisingType.Foreground)
        {
            listeningType = lisingType;

            _locMgr = new CLLocationManager {PausesLocationUpdatesAutomatically = false};

            // iOS 8 has additional permissions requirements
            if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0)) return;
            switch (lisingType)
            {
                case LisingType.Foreground:                      
                    _locMgr.RequestWhenInUseAuthorization(); // only in foreground
                    break;
                case LisingType.Background:
                    _locMgr.RequestAlwaysAuthorization();                        
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lisingType), lisingType, null);
            }

            _locMgr.AuthorizationChanged += (sender, e) => { 
                PermisionRequested?.Invoke(e.Status != CLAuthorizationStatus.Denied); 
            };
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Bss.iOS.Utils.LocationManager"/> debug info.
        /// </summary>
        /// <value><c>true</c> if debug info; otherwise, <c>false</c>.</value>
        public bool DebugInfo
        {
            get { return _debugInfo; }
            set
            {
                _debugInfo = value;
                switch (_debugInfo)
                {
                    case true:
                        if (_isDebugMode)
                            return;
                        _isDebugMode = true;
                        LocationUpdated += PrintLocation;
                        break;
                    case false:
                        _isDebugMode = false;
                        LocationUpdated -= PrintLocation;
                        break;
                }
            }
        }

        /// <summary>
        /// This is a Boolean that can be set depending on whether the system 
        /// is allowed to pause location updates. On some device it defaults to true,
        /// which can cause the device to stop getting background location 
        /// updates after about 15 minutes.
        /// </summary>
        /// <value><c>true</c> if pauses location updates automatically; otherwise, <c>false</c>.</value>
        public bool PausesLocationUpdatesAutomatically
        {
            get { return _locMgr.PausesLocationUpdatesAutomatically; }
            set { _locMgr.PausesLocationUpdatesAutomatically = value; }
        }

        /// <summary>
        /// This is a Boolean property, introduced in iOS 9 that can be set to 
        /// allow an app to receive location updates when suspended.
        /// </summary>
        /// <value><c>true</c> if allows background location updates; otherwise, <c>false</c>.</value>
        public bool AllowsBackgroundLocationUpdates
        {
            get { return UIDevice.CurrentDevice.CheckSystemVersion(9, 0) && _locMgr.AllowsBackgroundLocationUpdates; }
            set
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                    _locMgr.AllowsBackgroundLocationUpdates = value;
            }
        }

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        public CLLocationManager LocMgr => _locMgr;

        public void StartLocationUpdates()
        {
            if (_isUpdating)
                return;
            // We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
            // the popover when the app is first launched, or by changing the permissions for the app in Settings
            if (!CLLocationManager.LocationServicesEnabled) return;
            _isUpdating = true;
            //set the desired accuracy, in meters
            LocMgr.DesiredAccuracy = 1;
            LocMgr.LocationsUpdated += (sender, e) => LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
            LocMgr.StartUpdatingLocation();
        }

        public void StopLocationUpdates()
        {
            if (!_isUpdating)
                return;
            _isUpdating = false;
            LocMgr.StopUpdatingLocation();
        }

        private static void PrintLocation(object sender, LocationUpdatedEventArgs e)
        {
            var location = e.Location;
            Console.WriteLine("Altitude: " + location.Altitude + " meters");
            Console.WriteLine("Longitude: " + location.Coordinate.Longitude);
            Console.WriteLine("Latitude: " + location.Coordinate.Latitude);
            Console.WriteLine("Course: " + location.Course);
            Console.WriteLine("Speed: " + location.Speed);
        }
    }
}

