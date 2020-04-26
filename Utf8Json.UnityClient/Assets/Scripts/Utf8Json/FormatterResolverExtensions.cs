// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Utf8Json.Internal;
using Utf8Json.Internal.Reflection;

namespace Utf8Json
{
    public static class FormatterResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IJsonFormatter GetFormatterWithVerify(this IFormatterResolver resolver, Type targetType)
        {
#if CSHARP_8_OR_NEWER
            IJsonFormatter? formatter;
#else
            IJsonFormatter formatter;
#endif
            try
            {
                formatter = resolver.GetFormatter(targetType);
            }
            catch (TypeInitializationException ex)
            {
                // The fact that we're using static constructors to initialize this is an internal detail.
                // Rethrow the inner exception if there is one.
                // Do it carefully so as to not stomp on the original callstack.
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                throw new InvalidOperationException();
            }

            if (formatter == null)
            {
                throw new FormatterNotRegisteredException(targetType.FullName + " is not registered in resolver: " + resolver.GetType());
            }

            return formatter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IJsonFormatter<T> GetFormatterWithVerify<T>(this IFormatterResolver resolver)
        {
#if CSHARP_8_OR_NEWER
            IJsonFormatter<T>? formatter;
#else
            IJsonFormatter<T> formatter;
#endif
            try
            {
                formatter = resolver.GetFormatter<T>();
            }
            catch (TypeInitializationException ex)
            {
                // The fact that we're using static constructors to initialize this is an internal detail.
                // Rethrow the inner exception if there is one.
                // Do it carefully so as to not stomp on the original callstack.
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                throw new InvalidOperationException();
            }

            if (formatter == null)
            {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in resolver: " + resolver.GetType());
            }

            return formatter;
        }


#if CSHARP_8_OR_NEWER
        private static readonly ThreadSafeTypeKeyReferenceHashTable<Func<IFormatterResolver, IJsonFormatter?>> formatterGetters = new ThreadSafeTypeKeyReferenceHashTable<Func<IFormatterResolver, IJsonFormatter?>>();
#else
        private static readonly ThreadSafeTypeKeyReferenceHashTable<Func<IFormatterResolver, IJsonFormatter>> formatterGetters = new ThreadSafeTypeKeyReferenceHashTable<Func<IFormatterResolver, IJsonFormatter>>();
#endif

        private static readonly MethodInfo getFormatterRuntimeMethod =
            typeof(IFormatterResolver).GetRuntimeMethod(nameof(IFormatterResolver.GetFormatter), Type.EmptyTypes)
            ?? throw new InvalidOperationException(nameof(IFormatterResolver.GetFormatter));

#if CSHARP_8_OR_NEWER
        public static object? GetFormatterDynamic(this IFormatterResolver resolver, Type type)
#else
        public static object GetFormatterDynamic(this IFormatterResolver resolver, Type type)
#endif
        {
            if (formatterGetters.TryGetValue(type, out var formatterGetter))
            {
#if CSHARP_8_OR_NEWER
                return formatterGetter!(resolver);
#else
                return formatterGetter(resolver);
#endif
            }

            var getFormatterGenericInstanceMethod = getFormatterRuntimeMethod.MakeGeneric(type);
            var inputResolver = Expression.Parameter(typeof(IFormatterResolver), "inputResolver");
#if CSHARP_8_OR_NEWER
            formatterGetter = Expression.Lambda<Func<IFormatterResolver, IJsonFormatter?>>(Expression.Call(inputResolver, getFormatterGenericInstanceMethod), inputResolver).Compile();
            formatterGetters.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<Func<IFormatterResolver, IJsonFormatter?>>.Entry(type, formatterGetter));
#else
            formatterGetter = Expression.Lambda<Func<IFormatterResolver, IJsonFormatter>>(Expression.Call(inputResolver, getFormatterGenericInstanceMethod), inputResolver).Compile();
            formatterGetters.TryAdd(new ThreadSafeTypeKeyReferenceHashTable<Func<IFormatterResolver, IJsonFormatter>>.Entry(type, formatterGetter));
#endif

            return formatterGetter(resolver);
        }

        internal static object GetFormatterDynamicWithVerify(this IFormatterResolver resolver, Type type)
        {
            var result = GetFormatterDynamic(resolver, type);
            if (result == null)
            {
                throw new FormatterNotRegisteredException(type.FullName + " is not registered in resolver: " + resolver.GetType());
            }

            return result;
        }
    }

    public class FormatterNotRegisteredException : Exception
    {
        public FormatterNotRegisteredException(string message)
            : base(message)
        {
        }
    }
}