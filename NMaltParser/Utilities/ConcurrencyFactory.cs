using System;
using System.Collections.Concurrent;

namespace NMaltParser.Utilities
{
    public class ConcurrencyFactory
    {
        private readonly ConcurrentDictionary<string, object> dictionary = new ConcurrentDictionary<string, object>();

        public T Get<T>(string name, Func<object> func)
        {
            return (T) dictionary.GetOrAdd(name, new Lazy<object>(func, true));
        }
    }
}