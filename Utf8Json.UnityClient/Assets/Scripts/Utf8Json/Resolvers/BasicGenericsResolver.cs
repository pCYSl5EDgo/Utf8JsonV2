// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Resolvers
{
    public sealed partial class BasicGenericsResolver : IFormatterResolver
    {
#if CSHARP_8_OR_NEWER
        public IJsonFormatter<T>? GetFormatter<T>()
#else
        public IJsonFormatter<T> GetFormatter<T>()
#endif
        {
#if UNITY_2018_4_OR_NEWER
            return FormatterCache<T>.Formatter;
#else
            return default;
#endif
        }

        public IJsonFormatter GetFormatter(Type targetType)
        {
            throw new NotImplementedException();
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

        public IntPtr GetSerializeStaticTypeless(Type targetType)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetDeserializeStaticTypeless(Type targetType)
        {
            throw new NotImplementedException();
        }

        internal struct FormatterCache<T>
        {
            public static readonly IntPtr SerializeFunctionPointer;
            public static readonly IntPtr DeserializeFunctionPointer;
            public static readonly IntPtr CalcByteLengthForSerializationFunctionPointer;
            public static readonly IntPtr SerializeSpanFunctionPointer;

#if UNITY_2018_4_OR_NEWER
#if CSHARP_8_OR_NEWER
            public static readonly IJsonFormatter<T>? Formatter;
#else
            public static readonly IJsonFormatter<T> Formatter;
#endif
#endif

            static FormatterCache()
            {
                var type = typeof(T);
                if (type == typeof(object))
                {
                    return;
                }

                (SerializeFunctionPointer, DeserializeFunctionPointer, CalcByteLengthForSerializationFunctionPointer, SerializeSpanFunctionPointer)
                    = BasicGenericsResolverGetFormatterHelper.GetFunctionPointers(type);
#if UNITY_2018_4_OR_NEWER
                Formatter = BasicGenericsResolverGetFormatterHelper.CreateFormatter(type) as IJsonFormatter<T>;
#endif
            }
        }
    }
}
