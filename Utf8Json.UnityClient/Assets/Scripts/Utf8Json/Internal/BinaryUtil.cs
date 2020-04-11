// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Utf8Json.Internal
{
    public static class BinaryUtil
    {
        private const int ArrayMaxSize = 0x7FFFFFC7; // https://msdn.microsoft.com/en-us/library/system.array

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCapacityArrayPool(ref byte[] bytes, int offset, int appendLength)
        {
            if (CalculateNecessaryLength(bytes, offset, appendLength, out var newSize))
            {
                return;
            }

            var newArray = ArrayPool<byte>.Shared.Rent(newSize);
            Copy(newArray, bytes, newSize);
            ArrayPool<byte>.Shared.Return(bytes);
            bytes = newArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Copy(byte[] newArray, byte[] array, int newSize)
        {
            var sourceBytesToCopy = array.Length > newSize ? newSize : array.LongLength;
            fixed (byte* src = &array[0])
            fixed (byte* dest = &newArray[0])
            {
#if UNITY_2018_4_OR_NEWER
                Unity.Collections.LowLevel.Unsafe.UnsafeUtility.MemCpy(dest, src, sourceBytesToCopy);
#else
                Buffer.MemoryCopy(src, dest, newArray.LongLength, sourceBytesToCopy);
#endif
            }
        }

        private static bool CalculateNecessaryLength(byte[] bytes, int offset, int appendLength, out int actualAllocateNumber)
        {
            var newLength = offset + appendLength;

            // like MemoryStream.EnsureCapacity
            var current = bytes.Length;
            if (newLength <= current)
            {
                actualAllocateNumber = default;
                return true;
            }

            actualAllocateNumber = newLength;
            if (actualAllocateNumber < 256)
            {
                actualAllocateNumber = 256;
                return false;
            }

            if (current == ArrayMaxSize)
            {
                throw new InvalidOperationException("byte[] size reached maximum size of array(0x7FFFFFC7), can not write to single byte[]. Details: https://msdn.microsoft.com/en-us/library/system.array");
            }

            var newSize = unchecked(current * 2);
            if (newSize < 0) // overflow
            {
                actualAllocateNumber = ArrayMaxSize;
            }
            else if (actualAllocateNumber < newSize)
            {
                actualAllocateNumber = newSize;
            }

            return false;
        }
    }
}
