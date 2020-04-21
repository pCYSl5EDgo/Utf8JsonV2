// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Internal
{
    public sealed class ByteArrayKeyUnmanagedValueHashTable<T>
        where T : unmanaged
    {
#if CSHARP_8_OR_NEWER
        private readonly Entry[]?[] table;
#else
        private readonly Entry[][] table;
#endif

        public ByteArrayKeyUnmanagedValueHashTable(ReadOnlySpan<Entry> entries)
        {
            var maxLength = 0;
            foreach (ref readonly var entry in entries)
            {
                if (maxLength < entry.Key.Length)
                {
                    maxLength = entry.Key.Length;
                }
            }

            table = new Entry[maxLength + 1][];
            foreach (ref readonly var entry in entries)
            {
                ref var array = ref table[entry.Key.Length];
                HashTableHelper.SortInsert(ref array, entry);
            }
        }

        public bool TryGetValue(ReadOnlySpan<byte> key, out T value)
        {
            if (key.Length >= table.Length)
            {
                value = default;
                return false;
            }

            var array = table[key.Length];
            if (array == null)
            {
                goto NOT_FOUND;
            }

            for (var i = 0; i < array.Length; i++)
            {
                var otherKey = array[i].Key;
                for (var j = 0; j < otherKey.Length; j++)
                {
                    if (key[j] < otherKey[j])
                    {
                        goto NOT_FOUND;
                    }

                    if (key[j] > otherKey[j])
                    {
                        goto NEXT;
                    }
                }

                value = array[i].Value;
                return true;
            NEXT:;
            }

        NOT_FOUND:
            value = default;
            return false;
        }

        public readonly struct Entry : IComparable<Entry>
        {
            public readonly byte[] Key;
            public readonly T Value;

            public Entry(byte[] key, in T value)
            {
                Key = key;
                Value = value;
            }

            public int CompareTo(Entry other)
            {
                var otherKey = other.Key;
                for (var index = 0; index < Key.Length; index++)
                {
                    var b0 = Key[index];
                    var b1 = otherKey[index];
                    if (b0 == b1)
                    {
                        continue;
                    }

                    return b0 < b1 ? -1 : 1;
                }

                return 0;
            }
        }
    }
}
