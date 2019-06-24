using Bss.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Bss.Core.Extensions
{
    public static class ObjectMapperExtensions
    {
        public static T Map<T>(this object This)
        {
            return ObjectMapper.Map<T>(This);
        }

        public static IEnumerable<T> MapCollection<T>(this IEnumerable<object> This)
        {
            return This.Select(item => item.Map<T>());
        }
    }
}
