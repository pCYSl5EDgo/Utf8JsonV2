// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Unity.Mathematics;
// ReSharper disable InconsistentNaming

namespace Utf8Json.Formatters
{
    public sealed class bool3Formatter : IJsonFormatter<bool3>
    {
        public void Serialize(ref JsonWriter writer, bool3 value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public bool3 Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, bool3 value, JsonSerializerOptions options)
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

        public static bool3 DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();
            var x = default(Boolean);
            var y = default(Boolean);
            var z = default(Boolean);
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
                                x = reader.ReadBoolean();
                                continue;
                            case (byte)'y':
                                y = reader.ReadBoolean();
                                continue;
                            case (byte)'z':
                                z = reader.ReadBoolean();
                                continue;
                        }

                        goto default;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            return new bool3(x, y, z);
        }
    }
}
