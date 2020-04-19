// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
// ReSharper disable RedundantCaseLabel
// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Internal
{
    public static class TypeAnalyzer
    {
        public static void Analyze(Type type,
            out TypeAnalyzeResult result
        )
        {
            CollectFieldAndProperty(type, out var fieldValueTypes, out var fieldReferenceTypes, out var fieldValueTypeShouldSerializes, out var fieldReferenceTypeShouldSerializes, out var propertyValueTypes, out var propertyReferenceTypes, out var propertyValueTypeShouldSerializes, out var propertyReferenceTypeShouldSerializes, out var extensionDataProperty);

            CollectCallbacks(type, out var onSerializing, out var onSerialized, out var onDeserializing, out var onDeserialized);

            CollectConstructorOrFactory(type, out var constructorData);

            result = new TypeAnalyzeResult(
                fieldValueTypes, fieldReferenceTypes,
                propertyValueTypes, propertyReferenceTypes,
                fieldValueTypeShouldSerializes, fieldReferenceTypeShouldSerializes,
                propertyValueTypeShouldSerializes, propertyReferenceTypeShouldSerializes,
                onSerializing, onSerialized, onDeserializing, onDeserialized,
                extensionDataProperty, constructorData);
        }

        private static void CollectConstructorOrFactory(Type type, out ConstructorDataInfo constructorDataInfo)
        {
            constructorDataInfo = new ConstructorDataInfo(type);
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var customAttributes = constructor.CustomAttributes;
                foreach (var attribute in customAttributes)
                {
                    if (
                        attribute.AttributeType.FullName != "Newtonsoft.Json.JsonConstructorAttribute"
                        && attribute.AttributeType.FullName != "Utf8Json.SerializationConstructorAttribute"
                    )
                    {
                        continue;
                    }

                    constructorDataInfo = new ConstructorDataInfo(type, constructor);
                    break;
                }
            }

            if (constructorDataInfo.Constructor != null)
            {
                return;
            }

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (!method.IsStatic || method.ReturnType != type)
                {
                    continue;
                }

                var customAttributes = method.CustomAttributes;
                foreach (var attribute in customAttributes)
                {
                    if (
                        attribute.AttributeType.FullName != "Utf8Json.SerializationConstructorAttribute"
                    )
                    {
                        continue;
                    }

                    constructorDataInfo = new ConstructorDataInfo(type, method);
                    break;
                }
            }
        }

        private static void Add<T>(ref T[] array, T value)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
        }

        private static void AddIfNotContained<T>(ref T[] array, T value)
        {
            foreach (var equatable in array)
            {
                if (ReferenceEquals(equatable, value))
                {
                    return;
                }
            }

            Add(ref array, value);
        }

        private static void CollectCallbacks(Type type,
            out MethodInfo[] onSerializing,
            out MethodInfo[] onSerialized,
            out MethodInfo[] onDeserializing,
            out MethodInfo[] onDeserialized
            )
        {
            onDeserializing = Array.Empty<MethodInfo>();
            onDeserialized = Array.Empty<MethodInfo>();
            onSerializing = Array.Empty<MethodInfo>();
            onSerialized = Array.Empty<MethodInfo>();
            if (typeof(IAfterDeserializationCallback).IsAssignableFrom(type))
            {
                var method = type.GetMethod(nameof(IAfterDeserializationCallback.OnDeserialized), Array.Empty<Type>());
                if (method != null)
                {
                    Add(ref onDeserialized, method);
                }
            }

            if (typeof(IAfterSerializationCallback).IsAssignableFrom(type))
            {
                var method = type.GetMethod(nameof(IAfterSerializationCallback.OnSerialized), Array.Empty<Type>());
                if (method != null)
                {
                    Add(ref onSerialized, method);
                }
            }

            if (typeof(IBeforeDeserializationCallback).IsAssignableFrom(type))
            {
                var method = type.GetMethod(nameof(IBeforeDeserializationCallback.OnDeserializing), Array.Empty<Type>());
                if (method != null)
                {
                    Add(ref onDeserializing, method);
                }
            }

            if (typeof(IBeforeSerializationCallback).IsAssignableFrom(type))
            {
                var method = type.GetMethod(nameof(IBeforeSerializationCallback.OnSerializing), Array.Empty<Type>());
                if (method != null)
                {
                    Add(ref onSerializing, method);
                }
            }

            var methods = type.GetMethods();
            foreach (var methodInfo in methods)
            {
                if (methodInfo.IsStatic)
                {
                    continue;
                }

                var attributes = Attribute.GetCustomAttributes(methodInfo);
                if (attributes.Length == 0)
                {
                    continue;
                }

                foreach (var attribute in attributes)
                {
                    switch (attribute.GetType().FullName)
                    {
                        case "System.Runtime.Serialization.OnSerializingAttribute":
                            AddIfNotContained(ref onSerializing, methodInfo);
                            break;
                        case "System.Runtime.Serialization.OnSerializedAttribute":
                            AddIfNotContained(ref onSerialized, methodInfo);
                            break;
                        case "System.Runtime.Serialization.OnDeserializingAttribute":
                            AddIfNotContained(ref onDeserializing, methodInfo);
                            break;
                        case "System.Runtime.Serialization.OnDeserializedAttribute":
                            AddIfNotContained(ref onDeserialized, methodInfo);
                            break;
                    }
                }
            }
        }

        private static void CollectFieldAndProperty(Type type,
            out FieldSerializationInfo[] fieldValueTypes,
            out FieldSerializationInfo[] fieldReferenceTypes,
            out ShouldSerializeFieldSerializationInfo[] fieldValueTypeShouldSerializes,
            out ShouldSerializeFieldSerializationInfo[] fieldReferenceTypeShouldSerializes,
            out PropertySerializationInfo[] propertyValueTypes,
            out PropertySerializationInfo[] propertyReferenceTypes,
            out ShouldSerializePropertySerializationInfo[] propertyValueTypeShouldSerializes,
            out ShouldSerializePropertySerializationInfo[] propertyReferenceTypeShouldSerializes,
            out ExtensionDataInfo extensionDataProperty
        )
        {
#if CSHARP_8_OR_NEWER
            extensionDataProperty = new ExtensionDataInfo(null!);
#else
            extensionDataProperty = new ExtensionDataInfo(null);
#endif
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

                    (propertyValueTypes, propertyReferenceTypes, propertyValueTypeShouldSerializes, propertyReferenceTypeShouldSerializes) =
                        CollectProperties(type, ref extensionDataProperty, propertySerializeTypes, propertyIsValues, properties, propertyEncodedNames);

                    (fieldValueTypes, fieldReferenceTypes, fieldValueTypeShouldSerializes, fieldReferenceTypeShouldSerializes) = 
                        CollectFields(type, fieldSerializeTypes, fields, fieldIsValues, fieldEncodedNames);
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

        private static (
            FieldSerializationInfo[] fieldValueTypes,
            FieldSerializationInfo[] fieldReferenceTypes,
            ShouldSerializeFieldSerializationInfo[] fieldValueTypeShouldSerializes,
            ShouldSerializeFieldSerializationInfo[] fieldReferenceTypeShouldSerializes)
            CollectFields(Type type, Span<SerializeType> fieldSerializeTypes, Span<FieldInfo> fields, Span<bool> fieldIsValues, Span<string> fieldEncodedNames)
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
            var fieldValueTypes = new FieldSerializationInfo[fieldValueTypeCount];
            var fieldReferenceTypes = new FieldSerializationInfo[fieldReferenceTypeCount];
            var fieldValueTypeShouldSerializes = new ShouldSerializeFieldSerializationInfo[fieldValueTypeShouldSerializeCount];
            var fieldReferenceTypeShouldSerializes = new ShouldSerializeFieldSerializationInfo[fieldReferenceTypeShouldSerializeCount];
            for (var index = 0; index < fieldSerializeTypes.Length; index++)
            {
                var serializeType = fieldSerializeTypes[index];
                if (serializeType == SerializeType.Ignore)
                {
                    continue;
                }

                var isValue = fieldIsValues[index];
                var encodedName = fieldEncodedNames[index];
                var info = fields[index];
                if (serializeType == SerializeType.SeeShouldSerializeMethod)
                {
                    var shouldSerialize = type.GetMethod("ShouldSerialize" + info.Name, Array.Empty<Type>());
                    Debug.Assert(shouldSerialize != null);
                    if (isValue)
                    {
                        fieldValueTypeShouldSerializes[--fieldValueTypeShouldSerializeCount] = new ShouldSerializeFieldSerializationInfo(info, shouldSerialize, encodedName);
                    }
                    else
                    {
                        fieldReferenceTypeShouldSerializes[--fieldReferenceTypeShouldSerializeCount] = new ShouldSerializeFieldSerializationInfo(info, shouldSerialize, encodedName);
                    }
                }
                else
                {
                    if (isValue)
                    {
                        fieldValueTypes[--fieldValueTypeCount] = new FieldSerializationInfo(info, encodedName);
                    }
                    else
                    {
                        fieldReferenceTypes[--fieldReferenceTypeCount] = new FieldSerializationInfo(info, encodedName);
                    }
                }
            }

            return (fieldValueTypes, fieldReferenceTypes, fieldValueTypeShouldSerializes, fieldReferenceTypeShouldSerializes);
        }

        private static
            (PropertySerializationInfo[] propertyValueTypes,
            PropertySerializationInfo[] propertyReferenceTypes,
            ShouldSerializePropertySerializationInfo[] propertyValueTypeShouldSerializes,
            ShouldSerializePropertySerializationInfo[] propertyReferenceTypeShouldSerializes)
            CollectProperties(Type type,
                ref ExtensionDataInfo extensionDataProperty,
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
                        if (extensionDataProperty.Info != null)
                        {
                            throw new JsonSerializationException();
                        }

                        extensionDataProperty = new ExtensionDataInfo(info);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var propertyValueTypeShouldSerializes = new ShouldSerializePropertySerializationInfo[propertyValueTypeShouldSerializeCount];
            var propertyReferenceTypeShouldSerializes = new ShouldSerializePropertySerializationInfo[propertyReferenceTypeShouldSerializeCount];
            var propertyValueTypes = new PropertySerializationInfo[propertyValueTypeCount];
            var propertyReferenceTypes = new PropertySerializationInfo[propertyReferenceTypeCount];
            for (var index = 0; index < propertySerializeTypes.Length; index++)
            {
                var serializeType = propertySerializeTypes[index];
                if (serializeType == SerializeType.Ignore)
                {
                    continue;
                }

                var isValue = propertyIsValues[index];
                var encodedName = propertyEncodedNames[index];
                var info = properties[index];
                if (serializeType == SerializeType.SeeShouldSerializeMethod)
                {
                    var shouldSerialize = type.GetMethod("ShouldSerialize" + info.Name, Array.Empty<Type>());
                    Debug.Assert(shouldSerialize != null);
                    if (isValue)
                    {
                        propertyValueTypeShouldSerializes[--propertyValueTypeShouldSerializeCount] = new ShouldSerializePropertySerializationInfo(info, shouldSerialize, encodedName);
                    }
                    else
                    {
                        propertyReferenceTypeShouldSerializes[--propertyReferenceTypeShouldSerializeCount] = new ShouldSerializePropertySerializationInfo(info, shouldSerialize, encodedName);
                    }
                }
                else
                {
                    if (isValue)
                    {
                        propertyValueTypes[--propertyValueTypeCount] = new PropertySerializationInfo(info, encodedName);
                    }
                    else
                    {
                        propertyReferenceTypes[--propertyReferenceTypeCount] = new PropertySerializationInfo(info, encodedName);
                    }
                }
            }
            return (propertyValueTypes, propertyReferenceTypes, propertyValueTypeShouldSerializes, propertyReferenceTypeShouldSerializes);
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

        private static void CollectEachField(FieldInfo info, out SerializeType serializeType, out string encodedName, out bool isValue, Type type)
        {
            isValue = info.FieldType.IsValueType;
            serializeType = SerializeType.SerializeAlways;
            encodedName = info.Name;
            if (
                info.IsStatic
                || typeof(Delegate).IsAssignableFrom(info.FieldType)
            )
            {
                serializeType = SerializeType.Ignore;
                return;
            }

            var attributes = info.GetCustomAttributes();
            var hasSerializeFieldAttribute = false;

            foreach (var attribute in attributes)
            {
                var attributeType = attribute.GetType();
                var name = attributeType.FullName;
                switch (name)
                {
                    case "System.NonSerializedAttribute":
                    case "System.Runtime.Serialization.IgnoreDataMemberAttribute":
                        serializeType = SerializeType.Ignore;
                        return;
                    case "System.Runtime.Serialization.DataMemberAttribute":
                        encodedName = attributeType.GetProperty("Name")?.GetValue(attribute) as string ?? throw new NullReferenceException();
                        break;
                    case "UnityEngine.SerializeFieldAttribute":
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
            if (!info.CanRead)
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