// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class ValueTypeReflectionFormatter<T> : IJsonFormatter<T>
        where T : struct
    {
        private static readonly TypeAnalyzeResult data;
        private static readonly DeserializationParameterDictionary parameterDictionary;
        private static readonly DeserializationDictionary deserializationDictionary;

        static ValueTypeReflectionFormatter()
        {
            TypeAnalyzer.Analyze(typeof(T), out data);
            parameterDictionary = new DeserializationParameterDictionary(data.ConstructorData.Parameters);
            deserializationDictionary = TypeAnalyzeResultToDeserializationDictionaryConverter.Convert(data);
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var boxedValue = (object)value;
            writer.SerializeTypeless(boxedValue, options, data);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return (T)reader.DeserializeInternal(options, parameterDictionary, deserializationDictionary, data);
        }

        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var boxedValue = (object)value;
            writer.SerializeTypeless(boxedValue, options, data);
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return (T)reader.DeserializeInternal(options, parameterDictionary, deserializationDictionary, data);
        }


#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            writer.SerializeTypeless(value, options, in data);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.DeserializeInternal(options, parameterDictionary, deserializationDictionary, data);
        }
    }
}
