// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Internal
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct GuidBits
    {
        [FieldOffset(0)] public readonly Guid Value;

        [FieldOffset(0)] public readonly byte Byte0;
        [FieldOffset(1)] public readonly byte Byte1;
        [FieldOffset(2)] public readonly byte Byte2;
        [FieldOffset(3)] public readonly byte Byte3;
        [FieldOffset(4)] public readonly byte Byte4;
        [FieldOffset(5)] public readonly byte Byte5;
        [FieldOffset(6)] public readonly byte Byte6;
        [FieldOffset(7)] public readonly byte Byte7;
        [FieldOffset(8)] public readonly byte Byte8;
        [FieldOffset(9)] public readonly byte Byte9;
        [FieldOffset(10)] public readonly byte Byte10;
        [FieldOffset(11)] public readonly byte Byte11;
        [FieldOffset(12)] public readonly byte Byte12;
        [FieldOffset(13)] public readonly byte Byte13;
        [FieldOffset(14)] public readonly byte Byte14;
        [FieldOffset(15)] public readonly byte Byte15;

        public GuidBits(ref Guid value)
        {
            this = default;
            this.Value = value;
        }

        // 4-pattern, lower/upper and '-' or no
        public GuidBits(in ReadOnlySpan<byte> str)
        {
            this = default;

            switch (str.Length)
            {
                case 32:
                    {
                        if (BitConverter.IsLittleEndian)
                        {
                            this.Byte0 = Parse(str, 6);
                            this.Byte1 = Parse(str, 4);
                            this.Byte2 = Parse(str, 2);
                            this.Byte3 = Parse(str, 0);

                            this.Byte4 = Parse(str, 10);
                            this.Byte5 = Parse(str, 8);

                            this.Byte6 = Parse(str, 14);
                            this.Byte7 = Parse(str, 12);
                        }
                        else
                        {
                            this.Byte0 = Parse(str, 0);
                            this.Byte1 = Parse(str, 2);
                            this.Byte2 = Parse(str, 4);
                            this.Byte3 = Parse(str, 6);

                            this.Byte4 = Parse(str, 8);
                            this.Byte5 = Parse(str, 10);

                            this.Byte6 = Parse(str, 12);
                            this.Byte7 = Parse(str, 14);
                        }
                        this.Byte8 = Parse(str, 16);
                        this.Byte9 = Parse(str, 18);

                        this.Byte10 = Parse(str, 20);
                        this.Byte11 = Parse(str, 22);
                        this.Byte12 = Parse(str, 24);
                        this.Byte13 = Parse(str, 26);
                        this.Byte14 = Parse(str, 28);
                        this.Byte15 = Parse(str, 30);
                        return;
                    }
                case 36:
                    {
                        // '-' => 45
                        if (BitConverter.IsLittleEndian)
                        {
                            this.Byte0 = Parse(str, 6);
                            this.Byte1 = Parse(str, 4);
                            this.Byte2 = Parse(str, 2);
                            this.Byte3 = Parse(str, 0);

                            if (str[8] != '-') goto ERROR;

                            this.Byte4 = Parse(str, 11);
                            this.Byte5 = Parse(str, 9);

                            if (str[13] != '-') goto ERROR;

                            this.Byte6 = Parse(str, 16);
                            this.Byte7 = Parse(str, 14);
                        }
                        else
                        {
                            this.Byte0 = Parse(str, 0);
                            this.Byte1 = Parse(str, 2);
                            this.Byte2 = Parse(str, 4);
                            this.Byte3 = Parse(str, 6);

                            if (str[+8] != '-') goto ERROR;

                            this.Byte4 = Parse(str, 9);
                            this.Byte5 = Parse(str, 11);

                            if (str[13] != '-') goto ERROR;

                            this.Byte6 = Parse(str, 14);
                            this.Byte7 = Parse(str, 16);
                        }

                        if (str[18] != '-') goto ERROR;

                        this.Byte8 = Parse(str, 19);
                        this.Byte9 = Parse(str, 21);

                        if (str[23] != '-') goto ERROR;

                        this.Byte10 = Parse(str, 24);
                        this.Byte11 = Parse(str, 26);
                        this.Byte12 = Parse(str, 28);
                        this.Byte13 = Parse(str, 30);
                        this.Byte14 = Parse(str, 32);
                        this.Byte15 = Parse(str, 34);
                        return;
                    }
                default:
                ERROR:
                    throw new ArgumentException("Invalid Guid Pattern.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Parse(in ReadOnlySpan<byte> bytes, int highOffset)
        {
            byte b = bytes[highOffset];
            byte ret;
            // '0'(48) ~ '9'(57) => -48
            // 'A'(65) ~ 'F'(70) => -55
            // 'a'(97) ~ 'f'(102) => -87
            switch (b)
            {
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                    ret = unchecked((byte)(b - 48));
                    break;
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                    ret = unchecked((byte)(b - 55));
                    break;
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 102:
                    ret = unchecked((byte)(b - 87));
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                default:
                    throw new ArgumentException("Invalid Guid Pattern.");
            }
            ret <<= 4;
            byte b1 = bytes[highOffset + 1];
            // '0'(48) ~ '9'(57) => -48
            // 'A'(65) ~ 'F'(70) => -55
            // 'a'(97) ~ 'f'(102) => -87
            switch (b1)
            {
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                    ret |= unchecked((byte)(b1 - 48));
                    break;
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                    ret |= unchecked((byte)(b1 - 55));
                    break;
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 102:
                    ret |= unchecked((byte)(b1 - 87));
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                default:
                    throw new ArgumentException("Invalid Guid Pattern.");
            }

            return ret;
        }


        // 4(x2) - 2(x2) - 2(x2) - 2(x2) - 6(x2)
        public void Write(Span<byte> buffer)
        {
            // string.Join(", ", Enumerable.Range(0, 256).Select(x => (int)BitConverter.ToString(new byte[] { (byte)x }).ToLower()[0]))
            ReadOnlySpan<byte> byteToHexStringHigh = new[] { (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)48, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)49, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)50, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)51, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)52, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)53, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)54, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)55, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)56, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)57, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)97, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)98, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)99, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)100, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)101, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102, (byte)102 };
            // string.Join(", ", Enumerable.Range(0, 256).Select(x => (int)BitConverter.ToString(new byte[] { (byte)x }).ToLower()[1]))
            ReadOnlySpan<byte> byteToHexStringLow = new[] { (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56, (byte)57, (byte)97, (byte)98, (byte)99, (byte)100, (byte)101, (byte)102 };
            if (BitConverter.IsLittleEndian)
            {
                // int(_a)
                buffer[6] = byteToHexStringHigh[Byte0];
                buffer[7] = byteToHexStringLow[Byte0];
                buffer[4] = byteToHexStringHigh[Byte1];
                buffer[5] = byteToHexStringLow[Byte1];
                buffer[2] = byteToHexStringHigh[Byte2];
                buffer[3] = byteToHexStringLow[Byte2];
                buffer[0] = byteToHexStringHigh[Byte3];
                buffer[1] = byteToHexStringLow[Byte3];

                buffer[8] = (byte)'-';

                // short(_b)
                buffer[11] = byteToHexStringHigh[Byte4];
                buffer[12] = byteToHexStringLow[Byte4];
                buffer[9] = byteToHexStringHigh[Byte5];
                buffer[10] = byteToHexStringLow[Byte5];

                buffer[13] = (byte)'-';

                // short(_c)
                buffer[16] = byteToHexStringHigh[Byte6];
                buffer[17] = byteToHexStringLow[Byte6];
                buffer[14] = byteToHexStringHigh[Byte7];
                buffer[15] = byteToHexStringLow[Byte7];
            }
            else
            {
                buffer[0] = byteToHexStringHigh[Byte0];
                buffer[1] = byteToHexStringLow[Byte0];
                buffer[2] = byteToHexStringHigh[Byte1];
                buffer[3] = byteToHexStringLow[Byte1];
                buffer[4] = byteToHexStringHigh[Byte2];
                buffer[5] = byteToHexStringLow[Byte2];
                buffer[6] = byteToHexStringHigh[Byte3];
                buffer[7] = byteToHexStringLow[Byte3];

                buffer[8] = (byte)'-';

                buffer[9] = byteToHexStringHigh[Byte4];
                buffer[10] = byteToHexStringLow[Byte4];
                buffer[11] = byteToHexStringHigh[Byte5];
                buffer[12] = byteToHexStringLow[Byte5];

                buffer[13] = (byte)'-';

                buffer[14] = byteToHexStringHigh[Byte6];
                buffer[15] = byteToHexStringLow[Byte6];
                buffer[16] = byteToHexStringHigh[Byte7];
                buffer[17] = byteToHexStringLow[Byte7];
            }

            buffer[18] = (byte)'-';

            buffer[19] = byteToHexStringHigh[Byte8];
            buffer[20] = byteToHexStringLow[Byte8];
            buffer[21] = byteToHexStringHigh[Byte9];
            buffer[22] = byteToHexStringLow[Byte9];

            buffer[23] = (byte)'-';

            buffer[24] = byteToHexStringHigh[Byte10];
            buffer[25] = byteToHexStringLow[Byte10];
            buffer[26] = byteToHexStringHigh[Byte11];
            buffer[27] = byteToHexStringLow[Byte11];
            buffer[28] = byteToHexStringHigh[Byte12];
            buffer[29] = byteToHexStringLow[Byte12];
            buffer[30] = byteToHexStringHigh[Byte13];
            buffer[31] = byteToHexStringLow[Byte13];
            buffer[32] = byteToHexStringHigh[Byte14];
            buffer[33] = byteToHexStringLow[Byte14];
            buffer[34] = byteToHexStringHigh[Byte15];
            buffer[35] = byteToHexStringLow[Byte15];
        }
    }
}
