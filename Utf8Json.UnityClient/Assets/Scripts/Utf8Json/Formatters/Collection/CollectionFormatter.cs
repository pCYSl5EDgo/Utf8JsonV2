// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

#if !ENABLE_IL2CPP
using StaticFunctionPointerHelper;
#endif

namespace Utf8Json.Formatters
{
    public sealed unsafe class GenericClassCollectionFormatter<T, TCollection>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<TCollection?>
#else
    : IJsonFormatter<TCollection>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Add(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as TCollection, options);
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

    public sealed unsafe class LinkedListFormatter<T>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<LinkedList<T>?>
#else
    : IJsonFormatter<LinkedList<T>>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.AddLast(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.AddLast(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as LinkedList<T>, options);
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

    public sealed unsafe class QueueFormatter<T>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Queue<T>?>
#else
    : IJsonFormatter<Queue<T>>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Enqueue(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Enqueue(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Queue<T>, options);
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

    public sealed unsafe class StackFormatter<T>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Stack<T>?>
#else
    : IJsonFormatter<Stack<T>>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Push(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Push(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Stack<T>, options);
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

    public sealed unsafe class HashSetFormatter<T>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<HashSet<T>?>
#else
    : IJsonFormatter<HashSet<T>>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Add(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as HashSet<T>, options);
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

    public sealed unsafe class InterfaceListFormatter<T>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<IList<T>?>
#else
    : IJsonFormatter<IList<T>>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Add(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as IList<T>, options);
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

    public sealed unsafe class InterfaceCollectionFormatter<T>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<ICollection<T>?>
#else
    : IJsonFormatter<ICollection<T>>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            var enumerator = value.GetEnumerator();
            writer.WriteBeginArray();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }
#if !ENABLE_IL2CPP
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(enumerator.Current, options, serializer);
                    while (enumerator.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(enumerator.Current, options, serializer);
                    }
                    goto END;
                }
#endif
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, enumerator.Current, options);
                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    formatter.Serialize(ref writer, enumerator.Current, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndArray();
            --writer.Depth;
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
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var item = reader.Deserialize<T>(options, deserializer);
                    buffer.Add(item);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Add(item);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            return buffer;
        }

        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ICollection<T>, options);
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
