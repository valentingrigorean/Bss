//
// CollectionExtensions.cs
//
// Author:
//       valentingrigorean <>
//
// Copyright (c) 2017 ${CopyrightHolder}
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
using System.Collections.Concurrent;

namespace Bss.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var i in source)
            {
                yield return i;
                var children = childrenSelector(i);
                if (children != null)
                {
                    foreach (var child in SelectManyRecursive(children, childrenSelector))
                    {
                        yield return child;
                    }
                }
            }
        }

        public static void AddIfNotExists<T>(this ICollection<T> source, T item)
        {
            if (!source.Contains(item))
                source.Add(item);
        }

        public static void ClearAndReplace<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            source.Clear();

            if (items != null)
                foreach (var item in items)
                    source.Add(item);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static int IndexOf<T>(this T[] arr, T item)
        {
            return Array.IndexOf(arr, item);
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
                list.Add(item);
        }

        public static void AddOrRemove<T>(this ICollection<T> list, T item)
        {
            if (list.Contains(item))
                list.Remove(item);
            else
                list.Add(item);
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = val;
                return;
            }
            dict.Add(key, val);
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var item in enumerable)
                dict.Add(item.Key, item.Value);
            return dict;
        }

        public static void TryAddForever<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            while (true)
            {
                if (dict.TryAdd(key, value)) break;
            }
        }

        public static TValue TryRemoveForever<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            while (true)
            {
                if (dict.TryRemove(key, out value)) break;
            }
            return value;
        }

        public static void RemoveIfExists<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            if (!dict.ContainsKey(key)) return;

            dict.TryRemoveForever(key);
        }
    }
}
