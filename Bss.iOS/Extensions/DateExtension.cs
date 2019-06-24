using System;

namespace Foundation
{
	public static class DateExtension
	{
		private static readonly DateTime NsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

		/// <summary>Returns the seconds interval for a DateTime from NSDate reference data of January 1, 2001</summary>
		/// <param name="dt">The DateTime to evaluate</param>
		/// <returns>The seconds since NSDate reference date</returns>
		public static double SecondsSinceNsRefenceDate(this DateTime dt)
		{
			return (dt - NsRef).TotalSeconds;
		}       

		/// <summary>Convert a DateTime to NSDate</summary>
		/// <param name="dt">The DateTime to convert</param>
		/// <returns>An NSDate</returns>
		public static NSDate ToNsDate(this DateTime dt)
		{
            if (dt.Kind == DateTimeKind.Unspecified)
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            return (NSDate)dt;
		}

		/// <summary>Convert an NSDate to DateTime</summary>
		/// <param name="nsDate">The NSDate to convert</param>
		/// <returns>A DateTime</returns>
		public static DateTime ToDateTime(this NSDate nsDate)
		{
            return ((DateTime)nsDate).ToLocalTime();
		}
	}
}