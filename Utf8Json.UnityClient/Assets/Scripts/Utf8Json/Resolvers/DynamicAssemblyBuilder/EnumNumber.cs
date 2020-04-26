// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal.Reflection;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class EnumNumberEmbedHelper
    {
        public static void Factory(Type targetType, in BuilderSet builderSet)
        {
            builderSet.Type.AddInterfaceImplementation(typeof(IObjectPropertyNameFormatter<>).MakeGeneric(targetType));

            ValueTypeEmbedTypelessHelper.SerializeTypeless(targetType, builderSet.SerializeStatic, builderSet.SerializeTypeless);
            ValueTypeEmbedTypelessHelper.DeserializeTypeless(targetType, builderSet.DeserializeStatic, builderSet.DeserializeTypeless);

            var underlyingType = targetType.GetEnumUnderlyingType();

            var writeNumber = ReadWritePrimitive.GetWriteNumber(underlyingType);
            DefineEnumNumberSerializeToPropertyName(targetType, builderSet.Type, writeNumber);
            GenerateEnumNumberSerializeStatic(builderSet.SerializeStatic, writeNumber);

            var readNumber = ReadWritePrimitive.GetReadNumber(underlyingType);
            DefineEnumNumberDeserializeFromPropertyName(targetType, builderSet.Type, readNumber);
            GenerateEnumNumberDeserializeStatic(builderSet.DeserializeStatic, readNumber);
        }

        private static void GenerateEnumNumberDeserializeStatic(MethodBuilder deserializeStatic, MethodInfo readNumber)
        {
            var processor = deserializeStatic.GetILGenerator();

            processor.Emit(OpCodes.Ldarg_0);
            processor.EmitCall(OpCodes.Call, readNumber, null);

            processor.Emit(OpCodes.Ret);
        }

        private static void GenerateEnumNumberSerializeStatic(MethodBuilder serializeStatic, MethodInfo writeNumber)
        {
            var processor = serializeStatic.GetILGenerator();

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldarg_1);
            processor.EmitCall(OpCodes.Call, writeNumber, null);

            processor.Emit(OpCodes.Ret);
        }

        private static void DefineEnumNumberDeserializeFromPropertyName(Type targetType, TypeBuilder typeBuilder, MethodInfo readNumber)
        {
            var paramTypes = TypeArrayHolder.TypeArrayLength2;
            paramTypes[0] = typeof(JsonReader).MakeByRefType();
            paramTypes[1] = typeof(JsonSerializerOptions);
            var deserializeFromPropertyName = typeBuilder.DefineMethod(
                "DeserializeFromPropertyName",
                DynamicAssemblyBuilderResolver.InstanceMethodFlags,
                targetType,
                paramTypes
            );
            var skipWhiteSpace = typeof(JsonReader).GetMethodEmptyParameter("SkipWhiteSpace");
            var advance = typeof(JsonReader).GetMethodInstance("Advance");

            var processor = deserializeFromPropertyName.GetILGenerator();

            processor.Emit(OpCodes.Ldarg_1);
            processor.EmitCall(OpCodes.Call, skipWhiteSpace, null);

            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.EmitCall(OpCodes.Call, advance, null);

            processor.Emit(OpCodes.Ldarg_1);
            processor.EmitCall(OpCodes.Call, readNumber, null);

            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.EmitCall(OpCodes.Call, advance, null);

            processor.Emit(OpCodes.Ret);
        }

        private static void DefineEnumNumberSerializeToPropertyName(Type targetType, TypeBuilder typeBuilder, MethodInfo writeNumber)
        {
            var typeHolder = TypeArrayHolder.TypeArrayLength3;
            typeHolder[0] = typeof(JsonWriter).MakeByRefType();
            typeHolder[1] = targetType;
            typeHolder[2] = typeof(JsonSerializerOptions);
            var serializeToPropertyName = typeBuilder.DefineMethod(
                "SerializeToPropertyName",
                DynamicAssemblyBuilderResolver.InstanceMethodFlags,
                typeof(void),
                typeHolder);

            var writeQuotation = typeof(JsonWriter).GetMethodEmptyParameter(nameof(JsonWriter.WriteQuotation));

            var processor = serializeToPropertyName.GetILGenerator();
            processor.Emit(OpCodes.Ldarg_1);
            processor.EmitCall(OpCodes.Call, writeQuotation, null);

            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.EmitCall(OpCodes.Call, writeNumber, null);

            processor.Emit(OpCodes.Ldarg_1);
            processor.EmitCall(OpCodes.Call, writeQuotation, null);

            processor.Emit(OpCodes.Ret);
        }
    }
}
