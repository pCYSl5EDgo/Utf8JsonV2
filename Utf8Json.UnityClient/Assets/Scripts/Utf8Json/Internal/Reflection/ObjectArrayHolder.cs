// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal.Reflection
{
    public static class ObjectArrayHolder
    {
#if CSHARP_8_OR_NEWER
        [ThreadStatic] private static object?[]? length1;
        [ThreadStatic] private static object?[]? length2;
#else
        [ThreadStatic] private static object[] length1;
        [ThreadStatic] private static object[] length2;
#endif

#if CSHARP_8_OR_NEWER
        public static object?[] Length1
#else
        public static object[] Length1
#endif
        {
            get
            {
                if (length1 is null)
                {
                    length1 = new object[1];
                }

                return length1;
            }
        }

#if CSHARP_8_OR_NEWER
        public static object?[] Length2
#else
        public static object[] Length2
#endif
        {
            get
            {
                if (length2 is null)
                {
                    length2 = new object[2];
                }

                return length2;
            }
        }
    }
}
