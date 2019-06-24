using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Bss.Droid.Widgets
{
	public class LockableScrollView : ScrollView
	{
		public LockableScrollView(Context context) :
			base(context)
		{

		}

		public LockableScrollView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{

		}

		public LockableScrollView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{

		}

		public bool IsScrollEnabled { get; set; } = true;

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (e.ActionMasked == MotionEventActions.Down)
			{
				if (IsScrollEnabled)
					return base.OnTouchEvent(e);
				return IsScrollEnabled;
			}
			return base.OnTouchEvent(e);
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			if (!IsScrollEnabled) return false;
			return base.OnInterceptTouchEvent(ev);
		}

	}
}
