// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class GuidFormatter : IJsonFormatter<Guid>, IObjectPropertyNameFormatter<Guid>
    {
        public void Serialize(ref JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(38);
            span[0] = (byte)'\"';

            new GuidBits(ref value).Write(span.Slice(1)); // len = 36

            span[37] = (byte)'\"';
        }

        public Guid Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.SkipWhiteSpace();
            var segment = reader.ReadNotNullStringSegmentRaw();
            return new GuidBits(segment).Value;
        }

#pragma warning disable IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        public static void SerializeStatic(ref JsonWriter writer, Guid value, JsonSerializerOptions options)
#pragma warning restore IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        {
            var span = writer.Writer.GetSpan(38);
            span[0] = (byte)'\"';

            new GuidBits(ref value).Write(span.Slice(1)); // len = 36

            span[37] = (byte)'\"';
        }

#pragma warning disable IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        public static Guid DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        {
            reader.SkipWhiteSpace();
            var segment = reader.ReadNotNullStringSegmentRaw();
            return new GuidBits(segment).Value;
        }

        public void SerializeToPropertyName(ref JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            Serialize(ref writer, value, options);
        }

        public Guid DeserializeFromPropertyName(ref JsonReader reader, JsonSerializerOptions options)
        {
            return Deserialize(ref reader, options);
        }
    }

    public sealed class NullableGuidFormatter : IJsonFormatter<Guid?>
    {
        public void Serialize(ref JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                var span = writer.Writer.GetSpan(38);
                span[0] = (byte)'\"';

                var v = value.Value;
                new GuidBits(ref v).Write(span.Slice(1)); // len = 36

                span[37] = (byte)'\"';
            }
            else
            {
                writer.WriteNull();
            }
        }

        public Guid? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull()) return null;
            var segment = reader.ReadNotNullStringSegmentRaw();
            return new GuidBits(segment).Value;
        }

#pragma warning disable IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        public static void SerializeStatic(ref JsonWriter writer, Guid? value, JsonSerializerOptions options)
#pragma warning restore IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        {
            if (value.HasValue)
            {
                var span = writer.Writer.GetSpan(38);
                span[0] = (byte)'\"';

                var v = value.Value;
                new GuidBits(ref v).Write(span.Slice(1)); // len = 36

                span[37] = (byte)'\"';
            }
            else
            {
                writer.WriteNull();
            }
        }

#pragma warning disable IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        public static Guid? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore IDE0060 // ���g�p�̃p�����[�^�[���폜���܂�
        {
            if (reader.ReadIsNull()) return default;
            var segment = reader.ReadNotNullStringSegmentRaw();
            return new GuidBits(segment).Value;
        }
    }
}
