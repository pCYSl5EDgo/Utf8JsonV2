// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    internal static class StaticBinaryDataHelper
    {
        public static (FieldInfo fieldInfo, int offset) Copy(this ILGenerator processor, ReadOnlySpan<byte> wantsToEmbed, TypeBuilder typeBuilder, LocalBuilder spanVariable, BinaryDictionary dataFieldDictionary)
        {
            var tuple = dataFieldDictionary.GetOrAdd(wantsToEmbed, typeBuilder);
            
            processor
                .LdArg(0)
                .LdFieldAddress(BasicInfoContainer.FieldJsonWriterWriter)
                .Dup()
                .LdcI4(wantsToEmbed.Length)
                .Call(BasicInfoContainer.MethodBufferWriterGetSpan)
                .StLoc(spanVariable)
                .LdLocAddress(spanVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodSpanGetItem)
                .LdStaticFieldAddress(tuple.field);

            if (tuple.offset != 0)
            {
                processor
                    .LdcI4(tuple.offset)
                    .Emit(OpCodes.Add);
            }

            processor
                .LdcI4(wantsToEmbed.Length)
                .Emit(OpCodes.Cpblk);

            processor
                .LdcI4(wantsToEmbed.Length)
                .Call(BasicInfoContainer.MethodBufferWriterAdvance);
            
            return tuple;
        }

        public static void Copy(this ILGenerator processor, LocalBuilder spanVariable, int offset, int length, FieldInfo wantsToEmbed)
        {
            processor
                .LdArg(0)
                .LdFieldAddress(BasicInfoContainer.FieldJsonWriterWriter)
                .Dup()
                .LdcI4(length)
                .Call(BasicInfoContainer.MethodBufferWriterGetSpan)
                .StLoc(spanVariable)
                .LdLocAddress(spanVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodSpanGetItem)
                .LdStaticFieldAddress(wantsToEmbed);

            if (offset != 0)
            {
                processor
                    .LdcI4(offset)
                    .Emit(OpCodes.Add);
            }

            processor
                .LdcI4(length)
                .Emit(OpCodes.Cpblk);

            processor
                .LdcI4(length)
                .Call(BasicInfoContainer.MethodBufferWriterAdvance);
        }
    }
}
