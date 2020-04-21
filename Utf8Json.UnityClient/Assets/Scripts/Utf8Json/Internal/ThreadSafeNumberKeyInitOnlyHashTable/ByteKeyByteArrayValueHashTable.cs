// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public sealed class ByteKeyByteArrayValueHashTable
    {
#if CSHARP_8_OR_NEWER
        private readonly byte[]?[] values;
#else
        private readonly byte[][] values;
#endif

        public ByteKeyByteArrayValueHashTable(ReadOnlySpan<Entry> entries)
        {
            values = new byte[256][];
            foreach (ref readonly var entry in entries)
            {
                values[entry.Key] = entry.Value;
            }
        }

#if CSHARP_8_OR_NEWER
        public byte[]? this[byte key]
#else
        public byte[] this[byte key]
#endif
        {
            get
            {
                var answer = values[key];
                return answer;
            }
        }

        public readonly struct Entry
        {
            public readonly byte Key;
            public readonly byte[] Value;

            public Entry(byte key, byte[] value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
