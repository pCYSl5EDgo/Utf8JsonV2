// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

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
                Type targetType;
                IJsonFormatter formatter;
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

                    targetType = value.GetType();
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
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

                Type targetType;
                IJsonFormatter formatter;

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
                    targetType = value.GetType();
                    formatter = resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static bool SerializePropertyReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.PropertyReferenceTypeShouldSerializeArray[0];
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

                    Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return false;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.FieldReferenceTypeShouldSerializeArray[0];
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

                Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static bool SerializeFieldReferenceTypeShouldSerializesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.FieldReferenceTypeShouldSerializeArray[0];
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

                    Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static void SerializePropertyReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.PropertyReferenceTypeArray[0];

            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
            Serialize(info, ref writer, boxedValue, options, resolver);

            for (var index = 1; index < data.PropertyReferenceTypeArray.Length; index++)
            {
                info = ref data.PropertyReferenceTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                Serialize(info, ref writer, boxedValue, options, resolver);
            }
        }

        private static bool SerializePropertyReferenceTypesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
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

                Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static void SerializeFieldReferenceTypesWriteNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.FieldReferenceTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
            Serialize(info, ref writer, boxedValue, options, resolver);

            for (var index = 1; index < data.FieldReferenceTypeArray.Length; index++)
            {
                info = ref data.FieldReferenceTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                Serialize(info, ref writer, boxedValue, options, resolver);
            }
        }

        private static bool SerializeFieldReferenceTypesIgnoreNull(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
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

                Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static bool SerializePropertyValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
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

                Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static void SerializePropertyValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.PropertyValueTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
            Serialize(info, ref writer, boxedValue, options, resolver);

            for (var index = 1; index < data.PropertyValueTypeArray.Length; index++)
            {
                info = ref data.PropertyValueTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                Serialize(info, ref writer, boxedValue, options, resolver);
            }
        }

        private static bool SerializeFieldValueTypeShouldSerializes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
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

                Serialize(info, ref writer, boxedValue, options, resolver);
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

                Serialize(info, ref writer, boxedValue, options, resolver);
            }

            return isFirst;
        }

        private static void SerializeFieldValueTypes(ref JsonWriter writer, JsonSerializerOptions options, IFormatterResolver resolver, object boxedValue, bool isFirst, in TypeAnalyzeResult data)
        {
            ref readonly var info = ref data.FieldValueTypeArray[0];
            writer.WriteRaw(isFirst ? info.GetPropertyNameWithQuotationAndNameSeparator() : info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
            Serialize(info, ref writer, boxedValue, options, resolver);

            for (var index = 1; index < data.FieldValueTypeArray.Length; index++)
            {
                info = ref data.FieldValueTypeArray[index];
                writer.WriteRaw(info.GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator());
                Serialize(info, ref writer, boxedValue, options, resolver);
            }
        }

        private static void Serialize(in FieldSerializationInfo info, ref JsonWriter writer, object boxedValue, JsonSerializerOptions options, IFormatterResolver resolver)
        {
            var value = info.GetValue(boxedValue);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.None:
                    var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(info.TargetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                    break;
                case DirectTypeEnum.Byte:
                    Debug.Assert(value != null);
                    writer.Write((byte)value);
                    break;
                case DirectTypeEnum.SByte:
                    Debug.Assert(value != null);
                    writer.Write((sbyte)value);
                    break;
                case DirectTypeEnum.UInt16:
                    Debug.Assert(value != null);
                    writer.Write((ushort)value);
                    break;
                case DirectTypeEnum.Int16:
                    Debug.Assert(value != null);
                    writer.Write((short)value);
                    break;
                case DirectTypeEnum.UInt32:
                    Debug.Assert(value != null);
                    writer.Write((uint)value);
                    break;
                case DirectTypeEnum.Int32:
                    Debug.Assert(value != null);
                    writer.Write((int)value);
                    break;
                case DirectTypeEnum.UInt64:
                    Debug.Assert(value != null);
                    writer.Write((ulong)value);
                    break;
                case DirectTypeEnum.Int64:
                    Debug.Assert(value != null);
                    writer.Write((long)value);
                    break;
                case DirectTypeEnum.Boolean:
                    Debug.Assert(value != null);
                    writer.Write((bool)value);
                    break;
                case DirectTypeEnum.Char:
                    Debug.Assert(value != null);
                    writer.Write((char)value);
                    break;
                case DirectTypeEnum.Single:
                    Debug.Assert(value != null);
                    writer.Write((float)value);
                    break;
                case DirectTypeEnum.Double:
                    Debug.Assert(value != null);
                    writer.Write((double)value);
                    break;
                case DirectTypeEnum.String:
                    writer.Write(value as string);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Serialize(in ShouldSerializeFieldSerializationInfo info, ref JsonWriter writer, object boxedValue, JsonSerializerOptions options, IFormatterResolver resolver)
        {
            var value = info.GetValue(boxedValue);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.None:
                    var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(info.TargetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                    break;
                case DirectTypeEnum.Byte:
                    Debug.Assert(value != null);
                    writer.Write((byte)value);
                    break;
                case DirectTypeEnum.SByte:
                    Debug.Assert(value != null);
                    writer.Write((sbyte)value);
                    break;
                case DirectTypeEnum.UInt16:
                    Debug.Assert(value != null);
                    writer.Write((ushort)value);
                    break;
                case DirectTypeEnum.Int16:
                    Debug.Assert(value != null);
                    writer.Write((short)value);
                    break;
                case DirectTypeEnum.UInt32:
                    Debug.Assert(value != null);
                    writer.Write((uint)value);
                    break;
                case DirectTypeEnum.Int32:
                    Debug.Assert(value != null);
                    writer.Write((int)value);
                    break;
                case DirectTypeEnum.UInt64:
                    Debug.Assert(value != null);
                    writer.Write((ulong)value);
                    break;
                case DirectTypeEnum.Int64:
                    Debug.Assert(value != null);
                    writer.Write((long)value);
                    break;
                case DirectTypeEnum.Boolean:
                    Debug.Assert(value != null);
                    writer.Write((bool)value);
                    break;
                case DirectTypeEnum.Char:
                    Debug.Assert(value != null);
                    writer.Write((char)value);
                    break;
                case DirectTypeEnum.Single:
                    Debug.Assert(value != null);
                    writer.Write((float)value);
                    break;
                case DirectTypeEnum.Double:
                    Debug.Assert(value != null);
                    writer.Write((double)value);
                    break;
                case DirectTypeEnum.String:
                    writer.Write(value as string);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Serialize(in ShouldSerializePropertySerializationInfo info, ref JsonWriter writer, object boxedValue, JsonSerializerOptions options, IFormatterResolver resolver)
        {
            var value = info.GetValue(boxedValue);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.None:
                    var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(info.TargetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                    break;
                case DirectTypeEnum.Byte:
                    Debug.Assert(value != null);
                    writer.Write((byte)value);
                    break;
                case DirectTypeEnum.SByte:
                    Debug.Assert(value != null);
                    writer.Write((sbyte)value);
                    break;
                case DirectTypeEnum.UInt16:
                    Debug.Assert(value != null);
                    writer.Write((ushort)value);
                    break;
                case DirectTypeEnum.Int16:
                    Debug.Assert(value != null);
                    writer.Write((short)value);
                    break;
                case DirectTypeEnum.UInt32:
                    Debug.Assert(value != null);
                    writer.Write((uint)value);
                    break;
                case DirectTypeEnum.Int32:
                    Debug.Assert(value != null);
                    writer.Write((int)value);
                    break;
                case DirectTypeEnum.UInt64:
                    Debug.Assert(value != null);
                    writer.Write((ulong)value);
                    break;
                case DirectTypeEnum.Int64:
                    Debug.Assert(value != null);
                    writer.Write((long)value);
                    break;
                case DirectTypeEnum.Boolean:
                    Debug.Assert(value != null);
                    writer.Write((bool)value);
                    break;
                case DirectTypeEnum.Char:
                    Debug.Assert(value != null);
                    writer.Write((char)value);
                    break;
                case DirectTypeEnum.Single:
                    Debug.Assert(value != null);
                    writer.Write((float)value);
                    break;
                case DirectTypeEnum.Double:
                    Debug.Assert(value != null);
                    writer.Write((double)value);
                    break;
                case DirectTypeEnum.String:
                    writer.Write(value as string);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Serialize(in PropertySerializationInfo info, ref JsonWriter writer, object boxedValue, JsonSerializerOptions options, IFormatterResolver resolver)
        {
            var value = info.GetValue(boxedValue);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.None:
                    var formatter = info.Formatter ?? resolver.GetFormatterWithVerify(info.TargetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                    break;
                case DirectTypeEnum.Byte:
                    Debug.Assert(value != null);
                    writer.Write((byte)value);
                    break;
                case DirectTypeEnum.SByte:
                    Debug.Assert(value != null);
                    writer.Write((sbyte)value);
                    break;
                case DirectTypeEnum.UInt16:
                    Debug.Assert(value != null);
                    writer.Write((ushort)value);
                    break;
                case DirectTypeEnum.Int16:
                    Debug.Assert(value != null);
                    writer.Write((short)value);
                    break;
                case DirectTypeEnum.UInt32:
                    Debug.Assert(value != null);
                    writer.Write((uint)value);
                    break;
                case DirectTypeEnum.Int32:
                    Debug.Assert(value != null);
                    writer.Write((int)value);
                    break;
                case DirectTypeEnum.UInt64:
                    Debug.Assert(value != null);
                    writer.Write((ulong)value);
                    break;
                case DirectTypeEnum.Int64:
                    Debug.Assert(value != null);
                    writer.Write((long)value);
                    break;
                case DirectTypeEnum.Boolean:
                    Debug.Assert(value != null);
                    writer.Write((bool)value);
                    break;
                case DirectTypeEnum.Char:
                    Debug.Assert(value != null);
                    writer.Write((char)value);
                    break;
                case DirectTypeEnum.Single:
                    Debug.Assert(value != null);
                    writer.Write((float)value);
                    break;
                case DirectTypeEnum.Double:
                    Debug.Assert(value != null);
                    writer.Write((double)value);
                    break;
                case DirectTypeEnum.String:
                    writer.Write(value as string);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}