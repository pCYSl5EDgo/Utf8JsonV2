// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
#if CSHARP_8_OR_NEWER
        public static unsafe byte[] Serialize(string? value)
#else
        public static unsafe byte[] Serialize(string value)
#endif
        {
            if (value is null)
            {
                return new[] { (byte)'n', (byte)'u', (byte)'l', (byte)'l' };
            }

            if (value.Length == 0)
            {
                return new[] { (byte)'"', (byte)'"' };
            }

            var array = ArrayPool<byte>.Shared.Rent(value.Length * 3);
            var span = array.AsSpan();
            var input = value.AsSpan();
            try
            {
                var actualLength = 0;
                for (var index = 0; index < input.Length;)
                {
                    byte escapedChar;
                    switch (input[index])
                    {
                        case '"':
                            escapedChar = (byte)'"';
                            break;
                        case '\\':
                            escapedChar = (byte)'\\';
                            break;
                        case '\b':
                            escapedChar = (byte)'b';
                            break;
                        case '\f':
                            escapedChar = (byte)'f';
                            break;
                        case '\n':
                            escapedChar = (byte)'n';
                            break;
                        case '\r':
                            escapedChar = (byte)'r';
                            break;
                        case '\t':
                            escapedChar = (byte)'t';
                            break;
                        #region Other
                        case (char)0:
                        case (char)1:
                        case (char)2:
                        case (char)3:
                        case (char)4:
                        case (char)5:
                        case (char)6:
                        case (char)7:
                        case (char)11:
                        case (char)14:
                        case (char)15:
                        case (char)16:
                        case (char)17:
                        case (char)18:
                        case (char)19:
                        case (char)20:
                        case (char)21:
                        case (char)22:
                        case (char)23:
                        case (char)24:
                        case (char)25:
                        case (char)26:
                        case (char)27:
                        case (char)28:
                        case (char)29:
                        case (char)30:
                        case (char)31:
                        case (char)32:
                        case (char)33:
                        case (char)35:
                        case (char)36:
                        case (char)37:
                        case (char)38:
                        case (char)39:
                        case (char)40:
                        case (char)41:
                        case (char)42:
                        case (char)43:
                        case (char)44:
                        case (char)45:
                        case (char)46:
                        case (char)47:
                        case (char)48:
                        case (char)49:
                        case (char)50:
                        case (char)51:
                        case (char)52:
                        case (char)53:
                        case (char)54:
                        case (char)55:
                        case (char)56:
                        case (char)57:
                        case (char)58:
                        case (char)59:
                        case (char)60:
                        case (char)61:
                        case (char)62:
                        case (char)63:
                        case (char)64:
                        case (char)65:
                        case (char)66:
                        case (char)67:
                        case (char)68:
                        case (char)69:
                        case (char)70:
                        case (char)71:
                        case (char)72:
                        case (char)73:
                        case (char)74:
                        case (char)75:
                        case (char)76:
                        case (char)77:
                        case (char)78:
                        case (char)79:
                        case (char)80:
                        case (char)81:
                        case (char)82:
                        case (char)83:
                        case (char)84:
                        case (char)85:
                        case (char)86:
                        case (char)87:
                        case (char)88:
                        case (char)89:
                        case (char)90:
                        case (char)91:
                        default:
                            #endregion
                            index++;
                            continue;
                    }

                    if (index != 0)
                    {
#if SPAN_BUILTIN
                        var consumed = StringEncoding.Utf8.GetBytes(input.Slice(0, index), span);
#else
                        int consumed;
                        fixed (char* src = &input[0])
                        fixed (byte* dst = &span[0])
                        {
                            consumed = StringEncoding.Utf8.GetBytes(src, index, dst, span.Length);
                        }
#endif
                        actualLength += consumed;
                        span = span.Slice(consumed);
                    }

                    span[0] = (byte)'\\';
                    span[1] = escapedChar;
                    span = span.Slice(2);
                    input = input.Slice(index + 1);
                    index = 0;
                    actualLength += 2;
                }

                if (!input.IsEmpty)
                {
#if SPAN_BUILTIN
                    var consumed = StringEncoding.Utf8.GetBytes(input, span);
#else
                    int consumed;
                    fixed (char* src = &input[0])
                    fixed (byte* dst = &span[0])
                    {
                        consumed = StringEncoding.Utf8.GetBytes(src, input.Length, dst, span.Length);
                    }
#endif
                    actualLength += consumed;
                }

                var answer = new byte[actualLength + 2];
                answer[0] = answer[actualLength + 1] = (byte)'"';
                fixed (byte* dst = &answer[1])
                fixed (byte* src = &array[0])
                {
                    Buffer.MemoryCopy(src, dst, actualLength, actualLength);
                }

                return answer;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static unsafe byte[] Serialize(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return new[] { (byte)'"', (byte)'"' };
            }

            var array = ArrayPool<byte>.Shared.Rent(value.Length * 3);
            var span = array.AsSpan();
            try
            {
                var actualLength = 0;
                for (var index = 0; index < value.Length;)
                {
                    byte escapedChar;
                    switch (value[index])
                    {
                        case '"':
                            escapedChar = (byte)'"';
                            break;
                        case '\\':
                            escapedChar = (byte)'\\';
                            break;
                        case '\b':
                            escapedChar = (byte)'b';
                            break;
                        case '\f':
                            escapedChar = (byte)'f';
                            break;
                        case '\n':
                            escapedChar = (byte)'n';
                            break;
                        case '\r':
                            escapedChar = (byte)'r';
                            break;
                        case '\t':
                            escapedChar = (byte)'t';
                            break;
                        #region Other
                        case (char)0:
                        case (char)1:
                        case (char)2:
                        case (char)3:
                        case (char)4:
                        case (char)5:
                        case (char)6:
                        case (char)7:
                        case (char)11:
                        case (char)14:
                        case (char)15:
                        case (char)16:
                        case (char)17:
                        case (char)18:
                        case (char)19:
                        case (char)20:
                        case (char)21:
                        case (char)22:
                        case (char)23:
                        case (char)24:
                        case (char)25:
                        case (char)26:
                        case (char)27:
                        case (char)28:
                        case (char)29:
                        case (char)30:
                        case (char)31:
                        case (char)32:
                        case (char)33:
                        case (char)35:
                        case (char)36:
                        case (char)37:
                        case (char)38:
                        case (char)39:
                        case (char)40:
                        case (char)41:
                        case (char)42:
                        case (char)43:
                        case (char)44:
                        case (char)45:
                        case (char)46:
                        case (char)47:
                        case (char)48:
                        case (char)49:
                        case (char)50:
                        case (char)51:
                        case (char)52:
                        case (char)53:
                        case (char)54:
                        case (char)55:
                        case (char)56:
                        case (char)57:
                        case (char)58:
                        case (char)59:
                        case (char)60:
                        case (char)61:
                        case (char)62:
                        case (char)63:
                        case (char)64:
                        case (char)65:
                        case (char)66:
                        case (char)67:
                        case (char)68:
                        case (char)69:
                        case (char)70:
                        case (char)71:
                        case (char)72:
                        case (char)73:
                        case (char)74:
                        case (char)75:
                        case (char)76:
                        case (char)77:
                        case (char)78:
                        case (char)79:
                        case (char)80:
                        case (char)81:
                        case (char)82:
                        case (char)83:
                        case (char)84:
                        case (char)85:
                        case (char)86:
                        case (char)87:
                        case (char)88:
                        case (char)89:
                        case (char)90:
                        case (char)91:
                        default:
                            #endregion
                            index++;
                            continue;
                    }

                    if (index != 0)
                    {
#if SPAN_BUILTIN
                        var consumed = StringEncoding.Utf8.GetBytes(value.Slice(0, index), span);
#else
                        int consumed;
                        fixed (char* src = &value[0])
                        fixed (byte* dst = &span[0])
                        {
                            consumed = StringEncoding.Utf8.GetBytes(src, index, dst, span.Length);
                        }
#endif
                        actualLength += consumed;
                        span = span.Slice(consumed);
                    }

                    span[0] = (byte)'\\';
                    span[1] = escapedChar;
                    span = span.Slice(2);
                    value = value.Slice(index + 1);
                    index = 0;
                    actualLength += 2;
                }

                if (!value.IsEmpty)
                {
#if SPAN_BUILTIN
                    var consumed = StringEncoding.Utf8.GetBytes(value, span);
#else
                    int consumed;
                    fixed (char* src = &value[0])
                    fixed (byte* dst = &span[0])
                    {
                        consumed = StringEncoding.Utf8.GetBytes(src, value.Length, dst, span.Length);
                    }
#endif
                    actualLength += consumed;
                }

                var answer = new byte[actualLength + 2];
                answer[0] = answer[actualLength + 1] = (byte)'"';
                fixed (byte* dst = &answer[1])
                fixed (byte* src = &array[0])
                {
                    Buffer.MemoryCopy(src, dst, actualLength, actualLength);
                }

                return answer;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(bool value)
        {
            return value
                ? new[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e', }
                : new[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', };
        }

        public static byte[] Serialize(uint value)
        {
            byte[] buffer;
            var offset = 0;
            //4294967295 == uint.MaxValue 10 chars
            if (value < 10000)
            {
                if (value < 10)
                {
                    buffer = new byte[1];
                    goto L1;
                }
                if (value < 100)
                {
                    buffer = new byte[2];
                    goto L2;
                }
                if (value < 1000)
                {
                    buffer = new byte[3];
                    goto L3;
                }
                buffer = new byte[4];
                goto L4;
            }
            var num2 = value / 10000;
            value -= num2 * 10000;
            if (num2 < 10000)
            {
                if (num2 < 10)
                {
                    buffer = new byte[5];
                    goto L5;
                }
                if (num2 < 100)
                {
                    buffer = new byte[6];
                    goto L6;
                }
                if (num2 < 1000)
                {
                    buffer = new byte[7];
                    goto L7;
                }
                buffer = new byte[8];
                goto L8;
            }

            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10)
            {
                buffer = new byte[9];
                goto L9;
            }

            buffer = new byte[10];

            uint div;
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554U) >> 16));
            num3 -= div * 10U;
        L9:
            buffer[offset++] = (byte)('0' + num3);
        L8:
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389U) >> 23));
            num2 -= div * 1000U;
        L7:
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243U) >> 19));
            num2 -= div * 100U;
        L6:
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554U) >> 16));
            num2 -= div * 10U;
        L5:
            buffer[offset++] = (byte)('0' + num2);
        L4:
            buffer[offset++] = (byte)('0' + (div = (value * 8389U) >> 23));
            value -= div * 1000U;
        L3:
            buffer[offset++] = (byte)('0' + (div = (value * 5243U) >> 19));
            value -= div * 100U;
        L2:
            buffer[offset++] = (byte)('0' + (div = (value * 6554U) >> 16));
            value -= div * 10U;
        L1:
            buffer[offset] = (byte)('0' + value);
            return buffer;
        }

        public static byte[] Serialize(int value)
        {
            if (value == int.MinValue)
            {
                // -2147483648
                return new[] { (byte)'-', (byte)'2', (byte)'1', (byte)'4', (byte)'7', (byte)'4', (byte)'8', (byte)'3', (byte)'6', (byte)'4', (byte)'8', };
            }

            if (value >= 0)
            {
                return Serialize((uint)value);
            }

            var value1 = (uint)-value;
            byte[] buffer;
            var offset = 1;
            //4294967295 == uint.MaxValue 10 chars
            if (value1 < 10000)
            {
                if (value1 < 10)
                {
                    buffer = new byte[2];
                    goto L1;
                }
                if (value1 < 100)
                {
                    buffer = new byte[3];
                    goto L2;
                }
                if (value1 < 1000)
                {
                    buffer = new byte[4];
                    goto L3;
                }
                buffer = new byte[5];
                goto L4;
            }
            var num2 = value1 / 10000;
            value1 -= num2 * 10000;
            if (num2 < 10000)
            {
                if (num2 < 10)
                {
                    buffer = new byte[6];
                    goto L5;
                }
                if (num2 < 100)
                {
                    buffer = new byte[7];
                    goto L6;
                }
                if (num2 < 1000)
                {
                    buffer = new byte[8];
                    goto L7;
                }
                buffer = new byte[9];
                goto L8;
            }

            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10)
            {
                buffer = new byte[10];
                goto L9;
            }

            buffer = new byte[11];

            uint div;
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554U) >> 16));
            num3 -= div * 10U;
        L9:
            buffer[offset++] = (byte)('0' + num3);
        L8:
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389U) >> 23));
            num2 -= div * 1000U;
        L7:
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243U) >> 19));
            num2 -= div * 100U;
        L6:
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554U) >> 16));
            num2 -= div * 10U;
        L5:
            buffer[offset++] = (byte)('0' + num2);
        L4:
            buffer[offset++] = (byte)('0' + (div = (value1 * 8389U) >> 23));
            value1 -= div * 1000U;
        L3:
            buffer[offset++] = (byte)('0' + (div = (value1 * 5243U) >> 19));
            value1 -= div * 100U;
        L2:
            buffer[offset++] = (byte)('0' + (div = (value1 * 6554U) >> 16));
            value1 -= div * 10U;
        L1:
            buffer[offset] = (byte)('0' + value1);
            buffer[0] = (byte)'-';
            return buffer;
        }

        public static byte[] Serialize(ulong value)
        {
            if (value >> 32 == 0)
            {
                return Serialize((uint)value);
            }

            byte[] buffer;
            var offset = 0;
            var value1 = value;
            var num2 = value1 / 10000;
            value1 -= num2 * 10000;
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10000)
            {
                if (num3 < 100)
                {
                    buffer = new byte[10];
                    goto L10;
                }
                if (num3 < 1000)
                {
                    buffer = new byte[11];
                    goto L11;
                }
                buffer = new byte[12];
                goto L12;
            }

            var num4 = num3 / 10000;
            num3 -= num4 * 10000;
            if (num4 < 10000)
            {
                if (num4 < 10)
                {
                    buffer = new byte[13];
                    goto L13;
                }
                if (num4 < 100)
                {
                    buffer = new byte[14];
                    goto L14;
                }
                if (num4 < 1000)
                {
                    buffer = new byte[15];
                    goto L15;
                }
                buffer = new byte[16];
                goto L16;
            }

            var num5 = num4 / 10000;
            num4 -= num5 * 10000;
            if (num5 < 10)
            {
                buffer = new byte[17];
                goto L17;
            }
            if (num5 < 100)
            {
                buffer = new byte[18];
                goto L18;
            }
            if (num5 < 1000)
            {
                buffer = new byte[19];
                goto L19;
            }

            buffer = new byte[20];
            ulong div;
            buffer[offset++] = (byte)('0' + (div = (num5 * 8389UL) >> 23));
            num5 -= div * 1000;
        L19:
            buffer[offset++] = (byte)('0' + (div = (num5 * 5243UL) >> 19));
            num5 -= div * 100;
        L18:
            buffer[offset++] = (byte)('0' + (div = (num5 * 6554UL) >> 16));
            num5 -= div * 10;
        L17:
            buffer[offset++] = (byte)('0' + num5);
        L16:
            buffer[offset++] = (byte)('0' + (div = (num4 * 8389UL) >> 23));
            num4 -= div * 1000;
        L15:
            buffer[offset++] = (byte)('0' + (div = (num4 * 5243UL) >> 19));
            num4 -= div * 100;
        L14:
            buffer[offset++] = (byte)('0' + (div = (num4 * 6554UL) >> 16));
            num4 -= div * 10;
        L13:
            buffer[offset++] = (byte)('0' + num4);
        L12:
            buffer[offset++] = (byte)('0' + (div = (num3 * 8389UL) >> 23));
            num3 -= div * 1000;
        L11:
            buffer[offset++] = (byte)('0' + (div = (num3 * 5243UL) >> 19));
            num3 -= div * 100;
        L10:
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + num3 - div * 10);
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389UL) >> 23));
            num2 -= div * 1000;
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243UL) >> 19));
            num2 -= div * 100;
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + num2 - div * 10);
            buffer[offset++] = (byte)('0' + (div = (value1 * 8389UL) >> 23));
            value1 -= div * 1000;
            buffer[offset++] = (byte)('0' + (div = (value1 * 5243UL) >> 19));
            value1 -= div * 100;
            buffer[offset++] = (byte)('0' + (div = (value1 * 6554UL) >> 16));
            buffer[offset] = (byte)('0' + value1 - div * 10);
            return buffer;
        }

        public static byte[] Serialize(long value)
        {
            if (value >= 0)
            {
                return Serialize((ulong)value);
            }

            if (value >= int.MinValue)
            {
                return Serialize(unchecked((int)value));
            }

            if (value == long.MinValue)
            {
                // -922 3372 0368 5477 5808
                return new[] { (byte)'-', (byte)'9', (byte)'2', (byte)'2',
                    (byte)'3', (byte)'3', (byte)'7', (byte)'2',
                    (byte)'0', (byte)'3', (byte)'6', (byte)'8',
                    (byte)'5', (byte)'4', (byte)'7', (byte)'7',
                    (byte)'5', (byte)'8', (byte)'0', (byte)'8', };
            }

            byte[] buffer;
            var offset = 1;
            var value1 = (ulong)-value;
            var num2 = value1 / 10000;
            value1 -= num2 * 10000;
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10000)
            {
                if (num3 < 100)
                {
                    buffer = new byte[11];
                    goto L10;
                }
                if (num3 < 1000)
                {
                    buffer = new byte[12];
                    goto L11;
                }
                buffer = new byte[13];
                goto L12;
            }

            var num4 = num3 / 10000;
            num3 -= num4 * 10000;
            if (num4 < 10000)
            {
                if (num4 < 10)
                {
                    buffer = new byte[14];
                    goto L13;
                }
                if (num4 < 100)
                {
                    buffer = new byte[15];
                    goto L14;
                }
                if (num4 < 1000)
                {
                    buffer = new byte[16];
                    goto L15;
                }
                buffer = new byte[17];
                goto L16;
            }

            var num5 = num4 / 10000;
            num4 -= num5 * 10000;
            if (num5 < 10)
            {
                buffer = new byte[18];
                goto L17;
            }
            if (num5 < 100)
            {
                buffer = new byte[19];
                goto L18;
            }
            if (num5 < 1000)
            {
                buffer = new byte[20];
                goto L19;
            }

            buffer = new byte[21];
            ulong div;
            buffer[offset++] = (byte)('0' + (div = (num5 * 8389UL) >> 23));
            num5 -= div * 1000;
        L19:
            buffer[offset++] = (byte)('0' + (div = (num5 * 5243UL) >> 19));
            num5 -= div * 100;
        L18:
            buffer[offset++] = (byte)('0' + (div = (num5 * 6554UL) >> 16));
            num5 -= div * 10;
        L17:
            buffer[offset++] = (byte)('0' + num5);
        L16:
            buffer[offset++] = (byte)('0' + (div = (num4 * 8389UL) >> 23));
            num4 -= div * 1000;
        L15:
            buffer[offset++] = (byte)('0' + (div = (num4 * 5243UL) >> 19));
            num4 -= div * 100;
        L14:
            buffer[offset++] = (byte)('0' + (div = (num4 * 6554UL) >> 16));
            num4 -= div * 10;
        L13:
            buffer[offset++] = (byte)('0' + num4);
        L12:
            buffer[offset++] = (byte)('0' + (div = (num3 * 8389UL) >> 23));
            num3 -= div * 1000;
        L11:
            buffer[offset++] = (byte)('0' + (div = (num3 * 5243UL) >> 19));
            num3 -= div * 100;
        L10:
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + num3 - div * 10);
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389UL) >> 23));
            num2 -= div * 1000;
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243UL) >> 19));
            num2 -= div * 100;
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + num2 - div * 10);
            buffer[offset++] = (byte)('0' + (div = (value1 * 8389UL) >> 23));
            value1 -= div * 1000;
            buffer[offset++] = (byte)('0' + (div = (value1 * 5243UL) >> 19));
            value1 -= div * 100;
            buffer[offset++] = (byte)('0' + (div = (value1 * 6554UL) >> 16));
            buffer[offset] = (byte)('0' + value1 - div * 10);
            buffer[0] = (byte)'-';
            return buffer;
        }
    }
}
