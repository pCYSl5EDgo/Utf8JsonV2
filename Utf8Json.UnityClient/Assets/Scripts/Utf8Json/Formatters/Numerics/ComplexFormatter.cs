// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Numerics;

namespace Utf8Json.Formatters
{
    /// <summary>Convert to [Real, Imaginary]</summary>
    public sealed class ComplexFormatter : IJsonFormatter<Complex>
    {
        public void Serialize(ref JsonWriter writer, Complex value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Complex value, JsonSerializerOptions options)
        {
            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)'[';
            writer.Writer.Advance(1);
            writer.Write(value.Real);
            var span = writer.Writer.GetSpan(1);
            span[0] = (byte)',';
            writer.Writer.Advance(1);
            writer.Write(value.Imaginary);
            var span2 = writer.Writer.GetSpan(1);
            span2[0] = (byte)']';
            writer.Writer.Advance(1);
        }

        public Complex Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader);
        }

        public static Complex DeserializeStatic(ref JsonReader reader)
        {
            reader.ReadIsBeginArrayWithVerify();
            var real = reader.ReadDouble();
            reader.ReadIsValueSeparatorWithVerify();
            var imaginary = reader.ReadDouble();
            reader.ReadIsEndArrayWithVerify();

            return new Complex(real, imaginary);
        }
    }

    public sealed class NullableComplexFormatter : IJsonFormatter<Complex?>
    {
        public void Serialize(ref JsonWriter writer, Complex? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Complex? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                var inner = value.Value;
                var span1 = writer.Writer.GetSpan(1);
                span1[0] = (byte)'[';
                writer.Writer.Advance(1);
                writer.Write(inner.Real);
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
                writer.Write(inner.Imaginary);
                var span2 = writer.Writer.GetSpan(1);
                span2[0] = (byte)']';
                writer.Writer.Advance(1);
            }
            else
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
            }
        }

        public Complex? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader);
        }

        public static Complex? DeserializeStatic(ref JsonReader reader)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginArrayWithVerify();
            var real = reader.ReadDouble();
            reader.ReadIsValueSeparatorWithVerify();
            var imaginary = reader.ReadDouble();
            reader.ReadIsEndArrayWithVerify();

            return new Complex(real, imaginary);
        }
    }
}
