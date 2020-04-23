// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

// ReSharper disable RedundantExplicitArraySize

namespace Utf8Json.Internal.DoubleConversion
{
#pragma warning restore 661
#pragma warning restore 660

    // C# API
    internal static class StringToDoubleConverter
    {
        public static double ToDouble(in ReadOnlySpan<byte> span, out int readCount)
        {
            Iterator input = new Iterator(span);
            var current = input;
            var end = input + span.Length;

            readCount = 0;

            // bool allow_case_insensibility = (flags_ & Flags.ALLOW_CASE_INSENSIBILITY) != 0;

            // To make sure that iterator dereferencing is valid the following
            // convention is used:
            // 1. Each '++current' statement is followed by check for equality to 'end'.
            // 2. If AdvanceToNonSpace returned false then current == end.
            // 3. If 'current' becomes equal to 'end' the function returns or goes to
            // 'parsing_done'.
            // 4. 'current' is not dereferenced after the 'parsing_done' label.
            // 5. Code before 'parsing_done' may rely on 'current != end'.

            if (!AdvanceToNonSpace(ref current, end))
            {
                readCount = current - input;
                return default;
            }
            if (input != current)
            {
                // No leading spaces allowed, but AdvanceToNonSpace moved forward.
                return double.NaN;
            }

            // The longest form of simplified number is: "-<significant digits>.1eXXX\0".
            Span<byte> buffer = stackalloc byte[KBufferSize];  // NOLINT: size is known at compile time.
            var bufferPos = 0;

            // Exponent will be adjusted if insignificant digits of the integer part
            // or insignificant leading zeros of the fractional part are dropped.
            var exponent = 0;
            var significantDigits = 0;
            var insignificantDigits = 0;
            var nonzeroDigitDropped = false;

            var sign = false;

            if (current == '+' || current == '-')
            {
                sign = current == '-';
                current += 1;
                var nextNonSpace = current;
                // Skip following spaces (if allowed).
                if (!AdvanceToNonSpace(ref nextNonSpace, end)) return double.NaN;
                current = nextNonSpace;
            }

            ReadOnlySpan<byte> infinitySymbol = new byte[] { 0x49, 0x6E, 0x66, 0x69, 0x6E, 0x69, 0x74, 0x79, };
            if (current.Value == infinitySymbol[0])
            {
                if (!ConsumeSubString(ref current, end, infinitySymbol))
                {
                    return double.NaN;
                }

                readCount = current - input;
                return sign ? double.NegativeInfinity : double.PositiveInfinity;
            }

            // StringEncoding.UTF8.GetBytes(double.NaN.ToString(CultureInfo.InvariantCulture));
            ReadOnlySpan<byte> nanSymbol = new byte[] { 0x4E, 0x61, 0x4E, };
            if (current.Value == nanSymbol[0])
            {
                if (!ConsumeSubString(ref current, end, nanSymbol))
                {
                    return double.NaN;
                }

                readCount = current - input;
                return sign ? -double.NaN : double.NaN;
            }

            var leadingZero = false;
            if (current == '0')
            {
                current += 1;
                if (current == end)
                {
                    readCount = current - input;
                    return sign ? -0.0 : 0.0;
                }

                leadingZero = true;

                // Ignore leading zeros in the integer part.
                while (current == '0')
                {
                    current += 1;
                    if (current != end)
                    {
                        continue;
                    }

                    readCount = current - input;
                    return sign ? -0.0 : 0.0;
                }
            }

            // Copy significant digits of the integer part (if any) to the buffer.
            while (current >= '0' && current <= '9')
            {
                if (significantDigits < KMaxSignificantDigits)
                {
                    buffer[bufferPos++] = current.Value;
                    significantDigits++;
                    // Will later check if it's an octal in the buffer.
                }
                else
                {
                    insignificantDigits++;  // Move the digit into the exponential part.
                    nonzeroDigitDropped = nonzeroDigitDropped || current != '0';
                }

                current += 1;
                if (current == end) goto parsing_done;
            }

            if (current == '.')
            {
                current += 1;
                if (current == end)
                {
                    if (significantDigits == 0 && !leadingZero)
                    {
                        return double.NaN;
                    }

                    goto parsing_done;
                }

                if (significantDigits == 0)
                {
                    // octal = false;
                    // Integer part consists of 0 or is absent. Significant digits start after
                    // leading zeros (if any).
                    while (current == '0')
                    {
                        current += 1;
                        if (current == end)
                        {
                            readCount = current - input;
                            return sign ? -0.0 : 0.0;
                        }
                        exponent--;  // Move this 0 into the exponent.
                    }
                }

                // There is a fractional part.
                // We don't emit a '.', but adjust the exponent instead.
                while (current >= '0' && current <= '9')
                {
                    if (significantDigits < KMaxSignificantDigits)
                    {
                        buffer[bufferPos++] = current.Value;
                        significantDigits++;
                        exponent--;
                    }
                    else
                    {
                        // Ignore insignificant digits in the fractional part.
                        nonzeroDigitDropped = nonzeroDigitDropped || current != '0';
                    }

                    current += 1;
                    if (current == end) goto parsing_done;
                }
            }

            if (!leadingZero && exponent == 0 && significantDigits == 0)
            {
                // If leading_zeros is true then the string contains zeros.
                // If exponent < 0 then string was [+-]\.0*...
                // If significant_digits != 0 the string is not equal to 0.
                // Otherwise there are no digits in the string.
                return double.NaN;
            }

            // Parse exponential part.
            if (current == 'e' || current == 'E')
            {
                current += 1;
                if (current == end)
                {
                    goto parsing_done;
                }
                var exponentSign = (byte)'+';
                if (current == '+' || current == '-')
                {
                    exponentSign = current.Value;
                    current += 1;
                    if (current == end)
                    {
                        goto parsing_done;
                    }
                }

                if (current == end || current < '0' || current > '9')
                {
                    goto parsing_done;
                }

                const int maxExponent = int.MaxValue / 2;

                var num = 0;
                do
                {
                    // Check overflow.
                    var digit = current.Value - (byte)'0';
                    const int maxExponentDiv10 = maxExponent / 10;
                    const int maxExponentMod10 = maxExponent % 10;
                    if (num >= maxExponentDiv10
                        && !(num == maxExponentDiv10 && digit <= maxExponentMod10))
                    {
                        num = maxExponent;
                    }
                    else
                    {
                        num = num * 10 + digit;
                    }

                    current += 1;
                } while (current != end && current >= '0' && current <= '9');

                exponent += exponentSign == '-' ? -num : num;
            }

            AdvanceToNonSpace(ref current, end);

        parsing_done:
            exponent += insignificantDigits;

            if (nonzeroDigitDropped)
            {
                buffer[bufferPos++] = (byte)'1';
                exponent--;
            }

            buffer[bufferPos] = (byte)'\0';

            var success = StringToDouble.TryParseToDouble(buffer.Slice(0, bufferPos), exponent, out var converted);
            if (success)
            {
                readCount = current - input;
                return sign ? -converted : converted;
            }

            // read-again
            readCount = current - input;

            Span<byte> fallbackBuffer = stackalloc byte[readCount];

            var fallbackI = 0;
            while (input != current)
            {
                fallbackBuffer[fallbackI++] = input.Value;
                input += 1;
            }

#if SPAN_BUILTIN
            var lastStr = StringEncoding.Utf8.GetString(fallbackBuffer.Slice(0, fallbackI));
#else
            string lastStr;
            unsafe
            {
                fixed (byte* src = &fallbackBuffer[0])
                {
                    lastStr = StringEncoding.Utf8.GetString(src, fallbackI);
                }
            }
#endif
            return double.Parse(lastStr);
        }

