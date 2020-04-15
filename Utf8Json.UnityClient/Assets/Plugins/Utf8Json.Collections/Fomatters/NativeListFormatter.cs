// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using Unity.Collections;

namespace Utf8Json.Formatters
{
    public sealed unsafe class NativeListFormatter<T> : IJsonFormatter<NativeList<T>>
        where T : unmanaged
    {
        public void Serialize(ref JsonWriter writer, NativeList<T> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeList<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeList<T> value, JsonSerializerOptions options)
        {
            NativeArrayFormatter<T>.SerializeStatic(ref writer, value.AsArray(), options);
        }

        public static NativeList<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return new NativeList<T>(Allocator.Persistent);
            }

            reader.ReadIsBeginArrayWithVerify();
            var answer = new NativeList<T>(Allocator.Persistent);
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() == null)
            {
                DeserializeStaticWithFormatter(ref reader, options, answer);
            }
            else
            {
                var count = 0;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    var element = reader.Deserialize<T>(options, deserializer);
                    answer.Add(element);
                }
            }
            return answer;
        }

        private static void DeserializeStaticWithFormatter(ref JsonReader reader, JsonSerializerOptions options, NativeList<T> answer)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var element = formatter.Deserialize(ref reader, options);
                answer.Add(element);
            }
        }
    }
}
