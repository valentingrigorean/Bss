//
// IntentExtensions.cs
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
using Android.Content;
using Bss.Core.Extensions;
using Bss.Droid.Extensions.Json;

namespace Bss.Droid.Extensions
{
    public enum DataType
    {
        Binary,
        Json
    }

    public static class IntentExtensions
    {
        public static void SetData(this Intent intent, string key, object data, DataType datatype = DataType.Json)
        {
            intent.PutExtra($"data_type_{key}", (int)datatype);
            if (datatype == DataType.Binary)
                intent.PutExtra(key, data.ToBytes());
            else
                intent.PutExtra(key, data.ToJSON());
        }

        public static T GetData<T>(this Intent intent, string key)
        {
            if (!intent.HasExtra(key))
                return default(T);
            var dataType = (DataType)intent.GetIntExtra($"data_type_{key}", 0);
            return dataType == DataType.Binary
                ? intent.GetByteArrayExtra(key).FromBytes<T>()
                : intent.GetStringExtra(key).FromJSON<T>();
        }

        public static T GetData<T>(this Intent intent, string key, T defaultValue)
        {
            if (!intent.HasExtra(key))
                return defaultValue;
            var dataType = (DataType)intent.GetIntExtra($"data_type_{key}", 0);
            return dataType == DataType.Binary
                ? intent.GetByteArrayExtra(key).FromBytes<T>()
                : intent.GetStringExtra(key).FromJSON<T>();
        }
    }
}