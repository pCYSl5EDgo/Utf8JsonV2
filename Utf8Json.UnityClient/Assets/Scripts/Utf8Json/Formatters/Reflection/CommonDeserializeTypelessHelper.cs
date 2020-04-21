// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    internal static class CommonDeserializeTypelessHelper
    {
        public static object DeserializeTypeless(this ref JsonReader reader, JsonSerializerOptions options,
            DeserializationParameterDictionary parameterDictionary,
            DeserializationDictionary deserializationDictionary, in TypeAnalyzeResult data)
        {
            reader.ReadIsBeginObjectWithVerify();
            var empty = Array.Empty<object>();
            object answer;
            if (data.ConstructorData.Parameters.Length == 0)
            {
                answer = data.ConstructorData.Create(empty);
                foreach (var callback in data.OnDeserializing)
                {
                    callback.Invoke(answer, empty);
                }

                var dictionary = data.ExtensionData.GetValue(answer);
                if (dictionary == null)
                {
                    reader.DeserializeTypelessWithBoxedValue(options, answer, deserializationDictionary);
                }
                else
                {
                    reader.DeserializeTypelessWithBoxedValueWithExtensionData(options, answer, dictionary, deserializationDictionary);
                }
            }
            else
            {
#if CSHARP_8_OR_NEWER
                var parameters = new object?[data.ConstructorData.Parameters.Length];
                reader.DeserializeTypelessWithConstructor(options, parameters, data, parameterDictionary);
                answer = data.ConstructorData.Create(parameters!);
#else
                var parameters = new object[data.ConstructorData.Parameters.Length];
                reader.DeserializeTypelessWithConstructor(options, parameters, data, parameterDictionary);
                answer = data.ConstructorData.Create(parameters);
#endif
            }

            foreach (var callback in data.OnDeserialized)
            {
                callback.Invoke(answer, empty);
            }

            return answer;
        }


        private static void DeserializeTypelessWithBoxedValueWithExtensionData(
            this ref JsonReader reader, JsonSerializerOptions options, object boxedValue,
#if CSHARP_8_OR_NEWER
            Dictionary<string, object?> dictionary
#else
            Dictionary<string, object> dictionary
#endif
            , DeserializationDictionary deserializationDictionary
        )
        {
            var count = 0;
            var ignoreCase = options.IgnoreCase;
            var ignoreNull = options.IgnoreNullValues;
            var resolver = options.Resolver;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var propertyName = reader.ReadPropertyNameSegmentRaw();
                DeserializationDictionary.Setter setter;
                if (ignoreCase)
                {
                    if (!deserializationDictionary.TryFindParameterIgnoreCase(propertyName, out setter))
                    {
                        var name = PropertyNameHelper.FromSpanToString(propertyName);
                        var jsonObject = JsonObjectFormatter.DeserializeStatic(ref reader, options);
                        var item = jsonObject.ToObject();
                        dictionary[name] = item;
                        continue;
                    }
                }
                else
                {
                    if (!deserializationDictionary.TryFindParameter(propertyName, out setter))
                    {
                        var name = PropertyNameHelper.FromSpanToString(propertyName);
                        var jsonObject = JsonObjectFormatter.DeserializeStatic(ref reader, options);
                        var item = jsonObject.ToObject();
                        dictionary[name] = item;
                        continue;
                    }
                }

                var formatter = setter.Formatter ?? resolver.GetFormatterWithVerify(setter.TargetType);
                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                var value = formatter.DeserializeTypeless(ref reader, options);
                if (ignoreNull && value == null)
                {
                    continue;
                }

                setter.SetValue(boxedValue, value);
            }
        }

        private static void DeserializeTypelessWithBoxedValue(this ref JsonReader reader, JsonSerializerOptions options, object boxedValue, DeserializationDictionary deserializationDictionary)
        {
            var count = 0;
            var ignoreCase = options.IgnoreCase;
            var ignoreNull = options.IgnoreNullValues;
            var resolver = options.Resolver;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var propertyName = reader.ReadPropertyNameSegmentRaw();
                DeserializationDictionary.Setter setter;
                if (ignoreCase)
                {
                    if (!deserializationDictionary.TryFindParameterIgnoreCase(propertyName, out setter))
                    {
                        reader.ReadNextBlock();
                        continue;
                    }
                }
                else
                {
                    if (!deserializationDictionary.TryFindParameter(propertyName, out setter))
                    {
                        reader.ReadNextBlock();
                        continue;
                    }
                }

                var formatter = setter.Formatter ?? resolver.GetFormatterWithVerify(setter.TargetType);
                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                var value = formatter.DeserializeTypeless(ref reader, options);
                if (ignoreNull && value == null)
                {
                    continue;
                }

                setter.SetValue(boxedValue, value);
            }
        }

        private static void DeserializeTypelessWithConstructor(this ref JsonReader reader, JsonSerializerOptions options,
#if CSHARP_8_OR_NEWER
            Span<object?>
#else
            Span<object>
#endif
                parameters,
            in TypeAnalyzeResult data,
            DeserializationParameterDictionary parameterDictionary)
        {
            data.ConstructorData.Clear(parameters);
            var count = 0;
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
            var resolver = options.Resolver;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var propertyName = reader.ReadPropertyNameSegmentRaw();

                if (!parameterDictionary.TryFindParameterIgnoreCase(propertyName, out var parameterType, out var index))
                {
                    reader.ReadNextBlock();
                    continue;
                }

                if (!ReferenceEquals(targetType, parameterType))
                {
                    targetType = parameterType;
                    formatter = resolver.GetFormatter(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                var deserializedObject = formatter.DeserializeTypeless(ref reader, options);
                parameters[index] = deserializedObject;
            }
        }
    }
}
