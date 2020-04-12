// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Formatters
{
    public sealed class IgnoreFormatter<T> : IJsonFormatter<T>
    {
        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNull();
        }

#pragma warning disable IDE0060 // 未使用のパラメーターを削除します
        public static void SerializeStatic(ref JsonWriter writer, T value, JsonSerializerOptions options)
#pragma warning restore IDE0060 // 未使用のパラメーターを削除します
        {
            writer.WriteNull();
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
#if CSHARP_8_OR_NEWER
            return default!;
#else
            return default;
#endif
        }

        public static T DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadNextBlock();
#if CSHARP_8_OR_NEWER
            return default!;
#else
            return default;
#endif
        }
    }
}
