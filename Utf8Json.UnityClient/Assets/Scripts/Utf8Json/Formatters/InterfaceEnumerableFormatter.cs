// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace Utf8Json.Formatters
{
    public sealed unsafe class InterfaceEnumerableFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<IEnumerable<T>?>
#else
        : IJsonFormatter<IEnumerable<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IEnumerable<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IEnumerable<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }

                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = options.Resolver.GetFormatterWithVerify<T>();
                    formatter.Serialize(ref writer, enumerator.Current, options);

                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        formatter.Serialize(ref writer, enumerator.Current, options);
                    }
                }
                else
                {
                    writer.Serialize(enumerator.Current, options, serializer);

                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        public IEnumerable<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IEnumerable<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static IEnumerable<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static IEnumerable<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();

            var pool = ArrayPool<T>.Shared;

            var count = 0;
            var array = pool.Rent(32);
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
                            var tmp = pool.Rent(count);
                            Array.Copy(array, tmp, array.Length);
                            pool.Return(array);
                            array = tmp;
                        }
                        var item = formatter.Deserialize(ref reader, options);
                        array[count - 1] = item;
                    }
                }
                else
                {
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (array.Length < count)
                        {
                            var tmp = pool.Rent(count);
                            Array.Copy(array, tmp, array.Length);
                            pool.Return(array);
                            array = tmp;
                        }
                        var item = reader.Deserialize<T>(options, deserializer);
                        array[count - 1] = item;
                    }
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
    }
}
