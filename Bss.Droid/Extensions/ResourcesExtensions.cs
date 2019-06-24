using Android.Support.V4.Content;
using Android.Content;

namespace Bss.Droid.Extensions
{
	public static class ResourcesExtensions
	{
        public static Android.Graphics.Drawables.Drawable GetDrawableX(this Context This, int resource)
		{
            return ContextCompat.GetDrawable(This, resource);
		}
	}
}
