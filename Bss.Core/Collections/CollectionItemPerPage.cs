//
// CollectionItemPerPage.cs
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
using System.Collections.Generic;

namespace Bss.Core.Collections
{
    public delegate void CountChangeEventHandler(int count);

    public class CollectionItemPerPage<T> : Collection<T>
    {
        private int _itemsPerPage;
        private int _pages;

        public CollectionItemPerPage()
        {
        }

        public CollectionItemPerPage(IList<T> list)
            : base(list)
        {

        }


        public virtual event CountChangeEventHandler CountChange;

        public override void Add(T item)
        {
            base.Add(item);
            RecaculateItemPerPage();
        }

        public override bool Remove(T item)
        {
            var flag = base.Remove(item);
            if (flag)
                RecaculateItemPerPage();
            return flag;
        }

        public virtual int ItemPerPage
        {
            get
            {
                return _itemsPerPage;
            }
            set
            {
                _itemsPerPage = value;
                RecaculateItemPerPage();
            }
        }

        public virtual int Pages => _pages;

        public IList<T> GetItems(int page)
        {
            var index = page * ItemPerPage;
            IList<T> items = new List<T>();
            for (var i = 0; index < Count && i < ItemPerPage; i++, index++)
            {
                items.Add(this[index]);
            }
            return items;
        }

        private void RecaculateItemPerPage()
        {
            if (_itemsPerPage == 0)
                return;
            var rest = Count % _itemsPerPage;
            var cat = Count / _itemsPerPage;
            var pages = Count <= _itemsPerPage ? 1 :
                rest == 0 ? cat :
                cat + 1;
            if (_pages == 0)
            {
                _pages = pages;
                CountChange?.Invoke(pages);
                return;
            }
            if (pages == _pages) return;
            _pages = pages;
            CountChange?.Invoke(pages);
        }

        public override string ToString()
        {
            return
                $"[CollectionItemPerPage: EmitEvents={EmitEvents}, Count={Count}, IsReadOnly={IsReadOnly} ItemPerPage={ItemPerPage}," +
                $" Pages={Pages}]";
        }

    }
}
