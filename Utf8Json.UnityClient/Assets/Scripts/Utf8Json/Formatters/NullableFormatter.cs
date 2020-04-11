// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Formatters
{
    public sealed class NullableFormatter<T> : IJsonFormatter<T?>
        where T : struct
    {
        public void Serialize(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, options);
            }
        }

        public static void SerializeStatic(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, options);
            }
        }

        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            return options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
        }

        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            return options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
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
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                underlyingFormatter.Serialize(ref writer, value.Value, options);
            }
        }

        public T? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            return underlyingFormatter.Deserialize(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                underlyingFormatter.Serialize(ref writer, value.Value, options);
            }
        }

        public static T? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            return underlyingFormatter.Deserialize(ref reader, options);
        }
    }
}
