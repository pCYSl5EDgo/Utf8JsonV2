// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    internal static class StaticBinaryDataHelper
    {
        public static ILGenerator Copy(this ILGenerator processor, ReadOnlySpan<byte> wantsToEmbed, LocalBuilder spanVariable)
        {
            processor
                .LdArg(0)
                .LdFieldAddress(BasicInfoContainer.FieldJsonWriterWriter)
                .Dup()
                .LdcI4(wantsToEmbed.Length)
                .Call(BasicInfoContainer.MethodBufferWriterGetSpan)
                .StLoc(spanVariable)
                .LdLocAddress(spanVariable)
                .LdcI4(0)
                .Call(BasicInfoContainer.MethodSpanGetItem);

            var advance = wantsToEmbed.Length;
            while (!wantsToEmbed.IsEmpty)
            {
                switch (wantsToEmbed.Length)
                {
                    case 1:
                        processor.LdcI4(wantsToEmbed[0]).StIndI1();
                        wantsToEmbed = wantsToEmbed.Slice(1);
                        break;
                    case 2:
                        processor.LdcI4(MemoryMarshal.Cast<byte, ushort>(wantsToEmbed)[0]).StIndI2();
                        wantsToEmbed = wantsToEmbed.Slice(2);
                        break;
                    case 3:
                        var number3 = wantsToEmbed[2];
                        processor.Dup().LdcI4(MemoryMarshal.Cast<byte, ushort>(wantsToEmbed)[0]).StIndI2().LdcI4(2).Add().LdcI4(number3).StIndI1();
                        wantsToEmbed = wantsToEmbed.Slice(3);
                        break;
                    case 4:
                        processor.LdcI4(MemoryMarshal.Cast<byte, int>(wantsToEmbed)[0]).StIndI4();
                        wantsToEmbed = wantsToEmbed.Slice(4);
                        break;
                    case 5:
                        var number5 = wantsToEmbed[4];
                        processor.Dup().LdcI4(MemoryMarshal.Cast<byte, int>(wantsToEmbed)[0]).StIndI4().LdcI4(4).Add().LdcI4(number5).StIndI1();
                        wantsToEmbed = wantsToEmbed.Slice(5);
                        break;
                    case 6:
                        var number6 = MemoryMarshal.Cast<byte, ushort>(wantsToEmbed.Slice(4))[0];
                        processor.Dup().LdcI4(MemoryMarshal.Cast<byte, int>(wantsToEmbed)[0]).StIndI4().LdcI4(4).Add().LdcI4(number6).StIndI2();
                        wantsToEmbed = wantsToEmbed.Slice(6);
                        break;
                    case 7:
                        var number7 = MemoryMarshal.Cast<byte, ushort>(wantsToEmbed.Slice(4))[0];
                        processor.Dup().LdcI4(MemoryMarshal.Cast<byte, int>(wantsToEmbed)[0]).StIndI4().LdcI4(4).Add().Dup().LdcI4(number7).StIndI2().LdcI4(1).Add().LdcI4(wantsToEmbed[6]).StIndI1();
                        wantsToEmbed = wantsToEmbed.Slice(7);
                        break;
                    case 8:
                        processor.LdcI8(MemoryMarshal.Cast<byte, long>(wantsToEmbed)[0]).StIndI8();
                        wantsToEmbed = wantsToEmbed.Slice(8);
                        break;
                    default:
                        processor.Dup().LdcI8(MemoryMarshal.Cast<byte, long>(wantsToEmbed)[0]).StIndI8().LdcI4(8).Add();
                        wantsToEmbed = wantsToEmbed.Slice(8);
                        break;
                }
            }

            return processor
                .LdcI4(advance)
                .Call(BasicInfoContainer.MethodBufferWriterAdvance);
        }
    }
}
