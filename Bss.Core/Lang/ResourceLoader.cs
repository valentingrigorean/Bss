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
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bss.Core.Lang
{
    public static class ResourceLoader
    {
        /// <summary>
        ///     Attempts to find and return the given resource from within the calling assembly.
        /// </summary>
        /// <returns>The embedded resource as a stream.</returns>
        /// <param name="resourceFileName">Resource file name.</param>
        public static Stream GetEmbeddedResourceStream(string resourceFileName)
        {
            return GetEmbeddedResourceStream(Assembly.GetCallingAssembly(), resourceFileName);
        }

        /// <summary>
        ///     Attempts to find and return the given resource from within the calling assembly.
        /// </summary>
        /// <returns>The embedded resource as a byte array.</returns>
        /// <param name="resourceFileName">Resource file name.</param>
        public static byte[] GetEmbeddedResourceBytes(string resourceFileName)
        {
            return GetEmbeddedResourceBytes(Assembly.GetCallingAssembly(), resourceFileName);
        }

        /// <summary>
        ///     Attempts to find and return the given resource from within the calling assembly.
        /// </summary>
        /// <returns>The embedded resource as a string.</returns>
        /// <param name="resourceFileName">Resource file name.</param>
        public static string GetEmbeddedResourceString(string resourceFileName)
        {
            return GetEmbeddedResourceString(Assembly.GetCallingAssembly(), resourceFileName);
        }

        /// <summary>
        ///     Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource stream.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
        {
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames
                .Where(x => x.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            if (!resourcePaths.Any())
                throw new Exception($"Resource ending with {resourceFileName} not found.");

            if (resourcePaths.Count() > 1)
                throw new Exception(
                    $"Multiple resources ending with {resourceFileName} found: {Environment.NewLine}{string.Join(Environment.NewLine, resourcePaths)}");

            return assembly.GetManifestResourceStream(resourcePaths.Single());
        }

        /// <summary>
        ///     Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a byte array.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static byte[] GetEmbeddedResourceBytes(Assembly assembly, string resourceFileName)
        {
            var stream = GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        ///     Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a string.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static string GetEmbeddedResourceString(Assembly assembly, string resourceFileName)
        {
            var stream = GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static string[] GetEmbeddedResourceNameWithFilter(Func<string, bool> filter)
        {
            return GetEmbeddedResourceNameWithFilter(Assembly.GetCallingAssembly(), filter);
        }       

        public static string[] GetEmbeddedResourceNameWithFilter(Assembly assembly, Func<string, bool> filter)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            return resourceNames.Where(filter).ToArray();
        }
    }
}