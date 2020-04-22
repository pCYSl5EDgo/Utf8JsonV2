// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable IDE0060
using System;

namespace Utf8Json.Formatters
{
    public sealed class VersionFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Version?>
#else
        : IJsonFormatter<Version>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Version? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Version value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Version? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Version value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            writer.Write(value.ToString());
        }

#if CSHARP_8_OR_NEWER
        public Version? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Version Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Version? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Version DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            var str = reader.ReadString();
            if (str == null)
            {
                return default;
            }

            return new Version(str);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Version, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
