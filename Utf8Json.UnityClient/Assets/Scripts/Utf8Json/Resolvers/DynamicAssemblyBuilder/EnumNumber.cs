// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Resolvers
{
    public sealed partial class DynamicAssemblyBuilderResolver
    {
        private static void EnumNumberFactory(Type targetType, in BuilderSet builderSet)
        {
            builderSet.Type.AddInterfaceImplementation(typeof(IObjectPropertyNameFormatter<>).MakeGenericType(targetType));

            GenerateIntermediateLanguageCodesForSerializeTypeLessValueType(targetType, builderSet.SerializeStatic, builderSet.SerializeTypeless);
            GenerateIntermediateLanguageCodesForDeserializeTypelessValueType(targetType, builderSet.DeserializeStatic, builderSet.DeserializeTypeless);

            var underlyingType = targetType.GetEnumUnderlyingType();

            var writeNumber = GetWriteNumber(underlyingType);
            DefineEnumNumberSerializeToPropertyName(targetType, builderSet.Type, underlyingType, writeNumber);
            GenerateEnumNumberSerializeStatic(builderSet.SerializeStatic, writeNumber);

            var readNumber = GetReadNumber(underlyingType);
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
            var deserializeFromPropertyName = typeBuilder.DefineMethod(
                "DeserializeFromPropertyName",
                InstanceMethodFlags,
                targetType,
                new[]
                {
                    typeof(JsonReader).MakeByRefType(),
                    typeof(JsonSerializerOptions),
                }
            );
            var skipWhiteSpace = typeof(JsonReader).GetMethod("SkipWhiteSpace");
            Debug.Assert(!(skipWhiteSpace is null));
            var advance = typeof(JsonReader).GetMethod("Advance");
            Debug.Assert(!(advance is null));

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

        private static MethodInfo GetReadNumber(Type underlyingType)
        {
            var answer = typeof(JsonReaderExtension).GetMethod("Read" + underlyingType.Name);
            Debug.Assert(!(answer is null));
            return answer;
        }

        private static void DefineEnumNumberSerializeToPropertyName(Type targetType, TypeBuilder typeBuilder, Type underlyingType, MethodInfo writeNumber)
        {
            var serializeToPropertyName = typeBuilder.DefineMethod(
                "SerializeToPropertyName",
                InstanceMethodFlags,
                typeof(void),
                new[]
                {
                    typeof(JsonWriter).MakeByRefType(),
                    targetType,
                    typeof(JsonSerializerOptions),
                });
            var writeQuotation = typeof(JsonWriter).GetMethod(nameof(JsonWriter.WriteQuotation));
            Debug.Assert(!(writeQuotation is null));

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

        private static MethodInfo GetWriteNumber(Type underlyingType)
        {
#if CSHARP_8_OR_NEWER
            MethodInfo? writeNumber;
#else
            MethodInfo writeNumber;
#endif
            if (underlyingType == typeof(ulong))
            {
                writeNumber = typeof(JsonWriter).GetMethod("Write", new[] { underlyingType });
            }
            else if (underlyingType == typeof(uint))
            {
                writeNumber = typeof(JsonWriter).GetMethod("Write", new[] { underlyingType });
            }
            else
            {
                writeNumber = typeof(JsonWriterExtension).GetMethod("Write", new[] { typeof(JsonWriter).MakeByRefType(), underlyingType });
            }

            Debug.Assert(!(writeNumber is null));
            return writeNumber;
        }
    }
}
