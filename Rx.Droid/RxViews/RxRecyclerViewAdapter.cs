//
// RxRecyclerView.cs
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
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Android.Support.V7.Widget;
using Android.Views;
using ReactiveUI;
using Bss.Droid.Widgets;

namespace Rx.Droid.RxViews
{
    public class RecyclerItemClickEventArgs : EventArgs
    {
        public RecyclerItemClickEventArgs(int pos, object item, RecyclerView.ViewHolder viewHolder)
        {
            Position = pos;
            Item = item;
            ViewHolder = viewHolder;
        }

        public RecyclerView.ViewHolder ViewHolder { get; }
        public object Item { get; }
        public int Position { get; }
    }

    /// <summary>
    /// An adapter for the Android <see cref="RecyclerView"/>.
    /// 
    /// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method 
    /// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
    public abstract class ReactiveRecyclerViewAdapterFixed<TViewModel> : RecyclerView.Adapter, IStickyHeaderInterface
        where TViewModel : class, IReactiveObject
    {
        private readonly Subject<RecyclerItemClickEventArgs> _elementSelected = new Subject<RecyclerItemClickEventArgs>();
        private IReadOnlyReactiveList<TViewModel> _list;
        private IDisposable _inner;

        protected ReactiveRecyclerViewAdapterFixed()
        {

        }

        protected ReactiveRecyclerViewAdapterFixed(IReadOnlyReactiveList<TViewModel> backingList)
        {
            Data = backingList;
        }

        public IObservable<RecyclerItemClickEventArgs> ElementSelected => _elementSelected.AsObservable();

        public virtual int OffsetPosition => 0;

        public IReadOnlyReactiveList<TViewModel> Data
        {
            get => _list;
            set
            {
                if (_list == value)
                    return;
                _list = value;
                _inner?.Dispose();
                _inner = WireEvents(value);
                NotifyDataSetChanged();
            }
        }

        public TViewModel this[int index] => Data[index + OffsetPosition];

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((IViewFor)holder).ViewModel = this[position];
        }

