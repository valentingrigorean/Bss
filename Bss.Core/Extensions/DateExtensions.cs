using System;
using System.Globalization;

namespace Bss.Core.Extensions
{
    public enum DateTimePrecision
    {
        Day, Hour, Minute, Second
    }

    public static class DateExtensions
    {
        public static DateTime? ToDate(this string date, string format)
        {
            if (DateTime.TryParseExact(date, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
                return dateTime;
            return null;
        }

        public static int GetAge(this DateTime? date)
        {
            if (!date.HasValue) return 0;
            var today = DateTime.Today;
            var age = today.Year - date.GetValueOrDefault().Year;
            if (date > today.AddYears(-age))
                age--;
            return age;
        }

        public static long ToMilliseconds(this DateTime date)
        {
            var timeSpan = (date - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
            //long epoch = date.Ticks;
            //return epoch;
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime? TrimDate(this DateTime? This, DateTimePrecision precision)
        {
            if (This.HasValue)
                return TrimDate(This.Value, precision);
            return This;
        }

        public static DateTime TrimDate(this DateTime date, DateTimePrecision precision)
        {
            switch (precision)
            {
                case DateTimePrecision.Day:
                    return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                case DateTimePrecision.Hour:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
                case DateTimePrecision.Minute:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
                case DateTimePrecision.Second:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }
            return date;
        }

        public static long ToUnixTimeSeconds(this DateTime This)
        {
            var seconds = (long)(This.ToUniversalTime() - UnixEpoch).TotalSeconds;
            return seconds;
        }

        public static long ToUnixTimeMileseconds(this DateTime This)
        {
            return ToUnixTimeSeconds(This) * 1000;
        }

        public static DateTime UnixTimeSecondsToDate(this long This)
        {
            var timeSpan = TimeSpan.FromSeconds(This);
            var date = UnixEpoch.Add(timeSpan).ToLocalTime();
            return date;
        }

        public static DateTime UnixTimeSecondsToDate(this int This)
        {
            var timeSpan = TimeSpan.FromSeconds(This);
            var date = UnixEpoch.Add(timeSpan).ToLocalTime();
            return date;
        }

        public static DateTime UnixTimeMilesecondsToDate(this long This)
        {
            var timeSpan = TimeSpan.FromMilliseconds(This);
            var date = UnixEpoch.Add(timeSpan).ToLocalTime();
            return date;
        }

        public static DateTime SpecifyKindIfNeeded(this DateTime This, DateTimeKind kind = DateTimeKind.Local)
        {
            if (This.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(This, kind);
            return This;
        }
    }
}