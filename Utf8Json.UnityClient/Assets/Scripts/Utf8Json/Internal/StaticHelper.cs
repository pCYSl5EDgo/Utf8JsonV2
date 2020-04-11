// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public static class StaticHelper
    {
        public static IntPtr GetSerializeStatic<T, TFormatter>()
            where TFormatter : class, IJsonFormatter<T>, new()
        {
            var serialize = typeof(TFormatter).GetMethod("SerializeStatic");
            return serialize == null ? IntPtr.Zero : serialize.MethodHandle.GetFunctionPointer();
        }

        public static IntPtr GetDeserializeStatic<T, TFormatter>()
            where TFormatter : class, IJsonFormatter<T>, new()
        {
            var deserialize = typeof(TFormatter).GetMethod("DeserializeStatic");
            return deserialize == null ? IntPtr.Zero : deserialize.MethodHandle.GetFunctionPointer();
        }
    }
}
