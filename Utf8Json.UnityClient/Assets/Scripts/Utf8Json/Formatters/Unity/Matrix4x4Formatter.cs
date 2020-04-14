// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_2018_4_OR_NEWER

using System;
using UnityEngine;

namespace Utf8Json.Formatters
{
    // ReSharper disable once InconsistentNaming
    public sealed class Matrix4x4Formatter : IJsonFormatter<Matrix4x4>
    {
        public void Serialize(ref JsonWriter writer, Matrix4x4 value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public Matrix4x4 Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Matrix4x4 value, JsonSerializerOptions options)
        {
            const int sizeHint = 7;
            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'0';
                span[4] = (byte)'0';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m00);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'0';
                span[4] = (byte)'1';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m01);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'0';
                span[4] = (byte)'2';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m02);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'0';
                span[4] = (byte)'3';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m03);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'1';
                span[4] = (byte)'0';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m10);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'1';
                span[4] = (byte)'1';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m11);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'1';
                span[4] = (byte)'2';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m12);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'1';
                span[4] = (byte)'3';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m13);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'2';
                span[4] = (byte)'0';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m20);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'2';
                span[4] = (byte)'1';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m21);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'2';
                span[4] = (byte)'2';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m22);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'2';
                span[4] = (byte)'3';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m23);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'3';
                span[4] = (byte)'0';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m30);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'3';
                span[4] = (byte)'1';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m31);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'3';
                span[4] = (byte)'2';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m32);

            {
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'m';
                span[3] = (byte)'3';
                span[4] = (byte)'3';
                span[5] = (byte)'"';
                span[6] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }
            writer.Write(value.m33);

            writer.WriteEndObject();
        }

        public static Matrix4x4 DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();
            var answer = new Matrix4x4();
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                switch (stringKey.Length)
                {
                    case 3:
                        if (stringKey[0] != 'm') goto default;

                        switch (stringKey[1])
                        {
                            case (byte)'0':
                                switch (stringKey[2])
                                {
                                    case (byte)'0': answer.m00 = reader.ReadSingle(); continue;
                                    case (byte)'1': answer.m01 = reader.ReadSingle(); continue;
                                    case (byte)'2': answer.m02 = reader.ReadSingle(); continue;
                                    case (byte)'3': answer.m03 = reader.ReadSingle(); continue;
                                }
                                break;
                            case (byte)'1':
                                switch (stringKey[2])
                                {
                                    case (byte)'0': answer.m10 = reader.ReadSingle(); continue;
                                    case (byte)'1': answer.m11 = reader.ReadSingle(); continue;
                                    case (byte)'2': answer.m12 = reader.ReadSingle(); continue;
                                    case (byte)'3': answer.m13 = reader.ReadSingle(); continue;
                                }
                                break;
                            case (byte)'2':
                                switch (stringKey[2])
                                {
                                    case (byte)'0': answer.m20 = reader.ReadSingle(); continue;
                                    case (byte)'1': answer.m21 = reader.ReadSingle(); continue;
                                    case (byte)'2': answer.m22 = reader.ReadSingle(); continue;
                                    case (byte)'3': answer.m23 = reader.ReadSingle(); continue;
                                }
                                break;
                            case (byte)'3':
                                switch (stringKey[2])
                                {
                                    case (byte)'0': answer.m30 = reader.ReadSingle(); continue;
                                    case (byte)'1': answer.m31 = reader.ReadSingle(); continue;
                                    case (byte)'2': answer.m32 = reader.ReadSingle(); continue;
                                    case (byte)'3': answer.m33 = reader.ReadSingle(); continue;
                                }
                                break;
                        }

                        goto default;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            return answer;
        }
    }
}
#endif
