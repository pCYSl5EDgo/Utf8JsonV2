// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Utf8Json.Internal.DoubleConversion
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct UnionDoubleULong
    {
        [FieldOffset(0)]
        public double d;
        [FieldOffset(0)]
        public ulong u64;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct UnionFloatUInt
    {
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public uint u32;
    }
}
