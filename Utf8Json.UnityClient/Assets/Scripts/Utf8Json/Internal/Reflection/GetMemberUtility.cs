// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable RedundantExplicitArraySize

using System;
using System.Diagnostics;
using System.Reflection;

namespace Utf8Json.Internal.Reflection
{
    public static class GetMemberUtility
    {
        public static MethodInfo GetMethodInstance(this Type type, string name)
        {
            var answer = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(!(answer is null));
            return answer;
        }

        public static MethodInfo GetMethodStatic(this Type type, string name)
        {
            var answer = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(!(answer is null));
            return answer;
        }

        public static MethodInfo GetMethodEmptyParameter(this Type type, string name)
        {
            var answer = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            Debug.Assert(!(answer is null));
            return answer;
        }

        public static MethodInfo GetMethod(this Type type, string name, Type parameterType0)
        {
            var types = TypeArrayHolder.TypeArrayLength1;
            types[0] = parameterType0;
            var answer = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
            Debug.Assert(!(answer is null));
            return answer;
        }

        public static MethodInfo GetMethod(this Type type, string name, Type parameterType0, Type parameterType1)
        {
            var types = TypeArrayHolder.TypeArrayLength2;
            types[0] = parameterType0;
            types[1] = parameterType1;
            var answer = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
            Debug.Assert(!(answer is null));
            return answer;
        }

        public static MethodInfo GetMethod(this Type type, string name, Type parameterType0, Type parameterType1, Type parameterType2)
        {
            var types = TypeArrayHolder.TypeArrayLength3;
            types[0] = parameterType0;
            types[1] = parameterType1;
            types[2] = parameterType2;
            var answer = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
            Debug.Assert(!(answer is null));
            return answer;
        }
    }
}
