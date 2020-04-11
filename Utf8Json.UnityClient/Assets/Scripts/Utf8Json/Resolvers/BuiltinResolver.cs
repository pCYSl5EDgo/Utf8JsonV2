// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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

        private static class FormatterCache<T>
        {
#if CSHARP_8_OR_NEWER
            public static readonly IJsonFormatter<T>? Formatter;
#else
            public static readonly IJsonFormatter<T> Formatter;
#endif

            public static readonly IntPtr SerializeFunctionPointer;
            public static readonly IntPtr DeserializeFunctionPointer;

            static FormatterCache()
            {
                // Reduce IL2CPP code generate size(don't write long code in <T>)
                var type = typeof(T);
                Formatter = BuiltinResolverGetFormatterHelper.GetFormatter(type) as IJsonFormatter<T>;

                (SerializeFunctionPointer, DeserializeFunctionPointer) = BuiltinResolverGetFormatterHelper.GetFunctionPointers(type);
            }
        }
    }
}
