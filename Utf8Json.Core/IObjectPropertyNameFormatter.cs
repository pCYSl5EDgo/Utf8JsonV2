// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json
{
    public interface IObjectPropertyNameFormatter<T>
    {
        void SerializeToPropertyName(ref JsonWriter writer, T value, JsonSerializerOptions options);
        T DeserializeFromPropertyName(ref JsonReader reader, JsonSerializerOptions options);
    }
}
