// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
// ReSharper disable RedundantCaseLabel

#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace Utf8Json.Internal
{
    internal static class StringHelper
    {
        public static string Decode(ReadOnlySpan<byte> source)
        {
            if (source.IsEmpty)
            {
                return string.Empty;
            }

            var (length, escaped) = CalcDecodeLength(source);

#if SPAN_BUILTIN
            var answer = string.Create(length, 0, NoAction);
            unsafe
            {
                fixed (char* dest = answer)
                {
                    var span = new Span<char>(dest, length);
                    if (escaped)
                    {
                        DecodeFillEscapedBytes(span, source);
                    }
                    else
                    {
                        StringEncoding.Utf8.GetChars(source, span);
                    }
                }
            }

            return answer;
#else
            unsafe
            {
                if (!escaped)
                {
                    fixed (byte* ptr = &source[0])
                    {
                        return StringEncoding.Utf8.GetString(ptr, source.Length);
                    }
                }

                if (length < 1024)
                {
                    var destinationPtr = stackalloc char[length];
                    fixed (byte* ptr = &source[0])
                    {
                        CreateStringSpanActionFromEscapedBytes(ptr, source.Length, destinationPtr, length);
                    }

                    return new string(destinationPtr, 0, length);
                }

                {
#if UNITY_2018_4_OR_NEWER
                    var allocator = length < 65536 ? Allocator.Temp : Allocator.Persistent;
                    var destinationPtr = (char*)UnsafeUtility.Malloc(length << 1, sizeof(char), allocator);
                    try
                    {
                        fixed (byte* ptr = &source[0])
                        {
                            CreateStringSpanActionFromEscapedBytes(ptr, source.Length, destinationPtr, length);
                        }

                        return new string(destinationPtr, 0, length);
                    }
                    finally
                    {
                        UnsafeUtility.Free(destinationPtr, allocator);
                    }
#else
                    var destination = ArrayPool<char>.Shared.Rent(length);
                    try
                    {
                        fixed (byte* ptr = &source[0])
                        fixed (char* destinationPtr = &destination[0])
                        {
                            CreateStringSpanActionFromEscapedBytes(ptr, source.Length, destinationPtr, length);
                        }

                        return new string(destination);
                    }
                    finally
                    {
                        ArrayPool<char>.Shared.Return(destination);
                    }
#endif
                }
            }
#endif
        }

        internal static (int length, bool escaped) CalcDecodeLength(ReadOnlySpan<byte> sourceSpan)
        {
            var length = 0;
            var escaped = false;
            do
            {
                switch (sourceSpan[0])
                {
                    case 0x5c: // \\
                        escaped = true;
                        switch (sourceSpan[1])
                        {
                            case (byte)'\\':
                            case (byte)'/':
                            case (byte)'"':
                            case (byte)'b':
                            case (byte)'f':
                            case (byte)'n':
                            case (byte)'r':
                            case (byte)'t':
                                length++;
                                sourceSpan = sourceSpan.Slice(2);
                                break;
                            case (byte)'u':
                                length++;
                                sourceSpan = sourceSpan.Slice(6);
                                break;
                        }
                        break;

                    #region 1 byte

                    case 0x00:
                    case 0x01:
                    case 0x02:
                    case 0x03:
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                    case 0x08:
                    case 0x09:
                    case 0x0a:
                    case 0x0b:
                    case 0x0c:
                    case 0x0d:
                    case 0x0e:
                    case 0x0f:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x22:
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
                    case 0x2f:
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
                    case 0x62:
                    case 0x63:
                    case 0x64:
                    case 0x65:
                    case 0x66:
                    case 0x67:
                    case 0x68:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                    case 0x6e:
                    case 0x6f:
                    case 0x70:
                    case 0x71:
                    case 0x72:
                    case 0x73:
                    case 0x74:
                    case 0x75:
                    case 0x76:
                    case 0x77:
                    case 0x78:
                    case 0x79:
                    case 0x7a:
                    case 0x7b:
                    case 0x7c:
                    case 0x7d:
                    case 0x7e:
                    case 0x7f:

                        #endregion

                        length++;
                        sourceSpan = sourceSpan.Slice(1);
                        break;

                    #region 2 byte

                    case 0xc2:
                    case 0xc3:
                    case 0xc4:
                    case 0xc5:
                    case 0xc6:
                    case 0xc7:
                    case 0xc8:
                    case 0xc9:
                    case 0xca:
                    case 0xcb:
                    case 0xcc:
                    case 0xcd:
                    case 0xce:
                    case 0xcf:
                    case 0xd0:
                    case 0xd1:
                    case 0xd2:
                    case 0xd3:
                    case 0xd4:
                    case 0xd5:
                    case 0xd6:
                    case 0xd7:
                    case 0xd8:
                    case 0xd9:
                    case 0xda:
                    case 0xdb:
                    case 0xdc:
                    case 0xdd:
                    case 0xde:
                    case 0xdf:

                        #endregion

                        length += 2;
                        sourceSpan = sourceSpan.Slice(2);
                        break;

                    #region 3 byte

                    case 0xe0:
                    case 0xe1:
                    case 0xe2:
                    case 0xe3:
                    case 0xe4:
                    case 0xe5:
                    case 0xe6:
                    case 0xe7:
                    case 0xe8:
                    case 0xe9:
                    case 0xea:
                    case 0xeb:
                    case 0xec:
                    case 0xed:
                    case 0xee:
                    case 0xef:

                        #endregion

                        length += 3;
                        sourceSpan = sourceSpan.Slice(3);
                        break;

                    #region 4 byte

                    case 0xf0:
                    case 0xf1:
                    case 0xf2:
                    case 0xf3:
                    case 0xf4:

                        #endregion

                        length += 4;
                        sourceSpan = sourceSpan.Slice(4);
                        break;

                    #region Invalid

                    case 0x80:
                    case 0x81:
                    case 0x82:
                    case 0x83:
                    case 0x84:
                    case 0x85:
                    case 0x86:
                    case 0x87:
                    case 0x88:
                    case 0x89:
                    case 0x8a:
                    case 0x8b:
                    case 0x8c:
                    case 0x8d:
                    case 0x8e:
                    case 0x8f:
                    case 0x90:
                    case 0x91:
                    case 0x92:
                    case 0x93:
                    case 0x94:
                    case 0x95:
                    case 0x96:
                    case 0x97:
                    case 0x98:
                    case 0x99:
                    case 0x9a:
                    case 0x9b:
                    case 0x9c:
                    case 0x9d:
                    case 0x9e:
                    case 0x9f:
                    case 0xa0:
                    case 0xa1:
                    case 0xa2:
                    case 0xa3:
                    case 0xa4:
                    case 0xa5:
                    case 0xa6:
                    case 0xa7:
                    case 0xa8:
                    case 0xa9:
                    case 0xaa:
                    case 0xab:
                    case 0xac:
                    case 0xad:
                    case 0xae:
                    case 0xaf:
                    case 0xb0:
                    case 0xb1:
                    case 0xb2:
                    case 0xb3:
                    case 0xb4:
                    case 0xb5:
                    case 0xb6:
                    case 0xb7:
                    case 0xb8:
                    case 0xb9:
                    case 0xba:
                    case 0xbb:
                    case 0xbc:
                    case 0xbd:
                    case 0xbe:
                    case 0xbf:
                    case 0xc0:
                    case 0xc1:
                    case 0xf5:
                    case 0xf6:
                    case 0xf7:
                    case 0xf8:
                    case 0xf9:
                    case 0xfa:
                    case 0xfb:
                    case 0xfc:
                    case 0xfd:
                    case 0xfe:
                    case 0xff:

                        #endregion

                        throw new JsonParsingException("Invalid UTF8 character. : " + sourceSpan[0].ToString(CultureInfo.InvariantCulture));
                }
            } while (!sourceSpan.IsEmpty);
            return (length, escaped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ToNumber(uint b)
        {
            switch (b)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return b - 48;
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return b - 55;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return b - 87;
                default:
                    throw new JsonParsingException("Invalid Hex Character.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char From4HexToChar(byte b0, byte b1, byte b2, byte b3)
        {
            return (char)((ToNumber(b0) << 12) | (ToNumber(b1) << 8) | (ToNumber(b2) << 4) | ToNumber(b3));
        }

#if SPAN_BUILTIN
        private static void NoAction(Span<char> span, int arg)
        {
        }

        private static void DecodeFillEscapedBytes(Span<char> span, ReadOnlySpan<byte> sourceSpan)
        {
            var index = 0;
            var utf8 = StringEncoding.Utf8;

            while (index < sourceSpan.Length)
            {
                var b = sourceSpan[index];
                if (b != '\\')
                {
                    index++;
                    continue;
                }

                span = span.Slice(utf8.GetChars(sourceSpan.Slice(0, index), span));
                sourceSpan = sourceSpan.Slice(index + 1);
                index = 0;
                switch (sourceSpan[0])
                {
                    case (byte)'b':
                        span[0] = '\b';
                        span = span.Slice(1);
                        sourceSpan = sourceSpan.Slice(1);
                        break;
                    case (byte)'f':
                        span[0] = '\f';
                        span = span.Slice(1);
                        sourceSpan = sourceSpan.Slice(1);
                        break;
                    case (byte)'n':
                        span[0] = '\n';
                        span = span.Slice(1);
                        sourceSpan = sourceSpan.Slice(1);
                        break;
                    case (byte)'r':
                        span[0] = '\r';
                        span = span.Slice(1);
                        sourceSpan = sourceSpan.Slice(1);
                        break;
                    case (byte)'t':
                        span[0] = '\t';
                        span = span.Slice(1);
                        sourceSpan = sourceSpan.Slice(1);
                        break;
                    case (byte)'u':
                        span[0] = From4HexToChar(sourceSpan[1], sourceSpan[2], sourceSpan[3], sourceSpan[4]);
                        span = span.Slice(1);
                        sourceSpan = sourceSpan.Slice(5);
                        break;
                    case (byte)'"':
                    case (byte)'/':
                    case (byte)'\\':
                    default:
                        index++;
                        break;
                }
            }

            if (!sourceSpan.IsEmpty)
            {
                utf8.GetChars(sourceSpan, span);
            }
        }
#else
        private static unsafe void CreateStringSpanActionFromEscapedBytes(byte* sourcePtr, int sourceLength, char* destinationPtr, int destinationLength)
        {
            var index = 0;
            var utf8 = StringEncoding.Utf8;

            while (index < sourceLength)
            {
                var b = sourcePtr[index];
                if (b != '\\')
                {
                    index++;
                    continue;
                }

                var advance = utf8.GetChars(sourcePtr, index, destinationPtr, destinationLength);
                destinationPtr += advance;
                destinationLength -= advance;

                sourceLength -= index + 1;
                sourcePtr += index + 1;
                index = 0;

                switch (*sourcePtr)
                {
                    case (byte)'b':
                        *destinationPtr++ = '\b';
                        destinationLength--;
                        sourcePtr++;
                        sourceLength--;
                        break;
                    case (byte)'f':
                        *destinationPtr++ = '\f';
                        destinationLength--;
                        sourcePtr++;
                        sourceLength--;
                        break;
                    case (byte)'n':
                        *destinationPtr++ = '\n';
                        destinationLength--;
                        sourcePtr++;
                        sourceLength--;
                        break;
                    case (byte)'r':
                        *destinationPtr++ = '\r';
                        destinationLength--;
                        sourcePtr++;
                        sourceLength--;
                        break;
                    case (byte)'t':
                        *destinationPtr++ = '\t';
                        destinationLength--;
                        sourcePtr++;
                        sourceLength--;
                        break;
                    case (byte)'u':
                        *destinationPtr++ = From4HexToChar(*++sourcePtr, *++sourcePtr, *++sourcePtr, *++sourcePtr);
                        destinationLength--;
                        sourceLength--;
                        break;
                    case (byte)'"':
                    case (byte)'/':
                    case (byte)'\\':
                    default:
                        index++;
                        break;
                }
            }

            if (sourceLength != 0)
            {
                utf8.GetChars(sourcePtr, sourceLength, destinationPtr, destinationLength);
            }
        }
#endif
    }
}