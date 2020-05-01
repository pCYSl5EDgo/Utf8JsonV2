// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/* THIS (.cs) FILE IS GENERATED. DO NOT CHANGE IT.
 * CHANGE THE .tt FILE INSTEAD. */

using System;

#pragma warning disable IDE0060
namespace Utf8Json.Formatters
{
    public sealed class IntPtrFormatter
    : IJsonFormatter<IntPtr>
    {
        public static void SerializeStatic(ref JsonWriter writer, IntPtr value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, IntPtr value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static IntPtr DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIntPtr();
        }

        public IntPtr Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIntPtr();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is IntPtr innerValue))
            {
                throw new NullReferenceException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIntPtr();
        }
    }

    public sealed class UIntPtrFormatter
    : IJsonFormatter<UIntPtr>
    {
        public static void SerializeStatic(ref JsonWriter writer, UIntPtr value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, UIntPtr value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static UIntPtr DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUIntPtr();
        }

        public UIntPtr Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUIntPtr();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is UIntPtr innerValue))
            {
                throw new NullReferenceException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadUIntPtr();
        }
    }

    public sealed class SingleFormatter
    : IJsonFormatter<float>
    {
        public static void SerializeStatic(ref JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static float DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSingle();
        }

        public float Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSingle();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is float innerValue))
            {
                throw new NullReferenceException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadSingle();
        }
    }

    public sealed class DoubleFormatter
    : IJsonFormatter<double>
    {
        public static void SerializeStatic(ref JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static double DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDouble();
        }

        public double Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDouble();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is double innerValue))
            {
                throw new NullReferenceException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDouble();
        }
    }

    public sealed class DateTimeFormatter
    : IJsonFormatter<DateTime>
    {
        public static void SerializeStatic(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static DateTime DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDateTime();
        }

        public DateTime Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDateTime();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is DateTime innerValue))
            {
                throw new NullReferenceException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDateTime();
        }
    }

    public sealed class DecimalFormatter
    : IJsonFormatter<decimal>
    {
        public static void SerializeStatic(ref JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public void Serialize(ref JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.Write(value);
        }

        public static decimal DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDecimal();
        }

        public decimal Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDecimal();
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is decimal innerValue))
            {
                throw new NullReferenceException();
            }

            writer.Write(innerValue);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadDecimal();
        }
    }

}
