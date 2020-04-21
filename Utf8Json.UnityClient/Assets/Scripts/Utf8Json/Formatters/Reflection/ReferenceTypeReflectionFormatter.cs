// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Utf8Json.Internal;

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
        private static readonly TypeAnalyzeResult data;
        private static readonly DeserializationParameterDictionary parameterDictionary;
        private static readonly DeserializationDictionary deserializationDictionary;

        static ReferenceTypeReflectionFormatter()
        {
            TypeAnalyzer.Analyze(typeof(T), out data);
            parameterDictionary = new DeserializationParameterDictionary(data.ConstructorData.Parameters);
            deserializationDictionary = TypeAnalyzeResultToDeserializationDictionaryConverter.Convert(data);
        }

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
        public static void SerializeStatic(ref JsonWriter writer, T? boxedValue, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T boxedValue, JsonSerializerOptions options)
#endif
        {
            if (boxedValue == null)
            {
                writer.WriteNull();
                return;
            }

            writer.SerializeTypeless(boxedValue, options, data);
        }

#if CSHARP_8_OR_NEWER
        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var answer = reader.DeserializeTypeless(options, parameterDictionary, deserializationDictionary, data) as T;
            return answer;
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
            if (reader.ReadIsNull())
            {
                return null;
            }

            var answer = reader.DeserializeTypeless(options, parameterDictionary, deserializationDictionary, data);
            return answer;
        }
    }
}
