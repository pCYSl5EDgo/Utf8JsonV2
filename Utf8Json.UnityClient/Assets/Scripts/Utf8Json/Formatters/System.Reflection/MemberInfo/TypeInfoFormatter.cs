// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class TypeInfoFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<TypeInfo?>
#else
        : IJsonFormatter<TypeInfo>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as TypeInfo, options);
        }

        public object
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return default;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, TypeInfo? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, TypeInfo value, JsonSerializerOptions options)
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
            MemberInfoFormatterHelper.SerializeStaticWithoutEndObject(ref writer, value, options);
            writer.WriteEndObject();
            --writer.Depth;
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, TypeInfo? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, TypeInfo value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

        public TypeInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return default;
        }

        public static TypeInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return default;
        }
    }
}
