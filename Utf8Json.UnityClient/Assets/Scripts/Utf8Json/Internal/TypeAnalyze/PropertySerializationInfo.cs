// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct PropertySerializationInfo : IMemberContainer, IComparable<PropertySerializationInfo>
    {
        public readonly PropertyInfo Info;
        private readonly byte[] bytes;
#if CSHARP_8_OR_NEWER
        public IJsonFormatter? Formatter { get; }
#else
        public IJsonFormatter Formatter { get; }
#endif

        public Type TargetType => Info.PropertyType;

        public DirectTypeEnum IsFormatterDirect { get; }

        public ReadOnlySpan<byte> GetPropertyNameRaw()
        {
            return bytes.AsSpan(2, bytes.Length - 4);
        }

        public ReadOnlySpan<byte> GetPropertyNameWithQuotation()
        {
            return bytes.AsSpan(1, bytes.Length - 2);
        }

        public ReadOnlySpan<byte> GetPropertyNameWithQuotationAndNameSeparator()
        {
            return bytes.AsSpan(1);
        }

        public ReadOnlySpan<byte> GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator()
        {
            return bytes;
        }

        public PropertySerializationInfo(PropertyInfo info, string name,
#if CSHARP_8_OR_NEWER
            IJsonFormatter?
#else
            IJsonFormatter
#endif
                formatter)
        {
            Info = info;
            Formatter = formatter;
            bytes = PropertyNameHelper.CalculatePropertyNameBytes(name);
            IsFormatterDirect = DirectTypeEnumHelper.FromTypeAndFormatter(info.PropertyType, formatter);
        }

#if CSHARP_8_OR_NEWER
        public object? GetValue(object @this)
#else
        public object GetValue(object @this)
#endif
        {
            return Info.GetValue(@this);
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
