// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class MethodBaseFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<MethodBase?>
#else
        : IJsonFormatter<MethodBase>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as MethodBase, options);
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
        public static void SerializeStatic(ref JsonWriter writer, MethodBase? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, MethodBase value, JsonSerializerOptions options)
#endif
        {
            switch (value)
            {
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
        public void Serialize(ref JsonWriter writer, MethodBase? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, MethodBase value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

        public MethodBase
#if CSHARP_8_OR_NEWER
            ?
#endif
            Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return default;
        }

        public static MethodBase
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
