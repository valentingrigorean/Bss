//
// CalculateCoordonate.cs
//
// Author:
//       Songurov Fiodor <f.songurov@software-dep.net>
//
// Copyright (c) 2016 Songurov
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

using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using UIKit;

namespace Bss.iOS.Location
{
    public class CalculateCoordonate
    {
        public static async Task<double[]> IGetCoordonateFromName(string address)
        {
            CLPlacemark result;

            try
            {
                var geocoder = new CLGeocoder();
                var a = await geocoder.GeocodeAddressAsync(address);
                result = a.FirstOrDefault();
            }
            catch
            {
                return new[] { 0.0, 0.0 };
            }

            return result != null ? new[] { result.Location.Coordinate.Latitude, result.Location.Coordinate.Longitude } : new[] { 0.0, 0.0 };
        }

        public static async Task<CLPlacemark> GetNameFromCoordinates(CLLocation location)
        {
            try
            {
                var geocoder = new CLGeocoder();
                var placemarks = await geocoder.ReverseGeocodeLocationAsync(location);

                if (placemarks == null || placemarks.Length == 0)
                    return null;

                return placemarks[0];
            }
            catch
            {
                return null;
            }
        }

        public static void GoogleMap(double[] startAddress, double[] endAddress)
        {
            var request = "";

            var myLat = startAddress[0].ToString().Replace(",", ".");
            var myLong = startAddress[1].ToString().Replace(",", ".");

            var clientLat = endAddress[0].ToString().Replace(",", ".");
            var clientLong = endAddress[1].ToString().Replace(",", ".");

            request = string.Format("http://maps.google.com/maps?saddr={0}&daddr={1}",
                                        myLat + "," + myLong,
                                        clientLat + "," + clientLong
                                       );

            UIApplication.SharedApplication.OpenUrl(new NSUrl(request));
        }
    }
}

