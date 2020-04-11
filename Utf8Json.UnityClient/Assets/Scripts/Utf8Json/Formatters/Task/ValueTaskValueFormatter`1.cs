// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Utf8Json.Formatters
{
    public sealed class ValueTaskValueFormatter<T> : IJsonFormatter<ValueTask<T>>
    {
        public void Serialize(ref JsonWriter writer, ValueTask<T> value, JsonSerializerOptions options)
        {
            // value.Result -> wait...!
            options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Result, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTask<T> value, JsonSerializerOptions options)
        {
            // value.Result -> wait...!
            options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Result, options);
        }

        public ValueTask<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            var v = options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
            return new ValueTask<T>(v);
        }

        public static ValueTask<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var v = options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
            return new ValueTask<T>(v);
        }
    }
}
