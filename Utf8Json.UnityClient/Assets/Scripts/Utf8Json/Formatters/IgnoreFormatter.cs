// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Formatters
{
    public sealed class IgnoreFormatter<T> : IJsonFormatter<T>
    {
        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(4);
            span[0] = (byte)'n';
            span[1] = (byte)'u';
            span[2] = (byte)'l';
            span[3] = (byte)'l';
            writer.Writer.Advance(4);
        }

#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
#pragma warning restore IDE0060
        {
            var span = writer.Writer.GetSpan(4);
            span[0] = (byte)'n';
            span[1] = (byte)'u';
            span[2] = (byte)'l';
            span[3] = (byte)'l';
            writer.Writer.Advance(4);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
#if CSHARP_8_OR_NEWER
            return default!;
#else
            return default;
#endif
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
#if CSHARP_8_OR_NEWER
            return default!;
#else
            return default;
#endif
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            var span = writer.Writer.GetSpan(4);
            span[0] = (byte)'n';
            span[1] = (byte)'u';
            span[2] = (byte)'l';
            span[3] = (byte)'l';
            writer.Writer.Advance(4);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.ReadNextBlock();
            return default;
        }
    }
}
