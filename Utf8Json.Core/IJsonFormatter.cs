
// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace Utf8Json
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IJsonFormatter
    {
#if CSHARP_8_OR_NEWER
        void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options);
        object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options);
#else
        void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options);
        object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options);
#endif
    }

    public interface IJsonFormatter<T> : IJsonFormatter
    {
        void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options);
        T Deserialize(ref JsonReader reader, JsonSerializerOptions options);
    }
}