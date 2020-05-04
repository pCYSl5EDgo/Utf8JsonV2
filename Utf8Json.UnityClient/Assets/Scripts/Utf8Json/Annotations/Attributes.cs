// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonFormatterAttribute : Attribute
    {
        public Type FormatterType { get; private set; }

#if CSHARP_8_OR_NEWER
        public object?[]? Arguments { get; private set; }
#else
        public object[] Arguments { get; private set; }
#endif

        public JsonFormatterAttribute(Type formatterType)
        {
            FormatterType = formatterType;
            Arguments = default;
        }

#if CSHARP_8_OR_NEWER
        public JsonFormatterAttribute(Type formatterType, params object?[] arguments)
#else
        public JsonFormatterAttribute(Type formatterType, params object[] arguments)
#endif
        {
            FormatterType = formatterType;
            Arguments = arguments;
        }
    }

    [AttributeUsage(AttributeTargets.Constructor)]
    public class SerializationConstructorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class StringInternAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RegisterTargetTypeAttribute : Attribute, IComparable<RegisterTargetTypeAttribute>
    {
        public int Index { get; }
        public Type TargetType { get; }

        public RegisterTargetTypeAttribute(Type targetType, int index)
        {
            this.TargetType = targetType;
            this.Index = index;
        }

#if CSHARP_8_OR_NEWER
        public int CompareTo(RegisterTargetTypeAttribute? other)
#else
        public int CompareTo(RegisterTargetTypeAttribute other)
#endif
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            var compareTo = Index.CompareTo(other.Index);
            if (compareTo == 0)
            {
                compareTo = TargetType.IsValueType
                    ? other.TargetType.IsValueType
                        ? 0
                        : -1
                    : other.TargetType.IsValueType
                        ? 1
                        : 0;
            }
            return compareTo;
        }
    }
}
