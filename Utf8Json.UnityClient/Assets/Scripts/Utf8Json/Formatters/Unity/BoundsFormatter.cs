// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_2018_4_OR_NEWER

using System;
using StaticFunctionPointerHelper;
using UnityEngine;

namespace Utf8Json.Formatters
{
    public sealed unsafe class BoundsFormatter : IJsonFormatter<Bounds>
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

            var serializer = options.Resolver.GetSerializeStatic<Vector3>();
            if (serializer.ToPointer() == null)
            {
                SerializeStaticWithFormatter(ref writer, value.center, value.size, options);
            }
            else
            {
                writer.Serialize(value.center, options, serializer);

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

                writer.Serialize(value.size, options, serializer);
            }

            writer.WriteEndObject();
        }

        private static void SerializeStaticWithFormatter(ref JsonWriter writer, Vector3 center, Vector3 size, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<Vector3>();
            formatter.Serialize(ref writer, center, options);

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

            formatter.Serialize(ref writer, size, options);
        }

        public static Bounds DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("type code is null, struct not supported");
            }

            reader.ReadIsBeginObjectWithVerify();

            var deserializer = options.Resolver.GetDeserializeStatic<Vector3>();
            if (deserializer.ToPointer() == null)
            {
                return DeserializeStaticWithFormatter(ref reader, options);
            }

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

                        size = reader.Deserialize<Vector2>(options, deserializer);
                        break;
                    case 6: // center
                        if (stringKey[0] != 'c' || stringKey[1] != 'e' || stringKey[2] != 'n' || stringKey[3] != 't' || stringKey[4] != 'e' || stringKey[5] != 'r')
                        {
                            goto default;
                        }

                        center = reader.Deserialize<Vector2>(options, deserializer);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            var answer = new Bounds(center, size);
            return answer;
        }

        private static Bounds DeserializeStaticWithFormatter(ref JsonReader reader, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<Vector3>();
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

                        size = formatter.Deserialize(ref reader, options);
                        break;
                    case 6: // center
                        if (stringKey[0] != 'c' || stringKey[1] != 'e' || stringKey[2] != 'n' || stringKey[3] != 't' || stringKey[4] != 'e' || stringKey[5] != 'r')
                        {
                            goto default;
                        }

                        center = formatter.Deserialize(ref reader, options);
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
