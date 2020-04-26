// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed class ObjectFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<object?>
#else
        : IJsonFormatter<object>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                var span = writer.Writer.GetSpan(2);
                span[0] = (byte)'{';
                span[1] = (byte)'}';
                writer.Writer.Advance(2);
            }
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                switch (value)
                {
                    case string str:
                        NullableStringFormatter.SerializeStatic(ref writer, str, options);
                        return;
                    case int i32:
                        writer.Write(i32);
                        return;
                    case bool b:
                        writer.Write(b);
                        return;
                }

                var targetType = value.GetType();
                if (targetType == typeof(object))
                {
                    var span = writer.Writer.GetSpan(2);
                    span[0] = (byte)'{';
                    span[1] = (byte)'}';
                    writer.Writer.Advance(2);
                    return;
                }

                var formatter = options.Resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref writer, value, options);
            }
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static object? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static object DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.SkipWhiteSpace();
            switch (reader.GetCurrentJsonToken())
            {
                case JsonToken.BeginObject:
                    return StringKeyObjectValueDictionaryFormatter.DeserializeStatic(ref reader, options);
                case JsonToken.BeginArray:
                    return ObjectArrayFormatter.DeserializeStatic(ref reader, options);
                case JsonToken.Number:
                    return reader.ReadDouble();
                case JsonToken.String:
                    return reader.ReadString();
                case JsonToken.True:
                    reader.ReadNextBlock();
                    return ObjectHelper.True;
                case JsonToken.False:
                    reader.ReadNextBlock();
                    return ObjectHelper.False;
                case JsonToken.Null:
                    reader.ReadNextBlock();
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if CSHARP_8_OR_NEWER
        public object? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
