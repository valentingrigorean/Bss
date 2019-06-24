﻿//
// Subject.cs
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
using System.Collections.Generic;
using System.Linq;

namespace Bss.iOS.Observer
{
    public abstract class Subject : ISubject
    {
        private readonly Dictionary<string, List<IObserver>> _observers = new Dictionary<string, List<IObserver>>();


        public void Attach(IObserver observer, string notificationFor)
        {
            if (!_observers.ContainsKey(notificationFor))
                _observers.Add(notificationFor, new List<IObserver>());
            var list = _observers[notificationFor];
            if (!list.Contains(observer))
                list.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            foreach (var item in _observers.Where(item => item.Value.Contains(observer)))
            {
                item.Value.Remove(observer);
            }
        }

        public void Notify(string notificationFor, object obj = null)
        {
            if (!_observers.ContainsKey(notificationFor)) return;
            var list = _observers[notificationFor];
            foreach (var item in list)
                item.Update(notificationFor, obj);
        }
    }
}
