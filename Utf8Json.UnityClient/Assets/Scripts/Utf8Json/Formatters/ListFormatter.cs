// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;

namespace Utf8Json.Formatters
{
    public sealed class ListFormatter<T>
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<T>?>
#else
        : IOverwriteJsonFormatter<List<T>>
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
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            if (value.Count == 0)
            {
                goto END;
            }

            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value[0], options);

            for (var i = 1; i < value.Count; i++)
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, value[i], options);
            }

        END:
            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        public List<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public List<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
            => DeserializeStatic(ref reader, options);

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

            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(256);
            try
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

                    array[count - 1] = formatter.Deserialize(ref reader, options);
                }

                var answer = new List<T>(count);
                var span = array.AsSpan(0, count);
                for (var index = 0; index < span.Length; index++)
                {
                    answer.Add(span[index]);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref List<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                // null, do nothing
                return;
            }

            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var list = value; // use the reference

            reader.ReadIsBeginArrayWithVerify();

            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (list == null)
                {
                    list = new List<T>();
                }

                list.Add(formatter.Deserialize(ref reader, options));
            }
        }
    }

    public sealed class OverwriteListFormatter<T>
#if CSHARP_8_OR_NEWER
        : IOverwriteJsonFormatter<List<T>?>
#else
        : IOverwriteJsonFormatter<List<T>>
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
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            if (value.Count == 0)
            {
                goto END;
            }

            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value[0], options);

            for (var i = 1; i < value.Count; i++)
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, value[1], options);
            }

        END:
            writer.WriteEndArray();
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

            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(256);
            try
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

                    array[count - 1] = formatter.Deserialize(ref reader, options);
                }

                var answer = new List<T>(count);
                var span = array.AsSpan(0, count);
                for (var index = 0; index < span.Length; index++)
                {
                    answer.Add(span[index]);
                }

                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref List<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref List<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                // null, do nothing
                return;
            }

            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var list = value; // use the reference

            reader.ReadIsBeginArrayWithVerify();

            if (list == null)
            {
                list = new List<T>();
            }
            else
            {
                list.Clear();
            }

            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                list.Add(formatter.Deserialize(ref reader, options));
            }
        }
    }
}
