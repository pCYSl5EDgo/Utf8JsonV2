// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    // ReSharper disable once InconsistentNaming
    public sealed class DBNullFormatter : IJsonFormatter<DBNull>
    {
        public DBNull Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return DBNull.Value;
        }

        public static DBNull DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return DBNull.Value;
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
            return DBNull.Value;
        }

        public static void SerializeStatic(ref JsonWriter writer, DBNull value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(2);
            span[0] = (byte)'{';
            span[1] = (byte)'}';
            writer.Writer.Advance(2);
        }

        public void Serialize(ref JsonWriter writer, DBNull value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(2);
            span[0] = (byte)'{';
            span[1] = (byte)'}';
            writer.Writer.Advance(2);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            var span = writer.Writer.GetSpan(2);
            span[0] = (byte)'{';
            span[1] = (byte)'}';
            writer.Writer.Advance(2);
        }
    }
}
