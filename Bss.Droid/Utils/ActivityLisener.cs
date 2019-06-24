//
// ActivityLisener.cs
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

using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;


namespace Bss.Droid.Utils
{

    public class ActivityLisener : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        private static ActivityLisener Instance = new ActivityLisener();

        private static readonly IList<WeakReference<Application.IActivityLifecycleCallbacks>> Liseners =
          new List<WeakReference<Application.IActivityLifecycleCallbacks>>();

        private ActivityLisener()
        {

        }

        public void Initialize(Application application)
        {
            application.RegisterActivityLifecycleCallbacks(Instance);
        }

        public static void AddActivityLifecycleLisener(Application.IActivityLifecycleCallbacks lisener)
        {
            Liseners.Add(new WeakReference<Application.IActivityLifecycleCallbacks>(lisener));
        }

        public static void RemoveActivityLifecycleLisener(Application.IActivityLifecycleCallbacks lisener)
        {
            foreach (var item in Liseners)
            {
                var callback = GetRef(item);
                if (callback == lisener)
                    Liseners.Remove(item);
                return;
            }
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivityCreated(activity, savedInstanceState);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivityDestroyed(activity);
        }

        public void OnActivityPaused(Activity activity)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivityPaused(activity);
        }

        public void OnActivityResumed(Activity activity)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivityResumed(activity);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivitySaveInstanceState(activity, outState);
        }

        public void OnActivityStarted(Activity activity)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivityStarted(activity);
        }

        public void OnActivityStopped(Activity activity)
        {
            for (var i = Liseners.Count - 1; i >= 0; i--)
                GetRef(Liseners[i])?.OnActivityStopped(activity);
        }

        private static Application.IActivityLifecycleCallbacks GetRef(WeakReference<Application.IActivityLifecycleCallbacks> wr)
        {
            Application.IActivityLifecycleCallbacks callback;
            wr.TryGetTarget(out callback);
            return callback;
        }

        private static bool Contains(Application.IActivityLifecycleCallbacks lisener)
        {
            var flag = false;
            var dead = new List<WeakReference<Application.IActivityLifecycleCallbacks>>();
            foreach (var item in Liseners)
            {
                var callback = GetRef(item);
                flag |= callback == lisener;
                if (callback == null)
                    dead.Add(item);
            }
            foreach (var item in dead)
                Liseners.Remove(item);
            return flag;
        }
    }
}
