// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Utf8Json.Internal;
using Utf8Json.Internal.DoubleConversion;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json
{
    public static class JsonWriterExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, float value)
        {
            DoubleToStringConverter.GetBytes(ref writer.Writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, double value)
        {
            DoubleToStringConverter.GetBytes(ref writer.Writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, byte value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, ushort value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, uint value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, ulong value)
        {
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, sbyte value)
        {
            int value1 = value;
            if (value < 0)
            {
                writer.Writer.GetPointer(1) = (byte)'-';
                writer.Writer.Advance(1);
                value1 = -value1;
            }

            writer.Write((uint)value1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, short value)
        {
            int value1 = value;

            if (value1 < 0)
            {
                writer.Writer.GetPointer(1) = (byte)'-';
                writer.Writer.Advance(1);
                value1 = unchecked(-value1);
            }

            writer.Write((uint)value1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, int value)
        {
            if (value < 0)
            {
                if (value == int.MinValue)
                {
                    // -2147483648
                    const int size = 11;
                    var span = writer.Writer.GetSpan(size);
                    span[0] = (byte)'-';
                    span[1] = (byte)'2';
                    span[2] = (byte)'1';
                    span[3] = (byte)'4';
                    span[4] = (byte)'7';
                    span[5] = (byte)'4';
                    span[6] = (byte)'8';
                    span[7] = (byte)'3';
                    span[8] = (byte)'6';
                    span[9] = (byte)'4';
                    span[10] = (byte)'8';
                    writer.Writer.Advance(size);
                    return;
                }

                writer.Writer.GetPointer(1) = (byte)'-';
                writer.Writer.Advance(1);
                value = unchecked(-value);
            }

            writer.Write((uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, long value)
        {
            if (value < 0)
            {
                if (value == long.MinValue)
                {
                    // -9223372036854775808
                    const int size = 20;
                    var span = writer.Writer.GetSpan(size);
                    span[0] = (byte)'-';
                    span[1] = (byte)'9';
                    span[2] = (byte)'2';
                    span[3] = (byte)'2';
                    span[4] = (byte)'3';
                    span[5] = (byte)'3';
                    span[6] = (byte)'7';
                    span[7] = (byte)'2';
                    span[8] = (byte)'0';
                    span[9] = (byte)'3';
                    span[10] = (byte)'6';
                    span[11] = (byte)'8';
                    span[12] = (byte)'5';
                    span[13] = (byte)'4';
                    span[14] = (byte)'7';
                    span[15] = (byte)'7';
                    span[16] = (byte)'5';
                    span[17] = (byte)'8';
                    span[18] = (byte)'0';
                    span[19] = (byte)'8';
                    writer.Writer.Advance(size);
                    return;
                }

                writer.Writer.GetPointer(1) = (byte)'-';
                writer.Writer.Advance(1);
                value = unchecked(-value);
            }

            writer.Write((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, DateTime value)
        {
            DateTimeConverter.Write(ref writer.Writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, decimal value)
        {
            writer.Writer.Write((ReadOnlySpan<byte>)StringEncoding.Utf8.GetBytes(value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
