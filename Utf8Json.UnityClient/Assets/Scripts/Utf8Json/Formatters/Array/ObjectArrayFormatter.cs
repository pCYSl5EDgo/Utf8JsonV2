// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;

namespace Utf8Json.Formatters
{
    public sealed class ObjectArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<object?[]>
#else
        : IJsonFormatter<object[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
#if CSHARP_8_OR_NEWER
            if (value is object?[] innerValue)
#else
            if(value is object[] innerValue)
#endif
            {
                SerializeStatic(ref writer, innerValue, options);
            }
            else
            {
                writer.WriteNull();
            }
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, object?[] value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, object[] value, JsonSerializerOptions options)
#endif
        {
            writer.WriteBeginArray();
            ObjectFormatter.SerializeStatic(ref writer, value[0], options);
            for (var i = 1; i < value.Length; i++)
            {
                writer.WriteValueSeparator();
                ObjectFormatter.SerializeStatic(ref writer, value[i], options);
            }
            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, object?[] value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, object[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static object?[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static object[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
#if CSHARP_8_OR_NEWER
                return Array.Empty<object?>();
#else
                return Array.Empty<object>();
#endif
            }

            reader.ReadIsBeginArrayWithVerify();
            var count = 0;
#if CSHARP_8_OR_NEWER
            var pool = ArrayPool<object?>.Shared;
#else
            var pool = ArrayPool<object>.Shared;
#endif
            var array = pool.Rent(256);
            try
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (array.Length < count)
                    {
                        var tmp = pool.Rent(count << 1);
                        Array.Copy(array, tmp, array.Length);
                        pool.Return(array);
                        array = tmp;
                    }

                    array[count - 1] = ObjectFormatter.DeserializeStatic(ref reader, options);
                }

                return array.AsSpan(0, count).ToArray();
            }
            finally
            {
                pool.Return(array);
            }
        }

#if CSHARP_8_OR_NEWER
        public object?[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