        public override int ItemCount => Data?.Count ?? 0;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }

        protected void OnElementSelected(RecyclerItemClickEventArgs e)
        {
            _elementSelected.OnNext(e);
        }

        protected void OnElementSelected(RecyclerView.ViewHolder viewHolder)
        {
            OnElementSelected(new RecyclerItemClickEventArgs(
                viewHolder.AdapterPosition, this[viewHolder.AdapterPosition], viewHolder));
        }

        protected void OnElementSelected(int pos)
        {
            OnElementSelected(new RecyclerItemClickEventArgs(pos, this[pos], null));
        }

        protected virtual IDisposable WireEvents(IReadOnlyReactiveList<TViewModel> list)
        {
            return list.Changed.Subscribe(e =>
             {
                 switch (e.Action)
                 {
                     case NotifyCollectionChangedAction.Add:
                         NotifyItemInserted(e.NewStartingIndex + OffsetPosition);
                         break;
                     case NotifyCollectionChangedAction.Remove:
                         NotifyItemRemoved(e.OldStartingIndex + OffsetPosition);
                         break;
                     case NotifyCollectionChangedAction.Move:
                         NotifyItemMoved(e.OldStartingIndex + OffsetPosition, e.NewStartingIndex + OffsetPosition);
                         break;
                     case NotifyCollectionChangedAction.Replace:
                         NotifyItemChanged(e.NewStartingIndex + OffsetPosition);
                         break;
                     case NotifyCollectionChangedAction.Reset:
                         NotifyDataSetChanged();
                         break;
                 }
             });
        }

        public virtual int GetHeaderPositionForItem(int itemPosition)
        {
            var headerPosition = 0;
            do
            {
                if (IsHeader(itemPosition))
                {
                    headerPosition = itemPosition;
                    break;
                }
                itemPosition -= 1;
            } while (itemPosition >= 0);
            return headerPosition;
        }

        public virtual int GetHeaderLayout(int headerPosition)
        {
            throw new NotImplementedException();
        }

        public virtual void BindHeaderData(View header, int headerPosition)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsHeader(int itemPosition)
        {
            throw new NotImplementedException();
        }
    }

    public class RxRecyclerViewAdapter<T> : RecyclerView.Adapter, IStickyHeaderInterface
        where T : class, IReactiveObject
    {
        private readonly Subject<RecyclerItemClickEventArgs> _itemClick = new Subject<RecyclerItemClickEventArgs>();
        private WeakReference<RecyclerView> _recyclerViewWeakRef = new WeakReference<RecyclerView>(null);

        private IDisposable _inner;
        private CompositeDisposable _subscriptionDisposables = new CompositeDisposable();

        private IReadOnlyReactiveList<T> _list;

        private int _itemSelected;

        private RecyclerView RecyclerView
        {
            get
            {
                _recyclerViewWeakRef.TryGetTarget(out RecyclerView view);
                return view;
            }
        }

        protected RxRecyclerViewAdapter(CreateView createView)
        {
            ViewFactory = createView;
        }

        protected RxRecyclerViewAdapter(CreateView createView, IReadOnlyReactiveList<T> backingList)
        {
            ViewFactory = createView;

            InitialiazeDataChanged(backingList);
        }

        public static RxRecyclerViewAdapter<T> Create(
            int id,
            Func<View, RxViewHolder<T>> viewHolderFactory,
            IReadOnlyReactiveList<T> backinglist)
        {
            return new RxRecyclerViewAdapter<T>((parent, viewType) =>
            {
                var view = LayoutInflater.FromContext(parent.Context).Inflate(id, parent, false);
                return viewHolderFactory(view);
            }, backinglist);
        }

        public static RxRecyclerViewAdapter<T> Create(
          int id,
          Func<View, RxViewHolder<T>> viewHolderFactory)
        {
            return new RxRecyclerViewAdapter<T>((parent, viewType) =>
            {
                var view = LayoutInflater.FromContext(parent.Context).Inflate(id, parent, false);
                return viewHolderFactory(view);
            });
        }

        public IReadOnlyReactiveList<T> Data
        {
            get => _list;
            set
            {
                if (_list == value)
                    return;
                InitialiazeDataChanged(value);
            }
        }

        protected CreateView ViewFactory { get; set; }

        protected CompositeDisposable SubscriptionDisposables => _subscriptionDisposables;

        public delegate RecyclerView.ViewHolder CreateView(ViewGroup parent, int viewType);

        public IObservable<RecyclerItemClickEventArgs> ItemClick => _itemClick.AsObservable();

        public bool AllowSelection { get; set; } = true;

        public virtual int ItemSelected
        {
            get => _itemSelected;
            set
            {
                if (value == _itemSelected)
                    return;
                if (!AllowSelection)
                    return;

                var previewsItem = _itemSelected;
                _itemSelected = value;
                if (previewsItem >= 0)
                    SetSelected(GetViewHolderFrom(RecyclerView, previewsItem), false);

                if (value >= 0 && value < ItemCount)
                    SetSelected(GetViewHolderFrom(RecyclerView, value), true);
            }
        }

        public override int ItemCount => Data == null ? 0 : Data.Count;

        public void SetItemSelected(T item)
        {
            if (Data == null)
                return;
            var index = -1;
            foreach (var element in Data)
            {
                index++;
                if (element == item)
                    break;
            }

            ItemSelected = index;
        }

        protected override void Dispose(bool disposing)
        {
            Interlocked.Exchange(ref _subscriptionDisposables, new CompositeDisposable()).Dispose();
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
            base.Dispose(disposing);
        }

        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            _recyclerViewWeakRef.SetTarget(recyclerView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as RxViewHolder<T>;
            vh.ViewModel = Data[position];
            if (position == ItemSelected && AllowSelection)
                vh.ItemSelected = true;
        }

        /// <summary>
        /// Ons the create view holder.
        /// Notes: set ViewFactory  
        /// </summary>
        /// <returns>The create view holder.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="viewType">View type.</param>
        public sealed override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            var vh = ViewFactory(parent, viewType);

            switch (vh)
            {
                case RxViewHolder<T> rxVH:
                    rxVH.Selected
                         .Select(pos => new RecyclerItemClickEventArgs(pos, rxVH.ViewModel, vh))
                         .Do(args =>
                         {
                             if (!AllowSelection)
                                 return;
                             ItemSelected = args.Position;
                         })
                         .Subscribe(_itemClick)
                         .DisposeWith(SubscriptionDisposables);
                    break;
                default:
                    break;
            }

            return vh;
        }

        private void SetSelected(RecyclerView.ViewHolder vh, bool isSelected)
        {
            if (vh == null)
                return;
            switch (vh)
            {
                case RxViewHolder<T> rxVH:
                    rxVH.ItemSelected = isSelected;
                    break;
                default:
                    vh.ItemView.Selected = isSelected;
                    break;
            }
        }

        private void InitialiazeDataChanged(IReadOnlyReactiveList<T> backingList)
        {
            if (_inner != null)
                _inner.Dispose();
            _list = backingList;

            _inner = _list.Changed
                          .Subscribe(e =>
                          {
                              switch (e.Action)
                              {
                                  case NotifyCollectionChangedAction.Add:
                                      NotifyItemInserted(e.NewStartingIndex);
                                      break;
                                  case NotifyCollectionChangedAction.Remove:
                                      NotifyItemRemoved(e.OldStartingIndex);
                                      break;
                                  case NotifyCollectionChangedAction.Move:
                                      NotifyItemMoved(e.OldStartingIndex, e.NewStartingIndex);
                                      break;
                                  case NotifyCollectionChangedAction.Replace:
                                      NotifyItemChanged(e.NewStartingIndex);
                                      break;
                                  case NotifyCollectionChangedAction.Reset:
                                      NotifyDataSetChanged();
                                      break;
                              }
                          });

            NotifyDataSetChanged();
        }

        public virtual int GetHeaderPositionForItem(int itemPosition)
        {
            var headerPosition = 0;
            do
            {
                if (IsHeader(itemPosition))
                {
                    headerPosition = itemPosition;
                    break;
                }
                itemPosition -= 1;
            } while (itemPosition >= 0);
            return headerPosition;
        }

        public virtual int GetHeaderLayout(int headerPosition)
        {
            throw new NotImplementedException();
        }

        public virtual void BindHeaderData(View header, int headerPosition)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsHeader(int itemPosition)
        {
            throw new NotImplementedException();
        }

        private static RecyclerView.ViewHolder GetViewHolderFrom(RecyclerView recyclerView, int position)
        {
            return recyclerView?.FindViewHolderForLayoutPosition(position);
        }
    }
}

