// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using StaticFunctionPointerHelper;

namespace Utf8Json.Formatters
{
    public sealed class AddArrayFormatter<T>
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
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            if (value.Length == 0)
            {
                writer.WriteEndArray();
                return;
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, value[0], options);

                for (var i = 1; i < value.Length; i++)
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, value[i], options);
                }
            }
            else
            {
                writer.Serialize(value[0], options, serializer);
                for (var i = 1; i < value.Length; i++)
                {
                    writer.WriteValueSeparator();
                    writer.Serialize(value[i], options, serializer);
                }
            }

            writer.WriteEndArray();
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

            var count = 0;
            var array = ArrayPool<T>.Shared.Rent(256);
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            try
            {
                if (deserializer == IntPtr.Zero)
                {
                    var formatter = options.Resolver.GetFormatterWithVerify<T>();
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (array.Length < count)
                        {
                            var tmp = ArrayPool<T>.Shared.Rent(count << 1);
                            Array.Copy(array, 0, tmp, 0, array.Length);
                            ArrayPool<T>.Shared.Return(array);
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
                            var tmp = ArrayPool<T>.Shared.Rent(count << 1);
                            Array.Copy(array, 0, tmp, 0, array.Length);
                            ArrayPool<T>.Shared.Return(array);
                            array = tmp;
                        }

                        array[count - 1] = reader.Deserialize<T>(options, deserializer);
                    }
                }

                var answer = new T[count];
                Array.Copy(array, answer, count);
                return answer;
            }
            finally
            {
                ArrayPool<T>.Shared.Return(array);
            }
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
            var pool = ArrayPool<T>.Shared;
            var workingArea = pool.Rent(64);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();

                void ReAllocWorkingArea()
                {
                    var tmp = pool.Rent(count << 1);
                    Array.Copy(workingArea, tmp, workingArea.Length);
                    pool.Return(workingArea);
                    workingArea = tmp;
                }

                if (deserializer == IntPtr.Zero)
                {
                    var formatter = options.Resolver.GetFormatterWithVerify<T>();
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (workingArea.Length < count)
                        {
                            ReAllocWorkingArea();
                        }

                        workingArea[count - 1] = formatter.Deserialize(ref reader, options);
                    }
                }
                else
                {
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (workingArea.Length < count)
                        {
                            ReAllocWorkingArea();
                        }

                        workingArea[count - 1] = reader.Deserialize<T>(options, deserializer);
                    }
                }

                if (count == 0)
                {
                    return;
                }
                if (value == null)
                {
                    value = new T[count];
                    Array.Copy(workingArea, 0, value, value.Length, count);
                }
                else
                {
                    var result = new T[value.Length + count];
                    Array.Copy(value, 0, result, 0, value.Length);
                    Array.Copy(workingArea, 0, result, value.Length, count);
                    value = result;
                }
            }
            finally
            {
                pool.Return(workingArea);
            }
        }
    }

    public sealed class OverwriteArrayFormatter<T>
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
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();
            if (value.Length == 0)
            {
                goto END;
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, value[0], options);

                for (var i = 1; i < value.Length; i++)
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, value[i], options);
                }
            }
            else
            {
                writer.Serialize(value[0], options, serializer);

                for (var i = 1; i < value.Length; i++)
                {
                    writer.WriteValueSeparator();
                    writer.Serialize(value[i], options, serializer);
                }
            }
        END:
            writer.WriteEndArray();
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

            var count = 0;
            var pool = ArrayPool<T>.Shared;
            var array = pool.Rent(256);
            try
            {
                void ReAlloc()
                {
                    var tmp = pool.Rent(count << 1);
                    Array.Copy(array, 0, tmp, 0, array.Length);
                    pool.Return(array);
                    array = tmp;
                }

                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                if (deserializer == IntPtr.Zero)
                {
                    var formatter = options.Resolver.GetFormatterWithVerify<T>();
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        if (array.Length < count)
                        {
                            ReAlloc();
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
                            ReAlloc();
                        }

                        array[count - 1] = reader.Deserialize<T>(options, deserializer);
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

            if (deserializer == IntPtr.Zero)
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
