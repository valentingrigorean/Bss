using System;
namespace Bss.iOS.Utils
{
	public class BIWeakReference<T>
		where T : class
	{
		private readonly WeakReference<T> _weakRef;

		public BIWeakReference() : this(default(T))
		{
		}

		public BIWeakReference(T target)
		{
			_weakRef = new WeakReference<T>(target);
		}

		public bool HasValue => Target != default(T);

		public T Target
		{
			get
			{
				T target;
				_weakRef.TryGetTarget(out target);
				return target;
			}
			set { _weakRef.SetTarget(value); }
		}
	}
}
