// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
using System.Buffers;
using System.Runtime.InteropServices;
#endif

namespace Utf8Json.Formatters
{
    public sealed class UInt32Formatter : IJsonFormatter<uint>
    {
        public static void SerializeStatic(ref JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static uint DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt32();
        }

        public uint Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt32();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, uint value)
        {
            // 4294967295 == uint.MaxValue
            if (value < 10000)
            {
                if (value < 10)
                {
                    return 1;
                }

                if (value < 100)
                {
                    return 2;
                }

                return value < 1000 ? 3 : 4;
            }

            if (value < 10000_0000)
            {
                if (value < 10_0000)
                {
                    return 5;
                }

                if (value < 100_0000)
                {
                    return 6;
                }

                return value < 1000_0000 ? 7 : 8;
            }

            return value < 1_0_0000_0000 ? 9 : 10;
        }

        public static void SerializeSpan(JsonSerializerOptions options, uint value, Span<byte> span)
        {
            var num1 = value;
            var num2 = num1 / 10000;
            num1 -= num2 * 10000;
            var num3 = num2 / 10000;
            num2 -= num3 * 10000;
            var offset = 0;
            uint div;
            switch (span.Length)
            {
                case 10:
                    span[offset++] = (byte)('0' + (div = (num3 * 6554U) >> 16));
                    num3 -= div * 10U;
                    goto case 9;
                case  9:
                    span[offset++] = (byte)('0' + num3);
                    goto case 8;
                case  8:
                    span[offset++] = (byte)('0' + (div = (num2 * 8389U) >> 23));
                    num2 -= div * 1000U;
                    goto case 7;
                case  7:
                    span[offset++] = (byte)('0' + (div = (num2 * 5243U) >> 19));
                    num2 -= div * 100U;
                    goto case 6;
                case  6:
                    span[offset++] = (byte)('0' + (div = (num2 * 6554U) >> 16));
                    num2 -= div * 10U;
                    goto case 5;
                case  5:
                    span[offset++] = (byte)('0' + num2);
                    goto case 4;
                case  4:
                    span[offset++] = (byte)('0' + (div = (num1 * 8389U) >> 23));
                    num1 -= div * 1000U;
                    goto case 3;
                case  3:
                    span[offset++] = (byte)('0' + (div = (num1 * 5243U) >> 19));
                    num1 -= div * 100U;
                    goto case 2;
                case  2:
                    span[offset++] = (byte)('0' + (div = (num1 * 6554U) >> 16));
                    num1 -= div * 10U;
                    goto case 1;
                case  1:
                    span[offset] = (byte)('0' + num1);
                    return;
                // ReSharper disable once RedundantCaseLabel
                case  0:
                default:
                    throw new JsonSerializationException("Invalid number.");
            }
        }
    }

    public sealed class NullableUInt32Formatter : IJsonFormatter<uint?>
    {
        public static void SerializeStatic(ref JsonWriter writer, uint? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.Write(value.Value);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public void Serialize(ref JsonWriter writer, uint? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.Write(value.Value);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public static uint? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadUInt32();
        }

        public uint? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadUInt32();
        }
    }

    public sealed class UInt32ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<uint[]?>
#else
        : IJsonFormatter<uint[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, uint[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, uint[] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteBeginArray();
                if (value.Length != 0)
                {
                    writer.Write(value[0]);
                    for (int i = 1; i < value.Length; i++)
                    {
                        writer.WriteValueSeparator();
                        writer.Write(value[i]);
                    }
                }

                writer.WriteEndArray();
            }
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, uint[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, uint[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static uint[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static uint[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                var ptr = (uint*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
                var count = 0;
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 4 * capacity;
                        var tmp = (uint*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadUInt32();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<uint>();
                }

                var answer = new uint[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 4 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            Span<uint> span = stackalloc uint[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<uint> tmp = stackalloc uint[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }

                span[count - 1] = reader.ReadUInt32();
            }

            return count == 0 ? Array.Empty<uint>() : span.Slice(0, count).ToArray();
#endif
        }

#if CSHARP_8_OR_NEWER
        public uint[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public uint[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class UInt32ReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<uint>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<uint> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<uint> value, JsonSerializerOptions options)
        {
            writer.WriteBeginArray();
            if (value.Length == 0)
            {
                goto END;
            }

            var span = value.Span;
            writer.Write(span[0]);

            for (var i = 1; i < span.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(span[i]);
            }

        END:
            writer.WriteEndArray();
        }

        public ReadOnlyMemory<uint> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe ReadOnlyMemory<uint> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (uint*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt32();
            }

            uint[] answer;
            if (count == 0)
            {
                answer = Array.Empty<uint>();
            }
            else
            {
                answer = new uint[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count* 4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<uint>();
                }
                else
                {
                    var answer = new uint[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 4;
                        Buffer.MemoryCopy(src, dst, size, size);
                    }
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class UInt32NativeArrayFormatter : IJsonFormatter<NativeArray<uint>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<uint> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<uint> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<uint> value, JsonSerializerOptions options)
        {
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

        public static unsafe NativeArray<uint> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (uint*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt32();
            }

            NativeArray<uint> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<uint>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count* 4);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class UInt32MemoryFormatter : IJsonFormatter<Memory<uint>>
    {
        public void Serialize(ref JsonWriter writer, Memory<uint> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, Memory<uint> value, JsonSerializerOptions options)
        {
            writer.WriteBeginArray();
            if (value.Length == 0)
            {
                goto END;
            }

            var span = value.Span;
            writer.Write(span[0]);

            for (var i = 1; i < span.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(span[i]);
            }

        END:
            writer.WriteEndArray();
        }

        public Memory<uint> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe Memory<uint> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (uint*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt32();
            }

            uint[] answer;
            if (count == 0)
            {
                answer = Array.Empty<uint>();
            }
            else
            {
                answer = new uint[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count* 4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<uint>();
                }
                else
                {
                    var answer = new uint[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 4;
                        Buffer.MemoryCopy(src, dst, size, size);
                    }
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
#endif
        }
    }

    public sealed class UInt32ListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<uint>?>
#else
        : IOverwriteJsonFormatter<List<uint>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<uint>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<uint> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            if (value.Count == 0)
            {
                goto END;
            }

            writer.Write(value[0]);

            for (var i = 1; i < value.Count; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(value[i]);
            }

        END:
            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, List<uint>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<uint> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<uint>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<uint> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
            => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
        public static unsafe List<uint>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<uint> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (uint*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt32();
            }

            var answer = new List<uint>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt32();
                }

                var answer = new List<uint>(count);
                span = span.Slice(0, count);
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < span.Length; index++)
                {
                    answer.Add(span[index]);
                }

                return answer;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref List<uint>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<uint> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                // null, do nothing
                return;
            }

            var count = 0;
            reader.ReadIsBeginArrayWithVerify();

            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (value == null)
                {
                    value = new List<uint>();
                }

                value.Add(reader.ReadUInt32());
            }
        }
    }
}
