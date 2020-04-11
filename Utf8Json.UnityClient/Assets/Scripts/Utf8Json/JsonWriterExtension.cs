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
            long value1 = value;
            if (value < 0)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)'-';
                writer.Writer.Advance(1);
                value1 = unchecked(-value1);
            }

            writer.Write((ulong)value1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, short value)
        {
            long value1 = value;

            if (value1 < 0)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)'-';
                writer.Writer.Advance(1);
                value1 = unchecked(-value1);
            }

            writer.Write((ulong)value1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, int value)
        {
            long value1 = value;

            if (value1 < 0)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)'-';
                writer.Writer.Advance(1);
                value1 = unchecked(-value1);
            }

            writer.Write((ulong)value1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, long value)
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

            if (value < 0)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)'-';
                writer.Writer.Advance(1);
                value = unchecked(-value);
            }

            writer.Write((ulong)value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                Unsafe.As<byte, ushort>(ref writer.Writer.GetPointer(2)) = 0x2222;
                writer.Writer.Advance(2);
                return;
            }

            var max = value.Length * 3 + 2;
            var span = writer.Writer.GetSpan(max);
            span[0] = (byte)'\"';
            var from = 0;
            var offset = 1;
            for (var i = 0; i < value.Length; i++)
            {
                byte escapeChar;
                switch (value[i])
                {
                    case '"':
                        escapeChar = (byte)'"';
                        break;
                    case '\\':
                        escapeChar = (byte)'\\';
                        break;
                    case '\b':
                        escapeChar = (byte)'b';
                        break;
                    case '\f':
                        escapeChar = (byte)'f';
                        break;
                    case '\n':
                        escapeChar = (byte)'n';
                        break;
                    case '\r':
                        escapeChar = (byte)'r';
                        break;
                    case '\t':
                        escapeChar = (byte)'t';
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
                        continue;
                }

                max += 2;
                span = writer.Writer.GetSpan(max);
#if SPAN_BUILTIN
                offset += StringEncoding.Utf8.GetBytes(value.Slice(from, i - from), span.Slice(offset));
#else
                unsafe
                {
                    fixed (char* srcPtr = &value[from])
                    fixed (byte* dstPtr = &span[offset])
                    {
                        offset += StringEncoding.Utf8.GetBytes(srcPtr, i - from, dstPtr, span.Length - offset);
                    }
                }
#endif
                from = i + 1;
                span[offset++] = (byte)'\\';
                span[offset++] = escapeChar;
            }

            if (from != value.Length)
            {
#if SPAN_BUILTIN
                offset += StringEncoding.Utf8.GetBytes(value.Slice(from), span.Slice(offset));
#else
                unsafe
                {
                    fixed (char* srcPtr = &value[from])
                    fixed (byte* dstPtr = &span[offset])
                    {
                        offset += StringEncoding.Utf8.GetBytes(srcPtr, value.Length - from, dstPtr, span.Length - offset);
                    }
                }
#endif
            }

            span[offset++] = (byte)'\"';
            writer.Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WritePropertyName(ref this JsonWriter writer, string value)
        {
            if (ReferenceEquals(value, ""))
            {
                var emptySpan = writer.Writer.GetSpan(3);
                emptySpan[0] = (byte)'"';
                emptySpan[1] = (byte)'"';
                emptySpan[2] = (byte)':';
                writer.Writer.Advance(3);
                return;
            }

            var max = (value.Length + 1) * 3;
            var span = writer.Writer.GetSpan(max);
            span[0] = (byte)'\"';
            var from = 0;
            var offset = 1;
            var valueSpan = value.AsSpan();
            for (var i = 0; i < value.Length; i++)
            {
                byte escapeChar;
                switch (value[i])
                {
                    case '"':
                        escapeChar = (byte)'"';
                        break;
                    case '\\':
                        escapeChar = (byte)'\\';
                        break;
                    case '\b':
                        escapeChar = (byte)'b';
                        break;
                    case '\f':
                        escapeChar = (byte)'f';
                        break;
                    case '\n':
                        escapeChar = (byte)'n';
                        break;
                    case '\r':
                        escapeChar = (byte)'r';
                        break;
                    case '\t':
                        escapeChar = (byte)'t';
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
                        continue;
                }

                max += 2;
                span = writer.Writer.GetSpan(max);
#if SPAN_BUILTIN
                offset += StringEncoding.Utf8.GetBytes(valueSpan.Slice(from, i - from), span.Slice(offset));
#else
                unsafe
                {
                    fixed (char* srcPtr = &valueSpan[from])
                    fixed (byte* dstPtr = &span[offset])
                    {
                        offset += StringEncoding.Utf8.GetBytes(srcPtr, i - from, dstPtr, span.Length - offset);
                    }
                }
#endif
                from = i + 1;
                span[offset++] = (byte)'\\';
                span[offset++] = escapeChar;
            }

            if (from != value.Length)
            {
#if SPAN_BUILTIN
                offset += StringEncoding.Utf8.GetBytes(valueSpan.Slice(from), span.Slice(offset));
#else
                unsafe
                {
                    fixed (char* srcPtr = &valueSpan[from])
                    fixed (byte* dstPtr = &span[offset])
                    {
                        offset += StringEncoding.Utf8.GetBytes(srcPtr, value.Length - from, dstPtr, span.Length - offset);
                    }
                }
#endif
            }

            span[offset++] = (byte)'"';
            span[offset++] = (byte)':';
            writer.Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if CSHARP_8_OR_NEWER
        public static void Write(ref this JsonWriter writer, string? value)
#else
        public static void Write(ref this JsonWriter writer, string value)
#endif
        {
            if (ReferenceEquals(value, null))
            {
                writer.WriteNull();
                return;
            }

            Write(ref writer, value.AsSpan());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, DateTime value)
        {
            DateTimeConverter.Write(ref writer.Writer, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(ref this JsonWriter writer, decimal value)
        {
            writer.WriteRaw(StringEncoding.Utf8.GetBytes(value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}