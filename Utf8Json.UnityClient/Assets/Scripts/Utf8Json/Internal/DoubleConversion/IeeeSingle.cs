// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal.DoubleConversion
{
    internal readonly struct IeeeSingle
    {
        private const int KExponentBias = 0x7F + KPhysicalSignificandSize;
        private const int KDenormalExponent = -KExponentBias + 1;

        public const uint KExponentMask = 0x7F800000;
        public const uint KSignificandMask = 0x007FFFFF;
        public const uint KHiddenBit = 0x00800000;
        public const int KPhysicalSignificandSize = 23;  // Excludes the hidden bit.

        private readonly uint d32;

        public IeeeSingle(float f)
        {
            this.d32 = new UnionFloatUInt { f = f }.u32;
        }

        // The value encoded by this Single must be greater or equal to +0.0.
        // It must not be special (infinity, or NaN).
        public DiyFp AsDiyFp()
        {
            return new DiyFp(Significand(), Exponent());
        }

        public int Exponent()
        {
            if (IsDenormal())
            {
                return KDenormalExponent;
            }

            var biasedE = (int)((d32 & KExponentMask) >> KPhysicalSignificandSize);
            return biasedE - KExponentBias;
        }

        public uint Significand()
        {
            var significand = d32 & KSignificandMask;
            if (!IsDenormal())
            {
                return significand + KHiddenBit;
            }

            return significand;
        }

        // Returns true if the single is a denormal.
        public bool IsDenormal()
        {
            return (d32 & KExponentMask) == 0;
        }

        // Computes the two boundaries of this.
        // The bigger boundary (m_plus) is normalized. The lower boundary has the same
        // exponent as m_plus.
        // Precondition: the value encoded by this Single must be greater than 0.
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
            var physicalSignificandIsZero = (d32 & KSignificandMask) == 0;
            return physicalSignificandIsZero && Exponent() != KDenormalExponent;
        }
    }
}