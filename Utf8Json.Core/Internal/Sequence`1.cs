﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/* Original license and copyright from file copied from https://github.com/AArnott/Nerdbank.Streams/blob/d656899be26d4d7c72c11c9232b4952c64a89bcb/src/Nerdbank.Streams/Sequence%601.cs
 * Copyright (c) Andrew Arnott. All rights reserved.
 * Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/

using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Utf8Json.Internal
{
    /// <summary>
    /// Manages a sequence of elements, readily cast-able as a <see cref="ReadOnlySequence{Byte}"/>.
    /// </summary>
    /// <remarks>
    /// Instance members are not thread-safe.
    /// </remarks>
    internal sealed class Sequence : IBufferWriter<byte>, IDisposable
    {
        private const int DefaultLengthFromArrayPool = 4096;

        private readonly Stack<SequenceSegment> segmentPool = new Stack<SequenceSegment>();

#if CSHARP_8_OR_NEWER
        private readonly MemoryPool<byte>? memoryPool;

        private readonly ArrayPool<byte>? arrayPool;

        private SequenceSegment? first;

        private SequenceSegment? last;
#else
        private readonly MemoryPool<byte> memoryPool;

        private readonly ArrayPool<byte> arrayPool;

        private SequenceSegment first;

        private SequenceSegment last;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class
        /// that uses a private <see cref="ArrayPool{T}"/> for recycling arrays.
        /// </summary>
        public Sequence()
            : this(ArrayPool<byte>.Create())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="memoryPool">The pool to use for recycling backing arrays.</param>
        public Sequence(MemoryPool<byte> memoryPool)
        {
            this.memoryPool = memoryPool;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="arrayPool">The pool to use for recycling backing arrays.</param>
        public Sequence(ArrayPool<byte> arrayPool)
        {
            this.arrayPool = arrayPool;
        }

        /// <summary>
        /// Gets or sets the minimum length for any array allocated as a segment in the sequence.
        /// Any non-positive value allows the pool to determine the length of the array.
        /// </summary>
        /// <value>The default value is 0.</value>
        /// <remarks>
        /// <para>
        /// Each time <see cref="GetSpan(int)"/> or <see cref="GetMemory(int)"/> is called,
        /// previously allocated memory is used if it is large enough to satisfy the length demand.
        /// If new memory must be allocated, the argument to one of these methods typically dictate
        /// the length of array to allocate. When the caller uses very small values (just enough for its immediate need)
        /// but the high level scenario can predict that a large amount of memory will be ultimately required,
        /// it can be advisable to set this property to a value such that just a few larger arrays are allocated
        /// instead of many small ones.
        /// </para>
        /// <para>
        /// The <see cref="MemoryPool{Byte}"/> in use may itself have a minimum array length as well,
        /// in which case the higher of the two minimums dictate the minimum array size that will be allocated.
        /// </para>
        /// </remarks>
        public int MinimumSpanLength { get; set; } = 0;

        /// <summary>
        /// Expresses this sequence as a <see cref="ReadOnlySequence{Byte}"/>.
        /// </summary>
        /// <param name="sequence">The sequence to convert.</param>
        public static implicit operator ReadOnlySequence<byte>(Sequence sequence)
        {
            return sequence.first != null
#if CSHARP_8_OR_NEWER
                ? new ReadOnlySequence<byte>(sequence.first, sequence.first.Start, sequence.last!, sequence.last!.End)
#else
                ? new ReadOnlySequence<byte>(sequence.first, sequence.first.Start, sequence.last, sequence.last.End)
#endif
                : ReadOnlySequence<byte>.Empty;
        }

        /// <summary>
        /// Removes all elements from the sequence from its beginning to the specified position,
        /// considering that data to have been fully processed.
        /// </summary>
        /// <param name="position">
        /// The position of the first element that has not yet been processed.
        /// This is typically <see cref="ReadOnlySequence{Byte}.End"/> after reading all elements from that instance.
        /// </param>
        public void AdvanceTo(SequencePosition position)
        {
#if CSHARP_8_OR_NEWER
            var firstSegment = (SequenceSegment)position.GetObject()!;
#else
            var firstSegment = (SequenceSegment)position.GetObject();
#endif
            var firstIndex = position.GetInteger();

            // Before making any mutations, confirm that the block specified belongs to this sequence.
            var current = this.first;
            while (current != firstSegment && current != null)
            {
                current = current.Next;
            }

            if (current is null)
            {
                throw new ArgumentException("Position does not represent a valid position in this sequence.", nameof(position));
            }

            // Also confirm that the position is not a prior position in the block.
            if (firstIndex < current.Start)
            {
                throw new ArgumentException("Position must not be earlier than current position.", nameof(position));
            }

            // Now repeat the loop, performing the mutations.
#if CSHARP_8_OR_NEWER
            current = this.first!;
#else
            current = this.first;
#endif
            while (current != firstSegment)
            {
                current = this.RecycleAndGetNext(current);
            }

            firstSegment.AdvanceTo(firstIndex);

            if (firstSegment.Length == 0)
            {
                firstSegment = this.RecycleAndGetNext(firstSegment);
            }

            this.first = firstSegment;

            if (this.first == null)
            {
                this.last = null;
            }
        }

        /// <summary>
        /// Advances the sequence to include the specified number of elements initialized into memory
        /// returned by a prior call to <see cref="GetMemory(int)"/>.
        /// </summary>
        /// <param name="count">The number of elements written into memory.</param>
        public void Advance(int count)
        {
            if (this.last is null)
            {
                throw new InvalidOperationException("Cannot advance before acquiring memory.");
            }

            this.last.Advance(count);
        }

        /// <summary>
        /// Gets writable memory that can be initialized and added to the sequence via a subsequent call to <see cref="Advance(int)"/>.
        /// </summary>
        /// <param name="sizeHint">The size of the memory required, or 0 to just get a convenient (non-empty) buffer.</param>
        /// <returns>The requested memory.</returns>
        public Memory<byte> GetMemory(int sizeHint) => this.GetSegment(sizeHint).RemainingMemory;

        /// <summary>
        /// Gets writable memory that can be initialized and added to the sequence via a subsequent call to <see cref="Advance(int)"/>.
        /// </summary>
        /// <param name="sizeHint">The size of the memory required, or 0 to just get a convenient (non-empty) buffer.</param>
        /// <returns>The requested memory.</returns>
        public Span<byte> GetSpan(int sizeHint) => this.GetSegment(sizeHint).RemainingSpan;

        /// <summary>
        /// Clears the entire sequence, recycles associated memory into pools,
        /// and resets this instance for reuse.
        /// This invalidates any <see cref="ReadOnlySequence{Byte}"/> previously produced by this instance.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose() => this.Reset();

        /// <summary>
        /// Clears the entire sequence and recycles associated memory into pools.
        /// This invalidates any <see cref="ReadOnlySequence{Byte}"/> previously produced by this instance.
        /// </summary>
        public void Reset()
        {
            var current = this.first;
            while (current != null)
            {
                current = this.RecycleAndGetNext(current);
            }

            this.first = this.last = null;
        }

        private SequenceSegment GetSegment(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            int minBufferSize;
            if (sizeHint == 0)
            {
                if (this.last == null || this.last.WritableBytes == 0)
                {
                    // We're going to need more memory. Take whatever size the pool wants to give us.
                    minBufferSize = -1;
                }
                else
                {
                    return this.last;
                }
            }
            else
            {
                if (sizeHint < this.MinimumSpanLength)
                {
                    sizeHint = this.MinimumSpanLength;
                }

                if (this.last == null || this.last.WritableBytes < sizeHint)
                {
                    minBufferSize = sizeHint;
                }
                else
                {
                    return this.last;
                }
            }

            var segment = this.segmentPool.Count > 0 ? this.segmentPool.Pop() : new SequenceSegment();
            if (this.arrayPool != null)
            {
                segment.Assign(this.arrayPool.Rent(minBufferSize == -1 ? DefaultLengthFromArrayPool : minBufferSize));
            }
            else
            {
#if CSHARP_8_OR_NEWER
                segment.Assign(this.memoryPool!.Rent(minBufferSize));
#else
                segment.Assign(this.memoryPool.Rent(minBufferSize));
#endif
            }

            this.Append(segment);
#if CSHARP_8_OR_NEWER
            return this.last!;
#else
            return this.last;
#endif
        }

        private void Append(SequenceSegment segment)
        {
            if (this.last == null)
            {
                this.first = this.last = segment;
            }
            else
            {
                if (this.last.Length > 0)
                {
                    // Add a new block.
                    this.last.SetNext(segment);
                }
                else
                {
                    // The last block is completely unused. Replace it instead of appending to it.
                    var current = this.first;
                    if (current is null)
                    {
                        throw new NullReferenceException(nameof(this.first));
                    }

                    if (this.first != this.last)
                    {
                        while (current.Next != this.last)
                        {
                            current = current.Next;
                        }
                    }
                    else
                    {
                        this.first = segment;
                    }

                    current.SetNext(segment);
                    this.RecycleAndGetNext(this.last);
                }

                this.last = segment;
            }
        }

        private SequenceSegment RecycleAndGetNext(SequenceSegment segment)
        {
            var recycledSegment = segment;
            segment = segment.Next;
            recycledSegment.ResetMemory(this.arrayPool);
            this.segmentPool.Push(recycledSegment);
            return segment;
        }

        private class SequenceSegment : ReadOnlySequenceSegment<byte>
        {
            /// <summary>
            /// Gets the backing array, when using an <see cref="ArrayPool{T}"/> instead of a <see cref="MemoryPool{T}"/>.
            /// </summary>
#if CSHARP_8_OR_NEWER
            private byte[]? array;
#else
            private byte[] array;
#endif

            /// <summary>
            /// Gets the position within <see cref="ReadOnlySequenceSegment{T}.Memory"/> where the data starts.
            /// </summary>
            /// <remarks>This may be nonzero as a result of calling <see cref="Sequence.AdvanceTo(SequencePosition)"/>.</remarks>
            internal int Start { get; private set; }

            /// <summary>
            /// Gets the position within <see cref="ReadOnlySequenceSegment{T}.Memory"/> where the data ends.
            /// </summary>
            internal int End { get; private set; }

            /// <summary>
            /// Gets the tail of memory that has not yet been committed.
            /// </summary>
            internal Memory<byte> RemainingMemory => this.AvailableMemory.Slice(this.End);

            /// <summary>
            /// Gets the tail of memory that has not yet been committed.
            /// </summary>
            internal Span<byte> RemainingSpan => this.AvailableMemory.Span.Slice(this.End);

            /// <summary>
            /// Gets the tracker for the underlying array for this segment, which can be used to recycle the array when we're disposed of.
            /// Will be <c>null</c> if using an array pool, in which case the memory is held by <see cref="array"/>.
            /// </summary>
#if CSHARP_8_OR_NEWER
            private IMemoryOwner<byte>? MemoryOwner { get; set; }
#else
            private IMemoryOwner<byte> MemoryOwner { get; set; }
#endif

            /// <summary>
            /// Gets the full memory owned by the <see cref="MemoryOwner"/>.
            /// </summary>
            private Memory<byte> AvailableMemory => this.array ?? this.MemoryOwner?.Memory ?? default;

            /// <summary>
            /// Gets the number of elements that are committed in this segment.
            /// </summary>
            internal int Length => this.End - this.Start;

            /// <summary>
            /// Gets the amount of writable bytes in this segment.
            /// It is the amount of bytes between <see cref="Length"/> and <see cref="End"/>.
            /// </summary>
            internal int WritableBytes => this.AvailableMemory.Length - this.End;

            /// <summary>
            /// Gets or sets the next segment in the singly linked list of segments.
            /// </summary>
            internal new SequenceSegment Next
            {
#if CSHARP_8_OR_NEWER
                get => (SequenceSegment)base.Next!;
#else
                get => (SequenceSegment)base.Next;
#endif
                private set => base.Next = value;
            }

            /// <summary>
            /// Assigns this (recyclable) segment a new area in memory.
            /// </summary>
            /// <param name="memoryOwner">The memory and a means to recycle it.</param>
            internal void Assign(IMemoryOwner<byte> memoryOwner)
            {
                this.MemoryOwner = memoryOwner;
                this.Memory = memoryOwner.Memory;
            }

            /// <summary>
            /// Assigns this (recyclable) segment a new area in memory.
            /// </summary>
            /// <param name="array">An array drawn from an <see cref="ArrayPool{T}"/>.</param>
            // ReSharper disable once ParameterHidesMember
            internal void Assign(byte[] array)
            {
                this.array = array;
                this.Memory = array;
            }

            /// <summary>
            /// Clears all fields in preparation to recycle this instance.
            /// </summary>
#if CSHARP_8_OR_NEWER
            internal void ResetMemory(ArrayPool<byte>? arrayPool)
#else
            internal void ResetMemory(ArrayPool<byte> arrayPool)
#endif
            {
                this.Memory = default;
                base.Next = default;
                this.RunningIndex = 0;
                this.Start = 0;
                this.End = 0;
                if (this.array != null)
                {
#if CSHARP_8_OR_NEWER
                    arrayPool!.Return(this.array);
#else
                    arrayPool.Return(this.array);
#endif
                    this.array = null;
                }
                else
                {
                    this.MemoryOwner?.Dispose();
                    this.MemoryOwner = null;
                }
            }

            /// <summary>
            /// Adds a new segment after this one.
            /// </summary>
            /// <param name="segment">The next segment in the linked list.</param>
            internal void SetNext(SequenceSegment segment)
            {
                Debug.Assert(segment != null, "Null not allowed.");
                this.Next = segment;
                segment.RunningIndex = this.RunningIndex + this.Start + this.Length;

                // When setting Memory, we start with index 0 instead of this.Start because
                // the first segment has an explicit index set anyway,
                // and we don't want to double-count it here.
                this.Memory = this.AvailableMemory.Slice(0, this.Start + this.Length);
            }

            /// <summary>
            /// Commits more elements as written in this segment.
            /// </summary>
            /// <param name="count">The number of elements written.</param>
            internal void Advance(int count)
            {
                if (count < 0 || this.End + count > this.Memory.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(count));
                }

                this.End += count;
            }

            /// <summary>
            /// Removes some elements from the start of this segment.
            /// </summary>
            /// <param name="offset">The number of elements to ignore from the start of the underlying array.</param>
            internal void AdvanceTo(int offset)
            {
                Debug.Assert(offset >= this.Start, "Trying to rewind.");
                this.Start = offset;
            }
        }
    }
}
