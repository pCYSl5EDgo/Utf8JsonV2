// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Utf8Json.Internal
{
    public enum TypeAnalyzeResultMemberKind
    {
        FieldValueType,
        PropertyValueType,
        FieldReferenceType,
        PropertyReferenceType,
        FieldValueTypeShouldSerialize,
        PropertyValueTypeShouldSerialize,
        FieldReferenceTypeShouldSerialize,
        PropertyReferenceTypeShouldSerialize,
    }

    public readonly struct TypeAnalyzeResult
    {
        public readonly Type TargetType;
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

        public bool AreAllPublic()
        {
            for (var i = 0; i < FieldValueTypeArray.Length; i++)
            {
                if (!FieldValueTypeArray[i].Info.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < FieldReferenceTypeArray.Length; i++)
            {
                if (!FieldReferenceTypeArray[i].Info.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < FieldValueTypeShouldSerializeArray.Length; i++)
            {
                if (!FieldValueTypeShouldSerializeArray[i].Info.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < FieldReferenceTypeShouldSerializeArray.Length; i++)
            {
                if (!FieldReferenceTypeShouldSerializeArray[i].Info.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < PropertyValueTypeArray.Length; i++)
            {
                var info = PropertyValueTypeArray[i].Info;
                Debug.Assert(!(info.GetMethod is null));
                if (!info.GetMethod.IsPublic)
                {
                    return false;
                }

                if (info.SetMethod is null) continue;
                if (!info.SetMethod.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < PropertyReferenceTypeArray.Length; i++)
            {
                var info = PropertyReferenceTypeArray[i].Info;
                Debug.Assert(!(info.GetMethod is null));
                if (!info.GetMethod.IsPublic)
                {
                    return false;
                }

                if (info.SetMethod is null) continue;
                if (!info.SetMethod.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < PropertyValueTypeShouldSerializeArray.Length; i++)
            {
                var info = PropertyValueTypeShouldSerializeArray[i].Info;
                Debug.Assert(!(info.GetMethod is null));
                if (!info.GetMethod.IsPublic)
                {
                    return false;
                }

                if (info.SetMethod is null) continue;
                if (!info.SetMethod.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < PropertyReferenceTypeShouldSerializeArray.Length; i++)
            {
                var info = PropertyReferenceTypeShouldSerializeArray[i].Info;
                Debug.Assert(!(info.GetMethod is null));
                if (!info.GetMethod.IsPublic)
                {
                    return false;
                }

                if (info.SetMethod is null) continue;
                if (!info.SetMethod.IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < OnSerializing.Length; i++)
            {
                if (!OnSerializing[i].IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < OnSerialized.Length; i++)
            {
                if (!OnSerialized[i].IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < OnDeserializing.Length; i++)
            {
                if (!OnDeserializing[i].IsPublic)
                {
                    return false;
                }
            }

            for (var i = 0; i < OnDeserialized.Length; i++)
            {
                if (!OnDeserialized[i].IsPublic)
                {
                    return false;
                }
            }

            if (!(ExtensionData.Info is null))
            {
                Debug.Assert(!(ExtensionData.Info.GetMethod is null));
                if (!ExtensionData.Info.GetMethod.IsPublic)
                {
                    return false;
                }

                if (!(ExtensionData.Info.SetMethod is null) && !ExtensionData.Info.SetMethod.IsPublic)
                {
                    return false;
                }
            }

            if (!(ConstructorData.Constructor is null))
            {
                if (!ConstructorData.Constructor.IsPublic)
                {
                    return false;
                }
            }
            else if (!(ConstructorData.FactoryMethod is null))
            {
                if (!ConstructorData.FactoryMethod.IsPublic)
                {
                    return false;
                }
            }

            return true;
        }

        public bool GetShouldIntern(TypeAnalyzeResultMemberKind kind, int index)
        {
            switch (kind)
            {
                case TypeAnalyzeResultMemberKind.FieldValueType:
                    return FieldValueTypeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.PropertyValueType:
                    return PropertyValueTypeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.FieldReferenceType:
                    return FieldReferenceTypeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.PropertyReferenceType:
                    return PropertyReferenceTypeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.FieldValueTypeShouldSerialize:
                    return FieldValueTypeShouldSerializeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.PropertyValueTypeShouldSerialize:
                    return PropertyValueTypeShouldSerializeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.FieldReferenceTypeShouldSerialize:
                    return FieldReferenceTypeShouldSerializeArray[index].ShouldIntern;
                case TypeAnalyzeResultMemberKind.PropertyReferenceTypeShouldSerialize:
                    return PropertyReferenceTypeShouldSerializeArray[index].ShouldIntern;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        public string GetMemberName(TypeAnalyzeResultMemberKind kind, int index)
        {
            switch (kind)
            {
                case TypeAnalyzeResultMemberKind.FieldValueType:
                    return FieldValueTypeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.PropertyValueType:
                    return PropertyValueTypeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.FieldReferenceType:
                    return FieldReferenceTypeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.PropertyReferenceType:
                    return PropertyReferenceTypeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.FieldValueTypeShouldSerialize:
                    return FieldValueTypeShouldSerializeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.PropertyValueTypeShouldSerialize:
                    return PropertyValueTypeShouldSerializeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.FieldReferenceTypeShouldSerialize:
                    return FieldReferenceTypeShouldSerializeArray[index].MemberName;
                case TypeAnalyzeResultMemberKind.PropertyReferenceTypeShouldSerialize:
                    return PropertyReferenceTypeShouldSerializeArray[index].MemberName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

#if CSHARP_8_OR_NEWER
        public JsonFormatterAttribute? GetFormatterInfo(TypeAnalyzeResultMemberKind kind, int index)
#else
        public JsonFormatterAttribute GetFormatterInfo(TypeAnalyzeResultMemberKind kind, int index)
#endif
        {
            switch (kind)
            {
                case TypeAnalyzeResultMemberKind.FieldValueType:
                    return FieldValueTypeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.PropertyValueType:
                    return PropertyValueTypeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.FieldReferenceType:
                    return FieldReferenceTypeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.PropertyReferenceType:
                    return PropertyReferenceTypeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.FieldValueTypeShouldSerialize:
                    return FieldValueTypeShouldSerializeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.PropertyValueTypeShouldSerialize:
                    return PropertyValueTypeShouldSerializeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.FieldReferenceTypeShouldSerialize:
                    return FieldReferenceTypeShouldSerializeArray[index].FormatterInfo;
                case TypeAnalyzeResultMemberKind.PropertyReferenceTypeShouldSerialize:
                    return PropertyReferenceTypeShouldSerializeArray[index].FormatterInfo;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        public DirectTypeEnum GetDirectTypeEnum(TypeAnalyzeResultMemberKind kind, int index)
        {
            switch (kind)
            {
                case TypeAnalyzeResultMemberKind.FieldValueType:
                    return FieldValueTypeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.PropertyValueType:
                    return PropertyValueTypeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.FieldReferenceType:
                    return FieldReferenceTypeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.PropertyReferenceType:
                    return PropertyReferenceTypeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.FieldValueTypeShouldSerialize:
                    return FieldValueTypeShouldSerializeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.PropertyValueTypeShouldSerialize:
                    return PropertyValueTypeShouldSerializeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.FieldReferenceTypeShouldSerialize:
                    return FieldReferenceTypeShouldSerializeArray[index].IsFormatterDirect;
                case TypeAnalyzeResultMemberKind.PropertyReferenceTypeShouldSerialize:
                    return PropertyReferenceTypeShouldSerializeArray[index].IsFormatterDirect;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        public Type GetTargetType(TypeAnalyzeResultMemberKind kind, int index)
        {
            switch (kind)
            {
                case TypeAnalyzeResultMemberKind.FieldValueType:
                    return FieldValueTypeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.PropertyValueType:
                    return PropertyValueTypeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.FieldReferenceType:
                    return FieldReferenceTypeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.PropertyReferenceType:
                    return PropertyReferenceTypeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.FieldValueTypeShouldSerialize:
                    return FieldValueTypeShouldSerializeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.PropertyValueTypeShouldSerialize:
                    return PropertyValueTypeShouldSerializeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.FieldReferenceTypeShouldSerialize:
                    return FieldReferenceTypeShouldSerializeArray[index].TargetType;
                case TypeAnalyzeResultMemberKind.PropertyReferenceTypeShouldSerialize:
                    return PropertyReferenceTypeShouldSerializeArray[index].TargetType;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        public TypeAnalyzeResult(Type targetType, FieldSerializationInfo[] fieldValueTypeArray, FieldSerializationInfo[] fieldReferenceTypeArray, PropertySerializationInfo[] propertyValueTypeArray, PropertySerializationInfo[] propertyReferenceTypeArray, ShouldSerializeFieldSerializationInfo[] fieldValueTypeShouldSerializeArray, ShouldSerializeFieldSerializationInfo[] fieldReferenceTypeShouldSerializeArray, ShouldSerializePropertySerializationInfo[] propertyValueTypeShouldSerializeArray, ShouldSerializePropertySerializationInfo[] propertyReferenceTypeShouldSerializeArray, MethodInfo[] onSerializing, MethodInfo[] onSerialized, MethodInfo[] onDeserializing, MethodInfo[] onDeserialized, ExtensionDataInfo extensionData, ConstructorDataInfo constructorData)
        {
            TargetType = targetType;
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
