// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if ENABLE_IL2CPP
using System.Runtime.CompilerServices;
using Utf8Json.Formatters;

namespace Utf8Json.Resolvers
{
    public sealed class PrimitiveObjectResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly PrimitiveObjectResolver Instance;

        /// <summary>
        /// A <see cref="JsonSerializerOptions"/> instance with this formatter pre-configured.
        /// </summary>
        public static readonly JsonSerializerOptions Options;

        static PrimitiveObjectResolver()
        {
            Instance = new PrimitiveObjectResolver();
            Options = new JsonSerializerOptions(Instance);
        }

        private PrimitiveObjectResolver()
        {
        }

        public IntPtr GetSerializeStatic<T>() => default;
        public IntPtr GetDeserializeStatic<T>() => default;
        public IntPtr GetCalcByteLengthForSerialization<T>() => default;
        public IntPtr GetSerializeSpan<T>() => default;

#if CSHARP_8_OR_NEWER
        public IJsonFormatter<T>? GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T>? Formatter;

            static FormatterCache()
            {
                Formatter = typeof(T) == typeof(object)
                    ? Unsafe.As<IJsonFormatter<T>>(PrimitiveObjectFormatter.Instance)
                    : default;
            }
        }
#else
        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> Formatter;

            static FormatterCache()
            {
                Formatter = typeof(T) == typeof(object) 
                    ? Unsafe.As<IJsonFormatter<T>>(PrimitiveObjectFormatter.Instance) 
                    : default;
            }
        }
#endif
    }
}
#endif
