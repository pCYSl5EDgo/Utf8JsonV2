// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
#pragma warning disable IDE0060

namespace Utf8Json.Formatters
{
    public sealed class NullableStringFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<string?>
#else
        : IJsonFormatter<string>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, string? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, string value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public string? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public string Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadString();
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, string? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, string value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            if (value.Length == 0)
            {
                var emptySpan = writer.Writer.GetSpan(2);
                emptySpan[0] = 0x22;
                emptySpan[1] = 0x22;
                writer.Writer.Advance(2);
                return;
            }

            writer.Write(value.AsSpan());
        }

#if CSHARP_8_OR_NEWER
        public static string? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static string DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadString();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as string, options);
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
