// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Utf8Json.Internal;
using Utf8Json.Internal.DoubleConversion;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json
{
    public static class JsonReaderExtension
    {
        private const string ExpectedFirst = "expected:'";
        private const string ExpectedLast = "'";

        public static string ReadPropertyName(ref this JsonReader reader)
        {
            var name = reader.ReadString();
            if (name is null)
            {
                throw new JsonParsingException("Property should not be null");
            }

            reader.ReadIsNameSeparatorWithVerify();
            return name;
        }

#if SPAN_BUILTIN
#if CSHARP_8_OR_NEWER
        public static unsafe string? ReadString(ref this JsonReader reader)
#else
        public static unsafe string ReadString(ref this JsonReader reader)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            var sourceSpan = reader.ReadNotNullStringSegmentRaw();

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
        public static unsafe string ReadString(ref this JsonReader reader)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            var sourceSpan = reader.ReadNotNullStringSegmentRaw();

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();

            if (reader.AdvanceTrue()) return true;

            if (reader.AdvanceFalse()) return false;

            throw new JsonParsingException(ExpectedFirst + "true | false" + ExpectedLast);
        }

        /// <summary>
        /// Advance false. After SkipWhiteSpace().
        /// </summary>
        /// <returns>Is false?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AdvanceFalse(ref this JsonReader reader)
        {
#if CSHARP_8_OR_NEWER
            if (!reader.Reader.UnreadSpan.StartsWith(stackalloc byte[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' }))
#else
            ReadOnlySpan<byte> bytes = stackalloc byte[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
            if (!reader.Reader.UnreadSpan.StartsWith(bytes))
#endif
            {
                return false;
            }

            reader.Reader.Advance(4);
            return true;
        }

        /// <summary>
        /// Advance true. After SkipWhiteSpace().
        /// </summary>
        /// <returns>Is true?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AdvanceTrue(ref this JsonReader reader)
        {
#if CSHARP_8_OR_NEWER
            if (!reader.Reader.UnreadSpan.StartsWith(stackalloc byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' }))
#else
            ReadOnlySpan<byte> bytes = stackalloc byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
            if (!reader.Reader.UnreadSpan.StartsWith(bytes))
#endif
            {
                return false;
            }

            reader.Reader.Advance(4);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref this JsonReader reader)
        {
            return checked((sbyte)reader.ReadInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref this JsonReader reader)
        {
            return checked((short)reader.ReadInt64());
        }

        public static int ReadInt32(ref this JsonReader reader)
        {
            return checked((int)reader.ReadInt64());
        }

        public static long ReadInt64(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);

            if (span.IsEmpty || span.Length > (span[0] == '-' ? 21 : 20))
            {
                goto ERROR;
            }

            if (IntegerConverter.TryRead(span, out long answer))
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref this JsonReader reader)
        {
            return checked((byte)reader.ReadUInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref this JsonReader reader)
        {
            return checked((ushort)reader.ReadUInt64());
        }

        public static uint ReadUInt32(ref this JsonReader reader)
        {
            return checked((uint)reader.ReadUInt64());
        }

        public static ulong ReadUInt64(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);

            if (span.IsEmpty || span.Length > 20)
            {
                goto ERROR;
            }

            if (IntegerConverter.TryRead(span, out ulong answer))
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static float ReadSingle(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);
            var answer = StringToDoubleConverter.ToSingle(span, out var readCount);

            if (readCount == span.Length)
            {
                return answer;
            }

            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static double ReadDouble(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);
            var answer = StringToDoubleConverter.ToDouble(span, out var readCount);

            if (readCount == span.Length)
            {
                return answer;
            }

            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static decimal ReadDecimal(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);
            var answer = StringToDoubleConverter.ToDecimal(span, out var readCount);

            if (readCount == span.Length)
            {
                return answer;
            }

            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static DateTime ReadDateTime(ref this JsonReader reader)
        {
            if (reader.ReadIsNull())
            {
                throw new JsonParsingException("DateTime cannot be null.");
            }

            var text = reader.ReadNotNullStringSegmentRaw();
            if (text.IsEmpty)
            {
                return default;
            }

            if (DateTimeConverter.TryRead(text, out var answer))
            {
                return answer;
            }
#if SPAN_BUILTIN
            throw new JsonParsingException("Invalid DateTime. text + \"" + StringEncoding.Utf8.GetString(text) + "\"");
#else
            throw new JsonParsingException("Invalid DateTime.");
#endif
        }
    }
}