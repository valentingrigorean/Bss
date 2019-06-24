//
// RadioGroupWrapper.cs
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
using System.Collections.Generic;
using UIKit;

namespace Bss.iOS.UIKit.RadioViewHelper
{
    public class RadioGroupWrapper
    {
        private int _currentSelected = None;

        protected readonly List<IRadioView> Container = new List<IRadioView>();

        public RadioGroupWrapper()
        {

        }

        public RadioGroupWrapper(ICollection<IRadioView> radioViews) : this(radioViews, null)
        {

        }

        public RadioGroupWrapper(ICollection<IRadioView> radioViews, RadioViewWrapper currentRadioView)
        {
            CurrentRadioView = currentRadioView;
            if (radioViews == null || radioViews.Count < 2)
                throw new Exception("radioViews can't be null or have less then 2 elements");
            Container.AddRange(radioViews);
            Initialiaze();
        }

        public const int None = -1;

        public bool AllowDeselect { get; set; } = false;

        public sealed class SelectionChangedEventArgs : EventArgs
        {
            public SelectionChangedEventArgs(int position, IRadioView radioView)
            {
                Position = position;
                RadioView = radioView;
            }

            public int Position { get; }

            public IRadioView RadioView { get; }
        }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public virtual int CurrentSelection
        {
            get => _currentSelected;
            set
            {
                if (value < 0 || value > Container.Count - 1 ||
                    _currentSelected == value)
                    return;

                if (_currentSelected >= 0)
                    Container[_currentSelected].Checked = false;
                Container[value].Checked = true;
                _currentSelected = value;
            }
        }

        public RadioViewWrapper CurrentRadioView { get; private set; }

        public void ClearSelection()
        {
            if (_currentSelected >= 0)
                Container[_currentSelected].Checked = false;
            _currentSelected = None;
        }

        protected void EmitSelection(int position, IRadioView view)
        {
            SelectionChanged?.Invoke(
                this, new SelectionChangedEventArgs(position, view));
        }

        private void Initialiaze()
        {
            foreach (var radio in Container)
            {
                var view = radio.View;
                view.OnClick(_ =>
                    {
                        var now = Container.IndexOf(radio);
                        CurrentSelection = now;
                        var newRadio = Container[now];
                        EmitSelection(now, newRadio);
                    });
            }
        }
    }
}

