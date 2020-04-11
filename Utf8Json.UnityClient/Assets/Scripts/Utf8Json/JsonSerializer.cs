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
        /// <summary>
        /// Gets or sets the default set of options to use when not explicitly specified for a method call.
        /// </summary>
        /// <value>The default value is Utf8Json.Resolvers.Standard.</value>
        /// <remarks>
        /// This is an AppDomain or process-wide setting.
        /// If you're writing a library, you should NOT set or rely on this property but should instead pass
        /// in StandardResolver (or the required options) explicitly to every method call
        /// to guarantee appropriate behavior in any application.
        /// If you are an app author, realize that setting this property impacts the entire application so it should only be
        /// set once, and before any use of <see cref="JsonSerializer"/> occurs.
        /// </remarks>
        public static JsonSerializerOptions DefaultOptions { get; set; }

        #region Serialize
        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(IBufferWriter<byte> writer, T value, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            Serialize(ref fastWriter, value, options);
            fastWriter.Flush();
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(IBufferWriter<byte> writer, T value, CancellationToken cancellationToken = default)
        {
            var fastWriter = new JsonWriter(writer)
            {
                CancellationToken = cancellationToken,
            };
            Serialize(ref fastWriter, value);
            fastWriter.Flush();
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            try
            {
                options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value, options);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
            }
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(ref JsonWriter writer, T value)
        {
            try
            {
                DefaultOptions.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value, DefaultOptions);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
            }
        }

        /// <summary>
        /// A thread-local, recyclable array that may be used for short bursts of code.
        /// </summary>
#if CSHARP_8_OR_NEWER
        [ThreadStatic] private static byte[]? ScratchArray;
#else
        [ThreadStatic] private static byte[] ScratchArray;
#endif

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A byte array with the serialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static byte[] Serialize<T>(T value, JsonSerializerOptions options, CancellationToken cancellationToken = default)
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
            Serialize(ref jsonWriter, value, options);
            return jsonWriter.FlushAndGetArray();
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A byte array with the serialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static byte[] Serialize<T>(T value, CancellationToken cancellationToken = default)
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
            Serialize(ref jsonWriter, value);
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        /// <summary>
        /// Serializes a given value to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(Stream stream, T value, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                Serialize(sequenceRental.Value, value, options, cancellationToken);

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

        /// <summary>
        /// Serializes a given value to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                Serialize(sequenceRental.Value, value, cancellationToken);

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

        /// <summary>
        /// Serializes a given value to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that completes with the result of the async serialization operation.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static async Task SerializeAsync<T>(Stream stream, T value, JsonSerializerOptions options, CancellationToken cancellationToken = default)
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

        /// <summary>
        /// Serializes a given value to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that completes with the result of the async serialization operation.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
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
#endif
        #endregion

        #region Deserialize
        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="byteSequence">The sequence to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(in ReadOnlySequence<byte> byteSequence, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var reader = new JsonReader(byteSequence)
            {
                CancellationToken = cancellationToken,
            };
            return Deserialize<T>(ref reader, options);
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ref JsonReader reader, JsonSerializerOptions options)
        {
            try
            {
                return options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to deserialize {typeof(T).FullName} value.", ex);
            }
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ReadOnlyMemory<byte> buffer, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var reader = new JsonReader(buffer)
            {
                CancellationToken = cancellationToken,
            };
            return Deserialize<T>(ref reader, options);
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The memory to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ReadOnlyMemory<byte> buffer, JsonSerializerOptions options, out int bytesRead, CancellationToken cancellationToken = default)
        {
            var reader = new JsonReader(buffer)
            {
                CancellationToken = cancellationToken,
            };
            var result = Deserialize<T>(ref reader, options);
            bytesRead = buffer.Slice(0, (int)reader.Consumed).Length;
            return result;
        }

