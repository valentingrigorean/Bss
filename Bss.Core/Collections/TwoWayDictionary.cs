//
// Map.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Bss.Core.Collections
{
    public class TwoWayDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _forward = new Dictionary<TKey, TValue>();
        private readonly IDictionary<TValue, TKey> _reverse = new Dictionary<TValue, TKey>();

        public TValue this[TKey key]
        {
            get => _forward[key];
            set
            {
                _forward[key] = value;
                _reverse[value] = key;
            }
        }

        public TKey this[TValue key]
        {
            get => _reverse[key];
            set
            {
                _forward[value] = key;
                _reverse[key] = value;
            }
        }

        public ICollection<TKey> Keys => _forward.Keys;

        public ICollection<TValue> Values => _forward.Values;

        public int Count => _forward.Count;

        public bool IsReadOnly => _forward.IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _forward.Add(item);
            _reverse.Add(item.Value, item.Key);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _forward.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _forward.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _forward.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return _forward.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _forward.Remove(item);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _forward.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _forward.GetEnumerator();
        }
    }
}
