// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Formatters;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed class ReflectionResolver : IFormatterResolver
    {
        public static readonly ReflectionResolver Instance = new ReflectionResolver();

        private ReflectionResolver()
        {
        }

        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();

        public IJsonFormatter<T>
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter<T>()
        {
            var formatter = GetFormatter(typeof(T)) as IJsonFormatter<T>;
            return formatter;
        }

        public IJsonFormatter
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter(Type targetType)
        {
            var formatter = formatterTable.GetOrAdd(targetType, NotFoundCreateFactory);
            return formatter;
        }

#if CSHARP_8_OR_NEWER
        private static IJsonFormatter? NotFoundCreateFactory(Type targetType)
#else
        private static IJsonFormatter NotFoundCreateFactory(Type targetType)
#endif
        {
            if (targetType.IsEnum) return default;
            
            if (targetType.IsValueType)
            {
                var answer = Activator.CreateInstance(typeof(ValueTypeReflectionFormatter<>).MakeGenericType(targetType)) as IJsonFormatter;
                return answer;
            }
            else
            {
                var answer = Activator.CreateInstance(typeof(ReferenceTypeReflectionFormatter<>).MakeGenericType(targetType)) as IJsonFormatter;
                return answer;
            }

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
            return formatterTable.ToArray();
        }
    }
}
