// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal.DoubleConversion
{
#pragma warning disable 660
#pragma warning disable 661
    internal readonly ref struct Iterator
    {
        private readonly ReadOnlySpan<byte> buffer;
        private readonly int offset;

        public Iterator(ReadOnlySpan<byte> buffer, int offset)
        {
            this.buffer = buffer;
            this.offset = offset;
        }

        public Iterator(ReadOnlySpan<byte> buffer)
            : this(buffer, 0)
        {
        }

        public byte Value => buffer[offset];

        public static Iterator operator +(in Iterator self, int length)
        {
            return new Iterator(self.buffer, self.offset + length);
        }

        public static int operator -(in Iterator lhs, in Iterator rhs)
        {
            return lhs.offset - rhs.offset;
        }

        public static bool operator ==(in Iterator lhs, in Iterator rhs)
        {
            return lhs.offset == rhs.offset;
        }

        public static bool operator !=(in Iterator lhs, in Iterator rhs)
        {
            return lhs.offset != rhs.offset;
        }

        public static bool operator ==(in Iterator lhs, char rhs)
        {
            return lhs.buffer[lhs.offset] == (byte)rhs;
        }

        public static bool operator !=(in Iterator lhs, char rhs)
        {
            return lhs.buffer[lhs.offset] != (byte)rhs;
        }

        public static bool operator ==(in Iterator lhs, byte rhs)
        {
            return lhs.buffer[lhs.offset] == rhs;
        }

        public static bool operator !=(in Iterator lhs, byte rhs)
        {
            return lhs.buffer[lhs.offset] != rhs;
        }

        public static bool operator >=(in Iterator lhs, char rhs)
        {
            return lhs.buffer[lhs.offset] >= (byte)rhs;
        }

        public static bool operator <=(in Iterator lhs, char rhs)
        {
            return lhs.buffer[lhs.offset] <= (byte)rhs;
        }

        public static bool operator >(in Iterator lhs, char rhs)
        {
            return lhs.buffer[lhs.offset] > (byte)rhs;
        }

        public static bool operator <(in Iterator lhs, char rhs)
        {
            return lhs.buffer[lhs.offset] < (byte)rhs;
        }
    }
}