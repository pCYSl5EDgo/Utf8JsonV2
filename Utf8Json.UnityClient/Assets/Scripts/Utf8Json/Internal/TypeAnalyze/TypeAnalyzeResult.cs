// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct TypeAnalyzeResult
    {
        public readonly FieldSerializationInfo[] FieldValueTypeArray;
        public readonly FieldSerializationInfo[] FieldReferenceTypeArray;
        public readonly PropertySerializationInfo[] PropertyValueTypeArray;
        public readonly PropertySerializationInfo[] PropertyReferenceTypeArray;
        public readonly ShouldSerializeFieldSerializationInfo[] FieldValueTypeShouldSerializeArray;
        public readonly ShouldSerializeFieldSerializationInfo[] FieldReferenceTypeShouldSerializeArray;
        public readonly ShouldSerializePropertySerializationInfo[] PropertyValueTypeShouldSerializeArray;
        public readonly ShouldSerializePropertySerializationInfo[] PropertyReferenceTypeShouldSerializeArray;
        public readonly MethodInfo[] OnSerializing;
        public readonly MethodInfo[] OnSerialized;
        public readonly MethodInfo[] OnDeserializing;
        public readonly MethodInfo[] OnDeserialized;
        public readonly ExtensionDataInfo ExtensionData;
        public readonly ConstructorDataInfo ConstructorData;

        public TypeAnalyzeResult(FieldSerializationInfo[] fieldValueTypeArray, FieldSerializationInfo[] fieldReferenceTypeArray, PropertySerializationInfo[] propertyValueTypeArray, PropertySerializationInfo[] propertyReferenceTypeArray, ShouldSerializeFieldSerializationInfo[] fieldValueTypeShouldSerializeArray, ShouldSerializeFieldSerializationInfo[] fieldReferenceTypeShouldSerializeArray, ShouldSerializePropertySerializationInfo[] propertyValueTypeShouldSerializeArray, ShouldSerializePropertySerializationInfo[] propertyReferenceTypeShouldSerializeArray, MethodInfo[] onSerializing, MethodInfo[] onSerialized, MethodInfo[] onDeserializing, MethodInfo[] onDeserialized, ExtensionDataInfo extensionData, ConstructorDataInfo constructorData)
        {
            FieldValueTypeArray = fieldValueTypeArray;
            FieldReferenceTypeArray = fieldReferenceTypeArray;
            PropertyValueTypeArray = propertyValueTypeArray;
            PropertyReferenceTypeArray = propertyReferenceTypeArray;
            FieldValueTypeShouldSerializeArray = fieldValueTypeShouldSerializeArray;
            FieldReferenceTypeShouldSerializeArray = fieldReferenceTypeShouldSerializeArray;
            PropertyValueTypeShouldSerializeArray = propertyValueTypeShouldSerializeArray;
            PropertyReferenceTypeShouldSerializeArray = propertyReferenceTypeShouldSerializeArray;
            OnSerializing = onSerializing;
            OnSerialized = onSerialized;
            OnDeserializing = onDeserializing;
            OnDeserialized = onDeserialized;
            ExtensionData = extensionData;
            ConstructorData = constructorData;
        }
    }
}
