// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Utf8Json.Formatters;
using Utf8Json.Internal;
using Utf8Json.Internal.Reflection;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class BasicInfoContainer
    {
        public static readonly FieldInfo FieldJsonWriterWriter = typeof(JsonWriter).GetField("Writer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ?? throw new NullReferenceException("JsonWriter.Writer");

        public static readonly MethodInfo MethodJsonSerializerOptionsIgnoreNullValues = typeof(JsonSerializerOptions).GetMethodInstance("get_IgnoreNullValues");

        public static readonly MethodInfo MethodJsonWriterWriteEndObject = typeof(JsonWriter).GetMethodInstance("WriteEndObject");
        public static readonly MethodInfo MethodJsonWriterWriteBeginObject = typeof(JsonWriter).GetMethodInstance("WriteBeginObject");

        public static readonly MethodInfo MethodJsonReaderReadIsBeginObjectWithVerify = typeof(JsonReader).GetMethodInstance("ReadIsBeginObjectWithVerify");
        public static readonly MethodInfo MethodJsonReaderReadIsEndObjectWithSkipValueSeparator = typeof(JsonReader).GetMethodInstance("ReadIsEndObjectWithSkipValueSeparator");
        public static readonly MethodInfo MethodJsonReaderReadIsNull = typeof(JsonReader).GetMethodInstance("ReadIsNull");
        public static readonly MethodInfo MethodJsonReaderReadPropertyNameSegmentRaw = typeof(JsonReader).GetMethodInstance("ReadPropertyNameSegmentRaw");
        public static readonly MethodInfo MethodJsonReaderReadNextBlock = typeof(JsonReader).GetMethodInstance("ReadNextBlock");
        public static readonly MethodInfo MethodJsonReaderSkipWhiteSpace = typeof(JsonReader).GetMethodInstance("SkipWhiteSpace");
        public static readonly MethodInfo MethodJsonReaderReadIsNameSeparatorWithVerify = typeof(JsonReader).GetMethodInstance("ReadIsNameSeparatorWithVerify");

        public static readonly MethodInfo MethodBufferWriterGetSpan = typeof(BufferWriter).GetMethodInstance("GetSpan");
        public static readonly MethodInfo MethodBufferWriterAdvance = typeof(BufferWriter).GetMethodInstance("Advance");

        public static readonly MethodInfo MethodSpanGetItem = typeof(Span<byte>).GetMethodInstance("get_Item");

        public static readonly MethodInfo MethodReadOnlySpanGetLength = typeof(ReadOnlySpan<byte>).GetMethodInstance("get_Length");
        public static readonly MethodInfo MethodReadOnlySpanGetItem = typeof(SpanHelper).GetMethod("get_Item") ?? throw new NullReferenceException("ReadOnlySpan<byte>.get_Item");
        public static readonly MethodInfo MethodReadOnlySpanSlice = typeof(ReadOnlySpan<byte>).GetMethod("Slice", typeof(int));

        public static readonly MethodInfo MethodJsonSerializerOptionsExtensionsSerializeWithVerify = typeof(JsonSerializerOptionsExtensions).GetMethodStatic("SerializeWithVerify");
        public static readonly MethodInfo MethodJsonSerializerOptionsExtensionsDeserializeWithVerify = typeof(JsonSerializerOptionsExtensions).GetMethodStatic("DeserializeWithVerify");

        public static readonly MethodInfo MethodSerializeExtensionData = typeof(StringKeyObjectValueDictionaryFormatter).GetMethodStatic("SerializeExtensionDataStaticDetectIsFirst");

        public static readonly MethodInfo MethodStringKeyObjectValueDictionaryFormatterDeserializeStatic = typeof(StringKeyObjectValueDictionaryFormatter).GetMethodStatic("DeserializeStatic");

        public static readonly MethodInfo MethodStringIntern = typeof(string).GetMethodStatic("Intern");

        public static readonly ConstructorInfo ConstructorInfoStringKeyObjectValueDictionary = typeof(Dictionary<string, object>).GetConstructor(Array.Empty<Type>()) ?? throw new NullReferenceException();
        public static readonly MethodInfo MethodNullableStringDeserializeStaticInnerQuotation = typeof(NullableStringFormatter).GetMethodStatic("DeserializeStaticInnerQuotation");
        public static readonly MethodInfo MethodObjectFormatterDeserializeStatic = typeof(ObjectFormatter).GetMethodStatic("DeserializeStatic");
        public static readonly MethodInfo MethodStringKeyObjectValueDictionaryAdd = typeof(Dictionary<string, object>).GetMethodInstance("set_Item");

        public static readonly int Null;
        public static readonly int True;

        public static MethodInfo SerializeWithVerify(Type targetType)
        {
            return MethodJsonSerializerOptionsExtensionsSerializeWithVerify.MakeGeneric(targetType);
        }

        public static MethodInfo DeserializeWithVerify(Type targetType)
        {
            return MethodJsonSerializerOptionsExtensionsDeserializeWithVerify.MakeGeneric(targetType);
        }

        static BasicInfoContainer()
        {
            {
                ReadOnlySpan<byte> number = stackalloc byte[4]
                {
                    (byte)'n',
                    (byte)'u',
                    (byte)'l',
                    (byte)'l',
                };
                Null = MemoryMarshal.Cast<byte, int>(number)[0];
            }
            {
                ReadOnlySpan<byte> number = stackalloc byte[4]
                {
                    (byte)'t',
                    (byte)'r',
                    (byte)'u',
                    (byte)'e',
                };
                True = MemoryMarshal.Cast<byte, int>(number)[0];
            }
        }

        public static ILGenerator WriteLiteral(this ILGenerator processor, LocalBuilder spanVariable, int literal)
        {
            return processor
                .LdArg(0)
                .LdFieldAddress(FieldJsonWriterWriter)
                .Dup()
                .LdcI4(4)
                .TryCallIfNotPossibleCallVirtual(MethodBufferWriterGetSpan)
                .StLoc(spanVariable)
                .LdLocAddress(spanVariable)
                .LdcI4(0)
                .TryCallIfNotPossibleCallVirtual(MethodSpanGetItem)
                .LdcI4(literal)
                .StIndI4()
                .LdcI4(4)
                .TryCallIfNotPossibleCallVirtual(MethodBufferWriterAdvance);
        }
    }
}
