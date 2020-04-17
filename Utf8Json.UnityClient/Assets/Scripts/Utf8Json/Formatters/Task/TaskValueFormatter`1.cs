// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System.Threading.Tasks;

namespace Utf8Json.Formatters
{
    public sealed unsafe class TaskValueFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Task<T>?>
#else
        : IJsonFormatter<Task<T>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Task<T>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Task<T> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Task<T>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Task<T> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

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

        public Task<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static Task<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
#if CSHARP_8_OR_NEWER
                return Task.FromResult(default(T)!);
#else
                return Task.FromResult(default(T));
#endif
            }

            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            var v = deserializer.ToPointer() == null
                ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                : reader.Deserialize<T>(options, deserializer);
            return Task.FromResult(v);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Task<T>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
