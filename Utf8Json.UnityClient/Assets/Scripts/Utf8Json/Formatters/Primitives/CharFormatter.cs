// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Formatters
{
    public sealed class CharFormatter : IJsonFormatter<char>
    {
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
    }
}
