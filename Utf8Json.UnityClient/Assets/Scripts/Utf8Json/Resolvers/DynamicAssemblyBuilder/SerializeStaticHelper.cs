// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Formatters;
using Utf8Json.Internal;
using Utf8Json.Internal.Reflection;
// ReSharper disable RedundantCaseLabel
// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class SerializeStaticHelper
    {
        public static void SerializeStatic(in TypeAnalyzeResult analyzeResult, ILGenerator processor, Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
        {
            var spanVariable = processor.DeclareLocal(typeof(Span<byte>));
            var bufferWriterAddressVariable = processor.DeclareLocal(typeof(BufferWriter).MakeByRefType());
            processor
                .LdArg(0)
                .LdFieldAddress(BasicInfoContainer.FieldJsonWriterWriter)
                .StLoc(bufferWriterAddressVariable);

            if (!analyzeResult.TargetType.IsValueType)
            {
                var notNullLabel = processor.DefineLabel();
                processor
                    .LdArg(1)
                    .Emit(OpCodes.Brtrue_S, notNullLabel);

                processor.WriteLiteral(bufferWriterAddressVariable, spanVariable, BasicInfoContainer.Null).Emit(OpCodes.Ret);

                processor.MarkLabel(notNullLabel);
            }


            if (analyzeResult.OnSerializing.Length != 0)
            {
                CallCallbacks(analyzeResult.OnSerializing, processor, loadValueArgumentAsCallableFunc);
            }

            var didNotBeginObjectDuringValueTypePeriod = TrySerializeStaticOfValueTypeFieldAndProperty(analyzeResult, processor, spanVariable, bufferWriterAddressVariable, loadValueArgumentAsCallableFunc);

            var detectIsFirstVariable = default(LocalBuilder);
            if (analyzeResult.FieldValueTypeShouldSerializeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ValueType_ShouldSerialize_DetectIsFirst(analyzeResult.FieldValueTypeShouldSerializeArray,
                        GetShouldSerializeFieldInfo,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        loadValueArgumentAsCallableFunc,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ValueType_ShouldSerialize_NotFirst(analyzeResult.FieldValueTypeShouldSerializeArray,
                        GetShouldSerializeFieldInfo,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        loadValueArgumentAsCallableFunc
                    );
                }
            }

            if (analyzeResult.PropertyValueTypeShouldSerializeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ValueType_ShouldSerialize_DetectIsFirst(analyzeResult.PropertyValueTypeShouldSerializeArray,
                        GetShouldSerializeGetMethod,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                        loadValueArgumentAsCallableFunc,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ValueType_ShouldSerialize_NotFirst(analyzeResult.PropertyValueTypeShouldSerializeArray,
                        GetShouldSerializeGetMethod,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                        loadValueArgumentAsCallableFunc
                    );
                }
            }

            var ignoreNullValuesVariable = default(LocalBuilder);
            var referenceVariable = default(LocalBuilder);
            if (analyzeResult.FieldReferenceTypeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ReferenceType_DetectIsFirst(analyzeResult.FieldReferenceTypeArray,
                        GetFieldInfo,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ReferenceType_NotFirst(analyzeResult.FieldReferenceTypeArray,
                        GetFieldInfo,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable
                    );
                }
            }

            if (analyzeResult.PropertyReferenceTypeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ReferenceType_DetectIsFirst(analyzeResult.PropertyReferenceTypeArray,
                        GetGetMethod,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ReferenceType_NotFirst(
                        analyzeResult.PropertyReferenceTypeArray,
                        GetGetMethod,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable
                    );
                }
            }

            if (analyzeResult.FieldReferenceTypeShouldSerializeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ReferenceType_ShouldSerialize_DetectIsFirst(analyzeResult.FieldReferenceTypeShouldSerializeArray,
                        GetShouldSerializeFieldInfo,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ReferenceType_ShouldSerialize_NotFirst(analyzeResult.FieldReferenceTypeShouldSerializeArray,
                        GetShouldSerializeFieldInfo,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable
                    );
                }
            }

            if (analyzeResult.PropertyReferenceTypeShouldSerializeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ReferenceType_ShouldSerialize_DetectIsFirst(analyzeResult.PropertyReferenceTypeShouldSerializeArray,
                        GetShouldSerializeGetMethod,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ReferenceType_ShouldSerialize_NotFirst(analyzeResult.PropertyReferenceTypeShouldSerializeArray,
                        GetShouldSerializeGetMethod,
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                        loadValueArgumentAsCallableFunc,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable
                    );
                }
            }

            WriteExtensionData(analyzeResult, processor, didNotBeginObjectDuringValueTypePeriod, loadValueArgumentAsCallableFunc, referenceVariable, ref detectIsFirstVariable);

            WriteBeginObjectIfNotWritten(detectIsFirstVariable, didNotBeginObjectDuringValueTypePeriod, processor);

            processor
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonWriterWriteEndObject);

            if (analyzeResult.OnSerialized.Length != 0)
            {
                CallCallbacks(analyzeResult.OnSerialized, processor, loadValueArgumentAsCallableFunc);
            }

            processor.Emit(OpCodes.Ret);
        }

        private static void WriteExtensionData(
                in TypeAnalyzeResult analyzeResult,
                ILGenerator processor,
                bool didNotBeginObjectDuringValueTypePeriod,
                Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc,
#if CSHARP_8_OR_NEWER
                LocalBuilder? referenceVariable,
                ref LocalBuilder? detectIsFirstVariable)
#else
                LocalBuilder referenceVariable,
                ref LocalBuilder detectIsFirstVariable)
