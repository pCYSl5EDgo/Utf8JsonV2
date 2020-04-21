// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    internal static class CommonSerializeTypelessHelper
    {
        public static void SerializeTypeless(this ref JsonWriter writer, object boxedValue, JsonSerializerOptions options, in TypeAnalyzeResult data)
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
                SerializeFieldValueTypes(ref writer, options, resolver, boxedValue, true, in data);
                isFirst = false;
            }

            if (data.PropertyValueTypeArray.Length != 0)
            {
                SerializePropertyValueTypes(ref writer, options, resolver, boxedValue, isFirst, in data);
                isFirst = false;
            }

            if (data.FieldValueTypeShouldSerializeArray.Length != 0)
            {
                isFirst = SerializeFieldValueTypeShouldSerializes(ref writer, options, resolver, boxedValue, isFirst, in data);
            }

            if (data.PropertyValueTypeShouldSerializeArray.Length != 0)
            {
                isFirst = SerializePropertyValueTypeShouldSerializes(ref writer, options, resolver, boxedValue, isFirst, in data);
            }

            if (data.FieldReferenceTypeArray.Length != 0)
            {
                if (options.IgnoreNullValues)
                {
                    isFirst = SerializeFieldReferenceTypesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst, in data);
                }
                else
                {
                    SerializeFieldReferenceTypesWriteNull(ref writer, options, resolver, boxedValue, isFirst, in data);
                    isFirst = false;
                }
            }

            if (data.PropertyReferenceTypeArray.Length != 0)
            {
                if (options.IgnoreNullValues)
                {
                    isFirst = SerializePropertyReferenceTypesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst, in data);
                }
                else
                {
                    SerializePropertyReferenceTypesWriteNull(ref writer, options, resolver, boxedValue, isFirst, in data);
                    isFirst = false;
                }
            }

            if (data.FieldReferenceTypeShouldSerializeArray.Length != 0)
            {
                isFirst = options.IgnoreNullValues
                    ? SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst, in data)
                    : SerializeFieldReferenceTypeShouldSerializesWriteNull(ref writer, options, resolver, boxedValue, isFirst, in data);
            }

            if (data.PropertyReferenceTypeShouldSerializeArray.Length != 0)
            {
                isFirst = options.IgnoreNullValues
                    ? SerializePropertyReferenceTypeShouldSerializesIgnoreNull(ref writer, options, resolver, boxedValue, isFirst, in data)
                    : SerializePropertyReferenceTypeShouldSerializesWriteNull(ref writer, options, resolver, boxedValue, isFirst, in data);
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

        private static bool SerializePropertyReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

#if CSHARP_8_OR_NEWER
                formatter!.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#else
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
#endif
            }

            return isFirst;
        }

        private static bool SerializePropertyReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return false;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            return isFirst;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return isFirst;
        }

        private static void SerializePropertyReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.PropertyReferenceTypeArray[0];

            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.PropertyReferenceTypeArray.Length; index++)
            {
                info = ref data.PropertyReferenceTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

        private static bool SerializePropertyReferenceTypesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return isFirst;
        }

        private static void SerializeFieldReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.FieldReferenceTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.FieldReferenceTypeArray.Length; index++)
            {
                info = ref data.FieldReferenceTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

        private static bool SerializeFieldReferenceTypesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, value, options);
            }

            return isFirst;
        }

        private static bool SerializePropertyValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            return isFirst;
        }

        private static void SerializePropertyValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.PropertyValueTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.PropertyValueTypeArray.Length; index++)
            {
                info = ref data.PropertyValueTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

        private static bool SerializeFieldValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
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
                formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
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
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                Debug.Assert(formatter != null, nameof(formatter) + " != null");
                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }

            return isFirst;
        }

        private static void SerializeFieldValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.FieldValueTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());

            var targetType = info.TargetType;
            var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
            formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);

            for (var index = 1; index < data.FieldValueTypeArray.Length; index++)
            {
                info = ref data.FieldValueTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                var tmpTargetType = info.TargetType;
                if (!ReferenceEquals(tmpTargetType, targetType))
                {
                    targetType = tmpTargetType;
                    formatter = info.Formatter ?? resolver.GetFormatterWithVerify(targetType);
                }

                formatter.SerializeTypeless(ref writer, info.GetValue(boxedValue), options);
            }
        }

    }
}