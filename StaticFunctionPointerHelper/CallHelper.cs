﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Copyright (c) pCYSl5EDgo. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Utf8Json;
#pragma warning disable IDE0060

namespace StaticFunctionPointerHelper
{
    public static class CallHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<T>(this ref JsonWriter writer, T value, JsonSerializerOptions options, IntPtr functionPointer)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(this ref JsonReader reader, JsonSerializerOptions options, IntPtr functionPointer)
        {
#if CSHARP_8_OR_NEWER
            return default!;
#else
            return default;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcByteLengthForSerialization<T>(this JsonSerializerOptions options, T value, IntPtr functionPointer)
        {
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeSpan<T>(this JsonSerializerOptions options, T value, Span<byte> span, IntPtr functionPointer)
        {
        }
    }
}
