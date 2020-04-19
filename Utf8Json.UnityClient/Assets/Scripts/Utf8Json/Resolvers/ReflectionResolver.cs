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

        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();

        public IJsonFormatter<T>
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter<T>()
        {
            var targetType = typeof(T);
            if (formatterTable.TryGetValue(targetType, out var answer))
            {
                return answer as IJsonFormatter<T>;
            }

            if (targetType.IsValueType)
            {
                answer = Activator.CreateInstance(typeof(ValueTypeReflectionFormatter<>).MakeGenericType(targetType)) as IJsonFormatter;
                formatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(targetType, answer));
            }
            else
            {
                answer = Activator.CreateInstance(typeof(ReferenceTypeReflectionFormatter<>).MakeGenericType(targetType)) as IJsonFormatter;
                formatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(targetType, answer));
            }

            return answer as IJsonFormatter<T>;
        }

        public IJsonFormatter
#if CSHARP_8_OR_NEWER
            ?
#endif
            GetFormatter(Type targetType)
        {
            if (formatterTable.TryGetValue(targetType, out var answer))
            {
                return answer;
            }

            if (targetType.IsValueType)
            {
                answer = Activator.CreateInstance(typeof(ValueTypeReflectionFormatter<>).MakeGenericType(targetType)) as IJsonFormatter;
                formatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(targetType, answer));
            }
            else
            {
                answer = Activator.CreateInstance(typeof(ReferenceTypeReflectionFormatter<>).MakeGenericType(targetType)) as IJsonFormatter;
                formatterTable.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(targetType, answer));
            }

            return answer;
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
    }
}
