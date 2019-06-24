using System.Globalization;

namespace Bss.Core.Extensions.Number
{
    public static class NumberExtensions
    {
        public static double Double(this string text)
        {
            double val = 0;
            double.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out val);
            return val;
        }

        public static double Double(this string text, NumberStyles style)
        {
            double val = 0;
            double.TryParse(text, style, CultureInfo.InvariantCulture, out val);
            return val;
        }

        public static string String(this double nr)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ",";
            var text = nr.ToString(nfi);
            return text;
        }



    }
}
