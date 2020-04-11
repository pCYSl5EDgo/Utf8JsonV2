// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using StaticFunctionPointerHelper;

namespace Utf8Json.Formatters
{
    public sealed class AddGenericClassCollectionFormatter<T, TCollection>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<TCollection?>
#else
    : IOverwriteJsonFormatter<TCollection>
#endif
        where TCollection : class, ICollection<T>, new()
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, TCollection? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, TCollection value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, TCollection? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, TCollection value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public TCollection? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public TCollection Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static TCollection? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static TCollection DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new TCollection();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref TCollection? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref TCollection value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new TCollection();
                    }

                    value.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new TCollection();
                    }

                    value.Add(item);
                }
            }
        }
    }

    public sealed class OverwriteGenericClassCollectionFormatter<T, TCollection>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<TCollection?>
#else
    : IOverwriteJsonFormatter<TCollection>
#endif
        where TCollection : class, ICollection<T>, new()
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, TCollection? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, TCollection value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, TCollection? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, TCollection value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public TCollection? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public TCollection Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static TCollection? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static TCollection DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new TCollection();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref TCollection? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref TCollection value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();

            if (value == null)
            {
                value = new TCollection();
            }
            else
            {
                value.Clear();
            }
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.Add(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.Add(item);
                }
            }
        }
    }

    public sealed class AddLinkedListFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<LinkedList<T>?>
#else
    : IOverwriteJsonFormatter<LinkedList<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, LinkedList<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, LinkedList<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, LinkedList<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, LinkedList<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public LinkedList<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public LinkedList<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static LinkedList<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static LinkedList<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new LinkedList<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.AddLast(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.AddLast(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref LinkedList<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref LinkedList<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new LinkedList<T>();
                    }

                    value.AddLast(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new LinkedList<T>();
                    }

                    value.AddLast(item);
                }
            }
        }
    }

    public sealed class OverwriteLinkedListFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<LinkedList<T>?>
#else
    : IOverwriteJsonFormatter<LinkedList<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, LinkedList<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, LinkedList<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, LinkedList<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, LinkedList<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public LinkedList<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public LinkedList<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static LinkedList<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static LinkedList<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new LinkedList<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.AddLast(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.AddLast(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref LinkedList<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref LinkedList<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();

            if (value == null)
            {
                value = new LinkedList<T>();
            }
            else
            {
                value.Clear();
            }
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.AddLast(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.AddLast(item);
                }
            }
        }
    }

    public sealed class AddQueueFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<Queue<T>?>
#else
    : IOverwriteJsonFormatter<Queue<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Queue<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Queue<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Queue<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Queue<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public Queue<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Queue<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Queue<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Queue<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new Queue<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Enqueue(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Enqueue(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref Queue<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref Queue<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new Queue<T>();
                    }

                    value.Enqueue(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new Queue<T>();
                    }

                    value.Enqueue(item);
                }
            }
        }
    }

    public sealed class OverwriteQueueFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<Queue<T>?>
#else
    : IOverwriteJsonFormatter<Queue<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Queue<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Queue<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Queue<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Queue<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public Queue<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Queue<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static Queue<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Queue<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new Queue<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Enqueue(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Enqueue(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref Queue<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref Queue<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();

            if (value == null)
            {
                value = new Queue<T>();
            }
            else
            {
                value.Clear();
            }
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.Enqueue(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.Enqueue(item);
                }
            }
        }
    }

    public sealed class AddStackFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<Stack<T>?>
#else
    : IOverwriteJsonFormatter<Stack<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Stack<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Stack<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Stack<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Stack<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public Stack<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Stack<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Stack<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Stack<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new Stack<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Push(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Push(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref Stack<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref Stack<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new Stack<T>();
                    }

                    value.Push(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new Stack<T>();
                    }

                    value.Push(item);
                }
            }
        }
    }

    public sealed class OverwriteStackFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<Stack<T>?>
#else
    : IOverwriteJsonFormatter<Stack<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Stack<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Stack<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Stack<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Stack<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public Stack<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Stack<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static Stack<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Stack<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new Stack<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Push(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Push(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref Stack<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref Stack<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();

            if (value == null)
            {
                value = new Stack<T>();
            }
            else
            {
                value.Clear();
            }
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.Push(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.Push(item);
                }
            }
        }
    }

    public sealed class AddHashSetFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<HashSet<T>?>
#else
    : IOverwriteJsonFormatter<HashSet<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, HashSet<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, HashSet<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, HashSet<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, HashSet<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public HashSet<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public HashSet<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static HashSet<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static HashSet<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new HashSet<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref HashSet<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref HashSet<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new HashSet<T>();
                    }

                    value.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new HashSet<T>();
                    }

                    value.Add(item);
                }
            }
        }
    }

    public sealed class OverwriteHashSetFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<HashSet<T>?>
#else
    : IOverwriteJsonFormatter<HashSet<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, HashSet<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, HashSet<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, HashSet<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, HashSet<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public HashSet<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public HashSet<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static HashSet<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static HashSet<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new HashSet<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref HashSet<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref HashSet<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();

            if (value == null)
            {
                value = new HashSet<T>();
            }
            else
            {
                value.Clear();
            }
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.Add(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.Add(item);
                }
            }
        }
    }

    public sealed class AddInterfaceListFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<IList<T>?>
#else
    : IOverwriteJsonFormatter<IList<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IList<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, IList<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IList<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, IList<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public IList<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IList<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static IList<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static IList<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new List<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref IList<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref IList<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new List<T>();
                    }

                    value.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new List<T>();
                    }

                    value.Add(item);
                }
            }
        }
    }

    public sealed class OverwriteInterfaceListFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<IList<T>?>
#else
    : IOverwriteJsonFormatter<IList<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IList<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, IList<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IList<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, IList<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public IList<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IList<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static IList<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static IList<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new List<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref IList<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref IList<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
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
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.Add(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.Add(item);
                }
            }
        }
    }

    public sealed class AddInterfaceCollectionFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<ICollection<T>?>
#else
    : IOverwriteJsonFormatter<ICollection<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ICollection<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ICollection<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public ICollection<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ICollection<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ICollection<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ICollection<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new List<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref ICollection<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref ICollection<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    if (value == null)
                    {
                        value = new List<T>();
                    }

                    value.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    if (value == null)
                    {
                        value = new List<T>();
                    }

                    value.Add(item);
                }
            }
        }
    }

    public sealed class OverwriteInterfaceCollectionFormatter<T>
#if CSHARP_8_OR_NEWER
    : IOverwriteJsonFormatter<ICollection<T>?>
#else
    : IOverwriteJsonFormatter<ICollection<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ICollection<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ICollection<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
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
                if (serializer == IntPtr.Zero)
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
        public ICollection<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ICollection<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options); 
        }

#if CSHARP_8_OR_NEWER
        public static ICollection<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ICollection<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var buffer = new List<T>();
            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    buffer.Add(item);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
            }

            return buffer;
        }

#if CSHARP_8_OR_NEWER
        public void DeserializeTo(ref ICollection<T>? value, ref JsonReader reader, JsonSerializerOptions options)
#else
        public void DeserializeTo(ref ICollection<T> value, ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return; // do nothing
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
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = formatter.Deserialize(ref reader, options);
                    value.Add(item);
                }
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    value.Add(item);
                }
            }
        }
    }

}
