using System.Linq;
using System.Text;

namespace NMaltParser.Utilities
{
    internal static class StringLatin1
    {
        public static bool IsLatin(this string s)
        {
            return s.All(b => ((int)(((uint)b) >> 8)) == 0);
        }

        private static readonly Encoding encoding = Encoding.GetEncoding("Latin1");

        public static int HashCode(string value)
        {
            return encoding.GetBytes(value).Aggregate(0, (current, v) => 31 * current + (v & 0xff));
        }
    }
}
