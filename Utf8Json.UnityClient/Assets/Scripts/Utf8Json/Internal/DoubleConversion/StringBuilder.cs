// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Utf8Json.Internal.DoubleConversion
{
    internal ref struct StringBuilder
    {
        private byte[] buffer;
        private int length;

        public int Length => length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder Create() => new StringBuilder(ArrayPool<byte>.Shared.Rent(256));

        private StringBuilder(byte[] buffer)
        {
            this.buffer = buffer;
            this.length = default;
        }

        public ReadOnlySpan<byte> ReadableSpan => buffer.AsSpan(0, length);

        public Span<byte> WritableSpan => buffer.AsSpan(length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAdditionalCapacity(int appendLength) => BinaryUtil.EnsureCapacityArrayPool(ref buffer, length, appendLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int length) => this.length += length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => ArrayPool<byte>.Shared.Return(buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendChar0Unsafe() => AppendByteUnsafe((byte)'0');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendCharDotUnsafe() => AppendByteUnsafe((byte)'.');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendCharPlusUnsafe() => AppendByteUnsafe((byte)'+');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendCharMinusUnsafe() => AppendByteUnsafe((byte)'-');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendCharEUnsafe() => AppendByteUnsafe((byte)'E');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendByteUnsafe(byte value) => buffer[length++] = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendString(string str)
        {
            EnsureAdditionalCapacity(StringEncoding.Utf8.GetMaxByteCount(str.Length));
            length += StringEncoding.Utf8.GetBytes(str, 0, str.Length, buffer, length);
        }
    }
}
