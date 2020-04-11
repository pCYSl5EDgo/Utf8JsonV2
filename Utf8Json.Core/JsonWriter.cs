// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using Utf8Json.Internal;

namespace Utf8Json
{
    public ref struct JsonWriter
    {
        /// <summary>
        /// The writer to use.
        /// </summary>
        internal BufferWriter Writer;

        /// <summary>
        /// Gets or sets the cancellation token for this serialization operation.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

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
        public JsonWriter Clone(IBufferWriter<byte> writer) => new JsonWriter(writer)
        {
            CancellationToken = this.CancellationToken,
        };

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

        /// <summary>
        /// Copies bytes directly into the message pack writer.
        /// </summary>
        /// <param name="rawJsonBlock">The span of bytes to copy from.</param>
        public void WriteRaw(in ReadOnlySequence<byte> rawJsonBlock)
        {
            foreach (var segment in rawJsonBlock)
            {
                Writer.Write(segment.Span);
            }
        }

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
        public unsafe void Write(char value)
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
    }
}
