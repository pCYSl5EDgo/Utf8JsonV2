// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            byte b0;
            byte b1;
            switch (value)
            {
                case '\\': b0 = (byte)'\\'; goto ESCAPE;
                case '"': b0 = (byte)'"'; goto ESCAPE;
                case '\b': b0 = (byte)'b'; goto ESCAPE;
                case '\f': b0 = (byte)'f'; goto ESCAPE;
                case '\n': b0 = (byte)'n'; goto ESCAPE;
                case '\r': b0 = (byte)'r'; goto ESCAPE;
                case '\t': b0 = (byte)'t'; goto ESCAPE;
                case (char)0x00: b0 = (byte)'0'; b1 = (byte)'0'; goto HEX;
                case (char)0x01: b0 = (byte)'0'; b1 = (byte)'1'; goto HEX;
                case (char)0x02: b0 = (byte)'0'; b1 = (byte)'2'; goto HEX;
                case (char)0x03: b0 = (byte)'0'; b1 = (byte)'3'; goto HEX;
                case (char)0x04: b0 = (byte)'0'; b1 = (byte)'4'; goto HEX;
                case (char)0x05: b0 = (byte)'0'; b1 = (byte)'5'; goto HEX;
                case (char)0x06: b0 = (byte)'0'; b1 = (byte)'6'; goto HEX;
                case (char)0x07: b0 = (byte)'0'; b1 = (byte)'7'; goto HEX;
                case (char)0x0b: b0 = (byte)'0'; b1 = (byte)'b'; goto HEX;
                case (char)0x0e: b0 = (byte)'0'; b1 = (byte)'e'; goto HEX;
                case (char)0x0f: b0 = (byte)'0'; b1 = (byte)'f'; goto HEX;
                case (char)0x10: b0 = (byte)'1'; b1 = (byte)'0'; goto HEX;
                case (char)0x11: b0 = (byte)'1'; b1 = (byte)'1'; goto HEX;
                case (char)0x12: b0 = (byte)'1'; b1 = (byte)'2'; goto HEX;
                case (char)0x13: b0 = (byte)'1'; b1 = (byte)'3'; goto HEX;
                case (char)0x14: b0 = (byte)'1'; b1 = (byte)'4'; goto HEX;
                case (char)0x15: b0 = (byte)'1'; b1 = (byte)'5'; goto HEX;
                case (char)0x16: b0 = (byte)'1'; b1 = (byte)'6'; goto HEX;
                case (char)0x17: b0 = (byte)'1'; b1 = (byte)'7'; goto HEX;
                case (char)0x18: b0 = (byte)'1'; b1 = (byte)'8'; goto HEX;
                case (char)0x19: b0 = (byte)'1'; b1 = (byte)'9'; goto HEX;
                case (char)0x1a: b0 = (byte)'1'; b1 = (byte)'a'; goto HEX;
                case (char)0x1b: b0 = (byte)'1'; b1 = (byte)'b'; goto HEX;
                case (char)0x1c: b0 = (byte)'1'; b1 = (byte)'c'; goto HEX;
                case (char)0x1d: b0 = (byte)'1'; b1 = (byte)'d'; goto HEX;
                case (char)0x1e: b0 = (byte)'1'; b1 = (byte)'e'; goto HEX;
                case (char)0x1f: b0 = (byte)'1'; b1 = (byte)'f'; goto HEX;

                #region Other
                case (char)0x20:
                case (char)0x21:
                case (char)0x23:
                case (char)0x24:
                case (char)0x25:
                case (char)0x26:
                case (char)0x27:
                case (char)0x28:
                case (char)0x29:
                case (char)0x2a:
                case (char)0x2b:
                case (char)0x2c:
                case (char)0x2d:
                case (char)0x2e:
                case (char)0x2f:
                case (char)0x30:
                case (char)0x31:
                case (char)0x32:
                case (char)0x33:
                case (char)0x34:
                case (char)0x35:
                case (char)0x36:
                case (char)0x37:
                case (char)0x38:
                case (char)0x39:
                case (char)0x3a:
                case (char)0x3b:
                case (char)0x3c:
                case (char)0x3d:
                case (char)0x3e:
                case (char)0x3f:
                case (char)0x40:
                case (char)0x41:
                case (char)0x42:
                case (char)0x43:
                case (char)0x44:
                case (char)0x45:
                case (char)0x46:
                case (char)0x47:
                case (char)0x48:
                case (char)0x49:
                case (char)0x4a:
                case (char)0x4b:
                case (char)0x4c:
                case (char)0x4d:
                case (char)0x4e:
                case (char)0x4f:
                case (char)0x50:
                case (char)0x51:
                case (char)0x52:
                case (char)0x53:
                case (char)0x54:
                case (char)0x55:
                case (char)0x56:
                case (char)0x57:
                case (char)0x58:
                case (char)0x59:
                case (char)0x5a:
                case (char)0x5b:
                case (char)0x5d:
                case (char)0x5e:
                case (char)0x5f:
                case (char)0x60:
                case (char)0x61:
                case (char)0x62:
                case (char)0x63:
                case (char)0x64:
                case (char)0x65:
                case (char)0x66:
                case (char)0x67:
                case (char)0x68:
                case (char)0x69:
                case (char)0x6a:
                case (char)0x6b:
                case (char)0x6c:
                case (char)0x6d:
                case (char)0x6e:
                case (char)0x6f:
                case (char)0x70:
                case (char)0x71:
                case (char)0x72:
                case (char)0x73:
                case (char)0x74:
                case (char)0x75:
                case (char)0x76:
                case (char)0x77:
                case (char)0x78:
                case (char)0x79:
                case (char)0x7a:
                case (char)0x7b:
                case (char)0x7c:
                case (char)0x7d:
                case (char)0x7e:
                case (char)0x7f:
                    #endregion
                    {
                        const int sizeHint = 3;
                        var span = Writer.GetSpan(sizeHint);
                        span[0] = (byte)'"';
                        span[1] = (byte)value;
                        span[2] = (byte)'"';
                        Writer.Advance(sizeHint);
                    }
                    return;
            }

            if (value < 0x07ff)
            {
                const int sizeHint = 4;
                var span = Writer.GetSpan(sizeHint);
                span[0] = (byte)'"';
                span[1] = (byte)(uint)(0b110_00000 | (value >> 6));
                span[2] = (byte)(uint)(0b10_000000 | (value & 0b111111));
                span[3] = (byte)'"';
                Writer.Advance(sizeHint);
            }
            else
            {
                const int sizeHint = 5;
                var span = Writer.GetSpan(sizeHint);
                span[0] = (byte)'"';
                span[1] = (byte)(uint)(0b1110_0000 | (value >> 12));
                span[2] = (byte)(uint)(0b10_000000 | ((value >> 6) & 0b111111));
                span[3] = (byte)(uint)(0b10_000000 | (value & 0b111111));
                span[4] = (byte)'"';
                Writer.Advance(sizeHint);
            }
            return;

        ESCAPE:
            {
                const int sizeHint = 4;
                var span = Writer.GetSpan(sizeHint);
                span[0] = (byte)'"';
                span[1] = (byte)'\\';
                span[2] = b0;
                span[3] = (byte)'"';
                Writer.Advance(sizeHint);
            }
            return;

        HEX:
            {
                const int sizeHint = 8;
                var span = Writer.GetSpan(sizeHint);
                span[0] = (byte)'"';
                span[1] = (byte)'\\';
                span[2] = (byte)'u';
                span[3] = (byte)'0';
                span[4] = (byte)'0';
                span[5] = b0;
                span[6] = b1;
                span[7] = (byte)'"';
                Writer.Advance(sizeHint);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            Span<byte> buffer;
            var offset = 0;
            //4294967295 == uint.MaxValue 10 chars
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
            if (num3 < 10)
            {
                buffer = Writer.GetSpan(9);
                goto L9;
            }

            buffer = Writer.GetSpan(10);

            uint div;
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554U) >> 16));
            num3 -= div * 10U;
        L9:
            buffer[offset++] = (byte)('0' + num3);
        L8:
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389U) >> 23));
            num2 -= div * 1000U;
        L7:
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243U) >> 19));
            num2 -= div * 100U;
        L6:
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554U) >> 16));
            num2 -= div * 10U;
        L5:
            buffer[offset++] = (byte)('0' + num2);
        L4:
            buffer[offset++] = (byte)('0' + (div = (value * 8389U) >> 23));
            value -= div * 1000U;
        L3:
            buffer[offset++] = (byte)('0' + (div = (value * 5243U) >> 19));
            value -= div * 100U;
        L2:
            buffer[offset++] = (byte)('0' + (div = (value * 6554U) >> 16));
            value -= div * 10U;
        L1:
            buffer[offset++] = (byte)('0' + value);
            Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            if (value >> 32 == 0)
            {
                Write((uint)value);
                return;
            }

            Span<byte> buffer;
            var offset = 0;
            var value1 = value;
            var num2 = value1 / 10000;
            value1 -= num2 * 10000;
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            if (num3 < 10000)
            {
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
            buffer[offset++] = (byte)('0' + num5);
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
            buffer[offset++] = (byte)('0' + num4);
        L12:
            buffer[offset++] = (byte)('0' + (div = (num3 * 8389UL) >> 23));
            num3 -= div * 1000;
        L11:
            buffer[offset++] = (byte)('0' + (div = (num3 * 5243UL) >> 19));
            num3 -= div * 100;
        L10:
            buffer[offset++] = (byte)('0' + (div = (num3 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + num3 - div * 10);
            buffer[offset++] = (byte)('0' + (div = (num2 * 8389UL) >> 23));
            num2 -= div * 1000;
            buffer[offset++] = (byte)('0' + (div = (num2 * 5243UL) >> 19));
            num2 -= div * 100;
            buffer[offset++] = (byte)('0' + (div = (num2 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + num2 - div * 10);
            buffer[offset++] = (byte)('0' + (div = (value1 * 8389UL) >> 23));
            value1 -= div * 1000;
            buffer[offset++] = (byte)('0' + (div = (value1 * 5243UL) >> 19));
            value1 -= div * 100;
            buffer[offset++] = (byte)('0' + (div = (value1 * 6554UL) >> 16));
            buffer[offset++] = (byte)('0' + value1 - div * 10);
            Writer.Advance(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> value)
        {
            var span = Writer.GetSpan(value.Length * 6 + 2);
            span[0] = (byte)'"';
            span = span.Slice(1);
            var actualLength = 2;

            for (var index = 0; index < value.Length;)
            {
                byte escapedChar;
                byte escapedChar2;
                switch (value[index])
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
                continue;

            ESCAPE_HEX:
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
                span[1] = (byte)'u';
                span[2] = (byte)'0';
                span[3] = (byte)'0';
                span[4] = escapedChar;
                span[5] = escapedChar2;

                span = span.Slice(6);
                value = value.Slice(index + 1);
                index = 0;
                actualLength += 6;
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

            var value1 = value.AsSpan();
            var span = Writer.GetSpan(value1.Length * 6 + 3);
            span[0] = (byte)'"';
            span = span.Slice(1);
            var actualLength = 3;

            for (var index = 0; index < value1.Length;)
            {
                byte escapedChar;
                byte escapedChar2;
                switch (value1[index])
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
                    var consumed = StringEncoding.Utf8.GetBytes(value1.Slice(0, index), span);
#else
                    int consumed;
                    fixed (char* src = &value1[0])
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
                value1 = value1.Slice(index + 1);
                index = 0;
                actualLength += 2;
                continue;

            ESCAPE_HEX:
                if (index != 0)
                {
#if SPAN_BUILTIN
                    var consumed = StringEncoding.Utf8.GetBytes(value1.Slice(0, index), span);
#else
                    int consumed;
                    fixed (char* src = &value1[0])
                    fixed (byte* dst = &span[0])
                    {
                        consumed = StringEncoding.Utf8.GetBytes(src, index, dst, span.Length);
                    }
#endif
                    actualLength += consumed;
                    span = span.Slice(consumed);
                }

                span[0] = (byte)'\\';
                span[1] = (byte)'u';
                span[2] = (byte)'0';
                span[3] = (byte)'0';
                span[4] = escapedChar;
                span[5] = escapedChar2;

                span = span.Slice(6);
                value1 = value1.Slice(index + 1);
                index = 0;
                actualLength += 6;
            }

            if (!value1.IsEmpty)
            {
#if SPAN_BUILTIN
                var consumed = StringEncoding.Utf8.GetBytes(value1, span);
#else
                int consumed;
                fixed (char* src = &value1[0])
                fixed (byte* dst = &span[0])
                {
                    consumed = StringEncoding.Utf8.GetBytes(src, value1.Length, dst, span.Length);
                }
#endif
                actualLength += consumed;
                span = span.Slice(consumed);
            }

            span[0] = (byte)'"';
            span[1] = (byte)':';
            Writer.Advance(actualLength);
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
