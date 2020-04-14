// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class UInt64Formatter : IJsonFormatter<ulong>
    {
        public static void SerializeStatic(ref JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static ulong DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt64();
        }

        public ulong Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt64();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ulong value)
        {
            if (value < 10000)
            {
                if (value < 10)
                {
                    return 1;
                }

                if (value < 100)
                {
                    return 2;
                }

                return value < 1000 ? 3 : 4;
            }

            if (value < 10000_0000)
            {
                if (value < 10_0000)
                {
                    return 5;
                }

                if (value < 100_0000)
                {
                    return 6;
                }

                return value < 1000_0000 ? 7 : 8;
            }

            if (value < 10000_0000_0000)
            {
                if (value < 10_0000_0000)
                {
                    return 9;
                }

                if (value < 100_0000_0000)
                {
                    return 10;
                }

                return value < 1000_0000_0000 ? 11 : 12;
            }

            if (value < 10000_0000_0000_0000)
            {
                if (value < 10_0000_0000_0000)
                {
                    return 13;
                }

                if (value < 100_0000_0000_0000)
                {
                    return 14;
                }

                return value < 1000_0000_0000_0000 ? 15 : 16;
            }

            if (value < 10_0000_0000_0000_0000)
            {
                return 17;
            }

            if (value < 100_0000_0000_0000_0000)
            {
                return 18;
            }

            return value < 1000_0000_0000_0000_0000 ? 19 : 20;
        }

        public static void SerializeSpan(JsonSerializerOptions options, ulong value, Span<byte> span)
        {
            const int constantDiv = 10000;
            var num1 = value;
            var num2 = num1 / constantDiv;
            num1 -= num2 * constantDiv;
            var num3 = num2 / constantDiv;
            num2 -= num3 * constantDiv;
            var num4 = num3 / constantDiv;
            num3 -= num4 * constantDiv;
            var num5 = num4 / constantDiv;
            num4 -= num5 * constantDiv;
            var offset = 0;
            ulong div;
            switch (span.Length)
            {
            case 20:
                span[offset++] = (byte) ('0' + (div = (num5 * 8389UL) >> 23));
                num5 -= div * 1000;
                goto case 19;
            case 19:
                span[offset++] = (byte) ('0' + (div = (num5 * 5243UL) >> 19));
                num5 -= div * 100;
                goto case 18;
            case 18:
                span[offset++] = (byte) ('0' + (div = (num5 * 6554UL) >> 16));
                num5 -= div * 10;
                goto case 17;
            case 17:
                span[offset++] = (byte) ('0' + num5);
                goto case 16;
            case 16:
                span[offset++] = (byte) ('0' + (div = (num4 * 8389UL) >> 23));
                num4 -= div * 1000;
                goto case 15;
            case 15:
                span[offset++] = (byte) ('0' + (div = (num4 * 5243UL) >> 19));
                num4 -= div * 100;
                goto case 14;
            case 14:
                span[offset++] = (byte) ('0' + (div = (num4 * 6554UL) >> 16));
                num4 -= div * 10;
                goto case 13;
            case 13:
                span[offset++] = (byte) ('0' + num4);
                goto case 12;
            case 12:
                span[offset++] = (byte) ('0' + (div = (num3 * 8389UL) >> 23));
                num3 -= div * 1000;
                goto case 11;
            case 11:
                span[offset++] = (byte) ('0' + (div = (num3 * 5243UL) >> 19));
                num3 -= div * 100;
                goto case 10;
            case 10:
                span[offset++] = (byte) ('0' + (div = (num3 * 6554UL) >> 16));
                num3 -= div * 10U;
                goto case 9;
            case 9:
                span[offset++] = (byte) ('0' + num3);
                goto case 8;
            case 8:
                span[offset++] = (byte) ('0' + (div = (num2 * 8389UL) >> 23));
                num2 -= div * 1000U;
                goto case 7;
            case 7:
                span[offset++] = (byte) ('0' + (div = (num2 * 5243UL) >> 19));
                num2 -= div * 100U;
                goto case 6;
            case 6:
                span[offset++] = (byte) ('0' + (div = (num2 * 6554UL) >> 16));
                num2 -= div * 10U;
                goto case 5;
            case 5:
                span[offset++] = (byte) ('0' + num2);
                goto case 4;
            case 4:
                span[offset++] = (byte) ('0' + (div = (num1 * 8389UL) >> 23));
                num1 -= div * 1000U;
                goto case 3;
            case 3:
                span[offset++] = (byte) ('0' + (div = (num1 * 5243UL) >> 19));
                num1 -= div * 100U;
                goto case 2;
            case 2:
                span[offset++] = (byte) ('0' + (div = (num1 * 6554UL) >> 16));
                num1 -= div * 10U;
                goto case 1;
            case 1:
                span[offset] = (byte) ('0' + num1);
                return;
            // ReSharper disable once RedundantCaseLabel
            case 0:
            default:
                throw new JsonSerializationException("Invalid number.");
            }
        }
    }
}
