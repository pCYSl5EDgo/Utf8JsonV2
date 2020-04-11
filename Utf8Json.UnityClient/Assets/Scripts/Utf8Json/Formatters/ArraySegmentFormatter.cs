// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;

namespace Utf8Json.Formatters
{
    public sealed class ArraySegmentFormatter<T> : IJsonFormatter<ArraySegment<T>>
    {
        public void Serialize(ref JsonWriter writer, ArraySegment<T> value, JsonSerializerOptions options)
            => SerializeStatic(ref writer, value, options);

        public static void SerializeStatic(ref JsonWriter writer, ArraySegment<T> value, JsonSerializerOptions options)
        {
            if (value.Array == null)
            {
                writer.WriteNull();
                return;
            }

            var span = value.AsSpan();

            writer.WriteBeginArray();
            if (span.IsEmpty)
            {
                goto END;
            }

            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, span[0], options);

            for (var i = 1; i < span.Length; i++)
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, span[i], options);
            }

        END:
            writer.WriteEndArray();
        }

        public ArraySegment<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
            => DeserializeStatic(ref reader, options);

        public static ArraySegment<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            var count = 0;
            var formatter = options.Resolver.GetFormatterWithVerify<T>();

            var workingArea = ArrayPool<T>.Shared.Rent(64);
            try
            {
                reader.ReadIsBeginArrayWithVerify();
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (workingArea.Length < count)
                    {
                        var tmp = ArrayPool<T>.Shared.Rent(count << 1);
                        Array.Copy(workingArea, tmp, workingArea.Length);
                        ArrayPool<T>.Shared.Return(workingArea);
                        workingArea = tmp;
                    }

                    workingArea[count - 1] = formatter.Deserialize(ref reader, options);
                }

                var answer = new T[count];
                Array.Copy(workingArea, answer, count);
                return new ArraySegment<T>(answer, 0, answer.Length);
            }
            finally
            {
                ArrayPool<T>.Shared.Return(workingArea);
            }
        }
    }

    public sealed class NullableArraySegmentFormatter<T> : IJsonFormatter<ArraySegment<T>?>
    {
        public static void SerializeStatic(ref JsonWriter writer, ArraySegment<T>? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ArraySegmentFormatter<T>.SerializeStatic(ref writer, value.Value, options);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public void Serialize(ref JsonWriter writer, ArraySegment<T>? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ArraySegmentFormatter<T>.SerializeStatic(ref writer, value.Value, options);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public static ArraySegment<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default(ArraySegment<T>?) : ArraySegmentFormatter<T>.DeserializeStatic(ref reader, options);
        }

        public ArraySegment<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default(ArraySegment<T>?) : ArraySegmentFormatter<T>.DeserializeStatic(ref reader, options);
        }
    }
}
