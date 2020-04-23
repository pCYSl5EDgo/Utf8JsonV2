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

#if CSHARP_8_OR_NEWER
        public void Clear(Span<object?> parameterSpan)
#else
        public void Clear(Span<object> parameterSpan)
#endif
        {
            for (var index = 0; index < parameterSpan.Length; index++)
            {
                var type = Parameters[index].parameter.ParameterType;
                ref var param = ref parameterSpan[index];
                if (!type.IsValueType)
                {
                    param = default;
                    continue;
                }

                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (type.FullName)
                {
                    case "System.IntPtr":
                        param = ObjectHelper.IntPtr;
                        break;
                    case "System.UIntPtr":
                        param = ObjectHelper.UIntPtr;
                        break;
                    case "System.SByte":
                        param = ObjectHelper.SByteArray[0];
                        break;
                    case "System.Byte":
                        param = ObjectHelper.ByteArray[0];
                        break;
                    case "System.Int16":
                        param = ObjectHelper.Int16Array[0];
                        break;
                    case "System.UInt16":
                        param = ObjectHelper.UInt16Array[0];
                        break;
                    case "System.Int32":
                        param = ObjectHelper.Int32Array[1];
                        break;
                    case "System.UInt32":
                        param = ObjectHelper.UInt32Array[0];
                        break;
                    case "System.Int64":
                        param = ObjectHelper.Int64Array[1];
                        break;
                    case "System.UInt64":
                        param = ObjectHelper.UInt64Array[0];
                        break;
                    case "System.Boolean":
                        param = ObjectHelper.False;
                        break;
                    case "System.Char":
                        param = ObjectHelper.Char;
                        break;
                    case "System.Single":
                        param = ObjectHelper.Single;
                        break;
                    case "System.Double":
                        param = ObjectHelper.Double;
                        break;
                    default:
                        param = ObjectHelper.ValueTypeDefaultValueHashTable.GetOrAdd(type, ObjectHelper.DefaultValueFactory);
                        break;
                }
            }
        }

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
