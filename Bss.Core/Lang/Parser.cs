//
// Lang.cs
//
// Author:
//       Valentin Grigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 
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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bss.Core.Lang
{
    internal class Parser : IParser
	{
		private static readonly Regex Validator = new Regex("(\"[a-zA-Z_0-9].*\"[\\s]*=[\\s]*\".*\";)",
			RegexOptions.Compiled);

		private readonly Dictionary<string, string> MapStrings = new Dictionary<string, string>();

		public Parser(Locale locale)
		{
			Language = locale;
		}

		private Locale Language { get; }

		public IDictionary<string, string> Parse(string txt)
		{
			var lines = txt.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i].Trim();
				if (string.IsNullOrEmpty(line) || line.StartsWith("#", StringComparison.CurrentCulture)) continue;
				if (Validator.Match(line).Length == 0)
					throw new Exception($"Invalid format found at line {i} expected:\n\t \"key\" = \"value\";" +
										$"\nFound:\n\t{line}");
				KeyValuePair<string, string> keyValue;
				if (!TryGetValue(line, out keyValue))
					throw new Exception($"Invalid format found at line {i} expected:\n\t \"key\" = \"value\";" +
										$"\nFound:\n\t{line}");

				if (MapStrings.ContainsKey(keyValue.Key))
				{
					Console.WriteLine($"Warning: found duplicate key {keyValue.Key} " +
									  $" in {Language.ShortName}");
					continue;
				}
				MapStrings.Add(keyValue.Key, keyValue.Value);
			}
			return MapStrings;
		}

		private static bool TryGetValue(string str, out KeyValuePair<string, string> keyValue)
		{
            var place = str.LastIndexOf(";");

            if(place > -1)
                str = str.Remove(place, 1);

			var index = str.IndexOf("=", StringComparison.InvariantCulture);

			if (index < 3 || index > str.Length - 3)
			{
				keyValue = new KeyValuePair<string, string>();
				return false;
			}
			var key = str.Substring(0, index).Trim();
			key = key.Substring(1, key.Length - 2);
			var value = str.Substring(index + 1, str.Length - index - 1).Trim();
			value = value.Substring(1, value.Length - 2);

			keyValue = new KeyValuePair<string, string>(key, value);
			return true;
		}
	}

}