#endif
        {
            var getMethod = analyzeResult.ExtensionData.Info?.GetMethod;
            if (getMethod is null)
            {
                return;
            }

            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (referenceVariable is null)
            {
                referenceVariable = processor.DeclareLocal(typeof(System.Collections.Generic.Dictionary<string, object>));
            }

            var skipLabel = processor.DefineLabel();
            loadValueArgumentAsCallableFunc(processor)
                .TryCallIfNotPossibleCallVirtual(getMethod)
                .Dup()
                .StLoc(referenceVariable)
                .BrFalse(skipLabel);

            processor
                .LdLoc(referenceVariable)
                .LdArg(0)
                .LdArg(2);
            if (didNotBeginObjectDuringValueTypePeriod)
            {
                if (detectIsFirstVariable is null)
                {
                    detectIsFirstVariable = DefineDetectIsFirstVariable(processor);
                    processor.LdcI4(1);
                }
                else
                {
                    processor.LdLoc(detectIsFirstVariable);
                }
            }
            else
            {
                processor.LdcI4(0);
            }

            processor.TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodSerializeExtensionData);
            if (detectIsFirstVariable is null)
            {
                processor.Pop();
            }
            else
            {
                processor.StLoc(detectIsFirstVariable);
            }

            processor.MarkLabel(skipLabel);
        }

        private static void WriteBeginObjectIfNotWritten(
#if CSHARP_8_OR_NEWER
            LocalBuilder? detectIsFirstVariable,
#else
            LocalBuilder detectIsFirstVariable,
#endif
            bool didNotBeginObjectDuringValueTypePeriod,
            ILGenerator processor)
        {
            if (detectIsFirstVariable is null)
            {
                if (!didNotBeginObjectDuringValueTypePeriod)
                {
                    return;
                }

                processor
                    .LdArg(0)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonWriterWriteBeginObject);
                return;
            }

            var returnLabel = processor.DefineLabel();
            processor
                .LdLoc(detectIsFirstVariable)
                .BrFalse(returnLabel);

            processor
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonWriterWriteBeginObject)
                .MarkLabel(returnLabel);
        }

        private static void SerializeStatic_ReferenceType_ShouldSerialize_NotFirst<TContainer, T>(
                TContainer[] members,
                Function<TContainer, T> func,
                ILGenerator processor,
                LocalBuilder spanVariable,
                LocalBuilder bufferWriterAddressVariable,
                Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
                Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ignoreNullValuesVariable,
            ref LocalBuilder? referenceVariable)
#else
                ref LocalBuilder ignoreNullValuesVariable,
                ref LocalBuilder referenceVariable)
