// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Formatters;

namespace Utf8Json.Internal
{
    public enum DirectTypeEnum
    {
        Byte,
        SByte,
        UInt16,
        Int16,
        UInt32,
        Int32,
        UInt64,
        Int64,
        Boolean,
        Char,
        Single,
        Double,
        String,
        None,
    }

    public static class DirectTypeEnumHelper
    {
#if CSHARP_8_OR_NEWER
        public static DirectTypeEnum FromTypeAndFormatter(Type targetType, IJsonFormatter? formatter)
#else
        public static DirectTypeEnum FromTypeAndFormatter(Type targetType, IJsonFormatter formatter)
#endif
        {
            if (targetType.IsPrimitive)
            {
                if (targetType == typeof(int))
                {
                    if (formatter == null || formatter is Int32Formatter)
                    {
                        return DirectTypeEnum.Int32;
                    }
                }
                else if (targetType == typeof(byte))
                {
                    if (formatter == null || formatter is ByteFormatter)
                    {
                        return DirectTypeEnum.Byte;
                    }
                }
                else if (targetType == typeof(long))
                {
                    if (formatter == null || formatter is Int64Formatter)
                    {
                        return DirectTypeEnum.Int64;
                    }
                }
                else if (targetType == typeof(ulong))
                {
                    if (formatter == null || formatter is UInt64Formatter)
                    {
                        return DirectTypeEnum.UInt64;
                    }
                }
                else if (targetType == typeof(float))
                {
                    if (formatter == null || formatter is SingleFormatter)
                    {
                        return DirectTypeEnum.Single;
                    }
                }
                else if (targetType == typeof(double))
                {
                    if (formatter == null || formatter is DoubleFormatter)
                    {
                        return DirectTypeEnum.Double;
                    }
                }
                else if (targetType == typeof(bool))
                {
                    if (formatter == null || formatter is BooleanFormatter)
                    {
                        return DirectTypeEnum.Boolean;
                    }
                }
                else if (targetType == typeof(uint))
                {
                    if (formatter == null || formatter is UInt32Formatter)
                    {
                        return DirectTypeEnum.UInt32;
                    }
                }
                else if (targetType == typeof(sbyte))
                {
                    if (formatter == null || formatter is SByteFormatter)
                    {
                        return DirectTypeEnum.SByte;
                    }
                }
                else if (targetType == typeof(short))
                {
                    if (formatter == null || formatter is Int16Formatter)
                    {
                        return DirectTypeEnum.Int16;
                    }
                }
                else if (targetType == typeof(ushort))
                {
                    if (formatter == null || formatter is UInt16Formatter)
                    {
                        return DirectTypeEnum.UInt16;
                    }
                }
                else if (targetType == typeof(char))
                {
                    if (formatter == null || formatter is CharFormatter)
                    {
                        return DirectTypeEnum.Char;
                    }
                }
            }
            else if (targetType == typeof(string) && (formatter == null || formatter is NullableStringFormatter))
            {
                return DirectTypeEnum.String;
            }

            return DirectTypeEnum.None;
        }
    }
}
