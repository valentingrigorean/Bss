//
// MediaUtils.cs
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
using System;
using Android.Content;
using Android.App;
using Android.Provider;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Graphics;
using Java.IO;
using Bss.Droid.Extensions;
using System.Threading.Tasks;

namespace Bss.Droid.Utils
{
	/// <summary>
	/// Media utils.
	/// credits to https://gist.github.com/Mariovc/f06e70ebe8ca52fbbbe2
	/// </summary>
	public static class MediaUtils
	{
		private const string TempImageName = "tempImage.png";

		public static string PickSourceString = "Pick source:";

		public static Intent CreatePickImageIntent(Context ctx, string tempFile = TempImageName, bool takePhoto = true, bool pickImage = true)
		{
			var intentList = new List<Intent>();

			var file = GetTempFile(ctx, tempFile);

            if (takePhoto)
            {
                var takePhotoIntent = new Intent(MediaStore.ActionImageCapture);
                takePhotoIntent.PutExtra("return-data", true);
                takePhotoIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
                AddIntentsToList(ctx, intentList, takePhotoIntent);
            }

            if (pickImage)
            {
                var pickIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                AddIntentsToList(ctx, intentList, pickIntent);
            }

            var galleryIntent = new Intent();
			galleryIntent.SetType("image/*");
			galleryIntent.SetAction(Intent.ActionGetContent);
			var chooserIntent = Intent.CreateChooser(galleryIntent, PickSourceString);
			chooserIntent.PutExtra(Intent.ExtraInitialIntents, intentList.Cast<IParcelable>().ToArray());
			return chooserIntent;
		}

		public static async Task< string> GetUrlFromResult(Context ctx, Result result, Intent returnedIntent, string tempFile = TempImageName, int maxImageSize = -1)
		{
			if (result != Result.Ok) return null;
			var fileUri = Android.Net.Uri.FromFile(GetTempFile(ctx, tempFile));
			var isCamera = returnedIntent == null ||
				returnedIntent.Data == null ||
				returnedIntent.Data == fileUri;

            if (maxImageSize > 0) // resize
            {
                if (isCamera)
                {
                    return await ResizeImage(ctx, fileUri, tempFile, maxImageSize);
                }
                else
                {
                    //return await ResizeImage(ctx, Android.Net.Uri.Parse(returnedIntent.Data.GetPathToImage(ctx)), tempFile, maxImageSize);
                    return await ResizeImage(ctx, returnedIntent.Data, tempFile, maxImageSize);
                }
            }

            var uri = fileUri;
			return isCamera ? uri.Path : returnedIntent.Data.GetPathToImage(ctx);
		}

        private static async Task<string> ResizeImage(Context ctx, Android.Net.Uri uri, string tempFile, int maxImageSize)
        {
            using (Bitmap selectedImage = BitmapUtils.DecodeSampledBitmapFromUri(ctx, uri, maxImageSize, maxImageSize))
            {
                var file = GetTempFile(ctx, "temp-resized-" + tempFile);
                await selectedImage.SaveAsFileAsync(file.AbsolutePath);
                return Android.Net.Uri.FromFile(file).Path;
            }
        }

        public static Bitmap GetBitmapFromResult(Context ctx, Result result, Intent returnedIntent, string tempFile = TempImageName)
		{
			if (result != Result.Ok) return null;
			var fileUri = Android.Net.Uri.FromFile(GetTempFile(ctx, tempFile));
			var isCamera = returnedIntent == null ||
				returnedIntent.Data == null ||
				returnedIntent.Data == fileUri;
			var uri = isCamera ? fileUri : returnedIntent.Data;
			var fileDescriptor = ctx.ContentResolver.OpenAssetFileDescriptor(uri, "r");
			return BitmapFactory.DecodeFileDescriptor(fileDescriptor.FileDescriptor);
		}

		private static File GetTempFile(Context ctx, string name)
		{
			var file = new File(ctx.ExternalCacheDir, name);
			file.ParentFile.Mkdirs();
			return file;
		}

		private static void AddIntentsToList(Context ctx, IList<Intent> list, Intent intent)
		{
			var resInfos = ctx.PackageManager.QueryIntentActivities(intent, 0);
			foreach (var resInfo in resInfos)
			{
				var packageName = resInfo.ActivityInfo.PackageName;
				var targetedIntent = new Intent(intent);
				targetedIntent.SetPackage(packageName);
				list.Add(targetedIntent);
			}
		}
	}
}
