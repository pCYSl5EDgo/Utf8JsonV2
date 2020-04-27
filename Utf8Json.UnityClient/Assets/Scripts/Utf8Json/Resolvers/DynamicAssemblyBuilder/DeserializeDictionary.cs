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
#if CSHARP_8_OR_NEWER
    public ref struct DeserializeDictionary
#else
    public struct DeserializeDictionary : IDisposable
#endif

    {
        private readonly List<byte[]> disposeArray;

#if CSHARP_8_OR_NEWER
        private Entry[]?[] table;
        public Entry[]?[] Table => table;
#else
        private Entry[][] table;
        public Entry[][] Table => table;
#endif

        public int LengthVariationCount()
        {
            var answer = 0;
            foreach (var array in Table)
            {
                if (!(array is null)) ++answer;
            }

            return answer;
        }

        public int MaxLength => Table.Length - 1;

        public int MinLength
        {
            get
            {
                for (var answer = 0; answer < Table.Length; answer++)
                {
                    if (!(Table[answer] is null))
                    {
                        return answer;
                    }
                }

                return -1;
            }
        }

        public ReadOnlySpan<Entry> this[int length] => Table[length];

        public DeserializeDictionary(in TypeAnalyzeResult result)
        {
            disposeArray = new List<byte[]>();
            var pool = ArrayPool<byte>.Shared;
            table = Array.Empty<Entry[]>();
            var array = pool.Rent(1024);
            disposeArray.Add(array);
            var used = 0;

            for (var index = 0; index < result.FieldValueTypeArray.Length; index++)
            {
                array = InsertEntity(result.FieldValueTypeArray, index, array, pool, Type.FieldValueType, ref used);
            }
            for (var index = 0; index < result.FieldReferenceTypeArray.Length; index++)
            {
                array = InsertEntity(result.FieldReferenceTypeArray, index, array, pool, Type.FieldReferenceType, ref used);
            }
            for (var index = 0; index < result.FieldValueTypeShouldSerializeArray.Length; index++)
            {
                array = InsertEntity(result.FieldValueTypeShouldSerializeArray, index, array, pool, Type.FieldValueTypeShouldSerialize, ref used);
            }
            for (var index = 0; index < result.FieldReferenceTypeShouldSerializeArray.Length; index++)
            {
                array = InsertEntity(result.FieldReferenceTypeShouldSerializeArray, index, array, pool, Type.FieldReferenceTypeShouldSerialize, ref used);
            }
            for (var index = 0; index < result.PropertyValueTypeArray.Length; index++)
            {
                array = InsertPropertyEntity(result.PropertyValueTypeArray, index, array, pool, Type.PropertyValueType, ref used);
            }
            for (var index = 0; index < result.PropertyReferenceTypeArray.Length; index++)
            {
                array = InsertPropertyEntity(result.PropertyReferenceTypeArray, index, array, pool, Type.PropertyReferenceType, ref used);
            }
            for (var index = 0; index < result.PropertyValueTypeShouldSerializeArray.Length; index++)
            {
                array = InsertPropertyEntity(result.PropertyValueTypeShouldSerializeArray, index, array, pool, Type.PropertyValueTypeShouldSerialize, ref used);
            }
            for (var index = 0; index < result.PropertyReferenceTypeShouldSerializeArray.Length; index++)
            {
                array = InsertPropertyEntity(result.PropertyReferenceTypeShouldSerializeArray, index, array, pool, Type.PropertyReferenceTypeShouldSerialize, ref used);
            }
        }

        private byte[] InsertEntity<T>(T[] members, int index, byte[] array, ArrayPool<byte> pool, Type type, ref int used)
            where T : struct, IMemberContainer
        {
            var name = members[index].MemberName;
            var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
            ResizeTable(nameLength);
            array = EnsureArray(array, nameLength, pool, ref used);
            var memory = array.AsMemory(used, nameLength);
            NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
            var entry = new Entry(memory, type, index);
            ref var entryArray = ref Table[nameLength];
            HashTableHelper.SortInsert(ref entryArray, entry);
            used += nameLength;
            return array;
        }

        private byte[] InsertPropertyEntity<T>(T[] members, int index, byte[] array, ArrayPool<byte> pool, Type type, ref int used)
            where T : struct, IPropertyMemberContainer
        {
            ref var member = ref members[index];
            if (member.Info.SetMethod is null && member.AddMethodInfo is null)
            {
                return array;
            }

            var name = member.MemberName;
            var nameLength = NullableStringFormatter.CalcByteLength(name) - 2;
            ResizeTable(nameLength);
            array = EnsureArray(array, nameLength, pool, ref used);
            var memory = array.AsMemory(used, nameLength);
            NullableStringFormatter.SerializeSpanNotNullNoQuotation(name, memory.Span);
            var entry = new Entry(memory, type, index);
            ref var entryArray = ref Table[nameLength];
            HashTableHelper.SortInsert(ref entryArray, entry);
            used += nameLength;
            return array;
        }

        private void ResizeTable(int index)
        {
            if (Table.Length > index) return;
            Array.Resize(ref table, index + 1);
        }

        private byte[] EnsureArray(byte[] array, int nameLength, ArrayPool<byte> pool, ref int used)
        {
            if (array.Length - used >= nameLength)
            {
                return array;
            }

            used = 0;
            array = pool.Rent(nameLength);
            disposeArray.Add(array);
            return array;
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
                var restLength = span.Length - ((span.Length >> 3) << 3);
                if (restIndex >= restLength || restIndex < 0) throw new ArgumentOutOfRangeException();
                var answer = span[span.Length - restLength + restIndex];
                return answer;
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
                if (entryArray[i].Rest(restIndex) != restByte)
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
    }
}
