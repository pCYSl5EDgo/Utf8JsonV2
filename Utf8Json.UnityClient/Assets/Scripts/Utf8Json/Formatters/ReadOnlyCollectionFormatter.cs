// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utf8Json.Formatters
{
    public sealed class ReadOnlyCollectionFormatter<T> : IJsonFormatter<ReadOnlyCollection<T>>
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ReadOnlyCollection<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ReadOnlyCollection<T> value, JsonSerializerOptions options)
#endif
        => SerializeStatic(ref writer, value, options);

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyCollection<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ReadOnlyCollection<T> value, JsonSerializerOptions options)
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
        }

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public ReadOnlyCollection<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public ReadOnlyCollection<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public static ReadOnlyCollection<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public static ReadOnlyCollection<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var buffer = new List<T>(); ;
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var item = formatter.Deserialize(ref reader, options);
                buffer.Add(item);
            }

            return new ReadOnlyCollection<T>(buffer);
        }
    }
}
