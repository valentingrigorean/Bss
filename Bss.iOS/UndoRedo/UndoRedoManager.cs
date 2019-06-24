//
// Memento.cs
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

namespace Bss.iOS.UndoRedo
{
    public class UndoRedoManager
    {
        private LinkedList<ICommand> _undoStack;
        private LinkedList<ICommand> _redoStack;
        private bool _redoFlag;

        public UndoRedoManager()
        {
            Capacity = -1;
            Initialize();
        }

        public UndoRedoManager(int capacity) : this()
        {
            if (capacity <= 0)
                throw new Exception($"{nameof(capacity)} most be higher then 0");
            Capacity = capacity;
        }

        public event EventHandler StatedChanged;

        public int Capacity { get; }

        public int Count => _undoStack.Count + _redoStack.Count;

        public void Add(ICommand cmd)
        {
            if (_redoFlag)
            {
                _redoFlag = false;
                _redoStack.Clear();
            }
            if (Capacity > 0 && Count == Capacity)
            {
                _undoStack.RemoveFirst();
            }
            _undoStack.AddLast(cmd);
            NotifyStateChanged();
        }

        public bool CanUndo => _undoStack.Count > 0;


        public bool CanRedo => _redoStack.Count > 0;

        public void Undo()
        {
            if (!CanUndo) return;
            var cmd = _undoStack.Last();
            _undoStack.RemoveLast();
            _redoStack.AddLast(cmd);
            cmd.Undo();
            NotifyStateChanged();
        }

        public void Redo()
        {
            if (!CanRedo) return;
            var cmd = _redoStack.Last();
            _redoStack.RemoveLast();
            _undoStack.AddLast(cmd);
            cmd.Redo();
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            StatedChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Initialize()
        {
            _undoStack = new LinkedList<ICommand>();
            _redoStack = new LinkedList<ICommand>();
        }
    }
}

