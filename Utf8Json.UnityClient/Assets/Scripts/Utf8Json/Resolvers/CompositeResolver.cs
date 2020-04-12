// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    /// <summary>
    /// Represents a collection of formatters and resolvers acting as one.
    /// </summary>
    /// <remarks>
    /// This class is not thread-safe for mutations. It is thread-safe when not being written to.
    /// </remarks>
    public static class CompositeResolver
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="IFormatterResolver"/> with the specified formatters and sub-resolvers.
        /// </summary>
        /// <param name="formatters">
        /// A list of instances of <see cref="IJsonFormatter{T}"/> to prefer (above the <paramref name="resolvers"/>).
        /// The formatters are searched in the order given, so if two formatters support serializing the same type, the first one is used.
        /// </param>
        /// <param name="entries">
        /// A list of instances of <see cref="ThreadSafeTypeKeyFormatterHashTable.Entry"/>.
        /// </param>
        /// <param name="resolvers">
        /// A list of resolvers to use for serializing types for which <paramref name="formatters"/> does not include a formatter.
        /// The resolvers are searched in the order given, so if two resolvers support serializing the same type, the first one is used.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IFormatterResolver"/>.
        /// </returns>
        public static IFormatterResolver Create(ReadOnlySpan<IJsonFormatter> formatters, ReadOnlySpan<ThreadSafeTypeKeyFormatterHashTable.Entry> entries, ReadOnlySpan<IFormatterResolver> resolvers)
        {
            // Make a copy of the resolvers list provided by the caller to guard against them changing it later.
            IJsonFormatter[] immutableFormatters;
            if (formatters.Length != 0)
            {
                immutableFormatters = new IJsonFormatter[formatters.Length];
                formatters.CopyTo(immutableFormatters);
            }
            else
            {
                immutableFormatters = Array.Empty<IJsonFormatter>();
            }

            IFormatterResolver[] immutableResolvers;
            if (resolvers.Length != 0)
            {
                immutableResolvers = new IFormatterResolver[resolvers.Length];
                resolvers.CopyTo(immutableResolvers);
            }
            else
            {
                immutableResolvers = Array.Empty<IFormatterResolver>();
            }

            return new CachingResolver(immutableFormatters, entries, immutableResolvers);
        }

        public static IFormatterResolver Create(params IFormatterResolver[] resolvers) => Create(ReadOnlySpan<IJsonFormatter>.Empty, ReadOnlySpan<ThreadSafeTypeKeyFormatterHashTable.Entry>.Empty, resolvers);

        public static IFormatterResolver Create(params IJsonFormatter[] formatters) => Create(formatters, ReadOnlySpan<ThreadSafeTypeKeyFormatterHashTable.Entry>.Empty, ReadOnlySpan<IFormatterResolver>.Empty);

        private class CachingResolver : IFormatterResolver
        {
            private readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formattersCache = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();
            private readonly ThreadSafeTypeKeyFormatterHashTable functionsCache;
            private readonly IJsonFormatter[] subFormatters;
            private readonly IFormatterResolver[] subResolvers;

            /// <summary>
            /// Initializes a new instance of the <see cref="CachingResolver"/> class.
            /// </summary>
            internal CachingResolver(IJsonFormatter[] subFormatters, ReadOnlySpan<ThreadSafeTypeKeyFormatterHashTable.Entry> entries, IFormatterResolver[] subResolvers)
            {
                this.subFormatters = subFormatters;
                this.subResolvers = subResolvers;
                this.functionsCache = new ThreadSafeTypeKeyFormatterHashTable(entries);
            }

#if CSHARP_8_OR_NEWER
            public IJsonFormatter<T>? GetFormatter<T>()
#else
            public IJsonFormatter<T> GetFormatter<T>()
#endif
            {
                if (formattersCache.TryGetValue(typeof(T), out var formatter))
                {
                    goto RETURN;
                }

                foreach (var subFormatter in subFormatters)
                {
                    if (!(subFormatter is IJsonFormatter<T>))
                    {
                        continue;
                    }

                    formatter = subFormatter;
                    goto CACHE;
                }

                foreach (var resolver in subResolvers)
                {
                    formatter = resolver.GetFormatter<T>();
                    if (formatter != null)
                    {
                        goto CACHE;
                    }
                }

            // when not found, cache null.
            CACHE:
                formattersCache.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(T), formatter));

            RETURN:
                return Unsafe.As<IJsonFormatter<T>>(formatter);
            }

            public unsafe IntPtr GetSerializeStatic<T>()
            {
                var (answer, _) = functionsCache[typeof(T)];
                if (answer.ToPointer() != null)
                {
                    return answer;
                }

                foreach (var resolver in subResolvers)
                {
                    answer = resolver.GetSerializeStatic<T>();
                    if (answer.ToPointer() == null)
                    {
                        continue;
                    }

                    var other = resolver.GetDeserializeStatic<T>();
                    functionsCache.Add(typeof(T), answer, other);
                    return answer;
                }

                return new IntPtr(null);
            }

            public unsafe IntPtr GetDeserializeStatic<T>()
            {
                var (_, answer) = functionsCache[typeof(T)];
                if (answer.ToPointer() != null)
                {
                    return answer;
                }

                foreach (var resolver in subResolvers)
                {
                    answer = resolver.GetDeserializeStatic<T>();
                    if (answer.ToPointer() == null)
                    {
                        continue;
                    }

                    var other = resolver.GetSerializeStatic<T>();
                    functionsCache.Add(typeof(T), other, answer);
                    return answer;
                }

                return new IntPtr(null);
            }
        }
    }
}
