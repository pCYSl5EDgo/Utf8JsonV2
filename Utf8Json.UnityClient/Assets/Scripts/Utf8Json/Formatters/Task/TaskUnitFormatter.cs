// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Utf8Json.Formatters
{
    public sealed class TaskUnitFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Task?>
#else
        : IJsonFormatter<Task>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Task? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Task value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Task? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Task value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            value.Wait(); // wait!
            writer.WriteNull();
        }

#if CSHARP_8_OR_NEWER
        public Task? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Task Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Task? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Task DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (!reader.ReadIsNull()) throw new InvalidOperationException("Invalid input");

            return Task.CompletedTask;
        }
    }
}
