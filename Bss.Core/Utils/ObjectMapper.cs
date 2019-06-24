using System;
using System.Collections.Generic;

namespace Bss.Core.Utils
{
    public static class ObjectMapper
    {
        private readonly static IDictionary<Type, IDictionary<Type, Func<object, object>>> _mapper = new Dictionary<Type, IDictionary<Type, Func<object, object>>>();


        public static bool ContainsMap<TSource, TDestination>()
        {
            var src = typeof(TSource);
            var dest = typeof(TDestination);
            return _mapper.ContainsKey(src) && _mapper[src].ContainsKey(dest);
        }

        public static void CreateMap<TSource, TDestination>(Func<TSource, TDestination> createFunc)
        {
            var src = typeof(TSource);
            var dest = typeof(TDestination);

            if (!_mapper.ContainsKey(src))
                _mapper.Add(src, new Dictionary<Type, Func<object, object>>());
            var destDict = _mapper[src];
            if (destDict.ContainsKey(dest))
                throw new Exception("Already contains a function for" +
                                    $" {src} to ${dest}");

            destDict[dest] = (arg) => createFunc((TSource)arg);
        }

        public static TDestination Map<TDestination>(object obj)
        {
            var src = obj.GetType();
            var dest = typeof(TDestination);
            if (!_mapper.ContainsKey(src))
                throw new Exception($"{src} doesnt have any convertion register");

            var destDist = _mapper[src];
            if (!destDist.ContainsKey(dest))
                throw new Exception($"{dest} doesnt have any convertion register to {src}");

            var func = destDist[dest];
            return (TDestination)func(obj);
        }
    }
}
