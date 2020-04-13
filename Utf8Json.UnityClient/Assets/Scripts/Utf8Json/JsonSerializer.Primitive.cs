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
        public static byte[] Serialize(Int32 value, JsonSerializerOptions options)
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
                    throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(Int32 value)
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
                    throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int32 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

        public static byte[] Serialize(Int64 value, JsonSerializerOptions options)
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
                    throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(Int64 value)
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
                    throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int64 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

        public static byte[] Serialize(UInt64 value, JsonSerializerOptions options)
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
                    throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(UInt64 value)
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
                    throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt64 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

        public static byte[] Serialize(Char value, JsonSerializerOptions options)
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

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Char value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

        public static byte[] Serialize(Boolean value, JsonSerializerOptions options)
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
                    throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static byte[] Serialize(Boolean value)
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
                    throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Boolean value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

        public static byte[] Serialize(Single value, JsonSerializerOptions options)
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

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Single value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

        public static byte[] Serialize(Double value, JsonSerializerOptions options)
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

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Double value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                var fastWriter = new JsonWriter(sequenceRental.Value);
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
            finally
            {
                sequenceRental.Dispose();
            }
        }
#endif

    }
}
#pragma warning restore IDE0060
