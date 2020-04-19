// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
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
                var answer = options.DeserializeWithVerify<T>(ref reader);
                return answer;
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
        public static T Deserialize<T>(ReadOnlySpan<byte> span, JsonSerializerOptions options)
        {
            var reader = new JsonReader(span);
            try
            {
                var answer = options.DeserializeWithVerify<T>(ref reader);
                return answer;
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
        /// <param name="reader">The reader to deserialize from.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ref JsonReader reader)
        {
            try
            {
                var answer = DefaultOptions.DeserializeWithVerify<T>(ref reader);
                return answer;
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
        public static T Deserialize<T>(ReadOnlySpan<byte> span)
        {
            var reader = new JsonReader(span);
            try
            {
                var answer = DefaultOptions.DeserializeWithVerify<T>(ref reader);
                return answer;
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
        /// <param name="span">The memory to deserialize from.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonSerializationException">Thrown when any error occurs during deserialization.</exception>
        public static T Deserialize<T>(ReadOnlySpan<byte> span, out int bytesRead)
        {
            var reader = new JsonReader(span);
            try
            {
                var answer = DefaultOptions.DeserializeWithVerify<T>(ref reader);
                bytesRead = reader.Consumed;
                return answer;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Failed to deserialize {typeof(T).FullName} value.", ex);
            }
        }
    }
}
