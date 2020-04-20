// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed partial class BuiltinResolver : IFormatterResolver
    {
#if CSHARP_8_OR_NEWER
        public IJsonFormatter<T>? GetFormatter<T>()
#else
        public IJsonFormatter<T> GetFormatter<T>()
#endif
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
            return formattersCache.ToArray();
        }

        public IJsonFormatter
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter(Type targetType)
        {
            formattersCache.TryGetValue(targetType, out var answer);
            return answer;
        }

        private readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formattersCache = BuiltinResolverGetFormatterHelper.GetFormatterCache();

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
                (SerializeFunctionPointer, DeserializeFunctionPointer, CalcByteLengthForSerializationFunctionPointer, SerializeSpanFunctionPointer)
                    = BuiltinResolverGetFormatterHelper.GetFunctionPointers(typeof(T));
                BuiltinResolverGetFormatterHelper.GetFormatterCache().TryGetValue(typeof(T), out var formatter);
                Formatter = formatter as IJsonFormatter<T>;
            }
        }
    }
}
