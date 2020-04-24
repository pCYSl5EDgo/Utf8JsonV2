// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed partial class DynamicAssemblyBuilderResolver
    {
        private static void ValueTypeFactory(Type targetType, in TypeAnalyzeResult analyzeResult, in BuilderSet builderSet)
        {
            GenerateIntermediateLanguageCodesForSerializeTypeLessValueType(targetType, builderSet.SerializeStatic, builderSet.SerializeTypeless);
            GenerateIntermediateLanguageCodesForDeserializeTypelessValueType(targetType, builderSet.DeserializeStatic, builderSet.DeserializeTypeless);
        }

        private static readonly ConstructorInfo argumentNullExceptionConstructorInfo = typeof(ArgumentNullException).GetConstructor(Array.Empty<Type>()) ?? throw new InvalidOperationException();

        private static void GenerateIntermediateLanguageCodesForSerializeTypeLessValueType(Type targetType, MethodInfo serializeStatic, MethodBuilder serializeTypeless)
        {
            var processor = serializeTypeless.GetILGenerator();
            var notNull = processor.DefineLabel();
            processor.Emit(OpCodes.Ldarg_2);
            processor.Emit(OpCodes.Brtrue_S, notNull);
            {
                processor.Emit(OpCodes.Newobj, argumentNullExceptionConstructorInfo);
                processor.Emit(OpCodes.Throw);
            }

            processor.MarkLabel(notNull);
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.Emit(OpCodes.Unbox_Any, targetType);
            processor.Emit(OpCodes.Ldarg_3);
            processor.EmitCall(OpCodes.Call, serializeStatic, null);
            processor.Emit(OpCodes.Ret);
        }

        private static void GenerateIntermediateLanguageCodesForDeserializeTypelessValueType(Type targetType, MethodInfo deserializeStatic, MethodBuilder deserializeTypeless)
        {
            var processor = deserializeTypeless.GetILGenerator();
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.EmitCall(OpCodes.Call, deserializeStatic, null);
            processor.Emit(OpCodes.Box, targetType);
            processor.Emit(OpCodes.Ret);
        }
    }
}
