// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public sealed class UInt64KeyByteArrayValueHashTable
    {
#if CSHARP_8_OR_NEWER
        private readonly ulong[]?[] keys;
#else
        private readonly ulong[][] keys;
#endif
        private readonly byte[][][] values;
        private readonly ulong bitMask;

        public UInt64KeyByteArrayValueHashTable(ReadOnlySpan<Entry> entries, double loadFactor)
        {
            keys = new ulong[HashTableHelper.CalcSize(entries.Length, loadFactor)][];
            values = new byte[keys.Length][][];
            bitMask = (ulong)(keys.LongLength - 1);
            foreach (ref readonly var entry in entries)
            {
                var code = entry.Key & bitMask;
                ref var keyArray = ref keys[code];
                HashTableHelper.Add(ref keyArray, entry.Key);
                ref var valueArray = ref values[code];
                HashTableHelper.Add(ref valueArray, entry.Value);
                Array.Sort(keyArray, valueArray);
            }
        }

#if CSHARP_8_OR_NEWER
        public byte[]? this[ulong key]
#else
        public byte[] this[ulong key]
#endif
        {
            get
            {
                var code = key & bitMask;
                var keyArray = keys[code];
                if (keyArray == null)
                {
                    return default;
                }

                var index = Array.BinarySearch(keyArray, key);
                if (index == -1)
                {
                    return default;
                }

                var answer = values[code][index];
                return answer;
            }
        }

        public readonly struct Entry
        {
            public readonly ulong Key;
            public readonly byte[] Value;

            public Entry(ulong key, byte[] value)
            {
                Key = key;
                Value = value;
            }

            public int CompareTo(Entry other)
            {
                if (Key == other.Key)
                {
                    return 0;
                }

                return Key < other.Key ? -1 : 1;
            }
        }
    }
}
