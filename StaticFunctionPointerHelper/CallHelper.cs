// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Copyright (c) pCYSl5EDgo. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Utf8Json;

namespace StaticFunctionPointerHelper
{
    public static class CallHelper
    {
#pragma warning disable IDE0060
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<T>(this ref JsonWriter writer, T value, JsonSerializerOptions options, IntPtr functionPointer)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(this ref JsonReader reader, JsonSerializerOptions options, IntPtr functionPointer)
        {
            return default;
        }
#pragma warning restore IDE0060
    }
}
