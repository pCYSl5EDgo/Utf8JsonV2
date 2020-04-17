// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Utf8Json.Formatters
{
    internal enum SerializeType : byte
    {
        Ignore,
        SeeShouldSerializeMethod,
        SerializeAlways,
        ExtensionData,
    }

    public sealed class ValueTypeReflectionFormatter<T> : IJsonFormatter<T>
        where T : struct
    {
        private static readonly (FieldInfo, byte[])[] FieldValueTypes;
        private static readonly (FieldInfo, byte[])[] FieldReferenceTypes;
        private static readonly (FieldInfo, MethodInfo, byte[])[] FieldValueTypeShouldSerializes;
        private static readonly (FieldInfo, MethodInfo, byte[])[] FieldReferenceTypeShouldSerializes;

        private static readonly (PropertyInfo, byte[])[] PropertyValueTypes;
        private static readonly (PropertyInfo, byte[])[] PropertyReferenceTypes;
        private static readonly (PropertyInfo, MethodInfo, byte[])[] PropertyValueTypeShouldSerializes;
        private static readonly (PropertyInfo, MethodInfo, byte[])[] PropertyReferenceTypeShouldSerializes;

#if CSHARP_8_OR_NEWER
        private static readonly (PropertyInfo?, byte[]) ExtensionDataProperty;
#else
        private static readonly (PropertyInfo, byte[]) ExtensionDataProperty;
#endif

        static ValueTypeReflectionFormatter()
        {
            var type = typeof(T);
            var fields = type.GetRuntimeFields().ToArray();
            var properties = type.GetRuntimeProperties().ToArray();
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
                            if (ExtensionDataProperty.Item1 != null)
                            {
                                throw new JsonSerializationException();
                            }

                            ExtensionDataProperty.Item1 = info;
                            ExtensionDataProperty.Item2 = ToBytes(encodedName);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                PropertyValueTypeShouldSerializes = new (PropertyInfo, MethodInfo, byte[])[propertyValueTypeShouldSerializeCount];
                PropertyReferenceTypeShouldSerializes = new (PropertyInfo, MethodInfo, byte[])[propertyReferenceTypeShouldSerializeCount];
                PropertyValueTypes = new (PropertyInfo, byte[])[propertyValueTypeCount];
                PropertyReferenceTypes = new (PropertyInfo, byte[])[propertyReferenceTypeCount];
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
                            PropertyValueTypeShouldSerializes[--propertyValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
                            #else
                            PropertyValueTypeShouldSerializes[--propertyValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
                            #endif
                        }
                        else
                        {
                            #if CSHARP_8_OR_NEWER
                            PropertyReferenceTypeShouldSerializes[--propertyReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
                            #else
                            PropertyReferenceTypeShouldSerializes[--propertyReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
                            #endif
                        }
                    }
                    else
                    {
                        if (isValue)
                        {
                            PropertyValueTypes[--propertyValueTypeCount] = (info, encodedBytes);
                        }
                        else
                        {
                            PropertyReferenceTypes[--propertyReferenceTypeCount] = (info, encodedBytes);
                        }
                    }
                }

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
                FieldValueTypes = new (FieldInfo, byte[])[fieldValueTypeCount];
                FieldReferenceTypes = new (FieldInfo, byte[])[fieldReferenceTypeCount];
                FieldValueTypeShouldSerializes = new (FieldInfo, MethodInfo, byte[])[fieldValueTypeShouldSerializeCount];
                FieldReferenceTypeShouldSerializes = new (FieldInfo, MethodInfo, byte[])[fieldReferenceTypeShouldSerializeCount];
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
                            FieldValueTypeShouldSerializes[--fieldValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
                            #else
                            FieldValueTypeShouldSerializes[--fieldValueTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
                            #endif
                        }
                        else
                        {
                            #if CSHARP_8_OR_NEWER
                            FieldReferenceTypeShouldSerializes[--fieldReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes)!;
                            #else
                            FieldReferenceTypeShouldSerializes[--fieldReferenceTypeShouldSerializeCount] = (info, shouldSerialize, encodedBytes);
                            #endif
                        }
                    }
                    else
                    {
                        if (isValue)
                        {
                            FieldValueTypes[--fieldValueTypeCount] = (info, encodedBytes);
                        }
                        else
                        {
                            FieldReferenceTypes[--fieldReferenceTypeCount] = (info, encodedBytes);
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(byteArray);
                ArrayPool<string>.Shared.Return(stringArray);
            }
        }

        private static byte[] ToBytes(string value)
        {
            return Array.Empty<byte>();
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
            
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}