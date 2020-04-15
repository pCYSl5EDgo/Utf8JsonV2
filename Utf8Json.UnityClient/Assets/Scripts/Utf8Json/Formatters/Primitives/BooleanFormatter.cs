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
// ReSharper disable UseIndexFromEndExpression
#endif

namespace Utf8Json.Formatters
{
    public sealed class BooleanFormatter : IJsonFormatter<bool>
    {
        public static void SerializeStatic(ref JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            if (value)
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

        public void Serialize(ref JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            if (value)
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
            if (value)
            {
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
            }
            else
            {
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
            }
        }
    }

    public sealed class NullableBooleanFormatter : IJsonFormatter<bool?>
    {
        public static void SerializeStatic(ref JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                if (value.Value)
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
            else
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
            }
        }

        public void Serialize(ref JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                if (value.Value)
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
            else
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
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

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, bool? value)
        {
            if (!value.HasValue)
            {
                return 4;
            }

            if (value.Value)
            {
                return 4;
            }

            return 5;
        }

        public static void SerializeSpan(JsonSerializerOptions options, bool? value, Span<byte> span)
        {
            if (span.Length == 5)
            {
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
                return;
            }

            if (value.HasValue)
            {
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
            }
            else
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
            }
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
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
            }
            else
            {
                var span2 = writer.Writer.GetSpan(1);
                span2[0] = (byte)'[';
                writer.Writer.Advance(1);
                if (value.Length != 0)
                {
                    if (value[0])
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
                    for (int i = 1; i < value.Length; i++)
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
                }

                var span3 = writer.Writer.GetSpan(1);
                span3[0] = (byte)']';
                writer.Writer.Advance(1);
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
        public static unsafe bool[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static unsafe bool[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
#else
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(256);
            var span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (span.Length < count)
                    {
                        var size = span.Length;
                        var tmp = pool.Rent(size << 1);
                        fixed (byte* src = &array[0])
                        fixed (byte* dest = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dest, tmp.LongLength, size);
                        }

                        pool.Return(array);
                        array = tmp;
                        span = MemoryMarshal.Cast<byte, bool>(array.AsSpan());
                    }

                    span[count - 1] = reader.ReadBoolean();
                }

                if (count == 0)
                {
                    return Array.Empty<bool>();
                }

                var answer = new bool[count];
                fixed (void* dst = &answer[0])
                fixed (void* src = &array[0])
                {
                    var size = count;
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
        public bool[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public bool[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, bool[]? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, bool[] value)
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

            var answer = 1 + value.Length * 5;

            for (var i = 0; i < value.Length; i++)
            {
                if (!value[i])
                {
                    answer++;
                }
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, bool[]? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, bool[] value, Span<byte> span)
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
            if (span.IsEmpty)
            {
                return;
            }

            if (value[0])
            {
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
                span = span.Slice(4);
            }
            else
            {
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
                span = span.Slice(5);
            }

            for (var index = 1; index < value.Length; index++)
            {
                span[0] = (byte)',';
                if (value[index])
                {
                    span[1] = (byte)'t';
                    span[2] = (byte)'r';
                    span[3] = (byte)'u';
                    span[4] = (byte)'e';
                    span = span.Slice(5);
                }
                else
                {
                    span[1] = (byte)'f';
                    span[2] = (byte)'a';
                    span[3] = (byte)'l';
                    span[4] = (byte)'s';
                    span[5] = (byte)'e';
                    span = span.Slice(6);
                }
            }
        }
    }

    public sealed class BooleanReadOnlyMemoryFormatter : IJsonFormatter<ReadOnlyMemory<bool>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyMemory<bool> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyMemory<bool> value, JsonSerializerOptions options)
        {
            var span4 = writer.Writer.GetSpan(1);
            span4[0] = (byte)'[';
            writer.Writer.Advance(1);
            if (value.Length == 0)
            {
                goto END;
            }

            var span = value.Span;
            if (span[0])
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

            for (var i = 1; i < span.Length; i++)
            {
                var span3 = writer.Writer.GetSpan(1);
                span3[0] = (byte)',';
                writer.Writer.Advance(1);
                if (span[i])
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
            }

        END:
        var span5 = writer.Writer.GetSpan(1);
        span5[0] = (byte)']';
        writer.Writer.Advance(1);
        }

        public ReadOnlyMemory<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Memory<bool> value, JsonSerializerOptions options)
        {
            var span4 = writer.Writer.GetSpan(1);
            span4[0] = (byte)'[';
            writer.Writer.Advance(1);
            if (value.Length == 0)
            {
                goto END;
            }

            var span = value.Span;
            if (span[0])
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

            for (var i = 1; i < span.Length; i++)
            {
                var span3 = writer.Writer.GetSpan(1);
                span3[0] = (byte)',';
                writer.Writer.Advance(1);
                if (span[i])
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
            }

        END:
        var span5 = writer.Writer.GetSpan(1);
        span5[0] = (byte)']';
        writer.Writer.Advance(1);
        }

        public Memory<bool> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

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
            if (value.Count == 0)
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

            for (var i = 1; i < value.Count; i++)
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
        {
            return DeserializeStatic(ref reader, options);
        }

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