namespace ReactiveUI
{
    using Rx.Droid.RxViews;

    public static class RxRecyclerViewAdapterExtensions
    {
        public static void SetItemSelected<T>(this RecyclerView This, T item)
            where T : ReactiveObject
        {
            var adapter = This.GetAdapter() as RxRecyclerViewAdapter<T>;
            if (adapter == null)
                return;
            adapter.SetItemSelected(item);
        }

        public static IDisposable BindTo<TSource, TViewHolder>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            RecyclerView recyclerView,
            int cellId,
            Func<RxRecyclerViewAdapter<TSource>, IDisposable> initAdapter = null,
            Func<RecyclerView, RecyclerView.Adapter> getAdapter = null)
            where TSource : ReactiveObject
            where TViewHolder : RxViewHolder<TSource>
        {
            Func<RxRecyclerViewAdapter<TSource>> adapterFunction =
                () => RxRecyclerViewAdapter<TSource>.Create(cellId, (view) =>
                {
                    return Activator.CreateInstance(typeof(TViewHolder), view) as RxViewHolder<TSource>;
                });
            return BindTo(sourceObservable, recyclerView, adapterFunction, initAdapter, getAdapter);
        }

        public static IDisposable BindTo<TSource>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            RecyclerView recyclerView,
            Func<RxRecyclerViewAdapter<TSource>> adapterFactory,
            Func<RxRecyclerViewAdapter<TSource>, IDisposable> initAdapter = null,
            Func<RecyclerView, RecyclerView.Adapter> getAdapter = null)
            where TSource : ReactiveObject
        {
            var adapter = (getAdapter == null ? recyclerView.GetAdapter() : getAdapter.Invoke(recyclerView))
                as RxRecyclerViewAdapter<TSource>;

            if (adapter == null)
            {
                adapter = adapterFactory();
                recyclerView.SetAdapter(adapter);
            }
            var vhDisposable = initAdapter?.Invoke(adapter) ?? Disposable.Empty;
            var bind = sourceObservable.Cast<IReadOnlyReactiveList<TSource>>()
                                       .BindTo(adapter, x => x.Data);
            return new CompositeDisposable(bind, vhDisposable);
        }
    }
}