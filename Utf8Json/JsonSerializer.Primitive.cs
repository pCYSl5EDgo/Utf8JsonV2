// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json.Internal;

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        public static void Serialize(ref JsonWriter writer, Byte value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Byte value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Byte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Byte value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Byte value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Byte value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Byte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Byte value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Byte value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Byte value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Byte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Byte value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Byte value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Byte value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Byte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Byte value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Byte value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Byte value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, SByte value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, SByte value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, SByte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, SByte value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, SByte value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, SByte value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(SByte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(SByte value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(SByte value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(SByte value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, SByte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, SByte value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, SByte value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, SByte value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, SByte value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, SByte value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, SByte value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, SByte value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Int16 value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Int16 value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Int16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int16 value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int16 value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int16 value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Int16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int16 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int16 value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int16 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int16 value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int16 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int16 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Int16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int16 value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int16 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int16 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Int32 value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Int32 value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Int32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int32 value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int32 value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int32 value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Int32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int32 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int32 value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int32 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int32 value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int32 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int32 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Int32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int32 value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int32 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int32 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Int64 value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Int64 value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Int64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int64 value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int64 value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Int64 value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Int64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int64 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int64 value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int64 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int64 value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int64 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Int64 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Int64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int64 value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int64 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Int64 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, UInt16 value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, UInt16 value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt16 value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt16 value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt16 value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(UInt16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt16 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt16 value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt16 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt16 value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt16 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt16 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, UInt16 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt16 value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt16 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt16 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, UInt32 value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, UInt32 value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt32 value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt32 value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt32 value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(UInt32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt32 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt32 value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt32 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt32 value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt32 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt32 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, UInt32 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt32 value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt32 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt32 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, UInt64 value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, UInt64 value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt64 value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt64 value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, UInt64 value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(UInt64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt64 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt64 value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt64 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt64 value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt64 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, UInt64 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, UInt64 value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt64 value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt64 value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, UInt64 value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Char value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Char value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Char value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Char value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Char value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Char value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Char value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Char value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Char value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Char value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Char value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Char value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Char value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Char value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Char value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Char value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Char value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Char value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, String value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, String value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, String value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, String value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, String value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, String value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(String value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(String value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(String value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(String value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, String value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.String value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, String value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.String value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, String value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.String value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, String value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.String value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, String value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, String value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, String value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, String value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Boolean value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Boolean value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Boolean value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Boolean value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Boolean value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Boolean value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Boolean value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Boolean value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Boolean value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Boolean value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Boolean value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Boolean value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Boolean value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Boolean value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Boolean value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Boolean value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Boolean value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Boolean value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Single value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Single value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Single value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Single value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Single value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Single value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Single value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Single value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Single value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Single value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Single value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Single value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Single value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Single value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Single value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Single value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Single value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Single value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
        public static void Serialize(ref JsonWriter writer, Double value, JsonSerializerOptions options)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
        }

        public static void Serialize(ref JsonWriter writer, Double value)
        {
            try
            {
                writer.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
        }

        public static void Serialize(IBufferWriter<byte> writer, Double value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Double value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Double value, CancellationToken cancellationToken)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            fastWriter.Flush();
        }

        public static void Serialize(IBufferWriter<byte> writer, Double value)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                fastWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            fastWriter.Flush();
        }

        public static byte[] Serialize(Double value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Double value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Double value, CancellationToken cancellationToken)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = cancellationToken,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Double value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Double value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Double value, JsonSerializerOptions options)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Double value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = cancellationToken,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static void Serialize(Stream stream, Double value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
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
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }

        public static async Task SerializeAsync(Stream stream, Double value, JsonSerializerOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Double value, JsonSerializerOptions options)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Double value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
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

        public static async Task SerializeAsync(Stream stream, Double value)
        {
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in sequenceRental.Value.AsReadOnlySequence)
                    {
                        await stream.WriteAsync(segment).ConfigureAwait(false);
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
