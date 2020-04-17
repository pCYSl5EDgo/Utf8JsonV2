// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Utf8Json.Formatters
{
    public sealed unsafe class InterfaceGroupingFormatter<TKey, TElement>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<IGrouping<TKey, TElement>?>
#else
        : IJsonFormatter<IGrouping<TKey, TElement>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, IGrouping<TKey, TElement>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, IGrouping<TKey, TElement> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, IGrouping<TKey, TElement>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, IGrouping<TKey, TElement> value, JsonSerializerOptions options)
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

            {
                var nameSpan = writer.Writer.GetSpan(3);
                nameSpan[0] = 0x4B; // K
                nameSpan[1] = 0x65; // e
                nameSpan[2] = 0x79; // y
                writer.Writer.Advance(3);
            }

            var keySerializer = options.Resolver.GetSerializeStatic<TKey>();
            if (keySerializer.ToPointer() == null)
            {
                options.Resolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, value.Key, options);
            }
            else
            {
                writer.Serialize(value.Key, options, keySerializer);
            }

            {
                var nameSpan = writer.Writer.GetSpan(8);
                (nameSpan[0], nameSpan[1], nameSpan[2], nameSpan[3], nameSpan[4], nameSpan[5], nameSpan[6], nameSpan[7])
                    // Elements
                    = (0x45, 0x6C, 0x65, 0x6D, 0x65, 0x6E, 0x74, 0x73);
                writer.Writer.Advance(8);
            }

#if CSHARP_8_OR_NEWER
            var enumerableSerializer = options.Resolver.GetSerializeStatic<IEnumerable<TElement>?>();
#else
            var enumerableSerializer = options.Resolver.GetSerializeStatic<IEnumerable<TElement>>();
#endif
            if (enumerableSerializer.ToPointer() == null)
            {
                options.Resolver.GetFormatterWithVerify<IEnumerable<TElement>>().Serialize(ref writer, value.AsEnumerable(), options);
            }
            else
            {
                writer.Serialize<IEnumerable<TElement>>(value, options, enumerableSerializer);
            }

            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)'}';
            writer.Writer.Advance(1);
        }

#if CSHARP_8_OR_NEWER
        public IGrouping<TKey, TElement>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public IGrouping<TKey, TElement> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static IGrouping<TKey, TElement>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static IGrouping<TKey, TElement> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
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
                switch (keyString.Length)
                {
                    case 3:
                        if (keyString[0] == 0x4B && keyString[1] == 0x65 && keyString[2] == 0x79)
                        {
                            var deserializer = options.Resolver.GetDeserializeStatic<TKey>();
                            resultKey = deserializer.ToPointer() == null
                                ? options.Resolver.GetFormatterWithVerify<TKey>().Deserialize(ref reader, options)
                                : reader.Deserialize<TKey>(options, deserializer);
                        }
                        break;
                    case 8:
                        if (keyString[0] == 0x45 && keyString[1] == 0x6C && keyString[2] == 0x65 && keyString[3] == 0x6D && keyString[4] == 0x65 && keyString[5] == 0x6E && keyString[6] == 0x74 && keyString[7] == 0x73)
                        {
                            var deserializer = options.Resolver.GetDeserializeStatic<IEnumerable<TElement>>();
                            resultValue = deserializer.ToPointer() == null
                                ? options.Resolver.GetFormatterWithVerify<IEnumerable<TElement>>().Deserialize(ref reader, options)
                                : reader.Deserialize<IEnumerable<TElement>>(options, deserializer);
                        }
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            Debug.Assert(resultKey != null, nameof(resultKey) + " != null");
            Debug.Assert(resultValue != null, nameof(resultValue) + " != null");
            return new Internal.Grouping<TKey, TElement>(resultKey, resultValue);
        }
    }
}
