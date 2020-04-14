// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class SByteFormatter : IJsonFormatter<sbyte>
    {
        public static void SerializeStatic(ref JsonWriter writer, sbyte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, sbyte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static sbyte DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSByte();
        }

        public sbyte Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSByte();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, sbyte value)
        {
            return Int32Formatter.CalcByteLengthForSerialization(options, value);
        }

        public static void SerializeSpan(JsonSerializerOptions options, sbyte value, Span<byte> span)
        {
            int num1 = value;
            if (num1 < 0)
            {
                span[0] = (byte)'-';
                span = span.Slice(1);
                num1 = -num1;
            }

            var offset = 0;
            switch (span.Length)
            {
                case 3:
                    var hundred = num1 / 100;
                    span[offset++] = (byte)(hundred + '0');
                    num1 -= hundred * 100;
                    goto case 2;
                case 2:
                    var ten = num1 / 10;
                    span[offset++] = (byte)(ten + '0');
                    num1 -= ten * 10;
                    goto case 1;
                case 1:
                    span[offset] = (byte)(num1 + '0');
                    return;
                case 0:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
