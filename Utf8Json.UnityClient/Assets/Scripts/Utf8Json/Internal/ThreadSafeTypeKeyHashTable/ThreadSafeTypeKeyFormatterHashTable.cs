// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.InteropServices;

#if UNITY_2018_4_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
#endif

// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Internal
{
    public sealed class ThreadSafeTypeKeyFormatterHashTable : IDisposable
    {
#if CSHARP_8_OR_NEWER
        private readonly Type[]?[] typeArrayArray;
#else
        private readonly Type[][] typeArrayArray;
#endif
        private readonly int bitMask;
        private int shift;

        private byte[] functionBytes;

        public unsafe ThreadSafeTypeKeyFormatterHashTable(int count = 0, double loadFactor = 0.45d)
        {
            typeArrayArray = count <= 0 ? new Type[256][] : new Type[HashTableHelper.CalcSize(count, loadFactor)][];

            bitMask = typeArrayArray.Length - 1;
            shift = 0;
            functionBytes = ArrayPool<byte>.Shared.Rent(typeArrayArray.Length * sizeof(FunctionPair));
        }

        public unsafe ThreadSafeTypeKeyFormatterHashTable(ReadOnlySpan<Entry> entries, double loadFactor = 0.45d)
        {
            typeArrayArray = entries.Length == 0 ? new Type[256][] : new Type[HashTableHelper.CalcSize(entries.Length, loadFactor)][];

            bitMask = typeArrayArray.Length - 1;
            shift = 0;
            var maxCount = 0;
            foreach (ref readonly var entry in entries)
            {
                var entryKey = entry.Key;
                var code = entryKey.GetHashCode() & bitMask;
                ref var array = ref typeArrayArray[code];
                if (array == null)
                {
                    array = new[] { entryKey };
                }
                else
                {
                    Array.Resize(ref array, array.Length + 1);
                    array[array.Length - 1] = entryKey;
                }

                if (maxCount < array.Length)
                {
                    maxCount = array.Length;
                }
            }

            while (maxCount > 1 << shift)
            {
                shift++;
            }

            functionBytes = ArrayPool<byte>.Shared.Rent((typeArrayArray.Length * sizeof(FunctionPair)) << shift);

            var span = MemoryMarshal.Cast<byte, FunctionPair>(functionBytes.AsSpan());
            foreach (ref readonly var entry in entries)
            {
                var entryKey = entry.Key;
                var code = entryKey.GetHashCode() & bitMask;
#if CSHARP_8_OR_NEWER
                var array = typeArrayArray[code]!;
#else
                var array = typeArrayArray[code];
#endif

                if (ReferenceEquals(array[0], entryKey))
                {
                    span[code << shift] = entry.Pair;
                    continue;
                }

                for (var i = 1; i < array.Length; i++)
                {
                    if (!ReferenceEquals(array[i], entryKey))
                    {
                        continue;
                    }

                    span[(code << shift) + i] = entry.Pair;
                    break;
                }
            }
        }

        public FunctionPair this[Type type]
        {
            get
            {
                var code = type.GetHashCode() & bitMask;
                var array = typeArrayArray[code];
                if (array == null)
                {
                    return default;
                }

                var span = MemoryMarshal.Cast<byte, FunctionPair>(functionBytes.AsSpan());
                span = span.Slice(code << shift);
                if (ReferenceEquals(type, array[0]))
                {
                    return span[0];
                }

                for (var i = 1; i < array.Length; i++)
                {
                    if (!ReferenceEquals(array[i], type))
                    {
                        continue;
                    }

                    return span[i];
                }

                return default;
            }
        }

        public void Add(Type type, in FunctionPair pair)
        {
            var code = type.GetHashCode() & bitMask;
            ref var array = ref typeArrayArray[code];

            lock (typeArrayArray)
            {
                if (array == null)
                {
                    array = new[] { type };
                    var span = MemoryMarshal.Cast<byte, FunctionPair>(functionBytes.AsSpan());
                    span[code << shift] = pair;
                    return;
                }

                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = type;

                if (1 << shift < array.Length)
                {
                    ReAllocAndCopy();
                }

                {
                    var span = MemoryMarshal.Cast<byte, FunctionPair>(functionBytes.AsSpan());
                    span[(code << shift) + array.Length - 1] = pair;
                }
            }
        }

        private unsafe void ReAllocAndCopy()
        {
            shift++;
            var pool = ArrayPool<byte>.Shared;
            var tmp = pool.Rent((typeArrayArray.Length * sizeof(FunctionPair)) << shift);
            fixed (byte* dst = &tmp[0])
            fixed (byte* src = &functionBytes[0])
            {
                var dstSize = sizeof(FunctionPair) << shift;
                var srcSize = dstSize >> 1;
#if UNITY_2018_4_OR_NEWER
                UnsafeUtility.MemCpyStride(dst, dstSize, src, srcSize, srcSize, bitMask + 1);
#else
                var dstPtr = dst;
                var srcPtr = src;
                for (var i = 0; i < typeArrayArray.Length; i++, srcPtr += srcSize, dstPtr += dstSize)
                {
                    if (typeArrayArray[i] == null) continue;
                    Buffer.MemoryCopy(srcPtr, dstPtr, dstSize, srcSize);
                }
#endif
            }

            pool.Return(functionBytes);
            functionBytes = tmp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct FunctionPair
        {
            public readonly IntPtr SerializeFunctionPtr;
            public readonly IntPtr DeserializeFunctionPtr;
            public readonly IntPtr CalcByteLengthFunctionPtr;
            public readonly IntPtr SerializeSpanFunctionPtr;

            public FunctionPair(IntPtr serialize, IntPtr deserialize, IntPtr calcByteLength, IntPtr serializeSpan)
            {
                SerializeFunctionPtr = serialize;
                DeserializeFunctionPtr = deserialize;
                CalcByteLengthFunctionPtr = calcByteLength;
                SerializeSpanFunctionPtr = serializeSpan;
            }

            public void Deconstruct(out IntPtr serialize, out IntPtr deserialize, out IntPtr calcByteLength, out IntPtr serializeSpan)
            {
                serialize = SerializeFunctionPtr;
                deserialize = DeserializeFunctionPtr;
                calcByteLength = CalcByteLengthFunctionPtr;
                serializeSpan = SerializeSpanFunctionPtr;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct Entry
        {
            public readonly Type Key;
            public readonly FunctionPair Pair;

            public Entry(Type key, IntPtr serialize, IntPtr deserialize, IntPtr calcByteLength, IntPtr serializeSpan)
            {
                Key = key;
                Pair = new FunctionPair(serialize, deserialize, calcByteLength, serializeSpan);
            }
        }

        public void Dispose()
        {
            if (functionBytes != null)
            {
                ArrayPool<byte>.Shared.Return(functionBytes);
            }
        }
    }
}
