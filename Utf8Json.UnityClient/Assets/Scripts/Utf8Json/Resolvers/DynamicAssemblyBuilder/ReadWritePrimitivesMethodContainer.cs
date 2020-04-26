// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Utf8Json.Internal;
using Utf8Json.Internal.Reflection;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class ReadWritePrimitive
    {
        public static MethodInfo GetReadNumber(Type numberType)
        {
            if (numberType == typeof(int))
            {
                return MethodReadPrimitives[5];
            }

            if (numberType == typeof(byte))
            {
                return MethodReadPrimitives[0];
            }

            if (numberType == typeof(uint))
            {
                return MethodReadPrimitives[4];
            }

            if (numberType == typeof(ulong))
            {
                return MethodReadPrimitives[6];
            }

            return MethodReadPrimitives[
                numberType == typeof(sbyte)
                ? 1
                : numberType == typeof(ushort)
                    ? 2
                    : numberType == typeof(short)
                        ? 3
                        : 7
            ];
        }

        public static MethodInfo GetWriteNumber(Type numberType)
        {
            if (numberType == typeof(int))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.Int32];
            }

            if (numberType == typeof(ulong))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.UInt64];
            }

            if (numberType == typeof(uint))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.UInt32];
            }

            if (numberType == typeof(byte))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.Byte];
            }

            if (numberType == typeof(short))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.Int16];
            }

            if (numberType == typeof(long))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.Int64];
            }

            if (numberType == typeof(ushort))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.UInt16];
            }

            if (numberType == typeof(sbyte))
            {
                return MethodWritePrimitives[(int)DirectTypeEnum.SByte];
            }

            throw new ArgumentOutOfRangeException();
        }

        public static readonly MethodInfo[] MethodReadPrimitives =
        {
            typeof(JsonReaderExtension).GetMethodStatic("ReadByte"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadSByte"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadUInt16"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadInt16"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadUInt32"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadInt32"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadUInt64"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadInt64"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadSingle"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadDouble"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadChar"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadString"),
            typeof(JsonReaderExtension).GetMethodStatic("ReadBoolean"),
        };

        public static readonly MethodInfo[] MethodWritePrimitives =
        {
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(byte) ) ,
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(sbyte) ),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(ushort) ),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(short) ) ,
            typeof(JsonWriter).GetMethod("Write", typeof(uint)),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(int)),
            typeof(JsonWriter).GetMethod("Write", typeof(ulong)),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(long)),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(float)),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(double)),
            typeof(JsonWriterExtension).GetMethod("Write", typeof(JsonWriter).MakeByRefType(), typeof(char)) ,
            typeof(JsonWriter).GetMethod("Write", typeof(string)),
            typeof(JsonWriter).GetMethod("Write", typeof(bool)),
        };
    }
}
