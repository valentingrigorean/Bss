//
// App.cs
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
using UIKit;
using System.Linq;
using System;
using Foundation;
using SafariServices;
using CoreFoundation;

namespace Bss.iOS
{
	public static class Application
	{
		public static bool OpenUrl(NSUrl url)
		{
			OpenUrl(url, null);
			return true;
		}

		public static bool OpenUrl(string url)
		{
			OpenUrl(url, null);
			return true;
		}

		public static void OpenUrl(string url, Action<bool> completion)
		{
			OpenUrl(new NSUrl(url), completion);
		}

		public static void OpenUrl(NSUrl url, Action<bool> completion)
		{
			var options = new UIApplicationOpenUrlOptions();
			UIApplication.SharedApplication.OpenUrl(url, options, completion);
		}

		public static void OpenLink(string link, UIViewController fromViewController)
		{
			var version = new Version(UIDevice.CurrentDevice.SystemVersion);
			if (version.Major >= 9)
			{
				var dest = new SFSafariViewController(new NSUrl(link), true);
				dest.Delegate = new SafariDelegate();
				fromViewController.PresentViewController(dest, true, null);
			}
			else
				OpenUrl(link);
		}

		public static UIViewController TopViewController
		{
			get
			{
				var viewController = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
				var navController = viewController as UINavigationController;
				return navController != null ? navController.ViewControllers.LastOrDefault() : viewController;
			}
		}

		public static UINavigationController RootNavigationController
		{
			get
			{
				var viewController = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
				var navController = viewController as UINavigationController;
				return navController;
			}
		}

		public static void HideKeyboard()
		{
			UIApplication.SharedApplication.KeyWindow.EndEditing(true);
		}

		public static void InvokeOnMainThread(Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			UIApplication.SharedApplication.InvokeOnMainThread(action);
		}

		public static void InvokeOnMainThread(Action action, double delayMilesconds)
		{
			DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(
				DispatchTime.Now, TimeSpan.FromMilliseconds(delayMilesconds)), action);
		}

		public static bool CanCall => UIApplication.SharedApplication.CanOpenUrl(new NSUrl("tel://"));

		public static string ApplicationName
		{
			get
			{
				var t = NSBundle.MainBundle.InfoDictionary["CFBundleName"];
				return t.ToString();
			}
		}

        public static string BundleShortVersionString => NSBundle.MainBundle.ObjectForInfoDictionary(
            "CFBundleShortVersionString")?.ToString();

        public static string BundleVersion => NSBundle.MainBundle.ObjectForInfoDictionary(
            "CFBundleVersion")?.ToString();

		private class SafariDelegate : SFSafariViewControllerDelegate
		{
			public override void DidFinish(SFSafariViewController controller)
			{
				controller.DismissViewController(true, null);
			}
		}
	}
}

