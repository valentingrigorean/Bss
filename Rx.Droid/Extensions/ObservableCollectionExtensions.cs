using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Collections.Specialized;
using System;

namespace Rx.Extensions
{
	public static class ObservableCollectionExtensions
	{
		public static IObservable<NotifyCollectionChangedEventArgs> WhenCollectionChanged<T>(this ObservableCollection<T> This)
		{
			return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
				e => This.CollectionChanged += e, e => This.CollectionChanged -= e)
				.Select(args => args.EventArgs);
		}
	}
}
