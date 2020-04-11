// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal.DoubleConversion
{
    // https://github.com/google/double-conversion/blob/master/double-conversion/ieee.h
    internal readonly struct IeeeDouble
    {
        public const ulong KSignMask = 0x8000000000000000;
        public const ulong KExponentMask = 0x7FF0000000000000;
        public const ulong KSignificandMask = 0x000FFFFFFFFFFFFF;
        public const ulong KHiddenBit = 0x0010000000000000;
        public const int KPhysicalSignificandSize = 52;  // Excludes the hidden bit.
        public const int KSignificandSize = 53;

        private const int KExponentBias = 0x3FF + KPhysicalSignificandSize;
        private const int KDenormalExponent = -KExponentBias + 1;
        private const int KMaxExponent = 0x7FF - KExponentBias;
        private const ulong KInfinity = 0x7FF0000000000000;

        private readonly ulong d64;

        public IeeeDouble(double d)
        {
            d64 = new UnionDoubleULong { d = d }.u64;
        }

        public IeeeDouble(DiyFp d)
        {
            d64 = DiyFpToUint64(d);
        }

        // The value encoded by this Double must be greater or equal to +0.0.
        // It must not be special (infinity, or NaN).
        public DiyFp AsDiyFp()
        {
            return new DiyFp(Significand(), Exponent());
        }

        // The value encoded by this Double must be strictly greater than 0.
        public DiyFp AsNormalizedDiyFp()
        {
            var f = Significand();
            var e = Exponent();

            // The current double could be a denormal.
            while ((f & KHiddenBit) == 0)
            {
                f <<= 1;
                e--;
            }

            // Do the final shifts in one go.
            f <<= DiyFp.kSignificandSize - KSignificandSize;
            e -= DiyFp.kSignificandSize - KSignificandSize;
            return new DiyFp(f, e);
        }

        // Returns the next greater double. Returns +infinity on input +infinity.
        public double NextDouble()
        {
            if (d64 == KInfinity)
            {
                return new IeeeDouble(KInfinity).Value();
            }

            if (Sign() < 0 && Significand() == 0)
            {
                // -0.0
                return 0.0;
            }

            return Sign() < 0 ? new IeeeDouble(d64 - 1).Value() : new IeeeDouble(d64 + 1).Value();
        }

        public double PreviousDouble()
        {
            if (d64 == (KInfinity | KSignMask))
            {
                return -Infinity();
            }

            if (Sign() < 0)
            {
                return new IeeeDouble(d64 + 1).Value();
            }

            if (Significand() == 0)
            {
                return -0.0;
            }

            return new IeeeDouble(d64 - 1).Value();
        }

        public int Exponent()
        {
            if (IsDenormal())
            {
                return KDenormalExponent;
            }

            var biasedE = (int)((d64 & KExponentMask) >> KPhysicalSignificandSize);
            return biasedE - KExponentBias;
        }

        public ulong Significand()
        {
            var significand = d64 & KSignificandMask;
            if (!IsDenormal())
            {
                return significand + KHiddenBit;
            }

            return significand;
        }

        // Returns true if the double is a denormal.
        public bool IsDenormal()
        {
            return (d64 & KExponentMask) == 0;
        }

        // We consider denormals not to be special.
        // Hence only Infinity and NaN are special.
        public bool IsSpecial()
        {
            return (d64 & KExponentMask) == KExponentMask;
        }

        public bool IsNan()
        {
            return
                (d64 & KExponentMask) == KExponentMask
                && (d64 & KSignificandMask) != 0;
        }

        public bool IsInfinite()
        {
            return (d64 & KExponentMask) == KExponentMask
                   && (d64 & KSignificandMask) == 0;
        }

        public int Sign()
        {
            return (d64 & KSignMask) == 0 ? 1 : -1;
        }

        // Computes the two boundaries of this.
        // The bigger boundary (m_plus) is normalized. The lower boundary has the same
        // exponent as m_plus.
        // Precondition: the value encoded by this Double must be greater than 0.
        public void NormalizedBoundaries(out DiyFp outMMinus, out DiyFp outMPlus)
        {
            var v = this.AsDiyFp();
            var __ = new DiyFp((v.f << 1) + 1, v.e - 1);
            var mPlus = DiyFp.Normalize(ref __);

            var mMinus = LowerBoundaryIsCloser() ? new DiyFp((v.f << 2) - 1, v.e - 2) : new DiyFp((v.f << 1) - 1, v.e - 1);
            mMinus.f <<= mMinus.e - mPlus.e;
            mMinus.e = mPlus.e;
            outMPlus = mPlus;
            outMMinus = mMinus;
        }

        public bool LowerBoundaryIsCloser()
        {
            // The boundary is closer if the significand is of the form f == 2^p-1 then
            // the lower boundary is closer.
            // Think of v = 1000e10 and v- = 9999e9.
            // Then the boundary (== (v - v-)/2) is not just at a distance of 1e9 but
            // at a distance of 1e8.
            // The only exception is for the smallest normal: the largest denormal is
            // at the same distance as its successor.
            // Note: denormals have the same exponent as the smallest normals.
            var physicalSignificandIsZero = (d64 & KSignificandMask) == 0;
            return physicalSignificandIsZero && Exponent() != KDenormalExponent;
        }

        public double Value()
        {
            return new UnionDoubleULong { u64 = d64 }.d;
        }

        // Returns the significand size for a given order of magnitude.
        // If v = f*2^e with 2^p-1 <= f <= 2^p then p+e is v's order of magnitude.
        // This function returns the number of significant binary digits v will have
        // once it's encoded into a double. In almost all cases this is equal to
        // kSignificandSize. The only exceptions are denormals. They start with
        // leading zeroes and their effective significand-size is hence smaller.
        public static int SignificandSizeForOrderOfMagnitude(int order)
        {
            if (order >= KDenormalExponent + KSignificandSize)
            {
                return KSignificandSize;
            }

            if (order <= KDenormalExponent)
            {
                return 0;
            }

            return order - KDenormalExponent;
        }

        public static double Infinity()
        {
            return new IeeeDouble(KInfinity).Value();
        }

        public static ulong DiyFpToUint64(DiyFp diyFp)
        {
            var significand = diyFp.f;
            var exponent = diyFp.e;
            while (significand > KHiddenBit + KSignificandMask)
            {
                significand >>= 1;
                exponent++;
            }

            if (exponent >= KMaxExponent)
            {
                return KInfinity;
            }

            if (exponent < KDenormalExponent)
            {
                return 0;
            }

            while (exponent > KDenormalExponent && (significand & KHiddenBit) == 0)
            {
                significand <<= 1;
                exponent--;
            }

            ulong biasedExponent;
            if (exponent == KDenormalExponent && (significand & KHiddenBit) == 0)
            {
                biasedExponent = 0;
            }
            else
            {
                biasedExponent = (ulong)(exponent + KExponentBias);
            }

            return (significand & KSignificandMask) | (biasedExponent << KPhysicalSignificandSize);
        }
    }
}