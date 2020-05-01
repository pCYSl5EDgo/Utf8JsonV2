// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class FieldInfoFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<FieldInfo?>
#else
        : IJsonFormatter<FieldInfo>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as FieldInfo, options);
        }

        public object
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, FieldInfo? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, FieldInfo value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            MemberInfoFormatterHelper.SerializeStaticWithoutEndObject(ref writer, value, options);
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, FieldInfo? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, FieldInfo value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

        public FieldInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static FieldInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            var (name, declaringType) = MemberInfoFormatterHelper.ReadNameAndDeclaringType(ref reader, options);

            var answer = declaringType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            return answer;
        }
    }
}
