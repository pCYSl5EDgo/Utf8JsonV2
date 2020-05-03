// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;
using Utf8Json.Internal.Reflection;
using RuntimeFeature = Utf8Json.Internal.RuntimeFeature;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public sealed class DynamicAssemblyBuilderResolver : IFormatterResolver
    {
        public static readonly DynamicAssemblyBuilderResolver Instance;

#if CSHARP_8_OR_NEWER
        private static readonly AssemblyBuilder? assemblyBuilder;
        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>? formatterTable;
#else
        private static readonly AssemblyBuilder assemblyBuilder;
        private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatterTable;
#endif

        private static readonly ConcurrentDictionary<Assembly, object> assemblyDictionary;
        private static readonly ModuleBuilder moduleBuilder;
        private static readonly ConstructorInfo constructorIgnoresAccessChecksToAttribute;

        static DynamicAssemblyBuilderResolver()
        {
            Instance = new DynamicAssemblyBuilderResolver();
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                assemblyBuilder = default;
                formatterTable = default;
#if CSHARP_8_OR_NEWER
                moduleBuilder = default!;
                constructorIgnoresAccessChecksToAttribute = default!;
                assemblyDictionary = default!;
#else
                moduleBuilder = default;
                constructorIgnoresAccessChecksToAttribute = default;
                assemblyDictionary = default;
#endif
                return;
            }

            formatterTable = new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>();
            const string assemblyName = "Utf8Json.IL.Emit";
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            AddUnverifiable();
            constructorIgnoresAccessChecksToAttribute = CreateIgnoresAccessChecksToAttribute();
            assemblyDictionary = new ConcurrentDictionary<Assembly, object>();
            assemblyDictionary.GetOrAdd(assemblyBuilder, FactoryOfIgnoresAccess);
            assemblyDictionary.GetOrAdd(typeof(JsonSerializerOptions).Assembly, FactoryOfIgnoresAccess);
            assemblyDictionary.GetOrAdd(typeof(JsonSerializerOptionsExtensions).Assembly, FactoryOfIgnoresAccess);
        }

        // Unityというか多分Mono環境ではUnverifiableCodeAttributeを追加せず非publicメンバにアクセスすると死ぬ
        private static void AddUnverifiable()
        {
            /*
            えるしっているか、Mono環境ではAssemblyBuilderないしModuleBuilderに対して未定義な属性をGetCustomAttributeで求めると即死する
            var unverifiableCodeAttribute = moduleBuilder.GetCustomAttribute<System.Security.UnverifiableCodeAttribute>();
            if (!(unverifiableCodeAttribute is null))
            {
                return;
            }*/

            var attribute = typeof(System.Security.UnverifiableCodeAttribute);
            var ctor = attribute.GetConstructor(Array.Empty<Type>());
            if (ctor is null)
            {
                throw new NullReferenceException();
            }

            moduleBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, Array.Empty<object>()));
        }

        #region IgnoresAccessChecksTo
        private static void EnsurePrivateAccess(Type targetType)
        {
            assemblyDictionary.GetOrAdd(targetType.Assembly, FactoryOfIgnoresAccess);
        }

        private static object FactoryOfIgnoresAccess(Assembly assembly)
        {
            // IgnoresAccessChecksTo declarations cannot have a version, culture, public key token, or processor architecture specified.
            var name = assembly.GetName().Name ?? throw new NullReferenceException();
            assemblyBuilder?.SetCustomAttribute(new CustomAttributeBuilder(constructorIgnoresAccessChecksToAttribute, new object[]
            {
                name,
            }));

            return ObjectHelper.Object;
        }

        private static ConstructorInfo CreateIgnoresAccessChecksToAttribute()
        {
            Debug.Assert(!(assemblyBuilder is null));
            var attributeBuilder = moduleBuilder.DefineType("System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute", TypeAttributes.BeforeFieldInit, typeof(Attribute));

            AddCustomAttributeToIgnoresAccessChecksToAttribute(attributeBuilder);

            var constructor = DefineMembersInIgnoresAccessChecksToAttribute(attributeBuilder);

#if TYPEBUILDER_CREATE_TYPE
            attributeBuilder.CreateType();
#else
            attributeBuilder.CreateTypeInfo();
#endif
            return constructor;
        }

        private static void AddCustomAttributeToIgnoresAccessChecksToAttribute(TypeBuilder attributeBuilder)
        {
            var types = TypeArrayHolder.Length1;
            types[0] = typeof(AttributeTargets);
            var usageConstructor = typeof(AttributeUsageAttribute).GetConstructor(types);
            Debug.Assert(!(usageConstructor is null));
            var propertyAllowMultiple = typeof(AttributeUsageAttribute).GetProperty("AllowMultiple");
            Debug.Assert(!(propertyAllowMultiple is null));
            attributeBuilder.SetCustomAttribute(new CustomAttributeBuilder(usageConstructor,
            new object[]
            {
                AttributeTargets.Assembly,
            },
            new[]
            {
                propertyAllowMultiple
            },
            new[]
            {
                ObjectHelper.True,
            }));
        }

        private static ConstructorInfo DefineMembersInIgnoresAccessChecksToAttribute(TypeBuilder attributeBuilder)
        {
            var assemblyName = attributeBuilder.DefineField("<AssemblyName>k__BackingField", typeof(string), FieldAttributes.Private | FieldAttributes.InitOnly);
            var getAssemblyName = attributeBuilder.DefineMethod("get_AssemblyName", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, typeof(string), Array.Empty<Type>());
            {
                var processor = getAssemblyName.GetILGenerator();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldfld, assemblyName);
                processor.Emit(OpCodes.Ret);
            }
            var propertyAssemblyName = attributeBuilder.DefineProperty("AssemblyName", PropertyAttributes.None, typeof(string), Array.Empty<Type>());
            propertyAssemblyName.SetGetMethod(getAssemblyName);

            var paramTypes = TypeArrayHolder.Length1;
            paramTypes[0] = typeof(string);
            var constructor = attributeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, paramTypes);
            {
                var parentConstructor = typeof(Attribute).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Array.Empty<Type>(), null);
                Debug.Assert(!(parentConstructor is null));

                var processor = constructor.GetILGenerator();
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, parentConstructor);

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Stfld, assemblyName);

                processor.Emit(OpCodes.Ret);
            }

            return constructor;
        }
        #endregion

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
#if ENABLE_MONO
            return default;
