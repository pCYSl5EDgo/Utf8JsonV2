// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using Utf8Json.Internal;

#if SPAN_BUILTIN
using System.IO;
#endif

// ReSharper disable BuiltInTypeReferenceStyle
#pragma warning disable IDE0060

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        public static byte[] Serialize(Char value)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    writer.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(Single value)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    writer.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(Double value)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    writer.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

    }
}
