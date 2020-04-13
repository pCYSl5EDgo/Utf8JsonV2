// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/* THIS (.cs) FILE IS GENERATED. DO NOT CHANGE IT.
 * CHANGE THE .tt FILE INSTEAD. */

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#endif

// ReSharper disable StackAllocInsideLoop
#pragma warning disable SA1649
#pragma warning disable IDE0060

namespace Utf8Json.Formatters
{
    public sealed class Int16Formatter : IJsonFormatter<short>
    {
        public static void SerializeStatic(ref JsonWriter writer, short value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, short value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static short DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt16();
        }

        public short Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadInt16();
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
    }

    public sealed class Int16ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<short[]?>
#else
        : IJsonFormatter<short[]>
#endif
    {
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (short*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
                var count = 0;
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
            Span<short> span = stackalloc short[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<short> tmp = stackalloc short[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadInt16();
            }

            return count == 0 ? Array.Empty<short>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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

#if UNITY_2018_4_OR_NEWER
    public sealed class Int16NativeArrayFormatter : IJsonFormatter<NativeArray<short>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<short> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<short> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
            => DeserializeStatic(ref reader, options);

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


    public sealed class UInt16Formatter : IJsonFormatter<ushort>
    {
        public static void SerializeStatic(ref JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static ushort DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt16();
        }

        public ushort Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUInt16();
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
    }

    public sealed class UInt16ArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ushort[]?>
#else
        : IJsonFormatter<ushort[]>
#endif
    {
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (ushort*)UnsafeUtility.Malloc(32 * 2, 4, allocator);
                var count = 0;
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
            Span<ushort> span = stackalloc ushort[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<ushort> tmp = stackalloc ushort[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadUInt16();
            }

            return count == 0 ? Array.Empty<ushort>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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

#if UNITY_2018_4_OR_NEWER
    public sealed class UInt16NativeArrayFormatter : IJsonFormatter<NativeArray<ushort>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<ushort> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<ushort> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
            => DeserializeStatic(ref reader, options);

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

    public sealed class SingleArrayFormatter
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (float*)UnsafeUtility.Malloc(32 * 4, 4, allocator);
                var count = 0;
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
            Span<float> span = stackalloc float[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<float> tmp = stackalloc float[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadSingle();
            }

            return count == 0 ? Array.Empty<float>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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

#if UNITY_2018_4_OR_NEWER
    public sealed class SingleNativeArrayFormatter : IJsonFormatter<NativeArray<float>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<float> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<float> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
            => DeserializeStatic(ref reader, options);

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

    public sealed class DoubleArrayFormatter
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (double*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var count = 0;
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
            Span<double> span = stackalloc double[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<double> tmp = stackalloc double[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadDouble();
            }

            return count == 0 ? Array.Empty<double>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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

#if UNITY_2018_4_OR_NEWER
    public sealed class DoubleNativeArrayFormatter : IJsonFormatter<NativeArray<double>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<double> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<double> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
            => DeserializeStatic(ref reader, options);

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


    public sealed class ByteFormatter : IJsonFormatter<byte>
    {
        public static void SerializeStatic(ref JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static byte DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadByte();
        }

        public byte Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadByte();
        }
    }

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
    }

    public sealed class ByteArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<byte[]?>
#else
        : IJsonFormatter<byte[]>
#endif
    {
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (byte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
                var count = 0;
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
            Span<byte> span = stackalloc byte[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<byte> tmp = stackalloc byte[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadByte();
            }

            return count == 0 ? Array.Empty<byte>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
                    UnsafeUtility.MemCpy(dst, ptr, count);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = array.AsSpan();
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = array.AsSpan();
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
                        var size = count;
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
    public sealed class ByteNativeArrayFormatter : IJsonFormatter<NativeArray<byte>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<byte> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<byte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
                UnsafeUtility.MemCpy(dst, ptr, count);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class ByteMemoryFormatter : IJsonFormatter<Memory<byte>>
    {
        public void Serialize(ref JsonWriter writer, Memory<byte> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
                    UnsafeUtility.MemCpy(dst, ptr, count);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = array.AsSpan();
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = array.AsSpan();
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
                        var size = count;
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
            => DeserializeStatic(ref reader, options);

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
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = array.AsSpan();
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
                        var tmp = ArrayPool<byte>.Shared.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        ArrayPool<byte>.Shared.Return(array);
                        array = tmp;
                        span = array.AsSpan();
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


    public sealed class SByteFormatter : IJsonFormatter<sbyte>
    {
        public static void SerializeStatic(ref JsonWriter writer, sbyte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, sbyte value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static sbyte DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSByte();
        }

        public sbyte Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSByte();
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
    }

    public sealed class SByteArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<sbyte[]?>
#else
        : IJsonFormatter<sbyte[]>
#endif
    {
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (sbyte*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
                var count = 0;
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
            Span<sbyte> span = stackalloc sbyte[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<sbyte> tmp = stackalloc sbyte[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadSByte();
            }

            return count == 0 ? Array.Empty<sbyte>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
                    UnsafeUtility.MemCpy(dst, ptr, count);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
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
                        var size = count;
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
    public sealed class SByteNativeArrayFormatter : IJsonFormatter<NativeArray<sbyte>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<sbyte> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<sbyte> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
                UnsafeUtility.MemCpy(dst, ptr, count);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class SByteMemoryFormatter : IJsonFormatter<Memory<sbyte>>
    {
        public void Serialize(ref JsonWriter writer, Memory<sbyte> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
                    UnsafeUtility.MemCpy(dst, ptr, count);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
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
                        var size = count;
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
            => DeserializeStatic(ref reader, options);

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
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = MemoryMarshal.Cast<byte, sbyte>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
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

    public sealed class DateTimeArrayFormatter
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (DateTime*)UnsafeUtility.Malloc(32 * 8, 4, allocator);
                var count = 0;
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
            Span<DateTime> span = stackalloc DateTime[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<DateTime> tmp = stackalloc DateTime[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadDateTime();
            }

            return count == 0 ? Array.Empty<DateTime>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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

#if UNITY_2018_4_OR_NEWER
    public sealed class DateTimeNativeArrayFormatter : IJsonFormatter<NativeArray<DateTime>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<DateTime> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<DateTime> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
            => DeserializeStatic(ref reader, options);

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

    public sealed class DecimalArrayFormatter
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
#if UNITY_2018_4_OR_NEWER
            unsafe
            {
                const Allocator allocator = Allocator.Temp;
                var ptr = (decimal*)UnsafeUtility.Malloc(32 * 16, 4, allocator);
                var count = 0;
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
            Span<decimal> span = stackalloc decimal[16];
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (span.Length < count)
                {
                    Span<decimal> tmp = stackalloc decimal[span.Length << 1];
                    span.CopyTo(tmp);
                    span = tmp;
                }
                    
                span[count - 1] = reader.ReadDecimal();
            }

            return count == 0 ? Array.Empty<decimal>() : span.Slice(0, count).ToArray();
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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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

#if UNITY_2018_4_OR_NEWER
    public sealed class DecimalNativeArrayFormatter : IJsonFormatter<NativeArray<decimal>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<decimal> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<decimal> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

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
            => SerializeStatic(ref writer, value, options);

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
            => DeserializeStatic(ref reader, options);

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
            => DeserializeStatic(ref reader, options);

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
