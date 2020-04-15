// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable TypeParameterCanBeVariant

namespace Utf8Json.Internal
{
    public static class StaticHelper
    {
        public static IntPtr GetSerializeStatic(Type formatterType)
        {
            var method = formatterType.GetMethod("SerializeStatic");
            if (method == null)
            {
                return IntPtr.Zero;
            }

            return method.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetDeserializeStatic(Type formatterType)
        {
            var method = formatterType.GetMethod("DeserializeStatic");
            if (method == null)
            {
                return IntPtr.Zero;
            }

            return method.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetCalcByteLengthForSerialization(Type formatterType)
        {
            var method = formatterType.GetMethod("CalcByteLengthForSerialization");
            if (method == null)
            {
                return IntPtr.Zero;
            }

            return method.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetSerializeSpan(Type formatterType)
        {
            var method = formatterType.GetMethod("SerializeSpan");
            if (method == null)
            {
                return IntPtr.Zero;
            }

            return method.MethodHandle.GetFunctionPointer();
        }
    }
}
