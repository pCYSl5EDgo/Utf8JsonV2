// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utf8Json.Formatters
{
    public sealed class GenericDictionaryFormatter<TDictionary, TKey, TValue>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<TDictionary?>
        where TDictionary : class, IDictionary<TKey, TValue>, new()
        where TKey : notnull
#else
        : IJsonFormatter<TDictionary>
        where TDictionary : class, IDictionary<TKey, TValue>, new()
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, TDictionary? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, TDictionary value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, TDictionary? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, TDictionary value, JsonSerializerOptions options)
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyObject();
                return;
            }

            ++writer.Depth;
            var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();
            writer.WriteBeginObject();
            var e = value.GetEnumerator();
            try
            {
                if (options.Resolver.GetFormatterWithVerify<TKey>() is IObjectPropertyNameFormatter<TKey> keyFormatter)
                {
                    if (e.MoveNext())
                    {
                        var tuple = e.Current;
                        keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                        var span = writer.Writer.GetSpan(1);
                        span[0] = (byte)':';
                        writer.Writer.Advance(1);
                        valueFormatter.Serialize(ref writer, tuple.Value, options);
                    }
                    else
                    {
                        goto END;
                    }

                    while (e.MoveNext())
                    {
                        var span1 = writer.Writer.GetSpan(1);
                        span1[0] = (byte)',';
                        writer.Writer.Advance(1);
                        var tuple = e.Current;
                        keyFormatter.SerializeToPropertyName(ref writer, tuple.Key, options);
                        var span = writer.Writer.GetSpan(1);
                        span[0] = (byte)':';
                        writer.Writer.Advance(1);
                        valueFormatter.Serialize(ref writer, tuple.Value, options);
                    }
                }
                else
                {
                    if (e.MoveNext())
                    {
                        var tuple = e.Current;
                        var propertyName = tuple.Key.ToString();
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        valueFormatter.Serialize(ref writer, tuple.Value, options);
                    }
                    else
                    {
                        goto END;
                    }

                    while (e.MoveNext())
                    {
                        var span = writer.Writer.GetSpan(1);
                        span[0] = (byte)',';
                        writer.Writer.Advance(1);
                        var tuple = e.Current;
                        var propertyName = tuple.Key.ToString();
                        Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                        writer.WritePropertyName(propertyName);
                        valueFormatter.Serialize(ref writer, tuple.Value, options);
                    }
                }
            }
            finally
            {
                e.Dispose();
            }

        END:
            var span2 = writer.Writer.GetSpan(1);
            span2[0] = (byte)'}';
            writer.Writer.Advance(1);
            --writer.Depth;
        }

#if CSHARP_8_OR_NEWER
        public TDictionary? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public TDictionary Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static TDictionary? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static TDictionary DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
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

            var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();

            reader.ReadIsBeginObjectWithVerify();

            var answer = new TDictionary();
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var key = keyFormatter.DeserializeFromPropertyName(ref reader, options);
                reader.ReadIsNameSeparatorWithVerify();
                var value = valueFormatter.Deserialize(ref reader, options);
                answer.Add(key, value);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as TDictionary, options);
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
