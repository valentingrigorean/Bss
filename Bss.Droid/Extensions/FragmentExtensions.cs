using Android.Widget;
using Bss.Droid.Utils;

namespace Bss.Droid.Extensions
{
	public static class FragmentExtensions
	{
		public static void SetFont(this Android.Support.V4.App.Fragment This, int fontType, params TextView[] fields)
		{
			var font = FontManager.Get(fontType);
			foreach (var field in fields)
				field.Typeface = font;
		}
	}
}
