// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Resolvers
{
    public sealed partial class DynamicAssemblyBuilderResolver
    {
        private static readonly ConstructorInfo constructorReadOnlySpan = typeof(ReadOnlySpan<byte>).GetConstructor(new[]
        {
            typeof(void).MakePointerType(),
            typeof(int),
        }) ?? throw new NullReferenceException();

        private static void CreateReadOnlySpan(ILGenerator processor, ReadOnlySpan<byte> wantsToEmbed, TypeBuilder typeBuilder)
        {
            var (field, offset) = dataFieldDictionary.GetOrAdd(wantsToEmbed, typeBuilder);
            processor.Emit(OpCodes.Ldsflda, field);
            if (offset != 0)
            {
                processor.LdcI4(offset);
                processor.Emit(OpCodes.Add);
            }
            processor.LdcI4(wantsToEmbed.Length);
            processor.Emit(OpCodes.Newobj, constructorReadOnlySpan);
        }
    }
}
