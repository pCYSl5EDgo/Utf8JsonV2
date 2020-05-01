// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable RedundantExplicitArraySize

using System;
using System.Reflection;

namespace Utf8Json.Internal.Reflection
{
    public static class MakeGenericUtility
    {
        public static MethodInfo MakeGeneric(this MethodInfo method, Type parameterType0)
        {
            var types = TypeArrayHolder.Length1;
            types[0] = parameterType0;
            return method.MakeGenericMethod(types);
        }

        public static MethodInfo MakeGeneric(this MethodInfo method, Type parameterType0, Type parameterType1, Type parameterType2)
        {
            var types = TypeArrayHolder.Length3;
            types[0] = parameterType0;
            types[1] = parameterType1;
            types[2] = parameterType2;
            return method.MakeGenericMethod(types);
        }

        public static Type MakeGeneric(this Type baseType, Type parameterType0)
        {
            var types = TypeArrayHolder.Length1;
            types[0] = parameterType0;
            return baseType.MakeGenericType(types);
        }

        public static Type MakeGeneric(this Type baseType, Type parameterType0, Type parameterType1)
        {
            var types = TypeArrayHolder.Length2;
            types[0] = parameterType0;
            types[1] = parameterType1;
            return baseType.MakeGenericType(types);
        }

        public static Type MakeGeneric(this Type baseType, Type parameterType0, Type parameterType1, Type parameterType2)
        {
            var types = TypeArrayHolder.Length3;
            types[0] = parameterType0;
            types[1] = parameterType1;
            types[2] = parameterType2;
            return baseType.MakeGenericType(types);
        }
    }
}
