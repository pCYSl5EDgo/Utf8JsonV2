// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Formatters
{
    public sealed unsafe class NameEnumFormatter<T> : IJsonFormatter<T>, IObjectPropertyNameFormatter<T>
        where T : unmanaged, Enum
    {
        private static readonly ByteArrayStringEnumValueHashTable<T> nameValueMapping;
        private static readonly Dictionary<T, string> valueNameMapping;

        static NameEnumFormatter()
        {
            var type = typeof(T);

            var fieldInfos = type.GetFields().Where(fi => fi.FieldType == type).ToArray();
            var values = new T[fieldInfos.Length];
            var names = new string[fieldInfos.Length];
            var nameBytes = new byte[fieldInfos.Length][];
            var pool = ArrayPool<byte>.Shared;
            var buffer = pool.Rent(256);
            try
            {
                for (var index = 0; index < fieldInfos.Length; index++)
                {
                    var item = fieldInfos[index];
                    ref var value = ref values[index];
                    var objValue = item.GetValue(null);
                    Debug.Assert(objValue != null, nameof(objValue) + " != null");
                    value = (T)objValue;
                    var enumMember = item.GetCustomAttributes(typeof(EnumMemberAttribute), true)
                        .OfType<EnumMemberAttribute>()
                        .FirstOrDefault();
                    var name = enumMember?.Value;

                    if (name == null)
                    {
                        var dataMember = item.GetCustomAttributes(typeof(DataMemberAttribute), true)
                            .OfType<DataMemberAttribute>()
                            .FirstOrDefault();
                        name = dataMember?.Name;
                    }

                    if (name == null)
                    {
                        name = Enum.GetName(type, objValue);
                    }

                    names[index] = name ?? throw new InvalidOperationException("name should not be null.");

                    #region Encode Name
                    var max = name.Length * 3;
                    if (buffer.Length < max)
                    {
                        pool.Return(buffer);
                        buffer = pool.Rent(max);
                    }

                    var from = 0;
                    var offset = 0;
                    for (var i = 0; i < name.Length; i++)
                    {
                        byte escapeChar;
                        switch (name[i])
                        {
                            case '"':
                                escapeChar = (byte)'"';
                                break;
                            case '\\':
                                escapeChar = (byte)'\\';
                                break;
                            case '\b':
                                escapeChar = (byte)'b';
                                break;
                            case '\f':
                                escapeChar = (byte)'f';
                                break;
                            case '\n':
                                escapeChar = (byte)'n';
                                break;
                            case '\r':
                                escapeChar = (byte)'r';
                                break;
                            case '\t':
                                escapeChar = (byte)'t';
                                break;

                            #region Other
                            case (char)0:
                            case (char)1:
                            case (char)2:
                            case (char)3:
                            case (char)4:
                            case (char)5:
                            case (char)6:
                            case (char)7:
                            case (char)11:
                            case (char)14:
                            case (char)15:
                            case (char)16:
                            case (char)17:
                            case (char)18:
                            case (char)19:
                            case (char)20:
                            case (char)21:
                            case (char)22:
                            case (char)23:
                            case (char)24:
                            case (char)25:
                            case (char)26:
                            case (char)27:
                            case (char)28:
                            case (char)29:
                            case (char)30:
                            case (char)31:
                            case (char)32:
                            case (char)33:
                            case (char)35:
                            case (char)36:
                            case (char)37:
                            case (char)38:
                            case (char)39:
                            case (char)40:
                            case (char)41:
                            case (char)42:
                            case (char)43:
                            case (char)44:
                            case (char)45:
                            case (char)46:
                            case (char)47:
                            case (char)48:
                            case (char)49:
                            case (char)50:
                            case (char)51:
                            case (char)52:
                            case (char)53:
                            case (char)54:
                            case (char)55:
                            case (char)56:
                            case (char)57:
                            case (char)58:
                            case (char)59:
                            case (char)60:
                            case (char)61:
                            case (char)62:
                            case (char)63:
                            case (char)64:
                            case (char)65:
                            case (char)66:
                            case (char)67:
                            case (char)68:
                            case (char)69:
                            case (char)70:
                            case (char)71:
                            case (char)72:
                            case (char)73:
                            case (char)74:
                            case (char)75:
                            case (char)76:
                            case (char)77:
                            case (char)78:
                            case (char)79:
                            case (char)80:
                            case (char)81:
                            case (char)82:
                            case (char)83:
                            case (char)84:
                            case (char)85:
                            case (char)86:
                            case (char)87:
                            case (char)88:
                            case (char)89:
                            case (char)90:
                            case (char)91:
                            default:
                                #endregion
                                continue;
                        }

                        max += 2;
                        if (buffer.Length < max)
                        {
                            var tmp = pool.Rent(max);
                            fixed (byte* srcPtr = &buffer[0])
                            fixed (byte* dstPtr = &tmp[0])
                            {
                                Buffer.MemoryCopy(srcPtr, dstPtr, buffer.Length, buffer.Length);
                            }

                            pool.Return(buffer);
                            buffer = tmp;
                        }

                        offset += StringEncoding.Utf8.GetBytes(name, from, i - from, buffer, offset);
                        from = i + 1;
                        buffer[offset++] = (byte)'\\';
                        buffer[offset++] = escapeChar;
                    }

                    if (from != name.Length)
                    {
                        offset += StringEncoding.Utf8.GetBytes(name, from, name.Length - from, buffer, offset);
                    }

                    nameBytes[index] = new byte[offset];
                    fixed (byte* srcPtr = &buffer[0])
                    fixed (byte* dstPtr = &nameBytes[index][0])
                    {
                        Buffer.MemoryCopy(srcPtr, dstPtr, offset, offset);
                    }
                    #endregion
                }
            }
            finally
            {
                pool.Return(buffer);
            }

            nameValueMapping = new ByteArrayStringEnumValueHashTable<T>(nameBytes, values);
            valueNameMapping = new Dictionary<T, string>(nameBytes.Length);

            for (var index = 0; index < nameBytes.Length; index++)
            {
                valueNameMapping[values[index]] = names[index];
            }
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
#pragma warning restore IDE0060
        {
            if (!valueNameMapping.TryGetValue(value, out var name))
            {
                name = value.ToString(); // fallback for flags etc. But Enum.ToString is slow...
            }

            writer.Write(name);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var token = reader.GetCurrentJsonToken();

            if (token != JsonToken.String)
            {
                throw new InvalidOperationException("Can't parse JSON to Enum format.");
            }

            // avoid string decoding if possible.
            var key = reader.ReadNotNullStringSegmentRaw();

            if (nameValueMapping.TryGetValue(key, out var value))
            {
                return value;
            }
#if SPAN_BUILTIN
            var str = StringEncoding.Utf8.GetString(key);
#else
            string str;
            unsafe
            {
                fixed (byte* ptr = &key[0])
                {
                    str = StringEncoding.Utf8.GetString(ptr, key.Length);
                }
            }
#endif
            value = (T)Enum.Parse(typeof(T), str); // Enum.Parse is slow
            return value;
        }

        public void SerializeToPropertyName(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            Serialize(ref writer, value, options);
        }

        public static void SerializeToPropertyNameStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public T DeserializeFromPropertyName(ref JsonReader reader, JsonSerializerOptions options)
        {
            return Deserialize(ref reader, options);
        }

        public static T DeserializeFromPropertyNameStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class ValueEnumFormatter<T> : IJsonFormatter<T>, IObjectPropertyNameFormatter<T>
        where T : unmanaged, Enum
    {
        private static readonly JsonSerializeAction<T> defaultSerializeByUnderlyingValue;
        private static readonly JsonDeserializeFunc<T> defaultDeserializeByUnderlyingValue;

        static ValueEnumFormatter()
        {
            // boxed... or generate...
#if ENABLE_IL2CPP
            var serialize = EnumFormatterHelper.GetSerializeDelegate(typeof(T));
            defaultSerializeByUnderlyingValue = (ref JsonWriter writer, T value, JsonSerializerOptions _) => serialize(ref writer, value, _);
            var deserialize = EnumFormatterHelper.GetDeserializeDelegate(typeof(T));
            defaultDeserializeByUnderlyingValue = (ref JsonReader reader, JsonSerializerOptions _) => (T)deserialize.Invoke(ref reader, _);
#else
            var serialize = Emit.EnumFormatterHelper.GetSerializeDelegate(typeof(T));
            defaultSerializeByUnderlyingValue = (JsonSerializeAction<T>)(serialize);
            var deserialize = Emit.EnumFormatterHelper.GetDeserializeDelegate(typeof(T));
            defaultDeserializeByUnderlyingValue = (JsonDeserializeFunc<T>)deserialize;
#endif
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            defaultSerializeByUnderlyingValue(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            defaultSerializeByUnderlyingValue(ref writer, value, options);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var token = reader.GetCurrentJsonToken();

            if (token != JsonToken.Number)
                throw new InvalidOperationException("Can't parse JSON to Enum format.");

            return defaultDeserializeByUnderlyingValue(ref reader, options);
        }

        public void SerializeToPropertyName(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(1);
            span[0] = (byte)'"';
            writer.Writer.Advance(1);
            Serialize(ref writer, value, options);
            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)'"';
            writer.Writer.Advance(1);
        }

        public static void SerializeToPropertyNameStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var span = writer.Writer.GetSpan(1);
            span[0] = (byte)'"';
            writer.Writer.Advance(1);
            SerializeStatic(ref writer, value, options);
            var span1 = writer.Writer.GetSpan(1);
            span1[0] = (byte)'"';
            writer.Writer.Advance(1);
        }

        public T DeserializeFromPropertyName(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeFromPropertyNameStatic(ref reader, options);
        }

        public static T DeserializeFromPropertyNameStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String)
            {
                throw new InvalidOperationException("Can't parse JSON to Enum format.");
            }

            reader.Reader.Advance(1); // skip \""
            var t = DeserializeStatic(ref reader, options); // token is Number
            reader.SkipWhiteSpace();
            reader.Reader.Advance(1); // skip \""
            return t;
        }
    }
}