#if SPAN_BUILTIN
        /// <summary>
        /// Deserializes the entire content of a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="stream">
        /// The stream to deserialize from.
        /// The entire stream will be read, and the first json token deserialized will be returned.
        /// If <see cref="Stream.CanSeek"/> is true on the stream, its position will be set to just after the last deserialized byte.
        /// </param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        /// <remarks>
        /// If multiple top-level json data structures are expected on the stream, use <see cref="JsonStreamReader"/> instead.
        /// </remarks>
        public static T Deserialize<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (TryDeserializeFromMemoryStream(stream, options, cancellationToken, out T result))
            {
                return result;
            }

            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var sequence = sequenceRental.Value;
                try
                {
                    int bytesRead;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var span = sequence.GetSpan(stream.CanSeek ? (int)(stream.Length - stream.Position) : 0);
                        bytesRead = stream.Read(span);
                        sequence.Advance(bytesRead);
                    }
                    while (bytesRead > 0);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while reading from the stream.", ex);
                }

                return DeserializeFromSequenceAndRewindStreamIfPossible<T>(stream, options, sequence, cancellationToken);
            }
        }

        /// <summary>
        /// Deserializes the entire content of a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="stream">
        /// The stream to deserialize from.
        /// The entire stream will be read, and the first json token deserialized will be returned.
        /// If <see cref="Stream.CanSeek"/> is true on the stream, its position will be set to just after the last deserialized byte.
        /// </param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        /// <remarks>
        /// If multiple top-level json data structures are expected on the stream, use <see cref="JsonStreamReader"/> instead.
        /// </remarks>
        public static async ValueTask<T> DeserializeAsync<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (TryDeserializeFromMemoryStream(stream, options, cancellationToken, out T result))
            {
                return result;
            }

            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var sequence = sequenceRental.Value;
                try
                {
                    int bytesRead;
                    do
                    {
                        var memory = sequence.GetMemory(stream.CanSeek ? (int)(stream.Length - stream.Position) : 0);
                        bytesRead = await stream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);
                        sequence.Advance(bytesRead);
                    }
                    while (bytesRead > 0);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while reading from the stream.", ex);
                }

                return DeserializeFromSequenceAndRewindStreamIfPossible<T>(stream, options, sequence, cancellationToken);
            }
        }
#endif
        #endregion

        #region Deserialize without JsonSerializerOptions
        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="byteSequence">The sequence to deserialize from.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(in ReadOnlySequence<byte> byteSequence, CancellationToken cancellationToken = default)
        {
            var reader = new JsonReader(byteSequence)
            {
                CancellationToken = cancellationToken,
            };
            return Deserialize<T>(ref reader);
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ref JsonReader reader)
        {
            try
            {
                return DefaultOptions.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, DefaultOptions);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to deserialize {typeof(T).FullName} value.", ex);
            }
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var reader = new JsonReader(buffer)
            {
                CancellationToken = cancellationToken,
            };
            return Deserialize<T>(ref reader);
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The memory to deserialize from.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ReadOnlyMemory<byte> buffer, out int bytesRead, CancellationToken cancellationToken = default)
        {
            var reader = new JsonReader(buffer)
            {
                CancellationToken = cancellationToken,
            };
            var result = Deserialize<T>(ref reader);
            bytesRead = buffer.Slice(0, (int)reader.Consumed).Length;
            return result;
        }

