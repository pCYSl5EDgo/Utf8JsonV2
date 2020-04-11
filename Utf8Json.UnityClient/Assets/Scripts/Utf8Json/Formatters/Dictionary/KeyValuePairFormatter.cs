// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    internal static class KeyValuePairFormatterHelper
    {
        // {"Key":
        internal static readonly byte[] KeyEmbedBytes = { 0x7B, 0x22, 0x4B, 0x65, 0x79, 0x22, 0x3A, };
        // ,"Value":
        internal static readonly byte[] ValueEmbedBytes = { 0x2C, 0x22, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x22, 0x3A, };
    }

    public sealed class KeyValuePairFormatter<TKey, TValue> : IJsonFormatter<KeyValuePair<TKey, TValue>>
    {
        public void Serialize(ref JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializerOptions options)
        {
            writer.WriteRaw(KeyValuePairFormatterHelper.KeyEmbedBytes);
            options.Resolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, value.Key, options);
            writer.WriteRaw(KeyValuePairFormatterHelper.ValueEmbedBytes);
            options.Resolver.GetFormatterWithVerify<TValue>().Serialize(ref writer, value.Value, options);
            writer.WriteEndObject();
        }

        public KeyValuePair<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static KeyValuePair<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new JsonParsingException("KeyValuePair`2 cannot be null.");
            }

            var key = default(TKey);
            var value = default(TValue);
            var keyFormatter = options.Resolver.GetFormatterWithVerify<TKey>();
            var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();

            reader.ReadIsBeginObjectWithVerify();
            var span = reader.ReadPropertyNameSegmentRaw();

            switch (span.Length)
            {
                case 3:
                    if (span[0] != 'K' || span[1] != 'e' || span[2] != 'y')
                    {
                        goto ERROR;
                    }

                    key = keyFormatter.Deserialize(ref reader, options);
                    break;
                case 5:
                    if (span[0] != 'V' || span[1] != 'a' || span[2] != 'l' || span[3] != 'u' || span[4] != 'e')
                    {
                        goto ERROR;
                    }

                    value = valueFormatter.Deserialize(ref reader, options);
                    break;
                default:
                    goto ERROR;
            }

            reader.ReadIsValueSeparatorWithVerify();
            span = reader.ReadPropertyNameSegmentRaw();

            switch (span.Length)
            {
                case 3:
                    if (span[0] != 'K' || span[1] != 'e' || span[2] != 'y')
                    {
                        goto ERROR;
                    }

                    key = keyFormatter.Deserialize(ref reader, options);
                    break;
                case 5:
                    if (span[0] != 'V' || span[1] != 'a' || span[2] != 'l' || span[3] != 'u' || span[4] != 'e')
                    {
                        goto ERROR;
                    }

                    value = valueFormatter.Deserialize(ref reader, options);
                    break;
                default:
                    goto ERROR;
            }

            reader.ReadIsEndObjectWithVerify();

#if CSHARP_8_OR_NEWER
            return new KeyValuePair<TKey, TValue>(key!, value!);
#else
            return new KeyValuePair<TKey, TValue>(key, value);
#endif
        ERROR:
            throw new JsonParsingException("Invalid Property Name.");
        }
    }
}
