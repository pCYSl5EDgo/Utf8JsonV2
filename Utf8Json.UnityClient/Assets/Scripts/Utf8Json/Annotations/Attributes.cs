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
#if CSHARP_8_OR_NEWER
        public Type? Type;
#else
        public Type Type;
#endif
        public string MethodName;
        public Type[] ParameterTypes;

#if CSHARP_8_OR_NEWER
        public MethodInfo? GetMethod(Type type)
#else
        public MethodInfo GetMethod(Type type)
#endif
        {
            var methodInfo = type.GetMethod(MethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, ParameterTypes, null);
            return methodInfo;
        }

        public AddAttribute(string methodName, Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0 || parameterTypes.Length > 3)
            {
                throw new ArgumentOutOfRangeException(parameterTypes.Length.ToString(CultureInfo.InvariantCulture));
            }

            Type = default;
            MethodName = methodName;
            ParameterTypes = parameterTypes;
        }

        public AddAttribute(Type type, string methodName, Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0 || parameterTypes.Length > 3)
            {
                throw new ArgumentOutOfRangeException(parameterTypes.Length.ToString(CultureInfo.InvariantCulture));
            }

            Type = type;
            MethodName = methodName;
            ParameterTypes = parameterTypes;
        }
    }
}
