using System;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Bss.Core.Extensions
{
    public static class JsonExtensions
    {
        public static T FromJSON<T>(this string str, T defaultValue = default(T))
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse :{str}\n{ex}");

                return defaultValue;
            }
        }

        public static T FromJSONAnonymousType<T>(this string self, T definition)
        {
            try
            {
                return JsonConvert.DeserializeAnonymousType(self, definition);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse :{self}\n{ex}");

                return definition;
            }
        }

        public static T GetJSON<T>(this HttpResponseMessage resp, T defaultValue = default(T))
        {
            try
            {
                var json = resp.Content.ReadAsStringAsync().Result;
                return json.FromJSON<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        public static async Task<T> GetJSONAsync<T>(this HttpResponseMessage resp, T defaultValue = default(T))
        {
            try
            {
                var json = await resp.Content.ReadAsStringAsync();
                return json.FromJSON<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        public static IDictionary<string, string> ToStringDictionary<T>(this T This, params string[] ignoreMembers)
            where T : class
        {
            var dict = ToJSON(This).FromJSON<IDictionary<string, string>>(new Dictionary<string, string>());

            foreach (var ignoreMember in ignoreMembers)
                dict.Remove(ignoreMember);

            return dict;
        }

        public static string ToJSON<T>(this T obj)
        where T : class
        {
            if (obj == null)
                return "";

            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
