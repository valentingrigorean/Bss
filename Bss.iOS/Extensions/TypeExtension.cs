//
// TypeExtension.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 valentingrigorean
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
using UIKit;


// Analysis disable once CheckNamespace
using Bss.iOS.Utils;
public static class TypeExtension
{

    public static StoryboardManager StoryboardManager { get; set; } = StoryboardManager.SharedInstance;


    ///// <summary>
    ///// Gets the controller.
    ///// Will use StoryboardManager.SharedInstance if not set
    ///// </summary>
    ///// <returns>The controller.</returns>
    ///// <param name="type">Type.</param>
    ///// <typeparam name="T">The 1st type parameter.</typeparam>
    //public static T GetController<T>(this Type type)
    //    where T : UIViewController
    //{
    //    return (T)StoryboardManager.GetController(type);
    //}

    public static T GetController<T>(this Type type, string storyboard = "Main")
        where T : UIViewController
    {
#if DEBUG
        DebugInfo();
#endif
        var vc = (T)StoryboardManager.GetController(type, storyboard);
#if DEBUG
        ConsoleLogger.Write($"Successfully created the {type.Name}");
#endif
        return vc;
    }

    ///// <summary>
    ///// Gets the controller.
    ///// Will use StoryboardManager.SharedInstance if not set
    ///// </summary>
    ///// <returns>The controller.</returns>
    ///// <param name="type">Type.</param>
    //public static UIViewController GetController(this Type type)
    //{
    //    return StoryboardManager.GetController(type);
    //}

    public static UIViewController GetController(this Type type, string storyboard = "Main")
    {
#if DEBUG
        DebugInfo();
#endif
        var vc = StoryboardManager.GetController(type, storyboard);
#if DEBUG
        ConsoleLogger.Write($"Successfully created the {type.Name}");
#endif
        return vc;
    }

#if DEBUG
    private static void DebugInfo()
    {
        ConsoleLogger.Write("GetController function can fail if can't find the ViewController" +
                            " in storyboard without any exception", LogLevel.Warning);
    }
#endif
}


