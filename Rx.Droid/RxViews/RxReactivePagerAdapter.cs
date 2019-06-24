//
// RxReactivePagerAdapter.cs
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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Android.Support.V4.App;
using Android.Support.V4.View;
using ReactiveUI;
using Rx.Droid.App;

namespace Rx.Droid.RxViews
{
    public class RxReactiveFragmentPagerAdapter<T> : FragmentPagerAdapter
        where T : class
    {
        private readonly Func<RxSupportFragment<T>> _fragmentCreator;

        private IReadOnlyReactiveList<T> _data;
        private IDisposable _inner;

        public RxReactiveFragmentPagerAdapter(FragmentManager fm, Func<RxSupportFragment<T>> fragmentCreator)
            : base(fm)
        {
            _fragmentCreator = fragmentCreator;
        }

        public Action<T, RxSupportFragment<T>> FragmentInitializer { get; set; }

        public IReadOnlyReactiveList<T> Data
        {
            get => _data;
            set
            {
                if (_data == value)
                    return;
                _inner?.Dispose();
                _data = value;
                _inner = _data.Changed
                              .Subscribe(_ => NotifyDataSetChanged());
                NotifyDataSetChanged();
            }
        }

        public override int Count => _data == null ? 0 : _data.Count;

        public override Fragment GetItem(int position)
        {
            var fragment = _fragmentCreator();
            var vm = Data[position];
            fragment.ViewModel = vm;
            FragmentInitializer?.Invoke(vm, fragment);
            return fragment;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }
    }

    public static class RxPagerAdapterExtensions
    {
        public static IDisposable BindTo<TSource, TFragment>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            ViewPager viewPager,
            FragmentManager fm,
            Action<TSource, TFragment> fragmentInitializer = null)
            where TSource : class
            where TFragment : RxSupportFragment<TSource>
        {
            var adapter = viewPager.Adapter as RxReactiveFragmentPagerAdapter<TSource>;
            if (adapter == null)
            {
                adapter = new RxReactiveFragmentPagerAdapter<TSource>(
                    fm, () => Activator.CreateInstance<TFragment>());
                viewPager.Adapter = adapter;
            }

            if (fragmentInitializer != null)
                adapter.FragmentInitializer = (arg1, arg2) => fragmentInitializer(arg1, (TFragment)arg2);

            var bind = sourceObservable.Cast<IReadOnlyReactiveList<TSource>>()
                                       .BindTo(adapter, x => x.Data);
            return new CompositeDisposable(bind);
        }
    }
}