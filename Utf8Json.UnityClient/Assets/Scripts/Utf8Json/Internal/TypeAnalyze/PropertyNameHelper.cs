// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Utf8Json.Formatters;

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
    }
}
