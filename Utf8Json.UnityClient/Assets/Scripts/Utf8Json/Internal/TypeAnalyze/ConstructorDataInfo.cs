// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct ConstructorDataInfo
    {
        public readonly Type TargetType;
#if CSHARP_8_OR_NEWER
        public readonly ConstructorInfo? Constructor;
        public readonly MethodInfo? FactoryMethod;
#else
        public readonly ConstructorInfo Constructor;
        public readonly MethodInfo FactoryMethod;
#endif

        public ConstructorDataInfo(Type targetType)
        {
            TargetType = targetType;
            Constructor = default;
            FactoryMethod = default;
        }

        public ConstructorDataInfo(Type targetType, ConstructorInfo constructor)
        {
            TargetType = targetType;
            Constructor = constructor;
            FactoryMethod = default;
        }

        public ConstructorDataInfo(Type targetType, MethodInfo factoryMethod)
        {
            TargetType = targetType;
            Constructor = default;
            FactoryMethod = factoryMethod;
        }
    }
}
