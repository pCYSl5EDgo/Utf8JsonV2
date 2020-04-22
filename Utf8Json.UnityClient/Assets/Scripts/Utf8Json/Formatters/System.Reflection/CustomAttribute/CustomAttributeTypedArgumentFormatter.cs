// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class CustomAttributeTypedArgumentFormatter : IJsonFormatter<CustomAttributeTypedArgument>
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is CustomAttributeTypedArgument innerValue))
            {
                throw new ArgumentNullException(nameof(value));
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        private static readonly byte[] bytesArgumentType =
        {
            (byte)'{',
            (byte)'"',
            (byte)'A',
            (byte)'r',
            (byte)'g',
            (byte)'u',
            (byte)'m',
            (byte)'e',
            (byte)'n',
            (byte)'t',
            (byte)'T',
            (byte)'y',
            (byte)'p',
            (byte)'e',
            (byte)'"',
            (byte)':',
        };

        private static readonly byte[] bytesValue =
        {
            (byte)',',
            (byte)'"',
            (byte)'V',
            (byte)'a',
            (byte)'l',
            (byte)'u',
            (byte)'e',
            (byte)'"',
            (byte)':',
            (byte)'n',
            (byte)'u',
            (byte)'l',
            (byte)'l',
            (byte)'}',
        };

        public static void SerializeStatic(ref JsonWriter writer, CustomAttributeTypedArgument value, JsonSerializerOptions options)
        {
            writer.WriteRaw(bytesArgumentType);
            TypeFormatter.SerializeStatic(ref writer, value.ArgumentType, options);
            var valueObject = value.Value;
            if (valueObject != null)
            {
                writer.WriteRaw(bytesValue.AsSpan(0, 9));
                var formatter = options.Resolver.GetFormatterWithVerify(valueObject.GetType());
                formatter.SerializeTypeless(ref writer, value, options);
                writer.WriteEndObject();
            }
            else if (!options.IgnoreNullValues)
            {
                writer.WriteRaw(bytesValue);
            }
            else
            {
                writer.WriteEndObject();
            }
        }

        public void Serialize(ref JsonWriter writer, CustomAttributeTypedArgument value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static CustomAttributeTypedArgument DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return default;
        }

        public CustomAttributeTypedArgument Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
