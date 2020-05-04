// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal.Reflection;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public readonly struct BuilderSet
    {
        public readonly TypeBuilder Type;
        public readonly MethodBuilder Serialize;
        public readonly MethodBuilder SerializeTypeless;
        public readonly MethodBuilder Deserialize;
        public readonly MethodBuilder DeserializeTypeless;

        public static string CreateFormatterName(Type targetType)
        {
            return "Utf8Json.IL.Emit.Formatters." + targetType.FullName?.Replace("+", "<>") + "<>Formatter";
        }

        public BuilderSet(TypeBuilder type, MethodBuilder serialize, MethodBuilder serializeTypeless, MethodBuilder deserialize, MethodBuilder deserializeTypeless)
        {
            Type = type;
            Serialize = serialize;
            SerializeTypeless = serializeTypeless;
            Deserialize = deserialize;
            DeserializeTypeless = deserializeTypeless;
        }

        public static BuilderSet PrepareBuilderSet(Type targetType, ModuleBuilder moduleBuilder)
        {
            var typeBuilder = moduleBuilder.DefineType(
                CreateFormatterName(targetType),
                TypeAttributes.AutoLayout | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                typeof(object),
                new[]
                {
                    typeof(IJsonFormatter),
                    typeof(IJsonFormatter<>).MakeGeneric(targetType),
                }
            );

            var writerParams = TypeArrayHolder.Length3;
            writerParams[0] = typeof(JsonWriter).MakeByRefType();
            writerParams[1] = targetType;
            writerParams[2] = typeof(JsonSerializerOptions);

            const MethodAttributes instanceMethodFlags = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var serialize = typeBuilder.DefineMethod("Serialize", instanceMethodFlags, typeof(void), writerParams);
            serialize.DefineParameter(1, ParameterAttributes.None, "writer");
            serialize.DefineParameter(2, ParameterAttributes.None, "value");
            serialize.DefineParameter(3, ParameterAttributes.None, "options");

            var writerTypelessParams = TypeArrayHolder.Length3;
            writerTypelessParams[0] = typeof(JsonWriter).MakeByRefType();
            writerTypelessParams[1] = typeof(object);
            writerTypelessParams[2] = typeof(JsonSerializerOptions);

            var serializeTypeless = typeBuilder.DefineMethod("SerializeTypeless", instanceMethodFlags, typeof(void), writerTypelessParams);
            serializeTypeless.DefineParameter(1, ParameterAttributes.None, "writer");
            serializeTypeless.DefineParameter(2, ParameterAttributes.None, "value");
            serializeTypeless.DefineParameter(3, ParameterAttributes.None, "options");

            var readerParams = TypeArrayHolder.Length2;
            readerParams[0] = typeof(JsonReader).MakeByRefType();
            readerParams[1] = typeof(JsonSerializerOptions);

            var deserialize = typeBuilder.DefineMethod("Deserialize", instanceMethodFlags, targetType, readerParams);
            deserialize.DefineParameter(1, ParameterAttributes.None, "reader");
            deserialize.DefineParameter(2, ParameterAttributes.None, "options");

            var deserializeTypeless = typeBuilder.DefineMethod("DeserializeTypeless", instanceMethodFlags, typeof(object), readerParams);
            deserializeTypeless.DefineParameter(1, ParameterAttributes.None, "reader");
            deserializeTypeless.DefineParameter(2, ParameterAttributes.None, "options");
            var builderSet = new BuilderSet(typeBuilder, serialize, serializeTypeless, deserialize, deserializeTypeless);
            return builderSet;
        }
    }
}
