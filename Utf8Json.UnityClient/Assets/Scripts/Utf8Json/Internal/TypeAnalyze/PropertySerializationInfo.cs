// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct PropertySerializationInfo : IPropertyMemberContainer, IComparable<PropertySerializationInfo>
    {
        public PropertyInfo Info { get; }

#if CSHARP_8_OR_NEWER
        public JsonFormatterAttribute? FormatterInfo { get; }
        public MethodInfo? AddMethodInfo { get; }
#else
        public JsonFormatterAttribute FormatterInfo { get; }
        public MethodInfo AddMethodInfo { get; }
#endif

        public Type TargetType => Info.PropertyType;

        public DirectTypeEnum IsFormatterDirect { get; }

        public string MemberName { get; }

        public bool ShouldIntern => TargetType == typeof(string) && !(Info.GetCustomAttribute<StringInternAttribute>() is null);

#if CSHARP_8_OR_NEWER
        public PropertySerializationInfo(PropertyInfo info, string name, JsonFormatterAttribute? formatterInfo)
#else
        public PropertySerializationInfo(PropertyInfo info, string name, JsonFormatterAttribute formatterInfo)
#endif
        {
            Info = info;
            MemberName = name;
            FormatterInfo = formatterInfo;
            IsFormatterDirect = DirectTypeEnumHelper.FromTypeAndFormatter(info.PropertyType, FormatterInfo?.FormatterType);
            var addAttribute = info.GetCustomAttribute<AddAttribute>();
            AddMethodInfo = addAttribute?.GetMethod(addAttribute.Type ?? info.PropertyType);
        }

        public int CompareTo(PropertySerializationInfo other)
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
