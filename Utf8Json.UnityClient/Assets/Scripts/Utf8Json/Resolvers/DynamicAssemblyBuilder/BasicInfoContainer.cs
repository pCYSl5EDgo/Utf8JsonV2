// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        public static readonly MethodInfo MethodBufferWriterGetSpan = typeof(BufferWriter).GetMethodInstance("GetSpan");
        public static readonly MethodInfo MethodBufferWriterAdvance = typeof(BufferWriter).GetMethodInstance("Advance");
        public static readonly MethodInfo MethodSpanGetItem = typeof(Span<byte>).GetMethodInstance("get_Item");
        public static readonly MethodInfo MethodSerializeWithVerify = typeof(JsonSerializerOptionsExtensions).GetMethodStatic("SerializeWithVerify");
        public static readonly MethodInfo MethodSerializeExtensionData = typeof(StringKeyObjectValueDictionaryFormatter).GetMethodStatic("SerializeExtensionDataStaticDetectIsFirst");
        public static readonly ConstructorInfo ArgumentNullExceptionConstructorInfo = typeof(ArgumentNullException).GetConstructor(Array.Empty<Type>()) ?? throw new InvalidOperationException();

        public static readonly int Null;
        public static readonly int True;

        public static MethodInfo SerializeWithVerify(Type targetType)
        {
            return MethodSerializeWithVerify.MakeGeneric(targetType);
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

        public static void WriteLiteral(this ILGenerator processor, LocalBuilder spanVariable, int literal)
        {
            processor
                .LdArg(0)
                .LdFieldAddress(FieldJsonWriterWriter)
                .Dup()
                .LdcI4(4)
                .Call(MethodBufferWriterGetSpan)
                .StLoc(spanVariable)
                .LdLocAddress(spanVariable)
                .LdcI4(0)
                .Call(MethodSpanGetItem)
                .LdcI4(literal)
                .StIndI4()
                .LdcI4(4)
                .Call(MethodBufferWriterAdvance);
        }
    }
}
