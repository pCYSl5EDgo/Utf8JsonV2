// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Utf8Json.Formatters
{
    public sealed class InterfaceGroupingFormatter<TKey, TElement>
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

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            ++writer.Depth;
            {
                var span = writer.Writer.GetSpan(3);
                span[0] = 0x4B; // K
                span[1] = 0x65; // e
                span[2] = 0x79; // y
                writer.Writer.Advance(3);
            }

            options.SerializeWithVerify(ref writer, value.Key);

            {
                var span = writer.Writer.GetSpan(8);
                (span[0], span[1], span[2], span[3], span[4], span[5], span[6], span[7])
                    // Elements
                    = (0x45, 0x6C, 0x65, 0x6D, 0x65, 0x6E, 0x74, 0x73);
                writer.Writer.Advance(8);
            }

            options.SerializeWithVerify<IEnumerable<TElement>>(ref writer, value);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)'}';
                writer.Writer.Advance(1);
            }
            --writer.Depth;
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
                            resultKey = options.DeserializeWithVerify<TKey>(ref reader);
                        }
                        break;
                    case 8:
                        if (keyString[0] == 0x45 && keyString[1] == 0x6C && keyString[2] == 0x65 && keyString[3] == 0x6D && keyString[4] == 0x65 && keyString[5] == 0x6E && keyString[6] == 0x74 && keyString[7] == 0x73)
                        {
                            resultValue = options.DeserializeWithVerify<IEnumerable<TElement>>(ref reader);
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

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as IGrouping<TKey, TElement>, options);
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
