// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class ByteFormatter : IJsonFormatter<byte>
    {
#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static byte DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadByte();
        }

        public byte Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadByte();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, byte value)
        {
            if (value < 10) return 1;
            if (value < 100) return 2;
            return 3;
        }

        public static void SerializeSpan(JsonSerializerOptions options, byte value, Span<byte> span)
        {
            var num1 = (uint)value;
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
                // ReSharper disable once RedundantCaseLabel
                case 0:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is byte innerValue))
            {
                throw new ArgumentNullException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            var answer = reader.ReadByte();
            return ObjectHelper.ByteArray[answer];
        }
    }
}
