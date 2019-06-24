//
// ReactiveUIExtensions.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2017 (c) Grigorean Valentin
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
using System.Linq.Expressions;

namespace ReactiveUI
{
    public static class ReactiveUIExtensions
    {
        //public static IReactiveBinding<TView, TViewModel, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
        //   this TView view,
        //   TViewModel viewModel,
        //   Expression<Func<TViewModel, TVMProp>> vmProperty,
        //   Expression<Func<TView, TVProp>> viewProperty,
        //   Func<TVMProp, TVProp> vmToViewConverter)
        //  where TViewModel : class
        //  where TView : IViewFor
        //{
        //    var convertor = new BindingConvertor<TVMProp, TVProp>(vmToViewConverter);
        //    return view.OneWayBind(viewModel, vmProperty, viewProperty, null, convertor);
        //}

        //public static IReactiveBinding<TView, TViewModel, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
        //    this TView view,
        //    TViewModel viewModel,
        //    Expression<Func<TViewModel, TVMProp>> vmProperty,
        //    Expression<Func<TView, TVProp>> viewProperty,
        //    object conversionHint,
        //    Func<TVMProp, TVProp> vmToViewConverter)
        //   where TViewModel : class
        //   where TView : IViewFor
        //{
        //    var convertor = new BindingConvertor<TVMProp, TVProp>(vmToViewConverter);
        //    return view.OneWayBind(viewModel, vmProperty, viewProperty, conversionHint, convertor);
        //}

        //private class BindingConvertor<TVMProp, TVProp> : IBindingTypeConverter
        //{
        //    private readonly Func<TVMProp, TVProp> _vmToViewConverter;

        //    public BindingConvertor(Func<TVMProp, TVProp> vmToViewConverter)
        //    {
        //        _vmToViewConverter = vmToViewConverter;
        //    }

        //    public int GetAffinityForObjects(Type fromType, Type toType)
        //    {
        //        return 0;
        //    }

        //    public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
        //    {
        //        result = null;
        //        if (@from.GetType() == typeof(TVMProp) && toType == typeof(TVProp))
        //        {
        //            if (_vmToViewConverter == null)
        //                return false;
        //            result = _vmToViewConverter((TVMProp)@from);
        //            return true;
        //        }
        //        return false;
        //    }
        //}
    }
}
