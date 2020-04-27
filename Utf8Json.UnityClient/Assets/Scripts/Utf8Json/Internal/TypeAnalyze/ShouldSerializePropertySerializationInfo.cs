// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct ShouldSerializePropertySerializationInfo : IShouldSerializeMemberContainer, IPropertyMemberContainer, IComparable<ShouldSerializePropertySerializationInfo>
    {
        public PropertyInfo Info { get; }
        public MethodInfo ShouldSerialize { get; }

        public Type TargetType => Info.PropertyType;

        public DirectTypeEnum IsFormatterDirect { get; }

        public string MemberName { get; }

        public bool ShouldIntern => TargetType == typeof(string) && !(Info.GetCustomAttribute<StringInternAttribute>() is null);

#if CSHARP_8_OR_NEWER
        public JsonFormatterAttribute? FormatterInfo { get; }
        public MethodInfo? AddMethodInfo { get; }
#else
        public JsonFormatterAttribute FormatterInfo { get; }
        public MethodInfo AddMethodInfo { get; }
#endif

#if CSHARP_8_OR_NEWER
        public ShouldSerializePropertySerializationInfo(PropertyInfo info, MethodInfo shouldSerialize, string name, JsonFormatterAttribute? formatterInfo)
#else
        public ShouldSerializePropertySerializationInfo(PropertyInfo info, MethodInfo shouldSerialize, string name, JsonFormatterAttribute formatterInfo)
#endif
        {
            Info = info;
            ShouldSerialize = shouldSerialize;
            MemberName = name;
            FormatterInfo = formatterInfo;
            IsFormatterDirect = DirectTypeEnumHelper.FromTypeAndFormatter(info.PropertyType, FormatterInfo?.FormatterType);
            var addAttribute = info.GetCustomAttribute<AddAttribute>();
            AddMethodInfo = addAttribute?.GetMethod(addAttribute.Type ?? info.PropertyType);
        }

        public int CompareTo(ShouldSerializePropertySerializationInfo other)
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
