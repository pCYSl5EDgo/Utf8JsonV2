// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Utf8Json.Formatters
{
    public sealed class Int32Formatter : IJsonFormatter<int>
    {
        public static void SerializeStatic(ref JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static int DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt32();
        }

        public int Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt32();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, int value)
        {
            if (int.MinValue == value)
            {
                return 11;
            }

            var isNegative = value < 0;
            var num1 = (uint)(isNegative ? -value : value);
            // 4294967295 == uint.MaxValue
            if (num1 < 10000)
            {
                if (num1 < 10)
                {
                    return isNegative ? 2 : 1;
                }

                if (num1 < 100)
                {
                    return isNegative ? 3 : 2;
                }

                if (num1 < 1000)
                {
                    return isNegative ? 4 : 3;
                }

                return isNegative ? 5 : 4;
            }

            if (num1 < 10000_0000)
            {
                if (num1 < 10_0000)
                {
                    return isNegative ? 6 : 5;
                }

                if (num1 < 100_0000)
                {
                    return isNegative ? 7 : 6;
                }

                if (num1 < 1000_0000)
                {
                    return isNegative ? 8 : 7;
                }

                return isNegative ? 9 : 8;
            }

            if (num1 < 1_0_0000_0000)
                return isNegative ? 10 : 9;

            return isNegative ? 11 : 10;
        }

        public static void SerializeSpan(JsonSerializerOptions options, int value, Span<byte> span)
        {
            if (value == int.MinValue)
            {
                span[0] = (byte)'-';
                span[1] = (byte)'2';
                span[2] = (byte)'1';
                span[3] = (byte)'4';
                span[4] = (byte)'7';
                span[5] = (byte)'4';
                span[6] = (byte)'8';
                span[7] = (byte)'3';
                span[8] = (byte)'6';
                span[9] = (byte)'4';
                span[10] = (byte)'8';
                return;
            }

            if (value < 0)
            {
                span[0] = (byte)'-';
                span = span.Slice(1);
                value = -value;
            }

            var num1 = (uint)value;
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
                case 9:
                    span[offset++] = (byte)('0' + num3);
                    goto case 8;
                case 8:
                    span[offset++] = (byte)('0' + (div = (num2 * 8389U) >> 23));
                    num2 -= div * 1000U;
                    goto case 7;
                case 7:
                    span[offset++] = (byte)('0' + (div = (num2 * 5243U) >> 19));
                    num2 -= div * 100U;
                    goto case 6;
                case 6:
                    span[offset++] = (byte)('0' + (div = (num2 * 6554U) >> 16));
                    num2 -= div * 10U;
                    goto case 5;
                case 5:
                    span[offset++] = (byte)('0' + num2);
                    goto case 4;
                case 4:
                    span[offset++] = (byte)('0' + (div = (num1 * 8389U) >> 23));
                    num1 -= div * 1000U;
                    goto case 3;
                case 3:
                    span[offset++] = (byte)('0' + (div = (num1 * 5243U) >> 19));
                    num1 -= div * 100U;
                    goto case 2;
                case 2:
                    span[offset++] = (byte)('0' + (div = (num1 * 6554U) >> 16));
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

    public sealed class NullableInt32Formatter : IJsonFormatter<int?>
    {
        public static void SerializeStatic(ref JsonWriter writer, int? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, int? value, JsonSerializerOptions options)
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

        public static int? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadInt32();
        }

        public int? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadInt32();
        }
    }

    public sealed class Int32ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<int[]?>
#else
        : IJsonFormatter<int[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, int[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, int[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, int[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, int[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static int[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static int[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                var ptr = (int*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
                var count = 0;
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 4 * capacity;
                        var tmp = (int*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadInt32();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<int>();
                }

                var answer = new int[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 4 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            Span<int> span = stackalloc int[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<int> tmp = stackalloc int[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }

                span[count - 1] = reader.ReadInt32();
            }

            return count == 0 ? Array.Empty<int>() : span.Slice(0, count).ToArray();
#endif
        }

#if CSHARP_8_OR_NEWER
        public int[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public int[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class Int32ReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<int>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<int> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<int> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<int> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
                    => DeserializeStatic(ref reader, options);

        public static unsafe ReadOnlyMemory<int> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (int*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (int*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt32();
            }

            int[] answer;
            if (count == 0)
            {
                answer = Array.Empty<int>();
            }
            else
            {
                answer = new int[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count*4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<int>();
                }
                else
                {
                    var answer = new int[count];
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
    public sealed class Int32NativeArrayFormatter : IJsonFormatter<NativeArray<int>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<int> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<int> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<int> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<int> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (int*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (int*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt32();
            }

            NativeArray<int> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<int>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count*4);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class Int32MemoryFormatter : IJsonFormatter<Memory<int>>
    {
        public void Serialize(ref JsonWriter writer, Memory<int> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, Memory<int> value, JsonSerializerOptions options)
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

        public Memory<int> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
                    => DeserializeStatic(ref reader, options);

        public static unsafe Memory<int> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (int*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (int*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt32();
            }

            int[] answer;
            if (count == 0)
            {
                answer = Array.Empty<int>();
            }
            else
            {
                answer = new int[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count*4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<int>();
                }
                else
                {
                    var answer = new int[count];
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

    public sealed class Int32ListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<int>?>
#else
        : IOverwriteJsonFormatter<List<int>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<int>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<int> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<int>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<int> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<int>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<int> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
            => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
        public static unsafe List<int>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<int> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (int*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (int*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt32();
            }

            var answer = new List<int>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt32();
                }

                var answer = new List<int>(count);
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
        public void DeserializeTo(ref List<int>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<int> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<int>();
                }

                value.Add(reader.ReadInt32());
            }
        }
    }
}