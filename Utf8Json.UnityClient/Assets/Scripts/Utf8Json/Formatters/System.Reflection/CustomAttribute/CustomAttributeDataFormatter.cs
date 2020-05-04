// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class CustomAttributeDataFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<CustomAttributeData?>
#else
        : IJsonFormatter<CustomAttributeData>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as CustomAttributeData, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.ReadNextBlock();
            return default;
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, CustomAttributeData? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, CustomAttributeData value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, CustomAttributeData? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, CustomAttributeData value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyObject();
                return;
            }

            ++writer.Depth;
            writer.WriteRaw(new[] {
                (byte)'{',
                (byte)'"',
                (byte)'A',
                (byte)'t',
                (byte)'t',
                (byte)'r',
                (byte)'i',
                (byte)'b',
                (byte)'u',
                (byte)'t',
                (byte)'e',
                (byte)'T',
                (byte)'y',
                (byte)'p',
                (byte)'e',
                (byte)'"',
                (byte)':',
            });
            TypeFormatter.SerializeStatic(ref writer, value.AttributeType, options);

            writer.WriteRaw(new[] {
                (byte)',',
                (byte)'"',
                (byte)'C',
                (byte)'o',
                (byte)'n',
                (byte)'s',
                (byte)'t',
                (byte)'r',
                (byte)'u',
                (byte)'c',
                (byte)'t',
                (byte)'o',
                (byte)'r',
                (byte)'"',
                (byte)':',
            });
            ConstructorInfoFormatter.SerializeStatic(ref writer, value.Constructor, options);
            writer.WriteEndObject();
            --writer.Depth;
        }

#if CSHARP_8_OR_NEWER
        public CustomAttributeData? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public CustomAttributeData Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.ReadNextBlock();
            return default;
        }

#if CSHARP_8_OR_NEWER
        public static CustomAttributeData? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static CustomAttributeData DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.ReadNextBlock();
            return default;
        }
    }
}
