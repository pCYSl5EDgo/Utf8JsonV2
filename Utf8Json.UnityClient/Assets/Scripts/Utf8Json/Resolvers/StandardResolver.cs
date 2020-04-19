// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;
using Utf8Json.Internal.Resolvers;

namespace Utf8Json.Resolvers
{
    public sealed class StandardResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly StandardResolver Instance;

        /// <summary>
        /// A <see cref="JsonSerializerOptions"/> instance with this formatter pre-configured.
        /// </summary>
        public static readonly JsonSerializerOptions Options;

        private static readonly IFormatterResolver[] resolvers;

        private StandardResolver() { }

        static StandardResolver()
        {
            Instance = new StandardResolver();
            Options = new JsonSerializerOptions(Instance);
            resolvers = StandardResolverHelper.DefaultResolvers;
        }

        public IJsonFormatter<T>
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        public IntPtr GetSerializeStatic<T>()
        {
            return FormatterCache<T>.SerializeFunctionPointer;
        }

        public IntPtr GetDeserializeStatic<T>()
        {
            return FormatterCache<T>.DeserializeFunctionPointer;
        }

        public IntPtr GetCalcByteLengthForSerialization<T>()
        {
            return FormatterCache<T>.CalcByteLengthForSerializationFunctionPointer;
        }

        public IntPtr GetSerializeSpan<T>()
        {
            return FormatterCache<T>.SerializeSpanFunctionPointer;
        }

#if CSHARP_8_OR_NEWER
        public IJsonFormatter?
#else
        public IJsonFormatter
#endif
            GetFormatter(Type targetType)
        {
            if (StandardResolverHelper.FormatterTable.TryGetValue(targetType, out var formatter))
            {
                return formatter;
            }

            formatter = typeof(FormatterCache<>).MakeGenericType(targetType).GetField("Formatter")?.GetValue(null) as IJsonFormatter;
            StandardResolverHelper.FormatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(targetType, formatter));
            return formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T>
#if CSHARP_8_OR_NEWER
                ?
#endif
                Formatter;

            public static readonly IntPtr SerializeFunctionPointer;
            public static readonly IntPtr DeserializeFunctionPointer;
            public static readonly IntPtr CalcByteLengthForSerializationFunctionPointer;
            public static readonly IntPtr SerializeSpanFunctionPointer;

            static FormatterCache()
            {
                foreach (var formatterResolver in resolvers)
                {
                    if (SerializeFunctionPointer == IntPtr.Zero)
                    {
                        SerializeFunctionPointer = formatterResolver.GetSerializeStatic<T>();
                    }

                    if (DeserializeFunctionPointer == IntPtr.Zero)
                    {
                        DeserializeFunctionPointer = formatterResolver.GetDeserializeStatic<T>();
                    }

                    if (CalcByteLengthForSerializationFunctionPointer == IntPtr.Zero)
                    {
                        CalcByteLengthForSerializationFunctionPointer = formatterResolver.GetCalcByteLengthForSerialization<T>();
                    }

                    if (SerializeSpanFunctionPointer == IntPtr.Zero)
                    {
                        SerializeSpanFunctionPointer = formatterResolver.GetSerializeSpan<T>();
                    }

                    // ReSharper disable once InvertIf
                    if (Formatter is null)
                    {
                        Formatter = formatterResolver.GetFormatter<T>();
                        StandardResolverHelper.FormatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(T), Formatter));
                    }
                }
            }
        }
    }
}
