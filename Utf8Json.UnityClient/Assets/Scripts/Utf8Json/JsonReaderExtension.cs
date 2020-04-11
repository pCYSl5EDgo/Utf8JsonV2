// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Utf8Json.Internal;
using Utf8Json.Internal.DoubleConversion;

namespace Utf8Json
{
    public static class JsonReaderExtension
    {
        private const string ExpectedFirst = "expected:'";
        private const string ExpectedLast = "'";

        public static string ReadPropertyName(ref this JsonReader reader)
        {
            var name = reader.ReadString();
            if (name == null)
            {
                throw new JsonParsingException("Property name should not be null.");
            }

            reader.ReadIsNameSeparatorWithVerify();
            return name;
        }

#if CSHARP_8_OR_NEWER
        public static string? ReadString(ref this JsonReader reader)
#else
        public static string ReadString(ref this JsonReader reader)
#endif
        {
            return reader.ReadIsNull() ? default : StringHelper.Decode(reader.ReadNotNullStringSegmentRaw());
        }

        private static readonly byte[] trueBytes = { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
        private static readonly byte[] falseBytes = { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();

            if (reader.AdvanceTrue()) return true;

            if (reader.AdvanceFalse()) return false;

            throw new JsonParsingException(ExpectedFirst + "true | false" + ExpectedLast);
        }

        /// <summary>
        /// Advance false. After SkipWhiteSpace().
        /// </summary>
        /// <returns>Is false?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AdvanceFalse(ref this JsonReader reader)
        {
            return reader.Reader.IsNext(falseBytes, true);
        }

        /// <summary>
        /// Advance true. After SkipWhiteSpace().
        /// </summary>
        /// <returns>Is true?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AdvanceTrue(ref this JsonReader reader)
        {
            return reader.Reader.IsNext(trueBytes, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref this JsonReader reader)
        {
            return checked((sbyte)reader.ReadInt32());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref this JsonReader reader)
        {
            return checked((short)reader.ReadInt32());
        }

        public static int ReadInt32(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();

            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> span, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0 || rest > 11)
                {
                    goto ERROR;
                }

                span = reader.Reader.UnreadSpan;
                if (span.Length != rest)
                {
                    return reader.WithPoolReadInt32(rest);
                }
            }

            if (span.IsEmpty || span.Length > (span[0] == '-' ? 11 : 10))
            {
                goto ERROR;
            }

            if (IntegerConverter.TryRead(span, out int answer))
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static int WithPoolReadInt32(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                reader.Advance(rest);
                if (IntegerConverter.TryRead(span, out int answer))
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static long ReadInt64(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> span, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0 || rest > 21)
                {
                    goto ERROR;
                }

                span = reader.Reader.UnreadSpan;
                if (span.Length != rest)
                {
                    return reader.WithPoolReadInt64(rest);
                }
            }

            if (span.IsEmpty || span.Length > (span[0] == '-' ? 21 : 20))
            {
                goto ERROR;
            }

            if (IntegerConverter.TryRead(span, out long answer))
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static long WithPoolReadInt64(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                reader.Advance(rest);
                if (IntegerConverter.TryRead(span, out long answer))
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref this JsonReader reader)
        {
            return checked((byte)reader.ReadUInt32());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref this JsonReader reader)
        {
            return checked((ushort)reader.ReadUInt32());
        }

        public static uint ReadUInt32(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> span, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0 || rest > 10)
                {
                    goto ERROR;
                }

                span = reader.Reader.UnreadSpan;
                if (span.Length != rest)
                {
                    return reader.WithPoolReadUInt32(rest);
                }
            }

            if (span.IsEmpty || span.Length > 10)
            {
                goto ERROR;
            }

            if (IntegerConverter.TryRead(span, out uint answer))
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static uint WithPoolReadUInt32(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                reader.Advance(rest);
                if (IntegerConverter.TryRead(span, out uint answer))
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static ulong ReadUInt64(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> span, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0 || rest > 20)
                {
                    goto ERROR;
                }

                span = reader.Reader.UnreadSpan;
                if (span.Length != rest)
                {
                    return reader.WithPoolReadUInt64(rest);
                }
            }

            if (span.IsEmpty || span.Length > 20)
            {
                goto ERROR;
            }

            if (IntegerConverter.TryRead(span, out ulong answer))
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static ulong WithPoolReadUInt64(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                reader.Advance(rest);
                if (IntegerConverter.TryRead(span, out ulong answer))
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static float ReadSingle(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> numberBytes, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0)
                {
                    goto ERROR;
                }

                numberBytes = reader.Reader.UnreadSpan;
                if (numberBytes.Length != rest)
                {
                    return reader.WithPoolReadSingle(rest);
                }
            }

            var answer = StringToDoubleConverter.ToSingle(numberBytes, out var readCount);

            if (readCount == numberBytes.Length)
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static float WithPoolReadSingle(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                var answer = StringToDoubleConverter.ToSingle(span, out var readCount);
                reader.Advance(rest);
                if (readCount == rest)
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static double ReadDouble(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> numberBytes, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0)
                {
                    goto ERROR;
                }

                numberBytes = reader.Reader.UnreadSpan;
                if (numberBytes.Length != rest)
                {
                    return reader.WithPoolReadDouble(rest);
                }
            }

            var answer = StringToDoubleConverter.ToDouble(numberBytes, out var readCount);

            if (readCount == numberBytes.Length)
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static double WithPoolReadDouble(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                var answer = StringToDoubleConverter.ToDouble(span, out var readCount);
                reader.Advance(rest);
                if (readCount == rest)
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static decimal ReadDecimal(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            if (!reader.Reader.TryReadToAny(out ReadOnlySpan<byte> numberBytes, JsonReader.NumberBreaks))
            {
                var rest = (int)reader.Reader.Remaining;
                if (rest == 0)
                {
                    goto ERROR;
                }

                numberBytes = reader.Reader.UnreadSpan;
                if (numberBytes.Length != rest)
                {
                    return reader.WithPoolReadDecimal(rest);
                }
            }

            var answer = StringToDoubleConverter.ToDecimal(numberBytes, out var readCount);

            if (readCount == numberBytes.Length)
            {
                return answer;
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        private static decimal WithPoolReadDecimal(ref this JsonReader reader, int rest)
        {
            var array = ArrayPool<byte>.Shared.Rent(rest);
            try
            {
                var span = array.AsSpan(0, rest);
                if (!reader.Reader.TryCopyTo(span))
                {
                    goto ERROR;
                }

                var answer = StringToDoubleConverter.ToDecimal(span, out var readCount);
                reader.Advance(rest);
                if (readCount == rest)
                {
                    return answer;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

        ERROR:
            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static DateTime ReadDateTime(ref this JsonReader reader)
        {
            if (reader.ReadIsNull())
            {
                throw new JsonParsingException("DateTime cannot be null.");
            }

            var text = reader.ReadNotNullStringSegmentRaw();
            if (text.IsEmpty)
            {
                return default;
            }

            if (DateTimeConverter.TryRead(text, out var answer))
            {
                return answer;
            }
#if SPAN_BUILTIN
            throw new JsonParsingException("Invalid DateTime. text + \"" + StringEncoding.Utf8.GetString(text) + "\"");
#else
            throw new JsonParsingException("Invalid DateTime.");
#endif
        }
    }
}