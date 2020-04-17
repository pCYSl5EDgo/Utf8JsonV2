// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class BooleanFormatter : IJsonFormatter<bool>
    {
#pragma warning disable IDE0060
        public static void SerializeStatic(ref JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            if (value)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
                writer.Writer.Advance(4);
            }
            else
            {
                var span = writer.Writer.GetSpan(5);
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
                writer.Writer.Advance(5);
            }
        }

        public void Serialize(ref JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            if (value)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
                writer.Writer.Advance(4);
            }
            else
            {
                var span = writer.Writer.GetSpan(5);
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
                writer.Writer.Advance(5);
            }
        }

        public static bool DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadBoolean();
        }

        public bool Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadBoolean();
        }

        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, bool value)
        {
            return value ? 4 : 5;
        }

        public static void SerializeSpan(JsonSerializerOptions options, bool value, Span<byte> span)
        {
            if (value)
            {
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
            }
            else
            {
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
            }
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if(value==null)
            {
                throw new NullReferenceException();
            }

            SerializeStatic(ref writer, (bool)value, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options) ? ObjectHelper.True : ObjectHelper.False;
        }
    }
}
