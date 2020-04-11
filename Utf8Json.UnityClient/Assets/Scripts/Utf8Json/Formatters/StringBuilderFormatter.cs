// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace Utf8Json.Formatters
{
    public sealed class StringBuilderFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<StringBuilder?>
#else
        : IJsonFormatter<StringBuilder>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, StringBuilder? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, StringBuilder value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, StringBuilder? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, StringBuilder value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.Write(value.ToString());
        }

#if CSHARP_8_OR_NEWER
        public StringBuilder? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public StringBuilder Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
        public static StringBuilder? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static StringBuilder DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadIsNull() ? default : new StringBuilder(reader.ReadString());
        }
    }
}
