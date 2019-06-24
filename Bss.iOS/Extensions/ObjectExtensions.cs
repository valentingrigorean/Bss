//
// ObjectExtension.cs
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
using Foundation;

namespace Bss.iOS
{
	public static class ObjectExtensions
	{
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			if (val.CompareTo(max) > 0) return max;
			return val;
		}

		public static NSNumber ToNative(this int This)
		{
			return new NSNumber(This);
		}

		public static NSNumber ToNative(this float This)
		{
			return new NSNumber(This);
		}

		public static NSNumber ToNative(this double This)
		{
			return new NSNumber(This);
		}

		public static NSString ToNative(this string This)
		{
			return new NSString(This);
		}

        public static NSObject ToNative<T>(this T This)
        {
            return new NSObjectHolder<T>(This);
        }

        public static T ToNetObject<T>(this NSObject This)
        {
			if (This == null)
				return default(T);

			if (!(This is NSObjectHolder<T>))
				throw new InvalidOperationException("Unable to convert to .NET object. Only NSObject created with .ToNative<T>() can be converted.");

            return ((NSObjectHolder<T>)This).Instance;
        }


        private class NSObjectHolder<T> : NSObject
        {
            public NSObjectHolder(T instance)
            {
                Instance = instance;
            }

            public readonly T Instance;
        }
	}

}

