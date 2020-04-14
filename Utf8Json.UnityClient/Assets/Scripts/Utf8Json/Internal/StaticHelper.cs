// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public static class StaticHelper
    {
        public static IntPtr GetSerializeStatic(Type formatterType)
        {
            var serialize = formatterType.GetMethod("SerializeStatic");
            return serialize == null ? IntPtr.Zero : serialize.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetDeserializeStatic(Type formatterType)
        {
            var deserialize = formatterType.GetMethod("DeserializeStatic");
            return deserialize == null ? IntPtr.Zero : deserialize.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetCalcByteLengthForSerialization(Type formatterType)
        {
            var method = formatterType.GetMethod("CalcByteLengthForSerialization");
            return method == null ? IntPtr.Zero : method.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetSerializeSpan(Type formatterType)
        {
            var method = formatterType.GetMethod("SerializeSpan");
            return method == null ? IntPtr.Zero : method.MethodHandle.GetFunctionPointer();
        }
    }
}
