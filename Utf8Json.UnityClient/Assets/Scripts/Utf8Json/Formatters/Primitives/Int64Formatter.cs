// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class Int64Formatter : IJsonFormatter<long>
    {
#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static long DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt64();
        }

        public long Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt64();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, long value)
        {
            if (value == long.MinValue)
            {
                return 20;
            }
            var answer = 0;
            if (value < 0)
            {
                answer = 1;
                value = -value;
            }

            var value1 = (ulong)value;
            if (value1 < 10000)
            {
                if (value1 < 10)
                {
                    answer += 1;
                }
                else
                {
                    if (value1 < 100)
                    {
                        answer += 2;
                    }
                    else
                    {
                        answer += value1 < 1000 ? 3 : 4;
                    }
                }
            }
            else if (value1 < 10000_0000)
            {
                if (value1 < 10_0000)
                {
                    answer += 5;
                }
                else
                {
                    if (value1 < 100_0000)
                    {
                        answer += 6;
                    }
                    else
                    {
                        answer += value1 < 1000_0000 ? 7 : 8;
                    }
                }
            }
            else if (value1 < 10000_0000_0000)
            {
                if (value1 < 10_0000_0000)
                {
                    answer += 9;
                }
                else
                {
                    if (value1 < 100_0000_0000)
                    {
                        answer += 10;
                    }
                    else
                    {
                        answer += value1 < 1000_0000_0000 ? 11 : 12;
                    }
                }
            }
            else if (value1 < 10000_0000_0000_0000)
            {
                if (value1 < 10_0000_0000_0000)
                {
                    answer += 13;
                }
                else if (value1 < 100_0000_0000_0000)
                {
                    answer += 14;
                }
                else
                {
                    answer += value1 < 1000_0000_0000_0000 ? 15 : 16;
                }
            }
            else if (value1 < 10_0000_0000_0000_0000)
            {
                answer += 17;
            }
            else if (value1 < 100_0000_0000_0000_0000)
            {
                answer += 18;
            }
            else if (value1 < 1000_0000_0000_0000_0000)
            {
                answer += 19;
            }
            else
            {
                answer += 20;
            }

            return answer;
        }

        public static void SerializeSpan(JsonSerializerOptions options, long value, Span<byte> span)
        {
            if (value == long.MinValue)
            {
                span[0] = (byte)'-';
                span[1] = (byte)'9';
                span[2] = (byte)'2';
                span[3] = (byte)'2';
                span[4] = (byte)'3';
                span[5] = (byte)'3';
                span[6] = (byte)'7';
                span[7] = (byte)'2';
                span[8] = (byte)'0';
                span[9] = (byte)'3';
                span[10] = (byte)'6';
                span[11] = (byte)'8';
                span[12] = (byte)'5';
                span[13] = (byte)'4';
                span[14] = (byte)'7';
                span[15] = (byte)'7';
                span[16] = (byte)'5';
                span[17] = (byte)'8';
                span[18] = (byte)'0';
                span[19] = (byte)'8';
                return;
            }

            if (value < 0)
            {
                span[0] = (byte)'-';
                span = span.Slice(1);
                value = -value;
            }

            const int constantDiv = 10000;
            var num1 = (ulong)value;
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
                    span[offset++] = (byte)('0' + (div = (num5 * 8389UL) >> 23));
                    num5 -= div * 1000;
                    goto case 19;
                case 19:
                    span[offset++] = (byte)('0' + (div = (num5 * 5243UL) >> 19));
                    num5 -= div * 100;
                    goto case 18;
                case 18:
                    span[offset++] = (byte)('0' + (div = (num5 * 6554UL) >> 16));
                    num5 -= div * 10;
                    goto case 17;
                case 17:
                    span[offset++] = (byte)('0' + num5);
                    goto case 16;
                case 16:
                    span[offset++] = (byte)('0' + (div = (num4 * 8389UL) >> 23));
                    num4 -= div * 1000;
                    goto case 15;
                case 15:
                    span[offset++] = (byte)('0' + (div = (num4 * 5243UL) >> 19));
                    num4 -= div * 100;
                    goto case 14;
                case 14:
                    span[offset++] = (byte)('0' + (div = (num4 * 6554UL) >> 16));
                    num4 -= div * 10; goto case 13;
                case 13:
                    span[offset++] = (byte)('0' + num4);
                    goto case 12;
                case 12:
                    span[offset++] = (byte)('0' + (div = (num3 * 8389UL) >> 23));
                    num3 -= div * 1000;
                    goto case 11;
                case 11:
                    span[offset++] = (byte)('0' + (div = (num3 * 5243UL) >> 19));
                    num3 -= div * 100;
                    goto case 10;
                case 10:
                    span[offset++] = (byte)('0' + (div = (num3 * 6554UL) >> 16));
                    num3 -= div * 10U;
                    goto case 9;
                case 9:
                    span[offset++] = (byte)('0' + num3);
                    goto case 8;
                case 8:
                    span[offset++] = (byte)('0' + (div = (num2 * 8389UL) >> 23));
                    num2 -= div * 1000U;
                    goto case 7;
                case 7:
                    span[offset++] = (byte)('0' + (div = (num2 * 5243UL) >> 19));
                    num2 -= div * 100U;
                    goto case 6;
                case 6:
                    span[offset++] = (byte)('0' + (div = (num2 * 6554UL) >> 16));
                    num2 -= div * 10U;
                    goto case 5;
                case 5:
                    span[offset++] = (byte)('0' + num2);
                    goto case 4;
                case 4:
                    span[offset++] = (byte)('0' + (div = (num1 * 8389UL) >> 23));
                    num1 -= div * 1000U;
                    goto case 3;
                case 3:
                    span[offset++] = (byte)('0' + (div = (num1 * 5243UL) >> 19));
                    num1 -= div * 100U;
                    goto case 2;
                case 2:
                    span[offset++] = (byte)('0' + (div = (num1 * 6554UL) >> 16));
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
            if (!(value is long innerValue))
            {
                throw new ArgumentNullException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            var answer = reader.ReadInt64();
            if (answer == -1) return ObjectHelper.Int64Array[0];
            if (answer >= 0 && answer < 256) return ObjectHelper.Int64Array[answer + 1];
            return answer;
        }
    }
}
