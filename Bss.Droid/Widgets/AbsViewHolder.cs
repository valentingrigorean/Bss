using Android.Support.V7.Widget;
using Android.Views;

namespace Bss.Droid.Widgets
{

	public abstract class AbsViewHolder : RecyclerView.ViewHolder
	{
		protected AbsViewHolder(View view) : base(view)
		{
			Initialize();
		}

		public abstract void Bind(object model);

		public virtual void OnCreate() { }

		private void Initialize()
		{
			Cheeseknife.Cheeseknife.Inject(this, ItemView);
			OnCreate();
		}
	}


	public abstract class AbsViewHolder<T> : RecyclerView.ViewHolder
	{
		protected AbsViewHolder(View view) : base(view)
		{
			Initialize();
		}

		public abstract void Bind(T model);

		public virtual void OnCreate() { }

		private void Initialize()
		{
			Cheeseknife.Cheeseknife.Inject(this, ItemView);
			OnCreate();
		}
	}

}
