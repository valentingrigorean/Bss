//
// NSObjectExtensions.cs
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
using System;
using System.Runtime.InteropServices;
using Foundation;

namespace Bss.iOS.Extensions
{
    public enum AssociationPolicy
    {
        Assign = 0,
        RetainNonAtomic = 1,
        CopyNonAtomic = 3,
        Retain = 01401,
        Copy = 01403,
    }

    public static class NSObjectExtensions
    {
        [DllImport("/usr/lib/libobjc.dylib")]
        private static extern void objc_setAssociatedObject(IntPtr obj, IntPtr key, IntPtr value, AssociationPolicy policy);

        [DllImport("/usr/lib/libobjc.dylib")]
        private static extern IntPtr objc_getAssociatedObject(IntPtr obj, IntPtr key);

        public static void SetAssociatedObject<T>(this NSObject self, NSString key, T val, AssociationPolicy policy = AssociationPolicy.Retain)
            where T : NSObject
        {
            objc_setAssociatedObject(self.Handle, key.Handle, val.Handle, policy);
        }

        public static T GetAssociatedObject<T>(this NSObject self, NSString key)
            where T : NSObject
        {
            var pointer = objc_getAssociatedObject(self.Handle, key.Handle);
            return ObjCRuntime.Runtime.GetNSObject<T>(pointer);
        }    
    }
}