#else
            return FormatterCache<T>.DeserializeStatic;
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

            if (targetType.IsEnum)
            {
                return EnumFactory(targetType);
            }

            TypeAnalyzer.Analyze(targetType, out var analyzeResult);
#if ENABLE_MONO
            // 本当に嫌な話ではあるのですが、Mono環境ではIgnoresAccessChecksToをAssemblyBuilder内に定義してもinternal/privateアクセスを可能にしてくれないのです。
            // なのでDnSpyでデバッグできない悲しみを背負ったDynamicMethodを使わざるを得ないのです。
            var typeIsPublic = DetectTypeIsPublic(targetType) && analyzeResult.AreAllPublic();
            if (typeIsPublic)
            {
#endif
                EnsurePrivateAccess(targetType);
                var builderSet = BuilderSet.PrepareBuilderSet(targetType, moduleBuilder);
                var serializeStatic = DefineSerializeStatic(targetType, builderSet.Type);
                var deserializeStatic = DefineDeserializeStatic(targetType, builderSet.Type);

                if (targetType.IsValueType)
                {
                    ValueTypeEmbedTypelessHelper.SerializeTypeless(targetType, serializeStatic, builderSet.SerializeTypeless);
                    ValueTypeEmbedTypelessHelper.DeserializeTypeless(targetType, deserializeStatic, builderSet.DeserializeTypeless);
                }
                else
                {
                    ReferenceTypeEmbedTypelessHelper.SerializeTypeless(targetType, serializeStatic, builderSet.SerializeTypeless);
                    ReferenceTypeEmbedTypelessHelper.DeserializeTypeless(targetType, deserializeStatic, builderSet.DeserializeTypeless);
                }

                GenerateIntermediateLanguageCodesForSerialize(serializeStatic, builderSet.Serialize);
                GenerateIntermediateLanguageCodesForDeserialize(deserializeStatic, builderSet.Deserialize);

                if(targetType.IsValueType)
                {
                    SerializeStaticHelper.SerializeStatic(analyzeResult, serializeStatic.GetILGenerator(), processor => processor.LdArgAddress(1));
                }
                else
                {
                    SerializeStaticHelper.SerializeStatic(analyzeResult, serializeStatic.GetILGenerator(), processor => processor.LdArg(1));
                }
                DeserializeStaticHelper.DeserializeStatic(analyzeResult, deserializeStatic.GetILGenerator());
                var formatter = Closing(builderSet.Type);
                return formatter;
#if ENABLE_MONO
            }
            else
            {
                var writerParams = TypeArrayHolder.Length3;
                writerParams[0] = typeof(JsonWriter).MakeByRefType();
                writerParams[1] = targetType;
                writerParams[2] = typeof(JsonSerializerOptions);

                var serializeStatic = new DynamicMethod(targetType.FullName + "<>SerializeStatic", null, writerParams, targetType, true);
                serializeStatic.DefineParameter(1, ParameterAttributes.None, "writer");
                serializeStatic.DefineParameter(2, ParameterAttributes.None, "value");
                serializeStatic.DefineParameter(3, ParameterAttributes.None, "options");
                serializeStatic.InitLocals = false;
                if(targetType.IsValueType)
                {
                    SerializeStaticHelper.SerializeStatic(analyzeResult, serializeStatic.GetILGenerator(), processor => processor.LdArgAddress(1));
                }
                else
                {
                    SerializeStaticHelper.SerializeStatic(analyzeResult, serializeStatic.GetILGenerator(), processor => processor.LdArg(1));
                }

                var readerParams = TypeArrayHolder.Length2;
                readerParams[0] = typeof(JsonReader).MakeByRefType();
                readerParams[1] = typeof(JsonSerializerOptions);

                var deserializeStatic = new DynamicMethod(targetType.FullName + "<>DeserializeStatic", targetType, readerParams, targetType, true);
                deserializeStatic.DefineParameter(1, ParameterAttributes.None, "reader");
                deserializeStatic.DefineParameter(2, ParameterAttributes.None, "options");
                deserializeStatic.InitLocals = true;
                DeserializeStaticHelper.DeserializeStatic(analyzeResult, deserializeStatic.GetILGenerator());
                var formatter = DynamicMethodFormatterGenerator.CreateFromDynamicMethods(serializeStatic, deserializeStatic);
                return formatter;
            }
