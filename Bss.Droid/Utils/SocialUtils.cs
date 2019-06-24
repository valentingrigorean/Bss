//
// SocialUtils.cs
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

using Android.Content;
using Android.Content.PM;
using Android.Net;


namespace Bss.Droid.Utils
{
    public static class SocialUtils
    {
        /// <summary>
        /// Tweet the specified context, text and urlContent.
        /// Will try share with native app if not will share with ActionView
        /// </summary>
        /// <returns>The tweet.</returns>
        /// <param name="context">Context.</param>
        /// <param name="text">Text.</param>
        /// <param name="urlContent">URL content.</param>
        public static void Tweet(Context context, string text, string urlContent)
        {
            var tweetUrl = string.Format("https://twitter.com/intent/tweet?text={0}&url={1}", Uri.Encode(text),
                                         Uri.Encode(urlContent));

            var tweetIntent = new Intent(Intent.ActionView, Uri.Parse(tweetUrl));

            var pm = context.PackageManager;
            var resolvedInfoList = pm.QueryIntentActivities(tweetIntent, PackageInfoFlags.MatchDefaultOnly);

            foreach (var resolveInfo in resolvedInfoList)
            {
                if (resolveInfo.ActivityInfo.PackageName.ToLower().StartsWith(
                    "com.twitter.android", System.StringComparison.CurrentCulture))
                {
                    tweetIntent.SetPackage(resolveInfo.ActivityInfo.PackageName);
                    break;
                }
            }
            context.StartActivity(tweetIntent);
        }
    }
}
