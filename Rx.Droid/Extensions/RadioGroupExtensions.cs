using System;
using Android.Widget;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Rx.Extensions
{
	public static class RadioGroupExtensions
	{
		/// <summary>
		/// Warning: Don't set IOnCheckedChangeListener or this will not work
		/// </summary>
		/// <returns>The checked change.</returns>
		/// <param name="This">This.</param>
		public static IObservable<RadioGroup.CheckedChangeEventArgs> WhenCheckedChange(this RadioGroup This)
		{
			return Observable.Create<RadioGroup.CheckedChangeEventArgs>(observer =>
			{
				This.SetOnCheckedChangeListener(new CheckedChangeListener(observer));
				return Disposable.Create(() => This.SetOnCheckedChangeListener(null));
			});
		}

		private class CheckedChangeListener : Java.Lang.Object, RadioGroup.IOnCheckedChangeListener
		{
			private readonly IObserver<RadioGroup.CheckedChangeEventArgs> _observer;

			public CheckedChangeListener(IObserver<RadioGroup.CheckedChangeEventArgs> observer)
			{
				_observer = observer;
			}

			public void OnCheckedChanged(RadioGroup group, int checkedId)
			{
				_observer.OnNext(new RadioGroup.CheckedChangeEventArgs(checkedId));
			}
		}
	}
}
