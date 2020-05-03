// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Formatters;
using Utf8Json.Internal;
// ReSharper disable UseIndexFromEndExpression
// ReSharper disable ConvertIfStatementToNullCoalescingAssignment

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class DeserializeStaticHelper
    {
        public static void DeserializeStatic(in TypeAnalyzeResult analyzeResult, ILGenerator processor, Type returnType)
        {
            NullHandling(analyzeResult, processor);

            ref readonly var extensionDataInfo = ref analyzeResult.ExtensionData;
            var answerVariable = processor.DeclareLocal(returnType);
            LocalBuilder answerCallableVariable;
            if (analyzeResult.TargetType.IsValueType)
            {
                answerCallableVariable = processor.DeclareLocal(returnType.MakeByRefType());
                processor.LdLocAddress(answerVariable).StLoc(answerCallableVariable);
            }
            else
            {
                answerCallableVariable = answerVariable;
            }

            var readOnlyArguments = analyzeResult.ConstructorData.CanCreateInstanceBeforeDeserialization
                ? new DeserializeStaticReadOnlyArguments(answerVariable, answerCallableVariable, in analyzeResult, processor)
                : new DeserializeStaticReadOnlyArguments(answerVariable, answerCallableVariable, in analyzeResult, processor, ArrayPool<LocalBuilder>.Shared);
            try
            {
                if (readOnlyArguments.Dictionary.LengthVariations.IsEmpty)
                {
                    NoVariation(in readOnlyArguments);
                    return;
                }

                processor
                    .LdArg(0)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadIsBeginObjectWithVerify);
                var loopCountVariable = processor.DeclareLocal(typeof(int));
                var returnLabel = processor.DefineLabel();

                //var addMethodInfo = extensionDataInfo.AddMethodInfo;
                if (analyzeResult.ConstructorData.CanCreateInstanceBeforeDeserialization)
                {
                    NoConstructor(in readOnlyArguments, in extensionDataInfo, loopCountVariable, returnLabel);
                }
                else if (!(analyzeResult.ConstructorData.Constructor is null))
                {
                    WithConstructor(in readOnlyArguments, in extensionDataInfo, loopCountVariable, returnLabel, analyzeResult.ConstructorData.Constructor);
                }
                else
                {
                    Debug.Assert(!(analyzeResult.ConstructorData.FactoryMethod is null));
                    WithFactoryMethod(in readOnlyArguments, in extensionDataInfo, loopCountVariable, returnLabel, analyzeResult.ConstructorData.FactoryMethod);
                }

                processor.MarkLabel(returnLabel);

                CallCallbacks(analyzeResult.OnDeserialized, processor, answerVariable);

                processor
                    .LdLoc(answerVariable)
                    .Emit(OpCodes.Ret);
            }
            finally
            {
                readOnlyArguments.Dispose();
            }
        }

        private static void NullHandling(in TypeAnalyzeResult analyzeResult, ILGenerator processor)
        {
            var readIsNullNotNullLabel = processor.DefineLabel();
            processor
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadIsNull)
                .Emit(OpCodes.Brfalse_S, readIsNullNotNullLabel);

            if (analyzeResult.TargetType.IsValueType)
            {
                processor.ThrowException(typeof(NullReferenceException));
            }
            else
            {
                processor.LdNull().Emit(OpCodes.Ret);
            }

            processor.MarkLabel(readIsNullNotNullLabel);
        }

        private static void WithConstructor(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel, ConstructorInfo constructor)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;

            詰め込み作業(deserializeStaticReadOnlyArguments, extensionDataInfo, loopCountVariable, returnLabel, processor);
            var parameterInfos = constructor.GetParameters();
            Span<DeserializeDictionary.Entry.UnmanagedPart> correspondingEntrySpan = stackalloc DeserializeDictionary.Entry.UnmanagedPart[parameterInfos.Length];
            MatchConstructorAndLocateParameters(processor, in deserializeStaticReadOnlyArguments, parameterInfos, correspondingEntrySpan);
            processor.NewObj(constructor).StLoc(deserializeStaticReadOnlyArguments.AnswerVariable);

            CallCallbacks(deserializeStaticReadOnlyArguments.AnalyzeResult.OnDeserializing, processor, deserializeStaticReadOnlyArguments.AnswerCallableVariable);
            SetFromLocalVariables(in deserializeStaticReadOnlyArguments, correspondingEntrySpan);
        }

        private static void SetAssignFlag(ILGenerator processor, int uniqueIndex, LocalBuilder assignVariable)
        {
            var assignNumberIndex = uniqueIndex >> 5;
            var assignNumberBit = 1 << (uniqueIndex - (assignNumberIndex << 5));
            processor
                .LdLoc(assignVariable)
                .LdcI4(assignNumberIndex << 2)
                .Add()
                .Dup()
                .LdIndU4()
                .LdcI4(assignNumberBit)
                .Or()
                .StIndI4();
        }

        private static void DetectAssignFlag(ILGenerator processor, int uniqueIndex, LocalBuilder assignVariable, OpCode code, Label label)
        {
            var assignNumberIndex = uniqueIndex >> 5;
            var assignNumberBit = 1 << (uniqueIndex - (assignNumberIndex << 5));
            processor
                .LdLoc(assignVariable)
                .LdcI4(assignNumberIndex << 2)
                .Add()
                .LdIndU4()
                .LdcI4(assignNumberBit)
                .And()
                .Emit(code, label);
        }

        private static void WithFactoryMethod(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel, MethodInfo method)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;

            詰め込み作業(deserializeStaticReadOnlyArguments, extensionDataInfo, loopCountVariable, returnLabel, processor);
            var parameterInfos = method.GetParameters();
            Span<DeserializeDictionary.Entry.UnmanagedPart> correspondingEntrySpan = stackalloc DeserializeDictionary.Entry.UnmanagedPart[parameterInfos.Length];
            MatchConstructorAndLocateParameters(processor, in deserializeStaticReadOnlyArguments, parameterInfos, correspondingEntrySpan);
            processor.TryCallIfNotPossibleCallVirtual(method).StLoc(deserializeStaticReadOnlyArguments.AnswerVariable);

            CallCallbacks(deserializeStaticReadOnlyArguments.AnalyzeResult.OnDeserializing, processor, deserializeStaticReadOnlyArguments.AnswerCallableVariable);

            SetFromLocalVariables(in deserializeStaticReadOnlyArguments, correspondingEntrySpan);
        }

        private static void MatchConstructorAndLocateParameters(ILGenerator processor, in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ParameterInfo[] getParameters, Span<DeserializeDictionary.Entry.UnmanagedPart> correspondingEntrySpan)
        {
            var assignVariable = deserializeStaticReadOnlyArguments.AssignVariable;
            Debug.Assert(!(assignVariable is null));
            ref readonly var dictionary = ref deserializeStaticReadOnlyArguments.Dictionary;
            var notFilledLabel = processor.DefineLabel();
            for (var index = 0; index < getParameters.Length; index++)
            {
                ref var entryAnswer = ref correspondingEntrySpan[index];
                var parameterInfo = getParameters[index];
                var name = parameterInfo.Name;
                if (name is null)
                {
                    throw new ArgumentNullException();
                }

                FindMatchingParameterEntry(name, dictionary, out entryAnswer);
                DetectAssignFlag(processor, entryAnswer.UniqueIndex, assignVariable, OpCodes.Brfalse, notFilledLabel);
                processor.LdLoc(deserializeStaticReadOnlyArguments.ElementVariableSpan[entryAnswer.UniqueIndex]);
            }

            var successLabel = processor.DefineLabel();
            processor.Emit(OpCodes.Br_S, successLabel);
            processor.MarkLabel(notFilledLabel);
            processor.ThrowException(typeof(JsonParsingException));
            processor.MarkLabel(successLabel);
        }

        private static void FindMatchingParameterEntry(string name, in DeserializeDictionary dictionary, out DeserializeDictionary.Entry.UnmanagedPart entryAnswer)
        {
            var encodedNameByteLengthWithoutQuotation = NullableStringFormatter.CalcByteLength(name) - 2;
            Span<byte> encodedName = stackalloc byte[encodedNameByteLengthWithoutQuotation];
            NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, encodedName);
            var entries = dictionary[encodedNameByteLengthWithoutQuotation];
            foreach (ref readonly var entry in entries)
            {
                if (!PropertyNameHelper.SequenceEqualsIgnoreCase(entry.Key.Span, encodedName))
                {
                    continue;
                }

                entryAnswer = new DeserializeDictionary.Entry.UnmanagedPart(entry);
                break;
            }

            throw new ArgumentException();
        }

        private static void SetFromLocalVariables(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ReadOnlySpan<DeserializeDictionary.Entry.UnmanagedPart> alreadyUsedEntrySpan)
        {
            
        }

        private static void NoConstructor(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;

            if (!deserializeStaticReadOnlyArguments.AnalyzeResult.TargetType.IsValueType)
            {
                var defaultConstructor = deserializeStaticReadOnlyArguments.AnalyzeResult.TargetType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
                if (defaultConstructor is null)
                {
                    throw new InvalidOperationException(deserializeStaticReadOnlyArguments.AnalyzeResult.TargetType.FullName + " should have default constructor.");
                }

                processor.NewObj(defaultConstructor).StLoc(deserializeStaticReadOnlyArguments.AnswerVariable);
            }

            CallCallbacks(deserializeStaticReadOnlyArguments.AnalyzeResult.OnDeserializing, processor, deserializeStaticReadOnlyArguments.AnswerCallableVariable);

            詰め込み作業(deserializeStaticReadOnlyArguments, extensionDataInfo, loopCountVariable, returnLabel, processor);
        }

        private static void 詰め込み作業(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel, ILGenerator processor)
        {
            if (extensionDataInfo.Info is null)
            {
                LoopStartProcedure(processor, deserializeStaticReadOnlyArguments, loopCountVariable, returnLabel);
                var lengthVariations = deserializeStaticReadOnlyArguments.Dictionary.LengthVariations;
                Span<Label> destinations = stackalloc Label[lengthVariations.Length];
                var possibleLengthCount = 振り分けNoExtensionData(deserializeStaticReadOnlyArguments, lengthVariations, destinations);
                非default(deserializeStaticReadOnlyArguments, deserializeStaticReadOnlyArguments.Processor, destinations, possibleLengthCount);
            }
            else if (extensionDataInfo.Add)
            {
                var extensionVariable = processor.DeclareLocal(typeof(Dictionary<string, object>));
                Debug.Assert(!(extensionDataInfo.Info.GetMethod is null), "extensionDataInfo.Info != null");
                processor
                    .LdLoc(deserializeStaticReadOnlyArguments.AnswerCallableVariable)
                    .TryCallIfNotPossibleCallVirtual(extensionDataInfo.Info.GetMethod)
                    .StLoc(extensionVariable);

                LoopStartProcedure(processor, deserializeStaticReadOnlyArguments, loopCountVariable, returnLabel);
                var lengthVariations = deserializeStaticReadOnlyArguments.Dictionary.LengthVariations;
                Span<Label> destinations = stackalloc Label[lengthVariations.Length];
                var possibleLengthCount = 振り分けWithExtensionData(deserializeStaticReadOnlyArguments, extensionVariable, lengthVariations, destinations);
                非default(deserializeStaticReadOnlyArguments, deserializeStaticReadOnlyArguments.Processor, destinations, possibleLengthCount);
            }
            else
            {
                var extensionVariable = processor.DeclareLocal(typeof(Dictionary<string, object>));
                Debug.Assert(!(extensionDataInfo.Info.SetMethod is null), "extensionDataInfo.Info != null");
                processor
                    .LdLoc(deserializeStaticReadOnlyArguments.AnswerCallableVariable)
                    .NewObj(BasicInfoContainer.ConstructorInfoStringKeyObjectValueDictionary)
                    .Dup()
                    .StLoc(extensionVariable)
                    .TryCallIfNotPossibleCallVirtual(extensionDataInfo.Info.SetMethod);

                LoopStartProcedure(processor, deserializeStaticReadOnlyArguments, loopCountVariable, returnLabel);
                var lengthVariations = deserializeStaticReadOnlyArguments.Dictionary.LengthVariations;
                Span<Label> destinations = stackalloc Label[lengthVariations.Length];
                var possibleLengthCount = 振り分けWithExtensionData(deserializeStaticReadOnlyArguments, extensionVariable, lengthVariations, destinations);
                非default(deserializeStaticReadOnlyArguments, deserializeStaticReadOnlyArguments.Processor, destinations, possibleLengthCount);
            }
        }

        private static void LoopStartProcedure(ILGenerator processor, in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, LocalBuilder loopCountVariable, Label returnLabel)
        {
            // while(!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            processor.MarkLabel(deserializeStaticReadOnlyArguments.LoopStartLabel);
            processor
                .LdArg(0)
                .LdLocAddress(loopCountVariable)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadIsEndObjectWithSkipValueSeparator)
                .BrTrue(returnLabel); // if true goto return statement.

            // ReadOnlySpan<byte> name = reader.ReadPropertyNameSegmentRaw();
            // ref readonly byte b = ref name[0];
            // nameVariable.Length
            processor
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadPropertyNameSegmentRaw)
                .StLoc(deserializeStaticReadOnlyArguments.NameVariable) // var name = reader.ReadPropertyNameSegmentRaw();
                .LdLocAddress(deserializeStaticReadOnlyArguments.NameVariable)
                .LdcI4(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .StLoc(deserializeStaticReadOnlyArguments.ReferenceVariable)
                .LdLocAddress(deserializeStaticReadOnlyArguments.NameVariable)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodReadOnlySpanGetLength); // nameVariable.Length
        }

        private static void NoVariation(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;
            if (deserializeStaticReadOnlyArguments.AnalyzeResult.ExtensionData.Info is null)
            {
                processor
                    .LdArg(0)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadNextBlock);
            }
            else
            {
                var setMethod = deserializeStaticReadOnlyArguments.AnalyzeResult.ExtensionData.Info.SetMethod ?? throw new NullReferenceException();
                processor
                    .LdLoc(deserializeStaticReadOnlyArguments.AnswerCallableVariable)
                    .LdArg(0)
                    .LdArg(1)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringKeyObjectValueDictionaryFormatterDeserializeStatic)
                    .TryCallIfNotPossibleCallVirtual(setMethod);
            }

            if (deserializeStaticReadOnlyArguments.AnalyzeResult.TargetType.IsValueType)
            {
                processor.LdLoc(deserializeStaticReadOnlyArguments.AnswerVariable).Emit(OpCodes.Ret);
            }
            else
            {
                var defaultConstructor = deserializeStaticReadOnlyArguments.AnalyzeResult.TargetType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
                if (defaultConstructor is null)
                {
                    throw new InvalidOperationException(deserializeStaticReadOnlyArguments.AnalyzeResult.TargetType.FullName + " should have default constructor.");
                }

                processor.NewObj(defaultConstructor).Emit(OpCodes.Ret);
            }
        }

        private static void CallCallbacks(ReadOnlySpan<MethodInfo> methods, ILGenerator processor, LocalBuilder answerCallableVariable)
        {
            foreach (var methodInfo in methods)
            {
                if (methodInfo.IsStatic)
                {
                    processor.TryCallIfNotPossibleCallVirtual(methodInfo);
                }
                else
                {
                    processor
                        .LdLoc(answerCallableVariable)
                        .TryCallIfNotPossibleCallVirtual(methodInfo);
                }
            }
        }

        private static int 振り分けNoExtensionData(DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ReadOnlySpan<int> lengthVariations, Span<Label> destinations)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;
            for (var i = 0; i < destinations.Length; i++)
            {
                destinations[i] = processor.DefineLabel();
            }

            var possibleLengthCount = deserializeStaticReadOnlyArguments.Dictionary.Table.Length;
            EmbedSwitchLength(processor, lengthVariations, destinations, possibleLengthCount, deserializeStaticReadOnlyArguments.DefaultLabel);

            // default case
            processor.MarkLabel(deserializeStaticReadOnlyArguments.DefaultLabel);
            processor
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderSkipWhiteSpace) // reader.ReadSkipWhiteSpace();
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadNextBlock) // reader.ReadNextBlock();
                .Br(deserializeStaticReadOnlyArguments.LoopStartLabel); // continue;

            return possibleLengthCount;
        }

        private static int 振り分けWithExtensionData(DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, LocalBuilder extensionDataVariable, ReadOnlySpan<int> lengthVariations, Span<Label> destinations)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;
            for (var i = 0; i < destinations.Length; i++)
            {
                destinations[i] = processor.DefineLabel();
            }

            var possibleLengthCount = deserializeStaticReadOnlyArguments.Dictionary.Table.Length;
            EmbedSwitchLength(processor, lengthVariations, destinations, possibleLengthCount, deserializeStaticReadOnlyArguments.DefaultLabel);

            // default case
            processor.MarkLabel(deserializeStaticReadOnlyArguments.DefaultLabel);
            processor
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderSkipWhiteSpace) // reader.ReadSkipWhiteSpace();
                .LdLoc(extensionDataVariable)
                .LdLoc(deserializeStaticReadOnlyArguments.NameVariable)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodNullableStringDeserializeStaticInnerQuotation)
                .LdArg(0)
                .LdArg(1)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodObjectFormatterDeserializeStatic)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringKeyObjectValueDictionaryAdd)
                .Br(deserializeStaticReadOnlyArguments.LoopStartLabel); // continue;
            return possibleLengthCount;
        }

        private static void 非default(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ILGenerator processor, Span<Label> destinations, int possibleLengthCount)
        {
            int length = 0, labelIndex = 0;
            var entryArray = deserializeStaticReadOnlyArguments.Dictionary[length++];
            if (!entryArray.IsEmpty)
            {
                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                if (entryArray.Length != 1)
                {
                    throw new ArgumentOutOfRangeException(entryArray.Length.ToString(CultureInfo.InvariantCulture));
                }

                EmbedMatch(in entryArray[0], in deserializeStaticReadOnlyArguments);
            }

            var mutableArguments = new DeserializeStaticMutableArguments(ArrayPool<byte>.Shared);
            for (; length < 8; length++)
            {
                if (length >= possibleLengthCount)
                {
                    break;
                }

                entryArray = deserializeStaticReadOnlyArguments.Dictionary[length];
                if (entryArray.IsEmpty)
                {
                    continue;
                }

                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                mutableArguments.ClearWithRest(length);
                SwitchRestBytes(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments);
            }

            for (; length < possibleLengthCount; length++)
            {
                entryArray = deserializeStaticReadOnlyArguments.Dictionary[length];
                if (entryArray.IsEmpty)
                {
                    continue;
                }

                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                mutableArguments.ClearWithByteLength(length);
                長さ8以上に対する探索開始(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments);
            }
            mutableArguments.Dispose();
        }

        private static void EmbedSwitchLength(ILGenerator processor, ReadOnlySpan<int> lengthVariations, Span<Label> destinations, int possibleLengthCount, Label defaultLabel)
        {
            switch (lengthVariations.Length)
            {
                case 1:
                    processor
                        .LdcI4(lengthVariations[0])
                        .Beq(destinations[0]); // if (nameLength == maxLength)
                    break;
                case 2:
                    var nameLengthVariable = processor.DeclareLocal(typeof(int));
                    processor
                        .StLoc(nameLengthVariable)
                        .LdLoc(nameLengthVariable)
                        .LdcI4(lengthVariations[0])
                        .Beq(destinations[0]); // if (nameLength == minLength) goto MIN;
                    processor
                        .LdLoc(nameLengthVariable)
                        .LdcI4(lengthVariations[1])
                        .Beq(destinations[1]); // if (nameLength == maxLength) goto MAX;
                    break;
                default:
                    var cases = new Label[possibleLengthCount];
                    for (var j = 0; j < cases.Length; j++)
                    {
                        cases[j] = defaultLabel;
                    }

                    for (var index = 0; index < lengthVariations.Length; index++)
                    {
                        var lengthVariation = lengthVariations[index];
                        cases[lengthVariation] = destinations[index];
                    }

                    processor.Switch(cases); // switch (nameLength)
                    break;
            }
        }

        private static void 長さ8以上に対する探索開始(
            in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            ref DeserializeStaticMutableArguments mutableArguments
        )
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;
            processor
                .LdLoc(deserializeStaticReadOnlyArguments.ReferenceVariable)
                .LdIndI8();
            var position = mutableArguments.Position;
            var classCount = entryArray.CountUpVariation(position);

            if (classCount == 1)
            {
                var key = entryArray[0][position];
                processor.LdcI8(key).BneUn(deserializeStaticReadOnlyArguments.DefaultLabel);
                成功時の探索(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, new DeserializeDictionaryEntrySegment(key, 0, entryArray.Length));
                return;
            }

            ref var ulongVariable = ref mutableArguments.ULongVariable;
            if (ulongVariable is null)
            {
                ulongVariable = processor.DeclareLocal(typeof(ulong));
            }

            processor.StLoc(ulongVariable);

            var entrySegments = mutableArguments.Rent(entryArray, classCount, position);

            var middleIndex = entrySegments.Length >> 1;
            ref readonly var middleSegment = ref entrySegments[middleIndex];
            var notMiddleLabel = processor.DefineLabel();
            processor
                .LdLoc(ulongVariable)
                .LdcI8(middleSegment.Key)
                .BneUn(notMiddleLabel);

            成功時の探索(deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, middleSegment);

            processor.MarkLabel(notMiddleLabel);
            processor
                .LdLoc(ulongVariable);

            if (classCount == 2)
            {
                middleSegment = ref entrySegments[0];
                processor
                    .LdcI8(middleSegment.Key)
                    .BneUn(deserializeStaticReadOnlyArguments.DefaultLabel);
                成功時の探索(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, middleSegment);
            }
            else
            {
                var lesserLabel = processor.DefineLabel();
                processor
                    .LdcI8(middleSegment.Key)
                    .BltUn(lesserLabel);
                二分探索(in deserializeStaticReadOnlyArguments, ref mutableArguments, entryArray, entrySegments.Slice(middleIndex + 1), ulongVariable);
                processor.MarkLabel(lesserLabel);
                二分探索(in deserializeStaticReadOnlyArguments, ref mutableArguments, entryArray, entrySegments.Slice(0, middleIndex), ulongVariable);
            }
            mutableArguments.Return(classCount);
        }

        private static void 二分探索(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ref DeserializeStaticMutableArguments mutableArguments, ReadOnlySpan<DeserializeDictionary.Entry> entryArray, ReadOnlySpan<DeserializeDictionaryEntrySegment> segments, LocalBuilder ulongVariable)
        {
            while (true)
            {
                var processor = deserializeStaticReadOnlyArguments.Processor;
                processor.LdLoc(ulongVariable);

                if (segments.Length == 1)
                {
                    processor
                        .LdcI8(segments[0].Key)
                        .BneUn(deserializeStaticReadOnlyArguments.DefaultLabel);
                    成功時の探索(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, segments[0]);
                    return;
                }

                var middleIndex = segments.Length >> 1;
                ref readonly var middleSegment = ref segments[middleIndex];
                var notMiddleLabel = processor.DefineLabel();
                processor
                    .LdcI8(middleSegment.Key)
                    .BneUn(notMiddleLabel);

                成功時の探索(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, middleSegment);
                processor.MarkLabel(notMiddleLabel);
                processor.LdLoc(ulongVariable);

                if (segments.Length == 2)
                {
                    processor.LdcI8(segments[0].Key)
                        .BneUn(deserializeStaticReadOnlyArguments.DefaultLabel);

                    成功時の探索(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, segments[0]);
                    return;
                }

                var lesserLabel = processor.DefineLabel();
                processor.LdcI8(middleSegment.Key)
                    .BltUn(lesserLabel);
                二分探索(in deserializeStaticReadOnlyArguments, ref mutableArguments, entryArray, segments.Slice(middleIndex + 1), ulongVariable);
                processor.MarkLabel(lesserLabel);
                segments = segments.Slice(0, middleIndex);
            }
        }

        private static void 成功時の探索(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ReadOnlySpan<DeserializeDictionary.Entry> entryArray, ref DeserializeStaticMutableArguments mutableArguments, in DeserializeDictionaryEntrySegment middleSegment)
        {
            var position = mutableArguments.Position;
            if (position + 1 == mutableArguments.Length)
            {
                if (mutableArguments.Rest == 0)
                {
                    if (middleSegment.Length != 1)
                    {
                        throw new ArgumentOutOfRangeException(middleSegment.Length.ToString(CultureInfo.InvariantCulture));
                    }

                    EmbedMatch(entryArray[middleSegment.Offset], in deserializeStaticReadOnlyArguments);
                }
                else
                {
                    mutableArguments.Position = 0;
                    deserializeStaticReadOnlyArguments.Processor
                        .LdLoc(deserializeStaticReadOnlyArguments.ReferenceVariable)
                        .LdcI4(8)
                        .Add()
                        .StLoc(deserializeStaticReadOnlyArguments.ReferenceVariable);
                    SwitchRestBytes(in deserializeStaticReadOnlyArguments, entryArray.Slice(middleSegment.Offset, middleSegment.Length), ref mutableArguments);
                }
            }
            else
            {
                mutableArguments.Position = position + 1;
                deserializeStaticReadOnlyArguments.Processor
                    .LdLoc(deserializeStaticReadOnlyArguments.ReferenceVariable)
                    .LdcI4(8)
                    .Add()
                    .StLoc(deserializeStaticReadOnlyArguments.ReferenceVariable);
                長さ8以上に対する探索開始(in deserializeStaticReadOnlyArguments, entryArray.Slice(middleSegment.Offset, middleSegment.Length), ref mutableArguments);
            }
            mutableArguments.Position = position;
        }

        private static void SwitchRestBytes(
            in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            ref DeserializeStaticMutableArguments mutableArguments
        )
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;
            var (firstRestByte, notMatch) = SwitchRestBytesSetUp(processor, ref entryArray, deserializeStaticReadOnlyArguments.DefaultLabel, mutableArguments.Position, out var firstEntryArray);
            processor.LdLoc(deserializeStaticReadOnlyArguments.ReferenceVariable);
            if (mutableArguments.Position != 0)
            {
                processor
                    .LdcI4(mutableArguments.Position)
                    .Add();
            }

            processor.LdIndU1();

            // 他の候補がないならば
            if (entryArray.IsEmpty)
            {
                SwitchRestBytesTryMatchByteAndWhenMatchAction(in deserializeStaticReadOnlyArguments, firstRestByte, firstEntryArray, notMatch, ref mutableArguments);
                return;
            }

            ref var byteVariable = ref mutableArguments.ByteVariable;
            if (byteVariable is null)
            {
                byteVariable = processor.DeclareLocal(typeof(byte));
            }

            processor
                .StLoc(byteVariable)
                .LdLoc(byteVariable);

            // マッチしている場合の処理を記述
            SwitchRestBytesTryMatchByteAndWhenMatchAction(in deserializeStaticReadOnlyArguments, firstRestByte, firstEntryArray, notMatch, ref mutableArguments);

            // 他の候補を記述
            do
            {
                processor.MarkLabel(notMatch);
                (firstRestByte, notMatch) = SwitchRestBytesSetUp(processor, ref entryArray, deserializeStaticReadOnlyArguments.DefaultLabel, mutableArguments.Position, out firstEntryArray);
                processor.LdLoc(byteVariable);
                SwitchRestBytesTryMatchByteAndWhenMatchAction(in deserializeStaticReadOnlyArguments, firstRestByte, firstEntryArray, notMatch, ref mutableArguments);
            } while (!entryArray.IsEmpty);
        }

        private static (byte, Label) SwitchRestBytesSetUp(ILGenerator processor, ref ReadOnlySpan<DeserializeDictionary.Entry> entryArray, Label defaultLabel, int index, out ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray)
        {
            var (firstRestByte, firstRestSetCount) = entryArray.ClassifyByRest(index);
            firstEntryArray = entryArray.Slice(0, firstRestSetCount);
            entryArray = entryArray.Slice(firstRestSetCount);
            var notMatch = entryArray.IsEmpty ? defaultLabel : processor.DefineLabel();
            return (firstRestByte, notMatch);
        }

        private static void SwitchRestBytesTryMatchByteAndWhenMatchAction(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments,
            byte firstRestByte,
            ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
            Label notMatch,
            ref DeserializeStaticMutableArguments mutableArguments)
        {
            deserializeStaticReadOnlyArguments.Processor
                .LdcI4(firstRestByte)
                .BneUn(notMatch);

            var position = mutableArguments.Position;

            if (position + 1 == mutableArguments.Rest)
            {
                if (firstEntryArray.Length != 1)
                {
                    throw new ArgumentOutOfRangeException(firstEntryArray.Length.ToString(CultureInfo.InvariantCulture));
                }

                EmbedMatch(in firstEntryArray[0], in deserializeStaticReadOnlyArguments);
            }
            else
            {
                mutableArguments.Position++;
                SwitchRestBytes(in deserializeStaticReadOnlyArguments, firstEntryArray, ref mutableArguments);
            }

            mutableArguments.Position = position;
        }

        private static void EmbedMatch(in DeserializeDictionary.Entry entry, in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;
            ref readonly var analyzeResult = ref deserializeStaticReadOnlyArguments.AnalyzeResult;
            processor.LdLoc(deserializeStaticReadOnlyArguments.AnswerCallableVariable);

            EmbedMatchDeserializeStaticPart(entry.Type, entry.Index, analyzeResult, processor);

            if (deserializeStaticReadOnlyArguments.AssignVariable is null)
            {
                switch (entry.Type)
                {
                    case TypeAnalyzeResultMemberKind.FieldValueType:
                        {
                            ref readonly var info = ref analyzeResult.FieldValueTypeArray[entry.Index];
                            processor.StField(info.Info);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.PropertyValueType:
                        {
                            ref readonly var info = ref analyzeResult.PropertyValueTypeArray[entry.Index];
                            var setMethod = info.Info.SetMethod;
                            Debug.Assert(!(setMethod is null));
                            processor.TryCallIfNotPossibleCallVirtual(setMethod);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.FieldValueTypeShouldSerialize:
                        {
                            ref readonly var info = ref analyzeResult.FieldValueTypeShouldSerializeArray[entry.Index];
                            processor.StField(info.Info);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.PropertyValueTypeShouldSerialize:
                        {
                            ref readonly var info = ref analyzeResult.PropertyValueTypeShouldSerializeArray[entry.Index];
                            var setMethod = info.Info.SetMethod;
                            Debug.Assert(!(setMethod is null));
                            processor.TryCallIfNotPossibleCallVirtual(setMethod);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.FieldReferenceType:
                        {
                            ref readonly var info = ref analyzeResult.FieldReferenceTypeArray[entry.Index];
                            processor.StField(info.Info);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.PropertyReferenceType:
                        {
                            ref readonly var info = ref analyzeResult.PropertyReferenceTypeArray[entry.Index];
                            var setMethod = info.Info.SetMethod;
                            Debug.Assert(!(setMethod is null));
                            processor.TryCallIfNotPossibleCallVirtual(setMethod);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.FieldReferenceTypeShouldSerialize:
                        {
                            ref readonly var info = ref analyzeResult.FieldReferenceTypeShouldSerializeArray[entry.Index];
                            processor.StField(info.Info);
                        }
                        break;
                    case TypeAnalyzeResultMemberKind.PropertyReferenceTypeShouldSerialize:
                        {
                            ref readonly var info = ref analyzeResult.PropertyReferenceTypeShouldSerializeArray[entry.Index];
                            var setMethod = info.Info.SetMethod;
                            Debug.Assert(!(setMethod is null));
                            processor.TryCallIfNotPossibleCallVirtual(setMethod);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(TypeAnalyzeResultMemberKind), entry.Type, null);
                }
            }
            else
            {
                var elementVariable = deserializeStaticReadOnlyArguments.ElementVariableSpan[entry.UniqueIndex];
                processor.StLoc(elementVariable);
                SetAssignFlag(processor, entry.UniqueIndex, deserializeStaticReadOnlyArguments.AssignVariable);
            }

            processor.Br(deserializeStaticReadOnlyArguments.LoopStartLabel);
        }

        private static void EmbedMatchDeserializeStaticPart(TypeAnalyzeResultMemberKind entryType, int entryIndex, in TypeAnalyzeResult analyzeResult, ILGenerator processor)
        {
            var directTypeEnum = analyzeResult.GetDirectTypeEnum(entryType, entryIndex);
            switch (directTypeEnum)
            {
                case DirectTypeEnum.String:
                    var stringMethod = ReadWritePrimitive.MethodReadPrimitives[(int)DirectTypeEnum.String];
                    processor
                        .LdArg(0)
                        .TryCallIfNotPossibleCallVirtual(stringMethod);
                    if (analyzeResult.GetShouldIntern(entryType, entryIndex))
                    {
                        processor.TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringIntern);
                    }
                    break;
                case DirectTypeEnum.None:
                    var targetType = analyzeResult.GetTargetType(entryType, entryIndex);
                    var deserialize = BasicInfoContainer.DeserializeWithVerify(targetType);
                    processor
                        .LdArg(1)
                        .LdArg(0)
                        .TryCallIfNotPossibleCallVirtual(deserialize);
                    break;
                default:
                    var method = ReadWritePrimitive.MethodReadPrimitives[(int)directTypeEnum];
                    processor
                        .LdArg(0)
                        .TryCallIfNotPossibleCallVirtual(method);
                    break;
            }
        }
    }
}
