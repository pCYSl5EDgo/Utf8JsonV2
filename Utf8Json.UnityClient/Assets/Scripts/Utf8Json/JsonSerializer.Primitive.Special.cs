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
    }
}
