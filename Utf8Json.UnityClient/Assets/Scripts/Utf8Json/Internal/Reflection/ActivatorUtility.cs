// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal.Reflection
{
    public static class ActivatorUtility
    {
#if CSHARP_8_OR_NEWER
        public static object? CreateInstance(Type type, object argument0)
#else
        public static object CreateInstance(Type type, object argument0)
#endif
        {
            var length1 = ObjectArrayHolder.Length1;
            length1[0] = argument0;
            var instance = Activator.CreateInstance(type, length1);
            return instance;
        }

#if CSHARP_8_OR_NEWER
        public static object? CreateInstance(Type type, object argument0, object argument1)
#else
        public static object CreateInstance(Type type, object argument0, object argument1)
#endif
        {
            var length2 = ObjectArrayHolder.Length2;
            length2[0] = argument0;
            length2[1] = argument1;
            var instance = Activator.CreateInstance(type, length2);
            return instance;
        }
    }
}
