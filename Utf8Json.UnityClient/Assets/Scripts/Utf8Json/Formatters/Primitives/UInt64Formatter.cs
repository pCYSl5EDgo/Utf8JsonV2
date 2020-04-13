// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Utf8Json.Formatters
{
    public sealed class UInt64Formatter : IJsonFormatter<ulong>
    {
        public static void SerializeStatic(ref JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static ulong DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt64();
        }

        public ulong Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt64();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ulong value)
        {
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

            if (value < 10000_0000_0000)
            {
                if (value < 10_0000_0000)
                {
                    return 9;
                }

                if (value < 100_0000_0000)
                {
                    return 10;
                }

                return value < 1000_0000_0000 ? 11 : 12;
            }

            if (value < 10000_0000_0000_0000)
            {
                if (value < 10_0000_0000_0000)
                {
                    return 13;
                }

                if (value < 100_0000_0000_0000)
                {
                    return 14;
                }

                return value < 1000_0000_0000_0000 ? 15 : 16;
            }

            if (value < 10_0000_0000_0000_0000)
            {
                return 17;
            }

            if (value < 100_0000_0000_0000_0000)
            {
                return 18;
            }

            return value < 1000_0000_0000_0000_0000 ? 19 : 20;
        }

        public static void SerializeSpan(JsonSerializerOptions options, ulong value, Span<byte> span)
        {
            const int constantDiv = 10000;
            var num1 = value;
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

    public sealed class NullableUInt64Formatter : IJsonFormatter<ulong?>
    {
        public static void SerializeStatic(ref JsonWriter writer, ulong? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, ulong? value, JsonSerializerOptions options)
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

        public static ulong? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadUInt64();
        }

        public ulong? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadUInt64();
        }
    }

    public sealed class UInt64ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ulong[]?>
#else
        : IJsonFormatter<ulong[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ulong[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ulong[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, ulong[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ulong[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static ulong[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ulong[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                var ptr = (ulong*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var count = 0;
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 8 * capacity;
                        var tmp = (ulong*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadUInt64();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<ulong>();
                }

                var answer = new ulong[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 8 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            Span<ulong> span = stackalloc ulong[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<ulong> tmp = stackalloc ulong[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }

                span[count - 1] = reader.ReadUInt64();
            }

            return count == 0 ? Array.Empty<ulong>() : span.Slice(0, count).ToArray();
#endif
        }

#if CSHARP_8_OR_NEWER
        public ulong[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ulong[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class UInt64ReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<ulong>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<ulong> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<ulong> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<ulong> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe ReadOnlyMemory<ulong> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (ulong*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt64();
            }

            ulong[] answer;
            if (count == 0)
            {
                answer = Array.Empty<ulong>();
            }
            else
            {
                answer = new ulong[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count* 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<ulong>();
                }
                else
                {
                    var answer = new ulong[count];
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
    public sealed class UInt64NativeArrayFormatter : IJsonFormatter<NativeArray<ulong>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<ulong> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<ulong> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<ulong> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<ulong> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (ulong*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt64();
            }

            NativeArray<ulong> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<ulong>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count* 8);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class UInt64MemoryFormatter : IJsonFormatter<Memory<ulong>>
    {
        public void Serialize(ref JsonWriter writer, Memory<ulong> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, Memory<ulong> value, JsonSerializerOptions options)
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

        public Memory<ulong> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe Memory<ulong> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (ulong*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt64();
            }

            ulong[] answer;
            if (count == 0)
            {
                answer = Array.Empty<ulong>();
            }
            else
            {
                answer = new ulong[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count* 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<ulong>();
                }
                else
                {
                    var answer = new ulong[count];
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

    public sealed class UInt64ListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<ulong>?>
#else
        : IOverwriteJsonFormatter<List<ulong>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<ulong>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<ulong> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<ulong>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<ulong> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<ulong>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<ulong> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
            => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
        public static unsafe List<ulong>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<ulong> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (ulong*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt64();
            }

            var answer = new List<ulong>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt64();
                }

                var answer = new List<ulong>(count);
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
        public void DeserializeTo(ref List<ulong>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<ulong> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<ulong>();
                }

                value.Add(reader.ReadUInt64());
            }
        }
    }
}
