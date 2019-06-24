using System;
using Android.Widget;
using System.Reactive.Linq;
using Android.Text;
using Android.Support.V7.Widget;

namespace Rx.Extensions
{
	public static class EditTextExtensions
	{
		public static IObservable<TextChangedEventArgs> WhenTextChanged(this EditText This)
		{
			return Observable.FromEventPattern<TextChangedEventArgs>(e => This.TextChanged += e, e => This.TextChanged -= e)
							 .Select(args => args.EventArgs);
		}

		public static IObservable<TextChangedEventArgs> WhenTextChanged(this AppCompatEditText This)
		{
			return Observable.FromEventPattern<TextChangedEventArgs>(e => This.TextChanged += e, e => This.TextChanged -= e)
							 .Select(args => args.EventArgs);
		}

        public static IDisposable WhenTextChanged(this EditText This, Action<string> callback)
        {
            return This.WhenTextChanged()
                       .Select(e => e.Text)
                       .Subscribe(e => callback?.Invoke(e.ToString()));
        }

        public static IDisposable WhenTextChanged(this AppCompatEditText This, Action<string> callback)
        {
            return This.WhenTextChanged()
                       .Select(e => e.Text)
                       .Subscribe(e => callback?.Invoke(e.ToString()));
        }
    }
}
