// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection.Emit;
using Utf8Json.Internal.Reflection;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
#if CSHARP_8_OR_NEWER
    public delegate void JsonSerializeReferenceTypeAction<T>(ref JsonWriter writer, T? value, JsonSerializerOptions options) where T : class;
    public delegate T? JsonDeserializeReferenceTypeFunc<T>(ref JsonReader reader, JsonSerializerOptions options) where T : class;
#else
    public delegate void JsonSerializeReferenceTypeAction<T>(ref JsonWriter writer, T value, JsonSerializerOptions options) where T : class;
    public delegate T JsonDeserializeReferenceTypeFunc<T>(ref JsonReader reader, JsonSerializerOptions options) where T : class;
#endif

    public delegate void JsonSerializeValueTypeAction<T>(ref JsonWriter writer, T value, JsonSerializerOptions options) where T : struct;
    public delegate T JsonDeserializeValueTypeFunc<T>(ref JsonReader reader, JsonSerializerOptions options) where T : struct;

    public static class DynamicMethodFormatterGenerator
    {
        public static IJsonFormatter CreateFromDynamicMethods(DynamicMethod serialize, DynamicMethod deserialize)
        {
            var targetType = deserialize.ReturnType;
            var length1 = TypeArrayHolder.Length1;
            length1[0] = targetType;
            if (targetType.IsValueType)
            {
                var serializeType = typeof(JsonSerializeValueTypeAction<>).MakeGenericType(length1);
                var deserializeType = typeof(JsonDeserializeValueTypeFunc<>).MakeGenericType(length1);
                var formatterType = typeof(DynamicMethodValueTypeFormatter<>).MakeGenericType(length1);
                var serializeDelegate = serialize.CreateDelegate(serializeType);
                var deserializeDelegate = deserialize.CreateDelegate(deserializeType);
                var answer = ActivatorUtility.CreateInstance(formatterType, serializeDelegate, deserializeDelegate) as IJsonFormatter;
                if (answer is null) throw new NullReferenceException(targetType.FullName);
                return answer;
            }
            else
            {
                var serializeType = typeof(JsonSerializeReferenceTypeAction<>).MakeGenericType(length1);
                var deserializeType = typeof(JsonDeserializeReferenceTypeFunc<>).MakeGenericType(length1);
                var formatterType = typeof(DynamicMethodReferenceTypeFormatter<>).MakeGenericType(length1);
                var serializeDelegate = serialize.CreateDelegate(serializeType);
                var deserializeDelegate = deserialize.CreateDelegate(deserializeType);
                var answer = ActivatorUtility.CreateInstance(formatterType, serializeDelegate, deserializeDelegate) as IJsonFormatter;
                if (answer is null) throw new NullReferenceException(targetType.FullName);
                return answer;
            }
        }
    }

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
            serialize(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return deserialize(ref reader, options);
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            serialize(ref writer, value, options);
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