        public static float ToSingle(in ReadOnlySpan<byte> span, out int readCount)
        {
            Iterator input = new Iterator(span);
            var current = input;
            var end = input + span.Length;

            readCount = 0;

            // bool allow_case_insensibility = (flags_ & Flags.ALLOW_CASE_INSENSIBILITY) != 0;

            // To make sure that iterator dereferencing is valid the following
            // convention is used:
            // 1. Each '++current' statement is followed by check for equality to 'end'.
            // 2. If AdvanceToNonSpace returned false then current == end.
            // 3. If 'current' becomes equal to 'end' the function returns or goes to
            // 'parsing_done'.
            // 4. 'current' is not dereferenced after the 'parsing_done' label.
            // 5. Code before 'parsing_done' may rely on 'current != end'.

            if (!AdvanceToNonSpace(ref current, end))
            {
                readCount = current - input;
                return default;
            }
            if (input != current)
            {
                // No leading spaces allowed, but AdvanceToNonSpace moved forward.
                return float.NaN;
            }

            // The longest form of simplified number is: "-<significant digits>.1eXXX\0".
            Span<byte> buffer = stackalloc byte[KBufferSize];  // NOLINT: size is known at compile time.
            var bufferPos = 0;

            // Exponent will be adjusted if insignificant digits of the integer part
            // or insignificant leading zeros of the fractional part are dropped.
            var exponent = 0;
            var significantDigits = 0;
            var insignificantDigits = 0;
            var nonzeroDigitDropped = false;

            var sign = false;

            if (current == '+' || current == '-')
            {
                sign = current == '-';
                current += 1;
                var nextNonSpace = current;
                // Skip following spaces (if allowed).
                if (!AdvanceToNonSpace(ref nextNonSpace, end)) return float.NaN;
                current = nextNonSpace;
            }

            // StringEncoding.UTF8.GetBytes(double.PositiveInfinity.ToString(CultureInfo.InvariantCulture));
            ReadOnlySpan<byte> infinitySymbol = new byte[] { 0x49, 0x6E, 0x66, 0x69, 0x6E, 0x69, 0x74, 0x79, };
            if (current.Value == infinitySymbol[0])
            {
                if (!ConsumeSubString(ref current, end, infinitySymbol))
                {
                    return float.NaN;
                }

                readCount = current - input;
                return sign ? float.NegativeInfinity : float.PositiveInfinity;
            }

            // StringEncoding.UTF8.GetBytes(double.NaN.ToString(CultureInfo.InvariantCulture));
            ReadOnlySpan<byte> nanSymbol = new byte[] { 0x4E, 0x61, 0x4E, };
            if (current.Value == nanSymbol[0])
            {
                if (!ConsumeSubString(ref current, end, nanSymbol))
                {
                    return float.NaN;
                }

                readCount = current - input;
                return sign ? -float.NaN : float.NaN;
            }

            var leadingZero = false;
            if (current == '0')
            {
                current += 1;
                if (current == end)
                {
                    readCount = current - input;
                    return sign ? -0f : 0f;
                }

                leadingZero = true;

                // Ignore leading zeros in the integer part.
                while (current == '0')
                {
                    current += 1;
                    if (current != end)
                    {
                        continue;
                    }

                    readCount = current - input;
                    return sign ? -0f : default;
                }
            }

            // Copy significant digits of the integer part (if any) to the buffer.
            while (current >= '0' && current <= '9')
            {
                if (significantDigits < KMaxSignificantDigits)
                {
                    buffer[bufferPos++] = current.Value;
                    significantDigits++;
                    // Will later check if it's an octal in the buffer.
                }
                else
                {
                    insignificantDigits++;  // Move the digit into the exponential part.
                    nonzeroDigitDropped = nonzeroDigitDropped || current != '0';
                }

                current += 1;
                if (current == end) goto parsing_done;
            }

            if (current == '.')
            {
                current += 1;
                if (current == end)
                {
                    if (significantDigits == 0 && !leadingZero)
                    {
                        return float.NaN;
                    }

                    goto parsing_done;
                }

                if (significantDigits == 0)
                {
                    // octal = false;
                    // Integer part consists of 0 or is absent. Significant digits start after
                    // leading zeros (if any).
                    while (current == '0')
                    {
                        current += 1;
                        if (current == end)
                        {
                            readCount = current - input;
                            return sign ? -0f : default;
                        }
                        exponent--;  // Move this 0 into the exponent.
                    }
                }

                // There is a fractional part.
                // We don't emit a '.', but adjust the exponent instead.
                while (current >= '0' && current <= '9')
                {
                    if (significantDigits < KMaxSignificantDigits)
                    {
                        buffer[bufferPos++] = current.Value;
                        significantDigits++;
                        exponent--;
                    }
                    else
                    {
                        // Ignore insignificant digits in the fractional part.
                        nonzeroDigitDropped = nonzeroDigitDropped || current != '0';
                    }

                    current += 1;
                    if (current == end) goto parsing_done;
                }
            }

            if (!leadingZero && exponent == 0 && significantDigits == 0)
            {
                // If leading_zeros is true then the string contains zeros.
                // If exponent < 0 then string was [+-]\.0*...
                // If significant_digits != 0 the string is not equal to 0.
                // Otherwise there are no digits in the string.
                return float.NaN;
            }

            // Parse exponential part.
            if (current == 'e' || current == 'E')
            {
                current += 1;
                if (current == end)
                {
                    goto parsing_done;
                }
                var exponentSign = (byte)'+';
                if (current == '+' || current == '-')
                {
                    exponentSign = current.Value;
                    current += 1;
                    if (current == end)
                    {
                        goto parsing_done;
                    }
                }

                if (current == end || current < '0' || current > '9')
                {
                    goto parsing_done;
                }

                const int maxExponent = int.MaxValue / 2;

                var num = 0;
                do
                {
                    // Check overflow.
                    var digit = current.Value - (byte)'0';
                    const int maxExponentDiv10 = maxExponent / 10;
                    const int maxExponentMod10 = maxExponent % 10;
                    if (num >= maxExponentDiv10
                        && !(num == maxExponentDiv10 && digit <= maxExponentMod10))
                    {
                        num = maxExponent;
                    }
                    else
                    {
                        num = num * 10 + digit;
                    }

                    current += 1;
                } while (current != end && current >= '0' && current <= '9');

                exponent += exponentSign == '-' ? -num : num;
            }

            AdvanceToNonSpace(ref current, end);

        parsing_done:
            exponent += insignificantDigits;

            if (nonzeroDigitDropped)
            {
                buffer[bufferPos++] = (byte)'1';
                exponent--;
            }

            buffer[bufferPos] = (byte)'\0';

            var success = StringToDouble.TryParseToFloat(buffer.Slice(0, bufferPos), exponent, out var converted);
            if (success)
            {
                readCount = current - input;
                return sign ? -converted : converted;
            }

            // read-again
            readCount = current - input;

            Span<byte> fallbackBuffer = stackalloc byte[readCount];

            var fallbackI = 0;
            while (input != current)
            {
                fallbackBuffer[fallbackI++] = input.Value;
                input += 1;
            }

#if SPAN_BUILTIN
            var lastStr = StringEncoding.Utf8.GetString(fallbackBuffer.Slice(0, fallbackI));
#else
            string lastStr;
            unsafe
            {
                fixed (byte* src = &fallbackBuffer[0])
                {
                    lastStr = StringEncoding.Utf8.GetString(src, fallbackI);
                }
            }
#endif
            return float.Parse(lastStr);
        }

