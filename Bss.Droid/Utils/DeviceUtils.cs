//
// DeviceUtils.cs
//
// Author:
//       Valentin Grigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 
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
using Android.OS;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Net.Wifi;
using Android.Text.Format;
using Java.Math;
using System.Linq;
using Java.Net;
using Java.Security;
using System.Collections.Generic;
using Android.Util;
using System.Runtime.Remoting.Messaging;
using Android.Support.V4.Content;
using Android;
using Android.Provider;

namespace Bss.Droid.Utils
{
    public static class DeviceUtils
    {
        public static int DeviceApi => (int)Build.VERSION.SdkInt;

        public static bool ContainsPackage(string package)
        {
            var pm = Application.Context.PackageManager;

            try
            {
                pm.GetPackageInfo(package, PackageInfoFlags.Activities);
                return true;
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            return false;
        }

        /// <summary>
        /// Gets the device ip.
        /// Need permisions INTERNET,ACCESS_NETWORK_STATE,ACCESS_WIFI_STATE
        /// </summary>
        /// <returns>The device ip.</returns>
        public static string GetDeviceIp(Context context)
        {
            var wm = context.GetSystemService(Context.WifiService) as WifiManager;
            var address = BigInteger.ValueOf(wm.ConnectionInfo.IpAddress).ToByteArray().Reverse().ToArray();
            return InetAddress.GetByAddress(address).HostAddress;
        }

        public static string GetApplicationName(Context context)
        {
            return context.ApplicationInfo.LoadLabel(context.PackageManager);
        }

        /// <summary>
        /// Gets the hash keys for packageName.
        /// </summary>
        /// <returns>The hash keys.</returns>
        /// <param name="context">Context.</param>
        public static IList<string> GetHashKeys(Context context, string packageName)
        {
            var list = new List<string>();
            try
            {
                var info = context.PackageManager.GetPackageInfo(packageName, PackageInfoFlags.Signatures);

                foreach (var signature in info.Signatures)
                {
                    var md = MessageDigest.GetInstance("SHA");
                    md.Update(signature.ToByteArray());
                    list.Add(Base64.EncodeToString(md.Digest(), Base64Flags.Default));
                }
            }
            catch { }
            return list;
        }

        /// <summary>
        /// Gets the hash keys for current context.
        /// </summary>
        /// <returns>The hash keys.</returns>
        /// <param name="context">Context.</param>
		public static IList<string> GetHashKeys(Context context)
        {
            return GetHashKeys(context, context.PackageName);
        }

        public static bool IsLocationServicesAvailable(Context context)
        {
            var locationMode = 0;
            var locationProviders = "";
            var isAvailable = false;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                try
                {
                    locationMode = Settings.Secure.GetInt(context.ContentResolver, Settings.Secure.LocationMode);
                }
                catch (Settings.SettingNotFoundException e)
                {

                }

                isAvailable = (locationMode != (int)Settings.Secure.LocationModeOff);
            }
            else
            {
                locationProviders = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.LocationProvidersAllowed);
                isAvailable = !string.IsNullOrEmpty(locationProviders);
            }

            var coarsePermissionCheck = (ContextCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted);
            var finePermissionCheck = (ContextCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted);

            return isAvailable && (coarsePermissionCheck || finePermissionCheck);
        }
    }
}
