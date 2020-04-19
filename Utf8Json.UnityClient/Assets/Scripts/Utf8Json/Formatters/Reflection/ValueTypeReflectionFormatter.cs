// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            SerializeInternal(ref writer, boxedValue, options);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return (T)DeserializeInternal(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var boxedValue = (object)value;
            SerializeInternal(ref writer, boxedValue, options);
        }

        private static void SerializeInternal(ref JsonWriter writer, object boxedValue, JsonSerializerOptions options)
        {
            foreach (var callback in data.OnSerializing)
            {
                callback.Invoke(boxedValue, Array.Empty<object>());
            }

            writer.WriteBeginObject();
            var resolver = options.Resolver;

            var isFirst = true;

            if (data.FieldValueTypeArray.Length != 0)
            {
                SerializeFieldValueTypes(ref writer, options, resolver, boxedValue, true);
                isFirst = false;
            }

            if (data.PropertyValueTypeArray.Length != 0)
            {
                SerializePropertyValueTypes(ref writer, options, resolver, boxedValue, isFirst);
                isFirst = false;
            }

            if (data.FieldValueTypeShouldSerializeArray.Length != 0)
            {
                isFirst = SerializeFieldValueTypeShouldSerializes(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (data.PropertyValueTypeShouldSerializeArray.Length != 0)
            {
                isFirst = SerializePropertyValueTypeShouldSerializes(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (data.FieldReferenceTypeArray.Length != 0)
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

            if (data.PropertyReferenceTypeArray.Length != 0)
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

            if (data.FieldReferenceTypeShouldSerializeArray.Length != 0)
            {
                isFirst = options.IgnoreNullValues
                    ? SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst)
                    : SerializeFieldReferenceTypeShouldSerializesWriteNull(ref writer, options, resolver, boxedValue, isFirst);
            }

            if (data.PropertyReferenceTypeShouldSerializeArray.Length != 0)
            {
                isFirst = options.IgnoreNullValues
                    ? SerializePropertyReferenceTypeShouldSerializesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst)
                    : SerializePropertyReferenceTypeShouldSerializesWriteNull(ref writer, options, resolver, boxedValue, isFirst);
            }

            var dictionary = data.ExtensionData.GetValue(boxedValue);
            if (dictionary != null)
            {
                if (options.IgnoreNullValues)
                {
                    SerializeExtensionDataIgnoreNull(ref writer, options, resolver, dictionary, isFirst);
                }
                else
                {
                    SerializeExtensionDataWriteNull(ref writer, options, resolver, dictionary, isFirst);
                }
            }

            writer.WriteEndObject();

            foreach (var callback in data.OnSerialized)
            {
                callback.Invoke(boxedValue, Array.Empty<object>());
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool SerializeExtensionDataWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver,
#if CSHARP_8_OR_NEWER
            Dictionary<string, object?>
#else
            Dictionary<string, object>
#endif
                dictionary, bool isFirst)
        {
            var enumerator = dictionary.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    return isFirst;
                }

                if (!isFirst)
                {
                    writer.WriteValueSeparator();
                }

                var pair = enumerator.Current;
                writer.WritePropertyName(pair.Key);
                var value = pair.Value;
                var targetType = default(Type);
                var formatter = default(IJsonFormatter);
                if (value == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    targetType = value.GetType();
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }

                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    pair = enumerator.Current;
                    writer.WritePropertyName(pair.Key);
                    value = pair.Value;
                    if (value == null)
                    {
                        writer.WriteNull();
                        continue;
                    }

                    var tmpTargetType = value.GetType();
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
            }
            finally
            {
                enumerator.Dispose();
            }

            return false;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool SerializeExtensionDataIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver,
#if CSHARP_8_OR_NEWER
            Dictionary<string, object?>
#else
            Dictionary<string, object>
#endif
                dictionary, bool isFirst)
        {
            var enumerator = dictionary.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    return isFirst;
                }

                var targetType = default(Type);
                var formatter = default(IJsonFormatter);

                var pair = enumerator.Current;
                var value = pair.Value;
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

                    writer.WritePropertyName(pair.Key);
                    targetType = value.GetType();
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }

                while (enumerator.MoveNext())
                {
                    pair = enumerator.Current;
                    value = pair.Value;
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

                    writer.WritePropertyName(pair.Key);
                    var tmpTargetType = value.GetType();
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
            }
            finally
            {
                enumerator.Dispose();
            }

            return isFirst;
        }

        private static bool SerializePropertyReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.PropertyReferenceTypeShouldSerializeArray[0];
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            if (info.ShouldSerialize(boxedValue))
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                targetType = info.Info.PropertyType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < data.PropertyReferenceTypeShouldSerializeArray.Length; index++)
            {
                info = ref data.PropertyReferenceTypeShouldSerializeArray[index];
                if (!info.ShouldSerialize(boxedValue))
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.Info.PropertyType;
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
            ref readonly var info = ref data.PropertyReferenceTypeShouldSerializeArray[0];
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);

            if (info.ShouldSerialize(boxedValue))
            {
                var value = info.GetValue(boxedValue);
                if (value != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                    }
                    else
                    {
                        writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                    }

                    targetType = info.TargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }
            }

            for (var index = 1; index < data.PropertyReferenceTypeShouldSerializeArray.Length; index++)
            {
                info = ref data.PropertyReferenceTypeShouldSerializeArray[index];
                if (!info.ShouldSerialize(boxedValue))
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
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return false;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.FieldReferenceTypeShouldSerializeArray[0];
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            if (info.ShouldSerialize(boxedValue))
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                targetType = info.TargetType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < data.FieldReferenceTypeShouldSerializeArray.Length; index++)
            {
                info = ref data.FieldReferenceTypeShouldSerializeArray[index];
                if (!info.ShouldSerialize(boxedValue))
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(targetType, tmpTargetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            return isFirst;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.FieldReferenceTypeShouldSerializeArray[0];
            var targetType = default(Type);
            var formatter = default(IJsonFormatter);
            if (info.ShouldSerialize(boxedValue))
            {
                var value = info.GetValue(boxedValue);
                if (value != null)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                    }
                    else
                    {
                        writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                    }

                    targetType = info.TargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }
            }

            for (var index = 1; index < data.FieldReferenceTypeShouldSerializeArray.Length; index++)
            {
                info = ref data.FieldReferenceTypeShouldSerializeArray[index];
                if (!info.ShouldSerialize(boxedValue))
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
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(targetType, tmpTargetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return isFirst;
        }

        private static void SerializePropertyReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.PropertyReferenceTypeArray[0];

            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.PropertyReferenceTypeArray.Length; index++)
            {
                info = ref data.PropertyReferenceTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
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
            ref readonly var info = ref data.PropertyReferenceTypeArray[0];
            var value = info.GetValue(boxedValue);
            if (value != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                targetType = info.TargetType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, value, options);
            }

            for (var index = 1; index < data.PropertyReferenceTypeArray.Length; index++)
            {
                info = ref data.PropertyReferenceTypeArray[index];
                value = info.GetValue(boxedValue);
                if (value == null)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return isFirst;
        }

        private static void SerializeFieldReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.FieldReferenceTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.FieldReferenceTypeArray.Length; index++)
            {
                info = ref data.FieldReferenceTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
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
            ref readonly var info = ref data.FieldReferenceTypeArray[0];
            var value = info.GetValue(boxedValue);
            if (value != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                targetType = info.TargetType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, value, options);
            }

            for (var index = 1; index < data.FieldReferenceTypeArray.Length; index++)
            {
                info = ref data.FieldReferenceTypeArray[index];
                value = info.GetValue(boxedValue);
                if (value == null)
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return isFirst;
        }

        private static bool SerializePropertyValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
            ref readonly var info = ref data.PropertyValueTypeShouldSerializeArray[0];
            if (info.ShouldSerialize(boxedValue))
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                targetType = info.TargetType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < data.PropertyValueTypeShouldSerializeArray.Length; index++)
            {
                info = ref data.PropertyValueTypeShouldSerializeArray[index];
                if (!info.ShouldSerialize(boxedValue))
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            return isFirst;
        }

        private static void SerializePropertyValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.PropertyValueTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.PropertyValueTypeArray.Length; index++)
            {
                info = ref data.PropertyValueTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
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
            ref readonly var info = ref data.FieldValueTypeShouldSerializeArray[0];
            if (info.ShouldSerialize(boxedValue))
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                targetType = info.TargetType;
                formatter = resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            for (var index = 1; index < data.FieldValueTypeShouldSerializeArray.Length; index++)
            {
                info = ref data.FieldValueTypeShouldSerializeArray[index];
                if (!info.ShouldSerialize(boxedValue))
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;
                    writer.WriteRaw(info.GetPropertyNameWithQuotationAndNameSeparator());
                }
                else
                {
                    writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                }

                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            return isFirst;
        }

        private static void SerializeFieldValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst)
        {
            ref readonly var info = ref data.FieldValueTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            IJsonFormatter formatter;
            try
            {
                formatter = resolver.GetFormatterWithVerify(targetType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.FieldValueTypeArray.Length; index++)
            {
                info = ref data.FieldValueTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
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
            return (T)DeserializeInternal(ref reader, options);
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
            return DeserializeInternal(ref reader, options);
        }

        private static object DeserializeInternal(ref JsonReader reader, JsonSerializerOptions options)
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
                    DeserializeTypelessWithBoxedValue(ref reader, options, answer);
                }
                else
                {
                    DeserializeTypelessWithBoxedValueWithExtensionData(ref reader, options, answer, dictionary);
                }
            }
            else
            {
#if CSHARP_8_OR_NEWER
                var parameters = new object?[data.ConstructorData.Parameters.Length];
                DeserializeTypelessWithConstructor(ref reader, options, parameters);
                answer = data.ConstructorData.Create(parameters!);
#else
                var parameters = new object[data.ConstructorData.Parameters.Length];
                DeserializeTypelessWithConstructor(ref reader, options, parameters);
                answer = data.ConstructorData.Create(parameters);
#endif
            }

            foreach (var callback in data.OnDeserialized)
            {
                callback.Invoke(answer, empty);
            }

            return answer;
        }

        private static void DeserializeTypelessWithBoxedValueWithExtensionData(ref JsonReader reader, JsonSerializerOptions options, object boxedValue,
#if CSHARP_8_OR_NEWER
            Dictionary<string, object?> dictionary
#else
            Dictionary<string, object> dictionary
#endif
        )
        {
            var count = 0;
            var ignoreCase = options.IgnoreCase;
            var ignoreNull = options.IgnoreNullValues;
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
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

                if (!ReferenceEquals(targetType, setter.TargetType))
                {
                    targetType = setter.TargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                var value = formatter.DeserializeTypeless(ref reader, options);
                if (ignoreNull && value == null)
                {
                    continue;
                }

                setter.SetValue(boxedValue, value);
            }
        }

        private static void DeserializeTypelessWithBoxedValue(ref JsonReader reader, JsonSerializerOptions options, object boxedValue)
        {
            var count = 0;
            var ignoreCase = options.IgnoreCase;
            var ignoreNull = options.IgnoreNullValues;
            var formatter = default(IJsonFormatter);
            var targetType = default(Type);
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

                if (!ReferenceEquals(targetType, setter.TargetType))
                {
                    targetType = setter.TargetType;
                    formatter = resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                var value = formatter.DeserializeTypeless(ref reader, options);
                if (ignoreNull && value == null)
                {
                    continue;
                }

                setter.SetValue(boxedValue, value);
            }
        }

        private static void DeserializeTypelessWithConstructor(ref JsonReader reader, JsonSerializerOptions options,
#if CSHARP_8_OR_NEWER
            Span<object?>
#else
            Span<object>
#endif
                parameters)
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
