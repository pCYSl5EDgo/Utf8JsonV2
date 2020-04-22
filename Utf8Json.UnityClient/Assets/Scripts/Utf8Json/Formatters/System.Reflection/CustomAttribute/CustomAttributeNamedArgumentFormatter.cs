// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class CustomAttributeNamedArgumentFormatter : IJsonFormatter<CustomAttributeNamedArgument>
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is CustomAttributeNamedArgument innerValue))
            {
                throw new ArgumentNullException(nameof(value));
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public void Serialize(ref JsonWriter writer, CustomAttributeNamedArgument value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        private static readonly byte[] bytesIsField =
        {
            (byte)'{',
            (byte)'"',
            (byte)'I',
            (byte)'s',
            (byte)'F',
            (byte)'i',
            (byte)'e',
            (byte)'l',
            (byte)'d',
            (byte)'"',
            (byte)':',
            (byte)'t',
            (byte)'r',
            (byte)'u',
            (byte)'e',
            (byte)',',
            (byte)'"',
            (byte)'M',
            (byte)'e',
            (byte)'m',
            (byte)'b',
            (byte)'e',
            (byte)'r',
            (byte)'N',
            (byte)'a',
            (byte)'m',
            (byte)'e',
            (byte)'"',
            (byte)':',
        };

        private static readonly byte[] bytesFalseMemberName =
        {
            (byte)'f',
            (byte)'a',
            (byte)'l',
            (byte)'s',
            (byte)'e',
            (byte)',',
            (byte)'"',
            (byte)'M',
            (byte)'e',
            (byte)'m',
            (byte)'b',
            (byte)'e',
            (byte)'r',
            (byte)'N',
            (byte)'a',
            (byte)'m',
            (byte)'e',
            (byte)'"',
            (byte)':',
        };

        private static readonly byte[] bytesMemberInfo =
        {
            (byte)',',
            (byte)'"',
            (byte)'M',
            (byte)'e',
            (byte)'m',
            (byte)'b',
            (byte)'e',
            (byte)'r',
            (byte)'I',
            (byte)'n',
            (byte)'f',
            (byte)'o',
            (byte)'"',
            (byte)':',
        };

        private static readonly byte[] bytesTypedValue =
        {
            (byte)',',
            (byte)'"',
            (byte)'T',
            (byte)'y',
            (byte)'p',
            (byte)'e',
            (byte)'d',
            (byte)'V',
            (byte)'a',
            (byte)'l',
            (byte)'u',
            (byte)'e',
            (byte)'"',
            (byte)':',
        };

        public static void SerializeStatic(ref JsonWriter writer, CustomAttributeNamedArgument value, JsonSerializerOptions options)
        {
            if (value.IsField)
            {
                writer.WriteRaw(bytesIsField);
            }
            else
            {
                writer.WriteRaw(bytesIsField.AsSpan(0, 11));
                writer.WriteRaw(bytesFalseMemberName);
            }

            writer.Write(value.MemberName);
            writer.WriteRaw(bytesMemberInfo);
            MemberInfoFormatter.SerializeStatic(ref writer, value.MemberInfo, options);
            writer.WriteRaw(bytesTypedValue);
            CustomAttributeTypedArgumentFormatter.SerializeStatic(ref writer, value.TypedValue, options);
            writer.WriteEndObject();
        }

        public static CustomAttributeNamedArgument DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return default;
        }

        public CustomAttributeNamedArgument Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
