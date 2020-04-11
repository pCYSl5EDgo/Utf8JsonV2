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
#if CSHARP_8_OR_NEWER && !DEBUG
            jsonSerializerOptions = Type.GetType("Utf8Json.JsonSerializerOptions", true, false)!;
            var jsonWriter = Type.GetType("Utf8Json.JsonWriter", true, false)!;
            jsonWriterByRef = jsonWriter.MakeByRefType();
            var jsonReader = Type.GetType("Utf8Json.JsonReader", true, false)!;
            jsonReaderByRef = jsonReader.MakeByRefType();
            jsonReaderExtension = Type.GetType("Utf8Json.JsonReaderExtension", true, false)!;
            jsonWriterExtension = Type.GetType("Utf8Json.JsonWriterExtension", true, false)!;
            jsonSerializeAction = Type.GetType("Utf8Json.JsonSerializeAction`1", true, false)!;
            jsonDeserializeFunc = Type.GetType("Utf8Json.JsonDeserializeFunc`1", true, false)!;
#else
            jsonSerializerOptions = Type.GetType("Utf8Json.JsonSerializerOptions", true, false);
            Debug.Assert(jsonSerializerOptions != null, nameof(jsonSerializerOptions) + " != null");
            var jsonWriter = Type.GetType("Utf8Json.JsonWriter", true, false);
            Debug.Assert(jsonWriter != null, nameof(jsonWriter) + " != null");
            jsonWriterByRef = jsonWriter.MakeByRefType();
            var jsonReader = Type.GetType("Utf8Json.JsonReader", true, false);
            Debug.Assert(jsonReader != null, nameof(jsonReader) + " != null");
            jsonReaderByRef = jsonReader.MakeByRefType();
            jsonReaderExtension = Type.GetType("Utf8Json.JsonReaderExtension", true, false);
            Debug.Assert(jsonReaderExtension != null, nameof(jsonReaderExtension) + " != null");
            jsonWriterExtension = Type.GetType("Utf8Json.JsonWriterExtension", true, false);
            Debug.Assert(jsonWriterExtension != null, nameof(jsonWriterExtension) + " != null");
            jsonSerializeAction = Type.GetType("Utf8Json.JsonSerializeAction`1", true, false);
            Debug.Assert(jsonSerializeAction != null, nameof(jsonSerializeAction) + " != null");
            jsonDeserializeFunc = Type.GetType("Utf8Json.JsonDeserializeFunc`1", true, false);
            Debug.Assert(jsonDeserializeFunc != null, nameof(jsonDeserializeFunc) + " != null");
#endif
        }

        public static Delegate GetSerializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            var dynamicMethod = new DynamicMethod("EnumSerializeByUnderlyingValue", null, new[] { jsonWriterByRef, type, jsonSerializerOptions }, type.Module, true);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // writer
            il.Emit(OpCodes.Ldarg_1); // value
            il.Emit(OpCodes.Call, jsonWriterExtension.GetRuntimeMethod("Write", new[] { jsonWriterByRef, underlyingType }));
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(jsonSerializeAction.MakeGenericType(type));
        }

        public static Delegate GetDeserializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            var dynamicMethod = new DynamicMethod("EnumDeserializeByUnderlyingValue", type, new[] { jsonReaderByRef, jsonSerializerOptions }, type.Module, true);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // reader
            il.Emit(OpCodes.Call, jsonReaderExtension.GetRuntimeMethod("Read" + underlyingType.Name, new[] { jsonReaderByRef }));
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(jsonDeserializeFunc.MakeGenericType(type));
        }
    }
}
#endif
