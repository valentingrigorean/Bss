//
// UIButtonView.cs
//
// Author:
//       Valentin <valentin.grigorean1@gmail.com>
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
using System.Collections.Generic;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Linq;

namespace Bss.iOS.UIKit
{
    public interface IButtonDelegate
    {
        void HandleState(UIView view, TState state);
    }

    public enum TState
    {
        Began,
        MoveIn,
        MoveOut,
        Ended
    }

    public enum ButtonState
    {
        Normal,
        InsidePress,
        OutsidePress,
        Highlighted
    }

    /// <summary>
    /// A class that highlight when is pressed using TintColor
    /// Good to use when ur using view as a button
    /// </summary>
    [Register("UIButtonView"), DesignTimeVisible(true)]
    public class UIButtonView : ClickableView, IButtonDelegate
    {
        private bool _wasSetTint;
        private bool _useHalfCorner;
        private float _cornerRadius;

        private UILabel _titleLabel;

        private TState _currentState;
        private UIColor _currentColor;

        private bool _highlighted;

        private UIImageView _imageView;

        private readonly Dictionary<ButtonState, TextState> StateMap = new Dictionary<ButtonState, TextState>();

        public UIButtonView()
        {
            Initialize();
            Text = "";
        }

        public UIButtonView(IntPtr ptr)
            : base(ptr)
        {
            Initialize();
        }

        public UIButtonView(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        public UIButtonView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        public UIButtonView(NSObjectFlag t)
            : base(t)
        {
            Initialize();
        }

        public string Text
        {
            get { return GetTitle(); }
            set { SetTitle(value); }
        }

        public UIImageView ImageView
        {
            get
            {
                if (_imageView == null)
                    CheckIfHaveImage();
                return _imageView;
            }
            private set { _imageView = value; }
        }

        public UILabel TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                    CheckIfHaveLbl();
                return _titleLabel;
            }
            private set { _titleLabel = value; }
        }

        public override bool Highlighted
        {
            get { return _highlighted; }
            set
            {
                _highlighted = value;
                var state = value ? ButtonState.Highlighted : ButtonState.Normal;
                if (StateMap.ContainsKey(state))
                {
                    State = state;
                    SetCurrentState();
                }
            }
        }

        public new ButtonState State { get; private set; }

        public bool ShouldHighlightSubviewsOnTouch { get; set; } = true;

        public IButtonDelegate Delegate { get; set; }

        public delegate void StateChangedCallback(UIView view, TState state);

        public StateChangedCallback StateChanged { get; set; }

        [Obsolete("Title is deprecated, please use Text instead.")]
        public string Title
        {
            get { return GetTitle(); }
            set { SetTitle(value); }
        }

        protected sealed class HighlightChangedEventArgs : EventArgs
        {
            public HighlightChangedEventArgs(bool highlighted)
            {
                Highlighted = highlighted;
            }

            public bool Highlighted { get; }
        }

        protected event EventHandler<HighlightChangedEventArgs> HighlightChanged = delegate { };

        [Export("CornerRadius"), Browsable(true)]
        public virtual float CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = value;
                if (_useHalfCorner)
                    return;

