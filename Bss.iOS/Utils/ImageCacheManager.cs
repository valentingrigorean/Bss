//
// CacheManager.cs
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
using System.IO;
using UIKit;
using System.Linq;

namespace Bss.iOS.Utils
{
    public class ImageCacheManager
    {
        private readonly string _dir;

        private static readonly Lazy<ImageCacheManager> _lazyInstance = new
            Lazy<ImageCacheManager>(() => new ImageCacheManager(CacheDir));

        public static ImageCacheManager SharedInstance => _lazyInstance.Value;

        public ImageCacheManager(string dir)
        {
            _dir = dir;
        }

        public static string CacheDir { get; set; } = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments) + "temp_img";

        public const int UnlimitedSize = -1;

        /// <summary>
        /// Gets or sets the size of the cache in MB
        /// If set to -1 will have unlimited size
        /// </summary>
        /// <value>The size of the cache in MB.</value>
        public int CacheSize { get; set; } = 40;

        public delegate void AddImageCallback(string path);

        public string Add(UIImage img, AddImageCallback callback = null)
        {
            CreateDirectoryIfNotExists();
            var uuid = Guid.NewGuid();
            var path = $"{_dir}/{uuid}.png";
            img.SaveToFile(path, () =>
            {
                ValidateSize();
                Application.InvokeOnMainThread(() => callback?.Invoke(path));
            }, ImageExtension.PNG);
            return path;
        }

        public bool Remove(string path)
        {
            if (!File.Exists(path))
                return false;
            File.Delete(path);
            return true;
        }

        public string[] GetAllImagesInCache()
        {
            if (Directory.Exists(_dir))
                return Directory.GetFiles(_dir, "*.png");
            return new string[0];
        }

        public void Clear()
        {
            Clear(_dir);
        }

        private void Clear(string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
        }

        private void ValidateSize()
        {
            while (true)
            {
                if (CacheSize < 0) return;
                if (GetCurrentSizeInMb() <= CacheSize) return;
                var file = LastModifiedItem();
                if (file == null) return;
                File.Delete(file.FullName);
            }
        }

        private FileInfo LastModifiedItem()
        {
            var files = Directory.GetFiles(_dir, "*", SearchOption.AllDirectories).Select(
                _ => new FileInfo(_)).OrderByDescending(_ => _.LastAccessTime).ToArray();
            return files.Length > 0 ? files[0] : null;
        }

        private long GetCurrentSizeInMb()
        {
            return Directory.GetFiles(_dir, "*", SearchOption.AllDirectories).Sum(
                _ => new FileInfo(_).Length) / (1024 * 1024);
        }

        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(_dir))
                Directory.CreateDirectory(_dir);
        }

    }
}

