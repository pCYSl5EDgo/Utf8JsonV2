// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using System.Buffers;
using Utf8Json.Internal;
#if SPAN_BUILTIN
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endif

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
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static unsafe void Serialize<T>(IBufferWriter<byte> writer, T value, JsonSerializerOptions options)
        {
            var fastWriter = new JsonWriter(writer);
            try
            {
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    options.Resolver.GetFormatterWithVerify<T>().Serialize(ref fastWriter, value, options);
                }
                else
                {
                    fastWriter.Serialize(value, options, serializer);
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
            }
            fastWriter.Flush();
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static unsafe void Serialize<T>(IBufferWriter<byte> writer, T value)
        {
            var fastWriter = new JsonWriter(writer);
            try
            {
                var serializer = DefaultOptions.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    DefaultOptions.Resolver.GetFormatterWithVerify<T>().Serialize(ref fastWriter, value, DefaultOptions);
                }
                else
                {
                    fastWriter.Serialize(value, DefaultOptions, serializer);
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
            }
            fastWriter.Flush();
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static unsafe void Serialize<T>(ref JsonWriter writer, T value, JsonSerializerOptions options)
        {
            try
            {
                var serializer = options.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value, options);
                }
                else
                {
                    writer.Serialize(value, options, serializer);
                }
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
        public static unsafe void Serialize<T>(ref JsonWriter writer, T value)
        {
            try
            {
                var serializer = DefaultOptions.Resolver.GetSerializeStatic<T>();
                if (serializer.ToPointer() == null)
                {
                    DefaultOptions.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value, DefaultOptions);
                }
                else
                {
                    writer.Serialize(value, DefaultOptions, serializer);
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
            }
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <returns>A byte array with the serialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static unsafe byte[] Serialize<T>(T value, JsonSerializerOptions options)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var jsonWriter = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    var serializer = options.Resolver.GetSerializeStatic<T>();
                    if (serializer.ToPointer() == null)
                    {
                        options.Resolver.GetFormatterWithVerify<T>().Serialize(ref jsonWriter, value, options);
                    }
                    else
                    {
                        jsonWriter.Serialize(value, options, serializer);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
                }

                return jsonWriter.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <returns>A byte array with the serialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static unsafe byte[] Serialize<T>(T value)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var jsonWriter = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    var serializer = DefaultOptions.Resolver.GetSerializeStatic<T>();
                    if (serializer.ToPointer() == null)
                    {
                        DefaultOptions.Resolver.GetFormatterWithVerify<T>().Serialize(ref jsonWriter, value, DefaultOptions);
                    }
                    else
                    {
                        jsonWriter.Serialize(value, DefaultOptions, serializer);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException($"Failed to serialize {typeof(T).FullName} value.", ex);
                }
                return jsonWriter.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
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
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
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
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static void Serialize<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
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
                Serialize(sequenceRental.Value, value, options);

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
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
                Serialize(sequenceRental.Value, value);

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
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
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ref JsonReader reader, JsonSerializerOptions options)
        {
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                return deserializer.ToPointer() == null
                    ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                    : reader.Deserialize<T>(options, deserializer);
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
        /// <param name="span">The buffer to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ReadOnlySpan<byte> span, JsonSerializerOptions options)
        {
            var reader = new JsonReader(span);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                return deserializer.ToPointer() == null
                    ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                    : reader.Deserialize<T>(options, deserializer);
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
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ReadOnlyMemory<byte> buffer, JsonSerializerOptions options)
        {
            var reader = new JsonReader(buffer.Span);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                return deserializer.ToPointer() == null
                    ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                    : reader.Deserialize<T>(options, deserializer);
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
        /// <param name="buffer">The memory to deserialize from.</param>
        /// <param name="options">The options. Use <c>null</c> to use default options.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ReadOnlyMemory<byte> buffer, JsonSerializerOptions options, out int bytesRead)
        {
            var reader = new JsonReader(buffer.Span);
            try
            {
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                var result = deserializer.ToPointer() == null
                    ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                    : reader.Deserialize<T>(options, deserializer);
                bytesRead = reader.Consumed;
                return result;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to deserialize {typeof(T).FullName} value.", ex);
            }
        }

        #endregion

        #region Deserialize without JsonSerializerOptions
        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ref JsonReader reader)
        {
            try
            {
                var deserializer = DefaultOptions.Resolver.GetDeserializeStatic<T>();
                return deserializer.ToPointer() == null
                    ? DefaultOptions.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, DefaultOptions)
                    : reader.Deserialize<T>(DefaultOptions, deserializer);
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
        /// <param name="span">The buffer to deserialize from.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ReadOnlySpan<byte> span)
        {
            var reader = new JsonReader(span);
            try
            {
                var options = DefaultOptions;
                var deserializer = options.Resolver.GetDeserializeStatic<T>();
                return deserializer.ToPointer() == null
                    ? options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options)
                    : reader.Deserialize<T>(options, deserializer);
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
        /// <param name="buffer">The memory to deserialize from.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static unsafe T Deserialize<T>(ReadOnlyMemory<byte> buffer, out int bytesRead)
        {
            var reader = new JsonReader(buffer.Span);
            try
            {
                var deserializer = DefaultOptions.Resolver.GetDeserializeStatic<T>();
                var result = deserializer.ToPointer() == null
                    ? DefaultOptions.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, DefaultOptions)
                    : reader.Deserialize<T>(DefaultOptions, deserializer);
                bytesRead = reader.Consumed;
                return result;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to deserialize {typeof(T).FullName} value.", ex);
            }
        }
        #endregion
    }
}
