//
// StoryboardManager.cs
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
using UIKit;

namespace Bss.iOS.Utils
{
    public class StoryboardManager
    {
        private readonly HashSet<string> _storyboards = new HashSet<string> { "Main" };
        private readonly Dictionary<string, UIStoryboard> _storyboardMap = new Dictionary<string, UIStoryboard>();

        public static StoryboardManager SharedInstance { get; } = new StoryboardManager();

        public IEnumerable<string> Storyboards => _storyboards;

        public void Add(string storyboard)
        {
            if (_storyboards.Contains(storyboard)) return;
            _storyboards.Add(storyboard);
        }

        public void Remove(string storyboard)
        {
            _storyboards.Remove(storyboard);
        }

        [Obsolete(" This is not working as will throw native exception and can't be caught")]
        public T GetController<T>(Type type)
            where T : UIViewController
        {
            return (T)GetController(type);
        }

        public T GetController<T>(Type type, string storyboard)
            where T : UIViewController
        {
            return (T)GetController(type, storyboard);
        }

        [Obsolete(" This is not working as will throw native exception and can't be caught")]
        public UIViewController GetController(Type type)
        {
            return Storyboards.Select(GetStoryboard).Select(story => story.InstantiateViewController(type.Name)).FirstOrDefault(res => res != null);
        }

        public UIViewController GetController(Type type, string storyboard)
        {
            var story = GetStoryboard(storyboard);
            return story.InstantiateViewController(type.Name);
        }

        private UIStoryboard GetStoryboard(string name)
        {
            if (_storyboardMap.TryGetValue(name, out var storyboard))
                return storyboard;
            storyboard = UIStoryboard.FromName(name, null);
            if (storyboard == null)
                throw new Exception($"Could not initialize a storyboard from name:{name}");
            _storyboardMap.Add(name, storyboard);
            return storyboard;
        }


    }
}
