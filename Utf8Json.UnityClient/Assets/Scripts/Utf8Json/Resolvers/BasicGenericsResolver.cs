// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed partial class BasicGenericsResolver : IFormatterResolver
    {
        public static readonly BasicGenericsResolver Instance = new BasicGenericsResolver();

        private BasicGenericsResolver()
        {
        }

        public IJsonFormatter<T>
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();

        public IJsonFormatter
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter(Type targetType)
        {
            return formatterTable.GetOrAdd(targetType, Factory);
        }

#if CSHARP_8_OR_NEWER
        private static IJsonFormatter? Factory(Type targetType)
#else
        private static IJsonFormatter Factory(Type targetType)
#endif
        {
            return typeof(FormatterCache<>).MakeGenericType(targetType).GetField("Formatter")?.GetValue(null) as IJsonFormatter;
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
            return formatterTable.ToArray();
        }

        internal struct FormatterCache<T>
        {
            public static readonly IntPtr SerializeFunctionPointer;
            public static readonly IntPtr DeserializeFunctionPointer;
            public static readonly IntPtr CalcByteLengthForSerializationFunctionPointer;
            public static readonly IntPtr SerializeSpanFunctionPointer;
            public static readonly IJsonFormatter<T>
#if CSHARP_8_OR_NEWER
                ?
#endif            
                Formatter;

            static FormatterCache()
            {
                var type = typeof(T);
                if (type == typeof(object))
                {
                    return;
                }

                (SerializeFunctionPointer, DeserializeFunctionPointer, CalcByteLengthForSerializationFunctionPointer, SerializeSpanFunctionPointer)
                    = BasicGenericsResolverGetFormatterHelper.GetFunctionPointers(type);
                Formatter = BasicGenericsResolverGetFormatterHelper.CreateFormatter(type) as IJsonFormatter<T>;
                formatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(type, Formatter));
            }
        }
    }
}
