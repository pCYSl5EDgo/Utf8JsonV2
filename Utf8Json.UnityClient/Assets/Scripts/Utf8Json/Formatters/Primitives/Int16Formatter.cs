// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class Int16Formatter : IJsonFormatter<short>
    {
#pragma warning disable IDE0060
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

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is short innerValue))
            {
                throw new ArgumentNullException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            var answer = reader.ReadInt16();
            if (answer == -1) return ObjectHelper.Int16Array[256];
            if (answer >= 0 && answer < 256) return ObjectHelper.Int16Array[answer];
            return answer;
        }
    }
}
