// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;

#if !ENABLE_IL2CPP
using StaticFunctionPointerHelper;
#endif

namespace Utf8Json.Formatters
{
    public sealed class ArraySegmentFormatter<T> : IJsonFormatter<ArraySegment<T>>
    {
        public void Serialize(ref JsonWriter writer, ArraySegment<T> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ArraySegment<T> value, JsonSerializerOptions options)
        {
            ref var bufferWriter = ref writer.Writer;
            if (value.Array == null)
            {
                var span1 = bufferWriter.GetSpan(4);
                span1[0] = (byte)'n';
                span1[1] = (byte)'u';
                span1[2] = (byte)'l';
                span1[3] = (byte)'l';
                bufferWriter.Advance(4);
                return;
            }

            if (writer.Depth >= options.MaxDepth)
            {
                bufferWriter.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            {
                var span1 = bufferWriter.GetSpan(1);
                span1[0] = (byte)'[';
                bufferWriter.Advance(1);
            }

            var span = value.AsSpan();
            if (span.IsEmpty)
            {
                goto END;
            }

#if !ENABLE_IL2CPP
            var serializer = options.Resolver.GetSerializeStatic<T>();
            unsafe
            {
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(span[0], options, serializer);

                    for (var i = 1; i < span.Length; i++)
                    {
                        var span1 = bufferWriter.GetSpan(1);
                        span1[0] = (byte)',';
                        bufferWriter.Advance(1);
                        writer.Serialize(span[i], options, serializer);
                    }
                    goto END;
                }
            }
#endif

            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, span[0], options);

            for (var i = 1; i < span.Length; i++)
            {
                var span1 = bufferWriter.GetSpan(1);
                span1[0] = (byte)',';
                bufferWriter.Advance(1);
                formatter.Serialize(ref writer, span[i], options);
            }

        END:
            {
                var span1 = bufferWriter.GetSpan(1);
                span1[0] = (byte)']';
                bufferWriter.Advance(1);
            }
            --writer.Depth;
        }

        public ArraySegment<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ArraySegment<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
#if !ENABLE_IL2CPP
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                unsafe
                {
                    if (deserializer.ToPointer() != null)
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

                        goto END;
                    }
                }
#endif

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
#if !ENABLE_IL2CPP
            END:
#endif
                var answer = new T[count];
                Array.Copy(array, answer, count);
                return new ArraySegment<T>(answer, 0, answer.Length);
            }
            finally
            {
                pool.Return(array);
            }
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ArraySegment<T> innerValue))
            {
                throw new NullReferenceException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
