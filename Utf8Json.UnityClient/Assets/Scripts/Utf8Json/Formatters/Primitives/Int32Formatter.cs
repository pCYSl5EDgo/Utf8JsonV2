// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class Int32Formatter : IJsonFormatter<int>
    {
#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static int DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt32();
        }

        public int Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt32();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, int value)
        {
            if (int.MinValue == value)
            {
                return 11;
            }

            var isNegative = value < 0;
            var num1 = (uint)(isNegative ? -value : value);
            // 4294967295 == uint.MaxValue
            if (num1 < 10000)
            {
                if (num1 < 10)
                {
                    return isNegative ? 2 : 1;
                }

                if (num1 < 100)
                {
                    return isNegative ? 3 : 2;
                }

                if (num1 < 1000)
                {
                    return isNegative ? 4 : 3;
                }

                return isNegative ? 5 : 4;
            }

            if (num1 < 10000_0000)
            {
                if (num1 < 10_0000)
                {
                    return isNegative ? 6 : 5;
                }

                if (num1 < 100_0000)
                {
                    return isNegative ? 7 : 6;
                }

                if (num1 < 1000_0000)
                {
                    return isNegative ? 8 : 7;
                }

                return isNegative ? 9 : 8;
            }

            if (num1 < 1_0_0000_0000)
                return isNegative ? 10 : 9;

            return isNegative ? 11 : 10;
        }

        public static void SerializeSpan(JsonSerializerOptions options, int value, Span<byte> span)
        {
            if (value == int.MinValue)
            {
                span[0] = (byte)'-';
                span[1] = (byte)'2';
                span[2] = (byte)'1';
                span[3] = (byte)'4';
                span[4] = (byte)'7';
                span[5] = (byte)'4';
                span[6] = (byte)'8';
                span[7] = (byte)'3';
                span[8] = (byte)'6';
                span[9] = (byte)'4';
                span[10] = (byte)'8';
                return;
            }

            if (value < 0)
            {
                span[0] = (byte)'-';
                span = span.Slice(1);
                value = -value;
            }

            var num1 = (uint)value;
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
            if (!(value is int innerValue))
            {
                throw new ArgumentNullException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            var answer = reader.ReadInt32();
            if (answer == -1) return ObjectHelper.Int32Array[256];
            if (answer >= 0 && answer < 256) return ObjectHelper.Int32Array[answer];
            return answer;
        }
    }
}
