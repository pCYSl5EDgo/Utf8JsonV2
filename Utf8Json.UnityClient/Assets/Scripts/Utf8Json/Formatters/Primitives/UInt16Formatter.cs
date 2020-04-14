// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class UInt16Formatter : IJsonFormatter<ushort>
    {
        public static void SerializeStatic(ref JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static ushort DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt16();
        }

        public ushort Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt16();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ushort value)
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

            return 5;
        }

        public static void SerializeSpan(JsonSerializerOptions options, ushort value, Span<byte> span)
        {
            var num1 = (uint)value;
            var offset = 0;
            uint div;
            switch (span.Length)
            {
                case 5:
                    var num2 = num1 / 10000;
                    num1 -= num2 * 10000;
                    span[offset++] = (byte)('0' + num2);
                    goto case 4;
                case 4:
                    span[offset++] = (byte)('0' + (div = (num1 * 8389U) >> 23));
                    num1 -= div * 1000U;
                    goto case 3;
                case 3:
                    span[offset++] = (byte)('0' + (div = (num1 * 5243U) >> 19));
                    num1 -= div * 100U;
                    goto case 2;
                case 2:
                    span[offset++] = (byte)('0' + (div = (num1 * 6554U) >> 16));
                    num1 -= div * 10U;
                    goto case 1;
                case 1:
                    span[offset] = (byte)('0' + num1);
                    return;
                // ReSharper disable once RedundantCaseLabel
                case 0:
                default:
                    throw new JsonSerializationException("Invalid number.");
            }
        }
    }
}
