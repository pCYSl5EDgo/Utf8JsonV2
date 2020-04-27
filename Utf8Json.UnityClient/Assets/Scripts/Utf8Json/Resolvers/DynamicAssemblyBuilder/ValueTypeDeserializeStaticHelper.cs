// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;
// ReSharper disable UseIndexFromEndExpression
// ReSharper disable ConvertIfStatementToNullCoalescingAssignment

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class ValueTypeDeserializeStaticHelper
    {
        public static void DeserializeStatic(MethodBuilder deserializeStatic, in TypeAnalyzeResult analyzeResult)
        {
            var dictionary = new DeserializeDictionary(in analyzeResult);
            try
            {
                deserializeStatic.InitLocals = true;
                ref readonly var extensionDataInfo = ref analyzeResult.ExtensionData;
                var processor = deserializeStatic.GetILGenerator();
                var answerVariable = processor.DeclareLocal(deserializeStatic.ReturnType);

                var lengthVariationCount = dictionary.LengthVariationCount();

                if (lengthVariationCount == 0)
                {
                    NoVariation(processor, answerVariable, extensionDataInfo);

                    return;
                }

                processor
                    .LdArg(0)
                    .Call(BasicInfoContainer.MethodJsonReaderReadIsBeginObjectWithVerify);
                var loopCountVariable = processor.DeclareLocal(typeof(int));
                var loopStartLabel = processor.DefineLabel();
                var returnLabel = processor.DefineLabel();
                var nameVariable = processor.DeclareLocal(typeof(ReadOnlySpan<byte>));

                var addMethodInfo = extensionDataInfo.AddMethodInfo;
                if (analyzeResult.ConstructorData.CanCreateInstanceBeforeDeserialization)
                {
                    CallCallbacks(analyzeResult.OnDeserializing, processor, answerVariable);

                    // while(!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                    processor.MarkLabel(loopStartLabel);
                    processor
                        .LdArg(0)
                        .LdLocAddress(loopCountVariable)
                        .Call(BasicInfoContainer.MethodJsonReaderReadIsEndObjectWithSkipValueSeparator)
                        .BrTrueLong(returnLabel); // if true goto return statement.
                    processor
                        .LdArg(0)
                        .Call(BasicInfoContainer.MethodJsonReaderReadPropertyNameSegmentRaw)
                        .StLoc(nameVariable) // var name = reader.ReadPropertyNameSegmentRaw();
                        .LdLocAddress(nameVariable)
                        .Call(BasicInfoContainer.MethodReadOnlySpanGetLength); // nameVariable.Length

                    if (extensionDataInfo.Info is null)
                    {
                        DeserializeStatic_CreateInstanceBeforeDeserialization_NoExtensionData(processor, in analyzeResult, answerVariable, loopStartLabel, nameVariable, ref dictionary, lengthVariationCount);
                    }
                    else if (addMethodInfo is null)
                    {
                        DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData(processor, in analyzeResult, extensionDataInfo.Info, answerVariable, loopStartLabel, loopCountVariable, returnLabel, nameVariable, ref dictionary, lengthVariationCount);
                    }
                    else
                    {
                        AssertValidExtensionDataAddMethod(addMethodInfo);
                        DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData_Add(processor, in analyzeResult, extensionDataInfo.Info, addMethodInfo, answerVariable, loopStartLabel, loopCountVariable, returnLabel, nameVariable, ref dictionary, lengthVariationCount);
                    }

                    processor.BrLong(loopStartLabel); // goto while(!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                }
                else
                {
                    throw new NotSupportedException("Serialization Constructor is not supported.");
                    /*if (extensionDataInfo.Info is null)
                    {
                        DeserializeStatic_CreateInstanceAfterDeserialization_NoExtensionData(processor, in analyzeResult, answerVariable, loopStartLabel, loopCountVariable, returnLabel, nameVariable, ref dictionary, lengthVariationCount);
                    }
                    else if (addMethodInfo is null)
                    {
                        DeserializeStatic_CreateInstanceAfterDeserialization_WithExtensionData(processor, in analyzeResult, extensionDataInfo.Info, answerVariable, loopStartLabel, loopCountVariable, returnLabel, nameVariable, ref dictionary, lengthVariationCount);
                    }
                    else
                    {
                        AssertValidExtensionDataAddMethod(addMethodInfo);
                        DeserializeStatic_CreateInstanceAfterDeserialization_WithExtensionData_Add(processor, in analyzeResult, extensionDataInfo.Info, addMethodInfo, answerVariable, loopStartLabel, loopCountVariable, returnLabel, nameVariable, ref dictionary, lengthVariationCount);
                    }*/
                }

                processor.MarkLabel(returnLabel);

                CallCallbacks(analyzeResult.OnDeserialized, processor, answerVariable);

                processor
                    .LdLoc(answerVariable)
                    .Emit(OpCodes.Ret);
            }
            finally
            {
                dictionary.Dispose();
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

        [Conditional("DEBUG")]
        private static void AssertValidExtensionDataAddMethod(MethodInfo addMethodInfo)
        {
            var parameters = addMethodInfo.GetParameters();
            if (
                parameters[parameters.Length - 2].ParameterType != typeof(string)
                || parameters[parameters.Length - 1].ParameterType != typeof(object)
            )
            {
                throw new ArgumentException(addMethodInfo.Name);
            }

            if (addMethodInfo.IsStatic)
            {
                if (
                    parameters.Length != 3
                    || parameters[0].ParameterType != typeof(System.Collections.Generic.Dictionary<string, object>)
                )
                {
                    throw new ArgumentException(addMethodInfo.Name);
                }
            }
            else if (
                parameters.Length != 2
                || addMethodInfo.DeclaringType != typeof(System.Collections.Generic.Dictionary<string, object>)
            )
            {
                throw new ArgumentException(addMethodInfo.Name);
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

        /*private static void DeserializeStatic_CreateInstanceAfterDeserialization_WithExtensionData_Add(ILGenerator processor, in TypeAnalyzeResult analyzeResult, PropertyInfo info, MethodInfo addMethodInfo, LocalBuilder answerVariable, Label loopStartLabel, LocalBuilder loopCountVariable, Label returnLabel, LocalBuilder nameVariable, ref DeserializeDictionary dictionary, int lengthVariationCount)
        {
            throw new NotImplementedException();
        }

        private static void DeserializeStatic_CreateInstanceAfterDeserialization_WithExtensionData(ILGenerator processor, in TypeAnalyzeResult analyzeResult, PropertyInfo extensionDataInfo, LocalBuilder answerVariable, Label loopStartLabel, LocalBuilder loopCountVariable, Label returnLabel, LocalBuilder nameVariable, ref DeserializeDictionary dictionary, int lengthVariationCount)
        {
            throw new NotImplementedException();
        }

        private static void DeserializeStatic_CreateInstanceAfterDeserialization_NoExtensionData(ILGenerator processor, in TypeAnalyzeResult analyzeResult, LocalBuilder answerVariable, Label loopStartLabel, LocalBuilder loopCountVariable, Label returnLabel, LocalBuilder nameVariable, ref DeserializeDictionary dictionary, int lengthVariationCount)
        {
            throw new NotImplementedException();
        }*/

        private static void DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData_Add(ILGenerator processor, in TypeAnalyzeResult analyzeResult, PropertyInfo info, MethodInfo addMethodInfo, LocalBuilder answerVariable, Label loopStartLabel, LocalBuilder loopCountVariable, Label returnLabel, LocalBuilder nameVariable, ref DeserializeDictionary dictionary, int lengthVariationCount)
        {
            throw new NotImplementedException();
        }

        private static void DeserializeStatic_CreateInstanceBeforeDeserialization_WithExtensionData(ILGenerator processor, in TypeAnalyzeResult analyzeResult, PropertyInfo extensionDataInfo, LocalBuilder answerVariable, Label loopStartLabel, LocalBuilder loopCountVariable, Label returnLabel, LocalBuilder nameVariable, ref DeserializeDictionary dictionary, int lengthVariationCount)
        {
            throw new NotImplementedException();
        }

        private static void DeserializeStatic_CreateInstanceBeforeDeserialization_NoExtensionData(ILGenerator processor, in TypeAnalyzeResult analyzeResult, LocalBuilder answerVariable, Label loopStartLabel, LocalBuilder nameVariable, ref DeserializeDictionary dictionary, int lengthVariationCount)
        {
            Span<Label> destinations = stackalloc Label[lengthVariationCount];
            for (var i = 0; i < lengthVariationCount; i++)
            {
                destinations[i] = processor.DefineLabel();
            }

            var defaultLabel = processor.DefineLabel();
            EmbedSwitchLengthVariation(processor, dictionary, lengthVariationCount, destinations, defaultLabel);

            // default case
            processor
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonReaderSkipWhiteSpace) // reader.ReadSkipWhiteSpace();
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonReaderReadNextBlock) // reader.ReadNextBlock();
                .BrShort(loopStartLabel); // continue;

            int length = 0, labelIndex = 0;
            var entryArray = dictionary[length++];
            if (!entryArray.IsEmpty)
            {
                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                EmbedMatch(processor, answerVariable, entryArray[0].Type, entryArray[0].Index, analyzeResult);
            }

            var byteVariable = default(LocalBuilder);

            for (; length < 8; length++)
            {
                if (length >= dictionary.Table.Length)
                {
                    return;
                }

                entryArray = dictionary[length];
                if (entryArray.IsEmpty)
                {
                    continue;
                }

                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                SwitchRestBytes(processor, answerVariable, nameVariable, entryArray, defaultLabel, length, 0, analyzeResult, ref byteVariable);
            }

            var ulongVariable = default(LocalBuilder);
            for (; length < dictionary.Table.Length; length++)
            {
                entryArray = dictionary[length];
                if (entryArray.IsEmpty)
                {
                    continue;
                }

                processor.MarkLabel(destinations[labelIndex++]); // case (LENGTH):
                var rest = length - ((length >> 3) << 3);
                if (rest == 0)
                {
                    SwitchULongPartMultiple8(processor, nameVariable, entryArray, defaultLabel, length >> 3, 0, answerVariable, analyzeResult, ref ulongVariable);
                }
                else
                {
                    SwitchULongPartWithRest(processor, nameVariable, entryArray, defaultLabel, length >> 3, 0, rest, answerVariable, analyzeResult, ref ulongVariable, ref byteVariable);
                }
            }
        }

        private static void SwitchULongPartWithRest(
            ILGenerator processor,
            LocalBuilder nameVariable,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            Label defaultLabel,
            int length,
            int position,
            int rest,
            LocalBuilder answerVariable,
            in TypeAnalyzeResult analyzeResult,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ulongVariable,
            ref LocalBuilder? byteVariable)
#else
            ref LocalBuilder ulongVariable,
            ref LocalBuilder byteVariable)
#endif
        {
            var firstValue = SwitchULongPartSetUp(processor, ref entryArray, defaultLabel, position, out var firstEntryArray, out var notMatch);
            processor
                .LdLocAddress(nameVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .LdIndI8();
            if (!entryArray.IsEmpty)
            {
                if (ulongVariable is null)
                {
                    ulongVariable = processor.DeclareLocal(typeof(ulong));
                }

                processor.StLoc(ulongVariable).LdLoc(ulongVariable);
            }

            SwitchULongPartWithRestTryMatchByteAndWhenMatchAction(processor, answerVariable, nameVariable, defaultLabel, length, position, rest, firstValue, notMatch, firstEntryArray, analyzeResult, ref ulongVariable, ref byteVariable);
            while (!entryArray.IsEmpty)
            {
                processor.MarkLabel(notMatch);
                firstValue = SwitchULongPartSetUp(processor, ref entryArray, defaultLabel, position, out firstEntryArray, out notMatch);
#if CSHARP_8_OR_NEWER
                processor.LdLoc(ulongVariable!);
#else
                processor.LdLoc(ulongVariable);
#endif
                SwitchULongPartWithRestTryMatchByteAndWhenMatchAction(processor, answerVariable, nameVariable, defaultLabel, length, position, rest, firstValue, notMatch, firstEntryArray, analyzeResult, ref ulongVariable, ref byteVariable);
            }
        }

        private static void SwitchULongPartWithRestTryMatchByteAndWhenMatchAction(
            ILGenerator processor,
            LocalBuilder answerVariable,
            LocalBuilder nameVariable,
            Label defaultLabel,
            int length,
            int position,
            int rest,
            ulong firstValue,
            Label notMatch,
            ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
            in TypeAnalyzeResult analyzeResult,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ulongVariable,
            ref LocalBuilder? byteVariable)
#else
            ref LocalBuilder ulongVariable,
            ref LocalBuilder byteVariable)
#endif
        {
            processor
                .LdcI8(firstValue)
                .BrFalseLong(notMatch);

            processor
                .LdLocAddress(nameVariable)
                .LdcI4(8)
                .Call(BasicInfoContainer.MethodReadOnlySpanSlice)
                .StLoc(nameVariable);

            if (position + 1 == length)
            {
                SwitchRestBytes(processor, answerVariable, nameVariable, firstEntryArray, defaultLabel, rest, 0, analyzeResult, ref byteVariable);
            }
            else
            {
                SwitchULongPartMultiple8(processor, nameVariable, firstEntryArray, defaultLabel, length, position + 1, answerVariable, analyzeResult, ref ulongVariable);
            }
        }

        private static void SwitchULongPartMultiple8(
            ILGenerator processor,
            LocalBuilder nameVariable,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            Label defaultLabel,
            int length,
            int position,
            LocalBuilder answerVariable,
            in TypeAnalyzeResult analyzeResult,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ulongVariable)
#else
            ref LocalBuilder ulongVariable)
#endif
        {
            var firstValue = SwitchULongPartSetUp(processor, ref entryArray, defaultLabel, position, out var firstEntryArray, out var notMatch);
            processor
                .LdLocAddress(nameVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .LdIndI8();
            if (!entryArray.IsEmpty)
            {
                if (ulongVariable is null)
                {
                    ulongVariable = processor.DeclareLocal(typeof(ulong));
                }

                processor.StLoc(ulongVariable).LdLoc(ulongVariable);
            }

            SwitchULongPartMultiple8TryMatchByteAndWhenMatchAction(processor, answerVariable, nameVariable, defaultLabel, length, position, firstValue, notMatch, firstEntryArray, analyzeResult, ref ulongVariable);
            while (!entryArray.IsEmpty)
            {
                processor.MarkLabel(notMatch);
                firstValue = SwitchULongPartSetUp(processor, ref entryArray, defaultLabel, position, out firstEntryArray, out notMatch);
#if CSHARP_8_OR_NEWER
                processor.LdLoc(ulongVariable!);
#else
                processor.LdLoc(ulongVariable);
#endif
                SwitchULongPartMultiple8TryMatchByteAndWhenMatchAction(processor, answerVariable, nameVariable, defaultLabel, length, position, firstValue, notMatch, firstEntryArray, analyzeResult, ref ulongVariable);
            }
        }

        private static void SwitchULongPartMultiple8TryMatchByteAndWhenMatchAction(
            ILGenerator processor,
            LocalBuilder answerVariable,
            LocalBuilder nameVariable,
            Label defaultLabel,
            int length,
            int position,
            ulong firstValue,
            Label notMatch,
            ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
            in TypeAnalyzeResult analyzeResult,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ulongVariable)
#else
            ref LocalBuilder ulongVariable)
#endif
        {
            processor
                .LdcI8(firstValue)
                .BrFalseLong(notMatch);

            if (position + 1 == length)
            {
                EmbedMatch(processor, answerVariable, firstEntryArray[0].Type, firstEntryArray[0].Index, analyzeResult);
            }
            else
            {
                processor
                    .LdLocAddress(nameVariable)
                    .LdcI4(8)
                    .Call(BasicInfoContainer.MethodReadOnlySpanSlice)
                    .StLoc(nameVariable);
                SwitchULongPartMultiple8(processor, nameVariable, firstEntryArray, defaultLabel, length, position + 1, answerVariable, analyzeResult, ref ulongVariable);
            }
        }

        private static ulong SwitchULongPartSetUp(ILGenerator processor, ref ReadOnlySpan<DeserializeDictionary.Entry> entryArray, Label defaultLabel, int position, out ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray, out Label notMatch)
        {
            var (firstValue, count) = entryArray.Classify(position);
            firstEntryArray = entryArray.Slice(0, count);
            entryArray = entryArray.Slice(count);
            notMatch = entryArray.IsEmpty ? defaultLabel : processor.DefineLabel();
            return firstValue;
        }

        private static void SwitchRestBytes(
            ILGenerator processor,
            LocalBuilder answerVariable,
            LocalBuilder nameVariable,
            ReadOnlySpan<DeserializeDictionary.Entry> entryArray,
            Label defaultLabel,
            int restLength,
            int index,
            in TypeAnalyzeResult analyzeResult,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? byteVariable)
#else
            ref LocalBuilder byteVariable)
#endif
        {
            var firstRestByte = SwitchRestBytesSetUp(processor, ref entryArray, defaultLabel, index, out var firstEntryArray, out var notMatch);
            processor
                .LdLocAddress(nameVariable)
                .LdcI4(index)
                .Call(BasicInfoContainer.MethodReadOnlySpanGetItem)
                .LdIndU1();
            if (!entryArray.IsEmpty)
            {
                if (byteVariable is null)
                {
                    byteVariable = processor.DeclareLocal(typeof(bool));
                }

                processor
                    .StLoc(byteVariable)
                    .LdLoc(byteVariable);
            }

            SwitchRestBytesTryMatchByteAndWhenMatchAction(processor, answerVariable, nameVariable, defaultLabel, restLength, index, firstRestByte, notMatch, firstEntryArray, analyzeResult, ref byteVariable);

            while (!entryArray.IsEmpty)
            {
                processor.MarkLabel(notMatch);
                firstRestByte = SwitchRestBytesSetUp(processor, ref entryArray, defaultLabel, index, out firstEntryArray, out notMatch);
#if CSHARP_8_OR_NEWER
                processor.LdLoc(byteVariable!);
#else
                processor.LdLoc(byteVariable);
#endif
                SwitchRestBytesTryMatchByteAndWhenMatchAction(processor, answerVariable, nameVariable, defaultLabel, restLength, index, firstRestByte, notMatch, firstEntryArray, analyzeResult, ref byteVariable);
            }
        }

        private static byte SwitchRestBytesSetUp(ILGenerator processor, ref ReadOnlySpan<DeserializeDictionary.Entry> entryArray, Label defaultLabel, int index, out ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray, out Label notMatch)
        {
            byte firstRestByte;
            int firstRestSetCount;
            (firstRestByte, firstRestSetCount) = entryArray.ClassifyByRest(index);
            firstEntryArray = entryArray.Slice(0, firstRestSetCount);
            entryArray = entryArray.Slice(firstRestSetCount);
            notMatch = entryArray.IsEmpty ? defaultLabel : processor.DefineLabel();
            return firstRestByte;
        }

        private static void SwitchRestBytesTryMatchByteAndWhenMatchAction(
                ILGenerator processor,
                LocalBuilder answerVariable,
                LocalBuilder nameVariable,
                Label defaultLabel,
                int restLength,
                int index,
                byte firstRestByte,
                Label notMatch,
                ReadOnlySpan<DeserializeDictionary.Entry> firstEntryArray,
                in TypeAnalyzeResult analyzeResult,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? byteVariable)
#else
            ref LocalBuilder byteVariable)
