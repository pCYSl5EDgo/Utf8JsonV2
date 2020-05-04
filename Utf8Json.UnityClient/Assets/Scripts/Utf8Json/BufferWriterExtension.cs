// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Utf8Json.Internal;

// ReSharper disable RedundantCaseLabel

namespace Utf8Json
{
    public static class BufferWriterExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteEmptyObject(ref this BufferWriter writer)
        {
            const int sizeHint = 2;
            var span = writer.GetSpan(sizeHint);
            span[0] = (byte)'{';
            span[1] = (byte)'}';
            writer.Advance(sizeHint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteEmptyArray(ref this BufferWriter writer)
        {
            const int sizeHint = 2;
            var span = writer.GetSpan(sizeHint);
            span[0] = (byte)'[';
            span[1] = (byte)']';
            writer.Advance(sizeHint);
        }
    }
}
