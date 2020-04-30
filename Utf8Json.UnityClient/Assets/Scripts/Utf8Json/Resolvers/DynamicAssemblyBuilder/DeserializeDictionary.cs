// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Utf8Json.Formatters;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public readonly ref struct DeserializeDictionary
    {
        private readonly List<byte[]> disposeArray;

#if CSHARP_8_OR_NEWER
        public readonly Entry[]?[] Table;
#else
        public readonly Entry[][] Table;
#endif

        private readonly ReadOnlyMemory<byte> lengthVariations;

        public ReadOnlySpan<int> LengthVariations => MemoryMarshal.Cast<byte, int>(lengthVariations.Span);
        public readonly int MaxLength;
        public readonly int MinLength;

        public ReadOnlySpan<Entry> this[int length] => Table[length];

        public DeserializeDictionary(in TypeAnalyzeResult result)
        {
            disposeArray = new List<byte[]>();
            Table = Array.Empty<Entry[]>();
            var array = ArrayPool<byte>.Shared.Rent(1024);
            disposeArray.Add(array);
            var used = 0;

#if CSHARP_8_OR_NEWER
            static
#endif
            (byte[], int) EnsureArray(byte[] array1, int nameLength, int used1, List<byte[]> dispose)
            {
                if (array1.Length - used1 >= nameLength)
                {
                    return (array1, used1);
                }

                array1 = ArrayPool<byte>.Shared.Rent(nameLength);
                dispose.Add(array1);
                return (array1, 0);
            }

            #region Initialize Table
            for (var index = 0; index < result.FieldValueTypeArray.Length; index++)
            {
                var name = result.FieldValueTypeArray[index].MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.FieldValueType, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.FieldReferenceTypeArray.Length; index++)
            {
                var name = result.FieldReferenceTypeArray[index].MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.FieldReferenceType, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.FieldValueTypeShouldSerializeArray.Length; index++)
            {
                var name = result.FieldValueTypeShouldSerializeArray[index].MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.FieldValueTypeShouldSerialize, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.FieldReferenceTypeShouldSerializeArray.Length; index++)
            {
                var name = result.FieldReferenceTypeShouldSerializeArray[index].MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.FieldReferenceTypeShouldSerialize, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.PropertyValueTypeArray.Length; index++)
            {
                ref var member = ref result.PropertyValueTypeArray[index];
                if (member.Info.SetMethod is null)
                {
                    continue;
                }

                var name = member.MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.PropertyValueType, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.PropertyReferenceTypeArray.Length; index++)
            {
                ref var member = ref result.PropertyReferenceTypeArray[index];
                if (member.Info.SetMethod is null)
                {
                    continue;
                }

                var name = member.MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.PropertyReferenceType, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.PropertyValueTypeShouldSerializeArray.Length; index++)
            {
                ref var member = ref result.PropertyValueTypeShouldSerializeArray[index];
                if (member.Info.SetMethod is null)
                {
                    continue;
                }

                var name = member.MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.PropertyValueTypeShouldSerialize, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            for (var index = 0; index < result.PropertyReferenceTypeShouldSerializeArray.Length; index++)
            {
                ref var member = ref result.PropertyReferenceTypeShouldSerializeArray[index];
                if (member.Info.SetMethod is null)
                {
                    continue;
                }

                var name = member.MemberName;
                var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
                if (Table.Length <= nameLength)
                {
                    Array.Resize(ref Table, nameLength + 1);
                }

                (array, used) = EnsureArray(array, nameLength, used, disposeArray);
                var memory = array.AsMemory(used, nameLength);
                NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
                ref var entryArray = ref Table[nameLength];
                var entry = new Entry(memory, Type.PropertyReferenceTypeShouldSerialize, index);
                HashTableHelper.SortInsert(ref entryArray, entry);
                used += nameLength;
            }
            #endregion

            {
                MinLength = -1;
                MaxLength = Table.Length - 1;
                var lengthVariationCount = 0;
                for (var i = 0; i < Table.Length; i++)
                {
                    if (Table[i] is null)
                    {
                        continue;
                    }

                    lengthVariationCount++;
                    if (MinLength == -1)
                    {
                        MinLength = i;
                    }
                }

                if (MinLength == -1)
                {
                    lengthVariations = ReadOnlyMemory<byte>.Empty;
                    return;
                }

                (array, used) = EnsureArray(array, lengthVariationCount << 2, used, disposeArray);
                var memory = array.AsMemory(used, lengthVariationCount << 2);
                lengthVariations = memory;
                var span = MemoryMarshal.Cast<byte, int>(memory.Span);
                for (int i = MinLength, j = 0; i < Table.Length; i++)
                {
                    if (!(Table[i] is null))
                    {
                        span[j++] = i;
                    }
                }
            }
        }

        public readonly struct Entry : IComparable<Entry>
        {
            public readonly ReadOnlyMemory<byte> Key;
            public readonly Type Type;
            public readonly int Index;

            public Entry(ReadOnlyMemory<byte> key, Type type, int index)
            {
                Key = key;
                Type = type;
                Index = index;
            }

            public int CompareTo(Entry other)
            {
                return ByteArraySortHelper.CompareStatic(Key.Span, other.Key.Span);
            }

            public ulong this[int position] => MemoryMarshal.Cast<byte, ulong>(Key.Span)[position];

            public byte Rest(int restIndex)
            {
                var span = Key.Span;
                var restLength = span.Length % 8;

                if (restIndex >= restLength || restIndex < 0)
                {
                    throw new ArgumentOutOfRangeException("Span : " + span.Length + restIndex + " in " + restLength + " of " + Type + " index - " + Index);
                }

                var answer = span[span.Length - restLength + restIndex];
                return answer;
            }
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Pack = 1, Size = 16)]
        public readonly struct EntrySegment
        {
            [FieldOffset(0)]
            public readonly ulong Key;
            [FieldOffset(8)]
            public readonly int Offset;
            [FieldOffset(12)]
            public readonly int Length;

            public EntrySegment(ulong key, int offset, int length)
            {
                Key = key;
                Offset = offset;
                Length = length;
            }
        }

        public enum Type
        {
            FieldValueType,
            PropertyValueType,
            FieldReferenceType,
            PropertyReferenceType,
            FieldValueTypeShouldSerialize,
            PropertyValueTypeShouldSerialize,
            FieldReferenceTypeShouldSerialize,
            PropertyReferenceTypeShouldSerialize,
        }

        public void Dispose()
        {
            foreach (var item in disposeArray)
            {
                ArrayPool<byte>.Shared.Return(item);
            }
        }
    }

    public static class DeserializeDictionaryHelper
    {
        public static (byte restByte, int sameRestByteEntryArrayCount) ClassifyByRest(this ReadOnlySpan<DeserializeDictionary.Entry> entryArray, int restIndex)
        {
            var restByte = entryArray[0].Rest(restIndex);

            for (var i = 1; i < entryArray.Length; i++)
            {
                var restByteOther = entryArray[i].Rest(restIndex);
                if (restByteOther != restByte)
                {
                    return (restByte, i);
                }
            }

            return (restByte, entryArray.Length);
        }

        public static (ulong value, int sameValueEntryArrayCount) Classify(this ReadOnlySpan<DeserializeDictionary.Entry> entryArray, int position)
        {
            var value = entryArray[0][position];
            for (var i = 1; i < entryArray.Length; i++)
            {
                if (entryArray[i][position] != value)
                {
                    return (value, i);
                }
            }

            return (value, entryArray.Length);
        }

        public static int CountUpVariation(this ReadOnlySpan<DeserializeDictionary.Entry> entryArray, int position)
        {
            if (entryArray.IsEmpty)
            {
                return 0;
            }

            var answer = 1;
            var value = entryArray[0][position];
            for (var i = 1; i < entryArray.Length; i++)
            {
                var another = entryArray[i][position];
                if (another == value)
                {
                    continue;
                }

                answer++;
                value = another;
            }

            return answer;
        }

        public static void WriteVariation(this ReadOnlySpan<DeserializeDictionary.Entry> entryArray, int position, Span<DeserializeDictionary.EntrySegment> span)
        {
            if (entryArray.IsEmpty)
            {
                return;
            }

            var value = entryArray[0][position];
            var offset = 0;
            var length = 1;
            for (var i = 1; i < entryArray.Length; i++, length++)
            {
                var nextValue = entryArray[i][position];
                if (nextValue == value)
                {
                    continue;
                }

                span[0] = new DeserializeDictionary.EntrySegment(value, offset, length);
                offset += length;
                length = 0;
                value = nextValue;
                span = span.Slice(1);
            }

            span[0] = new DeserializeDictionary.EntrySegment(value, offset, length);
        }
    }
}
