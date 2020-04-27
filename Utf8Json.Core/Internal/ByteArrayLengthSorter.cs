// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

// ReSharper disable UseIndexFromEndExpression

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

            switch (x.Length - ((x.Length >> 3) << 3))
            {
                default: return CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                case 1:
                    var c1 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    return c1 != 0 ? c1 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
                case 2:
                    var c2 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    if (c2 == 0)
                    {
                        c2 = x[x.Length - 2].CompareTo(y[y.Length - 2]);
                    }
                    return c2 != 0 ? c2 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
                case 3:
                    var c3 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    if (c3 == 0)
                    {
                        c3 = x[x.Length - 3].CompareTo(y[y.Length - 3]);
                    }
                    if (c3 == 0)
                    {
                        c3 = x[x.Length - 2].CompareTo(y[y.Length - 2]);
                    }
                    return c3 != 0 ? c3 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
                case 4:
                    var c4 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    if (c4 == 0)
                    {
                        const int minus = 4;
                        c4 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c4 == 0)
                    {
                        const int minus = 3;
                        c4 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c4 == 0)
                    {
                        const int minus = 2;
                        c4 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    return c4 != 0 ? c4 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
                case 5:
                    var c5 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    if (c5 == 0)
                    {
                        const int minus = 5;
                        c5 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c5 == 0)
                    {
                        const int minus = 4;
                        c5 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c5 == 0)
                    {
                        const int minus = 3;
                        c5 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c5 == 0)
                    {
                        const int minus = 2;
                        c5 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    return c5 != 0 ? c5 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
                case 6:
                    var c6 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    if (c6 == 0)
                    {
                        const int minus = 6;
                        c6 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c6 == 0)
                    {
                        const int minus = 5;
                        c6 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c6 == 0)
                    {
                        const int minus = 4;
                        c6 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c6 == 0)
                    {
                        const int minus = 3;
                        c6 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c6 == 0)
                    {
                        const int minus = 2;
                        c6 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    return c6 != 0 ? c6 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
                case 7:
                    var c7 = CompareInternal(MemoryMarshal.Cast<byte, ulong>(x), MemoryMarshal.Cast<byte, ulong>(y));
                    if (c7 == 0)
                    {
                        const int minus = 7;
                        c7 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c7 == 0)
                    {
                        const int minus = 6;
                        c7 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c7 == 0)
                    {
                        const int minus = 5;
                        c7 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c7 == 0)
                    {
                        const int minus = 4;
                        c7 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c7 == 0)
                    {
                        const int minus = 3;
                        c7 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    if (c7 == 0)
                    {
                        const int minus = 2;
                        c7 = x[x.Length - minus].CompareTo(y[y.Length - minus]);
                    }
                    return c7 != 0 ? c7 : x[x.Length - 1].CompareTo(y[y.Length - 1]);
            }
        }

        private static int CompareInternal(ReadOnlySpan<ulong> x, ReadOnlySpan<ulong> y)
        {
            while (true)
            {
                if (x.IsEmpty)
                {
                    return 0;
                }

                var xValue = x[0];
                var yValue = y[0];
                if (xValue != yValue)
                {
                    return xValue > yValue ? 1 : -1;
                }

                x = x.Slice(1);
                y = y.Slice(1);
            }
        }
    }
}
