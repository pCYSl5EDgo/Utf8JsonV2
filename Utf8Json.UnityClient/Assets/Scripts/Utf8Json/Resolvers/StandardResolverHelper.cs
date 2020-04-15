// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Utf8Json.Resolvers;

namespace Utf8Json.Internal.Resolvers
{
    internal static class StandardResolverHelper
    {
        public static readonly IFormatterResolver[] DefaultResolvers =
        {
            new BuiltinResolver(), // Try Builtin
            new BasicGenericsResolver(), 
            //AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]

#if !ENABLE_IL2CPP
#if !NET_STANDARD_2_0
            //DynamicEnumResolver.Instance, // Try Enum
#endif
            //DynamicGenericResolver.Instance, // Try Array, Tuple, Collection, Enum(Generic Fallback)
#if !NET_STANDARD_2_0
            //DynamicUnionResolver.Instance, // Try Union(Interface)
#endif
#endif
        };
    }
}
