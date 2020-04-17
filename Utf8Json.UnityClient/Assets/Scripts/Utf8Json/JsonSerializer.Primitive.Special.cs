// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        public static
#if !SPAN_BUILTIN
            unsafe
#endif
            byte[] Serialize(string
#if CSHARP_8_OR_NEWER
                    ?
#endif
                    value)
        {
            if (value is null)
            {
                return new[] { (byte)'n', (byte)'u', (byte)'l', (byte)'l' };
            }

            if (value.Length == 0)
            {
                return new[] { (byte)'"', (byte)'"' };
            }

            var length = value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                var c = (int)value[index];
                switch (c)
                {
                    case '"':
                    case '\\':
                    case '\b':
                    case '\f':
                    case '\n':
                    case '\r':
                    case '\t': length++; continue;
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 11:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31: length += 5; continue;
                    #region Other
                    case 32:
                    case 33:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                    case 48:
                    case 49:
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                    case 54:
                    case 55:
                    case 56:
                    case 57:
                    case 58:
                    case 59:
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                    case 66:
                    case 67:
                    case 68:
                    case 69:
                    case 70:
                    case 71:
                    case 72:
                    case 73:
                    case 74:
                    case 75:
                    case 76:
                    case 77:
                    case 78:
                    case 79:
                    case 80:
                    case 81:
                    case 82:
                    case 83:
                    case 84:
                    case 85:
                    case 86:
                    case 87:
                    case 88:
                    case 89:
                    case 90:
                    case 91:
                    case 93:
                    case 94:
                    case 95:
                    case 96:
                    case 97:
                    case 98:
                    case 99:
                    case 100:
                    case 101:
                    case 102:
                    case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                    case 108:
                    case 109:
                    case 110:
                    case 111:
                    case 112:
                    case 113:
                    case 114:
                    case 115:
                    case 116:
                    case 117:
                    case 118:
                    case 119:
                    case 120:
                    case 121:
                    case 122:
                    case 123:
                    case 124:
                    case 125:
                    case 126:
                    case 127:
                        #endregion
                        continue;
                }
                if (c < 0x7ff)
                {
                    length++;
                }
                if (c >= 0xd800 && c <= 0xdfff)
                {
                    length++;
                }
                else
                {
                    length += 2;
                }
            }

            var answer = new byte[2 + length];
            var span = answer.AsSpan();

            span[0] = (byte)'"';
            span = span.Slice(1);
            var input = value.AsSpan();

            for (var index = 0; index < input.Length;)
            {
                byte escapedChar;
                byte escapedChar2;
                switch (input[index])
                {
                    case '"': escapedChar = (byte)'"'; goto ESCAPE_SINGLE;
                    case '\\': escapedChar = (byte)'\\'; goto ESCAPE_SINGLE;
                    case '\b': escapedChar = (byte)'b'; goto ESCAPE_SINGLE;
                    case '\f': escapedChar = (byte)'f'; goto ESCAPE_SINGLE;
                    case '\n': escapedChar = (byte)'n'; goto ESCAPE_SINGLE;
                    case '\r': escapedChar = (byte)'r'; goto ESCAPE_SINGLE;
                    case '\t': escapedChar = (byte)'t'; goto ESCAPE_SINGLE;
                    case (char)0: escapedChar = (byte)'0'; escapedChar2 = (byte)'0'; goto ESCAPE_HEX;
                    case (char)1: escapedChar = (byte)'0'; escapedChar2 = (byte)'1'; goto ESCAPE_HEX;
                    case (char)2: escapedChar = (byte)'0'; escapedChar2 = (byte)'2'; goto ESCAPE_HEX;
                    case (char)3: escapedChar = (byte)'0'; escapedChar2 = (byte)'3'; goto ESCAPE_HEX;
                    case (char)4: escapedChar = (byte)'0'; escapedChar2 = (byte)'4'; goto ESCAPE_HEX;
                    case (char)5: escapedChar = (byte)'0'; escapedChar2 = (byte)'5'; goto ESCAPE_HEX;
                    case (char)6: escapedChar = (byte)'0'; escapedChar2 = (byte)'6'; goto ESCAPE_HEX;
                    case (char)7: escapedChar = (byte)'0'; escapedChar2 = (byte)'7'; goto ESCAPE_HEX;
                    case (char)11: escapedChar = (byte)'0'; escapedChar2 = (byte)'b'; goto ESCAPE_HEX;
                    case (char)14: escapedChar = (byte)'0'; escapedChar2 = (byte)'e'; goto ESCAPE_HEX;
                    case (char)15: escapedChar = (byte)'0'; escapedChar2 = (byte)'f'; goto ESCAPE_HEX;
                    case (char)16: escapedChar = (byte)'1'; escapedChar2 = (byte)'0'; goto ESCAPE_HEX;
                    case (char)17: escapedChar = (byte)'1'; escapedChar2 = (byte)'1'; goto ESCAPE_HEX;
                    case (char)18: escapedChar = (byte)'1'; escapedChar2 = (byte)'2'; goto ESCAPE_HEX;
                    case (char)19: escapedChar = (byte)'1'; escapedChar2 = (byte)'3'; goto ESCAPE_HEX;
                    case (char)20: escapedChar = (byte)'1'; escapedChar2 = (byte)'4'; goto ESCAPE_HEX;
                    case (char)21: escapedChar = (byte)'1'; escapedChar2 = (byte)'5'; goto ESCAPE_HEX;
                    case (char)22: escapedChar = (byte)'1'; escapedChar2 = (byte)'6'; goto ESCAPE_HEX;
                    case (char)23: escapedChar = (byte)'1'; escapedChar2 = (byte)'7'; goto ESCAPE_HEX;
                    case (char)24: escapedChar = (byte)'1'; escapedChar2 = (byte)'8'; goto ESCAPE_HEX;
                    case (char)25: escapedChar = (byte)'1'; escapedChar2 = (byte)'9'; goto ESCAPE_HEX;
                    case (char)26: escapedChar = (byte)'1'; escapedChar2 = (byte)'a'; goto ESCAPE_HEX;
                    case (char)27: escapedChar = (byte)'1'; escapedChar2 = (byte)'b'; goto ESCAPE_HEX;
                    case (char)28: escapedChar = (byte)'1'; escapedChar2 = (byte)'c'; goto ESCAPE_HEX;
                    case (char)29: escapedChar = (byte)'1'; escapedChar2 = (byte)'d'; goto ESCAPE_HEX;
                    case (char)30: escapedChar = (byte)'1'; escapedChar2 = (byte)'e'; goto ESCAPE_HEX;
                    case (char)31: escapedChar = (byte)'1'; escapedChar2 = (byte)'f'; goto ESCAPE_HEX;
                    #region Other
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

            ESCAPE_SINGLE:
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
                    span = span.Slice(consumed);
                }

                span[0] = (byte)'\\';
                span[1] = escapedChar;
                span = span.Slice(2);
                input = input.Slice(index + 1);
                index = 0;
                continue;

            ESCAPE_HEX:
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
                    span = span.Slice(consumed);
                }

                span[0] = (byte)'\\';
                span[1] = (byte)'u';
                span[2] = (byte)'0';
                span[3] = (byte)'0';
                span[4] = escapedChar;
                span[5] = escapedChar2;

                span = span.Slice(6);
                input = input.Slice(index + 1);
                index = 0;
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
                span = span.Slice(consumed);
            }

            span[0] = (byte)'"';

            return answer;
        }
    }
}
