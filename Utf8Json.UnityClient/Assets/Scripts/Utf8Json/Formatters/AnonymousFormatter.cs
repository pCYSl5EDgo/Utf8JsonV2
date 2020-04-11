// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;

namespace Utf8Json.Formatters
{
    public sealed class AnonymousFormatter<T> : IJsonFormatter<T>
    {
        private readonly JsonSerializeAction<T> serialize;
        private readonly JsonDeserializeFunc<T> deserialize;

        public AnonymousFormatter(JsonSerializeAction<T> serialize, JsonDeserializeFunc<T> deserialize)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }

        public void Serialize(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (serialize == null) throw new InvalidOperationException(this.GetType().Name + " does not support Serialize.");
            serialize(ref writer, value, options);
        }

        public T Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (deserialize == null) throw new InvalidOperationException(this.GetType().Name + " does not support Deserialize.");
            return deserialize(ref reader, options);
        }
    }
}
#endif
