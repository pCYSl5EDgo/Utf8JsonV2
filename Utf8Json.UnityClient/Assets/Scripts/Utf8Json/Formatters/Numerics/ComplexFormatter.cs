// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
            return DeserializeStatic(ref reader, options);
        }

        public static Complex DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginArrayWithVerify();
            var real = reader.ReadDouble();
            reader.ReadIsValueSeparatorWithVerify();
            var imaginary = reader.ReadDouble();
            reader.ReadIsEndArrayWithVerify();

            return new Complex(real, imaginary);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is Complex innerValue))
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
