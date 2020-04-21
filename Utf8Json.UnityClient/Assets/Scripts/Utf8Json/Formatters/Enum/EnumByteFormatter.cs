// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed unsafe class EnumByteFormatter<T> : IObjectPropertyNameFormatter<T>, IJsonFormatter<T>
        where T : unmanaged, Enum
    {
        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var result = reader.ReadByte();
            return *(T*)&result;
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.Write(*(byte*)&value);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is T innerValue))
            {
                throw new ArgumentNullException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        private static readonly ByteKeyByteArrayValueHashTable table;
        private static readonly ByteArrayKeyUnmanagedValueHashTable<byte> deserializeTable;

        static EnumByteFormatter()
        {
            var entries = EnumFormatterHelper.CreateTable<T, byte, ByteKeyByteArrayValueHashTable.Entry>(EnumFormatterHelper.ByteFactory);
            table = new ByteKeyByteArrayValueHashTable(entries);
            var deserializeEntries = EnumFormatterHelper.CreateDeserializeTable(entries, EnumFormatterHelper.ByteConverter);
            deserializeTable = new ByteArrayKeyUnmanagedValueHashTable<byte>(deserializeEntries);
        }

        public void SerializeToPropertyName(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var number = *(byte*)&value;
            if (options.ProcessEnumAsString)
            {
                var bytes = table[number];
                if (bytes != null)
                {
                    writer.WriteRaw(bytes);
                    return;
                }
            }

            var size = ByteFormatter.CalcByteLengthForSerialization(options, number);
            var span = writer.Writer.GetSpan(2 + size);
            span[0] = (byte)'"';
            span[size + 1] = (byte)'"';
            ByteFormatter.SerializeSpan(options, number, span.Slice(1, size));
        }

        public T DeserializeFromPropertyName(ref JsonReader reader, JsonSerializerOptions options)
        {
            var number = EnumFormatterHelper.DeserializeFromPropertyNameByte(ref reader, options, deserializeTable);
            var answer = *(T*)&number;
            return answer;
        }
    }
}
