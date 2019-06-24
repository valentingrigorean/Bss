//
// UINavigationControllerExtension.cs
//
// Author:
//       Valentin <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 Valentin
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
using System.Collections.Generic;
using System.Linq;


[Flags]
// Analysis disable once CheckNamespace
public enum IntentFlags
{
    /// <summary>
    /// The just push a new view
    /// </summary>
    None = 0x0,
    /// <summary>
    /// A->B->C->D going B will result A->B
    /// </summary>
    ClearTop = 0x1,

    /// <summary>
    /// WIll create new view
    /// </summary>
    NewTask = 0x2,
    /// <summary>
    /// A->B->C going D will result in D
    /// </summary>
    ReplaceRoot = 0x4
}

// Analysis disable once CheckNamespace
public class Intent
{

    public Intent(Type type)
    {
        Type = type;
        Name = type.Name;
    }

    public Intent(string name)
    {
        Name = name;
    }

    public Type Type { get; set; }

    public bool Animated { get; set; } = true;

    public string Name { get; set; }

    public string Storyboard { get; set; } = "Main";

    public Action<UIViewController> BeforePush { get; set; }

    public IntentFlags Flags { get; private set; }

    public void SetFlag(IntentFlags flag)
    {
        Flags |= flag;
    }

    public override string ToString()
    {
        return $"[Intent: Type={Type}, Animated={Animated}, Name={Name}, " +
               $"Storyboard={Storyboard}, BeforePush={BeforePush}, Flags={Flags}]";
    }
}

// Analysis disable once CheckNamespace
public static class NavigationControllerExtension
{
    /// <summary>
    /// Shows the controller.
    /// </summary>
    /// <param name="controller">Controller.</param>
    /// <param name="type">Type.</param>
    public static void ShowController(this UINavigationController controller,
									  Type type, bool animated = true)
    {
        var intent = new Intent(type) { Animated = animated };
        intent.SetFlag(IntentFlags.ClearTop);
        ShowController(controller, intent);
    }

	public static bool IsTopViewController(this UINavigationController This,Type type)
	{
		return This.ViewControllers.Last()?.GetType() == type;
	}

    /// <summary>
    /// Pops the view controller.
    /// </summary>
    /// <param name="controller">Controller.</param>
    /// <param name="intent">Intent.</param>
    public static void ShowController(this UINavigationController controller,
                                      Intent intent)
    {
        UIViewController dest;
        IList<UIViewController> stack;
        if (intent.Flags == IntentFlags.None)
        {
            dest = intent.Type.GetController();
            intent.BeforePush?.Invoke(dest);
            controller.PushViewController(dest, intent.Animated);
            return;
        }

        if ((intent.Flags & IntentFlags.ReplaceRoot) == IntentFlags.ReplaceRoot)
        {
            if ((intent.Flags & IntentFlags.NewTask) == IntentFlags.NewTask)
                dest = intent.Type.GetController(intent.Storyboard);
            else
                dest = GetController(controller, intent.Name) ?? intent.Type.GetController(intent.Storyboard);
            intent.BeforePush?.Invoke(dest);
            controller.SetViewControllers(new[] { dest }, intent.Animated);
            return;
        }

        if ((intent.Flags & IntentFlags.NewTask) == IntentFlags.NewTask)
        {

            dest = intent.Type.GetController(intent.Storyboard);
            if ((intent.Flags & IntentFlags.ClearTop) == IntentFlags.ClearTop)
            {
                stack = GetStackTill(controller, intent.Name);
                if (controller.ViewControllers.Length == stack.Count)
                    stack.Add(dest);
                else
                    stack[stack.Count - 1] = dest;
            }
            else
            {
                stack = GetStackWithoutType(controller, intent.Name);
                stack.Add(dest);
            }
            intent.BeforePush?.Invoke(dest);
            controller.SetViewControllers(stack.ToArray(), intent.Animated);
            return;
        }

        if ((intent.Flags & IntentFlags.ClearTop) != IntentFlags.ClearTop) return;

        stack = GetStackTill(controller, intent.Name);
        if (controller.ViewControllers.Length == stack.Count)
        {
            dest = intent.Type.GetController(intent.Storyboard);
            stack.Add(dest);
        }
        else
            dest = stack[stack.Count - 1];
        intent.BeforePush?.Invoke(stack[stack.Count - 1]);
        controller.SetViewControllers(stack.ToArray(), intent.Animated);
    }



    private static IList<UIViewController> GetStackTill(
        UINavigationController controller, string name)
    {
        var list = new List<UIViewController>();
        foreach (var item in controller.ViewControllers)
        {
            list.Add(item);
            if (item.GetType().Name == name)
                return list;
        }
        return list;
    }

    private static IList<UIViewController> GetStackWithoutType(
        UINavigationController controller, string name)
    {
        return controller.ViewControllers.Where(item => item.GetType().Name != name).ToList();
    }

    private static UIViewController GetController(UINavigationController controller,
                                                  string name)
    {
        return controller.ViewControllers.FirstOrDefault(item => item.GetType().Name == name);
    }
}


