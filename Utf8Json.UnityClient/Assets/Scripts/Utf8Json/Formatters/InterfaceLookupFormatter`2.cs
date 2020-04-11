// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using StaticFunctionPointerHelper;

namespace Utf8Json.Formatters
{
    public sealed class InterfaceLookupFormatter<TKey, TElement>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ILookup<TKey, TElement>?>
        where TKey : notnull
#else
        : IJsonFormatter<ILookup<TKey, TElement>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ILookup<TKey, TElement>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ILookup<TKey, TElement> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ILookup<TKey, TElement>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ILookup<TKey, TElement> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var serializer = options.Resolver.GetSerializeStatic<IEnumerable<IGrouping<TKey, TElement>>>();
            if (serializer == IntPtr.Zero)
            {
                options.Resolver.GetFormatterWithVerify<IEnumerable<IGrouping<TKey, TElement>>>().Serialize(ref writer, value.AsEnumerable(), options);
            }
            else
            {
                writer.Serialize<IEnumerable<IGrouping<TKey, TElement>>>(value, options, serializer);
            }
        }

#if CSHARP_8_OR_NEWER
        public ILookup<TKey, TElement>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ILookup<TKey, TElement> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static ILookup<TKey, TElement>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ILookup<TKey, TElement> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var count = 0;

            var intermediateCollection = new Dictionary<TKey, IGrouping<TKey, TElement>>();

            reader.ReadIsBeginArrayWithVerify();

            var deserializer = options.Resolver.GetDeserializeStatic<IGrouping<TKey, TElement>>();
            if (deserializer == IntPtr.Zero)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<IGrouping<TKey, TElement>>();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var g = formatter.Deserialize(ref reader, options);
                    intermediateCollection.Add(g.Key, g);
                }
            }
            else
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var g = reader.Deserialize<IGrouping<TKey, TElement>>(options, deserializer);
                    intermediateCollection.Add(g.Key, g);
                }
            }

            return new Internal.Lookup<TKey, TElement>(intermediateCollection);
        }
    }
}
