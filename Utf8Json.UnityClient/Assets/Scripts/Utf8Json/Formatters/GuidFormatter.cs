// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class GuidFormatter : IJsonFormatter<Guid>, IObjectPropertyNameFormatter<Guid>
    {
        public void Serialize(ref JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public Guid Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(38);
            span[0] = (byte)'\"';

            new GuidBits(ref value).Write(span.Slice(1)); // len = 36

            span[37] = (byte)'\"';
        }

        public static Guid DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.SkipWhiteSpace();
            var segment = reader.ReadNotNullStringSegmentRaw();
            return new GuidBits(segment).Value;
        }

        public void SerializeToPropertyName(ref JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public Guid DeserializeFromPropertyName(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is Guid innerValue))
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
