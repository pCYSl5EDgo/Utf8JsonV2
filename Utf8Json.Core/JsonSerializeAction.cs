// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json
{
    public delegate void JsonSerializeAction<T>(ref JsonWriter writer, T value, JsonSerializerOptions options);
}