#if SPAN_BUILTIN
        /// <summary>
        /// Deserializes the entire content of a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="stream">
        /// The stream to deserialize from.
        /// The entire stream will be read, and the first json token deserialized will be returned.
        /// If <see cref="Stream.CanSeek"/> is true on the stream, its position will be set to just after the last deserialized byte.
        /// </param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        /// <remarks>
        /// If multiple top-level json data structures are expected on the stream, use <see cref="JsonStreamReader"/> instead.
        /// </remarks>
        public static T Deserialize<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (TryDeserializeFromMemoryStream(stream, cancellationToken, out T result))
            {
                return result;
            }

            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var sequence = sequenceRental.Value;
                try
                {
                    int bytesRead;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var span = sequence.GetSpan(stream.CanSeek ? (int)(stream.Length - stream.Position) : 0);
                        bytesRead = stream.Read(span);
                        sequence.Advance(bytesRead);
                    }
                    while (bytesRead > 0);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while reading from the stream.", ex);
                }

                return DeserializeFromSequenceAndRewindStreamIfPossible<T>(stream, sequence, cancellationToken);
            }
        }

        /// <summary>
        /// Deserializes the entire content of a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="stream">
        /// The stream to deserialize from.
        /// The entire stream will be read, and the first json token deserialized will be returned.
        /// If <see cref="Stream.CanSeek"/> is true on the stream, its position will be set to just after the last deserialized byte.
        /// </param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        /// <remarks>
        /// If multiple top-level json data structures are expected on the stream, use <see cref="JsonStreamReader"/> instead.
        /// </remarks>
        public static async ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (TryDeserializeFromMemoryStream(stream, cancellationToken, out T result))
            {
                return result;
            }

            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var sequence = sequenceRental.Value;
                try
                {
                    int bytesRead;
                    do
                    {
                        var memory = sequence.GetMemory(stream.CanSeek ? (int)(stream.Length - stream.Position) : 0);
                        bytesRead = await stream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);
                        sequence.Advance(bytesRead);
                    }
                    while (bytesRead > 0);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while reading from the stream.", ex);
                }

                return DeserializeFromSequenceAndRewindStreamIfPossible<T>(stream, sequence, cancellationToken);
            }
        }

        private static bool TryDeserializeFromMemoryStream<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken, out T result)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!(stream is MemoryStream ms) || !ms.TryGetBuffer(out var streamBuffer))
            {
#if CSHARP_8_OR_NEWER
                result = default!;
#else
                result = default;
#endif
                return false;
            }

            result = Deserialize<T>(streamBuffer.AsMemory(checked((int)ms.Position)), options, out var bytesRead, cancellationToken);

            // Emulate that we had actually "read" from the stream.
            ms.Seek(bytesRead, SeekOrigin.Current);
            return true;
        }

        private static bool TryDeserializeFromMemoryStream<T>(Stream stream, CancellationToken cancellationToken, out T result)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!(stream is MemoryStream ms) || !ms.TryGetBuffer(out var streamBuffer))
            {
#if CSHARP_8_OR_NEWER
                result = default!;
#else
                result = default;
#endif
                return false;
            }

            result = Deserialize<T>(streamBuffer.AsMemory(checked((int)ms.Position)), out var bytesRead, cancellationToken);

            // Emulate that we had actually "read" from the stream.
            ms.Seek(bytesRead, SeekOrigin.Current);
            return true;
        }

        private static T DeserializeFromSequenceAndRewindStreamIfPossible<T>(Stream streamToRewind, JsonSerializerOptions options, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken)
        {
            if (streamToRewind is null)
            {
                throw new ArgumentNullException(nameof(streamToRewind));
            }

            var reader = new JsonReader(sequence)
            {
                CancellationToken = cancellationToken,
            };
            var result = Deserialize<T>(ref reader, options);

            if (streamToRewind.CanSeek && !reader.End)
            {
                // Reverse the stream as many bytes as we left unread.
                var bytesNotRead = checked((int)reader.Sequence.Slice(reader.Position).Length);
                streamToRewind.Seek(-bytesNotRead, SeekOrigin.Current);
            }

            return result;
        }

        private static T DeserializeFromSequenceAndRewindStreamIfPossible<T>(Stream streamToRewind, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken)
        {
            if (streamToRewind is null)
            {
                throw new ArgumentNullException(nameof(streamToRewind));
            }

            var reader = new JsonReader(sequence)
            {
                CancellationToken = cancellationToken,
            };
            var result = Deserialize<T>(ref reader);

            if (streamToRewind.CanSeek && !reader.End)
            {
                // Reverse the stream as many bytes as we left unread.
                var bytesNotRead = checked((int)reader.Sequence.Slice(reader.Position).Length);
                streamToRewind.Seek(-bytesNotRead, SeekOrigin.Current);
            }

            return result;
        }
#endif
        #endregion
    }
}
