//
// DictionaryExtension.cs
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

using Foundation;
using System.Linq;
using System.Collections.Generic;
using System;

namespace System.Collections.Generic
{
    public static class DictionaryExtension
    {
        public static NSDictionary ToNsDictionary<TK, TV>(this IDictionary<TK, TV> dict)
            where TK : NSObject
            where TV : NSObject
        {
            return NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }

        public static string DictionaryToJson(this NSDictionary dictionary)
        {
            NSError error;
            var json = NSJsonSerialization.Serialize(dictionary, NSJsonWritingOptions.PrettyPrinted, out error);

            return json.ToString(NSStringEncoding.UTF8);
        }

    }
}

namespace iOS
{
    public static class DictionaryExtension
    {
        public static TValue GetValue<TKey, TValue>(this IDictionary<WeakReference, TValue> dict, TKey key)
        {
            foreach (var item in dict)
                if (item.Key.IsAlive && item.Key.Target.Equals(key))
                    return item.Value;
            return default(TValue);
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

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> keyValuePair)
        {
            AddOrUpdate(dict, keyValuePair.Key, keyValuePair.Value);
        }

        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ContainsKey(key) ? dict[key] : default(TValue);
        }

        public static NSDictionary<TKey, TValue> ToGeneric<TKey, TValue>(this NSDictionary dict)
            where TKey : NSObject
            where TValue : NSObject
        {
            var listKeys = new List<TKey>();
            var listValues = new List<TValue>();
            foreach (var item in dict)
            {
                listKeys.Add(item.Key as TKey);
                listValues.Add(item.Value as TValue);
            }
            return new NSDictionary<TKey, TValue>(listKeys.ToArray(), listValues.ToArray());
        }

        public static NSMutableDictionary ToNSDictionary(this IDictionary<NSObject, NSObject> dict)
        {
            return NSMutableDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }

        public static NSMutableDictionary ToNSDictionary(this IDictionary<string, object> dict)
        {
            var newDictionary = new NSMutableDictionary();
            foreach (var k in dict.Keys)
            {
                var value = dict[k];

                try
                {
                    if (value is string)
                        newDictionary.Add((NSString)k, new NSString((string)value));
                    else if (value is int)
                        newDictionary.Add((NSString)k, new NSNumber((int)value));
                    else if (value is float)
                        newDictionary.Add((NSString)k, new NSNumber((float)value));
                    else if (value is nfloat)
                        newDictionary.Add((NSString)k, new NSNumber((nfloat)value));
                    else if (value is double)
                        newDictionary.Add((NSString)k, new NSNumber((double)value));
                    else if (value is bool)
                        newDictionary.Add((NSString)k, new NSNumber((bool)value));
                    else if (value is DateTime)
                        newDictionary.Add((NSString)k, ((DateTime)value).ToNsDate());
                    else
                        newDictionary.Add((NSString)k, new NSString(value.ToString()));
                }
                catch { }
            }
            return newDictionary;
        }
    }
}




