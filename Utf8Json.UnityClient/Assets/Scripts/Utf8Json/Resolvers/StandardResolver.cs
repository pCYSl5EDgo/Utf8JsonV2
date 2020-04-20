// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

        public IJsonFormatter[] CollectCurrentRegisteredFormatters()
        {
            var formatterArrayArray = new IJsonFormatter[resolvers.Length][];
            var totalLength = 0;
            for (var index = 0; index < resolvers.Length; index++)
            {
                var resolver = resolvers[index];
                ref var formatterArray = ref formatterArrayArray[index];
                formatterArrayArray[index] = resolver.CollectCurrentRegisteredFormatters();
                totalLength += formatterArray.Length;
            }

            var answer = new IJsonFormatter[totalLength];
            var count = 0;
            foreach (var formatters in formatterArrayArray)
            {
                Array.Copy(formatters, 0, answer, count, formatters.Length);
                count += formatters.Length;
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public IJsonFormatter? GetFormatter(Type targetType)
#else
        public IJsonFormatter GetFormatter(Type targetType)
#endif
        {
            foreach (var resolver in resolvers)
            {
                var formatter = resolver.GetFormatter(targetType);
                if (formatter != null)
                {
                    return formatter;
                }
            }

            return default;
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
                    // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                    if (Formatter is null)
                    {
                        Formatter = formatterResolver.GetFormatter<T>();
                    }
                }
            }
        }
    }
}
