// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utf8Json.Formatters
{
    public sealed class NonGenericInterfaceDictionaryFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<IDictionary?>
#else
        : IJsonFormatter<IDictionary>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IDictionary? value, JsonSerializerOptions formatterResolver)
#else
        public void Serialize(ref JsonWriter writer, IDictionary value, JsonSerializerOptions formatterResolver)
#endif
        {
            SerializeStatic(ref writer, value, formatterResolver);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IDictionary? value, JsonSerializerOptions formatterResolver)
#else
        public static void SerializeStatic(ref JsonWriter writer, IDictionary value, JsonSerializerOptions formatterResolver)
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

#if CSHARP_8_OR_NEWER
            var valueFormatter = formatterResolver.Resolver.GetFormatterWithVerify<object?>();
#else
            var valueFormatter = formatterResolver.Resolver.GetFormatterWithVerify<object>();
#endif

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (e.MoveNext())
                {
                    Debug.Assert(e.Current != null, "e.Current != null");
                    var item = e.Entry;
                    var propertyName = item.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    valueFormatter.Serialize(ref writer, item.Value, formatterResolver);
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
                    Debug.Assert(e.Current != null, "e.Current != null");
                    var item = e.Entry;
                    var propertyName = item.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    valueFormatter.Serialize(ref writer, item.Value, formatterResolver);
                }
            }
            finally
            {
                if (e is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

        END:
        var span1 = writer.Writer.GetSpan(1);
        span1[0] = (byte)'}';
        writer.Writer.Advance(1);
        }

#if CSHARP_8_OR_NEWER
        public IDictionary? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IDictionary Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static IDictionary? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static IDictionary DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var valueFormatter = options.Resolver.GetFormatterWithVerify<object>();

            reader.ReadIsBeginObjectWithVerify();

            var dict = new Dictionary<object, object>();
            var i = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref i))
            {
                var key = reader.ReadPropertyName();
                var value = valueFormatter.Deserialize(ref reader, options);
                dict.Add(key, value);
            }

            return dict;
        }
    }
}
