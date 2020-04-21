// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public static class EnumFormatterHelper
    {
        public static TEntry[] CreateTable<TEnum, TKey, TEntry>(Func<TKey, byte[], TEntry> factory)
            where TEnum : unmanaged, Enum
            where TKey : unmanaged
            where TEntry : struct
        {
            var constants = typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public);
            var entries = new TEntry[constants.Length];
            for (var index = 0; index < constants.Length; index++)
            {
                var constant = constants[index];
                var rawValue = constant.GetRawConstantValue();
                if (!(rawValue is TKey key))
                {
                    throw new NullReferenceException(nameof(constant));
                }

                string encodedName = constant.Name;
                var customAttributes = constant.GetCustomAttributes();
                foreach (var attribute in customAttributes)
                {
                    if (!(attribute is System.Runtime.Serialization.DataMemberAttribute dataMemberAttribute))
                    {
                        continue;
                    }

                    encodedName = dataMemberAttribute.Name;
                    break;
                }
                var bytes = JsonSerializer.Serialize(encodedName);
                entries[index] = factory(key, bytes);
            }

            return entries;
        }

        public delegate TEntry EntryConverter<TBaseEntry, out TEntry>(in TBaseEntry baseEntry);

        public static TEntry[] CreateDeserializeTable<TBaseEntry, TEntry>(TBaseEntry[] entries, EntryConverter<TBaseEntry, TEntry> converter)
        {
            var deserializeEntries = new TEntry[entries.Length];
            for (var i = 0; i < entries.Length; i++)
            {
                ref readonly var entry = ref entries[i];
                deserializeEntries[i] = converter(in entry);
            }

            return deserializeEntries;
        }

        public static UInt64KeyByteArrayValueHashTable.Entry UInt64Factory(ulong key, byte[] value)
        {
            return new UInt64KeyByteArrayValueHashTable.Entry(key, value);
        }

        public static UInt32KeyByteArrayValueHashTable.Entry UInt32Factory(uint key, byte[] value)
        {
            return new UInt32KeyByteArrayValueHashTable.Entry(key, value);
        }

        public static UInt16KeyByteArrayValueHashTable.Entry UInt16Factory(ushort key, byte[] value)
        {
            return new UInt16KeyByteArrayValueHashTable.Entry(key, value);
        }

        public static ByteKeyByteArrayValueHashTable.Entry ByteFactory(byte key, byte[] value)
        {
            return new ByteKeyByteArrayValueHashTable.Entry(key, value);
        }

        public static ByteKeyByteArrayValueHashTable.Entry SByteFactory(sbyte key, byte[] value)
        {
            return new ByteKeyByteArrayValueHashTable.Entry((byte)key, value);
        }

        public static UInt16KeyByteArrayValueHashTable.Entry Int16Factory(short key, byte[] value)
        {
            return new UInt16KeyByteArrayValueHashTable.Entry((ushort)key, value);
        }

        public static UInt32KeyByteArrayValueHashTable.Entry Int32Factory(int key, byte[] value)
        {
            return new UInt32KeyByteArrayValueHashTable.Entry((uint)key, value);
        }

        public static UInt64KeyByteArrayValueHashTable.Entry Int64Factory(long key, byte[] value)
        {
            return new UInt64KeyByteArrayValueHashTable.Entry((ulong)key, value);
        }

        public static ByteArrayKeyUnmanagedValueHashTable<ulong>.Entry UInt64Converter(in UInt64KeyByteArrayValueHashTable.Entry entry)
        {
            var bytes = entry.Value.AsSpan(1, entry.Value.Length - 2).ToArray();
            return new ByteArrayKeyUnmanagedValueHashTable<ulong>.Entry(bytes, entry.Key);
        }

        public static ulong DeserializeFromPropertyNameUInt64(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<ulong> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadUInt64();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static long DeserializeFromPropertyNameInt64(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<ulong> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? (long)number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadInt64();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static ByteArrayKeyUnmanagedValueHashTable<uint>.Entry UInt32Converter(in UInt32KeyByteArrayValueHashTable.Entry entry)
        {
            var bytes = entry.Value.AsSpan(1, entry.Value.Length - 2).ToArray();
            return new ByteArrayKeyUnmanagedValueHashTable<uint>.Entry(bytes, entry.Key);
        }

        public static uint DeserializeFromPropertyNameUInt32(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<uint> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadUInt32();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static int DeserializeFromPropertyNameInt32(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<uint> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? (int)number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadInt32();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static ByteArrayKeyUnmanagedValueHashTable<ushort>.Entry UInt16Converter(in UInt16KeyByteArrayValueHashTable.Entry entry)
        {
            var bytes = entry.Value.AsSpan(1, entry.Value.Length - 2).ToArray();
            return new ByteArrayKeyUnmanagedValueHashTable<ushort>.Entry(bytes, entry.Key);
        }

        public static ushort DeserializeFromPropertyNameUInt16(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<ushort> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadUInt16();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static short DeserializeFromPropertyNameInt16(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<ushort> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? (short)number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadInt16();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static ByteArrayKeyUnmanagedValueHashTable<byte>.Entry ByteConverter(in ByteKeyByteArrayValueHashTable.Entry entry)
        {
            var bytes = entry.Value.AsSpan(1, entry.Value.Length - 2).ToArray();
            return new ByteArrayKeyUnmanagedValueHashTable<byte>.Entry(bytes, entry.Key);
        }

        public static byte DeserializeFromPropertyNameByte(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<byte> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadByte();
                reader.Reader.Advance(1);
                return number;
            }
        }

        public static sbyte DeserializeFromPropertyNameSByte(ref JsonReader reader, JsonSerializerOptions options, ByteArrayKeyUnmanagedValueHashTable<byte> deserializeTable)
        {
            reader.SkipWhiteSpace();
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new JsonParsingException("Property should be string.");
            }

            if (options.ProcessEnumAsString)
            {
                var name = reader.ReadNotNullStringSegmentRaw();
                return deserializeTable.TryGetValue(name, out var number) ? (sbyte)number : default;
            }
            else
            {
                reader.Reader.Advance(1);
                var number = reader.ReadSByte();
                reader.Reader.Advance(1);
                return number;
            }
        }
    }
}
