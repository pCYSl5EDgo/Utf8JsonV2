// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
// ReSharper disable UseIndexFromEndExpression

#if UNITY_2018_4_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace Utf8Json.Internal
{
    // like ArraySegment<byte> hashtable.
    // Add is safe for construction phase only and requires capacity(does not do rehash)

    internal sealed class ByteArrayStringEnumValueHashTable<T>
        where T : unmanaged, Enum
    {
#if CSHARP_8_OR_NEWER
        private readonly Entry[]?[] buckets;
#else
        private readonly Entry[][] buckets;
#endif
        private readonly ulong bitMask;

        public ByteArrayStringEnumValueHashTable(byte[][] byteArrayArray, T[] values, float loadFactor = 0.42f)
        {
            var tableSize = CalculateCapacity(values.Length, loadFactor);
            buckets = new Entry[tableSize][];
            bitMask = (ulong)buckets.Length - 1;

            for (var index = 0; index < values.Length; index++)
            {
                var key = byteArrayArray[index];
                if (key.Length == 0)
                {
                    throw new ArgumentException("key must not be empty.", nameof(key));
                }

                ref readonly var value = ref values[index];
                var code = FarmHash.Hash64(key);
                code &= bitMask;
                ref var array = ref buckets[code];
                if (array == null)
                {
                    array = new[] { new Entry(key, value) };
                }
                else
                {
                    var keySpan = key.AsSpan();
                    for (var j = 0; j < array.Length; j++)
                    {
                        ref readonly var entry = ref array[j];
                        if (keySpan.SequenceEqual(entry.Key))
                        {
                            throw new ArgumentException("Key was already exists.");
                        }
                    }

                    Array.Resize(ref array, array.Length + 1);
                    array[array.Length - 1] = new Entry(key, value);
                }
            }
        }

        public bool TryGetValue(ReadOnlySpan<byte> key, out T value)
        {
            var table = buckets;
            var hash = FarmHash.Hash64(key);
            var entry = table[hash & bitMask];

            if (entry == null) goto NOT_FOUND;

            ref readonly var v = ref entry[0];
            if (key.SequenceEqual(v.Key))
            {
                value = v.Value;
                return true;
            }

            for (var i = 1; i < entry.Length; i++)
            {
                v = ref entry[i];
                if (!key.SequenceEqual(v.Key))
                {
                    continue;
                }

                value = v.Value;
                return true;
            }

        NOT_FOUND:
            value = default;
            return false;
        }

        private static int CalculateCapacity(int collectionSize, float loadFactor)
        {
            var initialCapacity = (int)(collectionSize / loadFactor);
            var capacity = 1;
            while (capacity < initialCapacity)
            {
                capacity <<= 1;
            }

            return capacity < 8 ? 8 : capacity;
        }

        private readonly struct Entry
        {
            public readonly byte[] Key;
            public readonly T Value;

            public Entry(byte[] key, in T value)
            {
                Key = key;
                Value = value;
            }

#if DEBUG
            public override string ToString()
            {
                return "(" + Encoding.UTF8.GetString(Key) + ", " + Value + ")";
            }
#endif
        }
    }
}
