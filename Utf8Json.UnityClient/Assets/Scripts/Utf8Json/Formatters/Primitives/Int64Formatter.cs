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
    public sealed class Int64Formatter : IJsonFormatter<long>
    {
        public static void SerializeStatic(ref JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static long DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt64();
        }

        public long Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt64();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, long value)
        {
            if (value == long.MinValue)
            {
                return 20;
            }
            var answer = 0;
            if (value < 0)
            {
                answer = 1;
                value = -value;
            }

            var value1 = (ulong)value;
            if (value1 < 10000)
            {
                if (value1 < 10)
                {
                    answer += 1;
                }
                else
                {
                    if (value1 < 100)
                    {
                        answer += 2;
                    }
                    else
                    {
                        answer += value1 < 1000 ? 3 : 4;
                    }
                }
            }
            else if (value1 < 10000_0000)
            {
                if (value1 < 10_0000)
                {
                    answer += 5;
                }
                else
                {
                    if (value1 < 100_0000)
                    {
                        answer += 6;
                    }
                    else
                    {
                        answer += value1 < 1000_0000 ? 7 : 8;
                    }
                }
            }
            else if (value1 < 10000_0000_0000)
            {
                if (value1 < 10_0000_0000)
                {
                    answer += 9;
                }
                else
                {
                    if (value1 < 100_0000_0000)
                    {
                        answer += 10;
                    }
                    else
                    {
                        answer += value1 < 1000_0000_0000 ? 11 : 12;
                    }
                }
            }
            else if (value1 < 10000_0000_0000_0000)
            {
                if (value1 < 10_0000_0000_0000)
                {
                    answer += 13;
                }
                else if (value1 < 100_0000_0000_0000)
                {
                    answer += 14;
                }
                else
                {
                    answer += value1 < 1000_0000_0000_0000 ? 15 : 16;
                }
            }
            else if (value1 < 10_0000_0000_0000_0000)
            {
                answer += 17;
            }
            else if (value1 < 100_0000_0000_0000_0000)
            {
                answer += 18;
            }
            else if (value1 < 1000_0000_0000_0000_0000)
            {
                answer += 19;
            }
            else
            {
                answer += 20;
            }

            return answer;
        }

        public static void SerializeSpan(JsonSerializerOptions options, long value, Span<byte> span)
        {
            if (value == long.MinValue)
            {
                span[0] = (byte)'-';
                span[1] = (byte)'9';
                span[2] = (byte)'2';
                span[3] = (byte)'2';
                span[4] = (byte)'3';
                span[5] = (byte)'3';
                span[6] = (byte)'7';
                span[7] = (byte)'2';
                span[8] = (byte)'0';
                span[9] = (byte)'3';
                span[10] = (byte)'6';
                span[11] = (byte)'8';
                span[12] = (byte)'5';
                span[13] = (byte)'4';
                span[14] = (byte)'7';
                span[15] = (byte)'7';
                span[16] = (byte)'5';
                span[17] = (byte)'8';
                span[18] = (byte)'0';
                span[19] = (byte)'8';
                return;
            }

            if (value < 0)
            {
                span[0] = (byte)'-';
                span = span.Slice(1);
                value = -value;
            }

            const int constantDiv = 10000;
            var num1 = (ulong)value;
            var num2 = num1 / constantDiv;
            num1 -= num2 * constantDiv;
            var num3 = num2 / constantDiv;
            num2 -= num3 * constantDiv;
            var num4 = num3 / constantDiv;
            num3 -= num4 * constantDiv;
            var num5 = num4 / constantDiv;
            num4 -= num5 * constantDiv;
            var offset = 0;
            ulong div;
            switch (span.Length)
            {
                case 20:
                    span[offset++] = (byte)('0' + (div = (num5 * 8389UL) >> 23));
                    num5 -= div * 1000;
                    goto case 19;
                case 19:
                    span[offset++] = (byte)('0' + (div = (num5 * 5243UL) >> 19));
                    num5 -= div * 100;
                    goto case 18;
                case 18:
                    span[offset++] = (byte)('0' + (div = (num5 * 6554UL) >> 16));
                    num5 -= div * 10;
                    goto case 17;
                case 17:
                    span[offset++] = (byte)('0' + num5);
                    goto case 16;
                case 16:
                    span[offset++] = (byte)('0' + (div = (num4 * 8389UL) >> 23));
                    num4 -= div * 1000;
                    goto case 15;
                case 15:
                    span[offset++] = (byte)('0' + (div = (num4 * 5243UL) >> 19));
                    num4 -= div * 100;
                    goto case 14;
                case 14:
                    span[offset++] = (byte)('0' + (div = (num4 * 6554UL) >> 16));
                    num4 -= div * 10; goto case 13;
                case 13:
                    span[offset++] = (byte)('0' + num4);
                    goto case 12;
                case 12:
                    span[offset++] = (byte)('0' + (div = (num3 * 8389UL) >> 23));
                    num3 -= div * 1000;
                    goto case 11;
                case 11:
                    span[offset++] = (byte)('0' + (div = (num3 * 5243UL) >> 19));
                    num3 -= div * 100;
                    goto case 10;
                case 10:
                    span[offset++] = (byte)('0' + (div = (num3 * 6554UL) >> 16));
                    num3 -= div * 10U;
                    goto case 9;
                case 9:
                    span[offset++] = (byte)('0' + num3);
                    goto case 8;
                case 8:
                    span[offset++] = (byte)('0' + (div = (num2 * 8389UL) >> 23));
                    num2 -= div * 1000U;
                    goto case 7;
                case 7:
                    span[offset++] = (byte)('0' + (div = (num2 * 5243UL) >> 19));
                    num2 -= div * 100U;
                    goto case 6;
                case 6:
                    span[offset++] = (byte)('0' + (div = (num2 * 6554UL) >> 16));
                    num2 -= div * 10U;
                    goto case 5;
                case 5:
                    span[offset++] = (byte)('0' + num2);
                    goto case 4;
                case 4:
                    span[offset++] = (byte)('0' + (div = (num1 * 8389UL) >> 23));
                    num1 -= div * 1000U;
                    goto case 3;
                case 3:
                    span[offset++] = (byte)('0' + (div = (num1 * 5243UL) >> 19));
                    num1 -= div * 100U;
                    goto case 2;
                case 2:
                    span[offset++] = (byte)('0' + (div = (num1 * 6554UL) >> 16));
                    num1 -= div * 10U;
                    goto case 1;
                case 1:
                    span[offset] = (byte)('0' + num1);
                    return;
                // ReSharper disable once RedundantCaseLabel
                case 0:
                default:
                    throw new JsonSerializationException("Invalid number.");
            }
        }
    }

    public sealed class NullableInt64Formatter : IJsonFormatter<long?>
    {
        public static void SerializeStatic(ref JsonWriter writer, long? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, long? value, JsonSerializerOptions options)
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

        public static long? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadInt64();
        }

        public long? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadInt64();
        }
    }

    public sealed class Int64ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<long[]?>
