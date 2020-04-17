// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Formatters
{
    public sealed class ReferenceTypeReflectionFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T?>
#else
        : IJsonFormatter<T>
#endif
        where T : class, new()
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
#endif
        {
        }

#if CSHARP_8_OR_NEWER
        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            throw new System.NotImplementedException();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as T, options);
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
