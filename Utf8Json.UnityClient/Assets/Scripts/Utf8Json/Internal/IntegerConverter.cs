// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Internal
{
    /// <summary>
    /// zero-allocate itoa, atoi converters.
    /// </summary>
    public static class IntegerConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(byte c)
        {
            return (byte)'0' <= c && c <= (byte)'9';
        }

        #region CalcSize
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcSize(int value)
        {
            if (value >= 0) return CalcSize((uint)value);
            return value == int.MinValue ? 11 : CalcSize((uint)-value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcSize(long value)
        {
            if (value >= 0) return CalcSize((ulong)value);
            return value == long.MinValue ? 20 : CalcSize((ulong)-value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcSize(uint value)
        {
            if (value < 1000)
            {
                if (value < 10) return 1;
                return value < 100 ? 2 : 3;
            }

            if (value < 1000_000)
            {
                if (value < 1000_0) return 4;
                return value < 1000_00 ? 5 : 6;
            }

            if (value < 1000_000_000U)
            {
                if (value < 1000_000_0) return 7;
                return value < 1000_000_00 ? 8 : 9;
            }

            return 10;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcSize(ulong value)
        {
            if (value >> 32 == 0)
            {
                return CalcSize((uint)value);
            }

            if (value < 1000_000_000_000UL)
            {
                if (value < 1000_000_000_0) return 10;
                return value < 1000_000_000_00 ? 11 : 12;
            }

            if (value < 1000_000_000_000_000UL)
            {
                if (value < 1000_000_000_000_0UL) return 13;
                return value < 1000_000_000_000_00UL ? 14 : 15;
            }

            if (value < 1000_000_000_000_000_000UL)
            {
                if (value < 1000_000_000_000_000_0UL) return 16;
                return value < 1000_000_000_000_000_00UL ? 17 : 18;
            }

            return value < 1000_000_000_000_000_000_0UL ? 19 : 20;
        }
        #endregion

        #region Write
        #region UInt64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref BufferWriter writer, ulong value)
        {
            var buffer = default(Span<byte>);
            var offset = 0;
            if (value < 10000)
            {
                if (value < 10)
                {
                    buffer = writer.GetSpan(1);
                    goto L1;
                }
                if (value < 100)
                {
                    buffer = writer.GetSpan(2);
                    goto L2;
                }
                if (value < 1000)
                {
                    buffer = writer.GetSpan(3);
                    goto L3;
                }
                buffer = writer.GetSpan(4);
                goto L4;
            }
            var num2 = value / 10000;
            value -= num2 * 10000;
            if (num2 < 10000)
            {
                if (num2 < 10)
                {
                    buffer = writer.GetSpan(5);
                    goto L5;
                }
                if (num2 < 100)
                {
                    buffer = writer.GetSpan(6);
                    goto L6;
                }
                if (num2 < 1000)
                {
                    buffer = writer.GetSpan(7);
                    goto L7;
                }
                buffer = writer.GetSpan(8);
                goto L8;
            }
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10000)
            {
                if (num3 < 10)
                {
                    buffer = writer.GetSpan(9);
                    goto L9;
                }
                if (num3 < 100)
                {
                    buffer = writer.GetSpan(10);
                    goto L10;
                }
                if (num3 < 1000)
                {
                    buffer = writer.GetSpan(11);
                    goto L11;
                }
                buffer = writer.GetSpan(12);
                goto L12;
            }

            var num4 = num3 / 10000;
            num3 -= num4 * 10000;
            if (num4 < 10000)
            {
                if (num4 < 10)
                {
                    buffer = writer.GetSpan(13);
                    goto L13;
                }
                if (num4 < 100)
                {
                    buffer = writer.GetSpan(14);
                    goto L14;
                }
                if (num4 < 1000)
                {
                    buffer = writer.GetSpan(15);
                    goto L15;
                }
                buffer = writer.GetSpan(16);
                goto L16;
            }

            var num5 = num4 / 10000;
            num4 -= num5 * 10000;
            if (num5 < 10000)
            {
                if (num5 < 10)
                {
                    buffer = writer.GetSpan(17);
                    goto L17;
                }
                if (num5 < 100)
                {
                    buffer = writer.GetSpan(18);
                    goto L18;
                }
                if (num5 < 1000)
                {
                    buffer = writer.GetSpan(19);
                    goto L19;
                }
                buffer = writer.GetSpan(20);
            }
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
            buffer[offset++] = (byte)('0' + (num5));
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
            buffer[offset++] = (byte)('0' + (num4));
        L12:
            buffer[offset++] = (byte)('0' + (div = (num3 * 8389UL) >> 23));
            num3 -= div * 1000;
        L11:
            buffer[offset++] = (byte)('0' + (div = (num3 * 5243UL) >> 19));
            num3 -= div * 100;
        L10:
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554UL) >> 16));
            num3 -= div * 10;
        L9:
            buffer[offset++] = (byte)('0' + (num3));
        L8:
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389UL) >> 23));
            num2 -= div * 1000;
        L7:
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243UL) >> 19));
            num2 -= div * 100;
        L6:
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554UL) >> 16));
            num2 -= div * 10;
        L5:
            buffer[offset++] = (byte)('0' + (num2));
        L4:
            buffer[offset++] = (byte)('0' + (div = (value * 8389UL) >> 23));
            value -= div * 1000;
        L3:
            buffer[offset++] = (byte)('0' + (div = (value * 5243UL) >> 19));
            value -= div * 100;
        L2:
            buffer[offset++] = (byte)('0' + (div = (value * 6554UL) >> 16));
            value -= div * 10;
        L1:
            buffer[offset++] = (byte)('0' + value);
            writer.Advance(offset);
        }
        #endregion
        #endregion

        #region Read
        #region UInt32
        /// <summary>
        /// 
        /// </summary>
        /// <param name="span">Not Empty Span</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryRead(ReadOnlySpan<byte> span, out uint value)
        {
            uint answer = default;
            foreach (var b in span)
            {
                switch (b)
                {
                    case (byte)'0':
                        unchecked
                        {
                            answer *= 10;
                        }
                        break;
                    case (byte)'1':
                    case (byte)'2':
                    case (byte)'3':
                    case (byte)'4':
                    case (byte)'5':
                    case (byte)'6':
                    case (byte)'7':
                    case (byte)'8':
                    case (byte)'9':
                        answer = unchecked(answer * 10 + (uint)(b - 48));
                        break;
                    default:
                        value = default;
                        return false;
                }
            }

            value = answer;
            return true;
        }
        #endregion

        #region UInt64
        public static bool TryRead(ReadOnlySpan<byte> span, out ulong value)
        {
            ulong answer = default;
            foreach (var b in span)
            {
                switch (b)
                {
                    case (byte)'0':
                        unchecked
                        {
                            answer *= 10;
                        }
                        break;
                    case (byte)'1':
                    case (byte)'2':
                    case (byte)'3':
                    case (byte)'4':
                    case (byte)'5':
                    case (byte)'6':
                    case (byte)'7':
                    case (byte)'8':
                    case (byte)'9':
                        answer = unchecked(answer * 10 + (ulong)(b - 48));
                        break;
                    default:
                        value = default;
                        return false;
                }
            }

            value = answer;
            return true;
        }
        #endregion

        #region Int32
        public static bool TryRead(ReadOnlySpan<byte> span, out int value)
        {
            var isNegative = span[0] == '-';
            if (isNegative)
            {
                span = span.Slice(1);
                if (span.Length == 10)
                {
                }
            }

            int answer = default;
            foreach (var b in span)
            {
                switch (b)
                {
                    case (byte)'0':
                        unchecked
                        {
                            answer *= 10;
                        }
                        break;
                    case (byte)'1':
                    case (byte)'2':
                    case (byte)'3':
                    case (byte)'4':
                    case (byte)'5':
                    case (byte)'6':
                    case (byte)'7':
                    case (byte)'8':
                    case (byte)'9':
                        answer = unchecked(answer * 10 - 48 + b);
                        break;
                    default:
                        value = default;
                        return false;
                }
            }

            value = isNegative ? -answer : answer;
            return true;
        }
        #endregion

        #region Int64
        public static bool TryRead(ReadOnlySpan<byte> span, out long value)
        {
            var isNegative = span[0] == '-';
            if (isNegative)
            {
                span = span.Slice(1);
            }

            long answer = default;
            foreach (var b in span)
            {
                switch (b)
                {
                    case (byte)'0':
                        unchecked
                        {
                            answer *= 10;
                        }
                        break;
                    case (byte)'1':
                    case (byte)'2':
                    case (byte)'3':
                    case (byte)'4':
                    case (byte)'5':
                    case (byte)'6':
                    case (byte)'7':
                    case (byte)'8':
                    case (byte)'9':
                        answer = unchecked(answer * 10 - 48 + b);
                        break;
                    default:
                        value = default;
                        return false;
                }
            }

            value = isNegative ? -answer : answer;
            return true;
        }
        #endregion
        #endregion
    }
}
