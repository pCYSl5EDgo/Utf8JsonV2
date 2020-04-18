// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class ValueTypeReflectionFormatter<T> : IJsonFormatter<T>
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
            (PropertyInfo?, byte[], ExtensionDataKind)
#else
            (PropertyInfo, byte[], ExtensionDataKind)
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
            var boxedValue = (object)value;
            SerializeInternal(ref writer, boxedValue, options);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var boxedValue = (object)value;
            SerializeInternal(ref writer, boxedValue, options);
        }

        private static void SerializeInternal(ref JsonWriter writer, object boxedValue, JsonSerializerOptions options)
        {
            writer.WriteBeginObject();
            var resolver = options.Resolver;

            var isFirst = true;

            if (fieldValueTypes.Length != 0)
            {
                SerializeFieldValueTypes(ref writer, options, resolver, boxedValue, true);
                isFirst = false;
            }

            if (propertyValueTypes.Length != 0)
            {
                SerializePropertyValueTypes(ref writer, options, resolver, boxedValue, isFirst);
                isFirst = false;
            }

            if (fieldValueTypeShouldSerializes.Length != 0)
            {
                isFirst = SerializeFieldValueTypeShouldSerializes(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (propertyValueTypeShouldSerializes.Length != 0)
            {
                isFirst = SerializePropertyValueTypeShouldSerializes(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (fieldReferenceTypes.Length != 0)
            {
                if (options.IgnoreNullValues)
                {
                    isFirst = SerializeFieldReferenceTypesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst);
                }
                else
                {
                    SerializeFieldReferenceTypesWriteNull(ref writer, options, resolver, boxedValue, isFirst);
                    isFirst = false;
                }
            }

            if (propertyReferenceTypes.Length != 0)
            {
                if (options.IgnoreNullValues)
                {
                    isFirst = SerializePropertyReferenceTypesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst);
                }
                else
                {
                    SerializePropertyReferenceTypesWriteNull(ref writer, options, resolver, boxedValue, isFirst);
                    isFirst = false;
                }
            }

            if (fieldReferenceTypeShouldSerializes.Length != 0)
            {
                isFirst = options.IgnoreNullValues
                    ? SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst)
                    : SerializeFieldReferenceTypeShouldSerializesWriteNull(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (propertyReferenceTypeShouldSerializes.Length != 0)
            {
                isFirst = options.IgnoreNullValues
                    ? SerializePropertyReferenceTypeShouldSerializesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst)
                    : SerializePropertyReferenceTypeShouldSerializesWriteNull(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (extensionDataProperty.Item1 != null)
            {
                if (!isFirst)
                {
                    writer.WriteValueSeparator();
                }
                writer.WriteRaw(extensionDataProperty.Item2);
                writer.WriteNameSeparator();
#if CSHARP_8_OR_NEWER
                var formatter = resolver.GetFormatterWithVerify<Dictionary<string, object>?>();
#else
                var formatter = resolver.GetFormatterWithVerify<Dictionary<string, object>>();
#endif
                var extensionDataValue = extensionDataProperty.Item1.GetValue(boxedValue);
                formatter.Serialize(ref writer, extensionDataValue as Dictionary<string, object>, options);
            }

            writer.WriteEndObject();
        }

        private static bool SerializePropertyReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, methodInfo, bytes) = propertyReferenceTypeShouldSerializes[0];
            var shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
            if (shouldSerializeObject == null)
            {
                throw new NullReferenceException();
            }

            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            if ((bool)shouldSerializeObject)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                targetType = info.PropertyType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < propertyReferenceTypeShouldSerializes.Length; index++)
            {
                (info, methodInfo, bytes) = propertyReferenceTypeShouldSerializes[index];
                shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
                if (shouldSerializeObject == null)
                {
                    throw new NullReferenceException();
                }

                if (!(bool)shouldSerializeObject)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.PropertyType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#else
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#endif
            }

            return isFirst;
        }

        private static bool SerializePropertyReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, methodInfo, bytes) = propertyReferenceTypeShouldSerializes[0];
            var shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            if (shouldSerializeObject == null)
            {
                throw new NullReferenceException();
            }

            if ((bool)shouldSerializeObject)
            {
                var value = info.GetValue(boxedValue);
                if (value != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        writer.WriteValueSeparator();
                    }

                    writer.WriteRaw(bytes);
                    writer.WriteNameSeparator();
                    targetType = info.PropertyType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }
            }

            for (var index = 1; index < propertyReferenceTypeShouldSerializes.Length; index++)
            {
                (info, methodInfo, bytes) = propertyReferenceTypeShouldSerializes[index];
                shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
                if (shouldSerializeObject == null)
                {
                    throw new NullReferenceException();
                }

                if (!(bool)shouldSerializeObject)
                {
                    continue;
                }

                var value = info.GetValue(boxedValue);
                if (value == null)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.PropertyType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, value, options);
#else
                formatter.SerializeTypeless(ref writer, value, options);
