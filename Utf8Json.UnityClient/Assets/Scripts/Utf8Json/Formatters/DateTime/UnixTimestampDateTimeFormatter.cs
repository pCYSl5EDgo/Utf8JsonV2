// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class UnixTimestampDateTimeFormatter : IJsonFormatter<DateTime>
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Serialize(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value);
        }

        public static void SerializeStatic(ref JsonWriter writer, DateTime value)
        {
            var ticks = (long)(value.ToUniversalTime() - unixEpoch).TotalSeconds;
            writer.WriteQuotation();
            writer.Write(ticks);
            writer.WriteQuotation();
        }

        public DateTime Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static DateTime DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.SkipWhiteSpace();
            var str = reader.ReadNotNullStringSegmentRaw();
            if (IntegerConverter.TryRead(str, out ulong ticks))
            {
                return unixEpoch.AddSeconds(ticks);
            }

            throw new JsonParsingException("Invalid Unix timestamp.");
        }
    }
}