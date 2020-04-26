// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection.Emit;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public readonly struct BuilderSet
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
