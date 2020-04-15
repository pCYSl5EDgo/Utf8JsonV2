// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Unity.Mathematics;
// ReSharper disable InconsistentNaming

namespace Utf8Json.Formatters
{
    public sealed class float2Formatter : IJsonFormatter<float2>
    {
        public void Serialize(ref JsonWriter writer, float2 value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public float2 Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, float2 value, JsonSerializerOptions options)
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

            writer.WriteEndObject();
        }

        public static float2 DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();
            var x = default(Single);
            var y = default(Single);
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
                                x = reader.ReadSingle();
                                continue;
                            case (byte)'y':
                                y = reader.ReadSingle();
                                continue;
                        }

                        goto default;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            return new float2(x, y);
        }
    }
}
