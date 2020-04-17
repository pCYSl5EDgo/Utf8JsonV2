// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class CharFormatter : IJsonFormatter<char>
    {
#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, char value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, char value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static char DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadChar();
        }

        public char Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadChar();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is char innerValue))
            {
                throw new ArgumentNullException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadChar();
        }
    }
}
