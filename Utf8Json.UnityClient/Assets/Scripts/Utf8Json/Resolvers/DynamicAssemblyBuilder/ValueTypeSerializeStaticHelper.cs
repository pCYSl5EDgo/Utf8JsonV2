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
    public static class ValueTypeSerializeStaticHelper
    {
        public static void SerializeStatic(in TypeAnalyzeResult analyzeResult, ILGenerator processor)
        {
            var spanVariable = processor.DeclareLocal(typeof(Span<byte>));

            if (analyzeResult.OnSerializing.Length != 0)
            {
                CallCallbacks(analyzeResult.OnSerializing, processor);
            }

            var didNotBeginObjectDuringValueTypePeriod = TrySerializeStaticOfValueTypeFieldAndProperty(analyzeResult, processor, spanVariable);

            var detectIsFirstVariable = default(LocalBuilder);
            if (analyzeResult.FieldValueTypeShouldSerializeArray.Length != 0)
            {
                if (didNotBeginObjectDuringValueTypePeriod)
                {
                    SerializeStatic_ValueType_ShouldSerialize_DetectIsFirst(analyzeResult.FieldValueTypeShouldSerializeArray,
                        GetShouldSerializeFieldInfo,
                        processor,
                        spanVariable,
                        IntermediateLanguageGeneratorUtility.LdField,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ValueType_ShouldSerialize_NotFirst(analyzeResult.FieldValueTypeShouldSerializeArray,
                        GetShouldSerializeFieldInfo,
                        processor,
                        spanVariable,
                        IntermediateLanguageGeneratorUtility.LdField
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
                        IntermediateLanguageGeneratorUtility.Call,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ValueType_ShouldSerialize_NotFirst(analyzeResult.PropertyValueTypeShouldSerializeArray,
                        GetShouldSerializeGetMethod,
                        processor,
                        spanVariable,
                        IntermediateLanguageGeneratorUtility.Call
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
                        IntermediateLanguageGeneratorUtility.LdField,
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
                        IntermediateLanguageGeneratorUtility.LdField,
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
                        IntermediateLanguageGeneratorUtility.Call,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable,
                        ref detectIsFirstVariable
                    );
                }
                else
                {
                    SerializeStatic_ReferenceType_NotFirst(analyzeResult.PropertyReferenceTypeArray,
                        GetGetMethod,
                        processor,
                        spanVariable,
                        IntermediateLanguageGeneratorUtility.Call,
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
                        IntermediateLanguageGeneratorUtility.LdField,
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
                        IntermediateLanguageGeneratorUtility.LdField,
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
                        IntermediateLanguageGeneratorUtility.Call,
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
                        IntermediateLanguageGeneratorUtility.Call,
                        ref ignoreNullValuesVariable,
                        ref referenceVariable
                    );
                }
            }

            WriteExtensionData(analyzeResult, processor, didNotBeginObjectDuringValueTypePeriod, referenceVariable, ref detectIsFirstVariable);

            WriteBeginObjectIfNotWritten(detectIsFirstVariable, didNotBeginObjectDuringValueTypePeriod, processor);

            processor
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonWriterWriteEndObject);

            if (analyzeResult.OnSerialized.Length != 0)
            {
                CallCallbacks(analyzeResult.OnSerialized, processor);
            }

            processor.Emit(OpCodes.Ret);
        }

        private static void WriteExtensionData(
            in TypeAnalyzeResult analyzeResult,
            ILGenerator processor,
            bool didNotBeginObjectDuringValueTypePeriod,
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
            processor
                .LdArgAddress(1)
                .Call(getMethod)
                .Dup()
                .StLoc(referenceVariable)
                .BrFalseShort(skipLabel);

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

            processor.Call(BasicInfoContainer.MethodSerializeExtensionData);
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
                    .Call(BasicInfoContainer.MethodJsonWriterWriteBeginObject);
                return;
            }

            var returnLabel = processor.DefineLabel();
            processor
                .LdLoc(detectIsFirstVariable)
                .BrFalseShort(returnLabel);

            processor
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonWriterWriteBeginObject)
                .MarkLabel(returnLabel);
        }

        private static void SerializeStatic_ReferenceType_ShouldSerialize_NotFirst<TContainer, T>(
            TContainer[] members,
            Function<TContainer, T> func,
            ILGenerator processor,
            LocalBuilder spanVariable,
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
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
                processor
                    .LdArgAddress(1)
                    .Call(info.ShouldSerialize)
                    .BrFalseShort(skipLabel);

                var doNotIgnoreNull = processor.DefineLabel();
                loadTargetByFunc(processor.LdArgAddress(1), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalseShort(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalseShort(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeNotFirst(processor, spanVariable, referenceVariable, ref info, objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedReferenceTypeNotFirst<TContainer>(
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder referenceVariable,
            ref TContainer info,
            object objHelp)
            where TContainer : struct, IMemberContainer
        {
            EmbedPropertyNameNotBoolean(processor, info.MemberName, spanVariable, false);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.String:
                    LoadTargetReferenceType(processor.LdArg(0), referenceVariable, objHelp)
                        .Call(ReadWritePrimitive.MethodWritePrimitives[(int)DirectTypeEnum.String]);
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
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ignoreNullValuesVariable,
            ref LocalBuilder? referenceVariable,
            ref LocalBuilder? detectIsFirstVariable)
#else
            ref LocalBuilder ignoreNullValuesVariable,
            ref LocalBuilder referenceVariable,
            ref LocalBuilder detectIsFirstVariable)
#endif
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
                processor
                    .LdArgAddress(1)
                    .Call(info.ShouldSerialize)
                    .BrFalseShort(skipLabel);

                var doNotIgnoreNull = processor.DefineLabel();
                loadTargetByFunc(processor.LdArgAddress(1), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalseShort(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalseShort(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeDetectFirst(processor, spanVariable, referenceVariable, detectIsFirstVariable, ref info, objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static bool TrySerializeStaticOfValueTypeFieldAndProperty(
            in TypeAnalyzeResult analyzeResult,
            ILGenerator processor,
            LocalBuilder spanVariable)
        {
            var didNotWriteBeginObject = analyzeResult.FieldValueTypeArray.Length == 0;
            if (analyzeResult.FieldValueTypeArray.Length != 0)
            {
                SerializeStatic_ValueType(analyzeResult.FieldValueTypeArray,
                    GetFieldInfo,
                    processor,
                    spanVariable,
                    true,
                    IntermediateLanguageGeneratorUtility.LdField
                );
            }

            if (analyzeResult.PropertyValueTypeArray.Length != 0)
            {
                didNotWriteBeginObject = SerializeStatic_ValueType(analyzeResult.PropertyValueTypeArray,
                    GetGetMethod,
                    processor,
                    spanVariable,
                    didNotWriteBeginObject,
                    IntermediateLanguageGeneratorUtility.Call
                );
            }

            return didNotWriteBeginObject;
        }

        private static (LocalBuilder ignoreNullValuesVariable, LocalBuilder referenceVariable) DefineIgnoreNullValuesVariable(ILGenerator processor)
        {
            var ignoreNullValues = processor.DeclareLocal(typeof(bool));
            processor.LdArg(2).Call(BasicInfoContainer.MethodJsonSerializerOptionsIgnoreNullValues).StLoc(ignoreNullValues);
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
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ignoreNullValuesVariable,
            ref LocalBuilder? referenceVariable,
            ref LocalBuilder? detectIsFirstVariable)
#else
            ref LocalBuilder ignoreNullValuesVariable,
            ref LocalBuilder referenceVariable,
            ref LocalBuilder detectIsFirstVariable)
#endif
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
                loadTargetByFunc(processor.LdArgAddress(1), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalseShort(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalseShort(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeDetectFirst(processor, spanVariable, referenceVariable, detectIsFirstVariable, ref info, objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedReferenceTypeDetectFirst<TContainer>(
            ILGenerator processor,
            LocalBuilder spanVariable,
            LocalBuilder referenceVariable,
            LocalBuilder detectIsFirstVariable,
            ref TContainer info,
            object objHelp)
            where TContainer : struct, IMemberContainer
        {
            EmbedPropertyNameNotBoolean_DetectIsFirst(processor, info.MemberName, spanVariable, detectIsFirstVariable);
            switch (info.IsFormatterDirect)
            {
                case DirectTypeEnum.String:
                    LoadTargetReferenceType(processor.LdArg(0), referenceVariable, objHelp)
                        .Call(ReadWritePrimitive.MethodWritePrimitives[(int)DirectTypeEnum.String]);
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
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
#if CSHARP_8_OR_NEWER
            ref LocalBuilder? ignoreNullValuesVariable,
            ref LocalBuilder? referenceVariable)
#else
            ref LocalBuilder ignoreNullValuesVariable,
            ref LocalBuilder referenceVariable)
#endif
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
                loadTargetByFunc(processor.LdArgAddress(1), t)
                    .StLoc(referenceVariable)
                    .LdLoc(ignoreNullValuesVariable)
                    .BrFalseShort(doNotIgnoreNull);

                // ignore null values
                processor
                    .LdLoc(referenceVariable)
                    .BrFalseShort(skipLabel);

                processor.MarkLabel(doNotIgnoreNull);

                EmbedReferenceTypeNotFirst(processor, spanVariable, referenceVariable, ref info, objHelp);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void SerializeStatic_ValueType_ShouldSerialize_NotFirst<TContainer, T>(
            TContainer[] members,
            Function<TContainer, T> func,
            ILGenerator processor,
            LocalBuilder spanVariable,
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc)
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

                processor
                    .LdArgAddress(1)
                    .Call(info.ShouldSerialize)
                    .BrFalseShort(skipLabel);

                EmbedEachInfo_ValueType(processor, spanVariable, loadTargetByFunc, ref info, false, t);

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedEachInfo_ValueType<TContainer, T>(
            ILGenerator processor,
            LocalBuilder spanVariable,
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
            ref TContainer info,
            bool isFirst,
            T t)
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
                    EmbedPropertyNameNotBoolean(processor, info.MemberName, spanVariable, isFirst);
                    LoadTargetValueType(processor.LdArg(0), loadTargetByFunc, t)
                        .Call(ReadWritePrimitive.MethodWritePrimitives[(int)info.IsFormatterDirect]);
                    break;
                case DirectTypeEnum.Boolean:
                    EmbedBoolean(processor, info.MemberName, t, spanVariable, isFirst, loadTargetByFunc);
                    break;
                case DirectTypeEnum.None:
                    EmbedPropertyNameNotBoolean(processor, info.MemberName, spanVariable, isFirst);
                    Embed_None(processor, ref info, loadTargetByFunc, t, LoadTargetValueType);
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
            Func<ILGenerator, T, ILGenerator> loadTargetByFunc,
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

                processor
                    .LdArgAddress(1)
                    .Call(info.ShouldSerialize)
                    .BrFalseShort(skipLabel);

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
                        EmbedPropertyNameNotBoolean_DetectIsFirst(processor, info.MemberName, spanVariable, detectIsFirstVariable);
                        loadTargetByFunc(processor
                            .LdArg(0)
                            .LdArgAddress(1), t)
                            .Call(ReadWritePrimitive.MethodWritePrimitives[(int)info.IsFormatterDirect]);
                        break;
                    case DirectTypeEnum.Boolean:
                        EmbedBoolean_DetectIsFirst(processor, info.MemberName, t, spanVariable, loadTargetByFunc, detectIsFirstVariable, skipLabel);
                        break;
                    case DirectTypeEnum.None:
                        EmbedPropertyNameNotBoolean_DetectIsFirst(processor, info.MemberName, spanVariable, detectIsFirstVariable);
                        // ReSharper disable once RedundantTypeArgumentsOfMethod
                        Embed_None(processor, ref info, loadTargetByFunc, t, LoadTargetValueType<T>);
                        break;
                    case DirectTypeEnum.String:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                processor.MarkLabel(skipLabel);
            }
        }

        private static void EmbedBoolean_DetectIsFirst<T>(
            ILGenerator processor,
            string memberName,
            T t,
            LocalBuilder spanVariable,
            Func<ILGenerator, T, ILGenerator> process,
            LocalBuilder detectIsFirstVariable,
            Label endLabel)
            where T : class
        {
            var memberNameLength = NullableStringFormatter.CalcByteLength(memberName);
            Span<byte> name = stackalloc byte[memberNameLength + 7];
            name[0] = (byte)',';
            name[name.Length - 6] = (byte)':';
            name[name.Length - 5] = (byte)'f';
            name[name.Length - 4] = (byte)'a';
            name[name.Length - 3] = (byte)'l';
            name[name.Length - 2] = (byte)'s';
            name[name.Length - 1] = (byte)'e';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameLength));

            var writeTrueLabel = processor.DefineLabel();
            var whenFirstTime = processor.DefineLabel();
            processor
                .LdLoc(detectIsFirstVariable)
                .BrTrueShort(whenFirstTime);

            {
                var trueLabel = processor.DefineLabel();
                process(processor.LdArgAddress(1), t).Emit(OpCodes.Brtrue_S, trueLabel);

                processor.Copy(name, spanVariable).Emit(OpCodes.Br_S, endLabel);

                processor.MarkLabel(trueLabel);
                processor.Copy(name.Slice(0, memberNameLength + 2), spanVariable).Emit(OpCodes.Br_S, writeTrueLabel);
            }

            processor.MarkLabel(whenFirstTime);
            {
                processor
                    .LdcI4(0)
                    .StLoc(detectIsFirstVariable)
                    .LdArg(0)
                    .Call(BasicInfoContainer.MethodJsonWriterWriteBeginObject);
                var trueLabel = processor.DefineLabel();
                process(processor.LdArgAddress(1), t).BrTrueShort(trueLabel);
                processor.Copy(name.Slice(1), spanVariable).Emit(OpCodes.Br_S, endLabel);

                processor.MarkLabel(trueLabel);
                processor.Copy(name.Slice(1, memberNameLength + 1), spanVariable);
            }

            processor.MarkLabel(writeTrueLabel);
            processor.WriteLiteral(spanVariable, BasicInfoContainer.True);
        }

        private static void EmbedPropertyNameNotBoolean_DetectIsFirst(
            ILGenerator processor,
            string memberName,
            LocalBuilder spanVariable,
            LocalBuilder detectIsFirstVariable)
        {
            var memberNameLength = NullableStringFormatter.CalcByteLength(memberName);
            Span<byte> name = stackalloc byte[memberNameLength + 2];
            name[0] = (byte)',';
            name[name.Length - 1] = (byte)':';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameLength));

            var whenFirstTime = processor.DefineLabel();
            processor
                .LdLoc(detectIsFirstVariable)
                .BrTrueShort(whenFirstTime);

            var endLabel = processor.DefineLabel();
            processor.Copy(name, spanVariable).BrShort(endLabel);
            processor.MarkLabel(whenFirstTime);
            processor
                .LdcI4(0)
                .StLoc(detectIsFirstVariable)
                .LdArg(0)
                .Call(BasicInfoContainer.MethodJsonWriterWriteBeginObject)
                .Copy(name.Slice(1, memberNameLength + 1), spanVariable)
                .MarkLabel(endLabel);
        }

        private static void CallCallbacks(ReadOnlySpan<MethodInfo> methods, ILGenerator processor)
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
                        .LdArgAddress(1)
                        .Call(methodInfo);
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
            bool isFirst,
            Func<ILGenerator, T, ILGenerator> process)
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

                EmbedEachInfo_ValueType(processor, spanVariable, process, ref info, isFirst, t);

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
                loadTarget(processor.LdArg(2).LdArg(0), t0, t1)
                    .Call(BasicInfoContainer.SerializeWithVerify(info.TargetType));
                return;
            }

            var jsonFormatterType = jsonFormatterAttribute.FormatterType;
            var interfaceMethodSerialize = typeof(IJsonFormatter<>).MakeGeneric(info.TargetType).GetMethodInstance("Serialize");

#if CSHARP_8_OR_NEWER
            ConstructorInfo? jsonFormatterDefaultConstructor;
#else
            ConstructorInfo jsonFormatterDefaultConstructor;
#endif
            var arguments = jsonFormatterAttribute.Arguments;
            if (arguments is null)
            {
                if (Try_Embed_None_With_Static_Formatter(processor, info.TargetType, jsonFormatterType, interfaceMethodSerialize, loadTarget, t0, t1))
                {
                    return;
                }

                jsonFormatterDefaultConstructor = jsonFormatterType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            }
            else
            {
                var constructorTypes = new Type[arguments.Length];
                FillConstructorTypesAndEmbedValues(processor, arguments, constructorTypes);

                jsonFormatterDefaultConstructor = jsonFormatterType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, constructorTypes, null);
            }

            Debug.Assert(!(jsonFormatterDefaultConstructor is null));
            loadTarget(processor.NewObj(jsonFormatterDefaultConstructor).LdArg(0), t0, t1)
                .LdArg(2)
                .ConstrainedCallVirtual(info.TargetType, interfaceMethodSerialize);
        }

        private static ILGenerator LoadTargetValueType<T>(ILGenerator generator, Func<ILGenerator, T, ILGenerator> func, T t)
            where T : class
        {
            return func(generator.LdArgAddress(1), t);
        }

        private static bool Try_Embed_None_With_Static_Formatter<T0, T1>(ILGenerator processor, Type targetType, Type jsonFormatterType, MethodInfo interfaceMethodSerialize, Func<ILGenerator, T0, T1, ILGenerator> loadTarget, T0 t0, T1 t1)
            where T0 : class
            where T1 : class
        {
            var targetJsonFormattersSerializeStatic = jsonFormatterType.GetMethod("SerializeStatic", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (!(targetJsonFormattersSerializeStatic is null))
            {
                loadTarget(processor.LdArg(0), t0, t1)
                    .LdArg(2)
                    .Call(targetJsonFormattersSerializeStatic);
                return true;
            }

            var fieldInstance = jsonFormatterType.GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (fieldInstance is null)
            {
                fieldInstance = jsonFormatterType.GetField("Default", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInstance is null)
                {
                    return false;
                }
            }

            loadTarget(processor.LdStaticFieldAddress(fieldInstance).LdArg(0), t0, t1)
                .LdArg(2)
                .ConstrainedCallVirtual(targetType, interfaceMethodSerialize);
            return true;
        }

        private static void EmbedBoolean<T>(ILGenerator processor, string memberName, T t, LocalBuilder spanVariable, bool isFirst, Func<ILGenerator, T, ILGenerator> process)
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

            process(processor.LdArgAddress(1), t).Emit(OpCodes.Brtrue_S, trueLabel);

            processor.Copy(name, spanVariable).Emit(OpCodes.Br_S, endLabel);
            processor.MarkLabel(trueLabel);
            processor
                .Copy(name.Slice(0, memberNameLength + 2), spanVariable)
                .WriteLiteral(spanVariable, BasicInfoContainer.True)
                .MarkLabel(endLabel);
        }

        private static void EmbedPropertyNameNotBoolean(ILGenerator processor, string memberName, LocalBuilder spanVariable, bool isFirst)
        {
            var memberNameLength = NullableStringFormatter.CalcByteLength(memberName);
            Span<byte> name = stackalloc byte[memberNameLength + 2];
            name[0] = (byte)(isFirst ? '{' : ',');
            name[name.Length - 1] = (byte)':';
            NullableStringFormatter.SerializeSpanNotNull(memberName, name.Slice(1, memberNameLength));
            processor.Copy(name, spanVariable);
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
