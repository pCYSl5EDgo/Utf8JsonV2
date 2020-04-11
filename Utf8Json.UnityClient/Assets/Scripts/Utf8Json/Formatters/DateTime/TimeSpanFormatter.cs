// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class TimeSpanFormatter : IJsonFormatter<TimeSpan>
    {
        public TimeSpanFormatter()
        {
        }

        public void Serialize(ref JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.Write(value.ToString());
        }

        public TimeSpan Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static TimeSpan DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var str = reader.ReadString();
            if (str == null) throw new JsonParsingException("TimeSpan should not be null.");
            return TimeSpan.Parse(str, CultureInfo.InvariantCulture);
        }
    }

    public sealed class NullableTimeSpanFormatter : IJsonFormatter<TimeSpan?>
    {
        public NullableTimeSpanFormatter()
        {
        }

        public void Serialize(ref JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value == null) { writer.WriteNull(); return; }

            TimeSpanFormatter.SerializeStatic(ref writer, value.Value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value == null) { writer.WriteNull(); return; }

            TimeSpanFormatter.SerializeStatic(ref writer, value.Value, options);
        }

        public TimeSpan? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull()) return null;

            return TimeSpanFormatter.DeserializeStatic(ref reader, options);
        }

        public static TimeSpan? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull()) return null;

            return TimeSpanFormatter.DeserializeStatic(ref reader, options);
        }
    }
}