using System;
using System.Linq;
using System.Collections.Generic;

namespace Bss.Core.Utils
{
    public static class RandomUtils
    {
        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public static T GetRandomValue<T>(this IList<T> This, int startIndex = 0)
        {
            var min = Math.Min(_random.Next(startIndex, This.Count - 1), This.Count - 1);
            return This[min];
        }

        public static T GetRandomValueFromEnum<T>(this Type This, int startIndex = 0)
        {
            var array = Enum.GetValues(This).Cast<T>().ToArray();
            return array.GetRandomValue(startIndex);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static double NextDouble()
        {
            return _random.NextDouble();
        }

        public static bool NextBoolean()
        {
            return _random.NextDouble() < 0.5d;
        }

        public static DateTime NextDate()
        {
            return DateTime.Now.AddDays(_random.Next(0, 720));
        }

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static int Next()
        {
            return _random.Next();
        }

        public static int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public static IList<string> RandomImages(int count)
        {
            var list = new List<string>();
            for (var i = 0; i < count; i++)
                list.Add($"https://picsum.photos/200/300/?image={i}");
            return list;
        }
    }
}
