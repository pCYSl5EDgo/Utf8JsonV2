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
    public sealed class BooleanFormatter : IJsonFormatter<bool>
    {
        public static void SerializeStatic(ref JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static bool DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadBoolean();
        }

        public bool Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadBoolean();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, bool value)
        {
            return value ? 4 : 5;
        }

        public static void SerializeSpan(JsonSerializerOptions options, bool value, Span<byte> span)
        {
            span[span.Length - 1] = (byte)'e';
            if (value)
            {
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
            }
            else
            {
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
            }
        }
    }

    public sealed class NullableBooleanFormatter : IJsonFormatter<bool?>
    {
        public static void SerializeStatic(ref JsonWriter writer, bool? value, JsonSerializerOptions options)
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

        public void Serialize(ref JsonWriter writer, bool? value, JsonSerializerOptions options)
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

        public static bool? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadBoolean();
        }

        public bool? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : reader.ReadBoolean();
        }
    }

    public sealed class BooleanArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<bool[]?>
#else
        : IJsonFormatter<bool[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, bool[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, bool[] value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, bool[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, bool[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static bool[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static bool[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                    return Array.Empty<bool>();
                }

                var answer = new bool[count];
                fixed (void* dest = &answer[0])
                {
                    UnsafeUtility.MemCpy(dest, ptr, 1 * count);
                }

                UnsafeUtility.Free(ptr, allocator);
                return answer;
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

            return count == 0 ? Array.Empty<bool>() : span.Slice(0, count).ToArray();
#endif
        }

#if CSHARP_8_OR_NEWER
        public bool[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public bool[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class BooleanReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<bool>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<bool> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<bool> value, JsonSerializerOptions options)
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

        public ReadOnlyMemory<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe ReadOnlyMemory<bool> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (bool*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
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

            bool[] answer;
            if (count == 0)
            {
                answer = Array.Empty<bool>();
            }
            else
            {
                answer = new bool[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadBoolean();
                }

                if (count == 0)
                {
                    return Array.Empty<bool>();
                }
                else
                {
                    var answer = new bool[count];
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
    public sealed class BooleanNativeArrayFormatter : IJsonFormatter<NativeArray<bool>>
    {
        public void Serialize(ref JsonWriter writer, NativeArray<bool> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public NativeArray<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<bool> value, JsonSerializerOptions options)
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

        public static unsafe NativeArray<bool> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
            const Allocator allocator = Allocator.Temp;
            var ptr = (bool*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
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

            NativeArray<bool> answer;
            if (count == 0)
            {
                answer = default;
            }
            else
            {
                answer = new NativeArray<bool>(count, Allocator.Persistent);
                var dst = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
                UnsafeUtility.MemCpy(dst, ptr, count);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
        }
    }
#endif

    public sealed class BooleanMemoryFormatter : IJsonFormatter<Memory<bool>>
    {
        public void Serialize(ref JsonWriter writer, Memory<bool> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, Memory<bool> value, JsonSerializerOptions options)
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

        public Memory<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe Memory<bool> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var count = 0;
#if UNITY_2018_4_OR_NEWER
            const Allocator allocator = Allocator.Temp;
            var ptr = (bool*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
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

            bool[] answer;
            if (count == 0)
            {
                answer = Array.Empty<bool>();
            }
            else
            {
                answer = new bool[count];
                fixed (void* dst = &answer[0])
                {
                    UnsafeUtility.MemCpy(dst, ptr, count);
                }
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadBoolean();
                }

                if (count == 0)
                {
                    return Array.Empty<bool>();
                }
                else
                {
                    var answer = new bool[count];
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

    public sealed class BooleanListFormatter
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<bool>?>
#else
        : IOverwriteJsonFormatter<List<bool>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<bool>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<bool> value, JsonSerializerOptions options)
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
        public void Serialize(ref JsonWriter writer, List<bool>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<bool> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public List<bool>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
            => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
        public static unsafe List<bool>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe List<bool> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
            var ptr = (bool*)UnsafeUtility.Malloc(32 * 1, 4, allocator);
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

            var answer = new List<bool>(count);
            for (var i = 0; i < count; i++)
            {
                answer.Add(ptr[i]);
            }

            UnsafeUtility.Free(ptr, allocator);
            return answer;
#else
            var array = ArrayPool<byte>.Shared.Rent(256);
            var span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
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
                        span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadBoolean();
                }

                var answer = new List<bool>(count);
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
        public void DeserializeTo(ref List<bool>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<bool> value, ref JsonReader reader, JsonSerializerOptions options)
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
                    value = new List<bool>();
                }

                value.Add(reader.ReadBoolean());
            }
        }
    }
}
