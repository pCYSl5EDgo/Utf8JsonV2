using System;
using System.Text;

namespace Utf8Json.Test
{
    public static class ByteHelper
    {
        public static string ToArrayString(this byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException();
            }

            var builder = new StringBuilder((6 * bytes.Length) + 13).Append("new byte[] { ");
            foreach (var b in bytes)
            {
                builder.Append("0x").Append(b.ToString("X2")).Append(", ");
            }

            builder.Append("}");
            return builder.ToString();
        }
    }
}