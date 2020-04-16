// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers.Binary;

namespace Utf8Json.Internal
{
    public static class ByteArraySortHelper
    {
        public static int CompareStatic(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            if (x.Length < y.Length)
            {
                return -1;
            }

            if (x.Length > y.Length)
            {
                return 1;
            }

            ulong xRest;
            ulong yRest;
            switch (x.Length - ((x.Length >> 3) << 3))
            {
                default: return CompareInternal(x, 0UL, y, 0UL);
                case 1:
                    xRest = x[x.Length - 1];
                    yRest = y[y.Length - 1];
                    return CompareInternal(x.Slice(0, x.Length - 1), xRest, y.Slice(0, y.Length - 1), yRest);
                case 2:
                    xRest = x[x.Length - 1];
                    xRest <<= 8;
                    xRest |= x[x.Length - 2];
                    yRest = y[y.Length - 1];
                    yRest <<= 8;
                    yRest |= y[y.Length - 2];
                    return CompareInternal(x.Slice(0, x.Length - 2), xRest, y.Slice(0, y.Length - 2), yRest);
                case 3:
                    xRest = x[x.Length - 1];
                    xRest <<= 8;
                    xRest |= x[x.Length - 2];
                    xRest <<= 8;
                    xRest |= x[x.Length - 3];
                    yRest = y[y.Length - 1];
                    yRest <<= 8;
                    yRest |= y[y.Length - 2];
                    yRest <<= 8;
                    yRest |= y[y.Length - 3];
                    return CompareInternal(x.Slice(0, x.Length - 3), xRest, y.Slice(0, y.Length - 3), yRest);
                case 4:
                    xRest = x[x.Length - 1];
                    xRest <<= 8;
                    xRest |= x[x.Length - 2];
                    xRest <<= 8;
                    xRest |= x[x.Length - 3];
                    xRest <<= 8;
                    xRest |= x[x.Length - 4];
                    yRest = y[y.Length - 1];
                    yRest <<= 8;
                    yRest |= y[y.Length - 2];
                    yRest <<= 8;
                    yRest |= y[y.Length - 3];
                    yRest <<= 8;
                    yRest |= y[y.Length - 4];
                    return CompareInternal(x.Slice(0, x.Length - 4), xRest, y.Slice(0, y.Length - 4), yRest);
                case 5:
                    xRest = x[x.Length - 1];
                    xRest <<= 8;
                    xRest |= x[x.Length - 2];
                    xRest <<= 8;
                    xRest |= x[x.Length - 3];
                    xRest <<= 8;
                    xRest |= x[x.Length - 4];
                    xRest <<= 8;
                    xRest |= x[x.Length - 5];
                    yRest = y[y.Length - 1];
                    yRest <<= 8;
                    yRest |= y[y.Length - 2];
                    yRest <<= 8;
                    yRest |= y[y.Length - 3];
                    yRest <<= 8;
                    yRest |= y[y.Length - 4];
                    yRest <<= 8;
                    yRest |= y[y.Length - 5];
                    return CompareInternal(x.Slice(0, x.Length - 5), xRest, y.Slice(0, y.Length - 5), yRest);
                case 6:
                    xRest = x[x.Length - 1];
                    xRest <<= 8;
                    xRest |= x[x.Length - 2];
                    xRest <<= 8;
                    xRest |= x[x.Length - 3];
                    xRest <<= 8;
                    xRest |= x[x.Length - 4];
                    xRest <<= 8;
                    xRest |= x[x.Length - 5];
                    xRest <<= 8;
                    xRest |= x[x.Length - 6];
                    yRest = y[y.Length - 1];
                    yRest <<= 8;
                    yRest |= y[y.Length - 2];
                    yRest <<= 8;
                    yRest |= y[y.Length - 3];
                    yRest <<= 8;
                    yRest |= y[y.Length - 4];
                    yRest <<= 8;
                    yRest |= y[y.Length - 5];
                    yRest <<= 8;
                    yRest |= y[y.Length - 6];
                    return CompareInternal(x.Slice(0, x.Length - 6), xRest, y.Slice(0, y.Length - 6), yRest);
                case 7:
                    xRest = x[x.Length - 1];
                    xRest <<= 8;
                    xRest |= x[x.Length - 2];
                    xRest <<= 8;
                    xRest |= x[x.Length - 3];
                    xRest <<= 8;
                    xRest |= x[x.Length - 4];
                    xRest <<= 8;
                    xRest |= x[x.Length - 5];
                    xRest <<= 8;
                    xRest |= x[x.Length - 6];
                    xRest <<= 8;
                    xRest |= x[x.Length - 7];
                    yRest = y[y.Length - 1];
                    yRest <<= 8;
                    yRest |= y[y.Length - 2];
                    yRest <<= 8;
                    yRest |= y[y.Length - 3];
                    yRest <<= 8;
                    yRest |= y[y.Length - 4];
                    yRest <<= 8;
                    yRest |= y[y.Length - 5];
                    yRest <<= 8;
                    yRest |= y[y.Length - 6];
                    yRest <<= 8;
                    yRest |= y[y.Length - 7];
                    return CompareInternal(x.Slice(0, x.Length - 7), xRest, y.Slice(0, y.Length - 7), yRest);
            }
        }

        private static int CompareInternal(ReadOnlySpan<byte> x, ulong xRest, ReadOnlySpan<byte> y, ulong yRest)
        {
            while (true)
            {
                if (x.IsEmpty)
                {
                    if (xRest == yRest)
                    {
                        return 0;
                    }

                    return xRest < yRest ? -1 : 1;
                }

                var xValue = BinaryPrimitives.ReadUInt64LittleEndian(x);
                var yValue = BinaryPrimitives.ReadUInt64LittleEndian(y);
                if (xValue != yValue)
                {
                    return xValue > yValue ? 1 : -1;
                }

                x = x.Slice(8);
                y = y.Slice(8);
            }
        }
    }
}
