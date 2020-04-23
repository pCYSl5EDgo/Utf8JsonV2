// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed partial class DynamicAssemblyBuilderResolver : IFormatterResolver
    {
        public static readonly DynamicAssemblyBuilderResolver Instance;

#if CSHARP_8_OR_NEWER
        private static readonly AssemblyBuilder? assemblyBuilder;
        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>? formatterTable;
#else
        private static readonly AssemblyBuilder assemblyBuilder;
        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatterTable;
#endif
        private static readonly ModuleBuilder moduleBuilder;

        static DynamicAssemblyBuilderResolver()
        {
            Instance = new DynamicAssemblyBuilderResolver();
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                assemblyBuilder = default;
                formatterTable = default;
#if CSHARP_8_OR_NEWER
                moduleBuilder = default!;
#else
                moduleBuilder = default;
#endif
                return;
            }

            formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();
            const string assemblyName = "Utf8Json.IL.Emit";
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        public IJsonFormatter[] CollectCurrentRegisteredFormatters()
        {
            return formatterTable?.ToArray() ?? Array.Empty<IJsonFormatter>();
        }

        public IntPtr GetCalcByteLengthForSerialization<T>()
        {
            return default;
        }

        public IntPtr GetDeserializeStatic<T>()
        {
            return FormatterCache<T>.DeserializeStatic;
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
            return formatterTable?.GetOrAdd(targetType, Factory);
        }

#if CSHARP_8_OR_NEWER
        private static IJsonFormatter? Factory(Type targetType)
#else
        private static IJsonFormatter Factory(Type targetType)
#endif
        {
            if (assemblyBuilder is null)
            {
                return default;
            }

            if (targetType.IsInterface || targetType.IsAbstract || targetType.IsArray || targetType.IsByRef || targetType.IsGenericParameter)
            {
                return default;
            }

            var builderSet = PrepareBuilderSet(targetType);

            if (targetType.IsEnum)
            {
                var enumUnderlyingType = targetType.GetEnumUnderlyingType();
                EnumFactory(targetType, enumUnderlyingType, in builderSet);
            }
            else
            {
                TypeAnalyzer.Analyze(targetType, out var analyzeResult);
                if (targetType.IsValueType)
                {
                    ValueTypeFactory(targetType, in analyzeResult, in builderSet);
                }
                else
                {
                    ReferenceTypeFactory(targetType, in analyzeResult, in builderSet);
                }
            }

            var answer = Closing(builderSet.Type);
            return answer;
        }

        private static BuilderSet PrepareBuilderSet(Type targetType)
        {
            var typeBuilder = moduleBuilder.DefineType(
                "Utf8Json.IL.Emit.Formatters." + targetType.FullName + "<>Formatter",
                TypeAttributes.AutoLayout | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                typeof(object),
                new[]
                {
                    typeof(IJsonFormatter),
                    typeof(IJsonFormatter<>).MakeGenericType(targetType),
                });

            var defaultConstructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public | MethodAttributes.HideBySig);
            const MethodAttributes staticMethodFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
            const MethodAttributes instanceMethodFlags = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            var writerParams = new[]
            {
                typeof(JsonWriter).MakeByRefType(),
                targetType,
                typeof(JsonSerializerOptions),
            };
            var serializeStatic = typeBuilder.DefineMethod("SerializeStatic", staticMethodFlags, typeof(void), writerParams);
            var serialize = typeBuilder.DefineMethod("Serialize", instanceMethodFlags, typeof(void), writerParams);
            var writerTypelessParams = new[]
            {
                typeof(JsonWriter).MakeByRefType(),
                typeof(object),
                typeof(JsonSerializerOptions),
            };
            var serializeTypeless = typeBuilder.DefineMethod("SerializeTypeless", instanceMethodFlags, typeof(void), writerTypelessParams);
            var readerParams = new[]
            {
                typeof(JsonReader).MakeByRefType(),
                typeof(JsonSerializerOptions),
            };
            var deserializeStatic = typeBuilder.DefineMethod("DeserializeStatic", staticMethodFlags, targetType, readerParams);
            var deserialize = typeBuilder.DefineMethod("Deserialize", instanceMethodFlags, targetType, readerParams);
            var deserializeTypeless = typeBuilder.DefineMethod("DeserializeTypeless", instanceMethodFlags, typeof(object), readerParams);

            var builderSet = new BuilderSet(typeBuilder, defaultConstructor, serialize, serializeStatic, serializeTypeless, deserialize, deserializeStatic, deserializeTypeless);
            return builderSet;
        }

#if CSHARP_8_OR_NEWER
        private static IJsonFormatter? Closing(TypeBuilder typeBuilder)
#else
        private static IJsonFormatter Closing(TypeBuilder typeBuilder)
#endif
        {
#if TYPEBUILDER_CREATE_TYPE
            var closedType = typeBuilder.CreateType();
#else
            var closedTypeInfo = typeBuilder.CreateTypeInfo();
            var closedType = closedTypeInfo?.AsType();
#endif
            if (closedType is null)
            {
                return default;
            }

            var answerFormatter = Activator.CreateInstance(closedType) as IJsonFormatter;
            return answerFormatter;
        }

        public IntPtr GetSerializeSpan<T>()
        {
            return default;
        }

        public IntPtr GetSerializeStatic<T>()
        {
            return FormatterCache<T>.SerializeStatic;
        }

        private struct FormatterCache<T>
        {
#if CSHARP_8_OR_NEWER
            public static readonly IJsonFormatter<T>? Formatter;
#else
            public static readonly IJsonFormatter<T> Formatter;
#endif
            public static readonly IntPtr SerializeStatic;
            public static readonly IntPtr DeserializeStatic;

            static FormatterCache()
            {
                Formatter = formatterTable?.GetOrAdd(typeof(T), Factory) as IJsonFormatter<T>;
                if (Formatter == null)
                {
                    SerializeStatic = IntPtr.Zero;
                    DeserializeStatic = IntPtr.Zero;
                }
                else
                {
                    var formatterType = Formatter.GetType();
                    SerializeStatic = StaticHelper.GetSerializeStatic(formatterType);
                    DeserializeStatic = StaticHelper.GetDeserializeStatic(formatterType);
                }
            }
        }

        private readonly struct BuilderSet
        {
            public readonly TypeBuilder Type;
            public readonly ConstructorBuilder DefaultConstructor;
            public readonly MethodBuilder Serialize;
            public readonly MethodBuilder SerializeStatic;
            public readonly MethodBuilder SerializeTypeless;
            public readonly MethodBuilder Deserialize;
            public readonly MethodBuilder DeserializeStatic;
            public readonly MethodBuilder DeserializeTypeless;

            public BuilderSet(TypeBuilder type, ConstructorBuilder defaultConstructor, MethodBuilder serialize, MethodBuilder serializeStatic, MethodBuilder serializeTypeless, MethodBuilder deserialize, MethodBuilder deserializeStatic, MethodBuilder deserializeTypeless)
            {
                Type = type;
                DefaultConstructor = defaultConstructor;
                Serialize = serialize;
                SerializeStatic = serializeStatic;
                SerializeTypeless = serializeTypeless;
                Deserialize = deserialize;
                DeserializeStatic = deserializeStatic;
                DeserializeTypeless = deserializeTypeless;
            }
        }
    }
}
