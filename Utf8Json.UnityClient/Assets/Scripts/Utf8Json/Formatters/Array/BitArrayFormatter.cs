// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;

#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
using System.Buffers;
using System.Runtime.InteropServices;
#endif
#pragma warning disable IDE0060

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
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var span3 = writer.Writer.GetSpan(1);
            span3[0] = (byte)'[';
            writer.Writer.Advance(1);
            if (value.Length == 0)
            {
                goto END;
            }

            if (value[0])
            {
                var span1 = writer.Writer.GetSpan(4);
                span1[0] = (byte)'t';
                span1[1] = (byte)'r';
                span1[2] = (byte)'u';
                span1[3] = (byte)'e';
                writer.Writer.Advance(4);
            }
            else
            {
                var span2 = writer.Writer.GetSpan(5);
                span2[0] = (byte)'f';
                span2[1] = (byte)'a';
                span2[2] = (byte)'l';
                span2[3] = (byte)'s';
                span2[4] = (byte)'e';
                writer.Writer.Advance(5);
            }
            for (var i = 1; i < value.Length; i++)
            {
                var span1 = writer.Writer.GetSpan(1);
                span1[0] = (byte)',';
                writer.Writer.Advance(1);
                if (value[i])
                {
                    var span = writer.Writer.GetSpan(4);
                    span[0] = (byte)'t';
                    span[1] = (byte)'r';
                    span[2] = (byte)'u';
                    span[3] = (byte)'e';
                    writer.Writer.Advance(4);
                }
                else
                {
                    var span = writer.Writer.GetSpan(5);
                    span[0] = (byte)'f';
                    span[1] = (byte)'a';
                    span[2] = (byte)'l';
                    span[3] = (byte)'s';
                    span[4] = (byte)'e';
                    writer.Writer.Advance(5);
                }
            }

        END:
            var span4 = writer.Writer.GetSpan(1);
            span4[0] = (byte)']';
            writer.Writer.Advance(1);
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
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(32);
            var span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
            var count = 0;
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var tmp = pool.Rent(count);
                        var tmpSpan = MemoryMarshal.Cast<byte, bool>(tmp.AsSpan());
                        span.CopyTo(tmpSpan.Slice(0, span.Length));
                        span = tmpSpan;
                        pool.Return(array);
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
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            SerializeStatic(ref writer, value as BitArray, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
