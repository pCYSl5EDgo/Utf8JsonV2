// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;

#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace Utf8Json.Formatters
{
    public sealed class BitArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<BitArray?>
#else
        : IJsonFormatter<BitArray>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, BitArray? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, BitArray value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public BitArray? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public BitArray Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, BitArray? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, BitArray value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            if (value.Length == 0)
            {
                goto END;
            }

            writer.Write(value[0]);
            for (var i = 1; i < value.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(value[i]);
            }

        END:
            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        public static BitArray? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static BitArray DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (bool*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
                var count = 0;
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 1 * capacity;
                        var tmp = (bool*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }

                    ptr[count - 1] = reader.ReadBoolean();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return new BitArray(Array.Empty<bool>());
                }

                var answer = new bool[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 1 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return new BitArray(answer);
            }
#else
            Span<bool> span = stackalloc bool[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<bool> tmp = stackalloc bool[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }

                span[count - 1] = reader.ReadBoolean();
            }

            if (count == 0)
            {
                return new BitArray(Array.Empty<bool>());
            }

            var answer = new bool[count];
            unsafe
            {
                fixed (void* dst = &answer[0])
                fixed (void* src = &span[0])
                {
                    Buffer.MemoryCopy(src, dst, count, count);
                }
            }

            return new BitArray(answer);
#endif
        }
    }
}
