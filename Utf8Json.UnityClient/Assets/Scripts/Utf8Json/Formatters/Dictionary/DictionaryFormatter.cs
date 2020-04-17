// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using StaticFunctionPointerHelper;

#if IMMUTABLE
using System.Collections.Immutable;
#endif

namespace Utf8Json.Formatters
{

    public sealed unsafe class DictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Dictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<Dictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Dictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Dictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public Dictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Dictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Dictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Dictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new Dictionary<TKey, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(key, value);
                }
            }

            return answer;
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Dictionary<TKey, TValue>, options);
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


    public sealed unsafe class ReadOnlyDictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ReadOnlyDictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<ReadOnlyDictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ReadOnlyDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ReadOnlyDictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyDictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public ReadOnlyDictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ReadOnlyDictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ReadOnlyDictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ReadOnlyDictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new Dictionary<TKey, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(key, value);
                }
            }

            return new ReadOnlyDictionary<TKey, TValue>(answer);
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ReadOnlyDictionary<TKey, TValue>, options);
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


    public sealed unsafe class SortedDictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<SortedDictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<SortedDictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, SortedDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, SortedDictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, SortedDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, SortedDictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public SortedDictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public SortedDictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static SortedDictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static SortedDictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new SortedDictionary<TKey, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(key, value);
                }
            }

            return answer;
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as SortedDictionary<TKey, TValue>, options);
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


    public sealed unsafe class SortedListFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<SortedList<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<SortedList<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, SortedList<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, SortedList<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, SortedList<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, SortedList<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public SortedList<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public SortedList<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static SortedList<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static SortedList<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new SortedList<TKey, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(key, value);
                }
            }

            return answer;
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as SortedList<TKey, TValue>, options);
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


    public sealed unsafe class InterfaceDictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<IDictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<IDictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, IDictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, IDictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public IDictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IDictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static IDictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static IDictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new Dictionary<TKey, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(key, value);
                }
            }

            return answer;
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as IDictionary<TKey, TValue>, options);
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


    public sealed unsafe class ConcurrentDictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ConcurrentDictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<ConcurrentDictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ConcurrentDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ConcurrentDictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ConcurrentDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ConcurrentDictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public ConcurrentDictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ConcurrentDictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ConcurrentDictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ConcurrentDictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new ConcurrentDictionary<TKey, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.TryAdd(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.TryAdd(key, value);
                }
            }

            return answer;
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ConcurrentDictionary<TKey, TValue>, options);
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

#if IMMUTABLE
    public sealed unsafe class ImmutableDictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ImmutableDictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<ImmutableDictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ImmutableDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ImmutableDictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ImmutableDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ImmutableDictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public ImmutableDictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ImmutableDictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ImmutableDictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ImmutableDictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new List<KeyValuePair<TKey, TValue>>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }

            return ImmutableDictionary<TKey, TValue>.Empty.AddRange(answer);
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ImmutableDictionary<TKey, TValue>, options);
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
#endif
#if IMMUTABLE
    public sealed unsafe class ImmutableSortedDictionaryFormatter<TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ImmutableSortedDictionary<TKey, TValue>?>
        where TKey : notnull
#else
        : IJsonFormatter<ImmutableSortedDictionary<TKey, TValue>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ImmutableSortedDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ImmutableSortedDictionary<TKey, TValue> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ImmutableSortedDictionary<TKey, TValue>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ImmutableSortedDictionary<TKey, TValue> value, JsonSerializerOptions options)
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

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (!e.MoveNext())
                {
                    goto END;
                }

                var tuple = e.Current;
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                    writer.WriteNameSeparator();
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                            writer.WriteNameSeparator();
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
                else
                {
                    var propertyName = tuple.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    
                    if (valueSerializer.ToPointer() == null)
                    {
                        var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                        valueFormatter.Serialize(ref writer, tuple.Value, options);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            valueFormatter.Serialize(ref writer, tuple.Value, options);
                        }
                    }
                    else
                    {
                        writer.Serialize(tuple.Value, options, valueSerializer);

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            tuple = e.Current;
                            propertyName = tuple.Key.ToString();
                            Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                            writer.WritePropertyName(propertyName);
                            writer.Serialize(tuple.Value, options, valueSerializer);
                        }
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public ImmutableSortedDictionary<TKey, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ImmutableSortedDictionary<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ImmutableSortedDictionary<TKey, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ImmutableSortedDictionary<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            if (!(options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter))
            {
                throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new List<KeyValuePair<TKey, TValue>>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }

            return ImmutableSortedDictionary<TKey, TValue>.Empty.AddRange(answer);
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ImmutableSortedDictionary<TKey, TValue>, options);
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
#endif
}
