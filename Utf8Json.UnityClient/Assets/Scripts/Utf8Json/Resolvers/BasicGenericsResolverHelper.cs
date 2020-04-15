// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers
{
    public sealed partial class BasicGenericsResolver
    {
        internal static class BasicGenericsResolverGetFormatterHelper
        {
            public static ThreadSafeTypeKeyFormatterHashTable.FunctionPair GetFunctionPointers(Type t)
            {
                return default;
                /*if (t.IsArray)
                {
                    switch (@enum)
                    {
                        
                    }
                    (t.GetArrayRank())
                    StaticHelper.GetSerializeStatic(formatterType);
                    StaticHelper.GetDeserializeStatic(formatterType);
                    StaticHelper.GetCalcByteLengthForSerialization(formatterType);
                    StaticHelper.GetSerializeSpan(formatterType);
                }*/
            }
        }
    }
}