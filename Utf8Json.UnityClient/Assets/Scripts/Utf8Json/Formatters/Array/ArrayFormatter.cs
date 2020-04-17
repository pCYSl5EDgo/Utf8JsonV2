// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Buffers;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[]?>
#else
        : IJsonFormatter<T[]>
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
                span[0] = (byte)'[';
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
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as T[], options);
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
