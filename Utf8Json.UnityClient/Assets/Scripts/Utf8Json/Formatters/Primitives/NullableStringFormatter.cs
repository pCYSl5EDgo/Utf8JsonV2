// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

#pragma warning disable IDE0060

namespace Utf8Json.Formatters
{
    public sealed class NullableStringFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<string?>
#else
        : IJsonFormatter<string>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, string? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, string value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public string? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public string Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadString();
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, string? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, string value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            if (value.Length == 0)
            {
                var emptySpan = writer.Writer.GetSpan(2);
                emptySpan[0] = 0x22;
                emptySpan[1] = 0x22;
                writer.Writer.Advance(2);
                return;
            }

            writer.Write(value.AsSpan());
        }

#if CSHARP_8_OR_NEWER
        public static string? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static string DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadString();
        }

        public static unsafe string DeserializeStaticInnerQuotation(ReadOnlySpan<byte> sourceSpan)
        {
            if (sourceSpan.IsEmpty)
            {
                return string.Empty;
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

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as string, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

        public static int CalcByteLength(string value)
        {
            var answer = 2 + value.Length;
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"':
                    case '\\':
                    case '\b':
                    case '\f':
                    case '\n':
                    case '\r':
                    case '\t':
                        answer++;
                        continue;
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
                        answer += 5;
                        continue;

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
                    case (char)93:
                    case (char)94:
                    case (char)95:
                    case (char)96:
                    case (char)97:
                    case (char)98:
                    case (char)99:
                    case (char)100:
                    case (char)101:
                    case (char)102:
                    case (char)103:
                    case (char)104:
                    case (char)105:
                    case (char)106:
                    case (char)107:
                    case (char)108:
                    case (char)109:
                    case (char)110:
                    case (char)111:
                    case (char)112:
                    case (char)113:
                    case (char)114:
                    case (char)115:
                    case (char)116:
                    case (char)117:
                    case (char)118:
                    case (char)119:
                    case (char)120:
                    case (char)121:
                    case (char)122:
                    case (char)123:
                    case (char)124:
                    case (char)125:
                    case (char)126:
                    case (char)127:
                        #endregion
                        continue;
                }
                if (c < 0x0800 || char.IsSurrogate(c))
                {
                    answer++;
                }
                else
                {
                    answer += 2;
                }
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, string? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, string value)
#endif
        {
            return value is null ? 4 : CalcByteLength(value);
        }

        public static
#if !SPAN_BUILTIN
            unsafe
#endif
            void SerializeSpanNotNull(string value, Span<byte> span)
        {
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
        }

        public static
#if !SPAN_BUILTIN
            unsafe
#endif
            void SerializeSpanNotNullNoQuotation(string value, Span<byte> span)
        {
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

            if (input.IsEmpty)
            {
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, string? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, string value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            SerializeSpanNotNull(value, span);
        }
    }
}
