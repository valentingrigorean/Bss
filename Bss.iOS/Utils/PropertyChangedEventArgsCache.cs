//
// PropertyChangedEventArgsCache.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2018 (c) Grigorean Valentin
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

using System.Collections.Generic;
using System.ComponentModel;

namespace Bss.iOS.Utils
{
    /// <summary>
    /// Provides a cache for <see cref="PropertyChangedEventArgs"/> instances.
    /// </summary>
    internal sealed class PropertyChangedEventArgsCache
    {
        /// <summary>
        /// The underlying dictionary. This instance is its own mutex.
        /// </summary>
        private readonly Dictionary<string, PropertyChangedEventArgs> _cache = new Dictionary<string, PropertyChangedEventArgs>();

        /// <summary>
        /// Private constructor to prevent other instances.
        /// </summary>
        private PropertyChangedEventArgsCache()
        {
        }

        /// <summary>
        /// The global instance of the cache.
        /// </summary>
        public static PropertyChangedEventArgsCache Instance { get; } = new PropertyChangedEventArgsCache();

        public static PropertyChangedEventArgs GetArgs(string propertyName) => Instance.Get(propertyName);

        /// <summary>
        /// Retrieves a <see cref="PropertyChangedEventArgs"/> instance for the specified property, creating it and adding it to the cache if necessary.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedEventArgs Get(string propertyName)
        {
            lock (_cache)
            {
                PropertyChangedEventArgs result;
                if (_cache.TryGetValue(propertyName, out result))
                    return result;
                result = new PropertyChangedEventArgs(propertyName);
                _cache.Add(propertyName, result);
                return result;
            }
        }

    }
}
