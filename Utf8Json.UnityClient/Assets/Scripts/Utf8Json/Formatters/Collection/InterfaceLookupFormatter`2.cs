// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

#if !ENABLE_IL2CPP
using StaticFunctionPointerHelper;
#endif

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
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            options.SerializeWithVerify<IEnumerable<IGrouping<TKey, TElement>>>(ref writer, value);
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

            var intermediateCollection = new Dictionary<TKey, IGrouping<TKey, TElement>>();

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<IGrouping<TKey, TElement>>();
            unsafe
            {
                if (deserializer.ToPointer() != null)
                {
                    while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                    {
                        var g = reader.Deserialize<IGrouping<TKey, TElement>>(options, deserializer);
                        intermediateCollection.Add(g.Key, g);
                    }
                    goto END;
                }
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<IGrouping<TKey, TElement>>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var g = formatter.Deserialize(ref reader, options);
                intermediateCollection.Add(g.Key, g);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            var answer = new Internal.Lookup<TKey, TElement>(intermediateCollection);
            return answer;
        }


#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ILookup<TKey, TElement>, options);
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
