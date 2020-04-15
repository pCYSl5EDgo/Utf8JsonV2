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
#if ENABLE_IL2CPP
#else
            /*var defaultResolversLength = StandardResolverHelper.DefaultResolvers.Length;
            resolvers = new IFormatterResolver[defaultResolversLength + 1];
            Array.Copy(StandardResolverHelper.DefaultResolvers, resolvers, defaultResolversLength);*/
            // Resolvers[defaultResolversLength] = DynamicObjectResolver.Instance; // Try Object
#endif
        }

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

        private static class FormatterCache<T>
        {
#if CSHARP_8_OR_NEWER
            public static readonly IJsonFormatter<T>? Formatter;
#else
            public static readonly IJsonFormatter<T> Formatter;
#endif

            public static readonly IntPtr SerializeFunctionPointer;
            public static readonly IntPtr DeserializeFunctionPointer;
            public static readonly IntPtr CalcByteLengthForSerializationFunctionPointer;
            public static readonly IntPtr SerializeSpanFunctionPointer;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    // final fallback
#if ENABLE_IL2CPP
                    Formatter = PrimitiveObjectResolver.Instance.GetFormatter<T>();
#else
                    throw new NotImplementedException();
                    //Formatter =
                    //DynamicObjectTypeFallbackFormatter.Instance;
#endif
                }

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

                    if (Formatter is null)
                    {
                        Formatter = formatterResolver.GetFormatter<T>();
                    }
                }
            }
        }
    }
}
