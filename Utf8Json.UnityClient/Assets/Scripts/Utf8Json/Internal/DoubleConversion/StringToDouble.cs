﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Utf8Json.Internal.DoubleConversion
{
    internal static class StringToDouble
    {
        // 2^53 = 9007199254740992.
        // Any integer with at most 15 decimal digits will hence fit into a double
        // (which has a 53bit significand) without loss of precision.
        private const int KMaxExactDoubleIntegerDecimalDigits = 15;
        // 2^64 = 18446744073709551616 > 10^19
        private const int KMaxUint64DecimalDigits = 19;

        // Max double: 1.7976931348623157 x 10^308
        // Min non-zero double: 4.9406564584124654 x 10^-324
        // Any x >= 10^309 is interpreted as +infinity.
        // Any x <= 10^-324 is interpreted as 0.
        // Note that 2.5e-324 (despite being smaller than the min double) will be read
        // as non-zero (equal to the min non-zero double).
        private const int KMaxDecimalPower = 309;
        private const int KMinDecimalPower = -324;

        // 2^64 = 18446744073709551616
        private const ulong KMaxUint64 = 0xFFFFFFFFFFFFFFFF;

        // ReSharper disable once RedundantExplicitArraySize
        private static readonly double[] exactPowersOfTen = new double[KExactPowersOfTenSize]
        {
            1.0,  // 10^0
            10.0,
            100.0,
            1000.0,
            10000.0,
            100000.0,
            1000000.0,
            10000000.0,
            100000000.0,
            1000000000.0,
            10000000000.0,  // 10^10
            100000000000.0,
            1000000000000.0,
            10000000000000.0,
            100000000000000.0,
            1000000000000000.0,
            10000000000000000.0,
            100000000000000000.0,
            1000000000000000000.0,
            10000000000000000000.0,
            100000000000000000000.0,  // 10^20
            1000000000000000000000.0,
            // 10^22 = 0x21e19e0c9bab2400000 = 0x878678326eac9 * 2^22
            10000000000000000000000.0
        };

        private const int KExactPowersOfTenSize = 23;

        // Maximum number of significant digits in the decimal representation.
        // In fact the value is 772 (see conversions.cc), but to give us some margin
        // we round up to 780.
        private const int KMaxSignificantDecimalDigits = 780;

        private static ReadOnlySpan<byte> TrimLeadingZeros(ReadOnlySpan<byte> buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != '0')
                {
                    return buffer.Slice(i);
                }
            }

            return ReadOnlySpan<byte>.Empty;
        }

        private static ReadOnlySpan<byte> TrimTrailingZeros(ReadOnlySpan<byte> buffer)
        {
            for (var i = buffer.Length - 1; i >= 0; --i)
            {
                if (buffer[i] != '0')
                {
                    return buffer.Slice(0, i + 1);
                }
            }

            return ReadOnlySpan<byte>.Empty;
        }

        private static void CutToMaxSignificantDigits(ReadOnlySpan<byte> buffer, int exponent, Span<byte> significantBuffer, out int significantExponent)
        {
            for (var i = 0; i < KMaxSignificantDecimalDigits - 1; ++i)
            {
                significantBuffer[i] = buffer[i];
            }
            // The input buffer has been trimmed. Therefore the last digit must be
            // different from '0'.
            // ASSERT(buffer[buffer.Length - 1] != '0');
            // Set the last digit to be non-zero. This is sufficient to guarantee
            // correct rounding.
            significantBuffer[KMaxSignificantDecimalDigits - 1] = (byte)'1';
            significantExponent = exponent + (buffer.Length - KMaxSignificantDecimalDigits);
        }

        // Reads digits from the buffer and converts them to a uint64.
        // Reads in as many digits as fit into a uint64.
        // When the string starts with "1844674407370955161" no further digit is read.
        // Since 2^64 = 18446744073709551616 it would still be possible read another
        // digit if it was less or equal than 6, but this would complicate the code.
        private static ulong ReadUint64(ReadOnlySpan<byte> buffer,
                           out int numberOfReadDigits)
        {
            ulong result = 0;
            var i = 0;
            while (i < buffer.Length && result <= KMaxUint64 / 10 - 1)
            {
                var digit = buffer[i++] - '0';
                result = 10 * result + (ulong)digit;
            }
            numberOfReadDigits = i;
            return result;
        }

        // Reads a DiyFp from the buffer.
        // The returned DiyFp is not necessarily normalized.
        // If remaining_decimals is zero then the returned DiyFp is accurate.
        // Otherwise it has been rounded and has error of at most 1/2 ulp.
        private static void ReadDiyFp(ReadOnlySpan<byte> buffer, out DiyFp result, out int remainingDecimals)
        {
            var significand = ReadUint64(buffer, out var readDigits);
            if (buffer.Length == readDigits)
            {
                result = new DiyFp(significand, 0);
                remainingDecimals = 0;
            }
            else
            {
                // Round the significand.
                if (buffer[readDigits] >= '5')
                {
                    significand++;
                }

                // Compute the binary exponent.
                const int exponent = 0;
                result = new DiyFp(significand, exponent);
                remainingDecimals = buffer.Length - readDigits;
            }
        }

        private static bool DoubleStrToDouble(ReadOnlySpan<byte> trimmed, int exponent, out double result)
        {
            if (trimmed.Length > KMaxExactDoubleIntegerDecimalDigits)
            {
                goto FAIL;
            }

            // The trimmed input fits into a double.
            // If the 10^exponent (resp. 10^-exponent) fits into a double too then we
            // can compute the result-double simply by multiplying (resp. dividing) the
            // two numbers.
            // This is possible because IEEE guarantees that floating-point operations
            // return the best possible approximation.
            if (exponent < 0 && -exponent < KExactPowersOfTenSize)
            {
                // 10^-exponent fits into a double.
                result = ReadUint64(trimmed, out _);
                result /= exactPowersOfTen[-exponent];
                return true;
            }

            if (0 <= exponent && exponent < KExactPowersOfTenSize)
            {
                // 10^exponent fits into a double.
                result = ReadUint64(trimmed, out _);
                result *= exactPowersOfTen[exponent];
                return true;
            }

            var remainingDigits = KMaxExactDoubleIntegerDecimalDigits - trimmed.Length;
            if (0 <= exponent && exponent - remainingDigits < KExactPowersOfTenSize)
            {
                // The trimmed string was short and we can multiply it with
                // 10^remaining_digits. As a result the remaining exponent now fits
                // into a double too.
                result = ReadUint64(trimmed, out _);
                result *= exactPowersOfTen[remainingDigits];
                result *= exactPowersOfTen[exponent - remainingDigits];
                return true;
            }

        FAIL:
            result = default;
            return false;
        }

        // Returns 10^exponent as an exact DiyFp.
        // The given exponent must be in the range [1; kDecimalExponentDistance[.
        private static DiyFp AdjustmentPowerOfTen(int exponent)
        {
            // Simply hard code the remaining powers for the given decimal exponent
            // distance.
            switch (exponent)
            {
                case 1: return new DiyFp(0xa000000000000000, -60);
                case 2: return new DiyFp(0xc800000000000000, -57);
                case 3: return new DiyFp(0xfa00000000000000, -54);
                case 4: return new DiyFp(0x9c40000000000000, -50);
                case 5: return new DiyFp(0xc350000000000000, -47);
                case 6: return new DiyFp(0xf424000000000000, -44);
                case 7: return new DiyFp(0x9896800000000000, -40);
                default:
                    throw new Exception("unreached code.");
            }
        }

        // If the function returns true then the result is the correct double.
        // Otherwise it is either the correct double or the double that is just below
        // the correct double.
        private static bool DiyFpStrToDouble(ReadOnlySpan<byte> buffer, int exponent, out double result)
        {
            ReadDiyFp(buffer, out var input, out var remainingDecimals);
            // Since we may have dropped some digits the input is not accurate.
            // If remaining_decimals is different than 0 than the error is at most
            // .5 ulp (unit in the last place).
            // We don't want to deal with fractions and therefore keep a common
            // denominator.
            const int kDenominatorLog = 3;
            const int kDenominator = 1 << kDenominatorLog;
            // Move the remaining decimals into the exponent.
            exponent += remainingDecimals;
            var error = (ulong)(remainingDecimals == 0 ? 0 : kDenominator / 2);

            var oldE = input.e;
            input.Normalize();
            error <<= oldE - input.e;

            if (exponent < PowersOfTenCache.kMinDecimalExponent)
            {
                result = 0.0;
                return true;
            }

            PowersOfTenCache.GetCachedPowerForDecimalExponent(exponent, out var cachedPower, out var cachedDecimalExponent);

            if (cachedDecimalExponent != exponent)
            {
                var adjustmentExponent = exponent - cachedDecimalExponent;
                var adjustmentPower = AdjustmentPowerOfTen(adjustmentExponent);
                input.Multiply(ref adjustmentPower);
                if (KMaxUint64DecimalDigits - buffer.Length >= adjustmentExponent)
                {
                    // The product of input with the adjustment power fits into a 64 bit
                    // integer.
                }
                else
                {
                    // The adjustment power is exact. There is hence only an error of 0.5.
                    error += kDenominator / 2;
                }
            }

            input.Multiply(ref cachedPower);
            // The error introduced by a multiplication of a*b equals
            //   error_a + error_b + error_a*error_b/2^64 + 0.5
            // Substituting a with 'input' and b with 'cached_power' we have
            //   error_b = 0.5  (all cached powers have an error of less than 0.5 ulp),
            //   error_ab = 0 or 1 / kDenominator > error_a*error_b/ 2^64
            const int errorB = kDenominator / 2;
            var errorAb = error == 0 ? 0 : 1;  // We round up to 1.
            const int fixedError = kDenominator / 2;
            error += (ulong)(errorB + errorAb + fixedError);

            oldE = input.e;
            input.Normalize();
            error <<= oldE - input.e;

            // See if the double's significand changes if we add/subtract the error.
            var orderOfMagnitude = DiyFp.kSignificandSize + input.e;
            var effectiveSignificandSize = IeeeDouble.SignificandSizeForOrderOfMagnitude(orderOfMagnitude);
            var precisionDigitsCount = DiyFp.kSignificandSize - effectiveSignificandSize;
            if (precisionDigitsCount + kDenominatorLog >= DiyFp.kSignificandSize)
            {
                // This can only happen for very small denormals. In this case the
                // half-way multiplied by the denominator exceeds the range of an uint64.
                // Simply shift everything to the right.
                var shiftAmount = precisionDigitsCount + kDenominatorLog -
                    DiyFp.kSignificandSize + 1;
                input.f >>= shiftAmount;
                input.e += shiftAmount;
                // We add 1 for the lost precision of error, and kDenominator for
                // the lost precision of input.f().
                error = (error >> shiftAmount) + 1 + kDenominator;
                precisionDigitsCount -= shiftAmount;
            }

            // We use uint64_ts now. This only works if the DiyFp uses uint64_ts too.
            const ulong one64 = 1;
            var precisionBitsMask = (one64 << precisionDigitsCount) - 1;
            var precisionBits = input.f & precisionBitsMask;
            var halfWay = one64 << (precisionDigitsCount - 1);
            precisionBits *= kDenominator;
            halfWay *= kDenominator;
            var roundedInput = new DiyFp(input.f >> precisionDigitsCount, input.e + precisionDigitsCount);
            if (precisionBits >= halfWay + error)
            {
                roundedInput.f++;
            }
            // If the last_bits are too close to the half-way case than we are too
            // inaccurate and round down. In this case we return false so that we can
            // fall back to a more precise algorithm.

            result = new IeeeDouble(roundedInput).Value();
            // Too imprecise. The caller will have to fall back to a slower version.
            // However the returned number is guaranteed to be either the correct
            // double, or the next-lower double.
            return halfWay - error >= precisionBits || precisionBits >= halfWay + error;
        }

        // Returns true if the guess is the correct double.
        // Returns false, when guess is either correct or the next-lower double.
        private static bool ComputeGuess(ReadOnlySpan<byte> trimmed, int exponent, out double guess)
        {
            if (trimmed.Length == 0)
            {
                guess = default;
                return true;
            }

            if (exponent + trimmed.Length - 1 >= KMaxDecimalPower)
            {
                guess = IeeeDouble.Infinity();
                return true;
            }

            if (exponent + trimmed.Length <= KMinDecimalPower)
            {
                guess = default;
                return true;
            }

            if (DoubleStrToDouble(trimmed, exponent, out guess) || DiyFpStrToDouble(trimmed, exponent, out guess))
            {
                return true;
            }

            return guess == IeeeDouble.Infinity();
        }

        public static bool TryParseToDouble(ReadOnlySpan<byte> buffer, int exponent, out double value)
        {
            Span<byte> copyBuffer = stackalloc byte[KMaxSignificantDecimalDigits];

            var leftTrimmed = TrimLeadingZeros(buffer);
            var rightTrimmed = TrimTrailingZeros(leftTrimmed);
            var exponent1 = exponent + leftTrimmed.Length - rightTrimmed.Length;

            var shouldCut = rightTrimmed.Length > KMaxSignificantDecimalDigits;
            if (!shouldCut)
            {
                return ComputeGuess(rightTrimmed, exponent1, out value);
            }

            CutToMaxSignificantDigits(rightTrimmed, exponent1, copyBuffer, out exponent);
            return ComputeGuess(copyBuffer, exponent, out value);
        }

        public static bool TryParseToFloat(ReadOnlySpan<byte> buffer, int exponent, out float value)
        {
            Span<byte> copyBuffer = stackalloc byte[KMaxSignificantDecimalDigits];

            var leftTrimmed = TrimLeadingZeros(buffer);
            var rightTrimmed = TrimTrailingZeros(leftTrimmed);
            var shouldCut = rightTrimmed.Length > KMaxSignificantDecimalDigits;
            bool isCorrect;
            double doubleValue;

            var exponent1 = exponent + leftTrimmed.Length - rightTrimmed.Length;
            if (shouldCut)
            {
                CutToMaxSignificantDigits(rightTrimmed, exponent1, copyBuffer, out exponent);
                isCorrect = ComputeGuess(copyBuffer, exponent, out doubleValue);
            }
            else
            {
                isCorrect = ComputeGuess(rightTrimmed, exponent1, out doubleValue);
            }

            value = (float)doubleValue;
            if (value == doubleValue)
            {
                // This shortcut triggers for integer values.
                return true;
            }

            // We must catch double-rounding. Say the double has been rounded up, and is
            // now a boundary of a float, and rounds up again. This is why we have to
            // look at previous too.
            // Example (in decimal numbers):
            //    input: 12349
            //    high-precision (4 digits): 1235
            //    low-precision (3 digits):
            //       when read from input: 123
            //       when rounded from high precision: 124.
            // To do this we simply look at the neigbors of the correct result and see
            // if they would round to the same float. If the guess is not correct we have
            // to look at four values (since two different doubles could be the correct
            // double).

            var doubleNext = new IeeeDouble(doubleValue).NextDouble();
            var doublePrevious = new IeeeDouble(doubleValue).PreviousDouble();

            var f1 = (float)doublePrevious;
            var f3 = (float)doubleNext;
            float f4;
            if (isCorrect)
            {
                f4 = f3;
            }
            else
            {
                var doubleNext2 = new IeeeDouble(doubleNext).NextDouble();
                f4 = (float)doubleNext2;
            }

            // If the guess doesn't lie near a single-precision boundary we can simply
            // return its float-value.
            return f1 == f4;
        }
    }
}