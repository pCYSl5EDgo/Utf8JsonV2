// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class MemberInfoFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<MemberInfo?>
#else
        : IJsonFormatter<MemberInfo>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as MemberInfo, options);
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
        public static void SerializeStatic(ref JsonWriter writer, MemberInfo? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, MemberInfo value, JsonSerializerOptions options)
#endif
        {
            switch (value)
            {
                case FieldInfo fieldInfo:
                    FieldInfoFormatter.SerializeStatic(ref writer, fieldInfo, options);
                    return;
                case PropertyInfo propertyInfo:
                    PropertyInfoFormatter.SerializeStatic(ref writer, propertyInfo, options);
                    return;
                case TypeInfo typeInfo:
                    TypeInfoFormatter.SerializeStatic(ref writer, typeInfo, options);
                    return;
                case Type type:
                    TypeFormatter.SerializeStatic(ref writer, type, options);
                    return;
                case ConstructorInfo constructorInfo:
                    ConstructorInfoFormatter.SerializeStatic(ref writer, constructorInfo, options);
                    return;
                case MethodInfo methodInfo:
                    MethodInfoFormatter.SerializeStatic(ref writer, methodInfo, options);
                    return;
                default:
                    writer.WriteNull();
                    return;
            }
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, MemberInfo? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, MemberInfo value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

        public MemberInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return default;
        }

        public static MemberInfo
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
