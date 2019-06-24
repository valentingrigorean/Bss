//
// BitmapExtensions.cs
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
using Android.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace Bss.Droid.Extensions
{
	public static class BitmapExtensions
	{
		public static void SaveAsFile(this Bitmap bmp, string path)
		{
			var dir = System.IO.Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			using (var stream = new FileStream(path, FileMode.Create))
			{
				bmp.Compress(Bitmap.CompressFormat.Png, 100, stream);
			}
		}

		public static byte[] ToByteArray(this Bitmap bmp)
		{
			using (var ms = new MemoryStream())
			{
				bmp.Compress(Bitmap.CompressFormat.Png, 100, ms);
				return ms.ToArray();
			}
		}

		public static async Task SaveAsFileAsync(this Bitmap bmp, string path)
		{
            if (bmp == null) return;

			var dir = System.IO.Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			using (var stream = new FileStream(path, FileMode.Create))
			{
				await bmp.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
			}
		}

		public static Bitmap Resize(this Bitmap bmp, int reqWidth, int reqHeight)
		{
			return Bitmap.CreateScaledBitmap(bmp, reqWidth, reqHeight, true);
		}

		public static Bitmap ScaleToFit(this Bitmap bmp, int reqWidth, int reqHeight)
		{
			var output = Bitmap.CreateBitmap(reqWidth, reqHeight, Bitmap.Config.Argb8888);
			var originalWidth = bmp.Width;
			var originalHeight = bmp.Height;

			var canvas = new Canvas(output);

			float scale = reqWidth / originalWidth;

			var xTranslation = 0.0f;
			var yTranslation = (reqHeight - originalHeight * scale) / 2.0f;

			var transformation = new Matrix();
			transformation.PostTranslate(xTranslation, yTranslation);
			transformation.PreScale(scale, scale);

			var paint = new Paint();
			paint.FilterBitmap = true;

			canvas.DrawBitmap(output, transformation, paint);

			return output;
		}
	}
}