                Layer.CornerRadius = _cornerRadius;
                Layer.MasksToBounds = true;
                SetNeedsDisplay();
            }
        }

        [Export("HalfHeighCorner"), Browsable(true)]
        public bool HalfHeighCorner
        {
            get { return _useHalfCorner; }
            set
            {
                _useHalfCorner = value;
                Layer.CornerRadius = value ? Bounds.Height / 2f : _cornerRadius;
                Layer.MasksToBounds = true;
                SetNeedsDisplay();
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                if (!EnabledHandler?.Invoke(this, value) ?? false)
                {
                    if (value)
                    {
                        Alpha = 1;
                        UserInteractionEnabled = true;
                    }
                    else
                    {
                        Alpha = 0.5f;
                        UserInteractionEnabled = false;
                    }
                }
            }
        }

        public delegate bool EnabledHandlerDelegate(UIView view, bool enabled);

        public EnabledHandlerDelegate EnabledHandler { get; set; } = (view, enabled) => false;

        public override void SendActionForControlEvent()
        {
            _currentState = TState.Ended;
            HandleState();
            base.SendActionForControlEvent();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            foreach (var view in Subviews)
                view.UserInteractionEnabled = false;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (!Enabled)
                return;
            base.TouchesBegan(touches, evt);
            _currentState = TState.Began;
            HandleState();
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            if (!Enabled)
                return;
            base.TouchesMoved(touches, evt);
            if (_currentState == TState.Ended)
                return;
            var arr = touches.ToArray<UITouch>();
            var touch = arr[0];
            var state = PointInside(touch.LocationInView(touch.View), null) ?
                TState.MoveIn : TState.MoveOut;
            if (state == _currentState)
                return;
            _currentState = state;
            HandleState();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (!Enabled)
                return;
            base.TouchesEnded(touches, evt);
            if (_currentState == TState.Ended)
                return;
            _currentState = TState.Ended;
            HandleState();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            if (!Enabled)
                return;
            base.TouchesCancelled(touches, evt);
            if (_currentState == TState.Ended)
                return;
            _currentState = TState.Ended;
            HandleState();
        }

        /// <summary>
        /// Sets the title.
        /// Will get only first label in view
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="state">State.</param>
        public void SetTitle(string text, ButtonState state = ButtonState.Normal)
        {
            CheckIfHaveLbl();
            if (StateMap.ContainsKey(state))
                StateMap[state].Text = text;
            else
                StateMap.Add(state, new TextState
                {
                    Text = text
                });
            SetCurrentState();
        }

        public string GetTitle(ButtonState state = ButtonState.Normal)
        {
            if (_titleLabel == null)
                return "";
            if (StateMap.ContainsKey(state))
                return StateMap[state].Text;
            return _titleLabel.Text;
        }

        /// <summary>
        /// Sets the title.
        /// Will get only first label in view
        /// </summary>
        /// <param name="color">color.</param>
        /// <param name="state">State.</param>
        public void SetTitleColor(UIColor color, ButtonState state)
        {
            CheckIfHaveLbl();
            if (StateMap.ContainsKey(state))
                StateMap[state].Color = color;
            else
                StateMap.Add(state, new TextState
                {
                    Text = TitleLabel.Text,
                    Color = color,
                });

            SetCurrentState();
        }

        private void SetCurrentState()
        {
            if (_titleLabel == null || StateMap.Count == 0)
                return;

            var state = State;

            if (StateMap.Count == 1 && state != StateMap.Keys.First())
            {
                state = StateMap.Keys.First();
            }

            var currentState = StateMap[state];
            TitleLabel.Text = currentState.Text;
            if (currentState.Color != null)
                TitleLabel.TextColor = currentState.Color;
        }

        private void CheckIfHaveImage()
        {
            if (_imageView != null)
                return;
            _imageView = this.GetViewWithType<UIImageView>();
            if (_imageView == null)
                throw new Exception("Could not find any labels in subviews please have atleast 1 label");
        }

        private void CheckIfHaveLbl()
        {
            if (_titleLabel != null)
                return;
            _titleLabel = this.GetViewWithType<UILabel>();
            if (_titleLabel == null)
                throw new Exception("Could not find any labels in subviews please have atleast 1 label");
        }

        private void HandleState()
        {
            switch (_currentState)
            {
                case TState.Began:
                case TState.MoveIn:
                    State = ButtonState.InsidePress;
                    break;
                case TState.MoveOut:
                    State = ButtonState.OutsidePress;
                    break;
                case TState.Ended:
                    State = Highlighted ? ButtonState.Highlighted : ButtonState.Normal;
                    break;
            }

            if (Delegate != this && StateChanged != null)
                throw new Exception("You can't use both delegate and lambda function");
            if (StateChanged != null)
                StateChanged?.Invoke(this, _currentState);
            else
            {
                Delegate?.HandleState(this, _currentState);
            }
        }

        void IButtonDelegate.HandleState(UIView view, TState state)
        {
            SetCurrentState();
            switch (state)
            {
                case TState.Began:
                case TState.MoveIn:
                    if (!_wasSetTint)
                    {
                        _wasSetTint = true;
                        _currentColor = BackgroundColor;
                        BackgroundColor = TintColor;
                    }
                    break;
                case TState.Ended:
                case TState.MoveOut:
                    if (_wasSetTint)
                    {
                        _wasSetTint = false;
                        BackgroundColor = _currentColor;
                    }
                    break;
            }
            if (ShouldHighlightSubviewsOnTouch)
                NotifiyAllSubViews(_wasSetTint, this);
            HighlightChanged?.Invoke(this, new HighlightChangedEventArgs(_wasSetTint));
        }

        private void NotifiyAllSubViews(bool hightlighted, UIView view)
        {
            if (view.Subviews == null || view.Subviews.Length == 0)
                return;
            for (var i = 0; i < view.Subviews.Length; i++)
            {
                var child = view.Subviews[i];
                var img = child as UIImageView;
                if (img?.HighlightedImage != null)
                    img.Highlighted = hightlighted;
                var lbl = child as UILabel;
                if (lbl != null)
                    lbl.Highlighted = hightlighted;

                NotifiyAllSubViews(hightlighted, child);
            }
        }

        private void Initialize()
        {
            Layer.MasksToBounds = true;
            Delegate = this;
        }

        private class TextState
        {
            public string Text { get; set; }
            public UIColor Color { get; set; }
        }
    }
}
