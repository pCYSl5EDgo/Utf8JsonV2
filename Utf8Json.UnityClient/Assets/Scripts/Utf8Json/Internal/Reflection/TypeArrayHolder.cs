// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

// ReSharper disable RedundantExplicitArraySize
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace Utf8Json.Internal.Reflection
{
    internal static class TypeArrayHolder
    {
#if CSHARP_8_OR_NEWER
        [ThreadStatic] private static Type[]? length1;
        [ThreadStatic] private static Type[]? length2;
        [ThreadStatic] private static Type[]? length3;
#else
        [ThreadStatic] private static Type[] length1;
        [ThreadStatic] private static Type[] length2;
        [ThreadStatic] private static Type[] length3;
#endif

        public static Type[] Length1 => length1 ?? (length1 = new Type[1]);

        public static Type[] Length2 => length2 ?? (length2 = new Type[2]);

        public static Type[] Length3 => length3 ?? (length3 = new Type[3]);
    }
}
