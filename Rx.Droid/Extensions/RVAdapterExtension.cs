using Bss.Droid.Widgets;
using Android.Support.V7.Widget;
using System;
using System.Reactive.Linq;

namespace Rx.Extensions
{
	public static class RVAdapterExtension
	{
		public static IObservable<RVBaseAdapter<T, VH>.ItemClickEventArgs> WhenItemClick<T, VH>(this RVBaseAdapter<T, VH> This)
			where VH : RecyclerView.ViewHolder
		{
			return Observable.FromEventPattern<RVBaseAdapter<T, VH>.ItemClickEventArgs>(
				e => This.ItemClick += e, e => This.ItemClick -= e)
				.Select(args => args.EventArgs);
		}
	}
}
