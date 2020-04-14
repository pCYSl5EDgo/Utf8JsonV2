// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_2018_4_OR_NEWER

using System;
using UnityEngine;

namespace Utf8Json.Formatters
{
    public sealed class RectIntFormatter : IJsonFormatter<RectInt>
    {
        public void Serialize(ref JsonWriter writer, RectInt value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public RectInt Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, RectInt value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 5;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'x';
                span[3] = (byte)'"';
                span[4] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(value.x);

            {
                const int sizeHint = 5;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'y';
                span[3] = (byte)'"';
                span[4] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(value.y);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'w';
                span[3] = (byte)'i';
                span[4] = (byte)'d';
                span[5] = (byte)'t';
                span[6] = (byte)'h';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(value.width);

            {
                const int sizeHint = 10;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'h';
                span[3] = (byte)'e';
                span[4] = (byte)'i';
                span[5] = (byte)'g';
                span[6] = (byte)'h';
                span[7] = (byte)'t';
                span[8] = (byte)'"';
                span[9] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(value.height);

            writer.WriteEndObject();
        }

        public static RectInt DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();
            var x = default(int);
            var y = default(int);
            var width = default(int);
            var height = default(int);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                switch (stringKey.Length)
                {
                    case 1:
                        switch (stringKey[0])
                        {
                            case (byte)'x':
                                x = reader.ReadInt32();
                                continue;
                            case (byte)'y':
                                y = reader.ReadInt32();
                                continue;
                        }

                        goto default;
                    case 5: // width
                        if (stringKey[0] != 'w' || stringKey[1] != 'i' || stringKey[2] != 'd' || stringKey[3] != 't' || stringKey[4] != 'h')
                        {
                            goto default;
                        }

                        width = reader.ReadInt32();
                        continue;
                    case 6: // height
                        if (stringKey[0] != 'h' || stringKey[1] != 'e' || stringKey[2] != 'i' || stringKey[3] != 'g' || stringKey[4] != 'h' || stringKey[4] != 't')
                        {
                            goto default;
                        }

                        height = reader.ReadInt32();
                        continue;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            return new RectInt(x, y, width, height);
        }
    }
}
#endif
