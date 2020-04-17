// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class UInt32Formatter : IJsonFormatter<uint>
    {
        public static void SerializeStatic(ref JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

#pragma warning disable IDE0060
        public static uint DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt32();
        }

        public uint Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt32();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, uint value)
        {
            // 4294967295 == uint.MaxValue
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

            return value < 1_0_0000_0000 ? 9 : 10;
        }

        public static void SerializeSpan(JsonSerializerOptions options, uint value, Span<byte> span)
        {
            var num1 = value;
            var num2 = num1 / 10000;
            num1 -= num2 * 10000;
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            var offset = 0;
            uint div;
            switch (span.Length)
            {
                case 10:
                    span[offset++] = (byte)('0' + (div = (num3 * 6554U) >> 16));
                    num3 -= div * 10U;
                    goto case 9;
                case 9:
                    span[offset++] = (byte)('0' + num3);
                    goto case 8;
                case 8:
                    span[offset++] = (byte)('0' + (div = (num2 * 8389U) >> 23));
                    num2 -= div * 1000U;
                    goto case 7;
                case 7:
                    span[offset++] = (byte)('0' + (div = (num2 * 5243U) >> 19));
                    num2 -= div * 100U;
                    goto case 6;
                case 6:
                    span[offset++] = (byte)('0' + (div = (num2 * 6554U) >> 16));
                    num2 -= div * 10U;
                    goto case 5;
                case 5:
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

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is uint innerValue))
            {
                throw new NullReferenceException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            var answer = DeserializeStatic(ref reader, options);
            return answer < 256 ? ObjectHelper.UInt32Array[answer] : answer;
        }
    }
}
