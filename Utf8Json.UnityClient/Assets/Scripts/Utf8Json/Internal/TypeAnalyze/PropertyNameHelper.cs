// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable UseIndexFromEndExpression
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Internal
{
    public static
        unsafe
        class PropertyNameHelper
    {
#if SPAN_BUILTIN
        public static string FromSpanToString(ReadOnlySpan<byte> sourceSpan)
        {
            if (sourceSpan.IsEmpty)
            {
                return string.Empty;
            }

            if (sourceSpan.Length == 1)
            {
                var c = (sbyte)sourceSpan[0];
                return new string(&c, 0, 1);
            }

            var array = ArrayPool<byte>.Shared.Rent(sourceSpan.Length << 1);
            var unwrittenSpan = MemoryMarshal.Cast<byte, char>(array);
            try
            {
                do
                {
                    var escapeIndex = sourceSpan.IndexOf((byte)'\\');
                    if (escapeIndex == -1)
                    {
                        var restCharCount = StringEncoding.Utf8.GetChars(sourceSpan, unwrittenSpan);
                        unwrittenSpan = unwrittenSpan.Slice(restCharCount);
                        goto RETURN;
                    }

                    if (escapeIndex != 0)
                    {
                        var consumeTargetSpan = sourceSpan.Slice(0, escapeIndex);
                        var charCount = StringEncoding.Utf8.GetChars(consumeTargetSpan, unwrittenSpan);
                        unwrittenSpan = unwrittenSpan.Slice(charCount);
                        sourceSpan = sourceSpan.Slice(escapeIndex);
                    }

                    switch (sourceSpan[1])
                    {
                        case (byte)'u': // 0x75
                            unwrittenSpan[0] = (char)(ushort)((ToNumber(sourceSpan[2]) << 12) | (ToNumber(sourceSpan[3]) << 8) | (ToNumber(sourceSpan[4]) << 4) | ToNumber(sourceSpan[5]));
                            unwrittenSpan = unwrittenSpan.Slice(1);
                            sourceSpan = sourceSpan.Slice(6);
                            continue;
                        case (byte)'\\':// 0x5c
                        case (byte)'/': // 0x2f
                        case (byte)'"': // 0x22
                            unwrittenSpan[0] = (char)sourceSpan[1];
                            goto INCREMENT;
                        case (byte)'b':
                            unwrittenSpan[0] = '\b';
                            goto INCREMENT;
                        case (byte)'r':
                            unwrittenSpan[0] = '\r';
                            goto INCREMENT;
                        case (byte)'n':
                            unwrittenSpan[0] = '\n';
                            goto INCREMENT;
                        case (byte)'f':
                            unwrittenSpan[0] = '\f';
                            goto INCREMENT;
                        case (byte)'t':
                            unwrittenSpan[0] = '\t';
                            goto INCREMENT;

                        #region OTHER
                        case 0x23:
                        case 0x24:
                        case 0x25:
                        case 0x26:
                        case 0x27:
                        case 0x28:
                        case 0x29:
                        case 0x2a:
                        case 0x2b:
                        case 0x2c:
                        case 0x2d:
                        case 0x2e:
                        case 0x30:
                        case 0x31:
                        case 0x32:
                        case 0x33:
                        case 0x34:
                        case 0x35:
                        case 0x36:
                        case 0x37:
                        case 0x38:
                        case 0x39:
                        case 0x3a:
                        case 0x3b:
                        case 0x3c:
                        case 0x3d:
                        case 0x3e:
                        case 0x3f:
                        case 0x40:
                        case 0x41:
                        case 0x42:
                        case 0x43:
                        case 0x44:
                        case 0x45:
                        case 0x46:
                        case 0x47:
                        case 0x48:
                        case 0x49:
                        case 0x4a:
                        case 0x4b:
                        case 0x4c:
                        case 0x4d:
                        case 0x4e:
                        case 0x4f:
                        case 0x50:
                        case 0x51:
                        case 0x52:
                        case 0x53:
                        case 0x54:
                        case 0x55:
                        case 0x56:
                        case 0x57:
                        case 0x58:
                        case 0x59:
                        case 0x5a:
                        case 0x5b:
                        case 0x5d:
                        case 0x5e:
                        case 0x5f:
                        case 0x60:
                        case 0x61:
                        case 0x63:
                        case 0x64:
                        case 0x65:
                        case 0x67:
                        case 0x68:
                        case 0x69:
                        case 0x6a:
                        case 0x6b:
                        case 0x6c:
                        case 0x6d:
                        case 0x6f:
                        case 0x70:
                        case 0x71:
                        case 0x73:
                        default:
                            #endregion
                            throw new JsonParsingException("Invalid string.");
                    }

                INCREMENT:
                    unwrittenSpan = unwrittenSpan.Slice(1);
                    sourceSpan = sourceSpan.Slice(2);
                } while (!sourceSpan.IsEmpty);

            RETURN:
                return string.Create((array.Length >> 1) - unwrittenSpan.Length, array, WriteAction);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        private static void WriteAction(Span<char> span, byte[] array)
        {
            MemoryMarshal.Cast<byte, char>(array).Slice(0, span.Length).CopyTo(span);
        }
#else
        public static string FromSpanToString(ReadOnlySpan<byte> sourceSpan)
        {
            if (sourceSpan.IsEmpty)
            {
                return string.Empty;
            }

            if (sourceSpan.Length == 1)
            {
                var c = (sbyte)sourceSpan[0];
                return new string(&c, 0, 1);
            }

            var array = ArrayPool<byte>.Shared.Rent(sourceSpan.Length << 1);

            try
            {
                fixed (byte* sourceSpanPointer = &sourceSpan[0])
                fixed (void* arrayPointer = &array[0])
                {
                    var answerLength = 0;
                    var srcPtr = sourceSpanPointer;
                    var srcEndPtr = sourceSpanPointer + sourceSpan.Length;
                    var dstPtr = (char*)arrayPointer;
                    var dstCapacity = array.Length >> 1;
                    var length = 0;
                    while (srcPtr != srcEndPtr)
                    {
                        if (*srcPtr != '\\')
                        {
                            length++;
                            srcPtr++;
                            continue;
                        }

                        if (length != 0)
                        {
                            var consumed = StringEncoding.Utf8.GetChars(srcPtr - length, length, dstPtr, dstCapacity);
                            dstCapacity -= consumed;
                            dstPtr += consumed;
                            answerLength += consumed;
                            length = 0;
                        }

                        if (++srcPtr == srcEndPtr)
                        {
                            goto ERROR;
                        }

                        switch (*srcPtr++)
                        {
                            case (byte)'u':
                                if (srcPtr == srcEndPtr)
                                {
                                    goto ERROR;
                                }

                                var codePoint = ToNumber(*srcPtr++) << 4;
                                if (srcPtr == srcEndPtr)
                                {
                                    goto ERROR;
                                }

                                codePoint |= ToNumber(*srcPtr++);
                                codePoint <<= 4;
                                if (srcPtr == srcEndPtr)
                                {
                                    goto ERROR;
                                }

                                codePoint |= ToNumber(*srcPtr++);
                                codePoint <<= 4;
                                if (srcPtr == srcEndPtr)
                                {
                                    goto ERROR;
                                }

                                codePoint |= ToNumber(*srcPtr++);
                                *dstPtr++ = (char)(ushort)codePoint;
                                goto INCREMENT;
                            case (byte)'\\':// 0x5c
                                *dstPtr++ = '\\';
                                goto INCREMENT;
                            case (byte)'/': // 0x2f
                                *dstPtr++ = '/';
                                goto INCREMENT;
                            case (byte)'"': // 0x22
                                *dstPtr++ = '"';
                                goto INCREMENT;
                            case (byte)'b':
                                *dstPtr++ = '\b';
                                goto INCREMENT;
                            case (byte)'r':
                                *dstPtr++ = '\r';
                                goto INCREMENT;
                            case (byte)'n':
                                *dstPtr++ = '\n';
                                goto INCREMENT;
                            case (byte)'f':
                                *dstPtr++ = '\f';
                                goto INCREMENT;
                            case (byte)'t':
                                *dstPtr++ = '\t';
                                goto INCREMENT;
        #region OTHER
                            case 0x23:
                            case 0x24:
                            case 0x25:
                            case 0x26:
                            case 0x27:
                            case 0x28:
                            case 0x29:
                            case 0x2a:
                            case 0x2b:
                            case 0x2c:
                            case 0x2d:
                            case 0x2e:
                            case 0x30:
                            case 0x31:
                            case 0x32:
                            case 0x33:
                            case 0x34:
                            case 0x35:
                            case 0x36:
                            case 0x37:
                            case 0x38:
                            case 0x39:
                            case 0x3a:
                            case 0x3b:
                            case 0x3c:
                            case 0x3d:
                            case 0x3e:
                            case 0x3f:
                            case 0x40:
                            case 0x41:
                            case 0x42:
                            case 0x43:
                            case 0x44:
                            case 0x45:
                            case 0x46:
                            case 0x47:
                            case 0x48:
                            case 0x49:
                            case 0x4a:
                            case 0x4b:
                            case 0x4c:
                            case 0x4d:
                            case 0x4e:
                            case 0x4f:
                            case 0x50:
                            case 0x51:
                            case 0x52:
                            case 0x53:
                            case 0x54:
                            case 0x55:
                            case 0x56:
                            case 0x57:
                            case 0x58:
                            case 0x59:
                            case 0x5a:
                            case 0x5b:
                            case 0x5d:
                            case 0x5e:
                            case 0x5f:
                            case 0x60:
                            case 0x61:
                            case 0x63:
                            case 0x64:
                            case 0x65:
                            case 0x67:
                            case 0x68:
                            case 0x69:
                            case 0x6a:
                            case 0x6b:
                            case 0x6c:
                            case 0x6d:
                            case 0x6f:
                            case 0x70:
                            case 0x71:
                            case 0x73:
                            default:
        #endregion
                                goto ERROR;
                        }

                    INCREMENT:
                        dstCapacity--;
                        answerLength++;
                        continue;

                    ERROR:
                        throw new JsonParsingException("Invalid Json String Literal.");
                    }

                    if (length != 0)
                    {
                        answerLength += StringEncoding.Utf8.GetChars(srcPtr - length, length, dstPtr, dstCapacity);
                    }

                    return new string((char*)arrayPointer, 0, answerLength);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ToNumber(byte value)
        {
            // 0x30 ~ 0x7a
            switch (value)
            {
                case 0x30:
                    return 0;
                case 0x31:
                    return 1;
                case 0x32:
                    return 2;
                case 0x33:
                    return 3;
                case 0x34:
                    return 4;
                case 0x35:
                    return 5;
                case 0x36:
                    return 6;
                case 0x37:
                    return 7;
                case 0x38:
                    return 8;
                case 0x39:
                    return 9;
                case 0x41:
                    return 10;
                case 0x42:
                    return 11;
                case 0x43:
                    return 12;
                case 0x44:
                    return 13;
                case 0x45:
                    return 14;
                case 0x46:
                    return 15;
                case 0x61:
                    return 10;
                case 0x62:
                    return 11;
                case 0x63:
                    return 12;
                case 0x64:
                    return 13;
                case 0x65:
                    return 14;
                case 0x66:
                    return 15;

                #region Other
                case 0x3a:
                case 0x3b:
                case 0x3c:
                case 0x3d:
                case 0x3e:
                case 0x3f:
                case 0x40:
                case 0x47:
                case 0x48:
                case 0x49:
                case 0x4a:
                case 0x4b:
                case 0x4c:
                case 0x4d:
                case 0x4e:
                case 0x4f:
                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                case 0x58:
                case 0x59:
                case 0x5a:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x5f:
                case 0x60:
                default:
                    #endregion
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public static int CalculatePropertyNameByteRawLength(string value)
        {
            var length = value.Length;
            foreach (var character in value)
            {
                var c = (int)character;
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

            return length;
        }

        public static void WritePropertyNameByteRawToSpan(ReadOnlySpan<char> input, Span<byte> span)
        {
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
                StringEncoding.Utf8.GetBytes(input, span);
#else
                fixed (char* src = &input[0])
                fixed (byte* dst = &span[0])
                {
                    StringEncoding.Utf8.GetBytes(src, input.Length, dst, span.Length);
                }
#endif
            }
        }

        public static bool SequenceEqualsIgnoreCase(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            for (var index = 0; index < x.Length; index++)
            {
                var b0 = x[index];
                var b1 = y[index];
                if (b0 >= 0x41 && b0 <= 0x5A)
                {
                    b0 += 0x20;
                }
                if (b1 >= 0x41 && b1 <= 0x5A)
                {
                    b1 += 0x20;
                }
                if (b0 != b1)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ToSmall(Span<byte> span)
        {
            foreach (ref var b in span)
            {
                if (b >= 0x41 && b <= 0x5A)
                {
                    b += 0x20;
                }
            }
        }

        public static byte[] CalculatePropertyNameBytes(string value)
        {
            if (value.Length == 0)
            {
                return new[]
                {
                    (byte)',',
                    (byte)'"',
                    (byte)'"',
                    (byte)':',
                };
            }

            var length = CalculatePropertyNameByteRawLength(value) + 4;

            var answer = new byte[length];
            answer[0] = (byte)',';
            answer[1] = (byte)'"';
            answer[answer.Length - 2] = (byte)'"';
            answer[answer.Length - 1] = (byte)':';

            var input = value.AsSpan();
            var span = answer.AsSpan(2, answer.Length - 4);

            WritePropertyNameByteRawToSpan(input, span);

            return answer;
        }
    }
}
