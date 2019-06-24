//
// WeakReferenceExtensions.cs
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
using System.Linq;

namespace Bss.Droid.Extensions
{
    public static class WeakReferenceExtensions
    {
        public static bool Contains<T>(this IList<WeakReference<T>> list, T item) where T : class
        {
            ClearWeakRef(list);
            return list.FirstOrDefault(_ => GetTarget(_) == item) != default(T);
        }

        public static int IndexOf<T>(this IList<WeakReference<T>> list, T item) where T : class
        {
            ClearWeakRef(list);
            for (var i = 0; i < list.Count; i++)
                if (list[i].GetTarget() == item)
                    return i;
            return -1;
        }

        public static void Remove<T>(this IList<WeakReference<T>> list, T item) where T : class
        {
            ClearWeakRef(list);
            var index = list.IndexOf(item);
            if (index >= 0)
                list.RemoveAt(index);
        }

        public static T GetTarget<T>(this WeakReference<T> weakref) where T : class
        {
            T target = default(T);
            weakref.TryGetTarget(out target);
            return target;
        }

        public static void ClearWeakRef<T>(this IList<WeakReference<T>> list) where T : class
        {
            var deadRef = new List<WeakReference<T>>();
            T target;
            foreach (var item in list)
            {
                if (!item.TryGetTarget(out target))
                    deadRef.Add(item);
            }
            foreach (var item in deadRef)
                list.Remove(item);
        }
    }
}