#endif
            where TContainer : struct, IShouldSerializeMemberContainer
            where T : class
        {
            var objHelp = ObjectHelper.Object;
            foreach (ref var info in members.AsSpan())
            {
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                if (ignoreNullValuesVariable is null || referenceVariable is null)
                {
                    (ignoreNullValuesVariable, referenceVariable) = DefineIgnoreNullValuesVariable(processor);
                }

                var skipLabel = processor.DefineLabel();
                loadValueArgumentAsCallableFunc(processor)
                    .TryCallIfNotPossibleCallVirtual(info.ShouldSerialize)
                    .BrFalse(skipLabel);

                var doNotIgnoreNull = processor.DefineLabel();
                loadTargetByFunc(loadValueArgumentAsCallableFunc(processor), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalse(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalse(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeNotFirst(
                    processor,
                    spanVariable,
                    bufferWriterAddressVariable,
                    referenceVariable,
                    ref info,
                    objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedReferenceTypeNotFirst<TContainer>(
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            LocalBuilder referenceVariable,
            ref TContainer info,
            object objHelp)
            where TContainer : struct, IMemberContainer
        {
            EmbedPropertyNameNotBoolean(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, spanVariable, bufferWriterAddressVariable, false);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.String:
                    LoadTargetReferenceType(processor.LdArg(0), referenceVariable, objHelp)
                        .TryCallIfNotPossibleCallVirtual(ReadWritePrimitive.MethodWritePrimitives[(int)DirectTypeEnum.String]);
                    break;
                case DirectTypeEnum.None:
                    Embed_None(processor, ref info, referenceVariable, objHelp, LoadTargetReferenceType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SerializeStatic_ReferenceType_ShouldSerialize_DetectIsFirst<TContainer, T>(
                TContainer[] members,
                Function<TContainer, T> func,
                ILGenerator processor,
                LocalBuilder spanVariable,
                LocalBuilder bufferWriterAddressVariable,
                Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
                Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc,
#if CSHARP_8_OR_NEWER
                ref LocalBuilder? ignoreNullValuesVariable,
                ref LocalBuilder? referenceVariable,
                ref LocalBuilder? detectIsFirstVariable
#else
                ref LocalBuilder ignoreNullValuesVariable,
                ref LocalBuilder referenceVariable,
                ref LocalBuilder detectIsFirstVariable
#endif
        )
            where TContainer : struct, IShouldSerializeMemberContainer
            where T : class
        {
            var objHelp = ObjectHelper.Object;
            for (var i = 0; i < members.Length; i++)
            {
                ref var info = ref members[i];
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                if (ignoreNullValuesVariable is null || referenceVariable is null)
                {
                    (ignoreNullValuesVariable, referenceVariable) = DefineIgnoreNullValuesVariable(processor);
                }

                if (detectIsFirstVariable is null)
                {
                    detectIsFirstVariable = DefineDetectIsFirstVariable(processor);
                }

                var skipLabel = processor.DefineLabel();
                loadValueArgumentAsCallableFunc(processor)
                    .TryCallIfNotPossibleCallVirtual(info.ShouldSerialize)
                    .BrFalse(skipLabel);

                var doNotIgnoreNull = processor.DefineLabel();
                loadTargetByFunc(loadValueArgumentAsCallableFunc(processor), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalse(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalse(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeDetectFirst(
                    processor,
                    spanVariable,
                    bufferWriterAddressVariable,
                    referenceVariable,
                    detectIsFirstVariable,
                    ref info,
                    objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static bool TrySerializeStaticOfValueTypeFieldAndProperty(in TypeAnalyzeResult analyzeResult,
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
        {
            var didNotWriteBeginObject = true;
            if (analyzeResult.FieldValueTypeArray.Length != 0)
            {
                Func<ILGenerator, FieldInfo, ILGenerator> process = IntermediateLanguageGeneratorUtility.LdField;
                foreach (ref var info in analyzeResult.FieldValueTypeArray.AsSpan())
                {
                    EmbedEachInfo_ValueType(
                        processor,
                        spanVariable,
                        bufferWriterAddressVariable,
                        process,
                        ref info,
                        didNotWriteBeginObject,
                        info.Info,
                        loadValueArgumentAsCallableFunc
                    );
                    didNotWriteBeginObject = false;
                }
            }

            if (analyzeResult.PropertyValueTypeArray.Length != 0)
            {
                didNotWriteBeginObject = SerializeStatic_ValueType(analyzeResult.PropertyValueTypeArray,
                    GetGetMethod,
                    processor,
                    spanVariable,
                    bufferWriterAddressVariable,
                    didNotWriteBeginObject,
                    IntermediateLanguageGeneratorUtility.TryCallIfNotPossibleCallVirtual,
                    loadValueArgumentAsCallableFunc
                );
            }

            return didNotWriteBeginObject;
        }

        private static (LocalBuilder ignoreNullValuesVariable, LocalBuilder referenceVariable) DefineIgnoreNullValuesVariable(ILGenerator processor)
        {
            var ignoreNullValues = processor.DeclareLocal(typeof(bool));
            processor.LdArg(2).TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonSerializerOptionsIgnoreNullValues).StLoc(ignoreNullValues);
            var reference = processor.DeclareLocal(typeof(object));
            return (ignoreNullValues, reference);
        }

        private static LocalBuilder DefineDetectIsFirstVariable(ILGenerator processor)
        {
            var answer = processor.DeclareLocal(typeof(bool));
            processor.LdcI4(1).StLoc(answer);
            return answer;
        }

        private static void SerializeStatic_ReferenceType_DetectIsFirst<TContainer, T>(
                TContainer[] members,
                Function<TContainer, T> func,
                ILGenerator processor,
                LocalBuilder spanVariable,
                LocalBuilder bufferWriterAddressVariable,
                Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
                Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc,
#if CSHARP_8_OR_NEWER
                ref LocalBuilder? ignoreNullValuesVariable,
                ref LocalBuilder? referenceVariable,
                ref LocalBuilder? detectIsFirstVariable
#else
                ref LocalBuilder ignoreNullValuesVariable,
                ref LocalBuilder referenceVariable,
                ref LocalBuilder detectIsFirstVariable
#endif
        )
            where TContainer : struct, IMemberContainer
            where T : class
        {
            var objHelp = ObjectHelper.Object;
            for (var i = 0; i < members.Length; i++)
            {
                ref var info = ref members[i];
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                if (ignoreNullValuesVariable is null || referenceVariable is null)
                {
                    (ignoreNullValuesVariable, referenceVariable) = DefineIgnoreNullValuesVariable(processor);
                }

                if (detectIsFirstVariable is null)
                {
                    detectIsFirstVariable = DefineDetectIsFirstVariable(processor);
                }

                var skipLabel = processor.DefineLabel();
                var doNotIgnoreNull = processor.DefineLabel();
                loadTargetByFunc(loadValueArgumentAsCallableFunc(processor), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalse(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalse(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeDetectFirst(processor, spanVariable, bufferWriterAddressVariable, referenceVariable, detectIsFirstVariable, ref info, objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedReferenceTypeDetectFirst<TContainer>(
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            LocalBuilder referenceVariable,
            LocalBuilder detectIsFirstVariable,
            ref TContainer info,
            object objHelp)
            where TContainer : struct, IMemberContainer
        {
            EmbedPropertyNameNotBooleanDetectIsFirst(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, spanVariable, detectIsFirstVariable, bufferWriterAddressVariable);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.String:
                    LoadTargetReferenceType(processor.LdArg(0), referenceVariable, objHelp)
                        .TryCallIfNotPossibleCallVirtual(ReadWritePrimitive.MethodWritePrimitives[(int)DirectTypeEnum.String]);
                    break;
                case DirectTypeEnum.None:
                    Embed_None(processor, ref info, referenceVariable, objHelp, LoadTargetReferenceType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SerializeStatic_ReferenceType_NotFirst<TContainer, T>(
                TContainer[] members,
                Function<TContainer, T> func,
                ILGenerator processor,
                LocalBuilder spanVariable,
                LocalBuilder bufferWriterAddressVariable,
                Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
                Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc,
#if CSHARP_8_OR_NEWER
                ref LocalBuilder? ignoreNullValuesVariable,
                ref LocalBuilder? referenceVariable
#else
                ref LocalBuilder ignoreNullValuesVariable,
                ref LocalBuilder referenceVariable
#endif
        )
            where TContainer : struct, IMemberContainer
            where T : class
        {
            var objHelp = ObjectHelper.Object;
            for (var i = 0; i < members.Length; i++)
            {
                ref var info = ref members[i];
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                if (ignoreNullValuesVariable is null || referenceVariable is null)
                {
                    (ignoreNullValuesVariable, referenceVariable) = DefineIgnoreNullValuesVariable(processor);
                }

                var skipLabel = processor.DefineLabel();
                var doNotIgnoreNull = processor.DefineLabel();
                loadTargetByFunc(loadValueArgumentAsCallableFunc(processor), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalse(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalse(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeNotFirst(
                    processor,
                    spanVariable,
                    bufferWriterAddressVariable,
                    referenceVariable,
                    ref info,
                    objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void SerializeStatic_ValueType_ShouldSerialize_NotFirst<TContainer, T>(TContainer[] members,
            Function<TContainer, T> func,
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
            Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
            where TContainer : struct, IShouldSerializeMemberContainer
            where T : class
        {
            foreach (ref var info in members.AsSpan())
            {
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                var skipLabel = processor.DefineLabel();

                loadValueArgumentAsCallableFunc(processor).TryCallIfNotPossibleCallVirtual(info.ShouldSerialize).BrFalse(skipLabel);

                EmbedEachInfo_ValueType(processor, spanVariable, bufferWriterAddressVariable, loadTargetByFunc, ref info, false, t, loadValueArgumentAsCallableFunc);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedEachInfo_ValueType<TContainer, T>(ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
            ref TContainer info,
            bool isFirst,
            T t,
            Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
            where TContainer : struct, IMemberContainer
            where T : class
        {
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.Byte:
                case DirectTypeEnum.SByte:
                case DirectTypeEnum.UInt16:
                case DirectTypeEnum.Int16:
                case DirectTypeEnum.UInt32:
                case DirectTypeEnum.Int32:
                case DirectTypeEnum.UInt64:
                case DirectTypeEnum.Int64:
                case DirectTypeEnum.Single:
                case DirectTypeEnum.Double:
                case DirectTypeEnum.Char:
                    EmbedPropertyNameNotBoolean(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, spanVariable, bufferWriterAddressVariable, isFirst);
                    LoadTargetValueType(processor.LdArg(0), loadTargetByFunc, t, loadValueArgumentAsCallableFunc)
                        .TryCallIfNotPossibleCallVirtual(ReadWritePrimitive.MethodWritePrimitives[(int)info.IsFormatterDirect]);
                    break;
                case DirectTypeEnum.Boolean:
                    EmbedBoolean(processor, info.MemberName, t, spanVariable, bufferWriterAddressVariable, isFirst, loadTargetByFunc, loadValueArgumentAsCallableFunc);
                    break;
                case DirectTypeEnum.None:
                    EmbedPropertyNameNotBoolean(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, spanVariable, bufferWriterAddressVariable, isFirst);
                    Embed_None(processor, ref info, loadTargetByFunc, t, (generator, func, t1) => LoadTargetValueType(generator, func, t1, loadValueArgumentAsCallableFunc));
                    break;
                case DirectTypeEnum.String:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SerializeStatic_ValueType_ShouldSerialize_DetectIsFirst<TMemberContainer, T>(
                TMemberContainer[] members,
                Function<TMemberContainer, T> func,
                ILGenerator processor,
                LocalBuilder spanVariable,
                LocalBuilder bufferWriterAddressVariable,
                Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
                Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc,
#if CSHARP_8_OR_NEWER
                ref LocalBuilder? detectIsFirstVariable)
#else
                ref LocalBuilder detectIsFirstVariable)
#endif
            where TMemberContainer : struct, IShouldSerializeMemberContainer
            where T : class
        {
            foreach (ref var info in members.AsSpan())
            {
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                if (detectIsFirstVariable is null)
                {
                    detectIsFirstVariable = DefineDetectIsFirstVariable(processor);
                }

                var skipLabel = processor.DefineLabel();

                loadValueArgumentAsCallableFunc(processor)
                    .TryCallIfNotPossibleCallVirtual(info.ShouldSerialize)
                    .BrFalse(skipLabel);

                switch (info.IsFormatterDirect)
                {
                    case DirectTypeEnum.Byte:
                    case DirectTypeEnum.SByte:
                    case DirectTypeEnum.UInt16:
                    case DirectTypeEnum.Int16:
                    case DirectTypeEnum.UInt32:
                    case DirectTypeEnum.Int32:
                    case DirectTypeEnum.UInt64:
                    case DirectTypeEnum.Int64:
                    case DirectTypeEnum.Single:
                    case DirectTypeEnum.Double:
                    case DirectTypeEnum.Char:
                        EmbedPropertyNameNotBooleanDetectIsFirst(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, spanVariable, detectIsFirstVariable, bufferWriterAddressVariable);
                        loadTargetByFunc(loadValueArgumentAsCallableFunc(processor.LdArg(0)), t)
                            .TryCallIfNotPossibleCallVirtual(ReadWritePrimitive.MethodWritePrimitives[(int)info.IsFormatterDirect]);
                        break;
                    case DirectTypeEnum.Boolean:
                        EmbedBoolean_DetectIsFirst(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, t, spanVariable, loadTargetByFunc, detectIsFirstVariable, bufferWriterAddressVariable, skipLabel, loadValueArgumentAsCallableFunc);
                        break;
                    case DirectTypeEnum.None:
                        EmbedPropertyNameNotBooleanDetectIsFirst(processor, info.MemberName, info.MemberNameByteLengthWithQuotation, spanVariable, detectIsFirstVariable, bufferWriterAddressVariable);
                        // ReSharper disable once RedundantTypeArgumentsOfMethod
                        Embed_None(processor, ref info, loadTargetByFunc, t, (generator, func1, t1) => LoadTargetValueType(generator, func1, t1, loadValueArgumentAsCallableFunc));
                        break;
                    case DirectTypeEnum.String:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedBoolean_DetectIsFirst<T>(ILGenerator processor,
            string memberName,
            int memberNameByteLengthWithQuotation,
            T t,
            LocalBuilder spanVariable,
            Func<ILGenerator, T, ILGenerator> process,
            LocalBuilder detectIsFirstVariable,
            LocalBuilder bufferWriterAddressVariable,
            Label endLabel,
            Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc
        )
            where T : class
        {
            Span<byte> name = stackalloc byte[memberNameByteLengthWithQuotation + 7];
            name[0] = (byte)',';
            name[name.Length - 6] = (byte)':';
            name[name.Length - 5] = (byte)'f';
            name[name.Length - 4] = (byte)'a';
            name[name.Length - 3] = (byte)'l';
            name[name.Length - 2] = (byte)'s';
            name[name.Length - 1] = (byte)'e';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameByteLengthWithQuotation));

            var writeTrueLabel = processor.DefineLabel();
            var whenFirstTime = processor.DefineLabel();
            processor
                .LdLoc(detectIsFirstVariable)
                .BrTrue(whenFirstTime);

            {
                var trueLabel = processor.DefineLabel();
                process(loadValueArgumentAsCallableFunc(processor), t).BrTrue(trueLabel);

                processor.Copy(name, spanVariable, bufferWriterAddressVariable).Br(endLabel);

                processor.MarkLabel(trueLabel);
                processor.Copy(name.Slice(0, memberNameByteLengthWithQuotation + 2), spanVariable, bufferWriterAddressVariable).Br(writeTrueLabel);
            }

            processor.MarkLabel(whenFirstTime);
            {
                processor
                    .LdcI4(0)
                    .StLoc(detectIsFirstVariable)
                    .LdArg(0)
                    .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonWriterWriteBeginObject);
                var trueLabel = processor.DefineLabel();
                process(loadValueArgumentAsCallableFunc(processor), t).BrTrue(trueLabel);
                processor.Copy(name.Slice(1), spanVariable, bufferWriterAddressVariable).Br(endLabel);

                processor.MarkLabel(trueLabel);
                processor.Copy(name.Slice(1, memberNameByteLengthWithQuotation + 1), spanVariable, bufferWriterAddressVariable);
            }

            processor.MarkLabel(writeTrueLabel);
            processor.WriteLiteral(bufferWriterAddressVariable, spanVariable, BasicInfoContainer.True);
        }

        private static void EmbedPropertyNameNotBooleanDetectIsFirst(
            ILGenerator processor,
            string memberName,
            int memberNameByteLengthWithQuotation,
            LocalBuilder spanVariable,
            LocalBuilder detectIsFirstVariable,
            LocalBuilder bufferWriterAddressVariable)
        {
            Span<byte> name = stackalloc byte[memberNameByteLengthWithQuotation + 2];
            name[0] = (byte)',';
            name[name.Length - 1] = (byte)':';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameByteLengthWithQuotation));

            var whenFirstTime = processor.DefineLabel();
            processor
                .LdLoc(detectIsFirstVariable)
                .BrTrue(whenFirstTime);

            var endLabel = processor.DefineLabel();
            processor
                .Copy(name, spanVariable, bufferWriterAddressVariable)
                .Br(endLabel);

            processor.MarkLabel(whenFirstTime);
            processor
                .LdcI4(0)
                .StLoc(detectIsFirstVariable)
                .LdArg(0)
                .TryCallIfNotPossibleCallVirtual(BasicInfoContainer.MethodJsonWriterWriteBeginObject)
                .Copy(name.Slice(1, memberNameByteLengthWithQuotation + 1), spanVariable, bufferWriterAddressVariable)
                .MarkLabel(endLabel);
        }

        private static void CallCallbacks(ReadOnlySpan<MethodInfo> methods, ILGenerator processor, Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
        {
            foreach (var methodInfo in methods)
            {
                if (methodInfo.IsStatic)
                {
                    processor.TryCallIfNotPossibleCallVirtual(methodInfo);
                }
                else
                {
                    loadValueArgumentAsCallableFunc(processor).TryCallIfNotPossibleCallVirtual(methodInfo);
                }
            }
        }

#if CSHARP_8_OR_NEWER
        private delegate T? Function<TMemberInfo, out T>(in TMemberInfo info) where TMemberInfo : struct where T : class;
        private static MethodInfo? GetGetMethod(in PropertySerializationInfo info) => info.Info.GetMethod;
        private static MethodInfo? GetShouldSerializeGetMethod(in ShouldSerializePropertySerializationInfo info) => info.Info.GetMethod;
#else
        private delegate T Function<TMemberInfo, out T>(in TMemberInfo info) where TMemberInfo : struct where T : class;
        private static MethodInfo GetGetMethod(in PropertySerializationInfo info) => info.Info.GetMethod;
        private static MethodInfo GetShouldSerializeGetMethod(in ShouldSerializePropertySerializationInfo info) => info.Info.GetMethod;
#endif

        private static FieldInfo GetFieldInfo(in FieldSerializationInfo info) => info.Info;
        private static FieldInfo GetShouldSerializeFieldInfo(in ShouldSerializeFieldSerializationInfo info) => info.Info;

        private static bool SerializeStatic_ValueType<TContainer, T>(TContainer[] members,
            Function<TContainer, T> func,
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            bool isFirst,
            Func<ILGenerator, T, ILGenerator> process,
            Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
            where TContainer : struct, IMemberContainer
            where T : class
        {
            foreach (ref var info in members.AsSpan())
            {
                var t = func(info);
                if (t is null)
                {
                    continue;
                }

                EmbedEachInfo_ValueType(processor, spanVariable, bufferWriterAddressVariable, process, ref info, isFirst, t, loadValueArgumentAsCallableFunc);

                isFirst = false;
            }

            return isFirst;
        }

        private static ILGenerator LoadTargetReferenceType(ILGenerator processor, LocalBuilder referenceVariable, object _)
        {
            return processor.LdLoc(referenceVariable);
        }

        private static void Embed_None<TContainer, T0, T1>(
            ILGenerator processor,
            ref TContainer info,
            T0 t0,
            T1 t1,
            Func<ILGenerator, T0, T1, ILGenerator> loadTarget)
            where TContainer : struct, IMemberContainer
            where T0 : class
            where T1 : class
        {
            var jsonFormatterAttribute = info.FormatterInfo;
            if (jsonFormatterAttribute is null) // get formatter
            {
                var serialize = Type.GetType(BuilderSet.CreateFormatterName(info.TargetType))?.GetMethod("SerializeStatic", BindingFlags.Public | BindingFlags.Static);
                if (serialize is null)
                {
                    loadTarget(processor.LdArg(2).LdArg(0), t0, t1).TryCallIfNotPossibleCallVirtual(BasicInfoContainer.SerializeWithVerify(info.TargetType));
                }
                else
                {
                    loadTarget(processor.LdArg(0), t0, t1).LdArg(2).TryCallIfNotPossibleCallVirtual(serialize);
                }
                return;
            }

            var jsonFormatterType = jsonFormatterAttribute.FormatterType;
            var arguments = jsonFormatterAttribute.Arguments;
            var interfaceMethodSerialize = typeof(IJsonFormatter<>).MakeGeneric(info.TargetType).GetMethodInstance("Serialize");
            if (arguments is null)
            {
                var length3 = TypeArrayHolder.Length3;
                length3[0] = typeof(JsonWriter).MakeByRefType();
                length3[1] = info.TargetType;
                length3[2] = typeof(JsonSerializerOptions);
                var serialize = jsonFormatterType.GetMethod("SerializeStatic", BindingFlags.Public | BindingFlags.Static, null, length3, null);
                if (!(serialize is null))
                {
                    loadTarget(processor.LdArg(0), t0, t1).LdArg(2).TryCallIfNotPossibleCallVirtual(serialize);
                    return;
                }

                var field = jsonFormatterType.GetField("Instance", BindingFlags.Static | BindingFlags.Public)
                    ?? jsonFormatterType.GetField("Default", BindingFlags.Static | BindingFlags.Public);

                if (!(field is null))
                {
                    loadTarget(processor.LdStaticField(field).LdArg(0), t0, t1).LdArg(2).ConstrainedCallVirtual(jsonFormatterType, interfaceMethodSerialize);
                    return;
                }

                var propertyGetMethod = jsonFormatterType.GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public, null, Array.Empty<Type>(), null)
                    ?? jsonFormatterType.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public, null, Array.Empty<Type>(), null);

                if (!(propertyGetMethod is null))
                {
                    loadTarget(processor.TryCallIfNotPossibleCallVirtual(propertyGetMethod).LdArg(0), t0, t1).LdArg(2).ConstrainedCallVirtual(jsonFormatterType, interfaceMethodSerialize);
                    return;
                }
            }

            var constructorTypes = arguments is null ? Array.Empty<Type>() : arguments.Length == 1 ? TypeArrayHolder.Length1 : arguments.Length == 2 ? TypeArrayHolder.Length2 : arguments.Length == 3 ? TypeArrayHolder.Length3 : new Type[arguments.Length];
            FillConstructorTypesAndEmbedValues(processor, arguments ?? Array.Empty<object>(), constructorTypes);

            var jsonFormatterDefaultConstructor = jsonFormatterType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, constructorTypes, null);
            Debug.Assert(!(jsonFormatterDefaultConstructor is null));
            loadTarget(processor.NewObj(jsonFormatterDefaultConstructor).LdArg(0), t0, t1)
                .LdArg(2)
                .ConstrainedCallVirtual(jsonFormatterType, interfaceMethodSerialize);
        }

        private static ILGenerator LoadTargetValueType<T>(ILGenerator generator, Func<ILGenerator, T, ILGenerator> func, T t, Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc)
            where T : class
        {
            return func(loadValueArgumentAsCallableFunc(generator), t);
        }

        private static void EmbedBoolean<T>(ILGenerator processor,
            string memberName,
            T t,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            bool isFirst,
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
            Func<ILGenerator, ILGenerator> loadValueArgumentAsCallableFunc
        )
            where T : class
        {
            var memberNameLength = NullableStringFormatter.CalcByteLength(memberName);
            Span<byte> name = stackalloc byte[memberNameLength + 7];
            name[0] = (byte)(isFirst ? '{' : ',');
            name[name.Length - 6] = (byte)':';
            name[name.Length - 5] = (byte)'f';
            name[name.Length - 4] = (byte)'a';
            name[name.Length - 3] = (byte)'l';
            name[name.Length - 2] = (byte)'s';
            name[name.Length - 1] = (byte)'e';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameLength));

            var trueLabel = processor.DefineLabel();
            var endLabel = processor.DefineLabel();

            loadTargetByFunc(loadValueArgumentAsCallableFunc(processor), t).Emit(OpCodes.Brtrue_S, trueLabel);

            processor
                .Copy(name, spanVariable, bufferWriterAddressVariable)
                .Br(endLabel);
            processor.MarkLabel(trueLabel);
            processor
                .Copy(name.Slice(0, memberNameLength + 2), spanVariable, bufferWriterAddressVariable)
                .WriteLiteral(bufferWriterAddressVariable, spanVariable, BasicInfoContainer.True)
                .MarkLabel(endLabel);
        }

        private static void EmbedPropertyNameNotBoolean(
            ILGenerator processor,
            string memberName,
            int memberNameByteLengthWithQuotation,
            LocalBuilder spanVariable,
            LocalBuilder bufferWriterAddressVariable,
            bool isFirst)
        {
            Span<byte> name = stackalloc byte[memberNameByteLengthWithQuotation + 2];
            name[0] = (byte)(isFirst ? '{' : ',');
            name[name.Length - 1] = (byte)':';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameByteLengthWithQuotation));
            processor.Copy(name, spanVariable, bufferWriterAddressVariable);
        }

#if CSHARP_8_OR_NEWER
        private static void FillConstructorTypesAndEmbedValues(ILGenerator processor, object?[] arguments, Type[] constructorTypes)
#else
        private static void FillConstructorTypesAndEmbedValues(ILGenerator processor, object[] arguments, Type[] constructorTypes)
#endif
        {
            for (var i = 0; i < arguments.Length; i++)
            {
                ref var type = ref constructorTypes[i];
                var argument = arguments[i];
                switch (argument)
                {
                    case null:
                        type = typeof(object);
                        processor.LdNull();
                        break;
                    case int i32:
                        type = typeof(int);
                        processor.LdcI4(i32);
                        break;
                    case uint u32:
                        type = typeof(uint);
                        processor.LdcI4((int)u32);
                        break;
                    case byte u8:
                        type = typeof(byte);
                        processor.LdcI4(u8);
                        break;
                    case sbyte i8:
                        type = typeof(sbyte);
                        processor.LdcI4(i8);
                        break;
                    case ushort u16:
                        type = typeof(ushort);
                        processor.LdcI4(u16);
                        break;
                    case short i16:
                        type = typeof(short);
                        processor.LdcI4(i16);
                        break;
                    case ulong u64:
                        type = typeof(ulong);
                        processor.Emit(OpCodes.Ldc_I8, (long)u64);
                        break;
                    case long i64:
                        type = typeof(long);
                        processor.Emit(OpCodes.Ldc_I8, i64);
                        break;
                    case char c:
                        type = typeof(char);
                        processor.LdcI4(c);
                        break;
                    case string str:
                        type = typeof(string);
                        processor.LdStr(str);
                        break;
                    case Type t:
                        type = typeof(Type);
                        processor.LdType(t);
                        break;
                    default:
                        throw new NotSupportedException(argument.GetType().FullName);
                }
            }
        }
    }
}
