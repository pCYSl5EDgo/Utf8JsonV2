// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable InvalidXmlDocComment

// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        private static readonly System.Func<System.Type, CompiledMethods> createCompiledMethods;
        private static readonly Internal.ThreadSafeTypeKeyReferenceHashTable<CompiledMethods> serializes = new Internal.ThreadSafeTypeKeyReferenceHashTable<CompiledMethods>(64);

        static JsonSerializer()
        {
            createCompiledMethods = t => new CompiledMethods(t);
            var standardResolverType = System.Type.GetType("Utf8Json.Resolvers.StandardResolver");
            Debug.Assert(standardResolverType != null, nameof(standardResolverType) + " != null");
            var optionsFieldInfo = standardResolverType.GetField("Options", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Debug.Assert(optionsFieldInfo != null, nameof(optionsFieldInfo) + " != null");
            var defaultOptionsObject = optionsFieldInfo.GetValue(default);
            Debug.Assert(defaultOptionsObject != null, nameof(defaultOptionsObject) + " != null");
            DefaultOptions = (JsonSerializerOptions)defaultOptionsObject;
        }

#if CSHARP_8_OR_NEWER
        private static CompiledMethods GetOrAdd(System.Type type) => serializes.GetOrAdd(type, createCompiledMethods)!;
#else
        private static CompiledMethods GetOrAdd(System.Type type) => serializes.GetOrAdd(type, createCompiledMethods);
#endif
        #region Serialize
        /// <seealso cref="Serialize{T}(ref JsonWriter, T, JsonSerializerOptions)"/>
        public static void Serialize(System.Type type, ref JsonWriter writer, object obj, JsonSerializerOptions options)
        {
            GetOrAdd(type).Serialize_JsonWriter_T_Options.Invoke(ref writer, obj, options);
        }

        /// <seealso cref="Serialize{T}(IBufferWriter{byte}, T, JsonSerializerOptions, System.Threading.CancellationToken)"/>
        public static void Serialize(System.Type type, System.Buffers.IBufferWriter<byte> writer, object obj, JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            GetOrAdd(type).Serialize_IBufferWriter_T_Options_CancellationToken.Invoke(writer, obj, options, cancellationToken);
        }

        /// <seealso cref="Serialize{T}(T, JsonSerializerOptions, System.Threading.CancellationToken)"/>
        public static byte[] Serialize(System.Type type, object obj, JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            return GetOrAdd(type).Serialize_T_Options.Invoke(obj, options, cancellationToken);
        }

#if SPAN_BUILTIN
        /// <seealso cref="Serialize{T}(Stream, T, JsonSerializerOptions, System.Threading.CancellationToken)"/>
        public static void Serialize(System.Type type, System.IO.Stream stream, object obj, JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            GetOrAdd(type).Serialize_Stream_T_Options_CancellationToken.Invoke(stream, obj, options, cancellationToken);
        }

        /// <seealso cref="SerializeAsync{T}(Stream, T, JsonSerializerOptions, System.Threading.CancellationToken)"/>
        public static System.Threading.Tasks.Task SerializeAsync(System.Type type, System.IO.Stream stream, object obj, JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            return GetOrAdd(type).SerializeAsync_Stream_T_Options_CancellationToken.Invoke(stream, obj, options, cancellationToken);
        }
#endif
        #endregion

        #region Deserialize
        /// <seealso cref="Deserialize{T}(ref JsonReader, JsonSerializerOptions)"/>
        public static object Deserialize(System.Type type, ref JsonReader reader, JsonSerializerOptions options)
        {
            return GetOrAdd(type).Deserialize_JsonReader_Options.Invoke(ref reader, options);
        }

        /// <seealso cref="Deserialize{T}(ReadOnlyMemory{byte}, JsonSerializerOptions, System.Threading.CancellationToken)"/>
        public static object Deserialize(System.Type type, System.ReadOnlyMemory<byte> bytes, JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            return GetOrAdd(type).Deserialize_ReadOnlyMemory_Options.Invoke(bytes, options, cancellationToken);
        }

#if SPAN_BUILTIN
        /// <seealso cref="Deserialize{T}(Stream, JsonSerializerOptions, System.Threading.CancellationToken)"/>
        public static object Deserialize(System.Type type, System.IO.Stream stream, JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            return GetOrAdd(type).Deserialize_Stream_Options_CancellationToken.Invoke(stream, options, cancellationToken);
        }
#endif
        #endregion
    }
}
