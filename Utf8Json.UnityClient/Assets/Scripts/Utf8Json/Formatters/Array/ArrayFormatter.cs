// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Buffers;

namespace Utf8Json.Formatters
{
    public sealed unsafe class AddArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<T[]?>
#else
        : IOverwriteJsonFormatter<T[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[] value, JsonSerializerOptions options)
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

            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte) '[';
                writer.Writer.Advance(1);
            }
            if (value.Length == 0)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)']';
                writer.Writer.Advance(1);
                return;
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                SerializeWithFormatter(ref writer, value, options);
            }
            else
            {
                writer.Serialize(value[0], options, serializer);
                for (var i = 1; i < value.Length; i++)
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                    writer.Serialize(value[i], options, serializer);
                }
            }

            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)']';
            writer.Writer.Advance(1);
        }

        private static void SerializeWithFormatter(ref JsonWriter writer, T[] value, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value[0], options);

            for (var i = 1; i < value.Length; i++)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
                formatter.Serialize(ref writer, value[i], options);
            }
        }

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public T[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public T[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public static T[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public static T[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(256);
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            try
            {
                if (deserializer.ToPointer() == null)
                {
                    return DeserializeWithFormatter(ref reader, options, pool, ref array);
                }

                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (array.Length < count)
                    {
                        var tmp = pool.Rent(count << 1);
                        Array.Copy(array, 0, tmp, 0, array.Length);
                        pool.Return(array);
                        array = tmp;
                    }

                    array[count - 1] = reader.Deserialize<T>(options, deserializer);
                }

                var answer = new T[count];
                Array.Copy(array, answer, count);
                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }

        private static T[] DeserializeWithFormatter(ref JsonReader reader, JsonSerializerOptions options, ArrayPool<T> pool, ref T[] array)
        {
            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (array.Length < count)
                {
                    var tmp = pool.Rent(count << 1);
                    Array.Copy(array, 0, tmp, 0, array.Length);
                    pool.Return(array);
                    array = tmp;
                }

                array[count - 1] = formatter.Deserialize(ref reader, options);
            }

            var answer = new T[count];
            Array.Copy(array, answer, count);
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref T[]? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref T[] value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return;
            }

            reader.ReadIsBeginArrayWithVerify();
            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(64);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                if (deserializer.ToPointer() == null)
                {
                    DeserializeToWithFormatter(ref value, ref reader, options, ref array, pool);
                }
                else
                {
                    var count = 0;
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (array.Length < count)
                        {
                            var tmp = pool.Rent(count << 1);
                            Array.Copy(array, tmp, array.Length);
                            pool.Return(array);
                            array = tmp;
                        }

                        array[count - 1] = reader.Deserialize<T>(options, deserializer);
                    }

                    if (count == 0)
                    {
                        return;
                    }
                    if (value == null)
                    {
                        value = new T[count];
                        Array.Copy(array, 0, value, value.Length, count);
                    }
                    else
                    {
                        var result = new T[value.Length + count];
                        Array.Copy(value, 0, result, 0, value.Length);
                        Array.Copy(array, 0, result, value.Length, count);
                        value = result;
                    }
                }
            }
            finally
            {
                pool.Return(array);
            }
        }

#if CSHARP_8_OR_NEWER
        private static void DeserializeToWithFormatter(ref T[]? value, ref JsonReader reader, JsonSerializerOptions options, ref T[] array, ArrayPool<T> pool)
#else
        private static void DeserializeToWithFormatter(ref T[] value, ref JsonReader reader, JsonSerializerOptions options, ref T[] array, ArrayPool<T> pool)
#endif
        {
            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (array.Length < count)
                {
                    var tmp = pool.Rent(count << 1);
                    Array.Copy(array, tmp, array.Length);
                    pool.Return(array);
                    array = tmp;
                }

                array[count - 1] = formatter.Deserialize(ref reader, options);
            }

            if (count == 0)
            {
                return;
            }

            if (value == null)
            {
                value = new T[count];
                Array.Copy(array, 0, value, value.Length, count);
            }
            else
            {
                var result = new T[value.Length + count];
                Array.Copy(value, 0, result, 0, value.Length);
                Array.Copy(array, 0, result, value.Length, count);
                value = result;
            }
        }
    }

    public sealed unsafe class OverwriteArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<T[]?>
#else
        : IOverwriteJsonFormatter<T[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[] value, JsonSerializerOptions options)
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

            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)'[';
            writer.Writer.Advance(1);
            if (value.Length == 0)
            {
                goto END;
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                SerializeWithFormatter(ref writer, value, options);
            }
            else
            {
                writer.Serialize(value[0], options, serializer);

                for (var i = 1; i < value.Length; i++)
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                    writer.Serialize(value[i], options, serializer);
                }
            }
        END:
        var span2 = writer.Writer.GetSpan(1);
        span2[0] = (byte)']';
        writer.Writer.Advance(1);
        }

        private static void SerializeWithFormatter(ref JsonWriter writer, T[] value, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value[0], options);

            for (var i = 1; i < value.Length; i++)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
                formatter.Serialize(ref writer, value[i], options);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(256);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                if (deserializer.ToPointer() == null)
                {
                    return DeserializeWithFormatter(ref reader, options, pool, ref array);
                }

                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (array.Length < count)
                    {
                        var tmp = pool.Rent(count << 1);
                        Array.Copy(array, 0, tmp, 0, array.Length);
                        pool.Return(array);
                        array = tmp;
                    }

                    array[count - 1] = reader.Deserialize<T>(options, deserializer);
                }

                var answer = new T[count];
                Array.Copy(array, answer, count);
                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }

        private static T[] DeserializeWithFormatter(ref JsonReader reader, JsonSerializerOptions options, ArrayPool<T> pool, ref T[] array)
        {
            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (array.Length < count)
                {
                    var tmp = pool.Rent(count << 1);
                    Array.Copy(array, 0, tmp, 0, array.Length);
                    pool.Return(array);
                    array = tmp;
                }

                array[count - 1] = formatter.Deserialize(ref reader, options);
            }

            var answer = new T[count];
            Array.Copy(array, answer, count);
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref T[]? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref T[] value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return;
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            if (value == null)
            {
                value = Array.Empty<T>();
            }

            var deserializer = options.Resolver.GetDeserializeStatic<T>();

            if (deserializer.ToPointer() == null)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (value.Length < count)
                    {
                        Array.Resize(ref value, value.Length << 1);
                    }

                    value[count - 1] = formatter.Deserialize(ref reader, options);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (value.Length < count)
                    {
                        Array.Resize(ref value, value.Length << 1);
                    }

                    value[count - 1] = reader.Deserialize<T>(options, deserializer);
                }
            }

            Array.Resize(ref value, count); // resize, fit length.
        }
    }
}