#endif
        }

#if ENABLE_MONO
        private static bool DetectTypeIsPublic(Type targetType)
        {
            var typeIsPublic = targetType.IsPublic;
            if (typeIsPublic || !targetType.IsNested || targetType.DeclaringType is null)
            {
                return typeIsPublic;
            }

#if CSHARP_8_OR_NEWER
            for (var parent = targetType.DeclaringType; !(parent is null); parent = parent.DeclaringType!)
#else
            for (var parent = targetType.DeclaringType; !(parent is null); parent = parent.DeclaringType)
#endif
            {
                if (!parent.IsPublic && !parent.IsNestedPublic)
                {
                    return false;
                }
            }

            return true;
        }
#endif

#if CSHARP_8_OR_NEWER
        private static IJsonFormatter? EnumFactory(Type targetType)
#else
        private static IJsonFormatter EnumFactory(Type targetType)
#endif
        {
            EnsurePrivateAccess(targetType);
            var builderSet = BuilderSet.PrepareBuilderSet(targetType, moduleBuilder);
            var serializeStatic = DefineSerializeStatic(targetType, builderSet.Type);
            var deserializeStatic = DefineDeserializeStatic(targetType, builderSet.Type);

            ValueTypeEmbedTypelessHelper.SerializeTypeless(targetType, serializeStatic, builderSet.SerializeTypeless);
            ValueTypeEmbedTypelessHelper.DeserializeTypeless(targetType, deserializeStatic, builderSet.DeserializeTypeless);
            GenerateIntermediateLanguageCodesForSerialize(serializeStatic, builderSet.Serialize);
            GenerateIntermediateLanguageCodesForDeserialize(deserializeStatic, builderSet.Deserialize);

            var flags = targetType.GetCustomAttribute<FlagsAttribute>();
            if (flags is null)
            {
                EnumNumberEmbedHelper.Factory(targetType, builderSet.Type, serializeStatic, deserializeStatic);
            }
            else
            {
                EnumNumberEmbedHelper.Factory(targetType, builderSet.Type, serializeStatic, deserializeStatic);
            }

            var formatter = Closing(builderSet.Type);
            return formatter;
        }

        public const MethodAttributes StaticMethodFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        public const MethodAttributes InstanceMethodFlags = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

        private static MethodBuilder DefineSerializeStatic(Type targetType, TypeBuilder typeBuilder)
        {
            var writerParams = TypeArrayHolder.Length3;
            writerParams[0] = typeof(JsonWriter).MakeByRefType();
            writerParams[1] = targetType;
            writerParams[2] = typeof(JsonSerializerOptions);

            var serializeStatic = typeBuilder.DefineMethod("SerializeStatic", StaticMethodFlags, typeof(void), writerParams);
            serializeStatic.DefineParameter(1, ParameterAttributes.None, "writer");
            serializeStatic.DefineParameter(2, ParameterAttributes.None, "value");
            serializeStatic.DefineParameter(3, ParameterAttributes.None, "options");
            serializeStatic.InitLocals = false;
            return serializeStatic;
        }

        private static MethodBuilder DefineDeserializeStatic(Type targetType, TypeBuilder typeBuilder)
        {
            var readerParams = TypeArrayHolder.Length2;
            readerParams[0] = typeof(JsonReader).MakeByRefType();
            readerParams[1] = typeof(JsonSerializerOptions);

            var deserializeStatic = typeBuilder.DefineMethod("DeserializeStatic", StaticMethodFlags, targetType, readerParams);
            deserializeStatic.DefineParameter(1, ParameterAttributes.None, "reader");
            deserializeStatic.DefineParameter(2, ParameterAttributes.None, "options");
            deserializeStatic.InitLocals = true;
            return deserializeStatic;
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
#if ENABLE_MONO
            return default;
#else
            return FormatterCache<T>.SerializeStatic;
#endif
        }

        private struct FormatterCache<T>
        {
#if CSHARP_8_OR_NEWER
            public static readonly IJsonFormatter<T>? Formatter;
#else
            public static readonly IJsonFormatter<T> Formatter;
#endif

#if !ENABLE_MONO
            public static readonly IntPtr SerializeStatic;
            public static readonly IntPtr DeserializeStatic;
#endif

            static FormatterCache()
            {
                Formatter = formatterTable?.GetOrAdd(typeof(T), Factory) as IJsonFormatter<T>;
#if !ENABLE_MONO
                if (Formatter is null)
                {
                    SerializeStatic = default;
                    DeserializeStatic = default;
                    return;
                }

                var type = Formatter.GetType();
                SerializeStatic = StaticHelper.GetSerializeStatic(type);
                DeserializeStatic = StaticHelper.GetDeserializeStatic(type);
#endif
            }
        }
    }
}
