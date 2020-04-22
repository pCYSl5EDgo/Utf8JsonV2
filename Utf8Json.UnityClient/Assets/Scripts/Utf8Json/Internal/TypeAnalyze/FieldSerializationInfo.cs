// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct FieldSerializationInfo : IMemberContainer
    {
        public readonly FieldInfo Info;
        private readonly byte[] bytes;
#if CSHARP_8_OR_NEWER
        public IJsonFormatter? Formatter { get; }
#else
        public IJsonFormatter Formatter { get; }
#endif

        public DirectTypeEnum IsFormatterDirect { get; }

        public Type TargetType => Info.FieldType;

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

        public FieldSerializationInfo(FieldInfo info, string name,
#if CSHARP_8_OR_NEWER
            IJsonFormatter?
#else
            IJsonFormatter
#endif
                formatter)
        {
            Info = info;
            bytes = PropertyNameHelper.CalculatePropertyNameBytes(name);
            Formatter = formatter;
            IsFormatterDirect = DirectTypeEnumHelper.FromTypeAndFormatter(info.FieldType, formatter);
        }

#if CSHARP_8_OR_NEWER
        public object? GetValue(object @this)
#else
        public object GetValue(object @this)
#endif
        {
            return Info.GetValue(@this);
        }
    }
}
