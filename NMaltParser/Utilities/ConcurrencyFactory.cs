using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace NMaltParser.Utilities
{
    public class ConcurrencyFactory
    {
        private readonly ConcurrentDictionary<string, object> dictionary = new ConcurrentDictionary<string, object>();

        public T Get<T>(Func<object> func, [CallerMemberName] string methodName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            return (T) dictionary.GetOrAdd($"{methodName}@{filePath}:{lineNumber}", new Lazy<object>(func, true));
        }
    }
}