// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed class AttributeFormatterResolver : IFormatterResolver
    {
        public static readonly AttributeFormatterResolver Instance = new AttributeFormatterResolver();

        private AttributeFormatterResolver()
        {
        }

        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();
        private struct FormatterCache<T>
        {
#if CSHARP_8_OR_NEWER
            public static readonly IJsonFormatter<T>? Formatter;
#else
            public static readonly IJsonFormatter<T> Formatter;
#endif

            static FormatterCache()
            {
                Formatter = formatterTable.GetOrAdd(typeof(T), Factory) as IJsonFormatter<T>;
            }
        }

#if CSHARP_8_OR_NEWER
        private static IJsonFormatter? Factory(Type targetType)
#else
        private static IJsonFormatter Factory(Type targetType)
#endif
        {
            var jsonFormatterAttribute = targetType.GetCustomAttribute<JsonFormatterAttribute>();
            return JsonFormatterAttributeHelper.FromJsonFormatterAttribute(jsonFormatterAttribute);
        }

#if CSHARP_8_OR_NEWER
        public IJsonFormatter<T>? GetFormatter<T>()
#else
        public IJsonFormatter<T> GetFormatter<T>()
#endif
        {
            return FormatterCache<T>.Formatter;
        }

#if CSHARP_8_OR_NEWER
        public IJsonFormatter? GetFormatter(Type targetType)
#else
        public IJsonFormatter GetFormatter(Type targetType)
#endif
        {
            return formatterTable.GetOrAdd(targetType, Factory);
        }

        public IntPtr GetSerializeStatic<T>()
        {
            return default;
        }

        public IntPtr GetDeserializeStatic<T>()
        {
            return default;
        }

        public IntPtr GetCalcByteLengthForSerialization<T>()
        {
            return default;
        }

        public IntPtr GetSerializeSpan<T>()
        {
            return default;
        }

        public IJsonFormatter[] CollectCurrentRegisteredFormatters()
        {
            throw new NotImplementedException();
        }
    }
}
