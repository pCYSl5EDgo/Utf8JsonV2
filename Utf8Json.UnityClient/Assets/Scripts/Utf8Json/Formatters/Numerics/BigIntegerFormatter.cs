// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is BigInteger innerValue))
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
