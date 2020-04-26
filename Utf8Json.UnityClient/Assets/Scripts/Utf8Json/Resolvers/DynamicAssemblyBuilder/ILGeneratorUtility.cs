// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    internal static class IntermediateLanguageGeneratorUtility
    {
        public static ILGenerator Dup(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Dup);
            return processor;
        }

        public static ILGenerator Pop(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Pop);
            return processor;
        }

        public static ILGenerator LdStaticField(this ILGenerator processor, FieldInfo info)
        {
            Debug.Assert(info.IsStatic);
            processor.Emit(OpCodes.Ldsfld, info);
            return processor;
        }

        public static ILGenerator LdStaticFieldAddress(this ILGenerator processor, FieldInfo info)
        {
            Debug.Assert(info.IsStatic);
            processor.Emit(OpCodes.Ldsflda, info);
            return processor;
        }

        public static ILGenerator LdFieldAddress(this ILGenerator processor, FieldInfo info)
        {
            Debug.Assert(!info.IsStatic);
            processor.Emit(OpCodes.Ldflda, info);
            return processor;
        }

        public static ILGenerator LdField(this ILGenerator processor, FieldInfo info)
        {
            Debug.Assert(!info.IsStatic);
            processor.Emit(OpCodes.Ldfld, info);
            return processor;
        }

        public static ILGenerator Constrained(this ILGenerator processor, Type type)
        {
            processor.Emit(OpCodes.Constrained, type);
            return processor;
        }

        public static ILGenerator NewObj(this ILGenerator processor, ConstructorInfo constructor)
        {
            processor.Emit(OpCodes.Newobj, constructor);
            return processor;
        }

        public static ILGenerator LdArgAddress(this ILGenerator processor, uint index)
        {
            if (index < 256)
            {
                processor.Emit(OpCodes.Ldarga_S, (byte)index);
            }
            else
            {
                processor.Emit(OpCodes.Ldarga, (short)(ushort)index);
            }

            return processor;
        }

        public static ILGenerator LdArg(this ILGenerator processor, uint index)
        {
            switch (index)
            {
                case 0: processor.Emit(OpCodes.Ldarg_0); break;
                case 1: processor.Emit(OpCodes.Ldarg_1); break;
                case 2: processor.Emit(OpCodes.Ldarg_2); break;
                case 3: processor.Emit(OpCodes.Ldarg_3); break;
                default:
                    if (index < 256)
                    {
                        processor.Emit(OpCodes.Ldarg_S, (byte)index);
                    }
                    else
                    {
                        processor.Emit(OpCodes.Ldarg, checked((short)index));
                    }
                    break;
            }
            return processor;
        }

        public static ILGenerator StLoc(this ILGenerator processor, LocalBuilder info)
        {
            switch (info.LocalIndex)
            {
                case 0:
                    processor.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    processor.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    processor.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    processor.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    processor.Emit(info.LocalIndex < 256 ? OpCodes.Stloc_S : OpCodes.Stloc, info);
                    break;
            }

            return processor;
        }

        public static ILGenerator LdLoc(this ILGenerator processor, LocalBuilder info)
        {
            switch (info.LocalIndex)
            {
                case 0:
                    processor.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    processor.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    processor.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    processor.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    processor.Emit(info.LocalIndex < 256 ? OpCodes.Ldloc_S : OpCodes.Ldloc, info);
                    break;
            }

            return processor;
        }

        public static ILGenerator StIndI4(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Stind_I4);
            return processor;
        }

        public static ILGenerator LdLocAddress(this ILGenerator processor, LocalBuilder info)
        {
            processor.Emit(info.LocalIndex < 256 ? OpCodes.Ldloca_S : OpCodes.Ldloca, info);
            return processor;
        }

        public static void BrShort(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Br_S, label);
        }

        public static void BrFalseShort(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Brfalse_S, label);
        }

        public static void BrTrueShort(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Brtrue_S, label);
        }

        public static ILGenerator Call(this ILGenerator processor, MethodInfo method)
        {
            processor.Emit(OpCodes.Call, method);
            return processor;
        }

        public static ILGenerator CallVirtual(this ILGenerator processor, MethodInfo method)
        {
            processor.Emit(OpCodes.Callvirt, method);
            return processor;
        }

        public static ILGenerator ConstrainedCallVirtual(this ILGenerator processor, Type type, MethodInfo method)
        {
            return processor.Constrained(type).CallVirtual(method);
        }

        public static ILGenerator LdNull(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Ldnull);
            return processor;
        }

        public static readonly MethodInfo SystemTypeGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle") ?? throw new NullReferenceException();

        public static ILGenerator LdType(this ILGenerator processor, Type type)
        {
            processor.Emit(OpCodes.Ldtoken, type);
            processor.Call(SystemTypeGetTypeFromHandle);
            return processor;
        }

        public static ILGenerator LdStr(this ILGenerator processor, string str)
        {
            processor.Emit(OpCodes.Ldstr, str);
            return processor;
        }

        public static ILGenerator LdcI4(this ILGenerator processor, int number)
        {
            switch (number)
            {
                case -1: processor.Emit(OpCodes.Ldc_I4_M1); return processor;
                case 1: processor.Emit(OpCodes.Ldc_I4_1); return processor;
                case 2: processor.Emit(OpCodes.Ldc_I4_2); return processor;
                case 3: processor.Emit(OpCodes.Ldc_I4_3); return processor;
                case 4: processor.Emit(OpCodes.Ldc_I4_4); return processor;
                case 5: processor.Emit(OpCodes.Ldc_I4_5); return processor;
                case 6: processor.Emit(OpCodes.Ldc_I4_6); return processor;
                case 7: processor.Emit(OpCodes.Ldc_I4_7); return processor;
                case 8: processor.Emit(OpCodes.Ldc_I4_8); return processor;
            }

            if (number <= 127 && number >= -128)
            {
                processor.Emit(OpCodes.Ldc_I4_S, (sbyte)number);
            }
            else
            {
                processor.Emit(OpCodes.Ldc_I4, number);
            }

            return processor;
        }
    }
}
