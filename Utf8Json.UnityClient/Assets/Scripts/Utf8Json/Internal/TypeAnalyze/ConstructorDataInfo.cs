// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
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

        public readonly (ParameterInfo parameter, byte[] originalBytes)[] Parameters;

        public ConstructorDataInfo(Type targetType)
        {
            TargetType = targetType;
            Constructor = default;
            FactoryMethod = default;
            Parameters = Array.Empty<ValueTuple<ParameterInfo, byte[]>>();
        }

        public ConstructorDataInfo(Type targetType, ConstructorInfo constructor)
        {
            TargetType = targetType;
            Constructor = constructor;
            FactoryMethod = default;
            var infos = constructor.GetParameters();
            var count = infos.Length;
            if (count == 0)
            {
                Parameters = Array.Empty<ValueTuple<ParameterInfo, byte[]>>();
                return;
            }

            Parameters = new ValueTuple<ParameterInfo, byte[]>[count];
            FillParameters(infos);
        }

        public ConstructorDataInfo(Type targetType, MethodInfo factoryMethod)
        {
            TargetType = targetType;
            Constructor = default;
            FactoryMethod = factoryMethod;
            var infos = factoryMethod.GetParameters();
            var count = infos.Length;
            if (count == 0)
            {
                Parameters = Array.Empty<ValueTuple<ParameterInfo, byte[]>>();
                return;
            }

            Parameters = new ValueTuple<ParameterInfo, byte[]>[count];
            FillParameters(infos);
        }

        private void FillParameters(ParameterInfo[] infos)
        {
            for (var index = 0; index < infos.Length; index++)
            {
                var info = infos[index];
                var name = info.Name;
                if (name == null)
                {
                    throw new NullReferenceException();
                }

                ref var tuple = ref Parameters[index];
                tuple.parameter = info;
                tuple.originalBytes = new byte[PropertyNameHelper.CalculatePropertyNameByteRawLength(name)];
                PropertyNameHelper.WritePropertyNameByteRawToSpan(name.AsSpan(), tuple.originalBytes.AsSpan());
            }
        }

        public object Create(object[] parameters)
        {
            Debug.Assert(parameters.Length == Parameters.Length);
            if (Constructor != null)
            {
                return Constructor.Invoke(parameters);
            }

            var answer = FactoryMethod != null
                ? FactoryMethod.Invoke(null, parameters)
                : Activator.CreateInstance(TargetType, true);

            Debug.Assert(answer != null);
            return answer;
        }
    }
}
