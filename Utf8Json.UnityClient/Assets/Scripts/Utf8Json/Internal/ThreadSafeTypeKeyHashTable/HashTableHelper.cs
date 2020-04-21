// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public static class HashTableHelper
    {
        public static int CalcPow2Number(int requiredSize)
        {
            if ((requiredSize & (requiredSize - 1)) == 0)
            {
                return requiredSize;
            }

            var answer = 1;
            while (answer < requiredSize)
            {
                answer <<= 1;
            }

            return answer;
        }

        public static int CalcSize(int size, double loadFactor)
        {
            if (loadFactor <= 0 || loadFactor >= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(loadFactor), loadFactor, "should not be in the range (0.0, 1.0).");
            }

            return CalcPow2Number((int)Math.Ceiling(size / loadFactor));
        }

#if CSHARP_8_OR_NEWER
        public static void Add<T>([System.Diagnostics.CodeAnalysis.NotNull]ref T[]? array, in T item)
#else
        public static void Add<T>(ref T[] array, in T item)
#endif
        {
            if (array == null)
            {
                array = new[] { item };
            }
            else
            {
                Array.Resize(ref array, array.Length + 1);
                // ReSharper disable once UseIndexFromEndExpression
                array[array.Length - 1] = item;
            }
        }

#if CSHARP_8_OR_NEWER
        public static void SortInsert<T>([System.Diagnostics.CodeAnalysis.NotNull]ref T[]? array, in T item)
#else
        public static void SortInsert<T>(ref T[] array, in T item)
#endif
            where T : IComparable<T>
        {
            if (array == null)
            {
                array = new[] { item };
            }
            else
            {
                var newArray = new T[array.Length + 1];
                for (var i = 0; i < array.Length; i++)
                {
                    var c = array[i].CompareTo(item);
                    if (c < 0)
                    {
                        newArray[i] = array[i];
                    }
                    else
                    {
                        newArray[i] = item;
                        Array.Copy(array, i, newArray, i + 1, array.Length - i);
                        array = newArray;
                        return;
                    }
                }
            }
        }
    }
}
