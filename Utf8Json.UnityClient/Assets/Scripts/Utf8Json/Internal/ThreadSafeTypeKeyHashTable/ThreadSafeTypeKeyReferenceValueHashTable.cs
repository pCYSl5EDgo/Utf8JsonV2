// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Internal
{
    public class ThreadSafeTypeKeyReferenceHashTable<TValue>
        where TValue : class
    {
#if CSHARP_8_OR_NEWER
        private readonly Entry[]?[] pairArrayArray;
#else
        private readonly Entry[][] pairArrayArray;
#endif
        private readonly int bitMask;

        public ThreadSafeTypeKeyReferenceHashTable(int size = 16, double loadFactor = 0.75)
        {
            pairArrayArray = new Entry[HashTableHelper.CalcSize(size, loadFactor)][];
            bitMask = pairArrayArray.Length - 1;
        }

        public ThreadSafeTypeKeyReferenceHashTable(ReadOnlySpan<Entry> entries, double loadFactor) : this(entries.Length, loadFactor)
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

#if CSHARP_8_OR_NEWER
        public TValue? GetOrAdd(Type type, Func<Type, TValue?> factory)
#else
        public TValue GetOrAdd(Type type, Func<Type, TValue> factory)
#endif
        {
            if (TryGetValue(type, out var v))
            {
                return v;
            }

            lock (pairArrayArray)
            {
                TryAddInternal(new Entry(type, factory(type)), out v);
            }
            return v;
        }


#if CSHARP_8_OR_NEWER
        public bool TryGetValue(Type type, out TValue? value)
#else
        public bool TryGetValue(Type type, out TValue value)
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
                value = entry.Value;
                return true;
            }

            for (var i = 1; i < array.Length; i++)
            {
                entry = ref array[i];
                if (!ReferenceEquals(entry.Key, type))
                {
                    continue;
                }

                value = entry.Value;
                return true;
            }

        NOT_FOUND:
            value = default;
            return false;
        }

        public bool TryAdd(Entry entry)
        {
            lock (pairArrayArray)
            {
                return TryAddInternal(entry, out _);
            }
        }

#if CSHARP_8_OR_NEWER
        private bool TryAddInternal(Entry entry, out TValue? value)
#else
        private bool TryAddInternal(Entry entry, out TValue value)
#endif
        {

            var code = entry.Key.GetHashCode() & bitMask;
            ref var array = ref pairArrayArray[code];
            if (array == null)
            {
                array = new[] { entry };
                value = entry.Value;
                return true;
            }

            ref readonly var other = ref array[0];
            if (ReferenceEquals(other.Key, entry.Key))
            {
                value = other.Value;
                return false;
            }

            for (var i = 1; i < array.Length; i++)
            {
                other = ref array[i];
                if (ReferenceEquals(other.Key, entry.Key))
                {
                    value = other.Value;
                    return false;
                }
            }

            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = entry;
            value = entry.Value;
            return true;
        }


        public readonly struct Entry
        {
            public readonly Type Key;
#if CSHARP_8_OR_NEWER
            public readonly TValue? Value;
            public Entry(Type key, TValue? value)
#else
            public readonly TValue Value;
            public Entry(Type key, TValue value)
#endif
            {
                Key = key;
                Value = value;
            }
        }
    }
}