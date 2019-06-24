//
// UITextExtension.cs
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
using System.Collections.Generic;
using UIKit;
using System;
using System.Linq;
using Bss.iOS.Utils;

// Analysis disable once CheckNamespace
using Bss.Core.Extensions;

public static class UiTextExtension
{
    private static readonly HashSet<Type> Types = new HashSet<Type>
    {
        typeof(UIButton),
        typeof(UILabel),
        typeof(UITextField),
        typeof(UITextView)
    };

    public static void SetFonts(this UIView grp,
                                int fontType,
                                ICollection<UIView> ignoreList = null)
    {
        var font = FontManager.Get(fontType);
        SetFonts(grp, font, ignoreList);
    }

    public static void SetFonts(this UIView grp, int fontType, params UILabel[] labels)
    {
        var font = FontManager.Get(fontType);
        foreach (var label in labels)
            label.Font = font.WithSize(label.Font.PointSize);
    }



    public static void SetFont(this UIButton label, int fontType)
    {
        var font = FontManager.Get(fontType);
        label.Font = font.WithSize(label.Font.PointSize);
    }


    public static void SetFont(this UIButton label, int fontType,
                               float size)
    {
        var font = FontManager.Get(fontType);
        label.Font = font.WithSize(size);
    }


    public static void SetFont(this UILabel label, int fontType)
    {
        var font = FontManager.Get(fontType);
        label.Font = font.WithSize(label.Font.PointSize);
    }

    public static void SetFont(this UILabel[] labels, int fontType)
    {
        var font = FontManager.Get(fontType);
        foreach (var item in labels)
            item.Font = font.WithSize(item.Font.PointSize);
    }

    public static void SetFont(this UILabel[] labels, int fontType,
                              float size)
    {
        var font = FontManager.Get(fontType);
        foreach (var item in labels)
            item.Font = font.WithSize(size);
    }


    public static void SetFont(this UILabel label, int fontType,
                               float size)
    {
        var font = FontManager.Get(fontType);
        label.Font = font.WithSize(size);
    }

    public static void SetFont(this UITextField textField, int fontType)
    {
        var font = FontManager.Get(fontType);
        textField.Font = font.WithSize(textField.Font.PointSize);
    }


    public static void SetFont(this UITextField textField, int fontType,
                               float size)
    {
        var font = FontManager.Get(fontType);
        textField.Font = font.WithSize(size);
    }


    public static void SetFont(this UITextView textView, int fontType)
    {
        var font = FontManager.Get(fontType);
        textView.Font = font.WithSize(textView.Font.PointSize);
    }


    public static void SetFont(this UITextView textView, int fontType,
                               float size)
    {
        var font = FontManager.Get(fontType);
        textView.Font = font.WithSize(size);
    }

    public static void SetText(this UILabel label, params string[] list)
    {
        label.Text = StringExtensions.StringBuilder(list);
    }



    private static void SetFonts(UIView grp, UIFont font, ICollection<UIView> ignoreList = null)
    {
        if (Types.Contains(grp.GetType()))
            SetFont(grp, font);
        foreach (var view in grp.Subviews.Where(view => ignoreList == null || (ignoreList.Select(_ =>
            ReferenceEquals(view, _)).ToList().Count != 0)))
        {
            if (Types.Contains(view.GetType()))
                SetFont(view, font);
            SetFonts(view, font, ignoreList);
        }
    }

    private static void SetFont(UIView view, UIFont font)
    {
        var lbl = view as UILabel;
        if (lbl != null)
        {
            lbl.Font = font.WithSize(lbl.Font.PointSize);
            return;
        }
        var btn = view as UIButton;
        if (btn != null)
        {
            btn.Font = font.WithSize(btn.Font.PointSize);
            return;
        }

        var txtField = view as UITextField;
        if (txtField != null)
        {
            txtField.Font = font.WithSize(txtField.Font.PointSize);
            return;
        }
        var txtView = view as UITextView;
        if (txtView != null)
        {
            txtView.Font = font.WithSize(txtView.Font.PointSize);

        }
    }


}


