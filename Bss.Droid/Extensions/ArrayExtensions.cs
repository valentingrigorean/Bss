//
//  ArrayExtensions.cs
//
//  Author:
//       valen <valentin.grigorean1@gmail.com>
//
//  Copyright (c) 2016 (c) Grigorean Valentin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System.Collections.Generic;
using Java.Util;

namespace Bss.Droid.Extensions
{
    public static class ArrayExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
                list.Add(item);
        }

        public static IIterator GetIterator<T>(this IEnumerable<T> list)
                       where T : Java.Lang.Object
        {
            return new EnumerableIterator<T>(list);
        }

        public static Java.Lang.IIterable GetIterable<T>(this IEnumerable<T> list)
                           where T : Java.Lang.Object
        {
            return new EnumerableIterable<T>(list);
        }       

        private class EnumerableIterable<T> : Java.Lang.Object, Java.Lang.IIterable
         where T : Java.Lang.Object
        {
            private readonly EnumerableIterator<T> _iterator;

            public EnumerableIterable(IEnumerable<T> list)
            {
                _iterator = new EnumerableIterator<T>(list);
            }

            public IIterator Iterator()
            {
                return _iterator;
            }
        }


        private class EnumerableIterator<T> : Java.Lang.Object, IIterator
            where T : Java.Lang.Object
        {
            public EnumerableIterator(IEnumerable<T> list)
            {
                Enumerator = list.GetEnumerator();
            }

            public IEnumerator<T> Enumerator { get; }

            public bool HasNext => Enumerator.MoveNext();


            public Java.Lang.Object Next()
            {
                return Enumerator.Current;
            }

            public void Remove()
            {

            }
        }
    }
}