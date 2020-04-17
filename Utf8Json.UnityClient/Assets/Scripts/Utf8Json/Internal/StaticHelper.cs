// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
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
            var method = formatterType.GetMethod(name);
            return method == null ? IntPtr.Zero : method.MethodHandle.GetFunctionPointer();
        }
    }
}
