// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    internal readonly ref struct DeserializeStaticReadOnlyArguments
    {
        public readonly ILGenerator Processor;
        public readonly LocalBuilder AnswerVariable;
        public readonly LocalBuilder NameVariable;
        public readonly LocalBuilder ReferenceVariable;
        public readonly DeserializeDictionary Dictionary;

        public readonly Label LoopStartLabel;
        public readonly Label DefaultLabel;
        public readonly TypeAnalyzeResult AnalyzeResult;

        public DeserializeStaticReadOnlyArguments(LocalBuilder answerVariable, in TypeAnalyzeResult analyzeResult, ILGenerator processor)
        {
            Processor = processor;
            AnswerVariable = answerVariable;
            AnalyzeResult = analyzeResult;
            Dictionary = new DeserializeDictionary(in analyzeResult);
            NameVariable = processor.DeclareLocal(typeof(ReadOnlySpan<byte>));
            ReferenceVariable = processor.DeclareLocal(typeof(byte).MakeByRefType());
            LoopStartLabel = processor.DefineLabel();
            DefaultLabel = processor.DefineLabel();
        }

        public void Dispose()
        {
            Dictionary.Dispose();
        }
    }
}
