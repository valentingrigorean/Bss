//
// UITextViewExtension.cs
//
// Author:
//       Songurov Fiodor <f.songurov@software-dep.net>
//
// Copyright (c) 2016 Songurov
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

using Foundation;
using UIKit;
using Bss.iOS.UIKit;

namespace Bss.iOS.Extensions
{
    public static class UITextViewExtension
    {
        private static bool Close(UITextView textView, NSRange range, string value)
        {
            if (value.Equals("\n"))
            {
                textView.ResignFirstResponder();
            }
            return true;
        }

        public static ITextView ToTextView(this UITextView This)
        {
            return new TextViewWrapper(This);
        }

        public static void SetReturn(this UITextView textView)
        {
            textView.ShouldChangeText = Close;
            textView.ReturnKeyType = UIReturnKeyType.Done;
            textView.EnablesReturnKeyAutomatically = true;
        }

    }
}

