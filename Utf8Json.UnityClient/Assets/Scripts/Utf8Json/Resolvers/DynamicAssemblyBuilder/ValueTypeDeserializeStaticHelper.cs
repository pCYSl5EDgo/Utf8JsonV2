// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;
// ReSharper disable UseIndexFromEndExpression
// ReSharper disable ConvertIfStatementToNullCoalescingAssignment

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class ValueTypeDeserializeStaticHelper
    {
        public static void DeserializeStatic(in TypeAnalyzeResult analyzeResult, ILGenerator processor, Type returnType)
        {
            ref readonly var extensionDataInfo = ref analyzeResult.ExtensionData;
            var answerVariable = processor.DeclareLocal(returnType);
            var readOnlyArguments = new DeserializeStaticReadOnlyArguments(answerVariable, in analyzeResult, processor);
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
                else if (!(analyzeResult.ConstructorData.FactoryMethod is null))
                {
                    WithFactoryMethod(in readOnlyArguments, in extensionDataInfo, loopCountVariable, returnLabel, analyzeResult.ConstructorData.FactoryMethod);
                }
                else
                {
                    throw new InvalidOperationException();
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

        private static void WithFactoryMethod(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel, MethodInfo constructorDataFactoryMethod)
        {
            throw new NotImplementedException();
        }

        private static void WithConstructor(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel, ConstructorInfo constructorDataConstructor)
        {
            throw new NotImplementedException();
        }

        private static void NoConstructor(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, in ExtensionDataInfo extensionDataInfo, LocalBuilder loopCountVariable, Label returnLabel)
        {
            var processKind = extensionDataInfo.Info is null
                ? 0
                : extensionDataInfo.Add
                    ? 1
                    : 2;
            var processor = deserializeStaticReadOnlyArguments.Processor;
            CallCallbacks(deserializeStaticReadOnlyArguments.AnalyzeResult.OnDeserializing, processor, deserializeStaticReadOnlyArguments.AnswerVariable);

            switch (processKind)
            {
                case 0:
                    LoopStartProcedure(processor, deserializeStaticReadOnlyArguments, loopCountVariable, returnLabel);
                    DeserializeStatic_CreateInstanceBeforeDeserialization_NoExtensionData(in deserializeStaticReadOnlyArguments);
                    break;
                case 1:
                    {
                        var extensionVariable = processor.DeclareLocal(typeof(Dictionary<string, object>));
                        Debug.Assert(!(extensionDataInfo.Info?.GetMethod is null), "extensionDataInfo.Info != null");
                        processor
                            .LdLocAddress(deserializeStaticReadOnlyArguments.AnswerVariable)
                            .TryCallIfNotPossibleCallVirtual(extensionDataInfo.Info.GetMethod)
                            .StLoc(extensionVariable);

                        LoopStartProcedure(processor, deserializeStaticReadOnlyArguments, loopCountVariable, returnLabel);
                        DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData(in deserializeStaticReadOnlyArguments, extensionVariable);
                    }
                    break;
                case 2:
                    {
                        var extensionVariable = processor.DeclareLocal(typeof(Dictionary<string, object>));
                        Debug.Assert(!(extensionDataInfo.Info?.SetMethod is null), "extensionDataInfo.Info != null");
                        processor
                            .LdLocAddress(deserializeStaticReadOnlyArguments.AnswerVariable)
                            .NewObj(BasicInfoContainer.ConstructorInfoStringKeyObjectValueDictionary)
                            .Dup()
                            .StLoc(extensionVariable)
                            .TryCallIfNotPossibleCallVirtual(extensionDataInfo.Info.SetMethod);

                        LoopStartProcedure(processor, deserializeStaticReadOnlyArguments, loopCountVariable, returnLabel);
                        DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData(in deserializeStaticReadOnlyArguments, extensionVariable);
                    }
                    break;
            }

            // goto while(!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            processor.Br(deserializeStaticReadOnlyArguments.LoopStartLabel);
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
                var notNull = processor.DefineLabel();
                processor
                    .LdArg(0)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadIsNull)
                    .BrFalse(notNull);
                processor
                    .ThrowException(typeof(NullReferenceException));
                processor.MarkLabel(notNull);
                processor
                    .LdArg(0)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonReaderReadNextBlock)
                    .LdLoc(deserializeStaticReadOnlyArguments.AnswerVariable)
                    .Emit(OpCodes.Ret);
            }
            else
            {
                var setMethod = deserializeStaticReadOnlyArguments.AnalyzeResult.ExtensionData.Info.SetMethod ?? throw new NullReferenceException();
                processor
                    .LdLocAddress(deserializeStaticReadOnlyArguments.AnswerVariable)
                    .LdArg(0)
                    .LdArg(1)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringKeyObjectValueDictionaryFormatterDeserializeStatic)
                    .TryCallIfNotPossibleCallVirtual(setMethod)
                    .LdLoc(deserializeStaticReadOnlyArguments.AnswerVariable)
                    .Emit(OpCodes.Ret);
            }
        }

        private static void CallCallbacks(ReadOnlySpan<MethodInfo> methods, ILGenerator processor, LocalBuilder answerVariable)
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
                        .LdLocAddress(answerVariable)
                        .TryCallIfNotPossibleCallVirtual(methodInfo);
                }
            }
        }

        private static void DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, LocalBuilder extensionDataVariable)
        {
            var processor = deserializeStaticReadOnlyArguments.Processor;

            var lengthVariations = deserializeStaticReadOnlyArguments.Dictionary.LengthVariations;
            Span<Label> destinations = stackalloc Label[lengthVariations.Length];
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

            非default(deserializeStaticReadOnlyArguments, processor, destinations, possibleLengthCount);
        }

        private static void DeserializeStatic_CreateInstanceBeforeDeserialization_NoExtensionData(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments)
        {
            var lengthVariations = deserializeStaticReadOnlyArguments.Dictionary.LengthVariations;
            Span<Label> destinations = stackalloc Label[lengthVariations.Length];
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

            非default(deserializeStaticReadOnlyArguments, processor, destinations, possibleLengthCount);
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
                成功時の探索(in deserializeStaticReadOnlyArguments, entryArray, ref mutableArguments, new DeserializeDictionary.EntrySegment(key, 0, entryArray.Length));
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

        private static void 二分探索(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ref DeserializeStaticMutableArguments mutableArguments, ReadOnlySpan<DeserializeDictionary.Entry> entryArray, ReadOnlySpan<DeserializeDictionary.EntrySegment> segments, LocalBuilder ulongVariable)
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

        private static void 成功時の探索(in DeserializeStaticReadOnlyArguments deserializeStaticReadOnlyArguments, ReadOnlySpan<DeserializeDictionary.Entry> entryArray, ref DeserializeStaticMutableArguments mutableArguments, in DeserializeDictionary.EntrySegment middleSegment)
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
            processor.LdLocAddress(deserializeStaticReadOnlyArguments.AnswerVariable);
            switch (entry.Type)
            {
                case DeserializeDictionary.Type.FieldValueType:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.FieldValueTypeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                {
                                    var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                    processor
                                        .LdArg(1)
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(deserialize);
                                    break;
                                }
                            case DirectTypeEnum.String:
                                throw new ArgumentOutOfRangeException();
                            default:
                                {
                                    var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                    processor
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(method);
                                    break;
                                }
                        }
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyValueType:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.PropertyValueTypeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                {
                                    var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                    processor
                                        .LdArg(1)
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(deserialize);
                                    break;
                                }
                            case DirectTypeEnum.String:
                                throw new ArgumentOutOfRangeException();
                            default:
                                {
                                    var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                    processor
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(method);
                                    break;
                                }
                        }
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.TryCallIfNotPossibleCallVirtual(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldValueTypeShouldSerialize:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.FieldValueTypeShouldSerializeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                {
                                    var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                    processor
                                        .LdArg(1)
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(deserialize);
                                    break;
                                }
                            case DirectTypeEnum.String:
                                throw new ArgumentOutOfRangeException();
                            default:
                                {
                                    var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                    processor
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(method);
                                    break;
                                }
                        }
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyValueTypeShouldSerialize:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.PropertyValueTypeShouldSerializeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                {
                                    var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                    processor
                                        .LdArg(1)
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(deserialize);
                                    break;
                                }
                            case DirectTypeEnum.String:
                                throw new ArgumentOutOfRangeException();
                            default:
                                {
                                    var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                    processor
                                        .LdArg(0)
                                        .TryCallIfNotPossibleCallVirtual(method);
                                    break;
                                }
                        }
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.TryCallIfNotPossibleCallVirtual(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldReferenceType:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.FieldReferenceTypeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                processor
                                    .LdArg(1)
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(deserialize);
                                break;
                            case DirectTypeEnum.String:
                                var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                processor
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(method);
                                if (info.ShouldIntern)
                                {
                                    processor.TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringIntern);
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyReferenceType:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.PropertyReferenceTypeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                processor
                                    .LdArg(1)
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(deserialize);
                                break;
                            case DirectTypeEnum.String:
                                var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                processor
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(method);
                                if (info.ShouldIntern)
                                {
                                    processor.TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringIntern);
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.TryCallIfNotPossibleCallVirtual(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldReferenceTypeShouldSerialize:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.FieldReferenceTypeShouldSerializeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                processor
                                    .LdArg(1)
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(deserialize);
                                break;
                            case DirectTypeEnum.String:
                                var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                processor
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(method);
                                if (info.ShouldIntern)
                                {
                                    processor.TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringIntern);
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyReferenceTypeShouldSerialize:
                    {
                        ref readonly var info = ref deserializeStaticReadOnlyArguments.AnalyzeResult.PropertyReferenceTypeShouldSerializeArray[entry.Index];
                        switch (info.IsFormatterDirect)
                        {
                            case DirectTypeEnum.None:
                                var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                                processor
                                    .LdArg(1)
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(deserialize);
                                break;
                            case DirectTypeEnum.String:
                                var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                                processor
                                    .LdArg(0)
                                    .TryCallIfNotPossibleCallVirtual(method);
                                if (info.ShouldIntern)
                                {
                                    processor.TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodStringIntern);
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.TryCallIfNotPossibleCallVirtual(setMethod);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(DeserializeDictionary.Type), entry.Type, null);
            }

            processor.Br(deserializeStaticReadOnlyArguments.LoopStartLabel);
        }
    }
}
