// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel
#pragma warning disable IDE0057

namespace Utf8Json
{
    public unsafe ref struct JsonWriter
    {
        /// <summary>
        /// The writer to use.
        /// </summary>
        internal BufferWriter Writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> struct.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        public JsonWriter(IBufferWriter<byte> writer)
            : this()
        {
            Writer = new BufferWriter(writer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> struct.
        /// </summary>
        /// <param name="sequencePool">The pool from which to draw an <see cref="IBufferWriter{T}"/> if required..</param>
        /// <param name="array">An array to start with so we can avoid accessing the <paramref name="sequencePool"/> if possible.</param>
        internal JsonWriter(SequencePool sequencePool, byte[] array)
            : this()
        {
            Writer = new BufferWriter(sequencePool, array);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> struct,
        /// with the same settings as this one, but with its own buffer writer.
        /// </summary>
        /// <param name="writer">The writer to use for the new instance.</param>
        /// <returns>The new writer.</returns>
        public JsonWriter Clone(IBufferWriter<byte> writer) => new JsonWriter(writer);

        /// <summary>
        /// Ensures everything previously written has been flushed to the underlying <see cref="IBufferWriter{T}"/>.
        /// </summary>
        public void Flush() => Writer.Commit();

        internal byte[] FlushAndGetArray()
        {
            if (this.Writer.TryGetUncommittedSpan(out var span))
            {
                return span.ToArray();
            }

            if (this.Writer.SequenceRental.Value == null)
            {
                throw new NotSupportedException("This instance was not initialized to support this operation.");
            }

            this.Flush();
            var result = ((ReadOnlySequence<byte>)this.Writer.SequenceRental.Value).ToArray();
            this.Writer.SequenceRental.Dispose();
            return result;
        }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        public void WriteNull()
        {
            var span = Writer.GetSpan(4);
            span[0] = (byte)'n';
            span[1] = (byte)'u';
            span[2] = (byte)'l';
            span[3] = (byte)'l';
            Writer.Advance(4);
        }

        /// <summary>
        /// Copies bytes directly into the message pack writer.
        /// </summary>
        /// <param name="rawJsonBlock">The span of bytes to copy from.</param>
        public void WriteRaw(ReadOnlySpan<byte> rawJsonBlock) => Writer.Write(rawJsonBlock);

        /// <summary>:</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNameSeparator()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)':';
            Writer.Advance(1);
        }

        /// <summary>,</summary>
        public void WriteValueSeparator()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)',';
            Writer.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginArray()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)'[';
            Writer.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndArray()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)']';
            Writer.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginObject()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)'{';
            Writer.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndObject()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)'}';
            Writer.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteQuotation()
        {
            var span = Writer.GetSpan(1);
            span[0] = (byte)'"';
            Writer.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value)
        {
            if (value)
            {
                var span = Writer.GetSpan(4);
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
                Writer.Advance(4);
            }
            else
            {
                var span = Writer.GetSpan(5);
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
                Writer.Advance(5);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char value)
        {
            switch (value)
            {
                case '"':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'"';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                case '\\':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'\\';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                case '\b':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'b';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                case '\f':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'f';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                case '\n':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'n';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                case '\r':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'r';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                case '\t':
                    {
                        var span = Writer.GetSpan(4);
                        span[0] = (byte)'"';
                        span[1] = (byte)'\\';
                        span[2] = (byte)'t';
                        span[3] = (byte)'"';
                        Writer.Advance(4);
                    }
                    break;
                default:
                    {
                        var count = StringEncoding.Utf8.GetByteCount(&value, 1);
                        var span = Writer.GetSpan(count + 2);
                        span[0] = (byte)'"';
                        fixed (byte* dest = &span[1])
                        {
                            StringEncoding.Utf8.GetBytes(&value, 1, dest, count);
                        }
                        span[count + 1] = (byte)'"';
                        Writer.Advance(count + 2);
                    }
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            var buffer = default(Span<byte>);
            var offset = 0;
            if (value < 10000)
            {
                if (value < 10)
                {
                    buffer = Writer.GetSpan(1);
                    goto L1;
                }
                if (value < 100)
                {
                    buffer = Writer.GetSpan(2);
                    goto L2;
                }
                if (value < 1000)
                {
                    buffer = Writer.GetSpan(3);
                    goto L3;
                }
                buffer = Writer.GetSpan(4);
                goto L4;
            }
            var num2 = value / 10000;
            value -= num2 * 10000;
            if (num2 < 10000)
            {
                if (num2 < 10)
                {
                    buffer = Writer.GetSpan(5);
                    goto L5;
                }
                if (num2 < 100)
                {
                    buffer = Writer.GetSpan(6);
                    goto L6;
                }
                if (num2 < 1000)
                {
                    buffer = Writer.GetSpan(7);
                    goto L7;
                }
                buffer = Writer.GetSpan(8);
                goto L8;
            }
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10000)
            {
                if (num3 < 10)
                {
                    buffer = Writer.GetSpan(9);
                    goto L9;
                }
                if (num3 < 100)
                {
                    buffer = Writer.GetSpan(10);
                    goto L10;
                }
                if (num3 < 1000)
                {
                    buffer = Writer.GetSpan(11);
                    goto L11;
                }
                buffer = Writer.GetSpan(12);
                goto L12;
            }

            var num4 = num3 / 10000;
            num3 -= num4 * 10000;
            if (num4 < 10000)
            {
                if (num4 < 10)
                {
                    buffer = Writer.GetSpan(13);
                    goto L13;
                }
                if (num4 < 100)
                {
                    buffer = Writer.GetSpan(14);
                    goto L14;
                }
                if (num4 < 1000)
                {
                    buffer = Writer.GetSpan(15);
                    goto L15;
                }
                buffer = Writer.GetSpan(16);
                goto L16;
            }

            var num5 = num4 / 10000;
            num4 -= num5 * 10000;
            if (num5 < 10000)
            {
                if (num5 < 10)
                {
                    buffer = Writer.GetSpan(17);
                    goto L17;
                }
                if (num5 < 100)
                {
                    buffer = Writer.GetSpan(18);
                    goto L18;
                }
                if (num5 < 1000)
                {
                    buffer = Writer.GetSpan(19);
                    goto L19;
                }
                buffer = Writer.GetSpan(20);
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
            Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> value)
        {
            var span = Writer.GetSpan(value.Length * 3 + 2);
            span[0] = (byte)'"';
            span = span.Slice(1);
            var actualLength = 2;

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
                span = span.Slice(consumed);
            }

            span[0] = (byte)'"';
            Writer.Advance(actualLength);
        }
        /*

            
            

        private void WriteLargeString(in ReadOnlySpan<char> value)
        {
            var span = Writer.GetSpan(1024 * 24 * 3 + 2);
            for (var i = 0; i < value.Length; i++)
            {
                byte escapedChar;
                switch (value[i])
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
                        i++;
                        continue;
                }
            }
        }
        */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePropertyName(string value)
        {
            if (ReferenceEquals(value, ""))
            {
                var emptySpan = Writer.GetSpan(3);
                emptySpan[0] = (byte)'"';
                emptySpan[1] = (byte)'"';
                emptySpan[2] = (byte)':';
                Writer.Advance(3);
                return;
            }

            var max = (value.Length + 1) * 3;
            var span = Writer.GetSpan(max);
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
                span = Writer.GetSpan(max);
#if SPAN_BUILTIN
                offset += StringEncoding.Utf8.GetBytes(valueSpan.Slice(from, i - from), span.Slice(offset));
#else
                fixed (char* srcPtr = &valueSpan[from])
                fixed (byte* dstPtr = &span[offset])
                {
                    offset += StringEncoding.Utf8.GetBytes(srcPtr, i - from, dstPtr, span.Length - offset);
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
                fixed (char* srcPtr = &valueSpan[from])
                fixed (byte* dstPtr = &span[offset])
                {
                    offset += StringEncoding.Utf8.GetBytes(srcPtr, value.Length - from, dstPtr, span.Length - offset);
                }
#endif
            }

            span[offset++] = (byte)'"';
            span[offset++] = (byte)':';
            Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if CSHARP_8_OR_NEWER
        public void Write(string? value)
#else
        public void Write(string value)
#endif
        {
            if (value is null)
            {
                var span = Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                Writer.Advance(4);
                return;
            }

            if (value.Length == 0)
            {
                var emptySpan = Writer.GetSpan(2);
                emptySpan[0] = 0x22;
                emptySpan[1] = 0x22;
                Writer.Advance(2);
                return;
            }

            Write(value.AsSpan());
        }
    }
}
