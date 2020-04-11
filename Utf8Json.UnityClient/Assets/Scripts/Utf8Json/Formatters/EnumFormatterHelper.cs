// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#if ENABLE_IL2CPP
using System;

namespace Utf8Json.Formatters.Internal
{
    internal static class EnumFormatterHelper
    {
        public static JsonSerializeAction<object> GetSerializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            if (underlyingType == typeof(byte))
            {
                return JsonSerializeActionByte;
            }

            if (underlyingType == typeof(sbyte))
            {
                return JsonSerializeActionSByte;
            }

            if (underlyingType == typeof(short))
            {
                return JsonSerializeActionInt16;
            }

            if (underlyingType == typeof(ushort))
            {
                return JsonSerializeActionUInt16;
            }

            if (underlyingType == typeof(int))
            {
                return JsonSerializeActionInt32;
            }

            if (underlyingType == typeof(uint))
            {
                return JsonSerializeActionUInt32;
            }

            if (underlyingType == typeof(long))
            {
                return JsonSerializeActionInt64;
            }

            if (underlyingType == typeof(ulong))
            {
                return JsonSerializeActionUInt64;
            }
            throw new InvalidOperationException("Type is not Enum. Type:" + type);
        }

        private static void JsonSerializeActionUInt64(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((ulong)value);
        }

        private static void JsonSerializeActionInt64(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((long)value);
        }

        private static void JsonSerializeActionUInt32(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((uint)value);
        }

        private static void JsonSerializeActionInt32(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((int)value);
        }

        private static void JsonSerializeActionUInt16(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((uint)(ushort)value);
        }

        private static void JsonSerializeActionInt16(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((int)(short)value);
        }

        private static void JsonSerializeActionSByte(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((int)(sbyte)value);
        }

        private static void JsonSerializeActionByte(ref JsonWriter writer, object value, JsonSerializerOptions _)
        {
            writer.Write((uint)(byte)value);
        }

        public static JsonDeserializeFunc<object> GetDeserializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);
            if (underlyingType == typeof(byte))
            {
                return JsonDeserializeFuncByte;
            }

            if (underlyingType == typeof(sbyte))
            {
                return JsonDeserializeFuncSByte;
            }

            if (underlyingType == typeof(short))
            {
                return JsonDeserializeFuncInt16;
            }

            if (underlyingType == typeof(ushort))
            {
                return JsonDeserializeFuncUInt16;
            }

            if (underlyingType == typeof(int))
            {
                return JsonDeserializeFuncInt32;
            }

            if (underlyingType == typeof(uint))
            {
                return JsonDeserializeFuncUInt32;
            }

            if (underlyingType == typeof(long))
            {
                return JsonDeserializeFuncInt64;
            }

            if (underlyingType == typeof(ulong))
            {
                return JsonDeserializeFuncUInt64;
            }

            throw new InvalidOperationException("Type is not Enum. Type:" + type);
        }

        private static object JsonDeserializeFuncUInt64(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadUInt64();
        }

        private static object JsonDeserializeFuncInt64(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadInt64();
        }

        private static object JsonDeserializeFuncUInt32(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadUInt32();
        }

        private static object JsonDeserializeFuncInt32(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadInt32();
        }

        private static object JsonDeserializeFuncUInt16(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadUInt16();
        }

        private static object JsonDeserializeFuncInt16(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadInt16();
        }

        private static object JsonDeserializeFuncSByte(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadSByte();
        }

        private static object JsonDeserializeFuncByte(ref JsonReader reader, JsonSerializerOptions _)
        {
            return reader.ReadByte();
        }
    }
}
#endif
