
using Android.OS;
using Android.Content;
using Android.Net;
using Android.Provider;
using Android.Database;
using Bss.Droid.Extensions;

namespace Bss.Droid.Utils
{
	public static class ImageFilePath
	{
		private static bool IsKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

		public static string GetPath(Context context, string uri)
		{
			return GetPath(context, Uri.Parse(uri));
		}

		public static string GetPath(Context context, Uri uri)
		{
			if (IsKitKat && DocumentsContract.IsDocumentUri(context, uri))
			{

				// ExternalStorageProvider
				if (isExternalStorageDocument(uri))
				{
					var docId = DocumentsContract.GetDocumentId(uri);
					var Split = docId.Split(':');
					var type = Split[0];

					if ("primary".Equals(type, System.StringComparison.InvariantCultureIgnoreCase))
					{
						return Environment.ExternalStorageDirectory + "/" + Split[1];
					}
				}
				// DownloadsProvider
				else if (IsDownloadsDocument(uri))
				{

					var id = DocumentsContract.GetDocumentId(uri);
					var contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/public_downloads"),
																long.Parse(id));
					return GetDataColumn(context, contentUri, null, null);
				}
				if (IsMediaDocument(uri))
				{

					var docId = DocumentsContract.GetDocumentId(uri);
					var split = docId.Split(':');
					var type = split[0];

					Uri contentUri = null;
					if ("image".Equals(type))
					{
						contentUri = MediaStore.Images.Media.ExternalContentUri;
					}
					else if ("video".Equals(type))
					{
						contentUri = MediaStore.Video.Media.ExternalContentUri;
					}
					else if ("audio".Equals(type))
					{
						contentUri = MediaStore.Audio.Media.ExternalContentUri;
					}

					var selection = "_id=?";
					var selectionArgs = new[] { split[1] };

					return GetDataColumn(context, contentUri, selection, selectionArgs);
				}
			}

			if ("content".Equals(uri.Scheme, System.StringComparison.InvariantCultureIgnoreCase))
			{

				// Return the remote address
				if (IsGooglePhotosUri(uri))
					return uri.LastPathSegment;

				return GetDataColumn(context, uri, null, null);
			}

			if ("file".Equals(uri.Scheme, System.StringComparison.InvariantCultureIgnoreCase))
				return uri.Path;
			return uri.GetPathToImage(context);
		}


		public static string GetDataColumn(Context context, Uri uri, string selection, string[] selectionArgs)
		{

			ICursor cursor = null;
			var column = "_data";
			var projection = new[] { column };

			try
			{
				cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
				if (cursor != null && cursor.MoveToFirst())
				{
					int index = cursor.GetColumnIndexOrThrow(column);
					return cursor.GetString(index);
				}
			}
			finally
			{
				if (cursor != null)
					cursor.Close();
			}
			return null;
		}

		public static bool isExternalStorageDocument(Uri uri)
		{
			return "com.android.externalstorage.documents".Equals(uri.Authority);
		}

		public static bool IsDownloadsDocument(Uri uri)
		{
			return "com.android.providers.downloads.documents".Equals(uri.Authority);
		}


		public static bool IsMediaDocument(Uri uri)
		{
			return "com.android.providers.media.documents".Equals(uri.Authority);
		}

		public static bool IsGooglePhotosUri(Uri uri)
		{
			return "com.google.android.apps.photos.content".Equals(uri.Authority);
		}
	}
}
