// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes without BOM.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="reader">The reader to deserialize from.</param>
        /// <param name="options">The options.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ref JsonReader reader, JsonSerializerOptions options)
        {
            try
            {
                var answer = options.DeserializeWithVerify<T>(ref reader);
                return answer;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to deserialize " + typeof(T).FullName + " value.", ex);
            }
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes with/without BOM.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="span">The buffer to deserialize from.</param>
        /// <param name="options">The options.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
#if CSHARP_8_OR_NEWER
        public static T Deserialize<T>(ReadOnlySpan<byte> span, JsonSerializerOptions? options = default)
#else
        public static T Deserialize<T>(ReadOnlySpan<byte> span, JsonSerializerOptions options = default)
#endif
        {
            if (span.Length >= 3)
            {
                if (span[0] == JsonReader.Bom0 && span[1] == JsonReader.Bom1 && span[2] == JsonReader.Bom2)
                {
                    span = span.Slice(3);
                }
            }

            var reader = new JsonReader(span);
            try
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                if (options == null)
                {
                    options = DefaultOptions;
                }

                var answer = options.DeserializeWithVerify<T>(ref reader);
                return answer;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to deserialize " + typeof(T).FullName + " value.", ex);
            }
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes(with/without BOM).
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="span">The memory to deserialize from.</param>
        /// <param name="options">The options.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ReadOnlySpan<byte> span, JsonSerializerOptions options, out int bytesRead)
        {
            bytesRead = 0;
            if (span.Length >= 3)
            {
                if (span[0] == JsonReader.Bom0 && span[1] == JsonReader.Bom1 && span[2] == JsonReader.Bom2)
                {
                    span = span.Slice(3);
                    bytesRead = 3;
                }
            }

            var reader = new JsonReader(span);
            try
            {
                var answer = options.DeserializeWithVerify<T>(ref reader);
                bytesRead += reader.Consumed;
                return answer;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to deserialize " + typeof(T).FullName + " value.", ex);
            }
        }

        public static
#if CSHARP_8_OR_NEWER
            object?
#else
            object
#endif
            DeserializeTypeless(Type targetType, ReadOnlySpan<byte> span, JsonSerializerOptions options)
        {
            if (span.Length >= 3)
            {
                if (span[0] == JsonReader.Bom0 && span[1] == JsonReader.Bom1 && span[2] == JsonReader.Bom2)
                {
                    span = span.Slice(3);
                }
            }

            var reader = new JsonReader(span);
            try
            {
                var formatter = options.Resolver.GetFormatterWithVerify(targetType);
                var answer = formatter.DeserializeTypeless(ref reader, options);
                return answer;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to deserialize " + targetType.FullName + " value.", ex);
            }
        }

        public static async
#if CSHARP_8_OR_NEWER
            Task<object?>
#else
            Task<object>
#endif
            DeserializeTypelessAsync(Type targetType, Stream stream, JsonSerializerOptions options, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(1024);
            try
            {
                var length = 0;
                int read;
                while ((read = await stream.ReadAsync(array, length, array.Length - length, token).ConfigureAwait(false)) > 0)
                {
                    length += read;
                    if (length != array.Length)
                    {
                        continue;
                    }

                    var tmp = pool.Rent(length << 1);
                    unsafe
                    {
                        fixed (void* src = &array[0])
                        fixed (void* dst = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dst, tmp.LongLength, array.LongLength);
                        }
                    }
                    pool.Return(tmp);
                    array = tmp;
                }

                var answer = DeserializeTypeless(targetType, array.AsSpan(0, length), options);
                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream, JsonSerializerOptions options)
        {
            var pool = ArrayPool<byte>.Shared;
            var array = pool.Rent(1024);
            try
            {
                var length = 0;
                int read;
                while ((read = await stream.ReadAsync(array, length, array.Length - length).ConfigureAwait(false)) > 0)
                {
                    length += read;
                    if (length != array.Length)
                    {
                        continue;
                    }

                    var tmp = pool.Rent(length << 1);
                    unsafe
                    {
                        fixed (void* src = &array[0])
                        fixed (void* dst = &tmp[0])
                        {
                            Buffer.MemoryCopy(src, dst, tmp.LongLength, array.LongLength);
                        }
                    }
                    pool.Return(tmp);
                    array = tmp;
                }

                return Deserialize<T>(array.AsSpan(0, length), options);
            }
            finally
            {
                pool.Return(array);
            }
        }
    }
}
