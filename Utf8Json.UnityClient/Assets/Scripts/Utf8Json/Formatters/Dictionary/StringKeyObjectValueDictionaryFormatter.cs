// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Utf8Json.Formatters
{
    public sealed class StringKeyObjectValueDictionaryFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Dictionary<string, object?>>
    {
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (!(value is Dictionary<string, object?> dictionary))
#else
        : IJsonFormatter<Dictionary<string, object>>
    {
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (!(value is Dictionary<string, object> dictionary))
#endif
            {
                writer.WriteNull();
                return;
            }
            
            SerializeStatic(ref writer, dictionary, options);
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
        public static void SerializeStatic(ref JsonWriter writer, Dictionary<string, object?> value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
#endif
        {
            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyObject();
                return;
            }

            ++writer.Depth;
            SerializeExtensionDataStaticDetectIsFirst(value, ref writer, options, true);
            writer.WriteEndObject();
            --writer.Depth;
        }

#if CSHARP_8_OR_NEWER
        public static bool SerializeExtensionDataStaticDetectIsFirst(Dictionary<string, object?> value, ref JsonWriter writer, JsonSerializerOptions options, bool isFirst)
#else
        public static bool SerializeExtensionDataStaticDetectIsFirst(Dictionary<string, object> value, ref JsonWriter writer, JsonSerializerOptions options, bool isFirst)
#endif
        {
            var enumerator = value.GetEnumerator();
            try
            {
                var ignoreNull = options.IgnoreNullValues;
                ref var bufferWriter = ref writer.Writer;
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    var currentValue = current.Value;
                    if (ignoreNull && currentValue is null)
                    {
                        continue;
                    }

                    var nameLength = NullableStringFormatter.CalcByteLength(current.Key);
                    {
                        var span = bufferWriter.GetSpan(nameLength + 2);
                        if (isFirst)
                        {
                            isFirst = false;
                            span[0] = (byte)'{';
                        }
                        else
                        {
                            span[0] = (byte)',';
                        }

                        NullableStringFormatter.SerializeSpanNotNull(current.Key, span.Slice(1, nameLength));
                        span[nameLength + 1] = (byte)':';
                        bufferWriter.Advance(nameLength + 2);
                    }

                    ObjectFormatter.SerializeStatic(ref writer, currentValue, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

            return isFirst;
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Dictionary<string, object?> value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static Dictionary<string, object?> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Dictionary<string, object> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
#if CSHARP_8_OR_NEWER
            var answer = new Dictionary<string, object?>();
#else
            var answer = new Dictionary<string, object>();
#endif
            if (reader.ReadIsNull())
            {
                return answer;
            }

            reader.ReadIsBeginObjectWithVerify();
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var key = reader.ReadPropertyName();
                answer[key] = ObjectFormatter.DeserializeStatic(ref reader, options);
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public Dictionary<string, object?> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Dictionary<string, object> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
