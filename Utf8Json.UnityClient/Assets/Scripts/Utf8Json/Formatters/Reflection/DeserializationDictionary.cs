// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Utf8Json.Internal;

// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Formatters
{
    public sealed class DeserializationDictionary
    {
        public readonly struct Setter
        {
#if CSHARP_8_OR_NEWER
            public readonly FieldInfo? Field;
            public readonly PropertyInfo? Property;
            public readonly IJsonFormatter? Formatter;
#else
            public readonly FieldInfo Field;
            public readonly PropertyInfo Property;
            public readonly IJsonFormatter Formatter;
#endif

            public readonly Type TargetType;
            public readonly byte[] Bytes;

            public Setter(FieldInfo info, byte[] bytes,
#if CSHARP_8_OR_NEWER
                IJsonFormatter?
#else
                IJsonFormatter
#endif
                    formatter)
            {
                Field = info;
                Bytes = bytes;
                Formatter = formatter;
                Property = default;
                TargetType = info.FieldType;
            }

            public Setter(PropertyInfo info, byte[] bytes,
#if CSHARP_8_OR_NEWER
                IJsonFormatter?
#else
                IJsonFormatter
#endif
                    formatter)
            {
                Field = default;
                Property = info;
                Bytes = bytes;
                Formatter = formatter;
                TargetType = info.PropertyType;
            }

#if CSHARP_8_OR_NEWER
            public void SetValue(object @this, object? value)
#else
            public void SetValue(object @this, object value)
#endif
            {
                if (Field != null)
                {
                    Field.SetValue(@this, value);
                    return;
                }

                if (Property != null)
                {
                    Property.SetValue(@this, value);
                }
            }
        }

#if CSHARP_8_OR_NEWER
        private readonly Setter[]?[] valueArrayArray;
#else
        private readonly Setter[][] valueArrayArray;
#endif

        public DeserializationDictionary(ReadOnlySpan<Setter> setters)
        {
            if (setters.Length == 0)
            {
                valueArrayArray = Array.Empty<Setter[]>();
                return;
            }

            var maxLength = 0;
            foreach (ref readonly var setter in setters)
            {
                if (maxLength < setter.Bytes.Length)
                {
                    maxLength = setter.Bytes.Length;
                }
            }

            valueArrayArray = maxLength == 0 ? Array.Empty<Setter[]>() : new Setter[maxLength][];

            foreach (ref readonly var setter in setters)
            {
                var bytesLength = setter.Bytes.Length;
                ref var parameterTypeArray = ref valueArrayArray[bytesLength - 1];
                if (parameterTypeArray == null)
                {
                    parameterTypeArray = new[]
                    {
                        setter,
                    };
                }
                else
                {
                    Array.Resize(ref parameterTypeArray, parameterTypeArray.Length + 1);
                    parameterTypeArray[parameterTypeArray.Length - 1] = setter;
                }
            }
        }

        public bool TryFindParameter(ReadOnlySpan<byte> propertyName, out Setter setter)
        {
            if (propertyName.IsEmpty)
            {
                goto NOT_FOUND;
            }

            var key = propertyName.Length - 1;
            if (key >= valueArrayArray.Length)
            {
                goto NOT_FOUND;
            }

            var keyArray = valueArrayArray[key];
            if (keyArray == null)
            {
                goto NOT_FOUND;
            }

            if (propertyName.SequenceEqual(keyArray[0].Bytes))
            {
                setter = keyArray[0];
                return true;
            }

            for (var keyIndex = 1; keyIndex < keyArray.Length; keyIndex++)
            {
                if (!propertyName.SequenceEqual(keyArray[keyIndex].Bytes))
                {
                    continue;
                }

                setter = keyArray[keyIndex];
                return true;
            }

        NOT_FOUND:
            setter = default;
            return false;
        }

        public bool TryFindParameterIgnoreCase(ReadOnlySpan<byte> propertyName, out Setter setter)
        {
            if (propertyName.IsEmpty)
            {
                goto NOT_FOUND;
            }

            var key = propertyName.Length - 1;
            if (key >= valueArrayArray.Length)
            {
                goto NOT_FOUND;
            }

            var keyArray = valueArrayArray[key];
            if (keyArray == null)
            {
                goto NOT_FOUND;
            }

            if (PropertyNameHelper.SequenceEqualsIgnoreCase(keyArray[0].Bytes, propertyName))
            {
                setter = keyArray[0];
                return true;
            }

            for (var keyIndex = 1; keyIndex < keyArray.Length; keyIndex++)
            {
                if (!PropertyNameHelper.SequenceEqualsIgnoreCase(keyArray[keyIndex].Bytes, propertyName))
                {
                    continue;
                }

                setter = keyArray[keyIndex];
                return true;
            }

        NOT_FOUND:
            setter = default;
            return false;
        }
    }
}
