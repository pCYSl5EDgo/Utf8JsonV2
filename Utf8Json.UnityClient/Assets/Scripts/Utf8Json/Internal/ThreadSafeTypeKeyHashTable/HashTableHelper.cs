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
    }
}