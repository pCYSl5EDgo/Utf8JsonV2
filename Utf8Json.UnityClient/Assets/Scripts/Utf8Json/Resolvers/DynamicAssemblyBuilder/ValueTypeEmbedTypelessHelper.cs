// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public static class ValueTypeEmbedTypelessHelper
    {
        public static void SerializeTypeless(Type targetType, MethodInfo serializeStatic, MethodBuilder serializeTypeless)
        {
            var processor = serializeTypeless.GetILGenerator();
            var notNull = processor.DefineLabel();
            processor
                .LdArg(2)
                .Emit(OpCodes.Brtrue_S, notNull);
            {
                processor.ThrowException(typeof(ArgumentNullException));
            }

            processor.MarkLabel(notNull);
            processor
                .LdArg(1)
                .LdArg(2)
                .Emit(OpCodes.Unbox_Any, targetType);
            processor
                .LdArg(3)
                .Call(serializeStatic)
                .Emit(OpCodes.Ret);
        }

        public static void DeserializeTypeless(Type targetType, MethodInfo deserializeStatic, MethodBuilder deserializeTypeless)
        {
            var processor = deserializeTypeless.GetILGenerator();
            processor
                .LdArg(1)
                .LdArg(2)
                .Call(deserializeStatic)
                .Emit(OpCodes.Box, targetType);
            processor.Emit(OpCodes.Ret);
        }
    }
}
