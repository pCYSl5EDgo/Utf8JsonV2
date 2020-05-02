// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class ReferenceTypeEmbedTypelessHelper
    {
        public static void SerializeTypeless(Type targetType, MethodInfo serializeStatic, MethodBuilder serializeTypeless)
        {
            var processor = serializeTypeless.GetILGenerator();
            processor
                .LdArg(1)
                .LdArg(2)
                .Emit(OpCodes.Isinst, targetType);
            processor
                .LdArg(3)
                .TryCallIfNotPossibleCallVirtual(serializeStatic)
                .Emit(OpCodes.Ret);
        }

        public static void DeserializeTypeless(Type targetType, MethodInfo deserializeStatic, MethodBuilder deserializeTypeless)
        {
            var processor = deserializeTypeless.GetILGenerator();
            processor
                .LdArg(1)
                .LdArg(2)
                .TryCallIfNotPossibleCallVirtual(deserializeStatic)
                .Emit(OpCodes.Ret);
        }
    }
}
