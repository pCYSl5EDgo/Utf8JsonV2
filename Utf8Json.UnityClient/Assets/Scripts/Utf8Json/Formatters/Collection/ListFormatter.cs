// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;

#if !ENABLE_IL2CPP
using StaticFunctionPointerHelper;
#endif

namespace Utf8Json.Formatters
{
    public sealed class ListFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<List<T>?>
#else
        : IJsonFormatter<List<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, List<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, List<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, List<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, List<T> value, JsonSerializerOptions options)
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
            if (value.Count == 0)
            {
                goto END;
            }

#if !ENABLE_IL2CPP
            var serializer = options.Resolver.GetSerializeStatic<T>();
            unsafe
            {
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(value[0], options, serializer);

                    for (var i = 1; i < value.Count; i++)
                    {
                        var span = writer.Writer.GetSpan(1);
                        span[0] = (byte)',';
                        writer.Writer.Advance(1);
                        writer.Serialize(value[i], options, serializer);
                    }
                    goto END;
                }
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value[0], options);

            for (var i = 1; i < value.Count; i++)
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
                formatter.Serialize(ref writer, value[i], options);
            }
        END:
            var span2 = writer.Writer.GetSpan(1);
            span2[0] = (byte)']';
            writer.Writer.Advance(1);
        }

#if CSHARP_8_OR_NEWER
        public List<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static List<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static List<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                var count = 0;
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
                                Array.Copy(array, 0, tmp, 0, array.Length);
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
                        Array.Copy(array, 0, tmp, 0, array.Length);
                        pool.Return(array);
                        array = tmp;
                    }

                    array[count - 1] = formatter.Deserialize(ref reader, options);
                }
#if !ENABLE_IL2CPP
            END:
#endif
                var answer = new List<T>(count);
                var span = array.AsSpan(0, count);
                foreach (var item in span)
                {
                    answer.Add(item);
                }

                return answer;
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
            SerializeStatic(ref writer, value as List<T>, options);
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
