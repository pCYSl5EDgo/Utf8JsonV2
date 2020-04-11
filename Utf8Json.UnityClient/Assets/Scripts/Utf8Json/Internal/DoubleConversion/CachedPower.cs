// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal.DoubleConversion
{
    internal readonly struct CachedPower
    {
        public readonly ulong significand;
        public readonly short binary_exponent;
        public readonly short decimal_exponent;

        public CachedPower(ulong significand, short binaryExponent, short decimalExponent)
        {
            this.significand = significand;
            this.binary_exponent = binaryExponent;
            this.decimal_exponent = decimalExponent;
        }
    };
}