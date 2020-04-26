// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    internal static class StandardResolverHelper
    {
        public static readonly IFormatterResolver[] DefaultResolvers;

        static StandardResolverHelper()
        {
            if (RuntimeFeature.IsDynamicCodeSupported)
            {
                DefaultResolvers = new IFormatterResolver[]
                {
                    BuiltinResolver.Instance,
                    BasicGenericsResolver.Instance,
                    AttributeFormatterResolver.Instance,
                    DynamicAssemblyBuilder.DynamicAssemblyBuilderResolver.Instance,
                };
            }
            else
            {
                DefaultResolvers = new IFormatterResolver[]
                {
                    BuiltinResolver.Instance,
                    BasicGenericsResolver.Instance,
                    AttributeFormatterResolver.Instance,
                };
            }
        }
    }
}
