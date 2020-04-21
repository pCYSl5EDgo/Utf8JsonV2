// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Utf8Json.Resolvers;

namespace Utf8Json.Internal.Resolvers
{
    internal static class StandardResolverHelper
    {
        public static readonly IFormatterResolver[] DefaultResolvers =
        {
            BuiltinResolver.Instance,
            BasicGenericsResolver.Instance,
            ReflectionResolver.Instance,
        };
    }
}
