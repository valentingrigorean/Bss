//
// MemoryContainer.cs
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
namespace Bss.Droid.Utils
{
    public class MemoryContainer
    {
        private IDictionary<string, WeakReference> Map = new Dictionary<string, WeakReference>();


        public static MemoryContainer Instance { get; } = new MemoryContainer();

        public void AddOrUpdate(string key, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (Map.ContainsKey(key))
            {
                var val = Map[key];
                if (val.IsAlive)
                    val.Target = obj;
                else
                    Map[key] = new WeakReference(obj);
                return;
            }
            Map.Add(key, new WeakReference(obj));
        }

        public T Get<T>(string key)
        {
            return (T)Get(key);
        }

        public object Get(string key)
        {
            if (Map.ContainsKey(key))
            {
                var val = Map[key];
                if (val.IsAlive)
                    return val.Target;
                Map.Remove(key);
            }
            return null;
        }

        public object Pop(string key)
        {
            var val = Get(key);
            if (Map.ContainsKey(key))
                Map.Remove(key);
            return val;
        }

        public T Pop<T>(string key)
        {
            return (T)Pop(key);
        }

        public bool Contains(string key)
        {
            return Map.ContainsKey(key);
        }

        public void Clear()
        {
            Map.Clear();
        }
    }
}
