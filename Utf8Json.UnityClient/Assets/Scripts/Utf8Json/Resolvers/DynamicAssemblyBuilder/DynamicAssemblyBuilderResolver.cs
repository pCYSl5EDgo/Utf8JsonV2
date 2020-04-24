// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed partial class DynamicAssemblyBuilderResolver : IFormatterResolver
    {
        public static readonly DynamicAssemblyBuilderResolver Instance;

#if CSHARP_8_OR_NEWER
        private static readonly ConcurrentDictionary<byte[], FieldInfo>? dataFieldDictionary;
        private static readonly AssemblyBuilder? assemblyBuilder;
        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>? formatterTable;
#else
        private static readonly ConcurrentDictionary<byte[], FieldInfo> dataFieldDictionary;
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
                dataFieldDictionary = default;
                return;
            }

            formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();
            const string assemblyName = "Utf8Json.IL.Emit";
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            dataFieldDictionary = new ConcurrentDictionary<byte[], FieldInfo>(new DataByteArrayComparer());
        }

        private sealed class DataByteArrayComparer : IComparer<byte[]>, IEqualityComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                var c = x.LongLength.CompareTo(y.LongLength);
                if (c != 0)
                {
                    return c;
                }

                for (var index = 0; index < x.Length; index++)
                {
                    c = x[index].CompareTo(y[index]);
                    if (c != 0)
                    {
                        return c;
                    }
                }

                return 0;
            }

            public bool Equals(byte[] x, byte[] y)
            {
                if (x.LongLength != y.LongLength)
                {
                    return false;
                }

                for (var i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(byte[] obj) => obj.Length - 1;
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

            if (!targetType.IsEnum) return default;

            var builderSet = PrepareBuilderSet(targetType);

            if (targetType.IsEnum)
            {
                var flags = targetType.GetCustomAttribute<FlagsAttribute>();
                if (flags is null)
                {
                    EnumNumberFactory(targetType, in builderSet);
                }
                else
                {
                    EnumNumberFactory(targetType, in builderSet);
                }
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

        private const MethodAttributes StaticMethodFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        private const MethodAttributes InstanceMethodFlags = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

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

            var writerParams = new[]
            {
                typeof(JsonWriter).MakeByRefType(),
                targetType,
                typeof(JsonSerializerOptions),
            };
            var serializeStatic = typeBuilder.DefineMethod("SerializeStatic", StaticMethodFlags, typeof(void), writerParams);
            {
                var serialize = typeBuilder.DefineMethod("Serialize", InstanceMethodFlags, typeof(void), writerParams);
                GenerateIntermediateLanguageCodesForSerialize(serializeStatic, serialize);
            }
            var writerTypelessParams = new[]
            {
                typeof(JsonWriter).MakeByRefType(),
                typeof(object),
                typeof(JsonSerializerOptions),
            };
            var serializeTypeless = typeBuilder.DefineMethod("SerializeTypeless", InstanceMethodFlags, typeof(void), writerTypelessParams);
            var readerParams = new[]
            {
                typeof(JsonReader).MakeByRefType(),
                typeof(JsonSerializerOptions),
            };
            var deserializeStatic = typeBuilder.DefineMethod("DeserializeStatic", StaticMethodFlags, targetType, readerParams);
            {
                var deserialize = typeBuilder.DefineMethod("Deserialize", InstanceMethodFlags, targetType, readerParams);
                GenerateIntermediateLanguageCodesForDeserialize(deserializeStatic, deserialize);
            }
            var deserializeTypeless = typeBuilder.DefineMethod("DeserializeTypeless", InstanceMethodFlags, typeof(object), readerParams);

            var builderSet = new BuilderSet(typeBuilder, serializeStatic, serializeTypeless, deserializeStatic, deserializeTypeless);
            return builderSet;
        }

        private static void GenerateIntermediateLanguageCodesForDeserialize(MethodInfo deserializeStatic, MethodBuilder deserialize)
        {
            var processor = deserialize.GetILGenerator();
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.EmitCall(OpCodes.Call, deserializeStatic, null);
            processor.Emit(OpCodes.Ret);
        }

        private static void GenerateIntermediateLanguageCodesForSerialize(MethodInfo serializeStatic, MethodBuilder serialize)
        {
            var processor = serialize.GetILGenerator();
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldarg_2);
            processor.Emit(OpCodes.Ldarg_3);
            processor.EmitCall(OpCodes.Call, serializeStatic, null);
            processor.Emit(OpCodes.Ret);
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
            public readonly MethodBuilder SerializeStatic;
            public readonly MethodBuilder SerializeTypeless;
            public readonly MethodBuilder DeserializeStatic;
            public readonly MethodBuilder DeserializeTypeless;

            public BuilderSet(TypeBuilder type, MethodBuilder serializeStatic, MethodBuilder serializeTypeless, MethodBuilder deserializeStatic, MethodBuilder deserializeTypeless)
            {
                Type = type;
                SerializeStatic = serializeStatic;
                SerializeTypeless = serializeTypeless;
                DeserializeStatic = deserializeStatic;
                DeserializeTypeless = deserializeTypeless;
            }
        }
    }
}
