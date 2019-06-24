//
// UItextFieldExtension.cs
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
using Bss.Core.Utils;
using Bss.iOS.UIKit;
using CoreGraphics;
using Foundation;

namespace UIKit
{
    public static class UITextFieldExtension
    {
        public static IInputTextView ToTextView(this UITextField This)
        {
            return new TextFieldTextViewWrapper(This);
        }
        #region Placeholder
        public static void SetPlaceholderColor(this UITextField textField, UIColor color)
        {
            SetPlaceholderColor(textField, textField.Placeholder, color);
        }

        public static void SetPlaceholderColor(this UITextField textField, string text, UIColor color)
        {
            if (string.IsNullOrEmpty(text))
                return;
            textField.AttributedPlaceholder = new Foundation.NSAttributedString(text, null, color);
        }

        #endregion
        public static void SetPadding(this UITextField textField, nfloat paddingLeft)
        {
            var view = new UIView
            {
                Frame = new CGRect(paddingLeft, paddingLeft, paddingLeft, paddingLeft)
            };

            textField.LeftView = view;
            textField.LeftViewMode = UITextFieldViewMode.Always;
        }

        #region Return Key

        public static IDisposable SetReturnKey(this UITextField textView, UITextField nextView, UIReturnKeyType returnKey = UIReturnKeyType.Next)
        {
            textView.ReturnKeyType = returnKey;
            return textView.SetReturn(nextView);
        }

        public static IDisposable SetReturnKey(this UITextField textView, UIReturnKeyType returnKey = UIReturnKeyType.Done)
        {
            textView.ReturnKeyType = returnKey;
            return textView.SetReturn();
        }

        public static IDisposable SetReturn(this UITextField textView, UITextField nextView=null)
        {
            textView.ShouldReturn += (textField) => TextFieldShouldReturn(textField, nextView);
            return Disposable.Create(() => textView.ShouldReturn -= (textField) => TextFieldShouldReturn(textField));
        }

        private static bool TextFieldShouldReturn(IUITextInputTraits textfield, UITextField nextView = null)
        {
            switch (textfield.ReturnKeyType)
            {
                case UIReturnKeyType.Done:
                    (textfield as UITextField)?.ResignFirstResponder();
                    break;
                case UIReturnKeyType.Default:
                    break;
                case UIReturnKeyType.Go:
                    break;
                case UIReturnKeyType.Google:
                    break;
                case UIReturnKeyType.Join:
                    break;
                case UIReturnKeyType.Next:
                    nextView.BecomeFirstResponder();
                    break;
                case UIReturnKeyType.Route:
                    break;
                case UIReturnKeyType.Search:
                    break;
                case UIReturnKeyType.Send:
                    break;
                case UIReturnKeyType.Yahoo:
                    break;
                case UIReturnKeyType.EmergencyCall:
                    break;
                case UIReturnKeyType.Continue:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UIView.BeginAnimations(string.Empty, IntPtr.Zero);
            UIView.SetAnimationDuration(0.3);
            UIView.CommitAnimations();

            return false;
        }


        public static void ToolbarAddDone(this UITextField textView)
        {
			var btnKBFlexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			var btnKBDone = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) => textView.EndEditing(true));
            textView.InputAccessoryView = CreateToolbar(btnKBFlexibleSpace,btnKBDone);
        }

        private static UIToolbar CreateToolbar(params UIBarButtonItem[] arrayBtns)
        {
            var kbToolbar = new UIToolbar(CGRect.Empty);
            kbToolbar.BarStyle = UIBarStyle.Default;
			kbToolbar.Translucent = true;
			kbToolbar.UserInteractionEnabled = true;
			kbToolbar.SizeToFit();
            var btnKBItems = arrayBtns;
			kbToolbar.SetItems(btnKBItems, true);

            return kbToolbar;

        }

        #endregion

        #region Length

        public static void SetLength(this UITextField textView, int maxLength)
        {
            textView.ShouldChangeCharacters += MaxLength(maxLength);
        }

        private static UITextFieldChange MaxLength(int maxLength)
        {
            return new UITextFieldChange(delegate (UITextField textField, NSRange range, string replacementString)
            {
                var newLength = textField.Text.Length + replacementString.Length - (int)range.Length;
                return newLength <= maxLength;
            });
        }

        #endregion
    }
}
