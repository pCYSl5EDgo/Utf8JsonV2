// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Buffers;

namespace Utf8Json.Formatters
{
    public sealed class ArraySegmentFormatter<T> : IJsonFormatter<ArraySegment<T>>
    {
        public void Serialize(ref JsonWriter writer, ArraySegment<T> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static unsafe void SerializeStatic(ref JsonWriter writer, ArraySegment<T> value, JsonSerializerOptions options)
        {
            if (value.Array == null)
            {
                var span1 = writer.Writer.GetSpan(4);
                span1[0] = (byte)'n';
                span1[1] = (byte)'u';
                span1[2] = (byte)'l';
                span1[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var span = value.AsSpan();

            var span2 = writer.Writer.GetSpan(1);
            span2[0] = (byte)'[';
            writer.Writer.Advance(1);
            if (span.IsEmpty)
            {
                goto END;
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, span[0], options);

                for (var i = 1; i < span.Length; i++)
                {
                    var span1 = writer.Writer.GetSpan(1);
                    span1[0] = (byte)',';
                    writer.Writer.Advance(1);
                    formatter.Serialize(ref writer, span[i], options);
                }
            }
            else
            {
                writer.Serialize(span[0], options, serializer);

                for (var i = 1; i < span.Length; i++)
                {
                    var span1 = writer.Writer.GetSpan(1);
                    span1[0] = (byte)',';
                    writer.Writer.Advance(1);
                    writer.Serialize(span[i], options, serializer);
                }
            }

        END:
        var span3 = writer.Writer.GetSpan(1);
        span3[0] = (byte)']';
        writer.Writer.Advance(1);
        }

        public ArraySegment<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static unsafe ArraySegment<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(64);
            var count = 0;
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                if (deserializer.ToPointer() == null)
                {
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
                }
                else
                {
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
                }
                var answer = new T[count];
                Array.Copy(array, answer, count);
                return new ArraySegment<T>(answer, 0, answer.Length);
            }
            finally
            {
                pool.Return(array);
            }
        }
    }

    public sealed class NullableArraySegmentFormatter<T> : IJsonFormatter<ArraySegment<T>?>
    {
        public static void SerializeStatic(ref JsonWriter writer, ArraySegment<T>? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ArraySegmentFormatter<T>.SerializeStatic(ref writer, value.Value, options);
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

        public void Serialize(ref JsonWriter writer, ArraySegment<T>? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ArraySegmentFormatter<T>.SerializeStatic(ref writer, value.Value, options);
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

        public static ArraySegment<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default(ArraySegment<T>?) : ArraySegmentFormatter<T>.DeserializeStatic(ref reader, options);
        }

        public ArraySegment<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default(ArraySegment<T>?) : ArraySegmentFormatter<T>.DeserializeStatic(ref reader, options);
        }
    }
}