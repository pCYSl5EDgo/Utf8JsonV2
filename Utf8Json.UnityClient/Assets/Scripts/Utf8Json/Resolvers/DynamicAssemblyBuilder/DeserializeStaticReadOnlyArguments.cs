// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    internal readonly ref struct DeserializeStaticReadOnlyArguments
    {
        public readonly ILGenerator Processor;
        public readonly LocalBuilder AnswerVariable;
        public readonly LocalBuilder AnswerCallableVariable;
        public readonly LocalBuilder NameVariable;
        public readonly LocalBuilder ReferenceVariable;
        public readonly DeserializeDictionary Dictionary;

        public readonly Label LoopStartLabel;
        public readonly Label DefaultLabel;
        public readonly TypeAnalyzeResult AnalyzeResult;
        public readonly ReadOnlySpan<LocalBuilder> ElementVariableSpan;
        public readonly Module Module;
#if CSHARP_8_OR_NEWER
        public readonly LocalBuilder? AssignVariable;
        private readonly LocalBuilder[]? elementVariableArray;
        private readonly ArrayPool<LocalBuilder>? pool;
#else
        public readonly LocalBuilder AssignVariable;
        private readonly LocalBuilder[] elementVariableArray;
        private readonly ArrayPool<LocalBuilder> pool;
#endif

        public DeserializeStaticReadOnlyArguments(LocalBuilder answerVariable, LocalBuilder answerCallableVariable, in TypeAnalyzeResult analyzeResult, ILGenerator processor, Module module)
        {
            Processor = processor;
            AnswerVariable = answerVariable;
            AnswerCallableVariable = answerCallableVariable;
            AnalyzeResult = analyzeResult;
            Dictionary = new DeserializeDictionary(in analyzeResult);
            NameVariable = processor.DeclareLocal(typeof(ReadOnlySpan<byte>));
            ReferenceVariable = processor.DeclareLocal(typeof(byte).MakeByRefType());
            LoopStartLabel = processor.DefineLabel();
            DefaultLabel = processor.DefineLabel();
            ElementVariableSpan = ReadOnlySpan<LocalBuilder>.Empty;
            AssignVariable = default;
            elementVariableArray = default;
            pool = default;
            Module = module;
        }

        public DeserializeStaticReadOnlyArguments(LocalBuilder answerVariable, LocalBuilder answerCallableVariable, in TypeAnalyzeResult analyzeResult, ILGenerator processor, Module module, ArrayPool<LocalBuilder> pool)
        {
            Processor = processor;
            AnswerVariable = answerVariable;
            AnswerCallableVariable = answerCallableVariable;
            AnalyzeResult = analyzeResult;
            Dictionary = new DeserializeDictionary(in analyzeResult);
            NameVariable = processor.DeclareLocal(typeof(ReadOnlySpan<byte>));
            ReferenceVariable = processor.DeclareLocal(typeof(byte).MakeByRefType());
            LoopStartLabel = processor.DefineLabel();
            DefaultLabel = processor.DefineLabel();
            Module = module;

            this.pool = pool;
            var totalCount = Dictionary.TotalCount;
            AssignVariable = PrepareAssignVariable(processor, totalCount);
            elementVariableArray = pool.Rent(totalCount);
            ElementVariableSpan = elementVariableArray.AsSpan(0, totalCount);
            foreach (var lengthVariation in Dictionary.LengthVariations)
            {
                foreach (ref readonly var entry in Dictionary[lengthVariation])
                {
                    elementVariableArray[entry.UniqueIndex] = processor.DeclareLocal(analyzeResult.GetTargetType(entry.Type, entry.Index));
                }
            }
        }

        public void Dispose()
        {
            Dictionary.Dispose();
            if (elementVariableArray is null) return;
            Debug.Assert(!(pool is null));
            pool.Return(elementVariableArray);
        }

        private static LocalBuilder PrepareAssignVariable(ILGenerator processor, int totalCount)
        {
            var assignVariable = processor.DeclareLocal(typeof(uint).MakeByRefType());
            var assignVariableLength = (int)(((uint)(totalCount - 1) >> 5) + 1);
            processor
                .LdcI4(assignVariableLength << 2)
                .Emit(OpCodes.Conv_U);
            processor.Emit(OpCodes.Localloc);
            processor.Dup().LdcI4(0).LdcI4(assignVariableLength << 2).Emit(OpCodes.Initblk);
            processor.StLoc(assignVariable);
            return assignVariable;
        }
    }
}
