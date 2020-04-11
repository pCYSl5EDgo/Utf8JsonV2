// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Utf8Json.Internal.DoubleConversion
{
    // C# API
    internal static class DoubleToStringConverter
    {
        public static void GetBytes(ref BufferWriter writer, float value)
        {
            var resultBuilder = StringBuilder.Create();
            try
            {
                var doubleValue = new IeeeDouble(value);
                if (doubleValue.IsSpecial())
                {
                    if (!HandleSpecialValues(doubleValue, ref resultBuilder))
                    {
                        throw new InvalidOperationException("not support special float value:" + value);
                    }
                }
                else if (!ToShortestIeeeNumber(value, ref resultBuilder, DtoaMode.Shortest))
                {
                    throw new InvalidOperationException("not support float value:" + value);
                }

                var resultBuilderLength = resultBuilder.Length;
                var destination = writer.GetSpan(resultBuilderLength);
                resultBuilder.ReadableSpan.CopyTo(destination);
                writer.Advance(resultBuilderLength);
            }
            finally
            {
                resultBuilder.Dispose();
            }
        }

        public static void GetBytes(ref BufferWriter writer, double value)
        {
            var resultBuilder = StringBuilder.Create();
            try
            {
                var doubleValue = new IeeeDouble(value);
                if (doubleValue.IsSpecial())
                {
                    if (!HandleSpecialValues(doubleValue, ref resultBuilder))
                    {
                        throw new InvalidOperationException("not support special double value:" + value);
                    }
                }
                else if (!ToShortestIeeeNumber(value, ref resultBuilder, DtoaMode.Shortest))
                {
                    throw new InvalidOperationException("not support double value:" + value);
                }

                var resultBuilderLength = resultBuilder.Length;
                var destination = writer.GetSpan(resultBuilderLength);
                resultBuilder.ReadableSpan.CopyTo(destination);
                writer.Advance(resultBuilderLength);
            }
            finally
            {
                resultBuilder.Dispose();
            }
        }

        // private porting methods
        // https://github.com/google/double-conversion/blob/master/double-conversion/fast-dtoa.h
        // https://github.com/google/double-conversion/blob/master/double-conversion/fast-dtoa.cc

        private enum FastDtoaMode
        {
            // Computes the shortest representation of the given input. The returned
            // result will be the most accurate number of this length. Longer
            // representations might be more accurate.
            FastDtoaShortest,
            // Same as FAST_DTOA_SHORTEST but for single-precision floats.
            FastDtoaShortestSingle,
        };

        private enum DtoaMode
        {
            Shortest,
            ShortestSingle,
        }

        // C# constants
        private const int InfinitySymbolLength = 8;
        // StringEncoding.UTF8.GetBytes(double.PositiveInfinity.ToString(CultureInfo.InvariantCulture));
        // ReSharper disable once RedundantExplicitArraySize
        private static readonly byte[] infinitySymbol = new byte[InfinitySymbolLength] { 0x49, 0x6E, 0x66, 0x69, 0x6E, 0x69, 0x74, 0x79, };

        private const int NanSymbolLength = 3;
        // StringEncoding.UTF8.GetBytes(double.NaN.ToString(CultureInfo.InvariantCulture));
        // ReSharper disable once RedundantExplicitArraySize
        private static readonly byte[] nanSymbol = new byte[NanSymbolLength] { 0x4E, 0x61, 0x4E, };

        // constructor parameter, same as EcmaScriptConverter
        //DoubleToStringConverter(int flags,
        //                  const char* infinity_symbol,
        //                  const char* nan_symbol,
        //                  char exponent_character,
        //                  int decimal_in_shortest_low,
        //                  int decimal_in_shortest_high,
        //                  int max_leading_padding_zeroes_in_precision_mode,
        //                  int max_trailing_padding_zeroes_in_precision_mode)

        private const int DecimalInShortestLow = -4; // C# ToString("G")
        private const int DecimalInShortestHigh = 15; // C# ToString("G")

        private const int KBase10MaximalLength = 17;

        // The minimal and maximal target exponent define the range of w's binary
        // exponent, where 'w' is the result of multiplying the input by a cached power
        // of ten.
        //
        // A different range might be chosen on a different platform, to optimize digit
        // generation, but a smaller range requires more powers of ten to be cached.
        private const int KMinimalTargetExponent = -60;

        /// <summary>
        /// Adjusts the last digit of the generated number, and screens out generated solutions that may be inaccurate.
        /// A solution may be inaccurate if it is outside the safe interval, or if we cannot prove that it is closer to the input than a neighboring representation of the same length.
        /// </summary>
        /// <param name="buffer">buffer containing the digits of too_high / 10^kappa</param>
        /// <param name="length">the buffer's length</param>
        /// <param name="distanceTooHighW">(too_high - w).f() * unit</param>
        /// <param name="unsafeInterval">(too_high - too_low).f() * unit</param>
        /// <param name="rest">(too_high - buffer * 10^kappa).f() * unit</param>
        /// <param name="tenKappa">10^kappa * unit</param>
        /// <param name="unit">the common multiplier</param>
        /// <returns>returns true if the buffer is guaranteed to contain the closest representable number to the input. Modifies the generated digits in the buffer to approach (round towards) w.</returns>
        private static bool RoundWeed(Span<byte> buffer, int length, ulong distanceTooHighW, ulong unsafeInterval, ulong rest, ulong tenKappa, ulong unit)
        {
            var smallDistance = distanceTooHighW - unit;
            var bigDistance = distanceTooHighW + unit;

            #region Comment
            /*
            Let w_low  = too_high - big_distance, and
                w_high = too_high - small_distance.
            Note: w_low < w < w_high
            
            The real w (* unit) must lie somewhere inside the interval
            ]w_low; w_high[ (often written as "(w_low; w_high)")

            Basically the buffer currently contains a number in the unsafe interval
            ]too_low; too_high[ with too_low < w < too_high
            
             too_high - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
                                ^v 1 unit            ^      ^                 ^      ^
             boundary_high ---------------------     .      .                 .      .
                                ^v 1 unit            .      .                 .      .
              - - - - - - - - - - - - - - - - - - -  +  - - + - - - - - -     .      .
                                                     .      .         ^       .      .
                                                     .  big_distance  .       .      .
                                                     .      .         .       .    rest
                                         small_distance     .         .       .      .
                                                     v      .         .       .      .
             w_high - - - - - - - - - - - - - - - - - -     .         .       .      .
                                ^v 1 unit                   .         .       .      .
             w ----------------------------------------     .         .       .      .
                                ^v 1 unit                   v         .       .      .
             w_low  - - - - - - - - - - - - - - - - - - - - -         .       .      .
                                                                      .       .      v
             buffer --------------------------------------------------+-------+--------
                                                                      .       .
                                                             safe_interval    .
                                                                      v       .
              - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -     .
                                ^v 1 unit                                     .
             boundary_low -------------------------                     unsafe_interval
                                ^v 1 unit                                     v
             too_low  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            
            
            Note that the value of buffer could lie anywhere inside the range too_low to too_high.
            
            boundary_low, boundary_high and w are approximations of the real boundaries and v (the input number).
            They are guaranteed to be precise up to one unit.
            In fact the error is guaranteed to be strictly less than one unit.
            
            Anything that lies outside the unsafe interval is guaranteed not to round to v when read again.
            Anything that lies inside the safe interval is guaranteed to round to v when read again.
            If the number inside the buffer lies inside the unsafe interval but not inside the safe interval then we simply do not know and bail out (returning false).
            
            Similarly we have to take into account the imprecision of 'w' when finding the closest representation of 'w'.
            If we have two potential representations, and one is closer to both w_low and w_high, then we know it is closer to the actual value v.
            
            By generating the digits of too_high we got the largest (closest to too_high) buffer that is still in the unsafe interval.
            In the case where w_high < buffer < too_high we try to decrement the buffer.
            This way the buffer approaches (rounds towards) w.
            There are 3 conditions that stop the decrementation process:
              1) the buffer is already below w_high
              2) decrementing the buffer would make it leave the unsafe interval
              3) decrementing the buffer would yield a number below w_high and farther
                 away than the current number. In other words:
                         (buffer{-1} < w_high) && w_high - buffer{-1} > buffer - w_high
            Instead of using the buffer directly we use its distance to too_high.
            Conceptually rest ~= too_high - buffer We need to do the following tests in this order to avoid over- and underflow.
            */
            #endregion
            while (rest < smallDistance &&  // Negated condition 1
                   unsafeInterval - rest >= tenKappa &&  // Negated condition 2
                   (rest + tenKappa < smallDistance ||  // buffer{-1} > w_high
                    smallDistance - rest >= rest + tenKappa - smallDistance))
            {
                buffer[length - 1]--;
                rest += tenKappa;
            }

            // We have approached w+ as much as possible. We now test if approaching w-
            // would require changing the buffer. If yes, then we have two possible
            // representations close to w, but we cannot decide which one is closer.
            if (rest < bigDistance &&
                unsafeInterval - rest >= tenKappa &&
                (rest + tenKappa < bigDistance ||
                 bigDistance - rest > rest + tenKappa - bigDistance))
            {
                return false;
            }

            // Weeding test.
            //   The safe interval is [too_low + 2 ulp; too_high - 2 ulp]
            //   Since too_low = too_high - unsafe_interval this is equivalent to
            //      [too_high - unsafe_interval + 4 ulp; too_high - 2 ulp]
            //   Conceptually we have: rest ~= too_high - buffer
            return 2 * unit <= rest && rest <= unsafeInterval - 4 * unit;
        }

        // Returns the biggest power of ten that is less than or equal to the given
        // number. We furthermore receive the maximum number of bits 'number' has.
        //
        // Returns power == 10^(exponent_plus_one-1) such that
        //    power <= number < power * 10.
        // If number_bits == 0 then 0^(0-1) is returned.
        // The number of bits must be <= 32.
        // Precondition: number < (1 << (number_bits + 1)).

        // Inspired by the method for finding an integer log base 10 from here:
        // http://graphics.stanford.edu/~seander/bithacks.html#IntegerLog10
        private static readonly uint[] kSmallPowersOfTen = new uint[] { 0, 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        private static void BiggestPowerTen(uint number,
                                    int numberBits,
                                    out uint power,
                                    out int exponentPlusOne)
        {
            // 1233/4096 is approximately 1/lg(10).
            var exponentPlusOneGuess = (numberBits + 1) * 1233 >> 12;
            // We increment to skip over the first entry in the kPowersOf10 table.
            // Note: kPowersOf10[i] == 10^(i-1).
            exponentPlusOneGuess++;
            // We don't have any guarantees that 2^number_bits <= number.
            if (number < kSmallPowersOfTen[exponentPlusOneGuess])
            {
                exponentPlusOneGuess--;
            }
            power = kSmallPowersOfTen[exponentPlusOneGuess];
            exponentPlusOne = exponentPlusOneGuess;
        }

        // Generates the digits of input number w.
        // w is a floating-point number (DiyFp), consisting of a significand and an
        // exponent. Its exponent is bounded by kMinimalTargetExponent and
        // kMaximalTargetExponent.
        //       Hence -60 <= w.e() <= -32.
        //
        // Returns false if it fails, in which case the generated digits in the buffer
        // should not be used.
        // Preconditions:
        //  * low, w and high are correct up to 1 ulp (unit in the last place). That
        //    is, their error must be less than a unit of their last digits.
        //  * low.e() == w.e() == high.e()
        //  * low < w < high, and taking into account their error: low~ <= high~
        //  * kMinimalTargetExponent <= w.e() <= kMaximalTargetExponent
        // Post conditions: returns false if procedure fails.
        //   otherwise:
        //     * buffer is not null-terminated, but len contains the number of digits.
        //     * buffer contains the shortest possible decimal digit-sequence
        //       such that LOW < buffer * 10^kappa < HIGH, where LOW and HIGH are the
        //       correct values of low and high (without their error).
        //     * if more than one decimal representation gives the minimal number of
        //       decimal digits then the one closest to W (where W is the correct value
        //       of w) is chosen.
        // Remark: this procedure takes into account the imprecision of its input
        //   numbers. If the precision is not enough to guarantee all the post conditions
        //   then false is returned. This usually happens rarely (~0.5%).
        //
        // Say, for the sake of example, that
        //   w.e() == -48, and w.f() == 0x1234567890abcdef
        // w's value can be computed by w.f() * 2^w.e()
        // We can obtain w's integral digits by simply shifting w.f() by -w.e().
        //  -> w's integral part is 0x1234
        //  w's fractional part is therefore 0x567890abcdef.
        // Printing w's integral part is easy (simply print 0x1234 in decimal).
        // In order to print its fraction we repeatedly multiply the fraction by 10 and
        // get each digit. Example the first digit after the point would be computed by
        //   (0x567890abcdef * 10) >> 48. -> 3
        // The whole thing becomes slightly more complicated because we want to stop
        // once we have enough digits. That is, once the digits inside the buffer
        // represent 'w' we can stop. Everything inside the interval low - high
        // represents w. However we have to pay attention to low, high and w's
        // imprecision.
        private static bool DigitGen(DiyFp low,
                             DiyFp w,
                             DiyFp high,
                             Span<byte> buffer,
                             out int length,
                             out int kappa)
        {
            // low, w and high are imprecise, but by less than one ulp (unit in the last
            // place).
            // If we remove (resp. add) 1 ulp from low (resp. high) we are certain that
            // the new numbers are outside of the interval we want the final
            // representation to lie in.
            // Inversely adding (resp. removing) 1 ulp from low (resp. high) would yield
            // numbers that are certain to lie in the interval. We will use this fact
            // later on.
            // We will now start by generating the digits within the uncertain
            // interval. Later we will weed out representations that lie outside the safe
            // interval and thus _might_ lie outside the correct interval.
            ulong unit = 1;
            var tooLow = new DiyFp(low.f - unit, low.e);
            var tooHigh = new DiyFp(high.f + unit, high.e);
            // too_low and too_high are guaranteed to lie outside the interval we want the
            // generated number in.
            var unsafeInterval = DiyFp.Minus(ref tooHigh, ref tooLow);
            // We now cut the input number into two parts: the integral digits and the
            // fractionals. We will not write any decimal separator though, but adapt
            // kappa instead.
            // Reminder: we are currently computing the digits (stored inside the buffer)
            // such that:   too_low < buffer * 10^kappa < too_high
            // We use too_high for the digit_generation and stop as soon as possible.
            // If we stop early we effectively round down.
            var one = new DiyFp((ulong)1 << -w.e, w.e);
            // Division by one is a shift.
            var integrals = (uint)(tooHigh.f >> -one.e);
            // Modulo by one is an and.
            var fractionals = tooHigh.f & (one.f - 1);
            BiggestPowerTen(integrals, DiyFp.kSignificandSize - -one.e,
                            out var divisor, out var divisorExponentPlusOne);
            kappa = divisorExponentPlusOne;
            length = 0;
            // Loop invariant: buffer = too_high / 10^kappa  (integer division)
            // The invariant holds for the first iteration: kappa has been initialized
            // with the divisor exponent + 1. And the divisor is the biggest power of ten
            // that is smaller than integrals.
            while (kappa > 0)
            {
                var digit = unchecked((int)(integrals / divisor));
                buffer[length] = (byte)((byte)'0' + digit);
                length++;
                integrals %= divisor;
                kappa--;
                // Note that kappa now equals the exponent of the divisor and that the
                // invariant thus holds again.
                var rest =
                    ((ulong)integrals << -one.e) + fractionals;
                // Invariant: too_high = buffer * 10^kappa + DiyFp(rest, one.e())
                // Reminder: unsafe_interval.e() == one.e()
                if (rest < unsafeInterval.f)
                {
                    // Rounding down (by not emitting the remaining digits) yields a number
                    // that lies within the unsafe interval.
                    return RoundWeed(buffer, length, DiyFp.Minus(ref tooHigh, ref w).f,
                                     unsafeInterval.f, rest,
                                     (ulong)divisor << -one.e, unit);
                }
                divisor /= 10;
            }

            // The integrals have been generated. We are at the point of the decimal
            // separator. In the following loop we simply multiply the remaining digits by
            // 10 and divide by one. We just need to pay attention to multiply associated
            // data (like the interval or 'unit'), too.
            // Note that the multiplication by 10 does not overflow, because w.e >= -60
            // and thus one.e >= -60.
            for (; ; )
            {
                fractionals *= 10;
                unit *= 10;
                unsafeInterval.f *= 10;
                // Integer division by one.
                var digit = (int)(fractionals >> -one.e);
                buffer[length] = (byte)((byte)'0' + digit);
                length++;
                fractionals &= one.f - 1;  // Modulo by one.
                kappa--;
                if (fractionals < unsafeInterval.f)
                {
                    return RoundWeed(buffer, length, DiyFp.Minus(ref tooHigh, ref w).f * unit,
                                     unsafeInterval.f, fractionals, one.f, unit);
                }
            }
        }

        // Provides a decimal representation of v.
        // Returns true if it succeeds, otherwise the result cannot be trusted.
        // There will be *length digits inside the buffer (not null-terminated).
        // If the function returns true then
        //        v == (double) (buffer * 10^decimal_exponent).
        // The digits in the buffer are the shortest representation possible: no
        // 0.09999999999999999 instead of 0.1. The shorter representation will even be
        // chosen even if the longer one would be closer to v.
        // The last digit will be closest to the actual v. That is, even if several
        // digits might correctly yield 'v' when read again, the closest will be
        // computed.
        private static bool Grisu3(double v, FastDtoaMode mode, Span<byte> buffer, out int length, out int decimalExponent)
        {
            var w = new IeeeDouble(v).AsNormalizedDiyFp();
            // boundary_minus and boundary_plus are the boundaries between v and its
            // closest floating-point neighbors. Any number strictly between
            // boundary_minus and boundary_plus will round to v when convert to a double.
            // Grisu3 will never output representations that lie exactly on a boundary.
            DiyFp boundaryMinus, boundaryPlus;
            switch (mode)
            {
                case FastDtoaMode.FastDtoaShortest:
                    new IeeeDouble(v).NormalizedBoundaries(out boundaryMinus, out boundaryPlus);
                    break;
                case FastDtoaMode.FastDtoaShortestSingle:
                    {
                        var singleV = (float)v;
                        new IeeeSingle(singleV).NormalizedBoundaries(out boundaryMinus, out boundaryPlus);
                        break;
                    }
                default:
                    throw new Exception("Invalid Mode.");
            }

            var tenMkMinimalBinaryExponent = KMinimalTargetExponent - (w.e + DiyFp.kSignificandSize);
            PowersOfTenCache.GetCachedPowerForBinaryExponentRange(tenMkMinimalBinaryExponent, out var tenMk, out var mk);

            // Note that ten_mk is only an approximation of 10^-k. A DiyFp only contains a
            // 64 bit significand and ten_mk is thus only precise up to 64 bits.

            // The DiyFp::Times procedure rounds its result, and ten_mk is approximated
            // too. The variable scaled_w (as well as scaled_boundary_minus/plus) are now
            // off by a small amount.
            // In fact: scaled_w - w*10^k < 1ulp (unit in the last place) of scaled_w.
            // In other words: let f = scaled_w.f() and e = scaled_w.e(), then
            //           (f-1) * 2^e < w*10^k < (f+1) * 2^e
            var scaledW = DiyFp.Times(ref w, ref tenMk);

            // In theory it would be possible to avoid some recomputations by computing
            // the difference between w and boundary_minus/plus (a power of 2) and to
            // compute scaled_boundary_minus/plus by subtracting/adding from
            // scaled_w. However the code becomes much less readable and the speed
            // enhancements are not terrific.
            var scaledBoundaryMinus = DiyFp.Times(ref boundaryMinus, ref tenMk);
            var scaledBoundaryPlus = DiyFp.Times(ref boundaryPlus, ref tenMk);

            // DigitGen will generate the digits of scaled_w. Therefore we have
            // v == (double) (scaled_w * 10^-mk).
            // Set decimal_exponent == -mk and pass it to DigitGen. If scaled_w is not an
            // integer than it will be updated. For instance if scaled_w == 1.23 then
            // the buffer will be filled with "123" und the decimal_exponent will be
            // decreased by 2.
            var result = DigitGen(scaledBoundaryMinus, scaledW, scaledBoundaryPlus,
                                   buffer, out length, out var kappa);
            decimalExponent = -mk + kappa;
            return result;
        }

        private static bool FastDtoa(double v,
              FastDtoaMode mode,
              Span<byte> buffer,
              out int length,
              out int decimalPoint)
        {
            switch (mode)
            {
                case FastDtoaMode.FastDtoaShortest:
                case FastDtoaMode.FastDtoaShortestSingle:
                    var result = Grisu3(v, mode, buffer, out length, out var decimalExponent);
                    decimalPoint = result ? length + decimalExponent : -1;
                    return result;
                // case FastDtoaMode.FAST_DTOA_PRECISION:
                //result = Grisu3Counted(v, requested_digits, buffer, length, &decimal_exponent);
                default:
                    throw new Exception("unreachable code.");
            }
        }

        // https://github.com/google/double-conversion/blob/master/double-conversion/double-conversion.cc

        private static bool HandleSpecialValues(in IeeeDouble ieeeDoubleInspect, ref StringBuilder resultBuilder)
        {
            if (ieeeDoubleInspect.IsInfinite())
            {
                if (ieeeDoubleInspect.Sign() < 0)
                {
                    resultBuilder.EnsureAdditionalCapacity(1 + InfinitySymbolLength);
                    resultBuilder.AppendCharMinusUnsafe();
                }
                else
                {
                    resultBuilder.EnsureAdditionalCapacity(InfinitySymbolLength);
                }

                infinitySymbol.CopyTo(resultBuilder.WritableSpan);
                resultBuilder.Advance(InfinitySymbolLength);
                return true;
            }

            if (!ieeeDoubleInspect.IsNan() || nanSymbol == null)
            {
                return false;
            }

            resultBuilder.EnsureAdditionalCapacity(NanSymbolLength);

            nanSymbol.CopyTo(resultBuilder.WritableSpan);
            resultBuilder.Advance(NanSymbolLength);
            return true;
        }

        private static bool ToShortestIeeeNumber(double value, ref StringBuilder resultBuilder, DtoaMode mode)
        {
            const int kDecimalRepCapacity = KBase10MaximalLength + 1;
            Span<byte> decimalRep = stackalloc byte[kDecimalRepCapacity];

            var fastWorked = DoubleToAscii(value, mode, decimalRep, out var isNegativeSign, out var decimalRepLength, out var decimalPoint);

            if (!fastWorked)
            {
                // C# custom, slow path
                var str = value.ToString("G17", CultureInfo.InvariantCulture);
                resultBuilder.AppendString(str);
                return true;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (isNegativeSign && value != 0.0)
            {
                resultBuilder.AppendCharMinusUnsafe();
            }

            var exponent = decimalPoint - 1;
            if (DecimalInShortestLow <= exponent && exponent < DecimalInShortestHigh)
            {
                var digitsAfterPoint = decimalRepLength - decimalPoint;
                if (digitsAfterPoint < 0)
                {
                    digitsAfterPoint = 0;
                }

                // Create a representation that is padded with zeros if needed.
                if (decimalPoint <= 0)
                {
                    // "0.00000decimal_rep" or "0.000decimal_rep00".
                    if (digitsAfterPoint <= 0)
                    {
                        resultBuilder.EnsureAdditionalCapacity(1);

                        resultBuilder.AppendChar0Unsafe();
                    }
                    else
                    {
                        resultBuilder.EnsureAdditionalCapacity(digitsAfterPoint + 2);

                        resultBuilder.AppendChar0Unsafe();
                        resultBuilder.AppendCharDotUnsafe();

                        for (var i = -decimalPoint; --i >= 0;)
                        {
                            resultBuilder.AppendChar0Unsafe();
                        }

                        decimalRep.Slice(0, decimalRepLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(decimalRepLength);

                        var remainingDigits = digitsAfterPoint + decimalPoint - decimalRepLength;
                        for (var i = remainingDigits; --i >= 0;)
                        {
                            resultBuilder.AppendChar0Unsafe();
                        }
                    }
                }
                else if (decimalPoint >= decimalRepLength)
                {
                    // "decimal_rep0000.00000" or "decimal_rep.0000".
                    if (digitsAfterPoint <= 0)
                    {
                        resultBuilder.EnsureAdditionalCapacity(decimalPoint);

                        decimalRep.Slice(0, decimalRepLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(decimalRepLength);
                        for (var i = decimalPoint - decimalRepLength; --i >= 0;)
                        {
                            resultBuilder.AppendChar0Unsafe();
                        }
                    }
                    else
                    {
                        resultBuilder.EnsureAdditionalCapacity(decimalPoint + digitsAfterPoint + 1);

                        decimalRep.Slice(0, decimalRepLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(decimalRepLength);

                        for (var i = decimalPoint - decimalRepLength; --i >= 0;)
                        {
                            resultBuilder.AppendChar0Unsafe();
                        }

                        resultBuilder.AppendCharDotUnsafe();

                        for (var i = digitsAfterPoint; --i >= 0;)
                        {
                            resultBuilder.AppendChar0Unsafe();
                        }
                    }
                }
                else
                {
                    // ReSharper disable once CommentTypo
                    // "decima.l_rep000".
                    resultBuilder.EnsureAdditionalCapacity(decimalPoint + digitsAfterPoint + 1);

                    decimalRep.Slice(0, decimalPoint).CopyTo(resultBuilder.WritableSpan);
                    resultBuilder.Advance(decimalPoint);
                    resultBuilder.AppendCharDotUnsafe();

                    var pointLength = decimalRepLength - decimalPoint;
                    decimalRep.Slice(decimalPoint, pointLength).CopyTo(resultBuilder.WritableSpan);
                    resultBuilder.Advance(pointLength);

                    var remainingDigits = digitsAfterPoint - pointLength;
                    for (var i = remainingDigits; --i >= 0;)
                    {
                        resultBuilder.AppendChar0Unsafe();
                    }
                }
            }
            else
            {
                const int kMaxExponentLength = 5;
                if (decimalRepLength != 1)
                {
                    if (exponent < 0)
                    {
                        var exponentAbsoluteValue = -exponent;
                        Span<byte> buffer = stackalloc byte[kMaxExponentLength + 1];
                        buffer[kMaxExponentLength] = (byte)'\0';
                        var firstCharPos = kMaxExponentLength;
                        while (exponentAbsoluteValue > 0)
                        {
                            var div10 = exponentAbsoluteValue / 10;
                            buffer[--firstCharPos] = (byte)('0' + exponentAbsoluteValue - div10 * 10);
                            exponentAbsoluteValue = div10;
                        }

                        var maxExponentLength = kMaxExponentLength - firstCharPos;

                        resultBuilder.EnsureAdditionalCapacity(maxExponentLength + decimalRepLength + 3);

                        resultBuilder.AppendByteUnsafe(decimalRep[0]);
                        resultBuilder.AppendCharDotUnsafe();
                        decimalRep.Slice(1, decimalRepLength - 1).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(decimalRepLength - 1);
                        resultBuilder.AppendCharEUnsafe();
                        resultBuilder.AppendCharMinusUnsafe();
                        buffer.Slice(firstCharPos, maxExponentLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(maxExponentLength);
                    }
                    else if (exponent == 0)
                    {
                        resultBuilder.EnsureAdditionalCapacity(decimalRepLength + 4);

                        resultBuilder.AppendByteUnsafe(decimalRep[0]);
                        resultBuilder.AppendCharDotUnsafe();
                        decimalRep.Slice(1, decimalRepLength - 1).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(decimalRepLength - 1);
                        resultBuilder.AppendCharEUnsafe();
                        resultBuilder.AppendCharPlusUnsafe();
                        resultBuilder.AppendChar0Unsafe();
                    }
                    else
                    {
                        Span<byte> buffer = stackalloc byte[kMaxExponentLength + 1];
                        buffer[kMaxExponentLength] = (byte)'\0';
                        var firstCharPos = kMaxExponentLength;
                        var exponentAbsoluteValue = exponent;
                        while (exponentAbsoluteValue > 0)
                        {
                            var div10 = exponentAbsoluteValue / 10;
                            buffer[--firstCharPos] = (byte)('0' + exponentAbsoluteValue - div10 * 10);
                            exponentAbsoluteValue = div10;
                        }

                        var maxExponentLength = kMaxExponentLength - firstCharPos;

                        resultBuilder.EnsureAdditionalCapacity(maxExponentLength + decimalRepLength + 3);

                        resultBuilder.AppendByteUnsafe(decimalRep[0]);
                        resultBuilder.AppendCharDotUnsafe();
                        decimalRep.Slice(1, decimalRepLength - 1).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(decimalRepLength - 1);
                        resultBuilder.AppendCharEUnsafe();
                        resultBuilder.AppendCharPlusUnsafe();
                        buffer.Slice(firstCharPos, maxExponentLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(maxExponentLength);
                    }
                }
                else
                {
                    if (exponent < 0)
                    {
                        var exponentAbsoluteValue = -exponent;
                        Span<byte> buffer = stackalloc byte[kMaxExponentLength + 1];
                        buffer[kMaxExponentLength] = (byte)'\0';
                        var firstCharPos = kMaxExponentLength;
                        while (exponentAbsoluteValue > 0)
                        {
                            var div10 = exponentAbsoluteValue / 10;
                            buffer[--firstCharPos] = (byte)('0' + exponentAbsoluteValue - div10 * 10);
                            exponentAbsoluteValue = div10;
                        }

                        var maxExponentLength = kMaxExponentLength - firstCharPos;

                        resultBuilder.EnsureAdditionalCapacity(maxExponentLength + 3);

                        resultBuilder.AppendByteUnsafe(decimalRep[0]);
                        resultBuilder.AppendCharEUnsafe();
                        resultBuilder.AppendCharMinusUnsafe();
                        buffer.Slice(firstCharPos, maxExponentLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(maxExponentLength);
                    }
                    else if (exponent == 0)
                    {
                        resultBuilder.EnsureAdditionalCapacity(4);

                        resultBuilder.AppendByteUnsafe(decimalRep[0]);
                        resultBuilder.AppendCharEUnsafe();
                        resultBuilder.AppendCharPlusUnsafe();
                        resultBuilder.AppendChar0Unsafe();
                    }
                    else
                    {
                        Span<byte> buffer = stackalloc byte[kMaxExponentLength + 1];
                        buffer[kMaxExponentLength] = (byte)'\0';
                        var firstCharPos = kMaxExponentLength;
                        var exponentAbsoluteValue = exponent;
                        while (exponentAbsoluteValue > 0)
                        {
                            var div10 = exponentAbsoluteValue / 10;
                            buffer[--firstCharPos] = (byte)('0' + exponentAbsoluteValue - div10 * 10);
                            exponentAbsoluteValue = div10;
                        }

                        var maxExponentLength = kMaxExponentLength - firstCharPos;

                        resultBuilder.EnsureAdditionalCapacity(maxExponentLength + 3);

                        resultBuilder.AppendByteUnsafe(decimalRep[0]);
                        resultBuilder.AppendCharEUnsafe();
                        resultBuilder.AppendCharPlusUnsafe();
                        buffer.Slice(firstCharPos, maxExponentLength).CopyTo(resultBuilder.WritableSpan);
                        resultBuilder.Advance(maxExponentLength);
                    }
                }
            }

            return true;
        }


        // modified, return fast_worked.
        private static bool DoubleToAscii(double v, DtoaMode mode, Span<byte> vector, out bool isNegativeSign, out int length, out int point)
        {
            if (new IeeeDouble(v).Sign() < 0)
            {
                isNegativeSign = true;
                v = -v;
            }
            else
            {
                isNegativeSign = false;
            }

            if (v == 0)
            {
                vector[0] = (byte)'0';
                // vector[1] = '\0';
                length = 1;
                point = 1;
                return true;
            }

            switch (mode)
            {
                case DtoaMode.Shortest:
                    return FastDtoa(v, FastDtoaMode.FastDtoaShortest, vector, out length, out point);
                case DtoaMode.ShortestSingle:
                    return FastDtoa(v, FastDtoaMode.FastDtoaShortestSingle, vector, out length, out point);
                default:
                    throw new Exception("Unreachable code.");
            }
        }
    }
}