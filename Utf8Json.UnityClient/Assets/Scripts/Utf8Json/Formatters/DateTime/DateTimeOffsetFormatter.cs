// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class DateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        public void Serialize(ref JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SerializeStatic(ref JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public DateTimeOffset Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            var str = reader.ReadString();
            if (str == null) throw new JsonParsingException("DateTimeOffset cannot be null.");
            return DateTimeOffset.Parse(str, CultureInfo.InvariantCulture);
        }

        public static DateTimeOffset DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var str = reader.ReadString();
            if (str == null) throw new JsonParsingException("DateTimeOffset cannot be null.");
            return DateTimeOffset.Parse(str, CultureInfo.InvariantCulture);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is DateTimeOffset innerValue))
            {
                throw new ArgumentNullException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
