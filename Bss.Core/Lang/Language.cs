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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Reflection;

namespace Bss.Core.Lang
{
    [Serializable]
    public class Language
    {
        public enum CreateType
        {
            Custom,
            Resource,
            File,
            Url
        }

        private readonly IDictionary<string, string> _mapStrings = new Dictionary<string, string>();

        private Language(Locale locale, CreateType createType, IDictionary<string, string> map)
        {
            Locale = locale;
            Type = createType;
            if (map != null)
                _mapStrings = map;
        }

        public CreateType Type { get; }

        public Locale Locale { get; }

        public string this[string key]
        {
            get
            {
                string val;
                if (_mapStrings.TryGetValue(key, out val))
                    return val;
                Debug.Print($"Failed to get translation for {key} returning empty string!!!");
                return "";
            }
        }

        public static Language Create(Locale locale)
        {
            if (locale == null)
                throw new ArgumentNullException(nameof(locale));
            return new Language(locale, CreateType.Custom, null);
        }

        public static Language FromFile(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"There is no file at {path}");
            return DeserializeBinary<Language>(File.ReadAllBytes(path));
        }

        public static Language FromFile(Locale locale, string path, IParser parser = null)
        {
            if (locale == null)
                throw new ArgumentNullException(nameof(locale));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            var txt = File.ReadAllText(path);
            return Create(locale, parser, CreateType.File, txt);
        }

        public static Language FromResource(Locale locale, IParser parser = null, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            if (locale == null)
                throw new ArgumentNullException(nameof(locale));
            var allResources = Locale.GetAllLanguageEmbedded(assembly);

            if (allResources.All(_ => _ != locale))
                throw new Exception($"No embedded resource found in assembly for {locale.ShortName}");

            var ln = $".{locale.ShortName.ToLowerInvariant()}.";
            var allFiles = ResourceLoader.GetEmbeddedResourceNameWithFilter(assembly, _ =>
             {
                 var lower = _.ToLowerInvariant();
                 return lower.Contains(ln) && lower.EndsWith(".trans", StringComparison.CurrentCulture);
             });
            if (allFiles.Length == 0)
                throw new Exception($"Failed to load embedded resources for {locale.ShortName}");

            var sb = new StringBuilder();
            foreach (var file in allFiles)
                sb.AppendLine(ResourceLoader.GetEmbeddedResourceString(assembly, file));
            return Create(locale, parser, CreateType.Resource, sb.ToString());
        }

        public static Language FromUrl(Locale locale, string url, IParser parser = null)
        {
            if (locale == null)
                throw new ArgumentNullException(nameof(locale));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            var http = new HttpClient();
            var txt = http.GetStringAsync(url).Result;
            return Create(locale, parser, CreateType.Url, txt);
        }

        public void AddOrUpdate(string key, string val)
        {
            if (_mapStrings.ContainsKey(key))
            {
                _mapStrings[key] = val;
                return;
            }
            _mapStrings.Add(key, val);
        }

        public void Remove(string key)
        {
            _mapStrings.Remove(key);
        }

        public void SaveToFile(string path)
        {
            File.WriteAllBytes(path, SerializeBinary(this));
        }

        private static Language Create(Locale locale, IParser parser, CreateType createType, string txt)
        {
            parser = parser ?? new Parser(locale);
            return new Language(locale, createType, parser.Parse(txt));
        }

        private static byte[] SerializeBinary<T>(T obj)
        {
            byte[] arrayData;
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                arrayData = ms.ToArray();
            }
            return arrayData;
        }

        private static T DeserializeBinary<T>(byte[] arrayData)
        {
            T obj;
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                ms.Write(arrayData, 0, arrayData.Length);
                ms.Seek(0, SeekOrigin.Begin);
                obj = (T)formatter.Deserialize(ms);
            }
            return obj;
        }
    }
}