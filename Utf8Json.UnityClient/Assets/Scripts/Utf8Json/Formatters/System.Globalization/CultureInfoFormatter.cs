// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    // ReSharper disable once InconsistentNaming
    public sealed class CultureInfoFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<CultureInfo?>
#else
        : IJsonFormatter<CultureInfo>
#endif
    {
#if CSHARP_8_OR_NEWER
        public CultureInfo? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public CultureInfo Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

        public static CultureInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var token = reader.GetCurrentJsonToken();
            switch (token)
            {
                case JsonToken.Null:
                    if (!reader.ReadIsNull())
                    {
                        goto default;
                    }

                    return default;
                case JsonToken.Number:
                    {
                        var lcid = reader.ReadInt32();
                        var answer = CultureInfo.GetCultureInfo(lcid);
                        return answer;
                    }
                case JsonToken.String:
                    {
                        var name = reader.ReadString();
                        Debug.Assert(name != null);
                        var answer = CultureInfo.GetCultureInfo(name);
                        return answer;
                    }
                default: throw new JsonParsingException(nameof(CultureInfo));
            }
        }

        public object
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.Write(value.LCID);
            }
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as CultureInfo, options);
        }


#if CSHARP_8_OR_NEWER
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, CultureInfo? value)
#else
        public static int CalcByteLengthForSerialization(JsonSerializerOptions options, CultureInfo value)
#endif
        {
            return value == null
                ? 4
                : Int32Formatter.CalcByteLengthForSerialization(options, value.LCID);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeSpan(JsonSerializerOptions options, CultureInfo? value, Span<byte> span)
#else
        public static void SerializeSpan(JsonSerializerOptions options, CultureInfo value, Span<byte> span)
#endif
        {
            if (value != null)
            {
                Int32Formatter.SerializeSpan(options, value.LCID, span);
            }
            else
            {
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
            }
        }
    }
}