#else
        : IJsonFormatter<long[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, long[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, long[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, long[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, long[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static long[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static long[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                var ptr = (long*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var count = 0;
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 8 * capacity;
                        var tmp = (long*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadInt64();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<long>();
                }

                var answer = new long[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 8 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            Span<long> span = stackalloc long[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<long> tmp = stackalloc long[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }

                span[count - 1] = reader.ReadInt64();
            }

            return count == 0 ? Array.Empty<long>() : span.Slice(0, count).ToArray();
#endif
        }

#if CSHARP_8_OR_NEWER
        public long[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public long[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class Int64ReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<long>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<long> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<long> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<long> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe ReadOnlyMemory<long> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (long*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (long*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt64();
            }

            long[] answer;
            if (count == 0)
            {
                answer = Array.Empty<long>();
            }
            else
            {
                answer = new long[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<long>();
                }
                else
                {
                    var answer = new long[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 8;
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
    public sealed class Int64NativeArrayFormatter : IJsonFormatter<NativeArray<long>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<long> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<long> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<long> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<long> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (long*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (long*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt64();
            }

            NativeArray<long> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<long>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 8);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class Int64MemoryFormatter : IJsonFormatter<Memory<long>>
    {
        public void Serialize(ref JsonWriter writer, Memory<long> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, Memory<long> value, JsonSerializerOptions options)
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

        public Memory<long> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe Memory<long> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (long*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (long*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt64();
            }

            long[] answer;
            if (count == 0)
            {
                answer = Array.Empty<long>();
            }
            else
            {
                answer = new long[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<long>();
                }
                else
                {
                    var answer = new long[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 8;
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

    public sealed class Int64ListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<long>?>
#else
        : IOverwriteJsonFormatter<List<long>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<long>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<long> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<long>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<long> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<long>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<long> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
            => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
        public static unsafe List<long>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<long> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (long*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (long*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt64();
            }

            var answer = new List<long>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt64();
                }

                var answer = new List<long>(count);
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
        public void DeserializeTo(ref List<long>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<long> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<long>();
                }

                value.Add(reader.ReadInt64());
            }
        }
    }
}
