// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;

namespace Utf8Json.Formatters
{
    public sealed unsafe class NullableFormatter<T> : IJsonFormatter<T?>
        where T : struct
    {
        public void Serialize(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
            }
            else
            {
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, options);
                }
                else
                {
                    writer.Serialize(value.Value, options, serializer);
                }
            }
        }

        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            return deserializer.ToPointer() == null
                ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                : reader.Deserialize<T>(options, deserializer);
        }
    }

    public sealed class StaticNullableFormatter<T, TFormatter> : IJsonFormatter<T?>
        where T : struct
        where TFormatter : class, IJsonFormatter<T?>, new()
    {
        private static readonly TFormatter underlyingFormatter;

        static StaticNullableFormatter()
        {
            underlyingFormatter = new TFormatter();
        }

        public void Serialize(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
            }
            else
            {
                underlyingFormatter.Serialize(ref writer, value.Value, options);
            }
        }

        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull()
                ? null
                : underlyingFormatter.Deserialize(ref reader, options);
        }
    }
}
