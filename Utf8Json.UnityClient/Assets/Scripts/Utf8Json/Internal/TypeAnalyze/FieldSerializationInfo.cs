// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Utf8Json.Formatters;

namespace Utf8Json.Internal
{
    public readonly struct FieldSerializationInfo : IMemberContainer, IComparable<FieldSerializationInfo>
    {
        public readonly FieldInfo Info;

#if CSHARP_8_OR_NEWER
        public JsonFormatterAttribute? FormatterInfo { get; }
#else
        public JsonFormatterAttribute FormatterInfo { get; }
#endif

        public DirectTypeEnum IsFormatterDirect { get; }

        public Type TargetType => Info.FieldType;

        public string MemberName { get; }

        public int MemberNameByteLengthWithQuotation { get; }

        public bool ShouldIntern => TargetType == typeof(string) && !(Info.GetCustomAttribute<StringInternAttribute>() is null);

#if CSHARP_8_OR_NEWER
        public FieldSerializationInfo(FieldInfo info, string name, JsonFormatterAttribute? formatterInfo)
#else
        public FieldSerializationInfo(FieldInfo info, string name, JsonFormatterAttribute formatterInfo)
#endif
        {
            Info = info;
            MemberName = name;
            MemberNameByteLengthWithQuotation = NullableStringFormatter.CalcByteLength(name);
            FormatterInfo = formatterInfo;
            IsFormatterDirect = DirectTypeEnumHelper.FromTypeAndFormatter(info.FieldType, FormatterInfo?.FormatterType);
        }

        public int CompareTo(FieldSerializationInfo other)
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
