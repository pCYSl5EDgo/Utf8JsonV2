// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Diagnostics;

namespace Utf8Json.Formatters
{
    public sealed class NonGenericDictionaryFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T?>
#else
        : IJsonFormatter<T>
#endif
        where T : class, IDictionary, new()
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

#if CSHARP_8_OR_NEWER
            var valueFormatter = options.Resolver.GetFormatterWithVerify<object?>();
#else
            var valueFormatter = options.Resolver.GetFormatterWithVerify<object>();
#endif

            writer.WriteBeginObject();

            var e = value.GetEnumerator();
            try
            {
                if (e.MoveNext())
                {
                    Debug.Assert(e.Current != null, "e.Current != null");
                    var item = (DictionaryEntry)e.Current;
                    var propertyName = item.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    valueFormatter.Serialize(ref writer, item.Value, options);
                }
                else
                {
                    goto END;
                }

                while (e.MoveNext())
                {
                    writer.WriteValueSeparator();
                    Debug.Assert(e.Current != null, "e.Current != null");
                    var item = (DictionaryEntry)e.Current;
                    var propertyName = item.Key.ToString();
                    Debug.Assert(propertyName != null, nameof(propertyName) + " != null");
                    writer.WritePropertyName(propertyName);
                    valueFormatter.Serialize(ref writer, item.Value, options);
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
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif

        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();

            var answer = new T();
            var count = 0;
            var valueFormatter = options.Resolver.GetFormatterWithVerify<object>();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var key = reader.ReadPropertyName();
                var value = valueFormatter.Deserialize(ref reader, options);
                answer.Add(key, value);
            }

            return answer;
        }
    }
}
