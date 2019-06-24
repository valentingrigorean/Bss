using System.Collections;
using System.Collections.Generic;

namespace Bss.Core.Collections
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _items;

        public ReadOnlyList():this(new List<T>())
        {
           
        }

        public ReadOnlyList(IList<T> items)
        {
            _items = items;
        }

        public T this[int index] => _items[index];

        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
