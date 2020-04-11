// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Utf8Json.Formatters
{
    public sealed class TaskValueFormatter<T> : IJsonFormatter<Task<T>>
    {
        public void Serialize(ref JsonWriter writer, Task<T> value, JsonSerializerOptions options)
        {
            if (value == null) { writer.WriteNull(); return; }

            // value.Result -> wait...!
            options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Result, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Task<T> value, JsonSerializerOptions options)
        {
            if (value == null) { writer.WriteNull(); return; }

            // value.Result -> wait...!
            options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Result, options);
        }

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public Task<T>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public Task<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var v = options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
            return Task.FromResult(v);
        }

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public static Task<T>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public static Task<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var v = options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
            return Task.FromResult(v);
        }
    }
}
