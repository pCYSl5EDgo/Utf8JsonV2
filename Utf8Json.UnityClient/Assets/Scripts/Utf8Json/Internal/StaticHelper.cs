// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public static class StaticHelper
    {
        public static IntPtr GetSerializeStatic<TFormatter>()
        {
            var serialize = typeof(TFormatter).GetMethod("SerializeStatic");
            return serialize == null ? IntPtr.Zero : serialize.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetDeserializeStatic<TFormatter>()
        {
            var deserialize = typeof(TFormatter).GetMethod("DeserializeStatic");
            return deserialize == null ? IntPtr.Zero : deserialize.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetCalcByteLengthForSerialization<T>()
        {
            var method = typeof(T).GetMethod("CalcByteLengthForSerialization");
            return method == null ? IntPtr.Zero : method.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetSerializeSpan<T>()
        {
            var method = typeof(T).GetMethod("SerializeSpan");
            return method == null ? IntPtr.Zero : method.MethodHandle.GetFunctionPointer();
        }
    }
}
