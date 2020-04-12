// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Copyright (c) .NET Foundation. All rights reserved.
// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Utf8Json.Internal
{
    public ref struct BufferWriter
    {
        /// <summary>
        /// The underlying <see cref="IBufferWriter{T}"/>.
        /// </summary>
#if CSHARP_8_OR_NEWER
        private IBufferWriter<byte>? output;
#else
        private IBufferWriter<byte> output;
#endif

        /// <summary>
        /// The result of the last call to <see cref="IBufferWriter{T}.GetSpan(int)"/>, less any bytes already "consumed" with <see cref="Advance(int)"/>.
        /// Backing field for the <see cref="Span"/> property.
        /// </summary>
        private Span<byte> innerSpan;

        /// <summary>
        /// The result of the last call to <see cref="IBufferWriter{T}.GetMemory(int)"/>, less any bytes already "consumed" with <see cref="Advance(int)"/>.
        /// </summary>
        private ArraySegment<byte> segment;

        /// <summary>
        /// The number of uncommitted bytes (all the calls to <see cref="Advance(int)"/> since the last call to <see cref="Commit"/>).
        /// </summary>
        private int buffered;

        /// <summary>
        /// The total number of bytes written with this writer.
        /// Backing field for the <see cref="BytesCommitted"/> property.
        /// </summary>
        private long bytesCommitted;

#if CSHARP_8_OR_NEWER
        private SequencePool? sequencePool;
#else
        private SequencePool sequencePool;
#endif

        private SequencePool.Rental rental;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferWriter"/> struct.
        /// </summary>
        /// <param name="output">The <see cref="IBufferWriter{T}"/> to be wrapped.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BufferWriter(IBufferWriter<byte> output)
        {
            buffered = 0;
            bytesCommitted = 0;
            this.output = output;

            sequencePool = default;
            rental = default;

            var memory1 = this.output.GetMemory();
            if (memory1.IsEmpty)
            {
                throw new InvalidOperationException("The underlying IBufferWriter<byte>.GetMemory(int) method returned an empty memory block, which is not allowed. This is a bug in " + this.output.GetType().FullName);
            }
            var memory = memory1;
            MemoryMarshal.TryGetArray(memory, out segment);
            innerSpan = memory.Span;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferWriter"/> struct.
        /// </summary>
        /// <param name="sequencePool">The pool from which to draw an <see cref="IBufferWriter{T}"/> if required..</param>
        /// <param name="array">An array to start with so we can avoid accessing the <paramref name="sequencePool"/> if possible.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BufferWriter(SequencePool sequencePool, byte[] array)
        {
            buffered = 0;
            bytesCommitted = 0;
            this.sequencePool = sequencePool;
            rental = default;
            output = null;

            segment = new ArraySegment<byte>(array);
            innerSpan = segment.AsSpan();
        }

        /// <summary>
        /// Gets the result of the last call to <see cref="IBufferWriter{T}.GetSpan(int)"/>.
        /// </summary>
        public Span<byte> Span => innerSpan;

        /// <summary>
        /// Gets the total number of bytes written with this writer.
        /// </summary>
        public long BytesCommitted => bytesCommitted;

        /// <summary>
        /// Gets the <see cref="IBufferWriter{T}"/> underlying this instance.
        /// </summary>
#if CSHARP_8_OR_NEWER
        internal IBufferWriter<byte>? UnderlyingWriter => output;
#else
        internal IBufferWriter<byte> UnderlyingWriter => output;
#endif

        internal SequencePool.Rental SequenceRental => rental;

        public Span<byte> GetSpan(int sizeHint)
        {
            Ensure(sizeHint);
            return this.Span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetPointer(int sizeHint)
        {
            Ensure(sizeHint);

            if (segment.Array != null)
            {
                return ref segment.Array[segment.Offset + buffered];
            }
            else
            {
                return ref innerSpan.GetPinnableReference();
            }
        }

        /// <summary>
        /// Calls <see cref="IBufferWriter{T}.Advance(int)"/> on the underlying writer
        /// with the number of uncommitted bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Commit()
        {
            if (this.buffered <= 0)
            {
                return;
            }

            var oldBuffered = this.buffered;
            this.MigrateToSequence();

            bytesCommitted += oldBuffered;
            this.buffered = 0;
#if CSHARP_8_OR_NEWER
            output!.Advance(oldBuffered);
#else
            output.Advance(oldBuffered);
#endif
            innerSpan = default;
        }

        /// <summary>
        /// Used to indicate that part of the buffer has been written to.
        /// </summary>
        /// <param name="count">The number of bytes written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            buffered += count;
            innerSpan = innerSpan.Slice(count);
        }

        /// <summary>
        /// Copies the caller's buffer into this writer and calls <see cref="Advance(int)"/> with the length of the source buffer.
        /// </summary>
        /// <param name="source">The buffer to copy in.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<byte> source)
        {
            if (innerSpan.Length >= source.Length)
            {
                source.CopyTo(innerSpan);
                Advance(source.Length);
            }
            else
            {
                WriteMultiBuffer(source);
            }
        }

        /// <summary>
        /// Acquires a new buffer if necessary to ensure that some given number of bytes can be written to a single buffer.
        /// </summary>
        /// <param name="count">The number of bytes that must be allocated in a single buffer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Ensure(int count = 1)
        {
            if (innerSpan.Length < count)
            {
                EnsureMore(count);
            }
        }

        /// <summary>
        /// Gets the span to the bytes written if they were never committed to the underlying buffer writer.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        internal bool TryGetUncommittedSpan(out ReadOnlySpan<byte> span)
        {
            if (this.sequencePool != null)
            {
                span = segment.AsSpan(0, buffered);
                return true;
            }

            span = default;
            return false;
        }

        /// <summary>
        /// Gets a fresh span to write to, with an optional minimum size.
        /// </summary>
        /// <param name="count">The minimum size for the next requested buffer.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void EnsureMore(int count)
        {
            if (buffered > 0)
            {
                var oldBuffered = this.buffered;
                this.MigrateToSequence();

                bytesCommitted += oldBuffered;
                this.buffered = 0;
#if CSHARP_8_OR_NEWER
                output!.Advance(oldBuffered);
#else
                output.Advance(oldBuffered);
#endif
                innerSpan = default;
            }
            else
            {
                this.MigrateToSequence();
            }

#if CSHARP_8_OR_NEWER
            var memory = output!.GetMemory(count);
#else
            var memory = output.GetMemory(count);
#endif
            if (memory.IsEmpty)
            {
                throw new InvalidOperationException("The underlying IBufferWriter<byte>.GetMemory(int) method returned an empty memory block, which is not allowed. This is a bug in " + output.GetType().FullName);
            }
            MemoryMarshal.TryGetArray(memory, out segment);
            innerSpan = memory.Span;
        }

        /// <summary>
        /// Copies the caller's buffer into this writer, potentially across multiple buffers from the underlying writer.
        /// </summary>
        /// <param name="source">The buffer to copy into this writer.</param>
        private void WriteMultiBuffer(ReadOnlySpan<byte> source)
        {
            var copiedBytes = 0;
            var bytesLeftToCopy = source.Length;
            while (bytesLeftToCopy > 0)
            {
                if (innerSpan.Length == 0)
                {
                    EnsureMore(0);
                }

                var writable = Math.Min(bytesLeftToCopy, innerSpan.Length);
                source.Slice(copiedBytes, writable).CopyTo(innerSpan);
                copiedBytes += writable;
                bytesLeftToCopy -= writable;
                Advance(writable);
            }
        }

        private void MigrateToSequence()
        {
            if (this.sequencePool == null)
            {
                return;
            }

            // We were writing to our private scratch memory, so we have to copy it into the actual writer.
            rental = sequencePool.Rent();
            output = rental.Value;
            var realSpan = output.GetSpan(buffered);
            segment.AsSpan(0, buffered).CopyTo(realSpan);
            sequencePool = null;
        }
    }
}
