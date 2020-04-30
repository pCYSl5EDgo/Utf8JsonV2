// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reflection;

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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AddAttribute : Attribute
    {
    }
}
