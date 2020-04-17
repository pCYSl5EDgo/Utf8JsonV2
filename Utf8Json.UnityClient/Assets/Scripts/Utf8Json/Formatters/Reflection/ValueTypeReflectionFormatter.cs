// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ValueTypeReflectionFormatter<T> : IJsonFormatter<T>
        where T : struct
    {
        private static readonly (FieldInfo, byte[])[] fieldValueTypes;
        private static readonly (FieldInfo, byte[])[] fieldReferenceTypes;
        private static readonly (FieldInfo, MethodInfo, byte[])[] fieldValueTypeShouldSerializes;
        private static readonly (FieldInfo, MethodInfo, byte[])[] fieldReferenceTypeShouldSerializes;

        private static readonly (PropertyInfo, byte[])[] propertyValueTypes;
        private static readonly (PropertyInfo, byte[])[] propertyReferenceTypes;
        private static readonly (PropertyInfo, MethodInfo, byte[])[] propertyValueTypeShouldSerializes;
        private static readonly (PropertyInfo, MethodInfo, byte[])[] propertyReferenceTypeShouldSerializes;

        private static readonly
#if CSHARP_8_OR_NEWER
            (PropertyInfo?, byte[]?)
#else
            (PropertyInfo, byte[])
#endif
            extensionDataProperty;

        static ValueTypeReflectionFormatter()
        {
            TypeAnalyzer.Analyze(typeof(T),
                out fieldValueTypes,
                out fieldReferenceTypes,
                out fieldValueTypeShouldSerializes,
                out fieldReferenceTypeShouldSerializes,
                out propertyValueTypes,
                out propertyReferenceTypes,
                out propertyValueTypeShouldSerializes,
                out propertyReferenceTypeShouldSerializes,
                out extensionDataProperty);
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteBeginObject();
            var formatterInterface = typeof(IJsonFormatter<>);
            var getSerializeStatic = typeof(IFormatterResolver).GetMethod("GetSerializeStatic");
            Debug.Assert(getSerializeStatic != null);
            var getFormatter = typeof(IFormatProvider).GetMethod("GetFormatter");
            Debug.Assert(getFormatter != null);
            var resolver = options.Resolver;
            foreach (var (fieldInfo, bytes) in fieldValueTypes)
            {
                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var type = fieldInfo.FieldType;
                var serializerObject = getSerializeStatic.MakeGenericMethod(type).Invoke(resolver, null) ?? throw new NullReferenceException();
                var serializer = (IntPtr)serializerObject;
                if (serializer.ToPointer() == null)
                {
                    var formatter = getFormatter.MakeGenericMethod(type).Invoke(resolver, null) as IJsonFormatter;
                    if (formatter == null)
                    {
                        if (type.FullName != null)
                        {
                            throw new JsonSerializationException(type.FullName);
                        }

                        throw new JsonSerializationException();
                    }

                    var serializeMethod = formatterInterface.MakeGenericType(type).GetMethod("Serialize");
                    Debug.Assert(serializeMethod != null);

                }
                else
                {

                }
            }

            writer.WriteEndObject();
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            throw new System.NotImplementedException();
        }


#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is T innerValue))
            {
                throw new ArgumentNullException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
