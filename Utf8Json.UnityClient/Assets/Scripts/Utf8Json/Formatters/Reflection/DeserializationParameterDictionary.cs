// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using Utf8Json.Internal;

// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Formatters
{
    public sealed class DeserializationParameterDictionary
    {
#if CSHARP_8_OR_NEWER
        private readonly byte[][]?[] originalKeys;
        private readonly Type[]?[] parameterTypes;
        private readonly int[]?[] indices;
#else
        private readonly byte[][][] originalKeys;
        private readonly Type[][] parameterTypes;
        private readonly int[][] indices;
#endif


        public DeserializationParameterDictionary((ParameterInfo parameter, byte[] originalBytes)[] pairs)
        {
            if (pairs.Length == 0)
            {
                originalKeys = Array.Empty<byte[][]>();
                parameterTypes = Array.Empty<Type[]>();
                indices = Array.Empty<int[]>();
                return;
            }

            var maxLength = 0;
            for (var index = 0; index < pairs.Length; index++)
            {
                ref var valueTuple = ref pairs[index];
                if (maxLength < valueTuple.originalBytes.Length)
                {
                    maxLength = valueTuple.originalBytes.Length;
                }
            }

            originalKeys = new byte[maxLength - 1][][];
            parameterTypes = new Type[originalKeys.Length][];
            indices = new int[originalKeys.Length][];

            for (var index = 0; index < pairs.Length; index++)
            {
                ref var valueTuple = ref pairs[index];
                var bytesLength = valueTuple.originalBytes.Length;
                ref var keys = ref originalKeys[bytesLength - 1];
                ref var parameterTypeArray = ref parameterTypes[bytesLength - 1];
                ref var indexArray = ref indices[bytesLength - 1];
                if (keys == null)
                {
                    keys = new[]
                    {
                        valueTuple.originalBytes,
                    };
                    parameterTypeArray = new[]
                    {
                        valueTuple.parameter.ParameterType,
                    };
                    indexArray = new[]
                    {
                        index,
                    };
                }
                else
                {
                    Array.Resize(ref keys, keys.Length + 1);
                    keys[keys.Length - 1] = valueTuple.originalBytes;
                    Debug.Assert(parameterTypeArray != null);
                    Array.Resize(ref parameterTypeArray, parameterTypeArray.Length + 1);
                    parameterTypeArray[parameterTypeArray.Length - 1] = valueTuple.parameter.ParameterType;
                    Debug.Assert(indexArray != null);
                    Array.Resize(ref indexArray, indexArray.Length + 1);
                    indexArray[indexArray.Length - 1] = index;
                }
            }
        }

#if CSHARP_8_OR_NEWER
        public bool TryFindParameter(ReadOnlySpan<byte> propertyName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]out Type? parameterType, out int index)
#else
        public bool TryFindParameter(ReadOnlySpan<byte> propertyName, out Type parameterType, out int index)
#endif
        {
            parameterType = default;
            index = -1;
            if (propertyName.IsEmpty)
            {
                return false;
            }

            var key = propertyName.Length - 1;
            var keyArray = originalKeys[key];
            if (keyArray == null)
            {
                return false;
            }

            if (propertyName.SequenceEqual(keyArray[0]))
            {
#if CSHARP_8_OR_NEWER
                parameterType = parameterTypes[key]![0];
                index = indices[key]![0];
#else
                parameterType = parameterTypes[key][0];
                index = indices[key][0];
#endif
                return true;
            }

            for (var keyIndex = 1; keyIndex < originalKeys.Length; keyIndex++)
            {
                if (!propertyName.SequenceEqual(keyArray[keyIndex]))
                {
                    continue;
                }

#if CSHARP_8_OR_NEWER
                parameterType = parameterTypes[key]![keyIndex];
                index = indices[key]![keyIndex];
#else
                parameterType = parameterTypes[key][keyIndex];
                index = indices[key][keyIndex];
#endif
                return true;
            }

            return false;
        }

#if CSHARP_8_OR_NEWER
        public bool TryFindParameterIgnoreCase(ReadOnlySpan<byte> propertyName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]out Type? parameterType, out int index)
#else
        public bool TryFindParameterIgnoreCase(ReadOnlySpan<byte> propertyName, out Type parameterType, out int index)
#endif
        {
            parameterType = default;
            index = default;
            if (propertyName.IsEmpty)
            {
                return false;
            }

            var key = propertyName.Length - 1;
#if CSHARP_8_OR_NEWER
            var keyArray = originalKeys![key];
#else
            var keyArray = originalKeys[key];
#endif
            if (keyArray == null)
            {
                return false;
            }

            if (PropertyNameHelper.SequenceEqualsIgnoreCase(keyArray[0], propertyName))
            {
#if CSHARP_8_OR_NEWER
                parameterType = parameterTypes[key]![0];
                index = indices[key]![0];
#else
                parameterType = parameterTypes[key][0];
                index = indices[key][0];
#endif
                return true;
            }

            for (var keyIndex = 1; keyIndex < originalKeys.Length; keyIndex++)
            {
                if (!PropertyNameHelper.SequenceEqualsIgnoreCase(keyArray[keyIndex], propertyName))
                {
                    continue;
                }

#if CSHARP_8_OR_NEWER
                parameterType = parameterTypes[key]![keyIndex];
                index = indices[key]![keyIndex];
#else
                parameterType = parameterTypes[key][keyIndex];
                index = indices[key][keyIndex];
#endif
                return true;
            }

            return false;
        }
    }
}
