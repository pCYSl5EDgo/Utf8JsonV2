// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    internal ref struct DeserializeStaticMutableArguments
    {
#if CSHARP_8_OR_NEWER
        public LocalBuilder? ByteVariable;
        public LocalBuilder? ULongVariable;
#else
        public LocalBuilder ByteVariable;
        public LocalBuilder ULongVariable;
#endif
        public int Length;
        public int Position;
        public int Rest;
        private byte[] deserializeDictionaryEntrySegments;
        private int used;
        private readonly ArrayPool<byte> pool;

        public unsafe DeserializeStaticMutableArguments(ArrayPool<byte> pool)
        {
            this.pool = pool;
            deserializeDictionaryEntrySegments = pool.Rent(sizeof(DeserializeDictionaryEntrySegment) * 64);
            used = default;
            ByteVariable = default;
            ULongVariable = default;
            Length = default;
            Position = default;
            Rest = default;
        }

        public unsafe ReadOnlySpan<DeserializeDictionaryEntrySegment> Rent(ReadOnlySpan<DeserializeDictionary.Entry> entryArray, int classCount, int position)
        {
            var span = MemoryMarshal.Cast<byte, DeserializeDictionaryEntrySegment>(deserializeDictionaryEntrySegments).Slice(used);
            if (span.Length < classCount)
            {
                var length = sizeof(DeserializeDictionaryEntrySegment) * (used + classCount);
                var newBytes = pool.Rent(length);
                fixed (void* dst = &newBytes[0])
                fixed (void* src = &deserializeDictionaryEntrySegments[0])
                {
                    Buffer.MemoryCopy(src, dst, length, deserializeDictionaryEntrySegments.LongLength);
                }

                pool.Return(deserializeDictionaryEntrySegments);
                deserializeDictionaryEntrySegments = newBytes;
                span = MemoryMarshal.Cast<byte, DeserializeDictionaryEntrySegment>(deserializeDictionaryEntrySegments).Slice(used);
            }

            span = span.Slice(0, classCount);
            entryArray.WriteVariation(position, span);
            used += classCount;
            return span;
        }

        public void Return(int classCount)
        {
            used -= classCount;
        }

        public void ClearWithRest(int rest)
        {
            Length = Position = 0;
            Rest = rest;
        }

        public void ClearWithByteLength(int length)
        {
            Length = length >> 3;
            Rest = length - (Length << 3);
            Position = 0;
        }

        public void Dispose()
        {
            pool.Return(deserializeDictionaryEntrySegments);
        }
    }
}