        private const int KMaxSignificantDigits = 772;
        private const int KBufferSize = KMaxSignificantDigits + 10;

        private const int KWhitespaceTable7Length = 6;

        private const int KWhitespaceTable16Length = 20;
        private static readonly ushort[] kWhitespaceTable16 = new ushort[KWhitespaceTable16Length]
        {
            160,  8232, 8233, 5760, 6158, 8192, 8193, 8194, 8195,
            8196, 8197, 8198, 8199, 8200, 8201, 8202, 8239, 8287, 12288, 65279
        };


        private static bool IsWhitespace(int x)
        {
            if (x < 128)
            {
                ReadOnlySpan<byte> kWhitespaceTable7 = new byte[KWhitespaceTable7Length] { 32, 13, 10, 9, 11, 12 };
                for (var i = 0; i < KWhitespaceTable7Length; i++)
                {
                    if (kWhitespaceTable7[i] == x)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (var i = 0; i < KWhitespaceTable16Length; i++)
                {
                    if (kWhitespaceTable16[i] == x)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool AdvanceToNonSpace(ref Iterator current, in Iterator end)
        {
            while (current != end)
            {
                if (!IsWhitespace(current.Value))
                {
                    return true;
                }

                current += 1;
            }

            return false;
        }

        private static bool ConsumeSubString(ref Iterator current, in Iterator end, ReadOnlySpan<byte> substring)
        {
            for (var i = 1; i < substring.Length; i++)
            {
                current += 1;
                if (current == end || current != substring[i])
                {
                    return false;
                }
            }

            current += 1;
            return true;
        }

        public static decimal ToDecimal(in ReadOnlySpan<byte> span, out int readCount)
        {
#if SPAN_BUILTIN
            var text = StringEncoding.Utf8.GetString(span);
#else
            string text;
            unsafe
            {
                fixed (byte* src = &span[0])
                {
                    text = StringEncoding.Utf8.GetString(src, span.Length);
                }
            }
#endif
            readCount = decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var answer) ? span.Length : default;

            return answer;
        }
    }
}
