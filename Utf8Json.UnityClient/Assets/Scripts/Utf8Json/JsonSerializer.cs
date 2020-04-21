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
        public static void Serialize<TWriter, T>(TWriter writer, T value, JsonSerializerOptions options)
            where TWriter : IBufferWriter<byte>
        {
            var fastWriter = new JsonWriter(writer);
            try
            {
                options.SerializeWithVerify(ref fastWriter, value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize " + typeof(T).FullName + " value.", ex);
            }
            fastWriter.Flush();
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeTypeless<TWriter>(Type targetType, TWriter writer, object? value, JsonSerializerOptions options)
#else
        public static void SerializeTypeless<TWriter>(Type targetType, TWriter writer, object value, JsonSerializerOptions options)
#endif
            where TWriter : IBufferWriter<byte>
        {
            var fastWriter = new JsonWriter(writer);
            try
            {
                var formatter = options.Resolver.GetFormatterWithVerify(targetType);
                formatter.SerializeTypeless(ref fastWriter, value, options);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize " + targetType.FullName + " value.", ex);
            }
            fastWriter.Flush();
        }

#if CSHARP_8_OR_NEWER
        public static async Task SerializeTypelessAsync(Type targetType, Stream stream, object? value, JsonSerializerOptions options, CancellationToken cancellationToken)
#else
        public static async Task SerializeTypelessAsync(Type targetType, Stream stream, object value, JsonSerializerOptions options, CancellationToken cancellationToken)
#endif
        {
            cancellationToken.ThrowIfCancellationRequested();
#if SPAN_BUILTIN
            var sequenceRental = SequencePool.Shared.Rent();
            try
            {
                SerializeTypeless(targetType, sequenceRental.Value, value, options);

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
#else
            var array = SerializeTypeless(targetType, value, options);
            try
            {
                await stream.WriteAsync(array, 0, array.Length, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
            }
#endif
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
                options.SerializeWithVerify(ref writer, value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize " + typeof(T).FullName + " value.", ex);
            }
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The options.</param>
        /// <returns>A byte array with the serialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during serialization.</exception>
#if CSHARP_8_OR_NEWER
        public static byte[] Serialize<T>(T value, JsonSerializerOptions? options = default)
#else
        public static byte[] Serialize<T>(T value, JsonSerializerOptions options = default)
#endif
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (options == null)
            {
                options = DefaultOptions;
            }
#if !ENABLE_IL2CPP
            var calculator = options.Resolver.GetCalcByteLengthForSerialization<T>();
            unsafe
            {
                if (calculator.ToPointer() == null)
                {
                    goto INTERNAL;
                }

                var serializer = options.Resolver.GetSerializeSpan<T>();
                if (serializer.ToPointer() == null)
                {
                    goto INTERNAL;
                }

                var answer = new byte[StaticFunctionPointerHelper.CallHelper.CalcByteLengthForSerialization(options, value, calculator)];
                StaticFunctionPointerHelper.CallHelper.SerializeSpan(options, value, answer.AsSpan(), serializer);
                return answer;
            }

        INTERNAL:
#endif
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    options.SerializeWithVerify(ref writer, value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize " + typeof(T).FullName + " value.", ex);
                }
                return writer.FlushAndGetArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

#if CSHARP_8_OR_NEWER
        public static byte[] SerializeTypeless(Type targetType, object? value, JsonSerializerOptions options)
#else
        public static byte[] SerializeTypeless(Type targetType, object value, JsonSerializerOptions options)
#endif
        {
            var array = ArrayPool<byte>.Shared.Rent(80 * 1024);
            try
            {
                var writer = new JsonWriter(SequencePool.Shared, array);
                try
                {
                    var formatter = options.Resolver.GetFormatterWithVerify(targetType);
                    formatter.SerializeTypeless(ref writer, value, options);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize " + targetType.FullName + " value.", ex);
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
