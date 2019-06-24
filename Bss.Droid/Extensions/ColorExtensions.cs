using Android.Graphics;

namespace Bss.Droid.Extensions
{
	public static class ColorExtensions
	{
		public static Color WithAlpha(this Color This, float alpha)
		{
			var newColor = new Color(This.ToArgb());
			newColor.A = (byte)(alpha * 255);
			return newColor;
		}

		public static Color WithAlpha(this Color This, int alpha)
		{
			var newColor = new Color(This.ToArgb());
			newColor.A = (byte)alpha;
			return newColor;
		}

		public static Color WithAlpha(this Color This, byte alpha)
		{
			var newColor = new Color(This.ToArgb());
			newColor.A = alpha;
			return newColor;
		}
	}
}
