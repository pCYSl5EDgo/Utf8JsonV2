// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/* Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information. */

#if !SEQUENCE_READER_BUILTIN
using System.Diagnostics;
using System.Runtime.CompilerServices;
// ReSharper disable ParameterHidesMember

namespace System.Buffers
{
    public ref struct SequenceReader<T>
        where T : unmanaged, IEquatable<T>
    {
        /// <summary>
        /// A value indicating whether we're using <see cref="sequence"/> (as opposed to <see cref="memory"/>.
        /// </summary>
        private readonly bool usingSequence;

        /// <summary>
        /// Backing for the entire sequence when we're not using <see cref="memory"/>.
        /// </summary>
        private ReadOnlySequence<T> sequence;

        /// <summary>
        /// The position at the start of the <see cref="CurrentSpan"/>.
        /// </summary>
        private SequencePosition currentPosition;

        /// <summary>
        /// The position at the end of the <see cref="CurrentSpan"/>.
        /// </summary>
        private SequencePosition nextPosition;

        /// <summary>
        /// Backing for the entire sequence when we're not using <see cref="sequence"/>.
        /// </summary>
        private readonly ReadOnlyMemory<T> memory;

        /// <summary>
        /// A value indicating whether there is unread data remaining.
        /// </summary>
        private bool moreData;

        /// <summary>
        /// The total number of elements in the sequence.
        /// </summary>
        private long length;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceReader{T}"/> struct
        /// over the given <see cref="ReadOnlySequence{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SequenceReader(in ReadOnlySequence<T> sequence)
        {
            this.usingSequence = true;
            this.CurrentSpanIndex = 0;
            this.Consumed = 0;
            this.sequence = sequence;
            this.memory = default;
            this.currentPosition = sequence.Start;
            this.length = -1;

            var first = sequence.First.Span;
            this.nextPosition = sequence.GetPosition(first.Length);
            this.CurrentSpan = first;
            this.moreData = first.Length > 0;

            if (!this.moreData && !sequence.IsSingleSegment)
            {
                this.moreData = true;
                this.GetNextSpan();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceReader{T}"/> struct
        /// over the given <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SequenceReader(ReadOnlyMemory<T> memory)
        {
            this.usingSequence = false;
            this.CurrentSpanIndex = 0;
            this.Consumed = 0;
            this.memory = memory;
            this.CurrentSpan = memory.Span;
            this.length = memory.Length;
            this.moreData = memory.Length > 0;

            this.currentPosition = default;
            this.nextPosition = default;
            this.sequence = default;
        }

        /// <summary>
        /// Gets a value indicating whether there is no more data in the <see cref="Sequence"/>.
        /// </summary>
        public bool End => !this.moreData;

        /// <summary>
        /// Gets the underlying <see cref="ReadOnlySequence{T}"/> for the reader.
        /// </summary>
        public ReadOnlySequence<T> Sequence
        {
            get
            {
                if (this.sequence.IsEmpty && !this.memory.IsEmpty)
                {
                    // We're in memory mode (instead of sequence mode).
                    // Lazily fill in the sequence data.
                    this.sequence = new ReadOnlySequence<T>(this.memory);
                    this.currentPosition = this.sequence.Start;
                    this.nextPosition = this.sequence.End;
                }

                return this.sequence;
            }
        }

        /// <summary>
        /// Gets the current position in the <see cref="Sequence"/>.
        /// </summary>
        public SequencePosition Position
            => this.Sequence.GetPosition(this.CurrentSpanIndex, this.currentPosition);

        /// <summary>
        /// Gets the current segment in the <see cref="Sequence"/> as a span.
        /// </summary>
        public ReadOnlySpan<T> CurrentSpan { get; private set; }

        /// <summary>
        /// Gets the index in the <see cref="CurrentSpan"/>.
        /// </summary>
        public int CurrentSpanIndex { get; private set; }

        /// <summary>
        /// Gets the unread portion of the <see cref="CurrentSpan"/>.
        /// </summary>
        public ReadOnlySpan<T> UnreadSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.CurrentSpan.Slice(this.CurrentSpanIndex);
        }

        /// <summary>
        /// Gets the total number of <typeparamref name="T"/>'s processed by the reader.
        /// </summary>
        public long Consumed { get; private set; }

        /// <summary>
        /// Gets remaining <typeparamref name="T"/>'s in the reader's <see cref="Sequence"/>.
        /// </summary>
        public long Remaining => this.Length - this.Consumed;

        /// <summary>
        /// Gets count of <typeparamref name="T"/> in the reader's <see cref="Sequence"/>.
        /// </summary>
        public long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (this.length < 0)
                {
                    // Cache the length
                    this.length = this.Sequence.Length;
                }

                return this.length;
            }
        }

