// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.InteropServices;

namespace Utf8Json.Internal.DoubleConversion
{
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    internal struct IeeeDecimal
    {
        [FieldOffset(0)] public decimal Value;
        [FieldOffset(0)] public ulong Value0;
        [FieldOffset(0)] public byte Byte0;

        [FieldOffset(1)] public byte Byte1;
        [FieldOffset(2)] public byte Byte2;
        [FieldOffset(3)] public byte Byte3;
        [FieldOffset(4)] public byte Byte4;
        [FieldOffset(5)] public byte Byte5;
        [FieldOffset(6)] public byte Byte6;
        [FieldOffset(7)] public byte Byte7;

        [FieldOffset(8)] public ulong Value1;
        [FieldOffset(8)] public byte Byte8;

        [FieldOffset(9)] public byte Byte9;
        [FieldOffset(10)] public byte ByteA;
        [FieldOffset(11)] public byte ByteB;
        [FieldOffset(12)] public byte ByteC;
        [FieldOffset(13)] public byte ByteD;
        [FieldOffset(14)] public byte ByteE;
        [FieldOffset(15)] public byte ByteF;

#if DEBUG
        public override string ToString()
        {
            return new System.Text.StringBuilder()
                .Append(Value.ToString(CultureInfo.InvariantCulture))
                .Append(" : ")
                .Append(Byte0.ToString("X2")).Append(", ")
                .Append(Byte1.ToString("X2")).Append(", ")
                .Append(Byte2.ToString("X2")).Append(", ")
                .Append(Byte3.ToString("X2")).Append(", ")
                .Append(Byte4.ToString("X2")).Append(", ")
                .Append(Byte5.ToString("X2")).Append(", ")
                .Append(Byte6.ToString("X2")).Append(", ")
                .Append(Byte7.ToString("X2")).Append(", ")
                .Append(Byte8.ToString("X2")).Append(", ")
                .Append(Byte9.ToString("X2")).Append(", ")
                .Append(ByteA.ToString("X2")).Append(", ")
                .Append(ByteB.ToString("X2")).Append(", ")
                .Append(ByteC.ToString("X2")).Append(", ")
                .Append(ByteD.ToString("X2")).Append(", ")
                .Append(ByteE.ToString("X2")).Append(", ")
                .Append(ByteF.ToString("X2")).Append(", ")
                .ToString();
        }
#endif
    }
}