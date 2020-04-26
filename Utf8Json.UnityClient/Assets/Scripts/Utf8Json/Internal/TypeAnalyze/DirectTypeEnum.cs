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
        Single,
        Double,
        Char,
        String,
        Boolean,
        None,
    }

    public static class DirectTypeEnumHelper
    {
#if CSHARP_8_OR_NEWER
        public static DirectTypeEnum FromTypeAndFormatter(Type targetType, Type? formatterType)
#else
        public static DirectTypeEnum FromTypeAndFormatter(Type targetType, Type formatterType)
#endif
        {
            if (targetType.IsPrimitive)
            {
                if (targetType == typeof(int))
                {
                    if (formatterType == null || formatterType == typeof(Int32Formatter))
                    {
                        return DirectTypeEnum.Int32;
                    }
                }
                else if (targetType == typeof(byte))
                {
                    if (formatterType == null || formatterType == typeof(ByteFormatter))
                    {
                        return DirectTypeEnum.Byte;
                    }
                }
                else if (targetType == typeof(long))
                {
                    if (formatterType == null || formatterType == typeof(Int64Formatter))
                    {
                        return DirectTypeEnum.Int64;
                    }
                }
                else if (targetType == typeof(ulong))
                {
                    if (formatterType == null || formatterType == typeof(UInt64Formatter))
                    {
                        return DirectTypeEnum.UInt64;
                    }
                }
                else if (targetType == typeof(float))
                {
                    if (formatterType == null || formatterType == typeof(SingleFormatter))
                    {
                        return DirectTypeEnum.Single;
                    }
                }
                else if (targetType == typeof(double))
                {
                    if (formatterType == null || formatterType == typeof(DoubleFormatter))
                    {
                        return DirectTypeEnum.Double;
                    }
                }
                else if (targetType == typeof(bool))
                {
                    if (formatterType == null || formatterType == typeof(BooleanFormatter))
                    {
                        return DirectTypeEnum.Boolean;
                    }
                }
                else if (targetType == typeof(uint))
                {
                    if (formatterType == null || formatterType == typeof(UInt32Formatter))
                    {
                        return DirectTypeEnum.UInt32;
                    }
                }
                else if (targetType == typeof(sbyte))
                {
                    if (formatterType == null || formatterType == typeof(SByteFormatter))
                    {
                        return DirectTypeEnum.SByte;
                    }
                }
                else if (targetType == typeof(short))
                {
                    if (formatterType == null || formatterType == typeof(Int16Formatter))
                    {
                        return DirectTypeEnum.Int16;
                    }
                }
                else if (targetType == typeof(ushort))
                {
                    if (formatterType == null || formatterType == typeof(UInt16Formatter))
                    {
                        return DirectTypeEnum.UInt16;
                    }
                }
                else if (targetType == typeof(char))
                {
                    if (formatterType == null || formatterType == typeof(CharFormatter))
                    {
                        return DirectTypeEnum.Char;
                    }
                }
            }
            else if (targetType == typeof(string) && (formatterType == null || formatterType == typeof(NullableStringFormatter)))
            {
                return DirectTypeEnum.String;
            }

            return DirectTypeEnum.None;
        }
    }
}
