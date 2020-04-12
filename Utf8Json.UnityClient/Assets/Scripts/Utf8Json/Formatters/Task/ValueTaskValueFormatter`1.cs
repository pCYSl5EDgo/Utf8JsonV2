// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System.Threading.Tasks;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ValueTaskValueFormatter<T> : IJsonFormatter<ValueTask<T>>
    {
        public void Serialize(ref JsonWriter writer, ValueTask<T> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTask<T> value, JsonSerializerOptions options)
        {
            // value.Result -> wait...!
            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Result, options);
            }
            else
            {
                writer.Serialize(value.Result, options, serializer);
            }
        }

        public ValueTask<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTask<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            var v = deserializer.ToPointer() == null
                ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                : reader.Deserialize<T>(options, deserializer);
            return new ValueTask<T>(v);
        }
    }
}