#endif
        {
            processor
                .LdcI4(firstRestByte)
                .BrFalseLong(notMatch);

            if (index + 1 == restLength)
            {
                EmbedMatch(processor, answerVariable, firstEntryArray[0].Type, firstEntryArray[0].Index, analyzeResult);
            }
            else
            {
                SwitchRestBytes(processor, answerVariable, nameVariable, firstEntryArray, defaultLabel, restLength, index + 1, analyzeResult, ref byteVariable);
            }
        }

        private static void EmbedMatch(ILGenerator processor, LocalBuilder answerVariable, DeserializeDictionary.Type type, int index, in TypeAnalyzeResult analyzeResult)
        {
            processor.LdLocAddress(answerVariable);
            switch (type)
            {
                case DeserializeDictionary.Type.FieldValueType:
                    {
                        ref readonly var info = ref analyzeResult.FieldValueTypeArray[index];
                        ReadValueType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyValueType:
                    {
                        ref readonly var info = ref analyzeResult.PropertyValueTypeArray[index];
                        ReadValueType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldValueTypeShouldSerialize:
                    {
                        ref readonly var info = ref analyzeResult.FieldValueTypeShouldSerializeArray[index];
                        ReadValueType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyValueTypeShouldSerialize:
                    {
                        ref readonly var info = ref analyzeResult.PropertyValueTypeShouldSerializeArray[index];
                        ReadValueType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldReferenceType:
                    {
                        ref readonly var info = ref analyzeResult.FieldReferenceTypeArray[index];
                        ReadReferenceType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyReferenceType:
                    {
                        ref readonly var info = ref analyzeResult.PropertyReferenceTypeArray[index];
                        ReadReferenceType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                case DeserializeDictionary.Type.FieldReferenceTypeShouldSerialize:
                    {
                        ref readonly var info = ref analyzeResult.FieldReferenceTypeShouldSerializeArray[index];
                        ReadReferenceType(processor, info);
                        processor.StField(info.Info);
                    }
                    break;
                case DeserializeDictionary.Type.PropertyReferenceTypeShouldSerialize:
                    {
                        ref readonly var info = ref analyzeResult.PropertyReferenceTypeShouldSerializeArray[index];
                        ReadReferenceType(processor, info);
                        var setMethod = info.Info.SetMethod;
                        Debug.Assert(!(setMethod is null));
                        processor.Call(setMethod);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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

        private static void EmbedSwitchLengthVariation(ILGenerator processor, DeserializeDictionary dictionary, int lengthVariationCount, ReadOnlySpan<Label> destinations, Label defaultLabel)
        {
            switch (lengthVariationCount)
            {
                case 1:
                    processor
                        .LdcI4(dictionary.MaxLength)
                        .BeqShort(destinations[0]); // if (nameLength == maxLength)
                    break;
                case 2:
                    var nameLengthVariable = processor.DeclareLocal(typeof(int));
                    processor
                        .StLoc(nameLengthVariable)
                        .LdLoc(nameLengthVariable)
                        .LdcI4(dictionary.MinLength)
                        .BeqShort(destinations[0]); // if (nameLength == minLength) goto MIN;
                    processor
                        .LdLoc(nameLengthVariable)
                        .LdcI4(dictionary.MaxLength)
                        .BeqShort(destinations[1]); // if (nameLength == maxLength) goto MAX;
                    break;
                default:
                    var cases = new Label[dictionary.Table.Length];
                    for (int i = 0, j = 0; i < cases.Length; i++)
                    {
                        if (dictionary[i].IsEmpty)
                        {
                            cases[i] = defaultLabel;
                        }
                        else
                        {
                            cases[i] = destinations[j++];
                        }
                    }

                    processor.Switch(cases); // switch (nameLength)
                    break;
            }

            processor.MarkLabel(defaultLabel); // default:
        }
    }
}
