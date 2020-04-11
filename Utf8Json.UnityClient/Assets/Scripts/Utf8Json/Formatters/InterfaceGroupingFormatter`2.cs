// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Utf8Json.Internal.Formatters;

namespace Utf8Json.Formatters
{
    public sealed class InterfaceGroupingFormatter<TKey, TElement> : IJsonFormatter<IGrouping<TKey, TElement>>
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IGrouping<TKey, TElement>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, IGrouping<TKey, TElement> value, JsonSerializerOptions options)
#endif
        => SerializeStatic(ref writer, value, options);

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IGrouping<TKey, TElement>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, IGrouping<TKey, TElement> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteRaw(CollectionFormatterHelper.GroupingName[0]);
            options.Resolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, value.Key, options);
            writer.WriteRaw(CollectionFormatterHelper.GroupingName[1]);
            options.Resolver.GetFormatterWithVerify<IEnumerable<TElement>>().Serialize(ref writer, value.AsEnumerable(), options);

            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public IGrouping<TKey, TElement>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public IGrouping<TKey, TElement> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public static IGrouping<TKey, TElement>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public static IGrouping<TKey, TElement> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            throw new NotImplementedException();
            /*
            if (reader.ReadIsNull())
            {
                return null;
            }

            var resultKey = default(TKey);
            var resultValue = default(IEnumerable<TElement>);

            reader.ReadIsBeginObjectWithVerify();

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
#if !NET_STANDARD_2_0
                //CollectionFormatterHelper.groupingAutomata.TryGetValue(keyString, out var key);
#else
                //CollectionFormatterHelper.groupingAutomata.TryGetValueSafe(keyString, out var key);
#endif

                switch (key)
                {
                    case 0:
                        resultKey = options.Resolver.GetFormatterWithVerify<TKey>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        resultValue = options.Resolver.GetFormatterWithVerify<IEnumerable<TElement>>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            Debug.Assert(resultKey != null, nameof(resultKey) + " != null");
            Debug.Assert(resultValue != null, nameof(resultValue) + " != null");
            return new Utf8Json.Internal.Grouping<TKey, TElement>(resultKey, resultValue);
            */
        }
    }
}
