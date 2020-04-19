// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_2018_4_OR_NEWER

using System;
using UnityEngine;

namespace Utf8Json.Formatters
{
    public sealed class BoundsFormatter : IJsonFormatter<Bounds>
    {
        public void Serialize(ref JsonWriter writer, Bounds value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public Bounds Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Bounds value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 10;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'c';
                span[3] = (byte)'e';
                span[4] = (byte)'n';
                span[5] = (byte)'t';
                span[6] = (byte)'e';
                span[7] = (byte)'r';
                span[8] = (byte)'"';
                span[9] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            Vector3Formatter.SerializeStatic(ref writer, value.center, options);

            {
                const int sizeHint = 8;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'s';
                span[3] = (byte)'i';
                span[4] = (byte)'z';
                span[5] = (byte)'e';
                span[6] = (byte)'"';
                span[7] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            Vector3Formatter.SerializeStatic(ref writer, value.size, options);

            writer.WriteEndObject();
        }

        public static Bounds DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();
            var center = default(Vector3);
            var size = default(Vector3);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                switch (stringKey.Length)
                {
                    case 4: // size
                        if (stringKey[0] != 's' || stringKey[1] != 'i' || stringKey[2] != 'z' || stringKey[3] != 'e')
                        {
                            goto default;
                        }

                        size = Vector3Formatter.DeserializeStatic(ref reader, options);
                        break;
                    case 6: // center
                        if (stringKey[0] != 'c' || stringKey[1] != 'e' || stringKey[2] != 'n' || stringKey[3] != 't' || stringKey[4] != 'e' || stringKey[5] != 'r')
                        {
                            goto default;
                        }

                        center = Vector3Formatter.DeserializeStatic(ref reader, options);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            var answer = new Bounds(center, size);
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is Bounds innerValue))
            {
                throw new ArgumentNullException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
#endif
