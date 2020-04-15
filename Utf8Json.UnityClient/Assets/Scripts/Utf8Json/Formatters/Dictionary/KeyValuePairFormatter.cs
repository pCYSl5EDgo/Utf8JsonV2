// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Collections.Generic;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed unsafe class KeyValuePairFormatter<TKey, TValue>
        : IJsonFormatter<KeyValuePair<TKey, TValue>>
#if CSHARP_8_OR_NEWER
        where TKey : notnull
#endif
    {
        public void Serialize(ref JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializerOptions options)
        {
            {
                // {"Key":
                const int length = 7;
                var span = writer.Writer.GetSpan(length);
                span[0] = 0x7b;
                span[1] = 0x22;
                span[2] = 0x4b;
                span[3] = 0x65;
                span[4] = 0x79;
                span[5] = 0x22;
                span[6] = 0x3a;
                writer.Writer.Advance(length);
            }
            {
                var serializer = options.Resolver.GetSerializeStatic<TKey>();
                if (serializer.ToPointer() == null)
                {
                    options.Resolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, value.Key, options);
                }
                else
                {
                    writer.Serialize(value.Key, options, serializer);
                }
            }
            // ,"Value":
            {
                const int length = 9;
                var span = writer.Writer.GetSpan(length);
                span[0] = 0x2c;
                span[1] = 0x22;
                span[2] = 0x56;
                span[3] = 0x61;
                span[4] = 0x6c;
                span[5] = 0x75;
                span[6] = 0x65;
                span[7] = 0x22;
                span[8] = 0x3a;
                writer.Writer.Advance(length);
            }
            {
                var serializer = options.Resolver.GetSerializeStatic<TValue>();
                if (serializer.ToPointer() == null)
                {
                    options.Resolver.GetFormatterWithVerify<TValue>().Serialize(ref writer, value.Value, options);
                }
                else
                {
                    writer.Serialize(value.Value, options, serializer);
                }
            }
            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)'}';
            writer.Writer.Advance(1);
        }

        public KeyValuePair<TKey, TValue> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static KeyValuePair<TKey, TValue> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new JsonParsingException("KeyValuePair`2 cannot be null.");
            }

            reader.ReadIsBeginObjectWithVerify();
            var span = reader.ReadPropertyNameSegmentRaw();

            var keyDeserializer = options.Resolver.GetDeserializeStatic<TKey>();
            if (keyDeserializer.ToPointer() == null)
            {
                return WithFormatter(ref reader, options, span);
            }

            var valueDeserializer = options.Resolver.GetDeserializeStatic<TValue>();
            if (valueDeserializer.ToPointer() == null)
            {
                return WithFormatter(ref reader, options, span);
            }

            var key = default(TKey);
            var value = default(TValue);
            switch (span.Length)
            {
                case 3:
                    if (span[0] != 'K' || span[1] != 'e' || span[2] != 'y')
                    {
                        goto ERROR;
                    }

                    key = reader.Deserialize<TKey>(options, keyDeserializer);
                    break;
                case 5:
                    if (span[0] != 'V' || span[1] != 'a' || span[2] != 'l' || span[3] != 'u' || span[4] != 'e')
                    {
                        goto ERROR;
                    }

                    value = reader.Deserialize<TValue>(options, valueDeserializer);
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

                    key = reader.Deserialize<TKey>(options, keyDeserializer);
                    break;
                case 5:
                    if (span[0] != 'V' || span[1] != 'a' || span[2] != 'l' || span[3] != 'u' || span[4] != 'e')
                    {
                        goto ERROR;
                    }

                    value = reader.Deserialize<TValue>(options, valueDeserializer);
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

        private static KeyValuePair<TKey, TValue> WithFormatter(ref JsonReader reader, JsonSerializerOptions options, ReadOnlySpan<byte> span)
        {
            var keyFormatter = options.Resolver.GetFormatterWithVerify<TKey>();
            var valueFormatter = options.Resolver.GetFormatterWithVerify<TValue>();

            var key = default(TKey);
            var value = default(TValue);
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
