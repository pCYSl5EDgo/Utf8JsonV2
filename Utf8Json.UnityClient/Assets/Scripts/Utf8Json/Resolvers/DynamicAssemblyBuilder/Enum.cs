// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Utf8Json.Internal;
// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Resolvers
{
    public sealed partial class DynamicAssemblyBuilderResolver
    {
        private static void EnumFactory(Type targetType, in BuilderSet builderSet)
        {
            var underlyingType = targetType.GetEnumUnderlyingType();
            builderSet.Type.AddInterfaceImplementation(typeof(IObjectPropertyNameFormatter<>).MakeGenericType(targetType));

            GenerateIntermediateLanguageCodesForSerializeTypeLessValueType(targetType, builderSet.SerializeStatic, builderSet.SerializeTypeless);
            GenerateIntermediateLanguageCodesForDeserializeTypelessValueType(targetType, builderSet.DeserializeStatic, builderSet.DeserializeTypeless);
            
            DefineEnumSerializeToPropertyName(targetType, builderSet);
            DefineEnumDeserializeFromPropertyName(targetType, builderSet);

            var constants = targetType.GetFields(BindingFlags.Static | BindingFlags.Public);

            var readPropertyNameSegmentRaw = typeof(JsonReader).GetMethod("ReadPropertyNameSegmentRaw");
            Debug.Assert(!(readPropertyNameSegmentRaw is null));

            var writeRaw = typeof(JsonWriter).GetMethod("WriteRaw");
            Debug.Assert(!(writeRaw is null));

            if (underlyingType == typeof(int))
            {
            }
            else if (underlyingType == typeof(byte))
            {
            }
            else if (underlyingType == typeof(ulong))
            {
            }
            else if (underlyingType == typeof(uint))
            {
            }
            else if (underlyingType == typeof(long))
            {
            }
            else if (underlyingType == typeof(sbyte))
            {
            }
            else if (underlyingType == typeof(short))
            {
            }
            else if (underlyingType == typeof(ushort))
            {
            }
            else
            {
                throw new ArgumentException("Invalid Enum UnderlyingType");
            }
        }

        private static Entry<T> EnumEntryFactory<T>(FieldInfo info)
            where T : unmanaged, IEquatable<T>, IComparable<T>
        {
            var number = FillValue<T>(info);
            var bytesWithQuotation = FillName(info);
            return new Entry<T>(number, bytesWithQuotation);
        }

        private static void DefineEnumDeserializeFromPropertyName(Type targetType, in BuilderSet builderSet)
        {
            var deserializeFromPropertyName = builderSet.Type.DefineMethod(
                "DeserializeFromPropertyName",
                InstanceMethodFlags,
                targetType,
                new[]
                {
                    typeof(JsonReader).MakeByRefType(),
                    typeof(JsonSerializerOptions),
                }
            );
            var processor = deserializeFromPropertyName.GetILGenerator();
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.EmitCall(OpCodes.Call, builderSet.DeserializeStatic, null);
            processor.Emit(OpCodes.Ret);
        }

        private static void DefineEnumSerializeToPropertyName(Type targetType, in BuilderSet builderSet)
        {
            var serializeToPropertyName = builderSet.Type.DefineMethod(
                "SerializeToPropertyName",
                InstanceMethodFlags,
                typeof(void),
                new[]
                {
                    typeof(JsonWriter).MakeByRefType(),
                    targetType,
                    typeof(JsonSerializerOptions),
                });
            var processor = serializeToPropertyName.GetILGenerator();
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.Emit(OpCodes.Ldarg_3);
            processor.EmitCall(OpCodes.Call, builderSet.SerializeStatic, null);
            processor.Emit(OpCodes.Ret);
        }

        private readonly struct EnumDictionary<T>
            where T : unmanaged, IEquatable<T>, IComparable<T>
        {
            public readonly Entry<T>[][][] Mods;

            public EnumDictionary(FieldInfo[] infos, Func<FieldInfo, Entry<T>> factory)
            {
                Mods = new Entry<T>[8][][];
                for (var i = 0; i < Mods.Length; i++)
                {
                    Mods[i] = Array.Empty<Entry<T>[]>();
                }

                foreach (var fieldInfo in infos)
                {
                    var entry = factory(fieldInfo);
                    ref var entryArrayArray = ref Mods[entry.Mod];
                    var lengthULong = entry.LengthULong;
                    ref var entryArray = ref EnsureArray(ref entryArrayArray, lengthULong);
                    HashTableHelper.SortInsert(ref entryArray, entry);
                }
            }

            private static ref Entry<T>[] EnsureArray(ref Entry<T>[][] array, int length)
            {
                if (length < array.Length)
                {
                    goto RETURN;
                }

                var oldLength = array.Length;
                Array.Resize(ref array, length + 1);
                for (var i = oldLength; i < array.Length; i++)
                {
                    array[i] = Array.Empty<Entry<T>>();
                }

            RETURN:
                return ref array[length];
            }

            public Entry<T>[] this[int length]
            {
                get
                {
                    var mod = length - ((length >> 3) << 3);
                    var len = length >> 3;
                    return Mods[mod][len];
                }
            }

            public static int FindULongVariation(ReadOnlySpan<Entry<T>> span, int index)
            {
                if (span.IsEmpty)
                {
                    return 0;
                }

                var answer = 1;
                var value = span[0][index];
                for (var i = 1; i < span.Length; i++)
                {
                    var tmpValue = span[i][index];
                    if (tmpValue == value)
                    {
                        continue;
                    }

                    answer++;
                    value = tmpValue;
                }

                return answer;
            }

            public static ReadOnlySpan<Entry<T>> Find(ReadOnlySpan<Entry<T>> span, int index, ulong value)
            {
                while (!span.IsEmpty)
                {
                    if (span[0][index] == value)
                    {
                        break;
                    }

                    span = span.Slice(1);
                }

                if (span.IsEmpty)
                {
                    return span;
                }

                for (var i = 1; i < span.Length; i++)
                {
                    if (span[i][index] != value)
                    {
                        return span.Slice(0, i);
                    }
                }

                return span;
            }
        }

        private readonly struct Entry<T> : IComparable<Entry<T>>
            where T : unmanaged, IEquatable<T>, IComparable<T>
        {
            public readonly T Number;
            public readonly byte[] BytesWithQuotation; // At least 2 bytes.

            public Entry(T number, byte[] bytesWithQuotation)
            {
                Number = number;
                BytesWithQuotation = bytesWithQuotation;
            }

            public int Mod
            {
                get
                {
                    var num = BytesWithQuotation.Length - 2;
                    return num - ((num >> 3) << 3);
                }
            }

            public int LengthULong => (BytesWithQuotation.Length - 2) >> 3;

            public ulong this[int index] => MemoryMarshal.Cast<byte, ulong>(BytesWithQuotation.AsSpan(1))[index];

            public ulong Rest
            {
                get
                {
                    switch (Mod)
                    {
                        default:
                        case 0: return default;
                        case 1: return BytesWithQuotation[BytesWithQuotation.Length - 2];
                        case 2:
                            {
                                ulong answer = BytesWithQuotation[BytesWithQuotation.Length - 2];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 3];
                                return answer;
                            }
                        case 3:
                            {
                                ulong answer = BytesWithQuotation[BytesWithQuotation.Length - 2];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 3];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 4];
                                return answer;
                            }
                        case 4:
                            {
                                ulong answer = BytesWithQuotation[BytesWithQuotation.Length - 2];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 3];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 4];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 5];
                                return answer;
                            }
                        case 5:
                            {
                                ulong answer = BytesWithQuotation[BytesWithQuotation.Length - 2];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 3];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 4];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 5];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 6];
                                return answer;
                            }
                        case 6:
                            {
                                ulong answer = BytesWithQuotation[BytesWithQuotation.Length - 2];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 3];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 4];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 5];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 6];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 7];
                                return answer;
                            }
                        case 7:
                            {
                                ulong answer = BytesWithQuotation[BytesWithQuotation.Length - 2];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 3];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 4];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 5];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 6];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 7];
                                answer <<= 8;
                                answer |= BytesWithQuotation[BytesWithQuotation.Length - 8];
                                return answer;
                            }
                    }
                }
            }

            public int CompareTo(Entry<T> other)
            {
                var c = BytesWithQuotation.Length.CompareTo(other.BytesWithQuotation.Length);
                if (c == 0)
                {
                    for (int index = 0, length = LengthULong; index < length; index++)
                    {
                        c = this[index].CompareTo(other[index]);
                        if (c != 0)
                        {
                            return c;
                        }
                    }

                    c = Rest.CompareTo(other.Rest);
                }

                return c;
            }
        }

        private static T FillValue<T>(FieldInfo constant)
            where T : unmanaged
        {
            var rawConstantValue = constant.GetRawConstantValue();
            // ReSharper disable once JoinNullCheckWithUsage
            if (rawConstantValue is null)
            {
                throw new NullReferenceException();
            }

            var value = (T)rawConstantValue;
            return value;
        }

        private static byte[] FillName(MemberInfo constant)
        {
            var nameString = constant.Name;
            var customAttributes = constant.GetCustomAttributes();
            foreach (var attribute in customAttributes)
            {
                if (!(attribute is System.Runtime.Serialization.DataMemberAttribute dataMemberAttribute))
                {
                    continue;
                }

                nameString = dataMemberAttribute.Name;
                break;
            }

            return JsonSerializer.Serialize(nameString);
        }
    }
}
