
using System;

namespace Bss.Core.Extensions
{
	public static class RealNumbersExtensions
	{
		public static bool AreEqual(this float This, float nr, float epsilon = float.Epsilon)
		{
			return Math.Abs(This - nr) < epsilon;
		}

		public static bool AreEqual(this double This, double nr, double epsilon = double.Epsilon)
		{
			return Math.Abs(This - nr) < epsilon;
		}

		public static bool Invert(this bool val) { return !val; }
	}
}
