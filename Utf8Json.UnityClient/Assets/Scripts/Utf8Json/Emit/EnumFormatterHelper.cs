// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Emit
{
    public static class EnumFormatterHelper
    {
        private static readonly Type jsonSerializerOptions;
        private static readonly Type jsonWriterByRef;
        private static readonly Type jsonReaderByRef;
        private static readonly Type jsonWriterExtension;
        private static readonly Type jsonReaderExtension;
        private static readonly Type jsonSerializeAction;
        private static readonly Type jsonDeserializeFunc;

        static EnumFormatterHelper()
        {
            var _0 = Type.GetType("Utf8Json.JsonSerializerOptions", true, false);
            Debug.Assert(_0 != null, nameof(jsonSerializerOptions) + " != null");
            jsonSerializerOptions = _0;
            var jsonWriter = Type.GetType("Utf8Json.JsonWriter", true, false);
            Debug.Assert(jsonWriter != null, nameof(jsonWriter) + " != null");
            jsonWriterByRef = jsonWriter.MakeByRefType();
            var jsonReader = Type.GetType("Utf8Json.JsonReader", true, false);
            Debug.Assert(jsonReader != null, nameof(jsonReader) + " != null");
            jsonReaderByRef = jsonReader.MakeByRefType();
            var _1 = Type.GetType("Utf8Json.JsonReaderExtension", true, false);
            Debug.Assert(_1 != null, nameof(jsonReaderExtension) + " != null");
            jsonReaderExtension = _1;
            var _2 = Type.GetType("Utf8Json.JsonWriterExtension", true, false);
            Debug.Assert(_2 != null, nameof(jsonWriterExtension) + " != null");
            jsonWriterExtension = _2;
            var _3 = Type.GetType("Utf8Json.JsonSerializeAction`1", true, false);
            Debug.Assert(_3 != null, nameof(jsonSerializeAction) + " != null");
            jsonSerializeAction = _3;
            var _4 = Type.GetType("Utf8Json.JsonDeserializeFunc`1", true, false);
            Debug.Assert(_4 != null, nameof(jsonDeserializeFunc) + " != null");
            jsonDeserializeFunc = _4;
        }

        public static Delegate GetSerializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            Debug.Assert(underlyingType != null);
            var dynamicMethod = new DynamicMethod("EnumSerializeByUnderlyingValue", null, new[] { jsonWriterByRef, type, jsonSerializerOptions }, type.Module, true);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // writer
            il.Emit(OpCodes.Ldarg_1); // value
            var runtimeMethod = jsonWriterExtension.GetRuntimeMethod("Write", new[] { jsonWriterByRef, underlyingType });
            Debug.Assert(runtimeMethod != null);
            il.Emit(OpCodes.Call, runtimeMethod);
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(jsonSerializeAction.MakeGenericType(type));
        }

        public static Delegate GetDeserializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            var dynamicMethod = new DynamicMethod("EnumDeserializeByUnderlyingValue", type, new[] { jsonReaderByRef, jsonSerializerOptions }, type.Module, true);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // reader
            var runtimeMethod = jsonReaderExtension.GetRuntimeMethod("Read" + underlyingType.Name, new[] { jsonReaderByRef });
            Debug.Assert(runtimeMethod != null);
            il.Emit(OpCodes.Call, runtimeMethod);
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(jsonDeserializeFunc.MakeGenericType(type));
        }
    }
}
#endif
