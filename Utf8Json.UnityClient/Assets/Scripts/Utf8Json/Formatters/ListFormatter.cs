// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using StaticFunctionPointerHelper;

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

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, value[0], options);

                for (var i = 1; i < value.Count; i++)
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, value[i], options);
                }
            }
            else
            {
                writer.Serialize(value[0], options, serializer);

                for (var i = 1; i < value.Count; i++)
                {
                    writer.WriteValueSeparator();
                    writer.Serialize(value[i], options, serializer);
                }
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
            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(256);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                if (deserializer == IntPtr.Zero)
                {
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
                }
                else
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


            reader.ReadIsBeginArrayWithVerify();

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                DeserializeToWithFormatter(ref value, ref reader, options);
                return;
            }

            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (value == null)
                {
                    value = new List<T>();
                }

                value.Add(reader.Deserialize<T>(options, deserializer));
            }
        }

#if CSHARP_8_OR_NEWER
        private static void DeserializeToWithFormatter(ref List<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        private static void DeserializeToWithFormatter(ref List<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (value == null)
                {
                    value = new List<T>();
                }

                value.Add(formatter.Deserialize(ref reader, options));
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

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer == IntPtr.Zero)
            {
                SerializeWithFormatter(ref writer, value, options);
            }
            else
            {
                writer.Serialize(value[0], options, serializer);

                for (var i = 1; i < value.Count; i++)
                {
                    writer.WriteValueSeparator();
                    writer.Serialize(value[i], options, serializer);
                }
            }
        END:
            writer.WriteEndArray();
        }

        private static void SerializeWithFormatter(ref JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value[0], options);

            for (var i = 1; i < value.Count; i++)
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, value[i], options);
            }
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
            var count = 0;
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                if (deserializer == IntPtr.Zero)
                {
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
                }
                else
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

            reader.ReadIsBeginArrayWithVerify();

            if (value == null)
            {
                value = new List<T>();
            }
            else
            {
                value.Clear();
            }

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                DeserializeToWithFormatter(value, ref reader, options);
                return;
            }
            
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                value.Add(reader.Deserialize<T>(options, deserializer));
            }
        }

        private static void DeserializeToWithFormatter(List<T> value, ref JsonReader reader, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                value.Add(formatter.Deserialize(ref reader, options));
            }
        }
    }
}
