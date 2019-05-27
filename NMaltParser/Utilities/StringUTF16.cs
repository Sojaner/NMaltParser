using System;
using System.Linq;

namespace NMaltParser.Utilities
{
    internal static class StringUtf16
    {
        public static int HashCode(string value)
        {
            char FixOrder(char input)
            {
                byte[] bytes = BitConverter.GetBytes(input);

                Array.Reverse(bytes);

                return BitConverter.ToChar(bytes, 0);
            }

            return value.Select(c => BitConverter.IsLittleEndian ? c : FixOrder(c)).Aggregate(0, (current, v) => 31 * current + v);
        }
    }
}