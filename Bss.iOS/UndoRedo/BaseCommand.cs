//
// BaseCommand.cs
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
namespace Bss.iOS.UndoRedo
{
    public delegate void ChangeCallback<T>(T newValue, object target = null);

    public abstract class BaseCommand<T> : ICommand
    {
        protected BaseCommand(T currentValue, T newValue, ChangeCallback<T> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback), $"{nameof(callback)} == null");
            CurrentValue = currentValue;
            NewValue = newValue;
            Callback = callback;
        }

        public T CurrentValue { get; }
        public T NewValue { get; }

        public object Target { get; set; }

        public ChangeCallback<T> Callback { get; }

        public virtual void Redo()
        {
            Callback?.Invoke(NewValue, Target);
        }

        public virtual void Undo()
        {
            Callback?.Invoke(CurrentValue, Target);
        }
    }
}

