using Android.Views;
using System.Collections.Generic;

namespace Bss.Droid.Widgets
{
	public delegate AbsViewHolder<T> CreateViewHolderDelegate<T>(LayoutInflater inflater, ViewGroup parent);

	public static class RVAdapterFactory
	{
		public static RVBaseAdapter<T, AbsViewHolder<T>> CreateAdapter<T>(IList<T> items, CreateViewHolderDelegate<T> createFunc)
		{
			return new InternalRVAdapter<T>(items, createFunc);
		}

		public static RVBaseAdapter<T, AbsViewHolder<T>> CreateAdapter<T>(CreateViewHolderDelegate<T> createFunc)
		{
			return new InternalRVAdapter<T>(new List<T>(), createFunc);
		}

		private class InternalRVAdapter<T> : RVBaseAdapter<T, AbsViewHolder<T>>
		{
			private CreateViewHolderDelegate<T> _createFunc;

			public InternalRVAdapter(IList<T> items, CreateViewHolderDelegate<T> createFunc) : base(items)
			{
				_createFunc = createFunc;
			}

			public override AbsViewHolder<T> CreateHolder(ViewGroup parent, int viewType)
			{
				return _createFunc(LayoutInflater.FromContext(parent.Context), parent);
			}

			public override void OnBindViewHolder(AbsViewHolder<T> holder, int position)
			{
				holder.Bind(DataSource[position]);
			}
		}
	}
}