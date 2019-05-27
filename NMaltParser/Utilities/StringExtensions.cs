using System.Collections.Generic;

namespace NMaltParser.Utilities
{
    internal static class StringExtensions
    {
        private static readonly Dictionary<string,int> dictionary = new Dictionary<string, int>();

        public static int HashCode(this string s)
        {
            if (!dictionary.ContainsKey(s))
            {
                int hash = s.IsLatin() ? StringLatin1.HashCode(s) : StringUtf16.HashCode(s);

                dictionary.Add(s, hash);

                return hash;
            }
            else
            {
                return dictionary[s];
            }
        }
    }
}