// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;
using Utf8Json.Internal.Reflection;

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

        private sealed class CachingResolver : IFormatterResolver
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
                return formatter as IJsonFormatter<T>;
            }

            public unsafe IntPtr GetSerializeStatic<T>()
            {
                var answer = functionsCache[typeof(T)].SerializeFunctionPtr;
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

                    var other0 = resolver.GetDeserializeStatic<T>();
                    var other1 = resolver.GetCalcByteLengthForSerialization<T>();
                    var other2 = resolver.GetSerializeSpan<T>();
                    functionsCache.Add(typeof(T), new ThreadSafeTypeKeyFormatterHashTable.FunctionPair(answer, other0, other1, other2));
                    return answer;
                }

                return new IntPtr(null);
            }

            public unsafe IntPtr GetDeserializeStatic<T>()
            {
                var answer = functionsCache[typeof(T)].DeserializeFunctionPtr;
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

                    var other0 = resolver.GetSerializeStatic<T>();
                    var other1 = resolver.GetCalcByteLengthForSerialization<T>();
                    var other2 = resolver.GetSerializeSpan<T>();
                    functionsCache.Add(typeof(T), new ThreadSafeTypeKeyFormatterHashTable.FunctionPair(other0, answer, other1, other2));
                    return answer;
                }

                return new IntPtr(null);
            }

            public unsafe IntPtr GetCalcByteLengthForSerialization<T>()
            {
                var answer = functionsCache[typeof(T)].CalcByteLengthFunctionPtr;
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

                    var other0 = resolver.GetSerializeStatic<T>();
                    var other1 = resolver.GetDeserializeStatic<T>();
                    var other2 = resolver.GetSerializeSpan<T>();
                    functionsCache.Add(typeof(T), new ThreadSafeTypeKeyFormatterHashTable.FunctionPair(other0, other1, answer, other2));
                    return answer;
                }

                return new IntPtr(null);
            }

            public unsafe IntPtr GetSerializeSpan<T>()
            {
                var answer = functionsCache[typeof(T)].SerializeSpanFunctionPtr;
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

                    var other0 = resolver.GetSerializeStatic<T>();
                    var other1 = resolver.GetDeserializeStatic<T>();
                    var other2 = resolver.GetCalcByteLengthForSerialization<T>();
                    functionsCache.Add(typeof(T), new ThreadSafeTypeKeyFormatterHashTable.FunctionPair(other0, other1, other2, answer));
                    return answer;
                }

                return new IntPtr(null);
            }

            public IJsonFormatter[] CollectCurrentRegisteredFormatters()
            {
                var totalLength = this.subFormatters.Length;
                var registeredFormatterArray = formattersCache.ToArray();
                totalLength += registeredFormatterArray.Length;
                var subResolversFormatters = new IJsonFormatter[subResolvers.Length][];
                for (var index = 0; index < subResolvers.Length; index++)
                {
                    var resolver = subResolvers[index];
                    var formatters = resolver.CollectCurrentRegisteredFormatters();
                    subResolversFormatters[index] = formatters;
                    totalLength += formatters.Length;
                }

                if (totalLength == 0)
                {
                    return Array.Empty<IJsonFormatter>();
                }

                var answer = new IJsonFormatter[totalLength];
                Array.Copy(registeredFormatterArray, answer, registeredFormatterArray.Length);
                var count = registeredFormatterArray.Length;
                Array.Copy(this.subFormatters, 0, answer, count, this.subFormatters.Length);
                count += this.subFormatters.Length;
                foreach (var formatterArray in subResolversFormatters)
                {
                    Array.Copy(formatterArray, 0, answer, count, formatterArray.Length);
                    count += formatterArray.Length;
                }

                return answer;
            }

            public
#if CSHARP_8_OR_NEWER
                IJsonFormatter?
#else
                IJsonFormatter
#endif
                GetFormatter(Type targetType)
            {
                if (formattersCache.TryGetValue(targetType, out var formatter))
                {
                    goto RETURN;
                }

                var interfaceFormatter = typeof(IJsonFormatter<>).MakeGeneric(targetType);

                foreach (var subFormatter in subFormatters)
                {
                    if (!interfaceFormatter.IsInstanceOfType(subFormatter))
                    {
                        continue;
                    }

                    formatter = subFormatter;
                    goto CACHE;
                }

                foreach (var resolver in subResolvers)
                {
                    formatter = resolver.GetFormatter(targetType);
                    if (formatter != null)
                    {
                        goto CACHE;
                    }
                }

            // when not found, cache null.
            CACHE:
                formattersCache.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(targetType, formatter));

            RETURN:
                return formatter;
            }
        }
    }
}
