// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json
{
    public interface IOverwriteJsonFormatter<T> : IJsonFormatter<T>
    {
        void DeserializeTo(ref T value, ref JsonReader reader, JsonSerializerOptions options);
    }
}