#endif
            }

            return false;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, methodInfo, bytes) = fieldReferenceTypeShouldSerializes[0];
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            var shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
            if (shouldSerializeObject == null)
            {
                throw new NullReferenceException();
            }

            if ((bool)shouldSerializeObject)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                targetType = info.FieldType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < fieldReferenceTypeShouldSerializes.Length; index++)
            {
                (info, methodInfo, bytes) = fieldReferenceTypeShouldSerializes[index];
                shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
                if (shouldSerializeObject == null)
                {
                    throw new NullReferenceException();
                }

                if (!(bool)shouldSerializeObject)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.FieldType;
                if (!ReferenceEquals(targetType, tmpTargetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#else
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#endif
            }

            return isFirst;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, methodInfo, bytes) = fieldReferenceTypeShouldSerializes[0];
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            var shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
            if (shouldSerializeObject == null)
            {
                throw new NullReferenceException();
            }

            if ((bool)shouldSerializeObject)
            {
                var value = info.GetValue(boxedValue);
                if (value != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        writer.WriteValueSeparator();
                    }

                    writer.WriteRaw(bytes);
                    writer.WriteNameSeparator();
                    targetType = info.FieldType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }
            }

            for (var index = 1; index < fieldReferenceTypeShouldSerializes.Length; index++)
            {
                (info, methodInfo, bytes) = fieldReferenceTypeShouldSerializes[index];
                shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
                if (shouldSerializeObject == null)
                {
                    throw new NullReferenceException();
                }

                if (!(bool)shouldSerializeObject)
                {
                    continue;
                }

                var value = info.GetValue(boxedValue);
                if (value == null)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.FieldType;
                if (!ReferenceEquals(targetType, tmpTargetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, value, options);
#else
                formatter.SerializeTypeless(ref writer, value, options);
#endif
            }

            return isFirst;
        }

        private static void SerializePropertyReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, bytes) = propertyReferenceTypes[0];
            if (!isFirst)
            {
                writer.WriteValueSeparator();
            }

            writer.WriteRaw(bytes);
            writer.WriteNameSeparator();
            var targetType = info.PropertyType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < propertyReferenceTypes.Length; index++)
            {
                (info, bytes) = propertyReferenceTypes[index];
                writer.WriteValueSeparator();
                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.PropertyType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

        private static bool SerializePropertyReferenceTypesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
            var (info, bytes) = propertyReferenceTypes[0];
            var value = info.GetValue(boxedValue);
            if (value != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                targetType = info.PropertyType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, value, options);
            }

            for (var index = 1; index < propertyReferenceTypes.Length; index++)
            {
                (info, bytes) = propertyReferenceTypes[index];
                value = info.GetValue(boxedValue);
                if (value == null)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.PropertyType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, value, options);
#else
                formatter.SerializeTypeless(ref writer, value, options);
#endif
            }

            return isFirst;
        }

        private static void SerializeFieldReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, bytes) = fieldReferenceTypes[0];
            if (!isFirst)
            {
                writer.WriteValueSeparator();
            }

            writer.WriteRaw(bytes);
            writer.WriteNameSeparator();
            var targetType = info.FieldType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < fieldReferenceTypes.Length; index++)
            {
                (info, bytes) = fieldReferenceTypes[index];
                writer.WriteValueSeparator();
                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.FieldType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

        private static bool SerializeFieldReferenceTypesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
            var (info, bytes) = fieldReferenceTypes[0];
            var value = info.GetValue(boxedValue);
            if (value != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                targetType = info.FieldType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, value, options);
            }

            for (var index = 1; index < fieldReferenceTypes.Length; index++)
            {
                (info, bytes) = fieldReferenceTypes[index];
                value = info.GetValue(boxedValue);
                if (value == null)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.FieldType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, value, options);
#else
                formatter.SerializeTypeless(ref writer, value, options);
#endif
            }

            return isFirst;
        }

        private static bool SerializePropertyValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
            var (info, methodInfo, bytes) = propertyValueTypeShouldSerializes[0];
            var shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
            if (shouldSerializeObject == null)
            {
                throw new NullReferenceException();
            }

            if ((bool)shouldSerializeObject)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                targetType = info.PropertyType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < propertyValueTypeShouldSerializes.Length; index++)
            {
                (info, methodInfo, bytes) = propertyValueTypeShouldSerializes[index];
                shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
                if (shouldSerializeObject == null)
                {
                    throw new NullReferenceException();
                }

                if (!(bool)shouldSerializeObject)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.PropertyType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#else
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#endif
            }

            return isFirst;
        }

        private static void SerializePropertyValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, bytes) = propertyValueTypes[0];
            if (!isFirst)
            {
                writer.WriteValueSeparator();
            }

            writer.WriteRaw(bytes);
            writer.WriteNameSeparator();
            var targetType = info.PropertyType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < propertyValueTypes.Length; index++)
            {
                (info, bytes) = propertyValueTypes[index];
                writer.WriteValueSeparator();
                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.PropertyType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

        private static bool SerializeFieldValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
            var (info, methodInfo, bytes) = fieldValueTypeShouldSerializes[0];
            var shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
            if (shouldSerializeObject == null)
            {
                throw new NullReferenceException();
            }

            if ((bool)shouldSerializeObject)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                targetType = info.FieldType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < fieldValueTypeShouldSerializes.Length; index++)
            {
                (info, methodInfo, bytes) = fieldValueTypeShouldSerializes[index];
                shouldSerializeObject = methodInfo.Invoke(boxedValue, Array.Empty<object>());
                if (shouldSerializeObject == null)
                {
                    throw new NullReferenceException();
                }

                if (!(bool)shouldSerializeObject)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.FieldType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#else
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#endif
            }

            return isFirst;
        }

        private static void SerializeFieldValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var (info, bytes) = fieldValueTypes[0];
            if (!isFirst)
            {
                writer.WriteValueSeparator();
            }

            writer.WriteRaw(bytes);
            writer.WriteNameSeparator();
            var targetType = info.FieldType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < fieldValueTypes.Length; index++)
            {
                (info, bytes) = fieldValueTypes[index];
                writer.WriteValueSeparator();
                writer.WriteRaw(bytes);
                writer.WriteNameSeparator();
                var tmpTargetType = info.FieldType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
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
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            SerializeInternal(ref writer, value, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
