// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Unity.Mathematics;
// ReSharper disable InconsistentNaming

namespace Utf8Json.Formatters
{
    public sealed class uint3Formatter : IJsonFormatter<uint3>
    {
        public void Serialize(ref JsonWriter writer, uint3 value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public uint3 Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, uint3 value, JsonSerializerOptions options)
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
                const int sizeHint = 5;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'z';
                span[3] = (byte)'"';
                span[4] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(value.z);

            writer.WriteEndObject();
        }

        public static uint3 DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();
            var x = default(UInt32);
            var y = default(UInt32);
            var z = default(UInt32);
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
                                x = reader.ReadUInt32();
                                continue;
                            case (byte)'y':
                                y = reader.ReadUInt32();
                                continue;
                            case (byte)'z':
                                z = reader.ReadUInt32();
                                continue;
                        }

                        goto default;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            return new uint3(x, y, z);
        }
    }
}
