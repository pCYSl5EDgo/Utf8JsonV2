// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal.Reflection;

namespace Utf8Json.Internal
{
    // 大変腹立たしいことに普通にtypeof(ReadOnlySpan<byte>).GetMethod("get_Item")で得たMethodInfo（非null確認済み）を
    // DynamicAssemblyからCallするとMissingMethodExceptionで死ぬ
    public static class SpanHelper
    {
        public static ref readonly byte get_Item(ref ReadOnlySpan<byte> span, int index)
        {
            return ref span[index];
        }
    }

    public static class StaticHelper
    {
        public static IntPtr GetSerializeStatic(Type formatterType)
        {
            return GetFunctionPointer(formatterType, "SerializeStatic");
        }

        public static IntPtr GetDeserializeStatic(Type formatterType)
        {
            return GetFunctionPointer(formatterType, "DeserializeStatic");
        }

        public static IntPtr GetSerializeStaticTypeless(Type formatterType)
        {
            return GetFunctionPointer(formatterType, "SerializeStaticTypeless");
        }

        public static IntPtr GetDeserializeStaticTypeless(Type formatterType)
        {
            return GetFunctionPointer(formatterType, "DeserializeStaticTypeless");
        }

        public static IntPtr GetCalcByteLengthForSerialization(Type formatterType)
        {
            return GetFunctionPointer(formatterType, "CalcByteLengthForSerialization");
        }

        public static IntPtr GetSerializeSpan(Type formatterType)
        {
            return GetFunctionPointer(formatterType, "SerializeSpan");
        }

        private static IntPtr GetFunctionPointer(Type formatterType, string name)
        {
            var method = formatterType.GetMethodStatic(name);
            return method?.MethodHandle.GetFunctionPointer() ?? IntPtr.Zero;
        }
    }
}
