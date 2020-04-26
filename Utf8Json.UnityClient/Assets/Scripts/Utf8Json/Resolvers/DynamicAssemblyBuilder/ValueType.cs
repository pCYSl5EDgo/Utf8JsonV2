// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class ValueEmbedHelper
    {
        public static void Factory(Type targetType, in TypeAnalyzeResult analyzeResult, in BuilderSet builderSet, BinaryDictionary dataFieldDictionary)
        {
            ValueTypeEmbedTypelessHelper.SerializeTypeless(targetType, builderSet.SerializeStatic, builderSet.SerializeTypeless);
            ValueTypeEmbedTypelessHelper.DeserializeTypeless(targetType, builderSet.DeserializeStatic, builderSet.DeserializeTypeless);

            ValueTypeSerializeStaticHelper.SerializeStatic(builderSet.Type, builderSet.SerializeStatic, in analyzeResult, dataFieldDictionary);
            if (analyzeResult.ExtensionData.Info is null)
            {
                DeserializeStaticNoExtensionData(builderSet.Type, builderSet.DeserializeStatic, in analyzeResult);
            }
            else
            {
                DeserializeStaticWithExtensionData(builderSet.Type, builderSet.DeserializeStatic, in analyzeResult);
            }
        }

        private static void DeserializeStaticWithExtensionData(TypeBuilder typeBuilder, MethodBuilder deserializeStatic, in TypeAnalyzeResult analyzeResult)
        {
            deserializeStatic.InitLocals = true;
            var processor = deserializeStatic.GetILGenerator();
            var answerVariable = processor.DeclareLocal(deserializeStatic.ReturnType);
            processor.LdLoc(answerVariable).Emit(OpCodes.Ret);
        }

        private static void DeserializeStaticNoExtensionData(TypeBuilder typeBuilder, MethodBuilder deserializeStatic, in TypeAnalyzeResult analyzeResult)
        {
            deserializeStatic.InitLocals = true;
            var processor = deserializeStatic.GetILGenerator();
            var answerVariable = processor.DeclareLocal(deserializeStatic.ReturnType);
            processor.LdLoc(answerVariable).Emit(OpCodes.Ret);
        }
    }
}