        /// <summary>
        /// Peeks at the next value without advancing the reader.
        /// </summary>
        /// <param name="value">The next value or default if at the end.</param>
        /// <returns>False if at the end of the reader.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek(out T value)
        {
            if (this.moreData)
            {
                value = this.CurrentSpan[this.CurrentSpanIndex];
                return true;
            }
            
            value = default;
            return false;
        }

        /// <summary>
        /// Read the next value and advance the reader.
        /// </summary>
        /// <param name="value">The next value or default if at the end.</param>
        /// <returns>False if at the end of the reader.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRead(out T value)
        {
            if (this.End)
            {
                value = default;
                return false;
            }

            value = this.CurrentSpan[this.CurrentSpanIndex];
            this.CurrentSpanIndex++;
            this.Consumed++;

            if (this.CurrentSpanIndex >= this.CurrentSpan.Length)
            {
                if (this.usingSequence)
                {
                    this.GetNextSpan();
                }
                else
                {
                    this.moreData = false;
                }
            }

            return true;
        }

        /// <summary>
        /// Move the reader back the specified number of items.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rewind(long count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this.Consumed -= count;

            if (this.CurrentSpanIndex >= count)
            {
                this.CurrentSpanIndex -= (int)count;
                this.moreData = true;
            }
            else if (this.usingSequence)
            {
                // Current segment doesn't have enough data, scan backward through segments
                this.RetreatToPreviousSpan(this.Consumed);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Rewind went past the start of the memory.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RetreatToPreviousSpan(long consumed)
        {
            Debug.Assert(this.usingSequence, "usingSequence");
            this.ResetReader();
            this.Advance(consumed);
        }

        private void ResetReader()
        {
            Debug.Assert(this.usingSequence, "usingSequence");
            this.CurrentSpanIndex = 0;
            this.Consumed = 0;
            this.currentPosition = this.Sequence.Start;
            this.nextPosition = this.currentPosition;

            if (this.Sequence.TryGet(ref this.nextPosition, out var memoryLocalVariable))
            {
                this.moreData = true;

                if (memoryLocalVariable.Length == 0)
                {
                    this.CurrentSpan = default;

                    // No data in the first span, move to one with data
                    this.GetNextSpan();
                }
                else
                {
                    this.CurrentSpan = memoryLocalVariable.Span;
                }
            }
            else
            {
                // No data in any spans and at end of sequence
                this.moreData = false;
                this.CurrentSpan = default;
            }
        }

        /// <summary>
        /// Get the next segment with available data, if any.
        /// </summary>
        private void GetNextSpan()
        {
            Debug.Assert(this.usingSequence, "usingSequence");
            if (!this.Sequence.IsSingleSegment)
            {
                var previousNextPosition = this.nextPosition;
                while (this.Sequence.TryGet(ref this.nextPosition, out var memoryLocalVariable))
                {
                    this.currentPosition = previousNextPosition;
                    if (memoryLocalVariable.Length > 0)
                    {
                        this.CurrentSpan = memoryLocalVariable.Span;
                        this.CurrentSpanIndex = 0;
                        return;
                    }
                    this.CurrentSpan = default;
                    this.CurrentSpanIndex = 0;
                    previousNextPosition = this.nextPosition;
                }
            }

            this.moreData = false;
        }

        /// <summary>
        /// Move the reader ahead the specified number of items.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(long count)
        {
            const long tooBigOrNegative = unchecked((long)0xFFFFFFFF80000000);
            if ((count & tooBigOrNegative) == 0 && this.CurrentSpan.Length - this.CurrentSpanIndex > (int)count)
            {
                this.CurrentSpanIndex += (int)count;
                this.Consumed += count;
            }
            else if (this.usingSequence)
            {
                // Can't satisfy from the current span
                this.AdvanceToNextSpan(count);
            }
            else if (this.CurrentSpan.Length - this.CurrentSpanIndex == (int)count)
            {
                this.CurrentSpanIndex += (int)count;
                this.Consumed += count;
                this.moreData = false;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }

        /// <summary>
        /// Unchecked helper to avoid unnecessary checks where you know count is valid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AdvanceCurrentSpan(long count)
        {
            Debug.Assert(count >= 0, "count >= 0");

            this.Consumed += count;
            this.CurrentSpanIndex += (int)count;
            if (this.usingSequence && this.CurrentSpanIndex >= this.CurrentSpan.Length)
            {
                this.GetNextSpan();
            }
        }

        /// <summary>
        /// Only call this helper if you know that you are advancing in the current span
        /// with valid count and there is no need to fetch the next one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AdvanceWithinSpan(long count)
        {
            Debug.Assert(count >= 0, "count >= 0");

            this.Consumed += count;
            this.CurrentSpanIndex += (int)count;

            Debug.Assert(this.CurrentSpanIndex < this.CurrentSpan.Length, "this.CurrentSpanIndex < this.CurrentSpan.Length");
        }

        /// <summary>
        /// Move the reader ahead the specified number of items
        /// if there are enough elements remaining in the sequence.
        /// </summary>
        /// <returns><c>true</c> if there were enough elements to advance; otherwise <c>false</c>.</returns>
        internal bool TryAdvance(long count)
        {
            if (this.Remaining < count)
            {
                return false;
            }

            this.Advance(count);
            return true;
        }

        private void AdvanceToNextSpan(long count)
        {
            Debug.Assert(this.usingSequence, "usingSequence");
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this.Consumed += count;
            while (this.moreData)
            {
                var remaining = this.CurrentSpan.Length - this.CurrentSpanIndex;

                if (remaining > count)
                {
                    this.CurrentSpanIndex += (int)count;
                    count = 0;
                    break;
                }

                // As there may not be any further segments we need to
                // push the current index to the end of the span.
                this.CurrentSpanIndex += remaining;
                count -= remaining;
                Debug.Assert(count >= 0, "count >= 0");

                this.GetNextSpan();

                if (count == 0)
                {
                    break;
                }
            }

            if (count != 0)
            {
                // Not enough data left- adjust for where we actually ended and throw
                this.Consumed -= count;
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }

        /// <summary>
        /// Copies data from the current <see cref="Position"/> to the given <paramref name="destination"/> span.
        /// </summary>
        /// <param name="destination">Destination to copy to.</param>
        /// <returns>True if there is enough data to copy to the <paramref name="destination"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryCopyTo(Span<T> destination)
        {
            var firstSpan = this.UnreadSpan;
            if (firstSpan.Length >= destination.Length)
            {
                firstSpan.Slice(0, destination.Length).CopyTo(destination);
                return true;
            }

            return this.TryCopyMultisegment(destination);
        }

        internal bool TryCopyMultisegment(Span<T> destination)
        {
            if (this.Remaining < destination.Length)
            {
                return false;
            }

            var firstSpan = this.UnreadSpan;
            Debug.Assert(firstSpan.Length < destination.Length, "firstSpan.Length < destination.Length");
            firstSpan.CopyTo(destination);
            var copied = firstSpan.Length;

            var next = this.nextPosition;
            while (this.Sequence.TryGet(ref next, out var nextSegment))
            {
                if (nextSegment.Length > 0)
                {
                    var nextSpan = nextSegment.Span;
                    var toCopy = Math.Min(nextSpan.Length, destination.Length - copied);
                    nextSpan.Slice(0, toCopy).CopyTo(destination.Slice(copied));
                    copied += toCopy;
                    if (copied >= destination.Length)
                    {
                        break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Try to read everything up to the given <paramref name="delimiter"/>.
        /// </summary>
        /// <param name="span">The read data, if any.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="advancePastDelimiter">True to move past the <paramref name="delimiter"/> if found.</param>
        /// <returns>True if the <paramref name="delimiter"/> was found.</returns>
        public bool TryReadTo(out ReadOnlySpan<T> span, T delimiter, bool advancePastDelimiter = true)
        {
            var remaining = UnreadSpan;
            var index = remaining.IndexOf(delimiter);

            if (index != -1)
            {
                span = index == 0 ? default : remaining.Slice(0, index);
                AdvanceCurrentSpan(index + (advancePastDelimiter ? 1 : 0));
                return true;
            }

            return TryReadToSlow(out span, delimiter, advancePastDelimiter);
        }

        private bool TryReadToSlow(out ReadOnlySpan<T> span, T delimiter, bool advancePastDelimiter)
        {
            if (!TryReadToInternal(out var sequenceLocalVariable, delimiter, advancePastDelimiter, CurrentSpan.Length - CurrentSpanIndex))
            {
                span = default;
                return false;
            }

            span = sequenceLocalVariable.IsSingleSegment ? sequenceLocalVariable.First.Span : sequenceLocalVariable.ToArray();
            return true;
        }

        /// <summary>
        /// Try to read everything up to the given <paramref name="delimiter"/>, ignoring delimiters that are
        /// preceded by <paramref name="delimiterEscape"/>.
        /// </summary>
        /// <param name="span">The read data, if any.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="delimiterEscape">If found prior to <paramref name="delimiter"/> it will skip that occurrence.</param>
        /// <param name="advancePastDelimiter">True to move past the <paramref name="delimiter"/> if found.</param>
        /// <returns>True if the <paramref name="delimiter"/> was found.</returns>
        public bool TryReadTo(out ReadOnlySpan<T> span, T delimiter, T delimiterEscape, bool advancePastDelimiter = true)
        {
            var remaining = UnreadSpan;
            var index = remaining.IndexOf(delimiter);

            if ((index > 0 && !remaining[index - 1].Equals(delimiterEscape)) || index == 0)
            {
                span = remaining.Slice(0, index);
                AdvanceCurrentSpan(index + (advancePastDelimiter ? 1 : 0));
                return true;
            }

            // This delimiter might be skipped, go down the slow path
            return TryReadToSlow(out span, delimiter, delimiterEscape, index, advancePastDelimiter);
        }

        private bool TryReadToSlow(out ReadOnlySpan<T> span, T delimiter, T delimiterEscape, int index, bool advancePastDelimiter)
        {
            if (!TryReadToSlow(out ReadOnlySequence<T> sequenceLocalVariable, delimiter, delimiterEscape, index, advancePastDelimiter))
            {
                span = default;
                return false;
            }

            Debug.Assert(sequenceLocalVariable.Length > 0);
            span = sequenceLocalVariable.IsSingleSegment ? sequenceLocalVariable.First.Span : sequenceLocalVariable.ToArray();
            return true;
        }

        private bool TryReadToSlow(out ReadOnlySequence<T> sequence, T delimiter, T delimiterEscape, int index, bool advancePastDelimiter)
        {
            var copy = this;

            var remaining = UnreadSpan;
            var priorEscape = false;

            do
            {
                if (index >= 0)
                {
                    if (index == 0 && priorEscape)
                    {
                        // We were in the escaped state, so skip this delimiter
                        priorEscape = false;
                        Advance(index + 1);
                        remaining = UnreadSpan;
                        goto Continue;
                    }
                    if (index > 0 && remaining[index - 1].Equals(delimiterEscape))
                    {
                        // This delimiter might be skipped

                        // Count our escapes
                        var escapeCount = 1;
                        var i = index - 2;
                        for (; i >= 0; i--)
                        {
                            if (!remaining[i].Equals(delimiterEscape))
                                break;
                        }
                        if (i < 0 && priorEscape)
                        {
                            // Started and ended with escape, increment once more
                            escapeCount++;
                        }
                        escapeCount += index - 2 - i;

                        if ((escapeCount & 1) != 0)
                        {
                            // An odd escape count means we're currently escaped,
                            // skip the delimiter and reset escaped state.
                            Advance(index + 1);
                            priorEscape = false;
                            remaining = UnreadSpan;
                            goto Continue;
                        }
                    }

                    // Found the delimiter. Move to it, slice, then move past it.
                    AdvanceCurrentSpan(index);

                    sequence = Sequence.Slice(copy.Position, Position);
                    if (advancePastDelimiter)
                    {
                        Advance(1);
                    }
                    return true;
                }
                // No delimiter, need to check the end of the span for odd number of escapes then advance
                if (remaining.Length > 0 && remaining[remaining.Length - 1].Equals(delimiterEscape))
                {
                    var escapeCount = 1;
                    var i = remaining.Length - 2;
                    for (; i >= 0; i--)
                    {
                        if (!remaining[i].Equals(delimiterEscape))
                            break;
                    }

                    escapeCount += remaining.Length - 2 - i;
                    if (i < 0 && priorEscape)
                        priorEscape = (escapeCount & 1) == 0;   // equivalent to incrementing escapeCount before setting priorEscape
                    else
                        priorEscape = (escapeCount & 1) != 0;
                }
                else
                {
                    priorEscape = false;
                }

                // Nothing in the current span, move to the end, checking for the skip delimiter
                AdvanceCurrentSpan(remaining.Length);
                remaining = CurrentSpan;

            Continue:
                index = remaining.IndexOf(delimiter);
            } while (!End);

            // Didn't find anything, reset our original state.
            this = copy;
            sequence = default;
            return false;
        }

        /// <summary>
        /// Try to read everything up to the given <paramref name="delimiter"/>.
        /// </summary>
        /// <param name="sequence">The read data, if any.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="advancePastDelimiter">True to move past the <paramref name="delimiter"/> if found.</param>
        /// <returns>True if the <paramref name="delimiter"/> was found.</returns>
        public bool TryReadTo(out ReadOnlySequence<T> sequence, T delimiter, bool advancePastDelimiter = true)
        {
            return TryReadToInternal(out sequence, delimiter, advancePastDelimiter);
        }

        private bool TryReadToInternal(out ReadOnlySequence<T> sequence, T delimiter, bool advancePastDelimiter, int skip = 0)
        {
            Debug.Assert(skip >= 0);
            var copy = this;
            if (skip > 0)
                Advance(skip);
            var remaining = UnreadSpan;

            while (moreData)
            {
                var index = remaining.IndexOf(delimiter);
                if (index != -1)
                {
                    // Found the delimiter. Move to it, slice, then move past it.
                    if (index > 0)
                    {
                        AdvanceCurrentSpan(index);
                    }

                    sequence = Sequence.Slice(copy.Position, Position);
                    if (advancePastDelimiter)
                    {
                        Advance(1);
                    }
                    return true;
                }

                AdvanceCurrentSpan(remaining.Length);
                remaining = CurrentSpan;
            }

            // Didn't find anything, reset our original state.
            this = copy;
            sequence = default;
            return false;
        }

        /// <summary>
        /// Try to read everything up to the given <paramref name="delimiter"/>, ignoring delimiters that are
        /// preceded by <paramref name="delimiterEscape"/>.
        /// </summary>
        /// <param name="sequence">The read data, if any.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="delimiterEscape">If found prior to <paramref name="delimiter"/> it will skip that occurrence.</param>
        /// <param name="advancePastDelimiter">True to move past the <paramref name="delimiter"/> if found.</param>
        /// <returns>True if the <paramref name="delimiter"/> was found.</returns>
        public bool TryReadTo(out ReadOnlySequence<T> sequence, T delimiter, T delimiterEscape, bool advancePastDelimiter = true)
        {
            var copy = this;

            var remaining = UnreadSpan;
            var priorEscape = false;

            while (moreData)
            {
                var index = remaining.IndexOf(delimiter);
                if (index != -1)
                {
                    if (index == 0 && priorEscape)
                    {
                        // We were in the escaped state, so skip this delimiter
                        priorEscape = false;
                        Advance(index + 1);
                        remaining = UnreadSpan;
                        continue;
                    }
                    if (index > 0 && remaining[index - 1].Equals(delimiterEscape))
                    {
                        // This delimiter might be skipped

                        // Count our escapes
                        var escapeCount = 0;
                        for (var i = index; i > 0 && remaining[i - 1].Equals(delimiterEscape); i--, escapeCount++)
                        {
                        }

                        if (escapeCount == index && priorEscape)
                        {
                            // Started and ended with escape, increment once more
                            escapeCount++;
                        }

                        priorEscape = false;
                        if ((escapeCount & 1) != 0)
                        {
                            // Odd escape count means we're in the escaped state, so skip this delimiter
                            Advance(index + 1);
                            remaining = UnreadSpan;
                            continue;
                        }
                    }

                    // Found the delimiter. Move to it, slice, then move past it.
                    if (index > 0)
                    {
                        Advance(index);
                    }

                    sequence = Sequence.Slice(copy.Position, Position);
                    if (advancePastDelimiter)
                    {
                        Advance(1);
                    }
                    return true;
                }

                // No delimiter, need to check the end of the span for odd number of escapes then advance
                {
                    var escapeCount = 0;
                    for (var i = remaining.Length; i > 0 && remaining[i - 1].Equals(delimiterEscape); i--, escapeCount++)
                    {
                    }

                    if (priorEscape && escapeCount == remaining.Length)
                    {
                        escapeCount++;
                    }
                    priorEscape = escapeCount % 2 != 0;
                }

                // Nothing in the current span, move to the end, checking for the skip delimiter
                Advance(remaining.Length);
                remaining = CurrentSpan;
            }

            // Didn't find anything, reset our original state.
            this = copy;
            sequence = default;
            return false;
        }

        /// <summary>
        /// Try to read everything up to the given <paramref name="delimiters"/>.
        /// </summary>
        /// <param name="span">The read data, if any.</param>
        /// <param name="delimiters">The delimiters to look for.</param>
        /// <param name="advancePastDelimiter">True to move past the first found instance of any of the given <paramref name="delimiters"/>.</param>
        /// <returns>True if any of the <paramref name="delimiters"/> were found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadToAny(out ReadOnlySpan<T> span, ReadOnlySpan<T> delimiters, bool advancePastDelimiter = true)
        {
            var remaining = UnreadSpan;
            var index = delimiters.Length == 2
                ? remaining.IndexOfAny(delimiters[0], delimiters[1])
                : remaining.IndexOfAny(delimiters);

            if (index != -1)
            {
                span = remaining.Slice(0, index);
                Advance(index + (advancePastDelimiter ? 1 : 0));
                return true;
            }

            return TryReadToAnySlow(out span, delimiters, advancePastDelimiter);
        }

        private bool TryReadToAnySlow(out ReadOnlySpan<T> span, ReadOnlySpan<T> delimiters, bool advancePastDelimiter)
        {
            if (!TryReadToAnyInternal(out var sequenceLocalVariable, delimiters, advancePastDelimiter, CurrentSpan.Length - CurrentSpanIndex))
            {
                span = default;
                return false;
            }

            span = sequenceLocalVariable.IsSingleSegment ? sequenceLocalVariable.First.Span : sequenceLocalVariable.ToArray();
            return true;
        }

        /// <summary>
        /// Try to read everything up to the given <paramref name="delimiters"/>.
        /// </summary>
        /// <param name="sequence">The read data, if any.</param>
        /// <param name="delimiters">The delimiters to look for.</param>
        /// <param name="advancePastDelimiter">True to move past the first found instance of any of the given <paramref name="delimiters"/>.</param>
        /// <returns>True if any of the <paramref name="delimiters"/> were found.</returns>
        public bool TryReadToAny(out ReadOnlySequence<T> sequence, ReadOnlySpan<T> delimiters, bool advancePastDelimiter = true)
        {
            return TryReadToAnyInternal(out sequence, delimiters, advancePastDelimiter);
        }

        private bool TryReadToAnyInternal(out ReadOnlySequence<T> sequence, ReadOnlySpan<T> delimiters, bool advancePastDelimiter, int skip = 0)
        {
            var copy = this;
            if (skip > 0)
                Advance(skip);
            var remaining = UnreadSpan;

            while (!End)
            {
                var index = delimiters.Length == 2
                    ? remaining.IndexOfAny(delimiters[0], delimiters[1])
                    : remaining.IndexOfAny(delimiters);

                if (index != -1)
                {
                    // Found one of the delimiters. Move to it, slice, then move past it.
                    if (index > 0)
                    {
                        AdvanceCurrentSpan(index);
                    }

                    sequence = Sequence.Slice(copy.Position, Position);
                    if (advancePastDelimiter)
                    {
                        Advance(1);
                    }
                    return true;
                }

                Advance(remaining.Length);
                remaining = CurrentSpan;
            }

            // Didn't find anything, reset our original state.
            this = copy;
            sequence = default;
            return false;
        }

        /// <summary>
        /// Try to read data until the entire given <paramref name="delimiter"/> matches.
        /// </summary>
        /// <param name="sequence">The read data, if any.</param>
        /// <param name="delimiter">The multi (T) delimiter.</param>
        /// <param name="advancePastDelimiter">True to move past the <paramref name="delimiter"/> if found.</param>
        /// <returns>True if the <paramref name="delimiter"/> was found.</returns>
        public bool TryReadTo(out ReadOnlySequence<T> sequence, ReadOnlySpan<T> delimiter, bool advancePastDelimiter = true)
        {
            if (delimiter.Length == 0)
            {
                sequence = default;
                return true;
            }

            var copy = this;

            var advanced = false;
            while (!End)
            {
                if (!TryReadTo(out sequence, delimiter[0], advancePastDelimiter: false))
                {
                    this = copy;
                    return false;
                }

                if (delimiter.Length == 1)
                {
                    if (advancePastDelimiter)
                    {
                        Advance(1);
                    }
                    return true;
                }

                if (IsNext(delimiter))
                {
                    // Probably a faster way to do this, potentially by avoiding the Advance in the previous TryReadTo call
                    if (advanced)
                    {
                        sequence = copy.Sequence.Slice(copy.Consumed, Consumed - copy.Consumed);
                    }

                    if (advancePastDelimiter)
                    {
                        Advance(delimiter.Length);
                    }
                    return true;
                }
                Advance(1);
                advanced = true;
            }

            this = copy;
            sequence = default;
            return false;
        }

        /// <summary>
        /// Advance until the given <paramref name="delimiter"/>, if found.
        /// </summary>
        /// <param name="delimiter">The delimiter to search for.</param>
        /// <param name="advancePastDelimiter">True to move past the <paramref name="delimiter"/> if found.</param>
        /// <returns>True if the given <paramref name="delimiter"/> was found.</returns>
        public bool TryAdvanceTo(T delimiter, bool advancePastDelimiter = true)
        {
            var remaining = UnreadSpan;
            var index = remaining.IndexOf(delimiter);
            if (index != -1)
            {
                Advance(advancePastDelimiter ? index + 1 : index);
                return true;
            }

            return TryReadToInternal(out _, delimiter, advancePastDelimiter);
        }

        /// <summary>
        /// Advance until any of the given <paramref name="delimiters"/>, if found.
        /// </summary>
        /// <param name="delimiters">The delimiters to search for.</param>
        /// <param name="advancePastDelimiter">True to move past the first found instance of any of the given <paramref name="delimiters"/>.</param>
        /// <returns>True if any of the given <paramref name="delimiters"/> were found.</returns>
        public bool TryAdvanceToAny(ReadOnlySpan<T> delimiters, bool advancePastDelimiter = true)
        {
            var remaining = UnreadSpan;
            var index = remaining.IndexOfAny(delimiters);
            if (index != -1)
            {
                AdvanceCurrentSpan(index + (advancePastDelimiter ? 1 : 0));
                return true;
            }

            return TryReadToAnyInternal(out _, delimiters, advancePastDelimiter);
        }

        /// <summary>
        /// Advance past consecutive instances of the given <paramref name="value"/>.
        /// </summary>
        /// <returns>How many positions the reader has been advanced.</returns>
        public long AdvancePast(T value)
        {
            var start = Consumed;

            do
            {
                // Advance past all matches in the current span
                int i;
                for (i = CurrentSpanIndex; i < CurrentSpan.Length && CurrentSpan[i].Equals(value); i++)
                {
                }

                var advanced = i - CurrentSpanIndex;
                if (advanced == 0)
                {
                    // Didn't advance at all in this span, exit.
                    break;
                }

                AdvanceCurrentSpan(advanced);

                // If we're at position 0 after advancing and not at the End,
                // we're in a new span and should continue the loop.
            } while (CurrentSpanIndex == 0 && !End);

            return Consumed - start;
        }

        /// <summary>
        /// Skip consecutive instances of any of the given <paramref name="values"/>.
        /// </summary>
        /// <returns>How many positions the reader has been advanced.</returns>
        public long AdvancePastAny(ReadOnlySpan<T> values)
        {
            var start = Consumed;

            do
            {
                // Advance past all matches in the current span
                int i;
                for (i = CurrentSpanIndex; i < CurrentSpan.Length && values.IndexOf(CurrentSpan[i]) != -1; i++)
                {
                }

                var advanced = i - CurrentSpanIndex;
                if (advanced == 0)
                {
                    // Didn't advance at all in this span, exit.
                    break;
                }

                AdvanceCurrentSpan(advanced);

                // If we're at position 0 after advancing and not at the End,
                // we're in a new span and should continue the loop.
            } while (CurrentSpanIndex == 0 && !End);

            return Consumed - start;
        }

        /// <summary>
        /// Advance past consecutive instances of any of the given values.
        /// </summary>
        /// <returns>How many positions the reader has been advanced.</returns>
        public long AdvancePastAny(T value0, T value1, T value2, T value3)
        {
            var start = Consumed;

            do
            {
                // Advance past all matches in the current span
                int i;
                for (i = CurrentSpanIndex; i < CurrentSpan.Length; i++)
                {
                    var value = CurrentSpan[i];
                    if (!value.Equals(value0) && !value.Equals(value1) && !value.Equals(value2) && !value.Equals(value3))
                    {
                        break;
                    }
                }

                var advanced = i - CurrentSpanIndex;
                if (advanced == 0)
                {
                    // Didn't advance at all in this span, exit.
                    break;
                }

                AdvanceCurrentSpan(advanced);

                // If we're at position 0 after advancing and not at the End,
                // we're in a new span and should continue the loop.
            } while (CurrentSpanIndex == 0 && !End);

            return Consumed - start;
        }

        /// <summary>
        /// Advance past consecutive instances of any of the given values.
        /// </summary>
        /// <returns>How many positions the reader has been advanced.</returns>
        public long AdvancePastAny(T value0, T value1, T value2)
        {
            var start = Consumed;

            do
            {
                // Advance past all matches in the current span
                int i;
                for (i = CurrentSpanIndex; i < CurrentSpan.Length; i++)
                {
                    var value = CurrentSpan[i];
                    if (!value.Equals(value0) && !value.Equals(value1) && !value.Equals(value2))
                    {
                        break;
                    }
                }

                var advanced = i - CurrentSpanIndex;
                if (advanced == 0)
                {
                    // Didn't advance at all in this span, exit.
                    break;
                }

                AdvanceCurrentSpan(advanced);

                // If we're at position 0 after advancing and not at the End,
                // we're in a new span and should continue the loop.
            } while (CurrentSpanIndex == 0 && !End);

            return Consumed - start;
        }

        /// <summary>
        /// Advance past consecutive instances of any of the given values.
        /// </summary>
        /// <returns>How many positions the reader has been advanced.</returns>
        public long AdvancePastAny(T value0, T value1)
        {
            var start = Consumed;

            do
            {
                // Advance past all matches in the current span
                int i;
                for (i = CurrentSpanIndex; i < CurrentSpan.Length; i++)
                {
                    var value = CurrentSpan[i];
                    if (!value.Equals(value0) && !value.Equals(value1))
                    {
                        break;
                    }
                }

                var advanced = i - CurrentSpanIndex;
                if (advanced == 0)
                {
                    // Didn't advance at all in this span, exit.
                    break;
                }

                AdvanceCurrentSpan(advanced);

                // If we're at position 0 after advancing and not at the End,
                // we're in a new span and should continue the loop.
            } while (CurrentSpanIndex == 0 && !End);

            return Consumed - start;
        }

        /// <summary>
        /// Moves the reader to the end of the sequence.
        /// </summary>
        public void AdvanceToEnd()
        {
            if (moreData)
            {
                Consumed = Length;
                CurrentSpan = default;
                CurrentSpanIndex = 0;
                currentPosition = Sequence.End;
                nextPosition = default;
                moreData = false;
            }
        }

        /// <summary>
        /// Check to see if the given <paramref name="next"/> value is next.
        /// </summary>
        /// <param name="next">The value to compare the next items to.</param>
        /// <param name="advancePast">Move past the <paramref name="next"/> value if found.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNext(T next, bool advancePast = false)
        {
            if (End)
                return false;

            if (CurrentSpan[CurrentSpanIndex].Equals(next))
            {
                if (advancePast)
                {
                    AdvanceCurrentSpan(1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the given <paramref name="next"/> values are next.
        /// </summary>
        /// <param name="next">The span to compare the next items to.</param>
        /// <param name="advancePast">Move past the <paramref name="next"/> values if found.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNext(ReadOnlySpan<T> next, bool advancePast = false)
        {
            var unread = UnreadSpan;
            if (unread.StartsWith(next))
            {
                if (advancePast)
                {
                    AdvanceCurrentSpan(next.Length);
                }

                return true;
            }

            // Only check the slow path if there wasn't enough to satisfy next
            return unread.Length < next.Length && IsNextSlow(next, advancePast);
        }

        private bool IsNextSlow(ReadOnlySpan<T> next, bool advancePast)
        {
            var currentSpan = UnreadSpan;

            // We should only come in here if we need more data than we have in our current span
            Debug.Assert(currentSpan.Length < next.Length);

            var fullLength = next.Length;
            var nextPositionLocalVariable = this.nextPosition;

            while (next.StartsWith(currentSpan))
            {
                if (next.Length == currentSpan.Length)
                {
                    // Fully matched
                    if (advancePast)
                    {
                        Advance(fullLength);
                    }
                    return true;
                }

                // Need to check the next segment
                while (true)
                {
                    if (!Sequence.TryGet(ref nextPositionLocalVariable, out var nextSegment))
                    {
                        // Nothing left
                        return false;
                    }

                    if (nextSegment.Length > 0)
                    {
                        next = next.Slice(currentSpan.Length);
                        currentSpan = nextSegment.Span;
                        if (currentSpan.Length > next.Length)
                        {
                            currentSpan = currentSpan.Slice(0, next.Length);
                        }
                        break;
                    }
                }
            }

            return false;
        }
    }
}
#endif
