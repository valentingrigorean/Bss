using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bss.Droid.Extensions.Json
{
    internal class JavaHolder : Java.Lang.Object
    {
		public readonly object Instance;

		public JavaHolder(object instance)
		{
			Instance = instance;
		}
    }

	public static class ObjectExtensions
	{
		public static TObject ToNetObject<TObject>(this Java.Lang.Object This)
		{
			if (This == null)
				return default(TObject);

			if (!(This is JavaHolder))
				throw new InvalidOperationException("Unable to convert to .NET object. Only Java.Lang.Object created with .ToJavaObject() can be converted.");

			return (TObject)((JavaHolder)This).Instance;
		}

		public static Java.Lang.Object ToJavaObject<T>(this T This)
		{
#pragma warning disable RECS0017 // Possible compare of value type with 'null'
            if (This == null)
#pragma warning restore RECS0017 // Possible compare of value type with 'null'
                return null;

			var holder = new JavaHolder(This);

			return holder;
		}

		public static byte[] ToBytes(this object obj)
		{
			var formatter = new BinaryFormatter();

			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, obj);
				stream.Position = 0;
				return stream.GetBuffer();
			}
		}

		public static T FromBytes<T>(this byte[] data)
		{
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream(data))
			{
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}

		}
	}
}