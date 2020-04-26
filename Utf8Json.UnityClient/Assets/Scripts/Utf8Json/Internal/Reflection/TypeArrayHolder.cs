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
        [ThreadStatic] private static Type[]? typeArrayLength1;
        [ThreadStatic] private static Type[]? typeArrayLength2;
        [ThreadStatic] private static Type[]? typeArrayLength3;
#else
        [ThreadStatic] private static Type[] typeArrayLength1;
        [ThreadStatic] private static Type[] typeArrayLength2;
        [ThreadStatic] private static Type[] typeArrayLength3;
#endif

        public static Type[] TypeArrayLength1 => typeArrayLength1 ?? (typeArrayLength1 = new Type[1]);

        public static Type[] TypeArrayLength2 => typeArrayLength2 ?? (typeArrayLength2 = new Type[2]);

        public static Type[] TypeArrayLength3 => typeArrayLength3 ?? (typeArrayLength3 = new Type[3]);
    }
}
