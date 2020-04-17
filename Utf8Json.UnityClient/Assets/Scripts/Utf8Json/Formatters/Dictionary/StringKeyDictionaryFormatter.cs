﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeyDictionaryFormatter<TValue>
        : IJsonFormatter<Dictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, Dictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, Dictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeyDictionaryFormatter<TValue>
        : IJsonFormatter<Dictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public Dictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static Dictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Dictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static Dictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new Dictionary<string, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
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
            SerializeStatic(ref writer, value as Dictionary<string, TValue>, options);
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


#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeyReadOnlyDictionaryFormatter<TValue>
        : IJsonFormatter<ReadOnlyDictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyDictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyDictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeyReadOnlyDictionaryFormatter<TValue>
        : IJsonFormatter<ReadOnlyDictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, ReadOnlyDictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyDictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public ReadOnlyDictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static ReadOnlyDictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ReadOnlyDictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ReadOnlyDictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new Dictionary<string, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(key, value);
                }
            }

            return new ReadOnlyDictionary<string, TValue>(answer);
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ReadOnlyDictionary<string, TValue>, options);
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


#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeySortedDictionaryFormatter<TValue>
        : IJsonFormatter<SortedDictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, SortedDictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, SortedDictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeySortedDictionaryFormatter<TValue>
        : IJsonFormatter<SortedDictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, SortedDictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, SortedDictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public SortedDictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static SortedDictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public SortedDictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static SortedDictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new SortedDictionary<string, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
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
            SerializeStatic(ref writer, value as SortedDictionary<string, TValue>, options);
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


#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeySortedListFormatter<TValue>
        : IJsonFormatter<SortedList<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, SortedList<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, SortedList<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeySortedListFormatter<TValue>
        : IJsonFormatter<SortedList<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, SortedList<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, SortedList<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public SortedList<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static SortedList<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public SortedList<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static SortedList<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new SortedList<string, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
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
            SerializeStatic(ref writer, value as SortedList<string, TValue>, options);
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


#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeyInterfaceDictionaryFormatter<TValue>
        : IJsonFormatter<IDictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, IDictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, IDictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeyInterfaceDictionaryFormatter<TValue>
        : IJsonFormatter<IDictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, IDictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, IDictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public IDictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static IDictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IDictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static IDictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new Dictionary<string, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
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
            SerializeStatic(ref writer, value as IDictionary<string, TValue>, options);
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


#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeyConcurrentDictionaryFormatter<TValue>
        : IJsonFormatter<ConcurrentDictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, ConcurrentDictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ConcurrentDictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeyConcurrentDictionaryFormatter<TValue>
        : IJsonFormatter<ConcurrentDictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, ConcurrentDictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ConcurrentDictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public ConcurrentDictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static ConcurrentDictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ConcurrentDictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ConcurrentDictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new ConcurrentDictionary<string, TValue>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.TryAdd(key, value);
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
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
            SerializeStatic(ref writer, value as ConcurrentDictionary<string, TValue>, options);
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
#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeyImmutableDictionaryFormatter<TValue>
        : IJsonFormatter<ImmutableDictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, ImmutableDictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ImmutableDictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeyImmutableDictionaryFormatter<TValue>
        : IJsonFormatter<ImmutableDictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, ImmutableDictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ImmutableDictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public ImmutableDictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static ImmutableDictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ImmutableDictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ImmutableDictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new List<KeyValuePair<string, TValue>>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(new KeyValuePair<string, TValue>(key, value));
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(new KeyValuePair<string, TValue>(key, value));
                }
            }

            return ImmutableDictionary<string, TValue>.Empty.AddRange(answer);
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ImmutableDictionary<string, TValue>, options);
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
#if CSHARP_8_OR_NEWER
    public sealed unsafe class StringKeyImmutableSortedDictionaryFormatter<TValue>
        : IJsonFormatter<ImmutableSortedDictionary<string, TValue>?>
    {
        public void Serialize(ref JsonWriter writer, ImmutableSortedDictionary<string, TValue>? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ImmutableSortedDictionary<string, TValue>? value, JsonSerializerOptions options)
#else
    public sealed unsafe class StringKeyImmutableSortedDictionaryFormatter<TValue>
        : IJsonFormatter<ImmutableSortedDictionary<string, TValue>>
    {
        public void Serialize(ref JsonWriter writer, ImmutableSortedDictionary<string, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }
        
        public static void SerializeStatic(ref JsonWriter writer, ImmutableSortedDictionary<string, TValue> value, JsonSerializerOptions options)
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
                var propertyName = tuple.Key;
                Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                writer.WritePropertyName(propertyName);
                    
                var valueSerializer = options.Resolver.GetSerializeStatic<TValue>();
                if (valueSerializer.ToPointer() == null)
                {
                    var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                    valueFormatter.Serialize(ref writer, tuple.Value, options);

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        tuple = e.Current;
                        propertyName = tuple.Key;
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
                        propertyName = tuple.Key;
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        writer.Serialize(tuple.Value, options, valueSerializer);
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
        public ImmutableSortedDictionary<string, TValue>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
      
        public static ImmutableSortedDictionary<string, TValue>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ImmutableSortedDictionary<string, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ImmutableSortedDictionary<string, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new List<KeyValuePair<string, TValue>>();
            var count = 0;
            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = valueFormatter.Deserialize(ref reader, options);
                    answer.Add(new KeyValuePair<string, TValue>(key, value));
                }
            }
            else
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var key = reader.ReadPropertyName();
                    var value = reader.Deserialize<TValue>(options, valueDeserializer);
                    answer.Add(new KeyValuePair<string, TValue>(key, value));
                }
            }

            return ImmutableSortedDictionary<string, TValue>.Empty.AddRange(answer);
        }
        
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ImmutableSortedDictionary<string, TValue>, options);
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
