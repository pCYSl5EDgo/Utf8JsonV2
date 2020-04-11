// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
#if CSHARP_8_OR_NEWER
using System.Diagnostics.CodeAnalysis;
#endif

// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Internal
{
    public sealed class ThreadSafeTypeKeyKeyValuePairObjectKeyHashTable<TValue>
        where TValue : class
    {
#if CSHARP_8_OR_NEWER
        private readonly Entry[]?[] pairArrayArray;
#else
        private readonly Entry[][] pairArrayArray;
#endif

        private readonly int bitMask;

        public ThreadSafeTypeKeyKeyValuePairObjectKeyHashTable(double loadFactor = 0.75, int capacity = 16)
        {
            pairArrayArray = new Entry[HashTableHelper.CalcSize(capacity, loadFactor)][];
            bitMask = pairArrayArray.Length - 1;
        }

        public ThreadSafeTypeKeyKeyValuePairObjectKeyHashTable(double loadFactor, ReadOnlySpan<Entry> entries) : this(loadFactor, entries.Length)
        {
            foreach (ref readonly var entry in entries)
            {
                var code = entry.Key.GetHashCode() & bitMask;
                ref var array = ref pairArrayArray[code];
                if (array == null)
                {
                    array = new[] { entry };
                }
                else
                {
                    Array.Resize(ref array, array.Length + 1);
                    array[array.Length - 1] = entry;
                }
            }
        }

        public bool TryAdd(Entry entry)
        {
            lock (pairArrayArray)
            {
                var code = entry.Key.GetHashCode() & bitMask;
                ref var array = ref pairArrayArray[code];
                if (array == null)
                {
                    array = new[] { entry };
                    return true;
                }

                if (ReferenceEquals(array[0].Key, entry.Key))
                {
                    return false;
                }

                for (var i = 1; i < array.Length; i++)
                {
                    if (ReferenceEquals(array[i].Key, entry.Key))
                    {
                        return false;
                    }
                }

                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = entry;
                return true;
            }
        }

#if CSHARP_8_OR_NEWER
        public bool TryGetValue(Type type, [NotNullWhen(true)]out object? key, out TValue? value)
#else
        public bool TryGetValue(Type type, out object key, out TValue value)
#endif
        {
            var code = type.GetHashCode() & bitMask;
            var array = pairArrayArray[code];
            if (array == null)
            {
                goto NOT_FOUND;
            }

            ref readonly var entry = ref array[0];
            if (ReferenceEquals(entry.Key, type))
            {
                key = entry.PairKey;
                value = entry.PairValue;
                return true;
            }

            for (var i = 1; i < array.Length; i++)
            {
                entry = ref array[i];
                if (!ReferenceEquals(entry.Key, type))
                {
                    continue;
                }

                key = entry.PairKey;
                value = entry.PairValue;
                return true;
            }

        NOT_FOUND:
            key = default;
            value = default;
            return false;
        }

        public readonly struct Entry
        {
            public readonly Type Key;
            public readonly object PairKey;
#if CSHARP_8_OR_NEWER
            public readonly TValue? PairValue;

            public Entry(Type key, object pairKey, TValue? pairValue)
#else
            public readonly TValue PairValue;

            public Entry(Type key, object pairKey, TValue pairValue)
#endif
            {
                Key = key;
                PairKey = pairKey;
                PairValue = pairValue;
            }
        }
    }
}
