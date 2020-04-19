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
            = Type.GetType("Utf8Json.Resolvers.StandardResolver")
                ?.GetField("Options", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                ?.GetValue(default) as JsonSerializerOptions
                ?? throw new NullReferenceException();

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

        public static void Serialize(ref JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var targetType = value.GetType();
            try
            {
                options.Resolver.GetFormatterWithVerify(targetType).SerializeTypeless(ref writer, value, options);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to serialize {targetType.FullName} value.", ex);
            }
        }

        public static byte[] Serialize(object value, JsonSerializerOptions options)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                var targetType = value.GetType();
                try
                {
                    var formatter = options.Resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException($"Failed to serialize {targetType.FullName} value.", ex);
                }

                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
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
            var calculator = options.Resolver.GetCalcByteLengthForSerialization<T>();
            if (calculator.ToPointer() == null)
            {
                goto INTERNAL;
            }

            var serializer = options.Resolver.GetSerializeSpan<T>();
            if (serializer.ToPointer() == null)
            {
                goto INTERNAL;
            }

            var answer = new byte[options.CalcByteLengthForSerialization(value, calculator)];
            options.SerializeSpan(value, answer.AsSpan(), serializer);
            return answer;

        INTERNAL:
            return SerializeInternal(value, options);
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <returns>A byte array with the serialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
        public static unsafe byte[] Serialize<T>(T value)
        {
            var options = DefaultOptions;

            var calculator = options.Resolver.GetCalcByteLengthForSerialization<T>();
            if (calculator.ToPointer() == null)
            {
                goto INTERNAL;
            }

            var serializer = options.Resolver.GetSerializeSpan<T>();
            if (serializer.ToPointer() == null)
            {
                goto INTERNAL;
            }

            var answer = new byte[options.CalcByteLengthForSerialization(value, calculator)];
            options.SerializeSpan(value, answer.AsSpan(), serializer);
            return answer;

        INTERNAL:
            return SerializeInternal(value, options);
        }

        private static unsafe byte[] SerializeInternal<T>(T value, JsonSerializerOptions options)
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    var serializer = options.Resolver.GetSerializeStatic<T>();
                    if (serializer.ToPointer() == null)
                    {
                        var formatter = options.Resolver.GetFormatterWithVerify<T>();
                        formatter.Serialize(ref writer, value, options);
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
                return writer.FlushAndGetArray();
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
    }
}
