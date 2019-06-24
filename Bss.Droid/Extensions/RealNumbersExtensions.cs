using System;

namespace Bss.Droid.Extensions
{
	internal static class RealNumbersExtensions
	{
		public static bool AreEqual(this float This, float nr, float epsilon = float.Epsilon)
		{
			return Math.Abs(This - nr) < epsilon;
		}

		public static bool AreEqual(this double This, double nr, double epsilon = double.Epsilon)
		{
			return Math.Abs(This - nr) < epsilon;
		}
	}
}
