//
// StringExtensions.cs
//
// Author:
//       valentingrigorean <>
//
// Copyright (c) 2017 ${CopyrightHolder}
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace Bss.Core.Extensions
{
    public enum TitleCase
    {
        First,
        All
    }

    public enum Symbol
    {
        Euro,
        Dolar
    }


    public static class StringExtensions
    {
        /// <summary>
        /// Appends the symbol.
        /// </summary>
        /// <returns>if string string is null is returning the str</returns>
        /// <param name="str">String.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="appendSpace">If set to <c>true</c> append space.</param>
        public static string AppendSymbol(this string str, Symbol symbol, bool appendSpace = true)
        {

            switch (symbol)
            {
                case Symbol.Euro:
                    return AppendSymbol(str, "€", appendSpace);
                case Symbol.Dolar:
                    return AppendSymbol(str, "$", appendSpace);
                default:
                    throw new NotSupportedException();
            }

        }

        public static string AppendSymbol(this string str, string symbol, bool appendSpace = true)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (appendSpace)
                str += " ";
            return str + symbol;
        }

        public static bool IsOnlyNumbers(this string str)
        {
            return Regex.IsMatch(str, "^[0-9]+$");
        }

        public static bool IsOnlyLetters(this string str)
        {
            return Regex.IsMatch(str, @"^[ a-zA-Z]+$");
        }

        public static bool IsOnlyNumbersAndLetters(this string str)
        {
            return Regex.IsMatch(str, "[a-zA-Z0-9]*");
        }

        public static bool IsValidEmail(this string str)
        {
            return Regex.IsMatch(str,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
        }

        public static string ToTitleCase(this string str, TitleCase titleCase = TitleCase.First)
        {
            var toLower = str.ToLowerInvariant();
            if (titleCase == TitleCase.All)
                return ToTitleCaseAll(toLower);
            var arr = toLower.ToCharArray();
            arr[0] = char.ToUpperInvariant(arr[0]);
            return new string(arr);
        }

        public static int GetColorFromHexValue(this string hex)
        {
            var cleanHex = hex.Replace("0x", "").TrimStart('#');

            if (cleanHex.Length == 6)
            {
                //Affix fully opaque alpha hex value of FF (225)
                cleanHex = "FF" + cleanHex;
            }

            int argb;
            if (Int32.TryParse(cleanHex, NumberStyles.HexNumber,
                               CultureInfo.InvariantCulture, out argb))
            {
                return argb;
            }

            //If method hasn't returned a color yet, then there's a problem
            throw new ArgumentException("Invalid Hex value. Hex must be either an ARGB (8 digits) or RGB (6 digits)");

        }

        private static string ToTitleCaseAll(string str)
        {
            var words = str.Split(' ');
            var sb = new StringBuilder();
            foreach (var word in words)
                sb.Append(char.ToUpperInvariant(word[0])).Append(word.Substring(1)).Append(' ');
            return sb.ToString();
        }

        public static bool IsNull(this string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return true;
        }


        #region Builder

        public static string StringBuilder(params string[] list)
        {
            if (list.IsNullOrEmpty()) return "";
            var builder = new StringBuilder();
            foreach (var item in list)
                builder.Append(item);
            return builder.ToString();
        }

        public static string StringBuilder(this IEnumerable<string> self)
        {
            var sb = new StringBuilder();
            foreach (var item in self)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        #endregion
    }
}
