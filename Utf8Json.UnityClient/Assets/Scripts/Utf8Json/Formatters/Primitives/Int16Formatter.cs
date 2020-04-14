// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class Int16Formatter : IJsonFormatter<short>
    {
        public static void SerializeStatic(ref JsonWriter writer, short value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, short value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static short DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt16();
        }

        public short Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt16();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, short value)
        {
            return Int32Formatter.CalcByteLengthForSerialization(options, value);
        }

        public static void SerializeSpan(JsonSerializerOptions options, short value, Span<byte> span)
        {
            Int32Formatter.SerializeSpan(options, value, span);
        }
    }
}
