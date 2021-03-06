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

        public static ILGenerator StField(this ILGenerator processor, FieldInfo info)
        {
            Debug.Assert(!info.IsStatic);
            processor.Emit(OpCodes.Stfld, info);
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

        public static ILGenerator Add(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Add);
            return processor;
        }

        public static ILGenerator Or(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Or);
            return processor;
        }

        public static ILGenerator And(this ILGenerator processor)
        {
            processor.Emit(OpCodes.And);
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

        public static ILGenerator StIndI1(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Stind_I1);
            return processor;
        }

        public static ILGenerator StIndI2(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Stind_I2);
            return processor;
        }

        public static ILGenerator StIndI4(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Stind_I4);
            return processor;
        }

        public static ILGenerator StIndI8(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Stind_I8);
            return processor;
        }

        public static ILGenerator LdLocAddress(this ILGenerator processor, LocalBuilder info)
        {
            processor.Emit(info.LocalIndex < 256 ? OpCodes.Ldloca_S : OpCodes.Ldloca, info);
            return processor;
        }

        public static void Switch(this ILGenerator processor, Label[] labels)
        {
            processor.Emit(OpCodes.Switch, labels);
        }

        public static void Beq(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Beq, label);
        }

        public static void BltUn(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Blt_Un, label);
        }

        public static void BneUn(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Bne_Un, label);
        }

        public static void Br(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Br, label);
        }

        public static void BrFalse(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Brfalse, label);
        }

        public static void BrTrue(this ILGenerator processor, Label label)
        {
            processor.Emit(OpCodes.Brtrue, label);
        }

        public static ILGenerator TryCallIfNotPossibleCallVirtual(this ILGenerator processor, MethodInfo method)
        {
            if (!method.IsStatic && method.IsVirtual && !method.IsFinal)
            {
                var declaringType = method.DeclaringType;
                if (!(declaringType is null) && !declaringType.IsValueType && !declaringType.IsSealed)
                {
                    processor.Emit(OpCodes.Callvirt, method);
                    return processor;
                }
            }

            processor.Emit(OpCodes.Call, method);
            return processor;
        }

        public static ILGenerator LdIndU1(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Ldind_U1);
            return processor;
        }

        public static ILGenerator LdIndU4(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Ldind_U4);
            return processor;
        }

        public static ILGenerator LdIndI8(this ILGenerator processor)
        {
            processor.Emit(OpCodes.Ldind_I8);
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
            processor.TryCallIfNotPossibleCallVirtual(SystemTypeGetTypeFromHandle);
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

        public static ILGenerator LdcI8(this ILGenerator processor, long number)
        {
            if (int.MinValue <= number && number <= int.MaxValue)
            {
                processor
                    .LdcI4((int)number)
                    .Emit(OpCodes.Conv_I8);
            }
            else
            {
                processor.Emit(OpCodes.Ldc_I8, number);
            }

            return processor;
        }

        public static ILGenerator LdcI8(this ILGenerator processor, ulong number) => processor.LdcI8((long)number);
    }
}
