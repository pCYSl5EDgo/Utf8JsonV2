// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class UriFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Uri?>
#else
        : IJsonFormatter<Uri>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Uri? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Uri value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.Write(value.ToString());
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Uri? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Uri value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.Write(value.ToString());
        }

#if CSHARP_8_OR_NEWER
        public Uri? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Uri Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            var str = reader.ReadString();
            if (str == null)
            {
                return default;
            }

            return new Uri(str);
        }

#if CSHARP_8_OR_NEWER
        public static Uri? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Uri DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            var str = reader.ReadString();
            if (str == null)
            {
                return default;
            }

            return new Uri(str);
        }
    }
}
