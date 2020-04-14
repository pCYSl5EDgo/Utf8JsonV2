// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/* THIS (.cs) FILE IS GENERATED. DO NOT CHANGE IT.
 * CHANGE THE .tt FILE INSTEAD. */

using System;
using System.Collections.Generic;

#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
using System.Buffers;
using System.Runtime.InteropServices;
#endif

// ReSharper disable StackAllocInsideLoop
#pragma warning disable SA1649
#pragma warning disable IDE0060

namespace Utf8Json.Formatters
{
    public sealed class NullableByteFormatter : IJsonFormatter<byte?>
    {
        public static void SerializeStatic(ref JsonWriter writer, byte? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, byte? value, JsonSerializerOptions options)
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

        public static byte? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadByte();
        }

        public byte? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadByte();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, byte? value)
        {
            if (value.HasValue)
            {
                return ByteFormatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, byte? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            ByteFormatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class ByteArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<byte[]?>
#else
        : IJsonFormatter<byte[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, byte[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, byte[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += ByteFormatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, byte[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, byte[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = ByteFormatter.CalcByteLengthForSerialization(options, value[0]);
            ByteFormatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = ByteFormatter.CalcByteLengthForSerialization(options, item);
                ByteFormatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, byte[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, byte[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, byte[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, byte[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static byte[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static byte[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (byte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 1 * capacity;
                        var tmp = (byte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadByte();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<byte>();
                }

                var answer = new byte[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 1 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadByte();
                }

                if (count == 0)
                {
                    return Array.Empty<byte>();
                }

                var answer = new byte[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 1;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public byte[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public byte[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class ByteReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<byte>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<byte> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<byte> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<byte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<byte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (byte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (byte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadByte();
            }

            byte[] answer;
            if (count == 0)
            {
                answer = Array.Empty<byte>();
            }
            else
            {
                answer = new byte[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 1);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadByte();
                }

                if (count == 0)
                {
                    return Array.Empty<byte>();
                }

                var answer = new byte[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 1;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class ByteNativeArrayFormatter : IJsonFormatter<NativeArray<byte>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<byte> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<byte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<byte> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<byte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (byte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (byte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadByte();
            }

            NativeArray<byte> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<byte>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 1);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class ByteMemoryFormatter : IJsonFormatter<Memory<byte>>
    {
        public void Serialize(ref JsonWriter writer, Memory<byte> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<byte> value, JsonSerializerOptions options)
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

        public Memory<byte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<byte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (byte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (byte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadByte();
            }

            byte[] answer;
            if (count == 0)
            {
                answer = Array.Empty<byte>();
            }
            else
            {
                answer = new byte[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 1);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadByte();
                }

                if (count == 0)
                {
                    return Array.Empty<byte>();
                }
                else
                {
                    var answer = new byte[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 1;
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

    public sealed class ByteListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<byte>?>
#else
        : IOverwriteJsonFormatter<List<byte>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<byte>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<byte> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<byte>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<byte> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<byte>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<byte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<byte>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<byte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (byte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (byte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadByte();
            }

            var answer = new List<byte>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, byte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadByte();
                }

                var answer = new List<byte>(count);
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
        public void DeserializeTo(ref List<byte>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<byte> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<byte>();
                }

                value.Add(reader.ReadByte());
            }
        }
    }

    public sealed class NullableSByteFormatter : IJsonFormatter<sbyte?>
    {
        public static void SerializeStatic(ref JsonWriter writer, sbyte? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, sbyte? value, JsonSerializerOptions options)
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

        public static sbyte? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadSByte();
        }

        public sbyte? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadSByte();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, sbyte? value)
        {
            if (value.HasValue)
            {
                return SByteFormatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, sbyte? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            SByteFormatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class SByteArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<sbyte[]?>
#else
        : IJsonFormatter<sbyte[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, sbyte[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, sbyte[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += SByteFormatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, sbyte[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, sbyte[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = SByteFormatter.CalcByteLengthForSerialization(options, value[0]);
            SByteFormatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = SByteFormatter.CalcByteLengthForSerialization(options, item);
                SByteFormatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, sbyte[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, sbyte[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, sbyte[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, sbyte[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static sbyte[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static sbyte[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (sbyte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 1 * capacity;
                        var tmp = (sbyte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadSByte();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<sbyte>();
                }

                var answer = new sbyte[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 1 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSByte();
                }

                if (count == 0)
                {
                    return Array.Empty<sbyte>();
                }

                var answer = new sbyte[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 1;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public sbyte[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public sbyte[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class SByteReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<sbyte>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<sbyte> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<sbyte> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<sbyte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<sbyte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (sbyte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (sbyte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSByte();
            }

            sbyte[] answer;
            if (count == 0)
            {
                answer = Array.Empty<sbyte>();
            }
            else
            {
                answer = new sbyte[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 1);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSByte();
                }

                if (count == 0)
                {
                    return Array.Empty<sbyte>();
                }

                var answer = new sbyte[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 1;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class SByteNativeArrayFormatter : IJsonFormatter<NativeArray<sbyte>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<sbyte> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<sbyte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<sbyte> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<sbyte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (sbyte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (sbyte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSByte();
            }

            NativeArray<sbyte> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<sbyte>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 1);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class SByteMemoryFormatter : IJsonFormatter<Memory<sbyte>>
    {
        public void Serialize(ref JsonWriter writer, Memory<sbyte> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<sbyte> value, JsonSerializerOptions options)
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

        public Memory<sbyte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<sbyte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (sbyte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (sbyte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSByte();
            }

            sbyte[] answer;
            if (count == 0)
            {
                answer = Array.Empty<sbyte>();
            }
            else
            {
                answer = new sbyte[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 1);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSByte();
                }

                if (count == 0)
                {
                    return Array.Empty<sbyte>();
                }
                else
                {
                    var answer = new sbyte[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 1;
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

    public sealed class SByteListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<sbyte>?>
#else
        : IOverwriteJsonFormatter<List<sbyte>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<sbyte>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<sbyte> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<sbyte>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<sbyte> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<sbyte>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<sbyte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<sbyte>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<sbyte> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (sbyte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 1 * capacity;
                    var tmp = (sbyte*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSByte();
            }

            var answer = new List<sbyte>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 1);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 1;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSByte();
                }

                var answer = new List<sbyte>(count);
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
        public void DeserializeTo(ref List<sbyte>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<sbyte> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<sbyte>();
                }

                value.Add(reader.ReadSByte());
            }
        }
    }

    public sealed class NullableInt16Formatter : IJsonFormatter<short?>
    {
        public static void SerializeStatic(ref JsonWriter writer, short? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, short? value, JsonSerializerOptions options)
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

        public static short? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadInt16();
        }

        public short? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadInt16();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, short? value)
        {
            if (value.HasValue)
            {
                return Int16Formatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, short? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            Int16Formatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class Int16ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<short[]?>
#else
        : IJsonFormatter<short[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, short[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, short[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += Int16Formatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, short[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, short[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = Int16Formatter.CalcByteLengthForSerialization(options, value[0]);
            Int16Formatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = Int16Formatter.CalcByteLengthForSerialization(options, item);
                Int16Formatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, short[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, short[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, short[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, short[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static short[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static short[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (short*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 2 * capacity;
                        var tmp = (short*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadInt16();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<short>();
                }

                var answer = new short[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 2 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt16();
                }

                if (count == 0)
                {
                    return Array.Empty<short>();
                }

                var answer = new short[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 2;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public short[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public short[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class Int16ReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<short>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<short> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<short> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<short> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<short> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (short*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (short*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt16();
            }

            short[] answer;
            if (count == 0)
            {
                answer = Array.Empty<short>();
            }
            else
            {
                answer = new short[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 2);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt16();
                }

                if (count == 0)
                {
                    return Array.Empty<short>();
                }

                var answer = new short[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 2;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class Int16NativeArrayFormatter : IJsonFormatter<NativeArray<short>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<short> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<short> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<short> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<short> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (short*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (short*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt16();
            }

            NativeArray<short> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<short>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 2);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class Int16MemoryFormatter : IJsonFormatter<Memory<short>>
    {
        public void Serialize(ref JsonWriter writer, Memory<short> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<short> value, JsonSerializerOptions options)
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

        public Memory<short> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<short> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (short*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (short*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt16();
            }

            short[] answer;
            if (count == 0)
            {
                answer = Array.Empty<short>();
            }
            else
            {
                answer = new short[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 2);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt16();
                }

                if (count == 0)
                {
                    return Array.Empty<short>();
                }
                else
                {
                    var answer = new short[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 2;
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

    public sealed class Int16ListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<short>?>
#else
        : IOverwriteJsonFormatter<List<short>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<short>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<short> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<short>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<short> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<short>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<short> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<short>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<short> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (short*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (short*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadInt16();
            }

            var answer = new List<short>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, short>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt16();
                }

                var answer = new List<short>(count);
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
        public void DeserializeTo(ref List<short>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<short> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<short>();
                }

                value.Add(reader.ReadInt16());
            }
        }
    }

    public sealed class NullableUInt16Formatter : IJsonFormatter<ushort?>
    {
        public static void SerializeStatic(ref JsonWriter writer, ushort? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, ushort? value, JsonSerializerOptions options)
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

        public static ushort? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadUInt16();
        }

        public ushort? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadUInt16();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ushort? value)
        {
            if (value.HasValue)
            {
                return UInt16Formatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, ushort? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            UInt16Formatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class UInt16ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ushort[]?>
#else
        : IJsonFormatter<ushort[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ushort[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ushort[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += UInt16Formatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, ushort[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, ushort[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = UInt16Formatter.CalcByteLengthForSerialization(options, value[0]);
            UInt16Formatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = UInt16Formatter.CalcByteLengthForSerialization(options, item);
                UInt16Formatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ushort[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ushort[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, ushort[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ushort[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static ushort[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ushort[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (ushort*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 2 * capacity;
                        var tmp = (ushort*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadUInt16();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<ushort>();
                }

                var answer = new ushort[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 2 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt16();
                }

                if (count == 0)
                {
                    return Array.Empty<ushort>();
                }

                var answer = new ushort[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 2;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public ushort[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ushort[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class UInt16ReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<ushort>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<ushort> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<ushort> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<ushort> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<ushort> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (ushort*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (ushort*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt16();
            }

            ushort[] answer;
            if (count == 0)
            {
                answer = Array.Empty<ushort>();
            }
            else
            {
                answer = new ushort[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 2);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt16();
                }

                if (count == 0)
                {
                    return Array.Empty<ushort>();
                }

                var answer = new ushort[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 2;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class UInt16NativeArrayFormatter : IJsonFormatter<NativeArray<ushort>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<ushort> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<ushort> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<ushort> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<ushort> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (ushort*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (ushort*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt16();
            }

            NativeArray<ushort> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<ushort>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 2);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class UInt16MemoryFormatter : IJsonFormatter<Memory<ushort>>
    {
        public void Serialize(ref JsonWriter writer, Memory<ushort> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<ushort> value, JsonSerializerOptions options)
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

        public Memory<ushort> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<ushort> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (ushort*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (ushort*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt16();
            }

            ushort[] answer;
            if (count == 0)
            {
                answer = Array.Empty<ushort>();
            }
            else
            {
                answer = new ushort[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 2);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt16();
                }

                if (count == 0)
                {
                    return Array.Empty<ushort>();
                }
                else
                {
                    var answer = new ushort[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 2;
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

    public sealed class UInt16ListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<ushort>?>
#else
        : IOverwriteJsonFormatter<List<ushort>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<ushort>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<ushort> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<ushort>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<ushort> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<ushort>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<ushort> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<ushort>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<ushort> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (ushort*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 2 * capacity;
                    var tmp = (ushort*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadUInt16();
            }

            var answer = new List<ushort>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 2);
            var span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 2;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, ushort>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt16();
                }

                var answer = new List<ushort>(count);
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
        public void DeserializeTo(ref List<ushort>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<ushort> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<ushort>();
                }

                value.Add(reader.ReadUInt16());
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

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, int? value)
        {
            if (value.HasValue)
            {
                return Int32Formatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, int? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            Int32Formatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class Int32ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<int[]?>
#else
        : IJsonFormatter<int[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, int[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, int[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += Int32Formatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, int[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, int[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = Int32Formatter.CalcByteLengthForSerialization(options, value[0]);
            Int32Formatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = Int32Formatter.CalcByteLengthForSerialization(options, item);
                Int32Formatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


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
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
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
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<int>();
                }

                var answer = new int[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 4;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
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
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
                    UnsafeUtility.MemCpy(dst, ptr, count * 4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, int>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<int>();
                }

                var answer = new int[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 4;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class Int32NativeArrayFormatter : IJsonFormatter<NativeArray<int>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<int> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<int> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
                UnsafeUtility.MemCpy(dst, ptr, count * 4);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class Int32MemoryFormatter : IJsonFormatter<Memory<int>>
    {
        public void Serialize(ref JsonWriter writer, Memory<int> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
                    UnsafeUtility.MemCpy(dst, ptr, count * 4);
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
        {
            return DeserializeStatic(ref reader, options);
        }

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

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, uint? value)
        {
            if (value.HasValue)
            {
                return UInt32Formatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, uint? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            UInt32Formatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class UInt32ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<uint[]?>
#else
        : IJsonFormatter<uint[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, uint[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, uint[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += UInt32Formatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, uint[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, uint[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = UInt32Formatter.CalcByteLengthForSerialization(options, value[0]);
            UInt32Formatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = UInt32Formatter.CalcByteLengthForSerialization(options, item);
                UInt32Formatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


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
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (uint*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 8 * capacity;
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
                    UnsafeUtility.MemCpy(dest, ptr, 8 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<uint>();
                }

                var answer = new uint[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
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
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
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
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt32();
                }

                if (count == 0)
                {
                    return Array.Empty<uint>();
                }

                var answer = new uint[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class UInt32NativeArrayFormatter : IJsonFormatter<NativeArray<uint>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<uint> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<uint> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
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
                UnsafeUtility.MemCpy(dst, ptr, count * 8);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class UInt32MemoryFormatter : IJsonFormatter<Memory<uint>>
    {
        public void Serialize(ref JsonWriter writer, Memory<uint> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
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
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (uint*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
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
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, uint>(array.AsSpan());
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

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, long? value)
        {
            if (value.HasValue)
            {
                return Int64Formatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, long? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            Int64Formatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class Int64ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<long[]?>
#else
        : IJsonFormatter<long[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, long[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, long[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += Int64Formatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, long[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, long[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = Int64Formatter.CalcByteLengthForSerialization(options, value[0]);
            Int64Formatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = Int64Formatter.CalcByteLengthForSerialization(options, item);
                Int64Formatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


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
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
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
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<long>();
                }

                var answer = new long[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
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
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, long>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<long>();
                }

                var answer = new long[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class Int64NativeArrayFormatter : IJsonFormatter<NativeArray<long>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<long> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<long> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ulong? value)
        {
            if (value.HasValue)
            {
                return UInt64Formatter.CalcByteLengthForSerialization(options, value.Value);
            }

            return 4;
        }

        public static void SerializeSpan(JsonSerializerOptions options, ulong? value, Span<byte> span)
        {
            if (!value.HasValue)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            UInt64Formatter.SerializeSpan(options, value.Value, span);
        }
    }

    public sealed unsafe class UInt64ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ulong[]?>
#else
        : IJsonFormatter<ulong[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ulong[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, ulong[] value)
#endif
        {
            if (value == null)
            {
                return 4;
            }

            if (value.Length == 0)
            {
                return 2;
            }

            var answer = 1 + value.Length;
            for (var index = 0; index < value.Length; index++)
            {
                answer += UInt64Formatter.CalcByteLengthForSerialization(options, value[index]);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, ulong[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, ulong[] value, Span<byte> span)
#endif
        {
            if (value == null)
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                return;
            }

            span[0] = (byte)'[';
            span[span.Length - 1] = (byte)']';
            span = span.Slice(1, span.Length - 2);
            if (value.Length == 0)
            {
                return;
            }

            var length = UInt64Formatter.CalcByteLengthForSerialization(options, value[0]);
            UInt64Formatter.SerializeSpan(options, value[0], span.Slice(0, length));
            for (var index = 1; index < value.Length; index++)
            {
                span[length] = (byte)',';
                span = span.Slice(length + 1);
                var item = value[index];
                length = UInt64Formatter.CalcByteLengthForSerialization(options, item);
                UInt64Formatter.SerializeSpan(options, item, span.Slice(0, length));
            }
        }


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
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (ulong*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 16 * capacity;
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
                    UnsafeUtility.MemCpy(dest, ptr, 16 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<ulong>();
                }

                var answer = new ulong[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 16;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
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
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
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
                    UnsafeUtility.MemCpy(dst, ptr, count * 16);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadUInt64();
                }

                if (count == 0)
                {
                    return Array.Empty<ulong>();
                }

                var answer = new ulong[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 16;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class UInt64NativeArrayFormatter : IJsonFormatter<NativeArray<ulong>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<ulong> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<ulong> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
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
                UnsafeUtility.MemCpy(dst, ptr, count * 16);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class UInt64MemoryFormatter : IJsonFormatter<Memory<ulong>>
    {
        public void Serialize(ref JsonWriter writer, Memory<ulong> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
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
                    UnsafeUtility.MemCpy(dst, ptr, count * 16);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
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
                        var size = count * 16;
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
        {
            return DeserializeStatic(ref reader, options);
        }

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
            var ptr = (ulong*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
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
            var array = ArrayPool<byte>.Shared.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, ulong>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
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


    public sealed class SingleFormatter : IJsonFormatter<float>
    {
        public static void SerializeStatic(ref JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static float DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSingle();
        }

        public float Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSingle();
        }
    }

    public sealed class NullableSingleFormatter : IJsonFormatter<float?>
    {
        public static void SerializeStatic(ref JsonWriter writer, float? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, float? value, JsonSerializerOptions options)
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

        public static float? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadSingle();
        }

        public float? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadSingle();
        }
    }

    public sealed unsafe class SingleArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<float[]?>
#else
        : IJsonFormatter<float[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, float[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, float[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, float[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, float[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static float[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static float[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (float*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 4 * capacity;
                        var tmp = (float*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadSingle();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<float>();
                }

                var answer = new float[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 4 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSingle();
                }

                if (count == 0)
                {
                    return Array.Empty<float>();
                }

                var answer = new float[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 4;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public float[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public float[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class SingleReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<float>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<float> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<float> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<float> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<float> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (float*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (float*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSingle();
            }

            float[] answer;
            if (count == 0)
            {
                answer = Array.Empty<float>();
            }
            else
            {
                answer = new float[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 4;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSingle();
                }

                if (count == 0)
                {
                    return Array.Empty<float>();
                }

                var answer = new float[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 4;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class SingleNativeArrayFormatter : IJsonFormatter<NativeArray<float>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<float> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<float> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<float> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<float> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (float*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (float*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSingle();
            }

            NativeArray<float> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<float>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 4);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class SingleMemoryFormatter : IJsonFormatter<Memory<float>>
    {
        public void Serialize(ref JsonWriter writer, Memory<float> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<float> value, JsonSerializerOptions options)
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

        public Memory<float> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<float> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (float*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (float*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSingle();
            }

            float[] answer;
            if (count == 0)
            {
                answer = Array.Empty<float>();
            }
            else
            {
                answer = new float[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 4);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSingle();
                }

                if (count == 0)
                {
                    return Array.Empty<float>();
                }
                else
                {
                    var answer = new float[count];
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

    public sealed class SingleListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<float>?>
#else
        : IOverwriteJsonFormatter<List<float>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<float>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<float> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<float>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<float> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<float>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<float> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<float>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<float> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (float*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 4 * capacity;
                    var tmp = (float*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadSingle();
            }

            var answer = new List<float>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 4);
            var span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, float>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadSingle();
                }

                var answer = new List<float>(count);
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
        public void DeserializeTo(ref List<float>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<float> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<float>();
                }

                value.Add(reader.ReadSingle());
            }
        }
    }


    public sealed class DoubleFormatter : IJsonFormatter<double>
    {
        public static void SerializeStatic(ref JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static double DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDouble();
        }

        public double Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDouble();
        }
    }

    public sealed class NullableDoubleFormatter : IJsonFormatter<double?>
    {
        public static void SerializeStatic(ref JsonWriter writer, double? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, double? value, JsonSerializerOptions options)
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

        public static double? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadDouble();
        }

        public double? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadDouble();
        }
    }

    public sealed unsafe class DoubleArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<double[]?>
#else
        : IJsonFormatter<double[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, double[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, double[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, double[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, double[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static double[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static double[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (double*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 8 * capacity;
                        var tmp = (double*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadDouble();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<double>();
                }

                var answer = new double[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 8 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDouble();
                }

                if (count == 0)
                {
                    return Array.Empty<double>();
                }

                var answer = new double[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public double[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public double[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class DoubleReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<double>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<double> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<double> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<double> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<double> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (double*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (double*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDouble();
            }

            double[] answer;
            if (count == 0)
            {
                answer = Array.Empty<double>();
            }
            else
            {
                answer = new double[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDouble();
                }

                if (count == 0)
                {
                    return Array.Empty<double>();
                }

                var answer = new double[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class DoubleNativeArrayFormatter : IJsonFormatter<NativeArray<double>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<double> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<double> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<double> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<double> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (double*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (double*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDouble();
            }

            NativeArray<double> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<double>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 8);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class DoubleMemoryFormatter : IJsonFormatter<Memory<double>>
    {
        public void Serialize(ref JsonWriter writer, Memory<double> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<double> value, JsonSerializerOptions options)
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

        public Memory<double> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<double> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (double*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (double*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDouble();
            }

            double[] answer;
            if (count == 0)
            {
                answer = Array.Empty<double>();
            }
            else
            {
                answer = new double[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDouble();
                }

                if (count == 0)
                {
                    return Array.Empty<double>();
                }
                else
                {
                    var answer = new double[count];
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

    public sealed class DoubleListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<double>?>
#else
        : IOverwriteJsonFormatter<List<double>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<double>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<double> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<double>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<double> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<double>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<double> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<double>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<double> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (double*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (double*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDouble();
            }

            var answer = new List<double>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, double>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDouble();
                }

                var answer = new List<double>(count);
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
        public void DeserializeTo(ref List<double>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<double> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<double>();
                }

                value.Add(reader.ReadDouble());
            }
        }
    }


    public sealed class DateTimeFormatter : IJsonFormatter<DateTime>
    {
        public static void SerializeStatic(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static DateTime DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDateTime();
        }

        public DateTime Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDateTime();
        }
    }

    public sealed class NullableDateTimeFormatter : IJsonFormatter<DateTime?>
    {
        public static void SerializeStatic(ref JsonWriter writer, DateTime? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, DateTime? value, JsonSerializerOptions options)
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

        public static DateTime? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadDateTime();
        }

        public DateTime? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadDateTime();
        }
    }

    public sealed unsafe class DateTimeArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<DateTime[]?>
#else
        : IJsonFormatter<DateTime[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, DateTime[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, DateTime[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, DateTime[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, DateTime[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static DateTime[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static DateTime[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (DateTime*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 8 * capacity;
                        var tmp = (DateTime*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadDateTime();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<DateTime>();
                }

                var answer = new DateTime[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 8 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDateTime();
                }

                if (count == 0)
                {
                    return Array.Empty<DateTime>();
                }

                var answer = new DateTime[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public DateTime[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public DateTime[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class DateTimeReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<DateTime>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<DateTime> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<DateTime> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<DateTime> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<DateTime> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (DateTime*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (DateTime*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDateTime();
            }

            DateTime[] answer;
            if (count == 0)
            {
                answer = Array.Empty<DateTime>();
            }
            else
            {
                answer = new DateTime[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 8;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDateTime();
                }

                if (count == 0)
                {
                    return Array.Empty<DateTime>();
                }

                var answer = new DateTime[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 8;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class DateTimeNativeArrayFormatter : IJsonFormatter<NativeArray<DateTime>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<DateTime> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<DateTime> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<DateTime> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<DateTime> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (DateTime*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (DateTime*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDateTime();
            }

            NativeArray<DateTime> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<DateTime>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 8);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class DateTimeMemoryFormatter : IJsonFormatter<Memory<DateTime>>
    {
        public void Serialize(ref JsonWriter writer, Memory<DateTime> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<DateTime> value, JsonSerializerOptions options)
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

        public Memory<DateTime> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<DateTime> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (DateTime*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (DateTime*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDateTime();
            }

            DateTime[] answer;
            if (count == 0)
            {
                answer = Array.Empty<DateTime>();
            }
            else
            {
                answer = new DateTime[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 8);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDateTime();
                }

                if (count == 0)
                {
                    return Array.Empty<DateTime>();
                }
                else
                {
                    var answer = new DateTime[count];
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

    public sealed class DateTimeListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<DateTime>?>
#else
        : IOverwriteJsonFormatter<List<DateTime>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<DateTime>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<DateTime> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<DateTime>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<DateTime> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<DateTime>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<DateTime> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<DateTime>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<DateTime> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (DateTime*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 8 * capacity;
                    var tmp = (DateTime*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDateTime();
            }

            var answer = new List<DateTime>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 8);
            var span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, DateTime>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDateTime();
                }

                var answer = new List<DateTime>(count);
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
        public void DeserializeTo(ref List<DateTime>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<DateTime> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<DateTime>();
                }

                value.Add(reader.ReadDateTime());
            }
        }
    }


    public sealed class DecimalFormatter : IJsonFormatter<decimal>
    {
        public static void SerializeStatic(ref JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static decimal DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDecimal();
        }

        public decimal Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDecimal();
        }
    }

    public sealed class NullableDecimalFormatter : IJsonFormatter<decimal?>
    {
        public static void SerializeStatic(ref JsonWriter writer, decimal? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, decimal? value, JsonSerializerOptions options)
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

        public static decimal? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadDecimal();
        }

        public decimal? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadDecimal();
        }
    }

    public sealed unsafe class DecimalArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<decimal[]?>
#else
        : IJsonFormatter<decimal[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, decimal[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, decimal[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, decimal[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, decimal[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }
        
#if CSHARP_8_OR_NEWER
        public static decimal[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static decimal[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (decimal*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
                var capacity = 32;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        long size = 16 * capacity;
                        var tmp = (decimal*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                        UnsafeUtility.MemCpy(tmp, ptr, size);
                        capacity <<= 1;
                        UnsafeUtility.Free(ptr, allocator);
                        ptr = tmp;
                    }
                    
                    ptr[count - 1] = reader.ReadDecimal();
                }

                if (count == 0)
                {
                    UnsafeUtility.Free(ptr, allocator);
                    return Array.Empty<decimal>();
                }

                var answer = new decimal[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 16 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
            }
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDecimal();
                }

                if (count == 0)
                {
                    return Array.Empty<decimal>();
                }

                var answer = new decimal[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 16;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }

#if CSHARP_8_OR_NEWER
        public decimal[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public decimal[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class DecimalReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<decimal>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<decimal> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<decimal> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<decimal> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe ReadOnlyMemory<decimal> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (decimal*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
                    var tmp = (decimal*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDecimal();
            }

            decimal[] answer;
            if (count == 0)
            {
                answer = Array.Empty<decimal>();
            }
            else
            {
                answer = new decimal[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 16);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDecimal();
                }

                if (count == 0)
                {
                    return Array.Empty<decimal>();
                }

                var answer = new decimal[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count * 16;
                    Buffer.MemoryCopy(src, dst, size, size);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
#endif
        }
    }

#if UNITY_2018_4_OR_NEWER
    public sealed class DecimalNativeArrayFormatter : IJsonFormatter<NativeArray<decimal>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<decimal> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<decimal> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<decimal> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<decimal> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (decimal*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
                    var tmp = (decimal*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDecimal();
            }

            NativeArray<decimal> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<decimal>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count * 16);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class DecimalMemoryFormatter : IJsonFormatter<Memory<decimal>>
    {
        public void Serialize(ref JsonWriter writer, Memory<decimal> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<decimal> value, JsonSerializerOptions options)
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

        public Memory<decimal> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static unsafe Memory<decimal> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (decimal*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
                    var tmp = (decimal*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDecimal();
            }

            decimal[] answer;
            if (count == 0)
            {
                answer = Array.Empty<decimal>();
            }
            else
            {
                answer = new decimal[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count * 16);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDecimal();
                }

                if (count == 0)
                {
                    return Array.Empty<decimal>();
                }
                else
                {
                    var answer = new decimal[count];
                    fixed (void* dst = &answer[0])
                    fixed (void* src = &array[0])
                    {
                        var size = count * 16;
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

    public sealed class DecimalListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<decimal>?>
#else
        : IOverwriteJsonFormatter<List<decimal>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<decimal>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<decimal> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<decimal>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<decimal> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<decimal>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<decimal> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static unsafe List<decimal>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<decimal> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (decimal*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
            var capacity = 32;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    long size = 16 * capacity;
                    var tmp = (decimal*)UnsafeUtility.Malloc(size << 1, 4, allocator);
                    UnsafeUtility.MemCpy(tmp, ptr, size);
                    capacity <<= 1;
                    UnsafeUtility.Free(ptr, allocator);
                    ptr = tmp;
                }

                ptr[count - 1] = reader.ReadDecimal();
            }

            var answer = new List<decimal>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256 * 16);
            var span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length * 16;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, decimal>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadDecimal();
                }

                var answer = new List<decimal>(count);
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
        public void DeserializeTo(ref List<decimal>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<decimal> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<decimal>();
                }

                value.Add(reader.ReadDecimal());
            }
        }
    }

}
