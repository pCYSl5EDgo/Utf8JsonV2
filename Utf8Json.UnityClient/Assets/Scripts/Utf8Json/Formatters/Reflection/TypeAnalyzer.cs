// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.InteropServices;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Formatters
{
    internal enum SerializeType : byte
    {
        Ignore,
        SeeShouldSerializeMethod,
        SerializeAlways,
        ExtensionData,
    }

    public static class TypeAnalyzer
    {
        public static void Analyze(Type type,
            out (FieldInfo, byte[])[] fieldValueTypes,
            out (FieldInfo, byte[])[] fieldReferenceTypes,
            out (FieldInfo, MethodInfo, byte[])[] fieldValueTypeShouldSerializes,
            out (FieldInfo, MethodInfo, byte[])[] fieldReferenceTypeShouldSerializes,
            out (PropertyInfo, byte[])[] propertyValueTypes,
            out (PropertyInfo, byte[])[] propertyReferenceTypes,
            out (PropertyInfo, MethodInfo, byte[])[] propertyValueTypeShouldSerializes,
            out (PropertyInfo, MethodInfo, byte[])[] propertyReferenceTypeShouldSerializes,
#if CSHARP_8_OR_NEWER
            out (PropertyInfo?, byte[]?)
#else
            out (PropertyInfo, byte[])
#endif
                extensionDataProperty
        )
        {
            extensionDataProperty = default;
            var fieldInfoArray = ArrayPool<FieldInfo>.Shared.Rent(256);
            var propertyInfoArray = ArrayPool<PropertyInfo>.Shared.Rent(256);
            try
            {
                int fsCount;
                (fsCount, fieldInfoArray) = FillField(type, fieldInfoArray);

                int psCount;
                (psCount, propertyInfoArray) = FillProperty(type, propertyInfoArray);

                var fields = fieldInfoArray.AsSpan(0, fsCount);
                var properties = propertyInfoArray.AsSpan(0, psCount);
                var memberLength = fields.Length + properties.Length;
                var byteArray = ArrayPool<byte>.Shared.Rent(memberLength << 1);
                var stringArray = ArrayPool<string>.Shared.Rent(memberLength);
                try
                {
                    var fieldEncodedNames = stringArray.AsSpan(0, fields.Length);
                    var propertyEncodedNames = stringArray.AsSpan(fields.Length, properties.Length);
                    var fieldSerializeTypes = MemoryMarshal.Cast<byte, SerializeType>(byteArray.AsSpan(0, fields.Length));
                    var propertySerializeTypes = MemoryMarshal.Cast<byte, SerializeType>(byteArray.AsSpan(fields.Length, properties.Length));
                    var fieldIsValues = MemoryMarshal.Cast<byte, bool>(byteArray.AsSpan(memberLength, fields.Length));
                    var propertyIsValues = MemoryMarshal.Cast<byte, bool>(byteArray.AsSpan(memberLength + fields.Length, properties.Length));

                    (propertyValueTypes, propertyReferenceTypes, propertyValueTypeShouldSerializes, propertyReferenceTypeShouldSerializes, extensionDataProperty) = CollectProperties(type, extensionDataProperty, propertySerializeTypes, propertyIsValues, properties, propertyEncodedNames);

                    (fieldValueTypes, fieldReferenceTypes, fieldValueTypeShouldSerializes, fieldReferenceTypeShouldSerializes) = CollectFields(type, fieldSerializeTypes, fields, fieldIsValues, fieldEncodedNames);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(byteArray);
                    ArrayPool<string>.Shared.Return(stringArray);
                }
            }
            finally
            {
                ArrayPool<FieldInfo>.Shared.Return(fieldInfoArray);
                ArrayPool<PropertyInfo>.Shared.Return(propertyInfoArray);
            }
        }

        private static ((FieldInfo, byte[])[] fieldValueTypes, (FieldInfo, byte[])[] fieldReferenceTypes, (FieldInfo, MethodInfo, byte[])[] fieldValueTypeShouldSerializes, (FieldInfo, MethodInfo, byte[])[] fieldReferenceTypeShouldSerializes) CollectFields(Type type, Span<SerializeType> fieldSerializeTypes, Span<FieldInfo> fields, Span<bool> fieldIsValues, Span<string> fieldEncodedNames)
        {
            var fieldValueTypeShouldSerializeCount = 0;
            var fieldReferenceTypeShouldSerializeCount = 0;
            var fieldValueTypeCount = 0;
            var fieldReferenceTypeCount = 0;
            for (var index = 0; index < fieldSerializeTypes.Length; index++)
            {
                var info = fields[index];
                ref var serializeType = ref fieldSerializeTypes[index];
                ref var isValue = ref fieldIsValues[index];
                CollectEachField(info, out serializeType, out fieldEncodedNames[index], out isValue, type);
                switch (serializeType)
                {
                    case SerializeType.Ignore:
                        continue;
                    case SerializeType.SeeShouldSerializeMethod:
                        if (isValue)
                        {
                            fieldValueTypeShouldSerializeCount++;
                        }
                        else
                        {
                            fieldReferenceTypeShouldSerializeCount++;
                        }
                        break;
                    case SerializeType.SerializeAlways:
                        if (isValue)
                        {
                            fieldValueTypeCount++;
                        }
                        else
                        {
                            fieldReferenceTypeCount++;
                        }
                        break;
                    case SerializeType.ExtensionData:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            var fieldValueTypes = new (FieldInfo, byte[])[fieldValueTypeCount];
            var fieldReferenceTypes = new (FieldInfo, byte[])[fieldReferenceTypeCount];
            var fieldValueTypeShouldSerializes = new (FieldInfo, MethodInfo, byte[])[fieldValueTypeShouldSerializeCount];
            var fieldReferenceTypeShouldSerializes = new (FieldInfo, MethodInfo, byte[])[fieldReferenceTypeShouldSerializeCount];
            for (var index = 0; index < fieldSerializeTypes.Length; index++)
            {
                var serializeType = fieldSerializeTypes[index];
                if (serializeType == SerializeType.Ignore)
                {
                    continue;
                }

                var isValue = fieldIsValues[index];
                var encodedName = fieldEncodedNames[index];
                var encodedBytes = ToBytes(encodedName);
                var info = fields[index];
                if (serializeType == SerializeType.SeeShouldSerializeMethod)
                {
                    var shouldSerialize = type.GetMethod("ShouldSerialize" + info.Name, Array.Empty<Type>());
                    if (isValue)
                    {
#if CSHARP_8_OR_NEWER
                        fieldValueTypeShouldSerializes[--fieldValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
#else
                        fieldValueTypeShouldSerializes[--fieldValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
#endif
                    }
                    else
                    {
#if CSHARP_8_OR_NEWER
                        fieldReferenceTypeShouldSerializes[--fieldReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
#else
                        fieldReferenceTypeShouldSerializes[--fieldReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
#endif
                    }
                }
                else
                {
                    if (isValue)
                    {
                        fieldValueTypes[--fieldValueTypeCount] = (info, encodedBytes);
                    }
                    else
                    {
                        fieldReferenceTypes[--fieldReferenceTypeCount] = (info, encodedBytes);
                    }
                }
            }
            return (fieldValueTypes, fieldReferenceTypes, fieldValueTypeShouldSerializes, fieldReferenceTypeShouldSerializes);
        }

        private static
            ((PropertyInfo, byte[])[] propertyValueTypes,
            (PropertyInfo, byte[])[] propertyReferenceTypes,
            (PropertyInfo, MethodInfo, byte[])[] propertyValueTypeShouldSerializes,
            (PropertyInfo, MethodInfo, byte[])[] propertyReferenceTypeShouldSerializes,
#if CSHARP_8_OR_NEWER
            (PropertyInfo?, byte[]?) extensionDataProperty
#else
            (PropertyInfo, byte[]) extensionDataProperty
#endif
            )
            CollectProperties(Type type,
#if CSHARP_8_OR_NEWER
                (PropertyInfo?, byte[]?) extensionDataProperty,
#else
                (PropertyInfo, byte[]) extensionDataProperty, 
#endif
                Span<SerializeType> propertySerializeTypes, Span<bool> propertyIsValues, Span<PropertyInfo> properties, Span<string> propertyEncodedNames)
        {
            var propertyValueTypeShouldSerializeCount = 0;
            var propertyReferenceTypeShouldSerializeCount = 0;
            var propertyValueTypeCount = 0;
            var propertyReferenceTypeCount = 0;
            for (var index = 0; index < propertySerializeTypes.Length; index++)
            {
                ref var serializeType = ref propertySerializeTypes[index];
                ref var isValue = ref propertyIsValues[index];
                var info = properties[index];
                ref var encodedName = ref propertyEncodedNames[index];
                CollectEachProperty(info, out serializeType, out encodedName, out isValue, type);
                switch (serializeType)
                {
                    case SerializeType.Ignore:
                        continue;
                    case SerializeType.SeeShouldSerializeMethod:
                        if (isValue)
                        {
                            propertyValueTypeShouldSerializeCount++;
                        }
                        else
                        {
                            propertyReferenceTypeShouldSerializeCount++;
                        }
                        break;
                    case SerializeType.SerializeAlways:
                        if (isValue)
                        {
                            propertyValueTypeCount++;
                        }
                        else
                        {
                            propertyReferenceTypeCount++;
                        }
                        break;
                    case SerializeType.ExtensionData:
                        if (extensionDataProperty.Item1 != null)
                        {
                            throw new JsonSerializationException();
                        }

                        extensionDataProperty.Item1 = info;
                        extensionDataProperty.Item2 = ToBytes(encodedName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            var propertyValueTypeShouldSerializes = new (PropertyInfo, MethodInfo, byte[])[propertyValueTypeShouldSerializeCount];
            var propertyReferenceTypeShouldSerializes = new (PropertyInfo, MethodInfo, byte[])[propertyReferenceTypeShouldSerializeCount];
            var propertyValueTypes = new (PropertyInfo, byte[])[propertyValueTypeCount];
            var propertyReferenceTypes = new (PropertyInfo, byte[])[propertyReferenceTypeCount];
            for (var index = 0; index < propertySerializeTypes.Length; index++)
            {
                var serializeType = propertySerializeTypes[index];
                if (serializeType == SerializeType.Ignore)
                {
                    continue;
                }

                var isValue = propertyIsValues[index];
                var encodedName = propertyEncodedNames[index];
                var encodedBytes = ToBytes(encodedName);
                var info = properties[index];
                if (serializeType == SerializeType.SeeShouldSerializeMethod)
                {
                    var shouldSerialize = type.GetMethod("ShouldSerialize" + info.Name, Array.Empty<Type>());
                    if (isValue)
                    {
#if CSHARP_8_OR_NEWER
                        propertyValueTypeShouldSerializes[--propertyValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
#else
                        propertyValueTypeShouldSerializes[--propertyValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
#endif
                    }
                    else
                    {
#if CSHARP_8_OR_NEWER
                        propertyReferenceTypeShouldSerializes[--propertyReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
#else
                        propertyReferenceTypeShouldSerializes[--propertyReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
#endif
                    }
                }
                else
                {
                    if (isValue)
                    {
                        propertyValueTypes[--propertyValueTypeCount] = (info, encodedBytes);
                    }
                    else
                    {
                        propertyReferenceTypes[--propertyReferenceTypeCount] = (info, encodedBytes);
                    }
                }
            }
            return (propertyValueTypes, propertyReferenceTypes, propertyValueTypeShouldSerializes, propertyReferenceTypeShouldSerializes, extensionDataProperty);
        }

        private static (int psCount, PropertyInfo[] propertyInfoArray) FillProperty(Type type, PropertyInfo[] propertyInfoArray)
        {
            var psCount = 0;
            var pEnumerator = type.GetRuntimeProperties().GetEnumerator();
            try
            {
                while (pEnumerator.MoveNext())
                {
                    if (propertyInfoArray.Length < ++psCount)
                    {
                        var tmp = ArrayPool<PropertyInfo>.Shared.Rent(psCount << 1);
                        Array.Copy(propertyInfoArray, tmp, propertyInfoArray.Length);
                        ArrayPool<PropertyInfo>.Shared.Return(propertyInfoArray);
                        propertyInfoArray = tmp;
                    }

                    propertyInfoArray[psCount - 1] = pEnumerator.Current;
                }
            }
            finally
            {
                pEnumerator.Dispose();
            }
            return (psCount, propertyInfoArray);
        }

        private static (int fsCount, FieldInfo[] fieldInfoArray) FillField(Type type, FieldInfo[] fieldInfoArray)
        {
            var fsCount = 0;
            var fEnumerator = type.GetRuntimeFields().GetEnumerator();
            try
            {
                while (fEnumerator.MoveNext())
                {
                    if (fieldInfoArray.Length < ++fsCount)
                    {
                        var tmp = ArrayPool<FieldInfo>.Shared.Rent(fsCount << 1);
                        Array.Copy(fieldInfoArray, tmp, fieldInfoArray.Length);
                        ArrayPool<FieldInfo>.Shared.Return(fieldInfoArray);
                        fieldInfoArray = tmp;
                    }

                    fieldInfoArray[fsCount - 1] = fEnumerator.Current;
                }
            }
            finally
            {
                fEnumerator.Dispose();
            }
            return (fsCount, fieldInfoArray);
        }

        private static byte[] ToBytes(string value)
        {
            return JsonSerializer.Serialize(value);
        }

        private static void CollectEachField(FieldInfo info, out SerializeType serializeType, out string encodedName, out bool isValue, Type type)
        {
            isValue = info.FieldType.IsValueType;
            serializeType = SerializeType.SerializeAlways;
            encodedName = info.Name;

            var attributes = info.GetCustomAttributes();
            var hasSerializeFieldAttribute = false;

            foreach (var attribute in attributes)
            {
                var attributeType = attribute.GetType();
                var name = attributeType.FullName;
                switch (name)
                {
                    case "System.Runtime.Serialization.IgnoreDataMember":
                        serializeType = SerializeType.Ignore;
                        return;
                    case "System.Runtime.Serialization.DataMember":
                        encodedName = attributeType.GetProperty("Name")?.GetValue(attribute) as string ?? throw new NullReferenceException();
                        break;
                    case "UnityEngine.SerializeField":
                        hasSerializeFieldAttribute = true;
                        break;
                }
            }

            if (!hasSerializeFieldAttribute && !info.IsPublic)
            {
                serializeType = SerializeType.Ignore;
                return;
            }

            var shouldSerialize = type.GetMethod("ShouldSerialize" + info.Name);
            if (shouldSerialize != null && shouldSerialize.ReturnType == typeof(bool) && !shouldSerialize.IsStatic && shouldSerialize.GetParameters().Length == 0)
            {
                serializeType = SerializeType.SeeShouldSerializeMethod;
            }
        }

        private static void CollectEachProperty(PropertyInfo info, out SerializeType serializeType, out string encodedName, out bool isValue, Type type)
        {
            isValue = info.PropertyType.IsValueType;
            if (info.CanRead)
            {
                serializeType = SerializeType.Ignore;
                encodedName = string.Empty;
                return;
            }

            serializeType = SerializeType.SerializeAlways;
            encodedName = info.Name;

            var attributes = info.GetCustomAttributes();
            var mustReturn = false;
            foreach (var attribute in attributes)
            {
                var attributeType = attribute.GetType();
                var name = attributeType.FullName;
                switch (name)
                {
                    case "System.Runtime.Serialization.IgnoreDataMember":
                    case "System.Text.Json.Serialization.JsonIgnoreAttribute":
                        serializeType = SerializeType.Ignore;
                        return;
                    case "System.Text.Json.Serialization.JsonExtensionDataAttribute":
                        serializeType = SerializeType.ExtensionData;
                        mustReturn = true;
                        break;
                    case "System.Runtime.Serialization.DataMember":
                    case "System.Text.Json.Serialization.JsonPropertyNameAttribute":
                        encodedName = attributeType.GetProperty("Name")?.GetValue(attribute) as string ?? throw new NullReferenceException();
                        break;
                }
            }

            if (mustReturn)
            {
                return;
            }

            var shouldSerialize = type.GetMethod("ShouldSerialize" + info.Name);
            if (shouldSerialize != null && shouldSerialize.ReturnType == typeof(bool) && !shouldSerialize.IsStatic && shouldSerialize.GetParameters().Length == 0)
            {
                serializeType = SerializeType.SeeShouldSerializeMethod;
            }
        }
    }
}
