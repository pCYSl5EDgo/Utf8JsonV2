// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
#if CSHARP_8_OR_NEWER
    public delegate void JsonSerializeReferenceTypeAction<T>(ref JsonWriter writer, T? value, JsonSerializerOptions options) where T : class;
    public delegate T? JsonDeserializeReferenceTypeFunc<T>(ref JsonReader reader, JsonSerializerOptions options) where T : class;
#else
    public delegate void JsonSerializeReferenceTypeAction<T>(ref JsonWriter writer, T value, JsonSerializerOptions options) where T : class;
    public delegate T JsonDeserializeReferenceTypeFunc<T>(ref JsonReader reader, JsonSerializerOptions options) where T : class;
#endif

    public delegate void JsonSerializeValueTypeAction<T>(ref JsonWriter writer, ref T value, JsonSerializerOptions options) where T : struct;
    public delegate T JsonDeserializeValueTypeFunc<T>(ref JsonReader reader, JsonSerializerOptions options) where T : struct;

    public sealed class DynamicMethodValueTypeFormatter<T>
        : IJsonFormatter<T>
        where T : struct
    {
        private readonly JsonSerializeValueTypeAction<T> serialize;
        private readonly JsonDeserializeValueTypeFunc<T> deserialize;

        public DynamicMethodValueTypeFormatter(JsonSerializeValueTypeAction<T> serialize, JsonDeserializeValueTypeFunc<T> deserialize)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                throw new ArgumentNullException();
            }

            var innerValue = (T)value;
            serialize(ref writer, ref innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return deserialize(ref reader, options);
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            serialize(ref writer, ref value, options);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return deserialize(ref reader, options);
        }
    }

    public sealed class DynamicMethodReferenceTypeFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T?>
#else
        : IJsonFormatter<T>
#endif
        where T : class
    {
        private readonly JsonSerializeReferenceTypeAction<T> serialize;
        private readonly JsonDeserializeReferenceTypeFunc<T> deserialize;

        public DynamicMethodReferenceTypeFormatter(JsonSerializeReferenceTypeAction<T> serialize, JsonDeserializeReferenceTypeFunc<T> deserialize)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value is T innerValue)
            {
                serialize(ref writer, innerValue, options);
            }
            else
            {
                writer.WriteNull();
            }
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return deserialize(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
#endif
        {
            serialize(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return deserialize(ref reader, options);
        }
    }
}
