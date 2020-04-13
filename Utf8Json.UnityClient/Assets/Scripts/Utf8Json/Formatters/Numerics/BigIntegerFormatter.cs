// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Numerics;

namespace Utf8Json.Formatters
{
    /// <summary>
    /// JSON.NET writes Integer format, not compatible.
    /// </summary>
    public sealed class BigIntegerFormatter : IJsonFormatter<BigInteger>
    {
        public void Serialize(ref JsonWriter writer, BigInteger value, JsonSerializerOptions options)
        {
            // JSON.NET writes Integer format, not compatible.
            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SerializeStatic(ref JsonWriter writer, BigInteger value, JsonSerializerOptions options)
        {
            // JSON.NET writes Integer format, not compatible.
            writer.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public BigInteger Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            var s = reader.ReadString();
            return BigInteger.Parse(s, CultureInfo.InvariantCulture);
        }

        public static BigInteger DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var s = reader.ReadString();
            return BigInteger.Parse(s, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// JSON.NET writes Integer format, not compatible.
    /// </summary>
    public sealed class NullableBigIntegerFormatter : IJsonFormatter<BigInteger?>
    {
        public void Serialize(ref JsonWriter writer, BigInteger? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.Write(value.Value.ToString(CultureInfo.InvariantCulture));
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

        /// <summary>
        /// JSON.NET writes Integer format, not compatible.
        /// </summary>
        public static void SerializeStatic(ref JsonWriter writer, BigInteger? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.Write(value.Value.ToString(CultureInfo.InvariantCulture));
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

        public BigInteger? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            var s = reader.ReadString();
            if (s == null) return null;
            return BigInteger.Parse(s, CultureInfo.InvariantCulture);
        }

        public static BigInteger? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var s = reader.ReadString();
            if (s == null) return null;
            return BigInteger.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
