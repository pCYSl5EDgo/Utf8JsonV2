// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
#pragma warning disable IDE0060 // 未使用のパラメーターを削除します

namespace Utf8Json.Formatters
{
    public sealed class NullableStringFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<string?>
#else
        : IJsonFormatter<string>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, string? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, string value, JsonSerializerOptions options)
#endif
        {
            if (value == default)
            {
                writer.WriteNull();
            }
            else
            {
                writer.Write(value);
            }
        }

#if CSHARP_8_OR_NEWER
        public string? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public string Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadString();
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, string? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, string value, JsonSerializerOptions options)
#endif
        {
            if (value == default)
            {
                writer.WriteNull();
            }
            else
            {
                writer.Write(value);
            }
        }

#if CSHARP_8_OR_NEWER
        public static string? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static string DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return reader.ReadString();
        }
    }

    public sealed class NullableStringArrayFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<string?[]?>
#else
        : IJsonFormatter<string[]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, string?[]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, string[] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public string?[]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public string[] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, string?[]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, string[] value, JsonSerializerOptions options)
#endif
        {
            if (value == default)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteBeginArray();
            if (value.Length == 0)
            {
                goto END;
            }

            var str = value[0];
            if (str == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.Write(str);
            }

            for (var i = 1; i < value.Length; i++)
            {
                writer.WriteValueSeparator();
                str = value[i];
                if (str == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Write(str);
                }
            }

        END:
            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        public static string?[]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static string[] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var pool = ArrayPool<string>.Shared;
            var array = pool.Rent(32);
            try
            {
                var count = 0;
                while (reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (array.Length < count)
                    {
                        var tmp = pool.Rent(array.Length << 1);
                        Array.Copy(array, tmp, array.Length);
                        pool.Return(array);
                        array = tmp;
                    }

#if CSHARP_8_OR_NEWER
                    array[count - 1] = reader.ReadString()!;
#else
                    array[count - 1] = reader.ReadString();
#endif
                }

                var answer = new string[count];
                Array.Copy(array, answer, answer.Length);
                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }
    }
}
#pragma warning restore IDE0060 // 未使用のパラメーターを削除します
