// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        private readonly ref struct ReadOnlyArguments
        {
            public readonly ILGenerator Processor;
            public readonly LocalBuilder AnswerVariable;
            public readonly LocalBuilder NameVariable;
            public readonly DeserializeDictionary Dictionary;

            public readonly Label LoopStartLabel;
            public readonly Label DefaultLabel;
            public readonly TypeAnalyzeResult AnalyzeResult;

            public ReadOnlyArguments(LocalBuilder answerVariable, in TypeAnalyzeResult analyzeResult, ILGenerator processor)
            {
                Processor = processor;
                AnswerVariable = answerVariable;
                AnalyzeResult = analyzeResult;
                Dictionary = new DeserializeDictionary(in analyzeResult);
                NameVariable = processor.DeclareLocal(typeof(ReadOnlySpan<byte>));
                LoopStartLabel = processor.DefineLabel();
                DefaultLabel = processor.DefineLabel();
            }

            public void Dispose()
            {
                Dictionary.Dispose();
            }
        }

        private ref struct MutableArguments
        {
#if CSHARP_8_OR_NEWER
            public LocalBuilder? ByteVariable;
            public LocalBuilder? ULongVariable;
#else
            public LocalBuilder ByteVariable;
            public LocalBuilder ULongVariable;
#endif
            public int Length;
            public int Position;
            public int Rest;

            public void ClearWithRest(int rest)
            {
                Length = Position = 0;
                Rest = rest;
            }

            public void ClearWithByteLength(int length)
            {
                Length = length >> 3;
                Rest = length - (Length << 3);
                Position = 0;
            }
        }

        public static void DeserializeStatic(MethodBuilder deserializeStatic, in TypeAnalyzeResult analyzeResult)
        {
            deserializeStatic.InitLocals = true;
            ref readonly var extensionDataInfo = ref analyzeResult.ExtensionData;
            var processor = deserializeStatic.GetILGenerator();
            var answerVariable = processor.DeclareLocal(deserializeStatic.ReturnType);
            var arguments = new ReadOnlyArguments(answerVariable, in analyzeResult, processor);
            try
            {
                if (arguments.Dictionary.LengthVariations.IsEmpty)
                {
                    NoVariation(processor, answerVariable, extensionDataInfo);
                    return;
                }

                processor
                    .LdArg(0)
                    .Call(BasicInfoContainer.MethodJsonReaderReadIsBeginObjectWithVerify);
                var loopCountVariable = processor.DeclareLocal(typeof(int));
                var returnLabel = processor.DefineLabel();

                //var addMethodInfo = extensionDataInfo.AddMethodInfo;
                if (analyzeResult.ConstructorData.CanCreateInstanceBeforeDeserialization)
                {
                    CallCallbacks(analyzeResult.OnDeserializing, processor, answerVariable);

                    // while(!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                    processor.MarkLabel(arguments.LoopStartLabel);
                    processor
                        .LdArg(0)
                        .LdLocAddress(loopCountVariable)
                        .Call(BasicInfoContainer.MethodJsonReaderReadIsEndObjectWithSkipValueSeparator)
                        .BrTrueLong(returnLabel); // if true goto return statement.
                    processor
                        .LdArg(0)
                        .Call(BasicInfoContainer.MethodJsonReaderReadPropertyNameSegmentRaw)
                        .StLoc(arguments.NameVariable) // var name = reader.ReadPropertyNameSegmentRaw();
                        .LdLocAddress(arguments.NameVariable)
                        .Call(BasicInfoContainer.MethodReadOnlySpanGetLength); // nameVariable.Length

                    if (extensionDataInfo.Info is null)
                    {
                        DeserializeStatic_CreateInstanceBeforeDeserialization_NoExtensionData(in arguments);
                    }

                    processor.BrLong(arguments.LoopStartLabel); // goto while(!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                }
                else
                {
                    throw new NotSupportedException("Serialization Constructor is not supported.");
                }

                processor.MarkLabel(returnLabel);

                CallCallbacks(analyzeResult.OnDeserialized, processor, answerVariable);

                processor
                    .LdLoc(answerVariable)
                    .Emit(OpCodes.Ret);
            }
            finally
            {
                arguments.Dispose();
            }
        }

        private static void NoVariation(ILGenerator processor, LocalBuilder answerVariable, in ExtensionDataInfo extensionDataInfo)
        {
            if (extensionDataInfo.Info is null)
            {
                var notNull = processor.DefineLabel();
                processor
                    .LdArg(0)
                    .Call(BasicInfoContainer.MethodJsonReaderReadIsNull)
                    .BrFalseShort(notNull);
                processor
                    .ThrowException(typeof(NullReferenceException));
                processor.MarkLabel(notNull);
                processor
                    .LdArg(0)
                    .Call(BasicInfoContainer.MethodJsonReaderReadNextBlock)
                    .LdLoc(answerVariable)
                    .Emit(OpCodes.Ret);
            }
            else
            {
                var setMethod = extensionDataInfo.Info.SetMethod ?? throw new NullReferenceException();
                processor
                    .LdLocAddress(answerVariable)
                    .LdArg(0)
                    .LdArg(1)
                    .Call(BasicInfoContainer.MethodStringKeyObjectValueDictionaryFormatterDeserializeStatic)
                    .Call(setMethod)
                    .LdLoc(answerVariable)
                    .Emit(OpCodes.Ret);
            }
        }

        private static void CallCallbacks(ReadOnlySpan<MethodInfo> methods, ILGenerator processor, LocalBuilder answerVariable)
        {
            foreach (var methodInfo in methods)
            {
                if (methodInfo.IsStatic)
                {
                    processor.Call(methodInfo);
                }
                else
                {
                    processor
                        .LdLocAddress(answerVariable)
                        .Call(methodInfo);
                }
            }
        }

        private static void DeserializeStatic_CreateInstanceBeforeDeserialization_NoExtensionData(in ReadOnlyArguments readOnlyArguments)
        {
            var lengthVariations = readOnlyArguments.Dictionary.LengthVariations;
            Span<Label> destinations = stackalloc Label[lengthVariations.Length];
            var processor = readOnlyArguments.Processor;
            for (var i = 0; i < destinations.Length; i++)
            {
                destinations[i] = processor.DefineLabel();
            }

            var possibleLengthCount = readOnlyArguments.Dictionary.Table.Length;
            EmbedSwitchLength(processor, lengthVariations, destinations, possibleLengthCount, readOnlyArguments.DefaultLabel);

            processor.MarkLabel(readOnlyArguments.DefaultLabel); // default:

            // default case
            processor
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonReaderSkipWhiteSpace) // reader.ReadSkipWhiteSpace();
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonReaderReadNextBlock) // reader.ReadNextBlock();
                .BrShort(readOnlyArguments.LoopStartLabel); // continue;

            int length = 0, labelIndex = 0;
            var entryArray = readOnlyArguments.Dictionary[length++];
            if (!entryArray.IsEmpty)
            {
                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                if (entryArray.Length != 1)
                {
                    throw new ArgumentOutOfRangeException(entryArray.Length.ToString(CultureInfo.InvariantCulture));
                }

                EmbedMatch(in entryArray[0], in readOnlyArguments);
            }

            MutableArguments mutableArguments = default;
            for (; length < 8; length++)
            {
                if (length >= possibleLengthCount)
                {
                    return;
                }

                entryArray = readOnlyArguments.Dictionary[length];
                if (entryArray.IsEmpty)
                {
                    continue;
                }

                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                mutableArguments.ClearWithRest(length);
                SwitchRestBytes(in readOnlyArguments, entryArray, ref mutableArguments);
            }

            for (; length < possibleLengthCount; length++)
            {
                entryArray = readOnlyArguments.Dictionary[length];
                if (entryArray.IsEmpty)
                {
                    continue;
                }

                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                mutableArguments.ClearWithByteLength(length);
                if (mutableArguments.Rest == 0)
                {
                    SwitchULongPartMultiple8(in readOnlyArguments, entryArray, ref mutableArguments);
                }
                else
                {
                    SwitchULongPartWithRest(in readOnlyArguments, entryArray, ref mutableArguments);
                }
            }
        }

        private static void EmbedSwitchLength(ILGenerator processor, ReadOnlySpan<int> lengthVariations, Span<Label> destinations, int possibleLengthCount, Label defaultLabel)
        {
            switch (lengthVariations.Length)
            {
                case 1:
                    processor
                        .LdcI4(lengthVariations[0])
                        .BeqShort(destinations[0]); // if (nameLength == maxLength)
                    break;
                case 2:
                    var nameLengthVariable = processor.DeclareLocal(typeof(int));
                    processor
                        .StLoc(nameLengthVariable)
                        .LdLoc(nameLengthVariable)
                        .LdcI4(lengthVariations[0])
                        .BeqShort(destinations[0]); // if (nameLength == minLength) goto MIN;
                    processor
                        .LdLoc(nameLengthVariable)
                        .LdcI4(lengthVariations[1])
                        .BeqShort(destinations[1]); // if (nameLength == maxLength) goto MAX;
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

        private static void SwitchULongPartWithRest(
                in ReadOnlyArguments readOnlyArguments,
                ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
                ref MutableArguments mutableArguments)
        {
            var (firstValue, notMatch) = SwitchULongPartSetUp(in readOnlyArguments, entryArray, mutableArguments.Position, out var firstEntryArray);
            var processor = readOnlyArguments.Processor;
            processor
                .LdLocAddress(readOnlyArguments.NameVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .LdIndI8();
            if (!entryArray.IsEmpty)
            {
                if (mutableArguments.ULongVariable is null)
                {
                    mutableArguments.ULongVariable = processor.DeclareLocal(typeof(ulong));
                }

                processor.StLoc(mutableArguments.ULongVariable).LdLoc(mutableArguments.ULongVariable);
            }

            SwitchULongPartWithRestTryMatchByteAndWhenMatchAction(in readOnlyArguments, firstValue, notMatch, firstEntryArray, ref mutableArguments);
            while (!entryArray.IsEmpty)
            {
                processor.MarkLabel(notMatch);
                (firstValue, notMatch) = SwitchULongPartSetUp(in readOnlyArguments, entryArray, mutableArguments.Position, out firstEntryArray);
#if CSHARP_8_OR_NEWER
                processor.LdLoc(mutableArguments.ULongVariable!);
#else
                processor.LdLoc(mutableArguments.ULongVariable);
#endif
                SwitchULongPartWithRestTryMatchByteAndWhenMatchAction(in readOnlyArguments, firstValue, notMatch, firstEntryArray, ref mutableArguments);
            }
        }

        private static void SwitchULongPartWithRestTryMatchByteAndWhenMatchAction(
            in ReadOnlyArguments readOnlyArguments,
            ulong firstValue,
            Label notMatch,
            ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
            ref MutableArguments mutableArguments
        )
        {
            var processor = readOnlyArguments.Processor;
            processor
                .LdcI8(firstValue)
                .BneUn(notMatch);

            processor
                .LdLocAddress(readOnlyArguments.NameVariable)
                .LdcI4(8)
                .Call(BasicInfoContainer.MethodReadOnlySpanSlice)
                .StLoc(readOnlyArguments.NameVariable);

            var position = mutableArguments.Position;
            if (mutableArguments.Position + 1 == mutableArguments.Length)
            {
                mutableArguments.Position = 0;
                SwitchRestBytes(in readOnlyArguments, firstEntryArray, ref mutableArguments);
            }
            else
            {
                mutableArguments.Position++;
                SwitchULongPartMultiple8(in readOnlyArguments, firstEntryArray, ref mutableArguments);
            }

            mutableArguments.Position = position;
        }

        private static void SwitchULongPartMultiple8(
            in ReadOnlyArguments readOnlyArguments,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            ref MutableArguments mutableArguments
        )
        {
            var processor = readOnlyArguments.Processor;
            var pair = SwitchULongPartSetUp(in readOnlyArguments, entryArray, mutableArguments.Position, out var firstEntryArray);
            processor
                .LdLocAddress(readOnlyArguments.NameVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .LdIndI8();
            if (!entryArray.IsEmpty)
            {
                if (mutableArguments.ULongVariable is null)
                {
                    mutableArguments.ULongVariable = processor.DeclareLocal(typeof(ulong));
                }

                processor.StLoc(mutableArguments.ULongVariable).LdLoc(mutableArguments.ULongVariable);
            }

            SwitchULongPartMultiple8TryMatchByteAndWhenMatchAction(in readOnlyArguments, pair, firstEntryArray, ref mutableArguments);
            while (!entryArray.IsEmpty)
            {
                processor.MarkLabel(pair.Item2);
                pair = SwitchULongPartSetUp(in readOnlyArguments, entryArray, mutableArguments.Position, out firstEntryArray);
#if CSHARP_8_OR_NEWER
                processor.LdLoc(mutableArguments.ULongVariable!);
#else
                processor.LdLoc(mutableArguments.ULongVariable);
#endif
                SwitchULongPartMultiple8TryMatchByteAndWhenMatchAction(in readOnlyArguments, pair, firstEntryArray, ref mutableArguments);
            }
        }

        private static void SwitchULongPartMultiple8TryMatchByteAndWhenMatchAction(
            in ReadOnlyArguments readOnlyArguments,
            (ulong firstValue, Label notMatch) pair,
            ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
            ref MutableArguments mutableArguments
        )
        {
            var processor = readOnlyArguments.Processor;
            processor
                .LdcI8(pair.firstValue)
                .BneUn(pair.notMatch);

            var position = mutableArguments.Position;
            if (position + 1 == mutableArguments.Length)
            {
                if (firstEntryArray.Length != 1)
                {
                    throw new ArgumentOutOfRangeException(firstEntryArray.Length.ToString(CultureInfo.InvariantCulture));
                }

                EmbedMatch(in firstEntryArray[0], in readOnlyArguments);
            }
            else
            {
                mutableArguments.Position++;
                processor
                    .LdLocAddress(readOnlyArguments.NameVariable)
                    .LdcI4(8)
                    .Call(BasicInfoContainer.MethodReadOnlySpanSlice)
                    .StLoc(readOnlyArguments.NameVariable);
                SwitchULongPartMultiple8(in readOnlyArguments, firstEntryArray, ref mutableArguments);
            }

            mutableArguments.Position = position;
        }

        private static (ulong, Label) SwitchULongPartSetUp(
            in ReadOnlyArguments readOnlyArguments,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            int position,
            out ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray
        )
        {
            var (firstValue, count) = entryArray.Classify(position);
            firstEntryArray = entryArray.Slice(0, count);
            entryArray = entryArray.Slice(count);
            var notMatch = entryArray.IsEmpty ? readOnlyArguments.DefaultLabel : readOnlyArguments.Processor.DefineLabel();
            return (firstValue, notMatch);
        }

        private static void SwitchRestBytes(
            in ReadOnlyArguments readOnlyArguments,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            ref MutableArguments mutableArguments
        )
        {
            var processor = readOnlyArguments.Processor;
            var (firstRestByte, notMatch) = SwitchRestBytesSetUp(processor, ref entryArray, readOnlyArguments.DefaultLabel, mutableArguments.Position, out var firstEntryArray);
            processor
                .LdLocAddress(readOnlyArguments.NameVariable)
                .LdcI4(mutableArguments.Position)
                .Call(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .LdIndU1();

            // 他の候補がないならば
            if (entryArray.IsEmpty)
            {
                SwitchRestBytesTryMatchByteAndWhenMatchAction(in readOnlyArguments, firstRestByte, firstEntryArray, notMatch, ref mutableArguments);
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
            SwitchRestBytesTryMatchByteAndWhenMatchAction(in readOnlyArguments, firstRestByte, firstEntryArray, notMatch, ref mutableArguments);

            // 他の候補を記述
            do
            {
                processor.MarkLabel(notMatch);
                (firstRestByte, notMatch) = SwitchRestBytesSetUp(processor, ref entryArray, readOnlyArguments.DefaultLabel, mutableArguments.Position, out firstEntryArray);
                processor.LdLoc(byteVariable);
                SwitchRestBytesTryMatchByteAndWhenMatchAction(in readOnlyArguments, firstRestByte, firstEntryArray, notMatch, ref mutableArguments);
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

        private static void SwitchRestBytesTryMatchByteAndWhenMatchAction(in ReadOnlyArguments readOnlyArguments,
            byte firstRestByte,
            ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
            Label notMatch,
            ref MutableArguments mutableArguments)
        {
            readOnlyArguments.Processor
                .LdcI4(firstRestByte)
                .BneUn(notMatch);

            var position = mutableArguments.Position;

            if (position + 1 == mutableArguments.Rest)
            {
                if (firstEntryArray.Length != 1)
                {
                    throw new ArgumentOutOfRangeException(firstEntryArray.Length.ToString(CultureInfo.InvariantCulture));
                }

                EmbedMatch(in firstEntryArray[0], in readOnlyArguments);
            }
            else
            {
                mutableArguments.Position++;
                SwitchRestBytes(in readOnlyArguments, firstEntryArray, ref mutableArguments);
            }

            mutableArguments.Position = position;
        }

        private static void EmbedMatch(in DeserializeDictionary.Entry entry, in ReadOnlyArguments readOnlyArguments)
        {
            var processor = readOnlyArguments.Processor;
            processor.LdLocAddress(readOnlyArguments.AnswerVariable);
            switch (entry.Type)
            {
                case DeserializeDictionary.Type.FieldValueType:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.FieldValueTypeArray[entry.Index];
                        ReadValueType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyValueType:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.PropertyValueTypeArray[entry.Index];
                        ReadValueType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldValueTypeShouldSerialize:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.FieldValueTypeShouldSerializeArray[entry.Index];
                        ReadValueType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyValueTypeShouldSerialize:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.PropertyValueTypeShouldSerializeArray[entry.Index];
                        ReadValueType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldReferenceType:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.FieldReferenceTypeArray[entry.Index];
                        ReadReferenceType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyReferenceType:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.PropertyReferenceTypeArray[entry.Index];
                        ReadReferenceType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldReferenceTypeShouldSerialize:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.FieldReferenceTypeShouldSerializeArray[entry.Index];
                        ReadReferenceType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyReferenceTypeShouldSerialize:
                    {
                        ref readonly var info = ref readOnlyArguments.AnalyzeResult.PropertyReferenceTypeShouldSerializeArray[entry.Index];
                        ReadReferenceType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(DeserializeDictionary.Type), entry.Type, null);
            }

            processor.BrLong(readOnlyArguments.LoopStartLabel);
        }

        private static void ReadReferenceType<T>(ILGenerator processor, T info)
            where T : struct, IMemberContainer
        {
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.None:
                    var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                    processor
                        .LdArg(1)
                        .LdArg(0)
                        .Call(deserialize);
                    break;
                case DirectTypeEnum.String:
                    var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                    processor
                        .LdArg(0)
                        .Call(method);
                    if (info.ShouldIntern)
                    {
                        processor.Call(BasicInfoContainer.MethodStringIntern);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ReadValueType<T>(ILGenerator processor, T info)
            where T : struct, IMemberContainer
        {
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.None:
                    {
                        var deserialize = BasicInfoContainer.DeserializeWithVerify(info.TargetType);
                        processor
                            .LdArg(1)
                            .LdArg(0)
                            .Call(deserialize);
                        break;
                    }
                case DirectTypeEnum.String:
                    throw new ArgumentOutOfRangeException();
                default:
                    {
                        var method = ReadWritePrimitive.MethodReadPrimitives[(int)info.IsFormatterDirect];
                        processor
                            .LdArg(0)
                            .Call(method);
                        break;
                    }
            }
        }
    }
}
