// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ReadOnlyCollectionFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ReadOnlyCollection<T>?>
#else
        : IJsonFormatter<ReadOnlyCollection<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ReadOnlyCollection<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ReadOnlyCollection<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyCollection<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyCollection<T> value, JsonSerializerOptions options)
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
            var enumerator = value.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }

                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    SerializeWithFormatter(ref writer, options, enumerator);
                    goto END;
                }

                writer.Serialize(enumerator.Current, options, serializer);

                while (enumerator.MoveNext())
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                    writer.Serialize(enumerator.Current, options, serializer);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
        var span2 = writer.Writer.GetSpan(1);
        span2[0] = (byte)']';
        writer.Writer.Advance(1);
        }

        private static void SerializeWithFormatter(ref JsonWriter writer, JsonSerializerOptions options, IEnumerator<T> enumerator)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, enumerator.Current, options);

            while (enumerator.MoveNext())
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
                formatter.Serialize(ref writer, enumerator.Current, options);
            }
        }

#if CSHARP_8_OR_NEWER
        public ReadOnlyCollection<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ReadOnlyCollection<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ReadOnlyCollection<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ReadOnlyCollection<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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
                    return DeserializeWithFormatter(ref reader, options, ref array, pool);
                }
                else
                {
                    var count = 0;
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (array.Length < count)
                        {
                            var tmp = pool.Rent(count);
                            Array.Copy(array, tmp, array.Length);
                            pool.Return(array);
                            array = tmp;
                        }

                        array[count - 1] = reader.Deserialize<T>(options, deserializer);
                    }

                    var answer = new T[count];
                    Array.Copy(array, answer, count);
                    return new ReadOnlyCollection<T>(answer);
                }

            }
            finally
            {
                pool.Return(array);
            }
        }

        private static ReadOnlyCollection<T> DeserializeWithFormatter(ref JsonReader reader, JsonSerializerOptions options, ref T[] array, ArrayPool<T> pool)
        {
            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (array.Length < count)
                {
                    var tmp = pool.Rent(count);
                    Array.Copy(array, tmp, array.Length);
                    pool.Return(array);
                    array = tmp;
                }

                array[count - 1] = formatter.Deserialize(ref reader, options);
            }

            var answer = new T[count];
            Array.Copy(array, answer, count);
            return new ReadOnlyCollection<T>(answer);
        }
    }
}
