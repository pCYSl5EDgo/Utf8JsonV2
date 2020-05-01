// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct ShouldSerializeFieldSerializationInfo : IShouldSerializeMemberContainer, IComparable<ShouldSerializeFieldSerializationInfo>
    {
        public readonly FieldInfo Info;
        public MethodInfo ShouldSerialize { get; }

#if CSHARP_8_OR_NEWER
        public JsonFormatterAttribute? FormatterInfo { get; }
#else
        public JsonFormatterAttribute FormatterInfo { get; }
#endif

        public Type TargetType => Info.FieldType;

        public DirectTypeEnum IsFormatterDirect { get; }

        public string MemberName { get; }

        public bool ShouldIntern => TargetType == typeof(string) && !(Info.GetCustomAttribute<StringInternAttribute>() is null);

#if CSHARP_8_OR_NEWER
        public ShouldSerializeFieldSerializationInfo(FieldInfo info, MethodInfo shouldSerialize, string name, JsonFormatterAttribute? formatterInfo)
#else
        public ShouldSerializeFieldSerializationInfo(FieldInfo info, MethodInfo shouldSerialize, string name, JsonFormatterAttribute formatterInfo)
#endif
        {
            Info = info;
            ShouldSerialize = shouldSerialize;
            MemberName = name;
            FormatterInfo = formatterInfo;
            IsFormatterDirect = DirectTypeEnumHelper.FromTypeAndFormatter(info.FieldType, FormatterInfo?.FormatterType);
        }

        public int CompareTo(ShouldSerializeFieldSerializationInfo other)
        {
            var c = IsFormatterDirect.CompareTo(other.IsFormatterDirect);
            if (c == 0)
            {
                c = string.CompareOrdinal(TargetType.FullName, other.TargetType.FullName);
            }

            return c;
        }
    }
}
