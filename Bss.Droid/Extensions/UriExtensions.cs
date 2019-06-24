using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Net;

namespace Bss.Droid.Extensions
{
	public static class UriExtensions
	{
        public static bool IsExternalStorageDocument(Uri uri) => "com.android.externalstorage.documents".Equals(uri.Authority);
        public static bool IsDownloadsDocument(Uri uri) => "com.android.providers.downloads.documents".Equals(uri.Authority);
        public static bool IsMediaDocument(Uri uri) => "com.android.providers.media.documents".Equals(uri.Authority);

		public static string GetPathToImage(this Uri uri, Context context)
		{
            bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    if ("primary".Equals(type, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {

                    string id = DocumentsContract.GetDocumentId(uri);
                    Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    return GetDataColumn(context, contentUri, null, null);
                }
                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

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

                    string selection = "_id=?";
                    string[] selectionArgs = {
                        split[1]
                    };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return GetDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return uri.Path;
            }

            return null;

		}

        public static string GetDataColumn(Context context, Uri uri, string selection, string[] selectionArgs)
        {
            string column = "_data";
            string[] projection = {
                column
            };

            try
            {
                var cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                        null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndexOrThrow(column);
                    string rez;
                    rez = cursor.GetString(column_index);
                    return rez;
                }
            }
            catch
            {
                return uri.Path;
            }

            return null;
        }
	}
}
