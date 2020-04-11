// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
            return reader.Reader.IsNextAdvancePast(falseBytes);
        }

        /// <summary>
        /// Advance true. After SkipWhiteSpace().
        /// </summary>
        /// <returns>Is true?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AdvanceTrue(ref this JsonReader reader)
        {
            return reader.Reader.IsNextAdvancePast(trueBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(ref this JsonReader reader)
        {
            return checked((sbyte)reader.ReadInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ref this JsonReader reader)
        {
            return checked((short)reader.ReadInt64());
        }

        public static int ReadInt32(ref this JsonReader reader)
        {
            return checked((int)reader.ReadInt64());
        }

        public static long ReadInt64(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(ref this JsonReader reader)
        {
            return checked((byte)reader.ReadUInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ref this JsonReader reader)
        {
            return checked((ushort)reader.ReadUInt64());
        }

        public static uint ReadUInt32(ref this JsonReader reader)
        {
            return checked((uint)reader.ReadUInt64());
        }

        public static ulong ReadUInt64(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);

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

        public static float ReadSingle(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);
            var answer = StringToDoubleConverter.ToSingle(span, out var readCount);

            if (readCount == span.Length)
            {
                return answer;
            }

            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static double ReadDouble(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);
            var answer = StringToDoubleConverter.ToDouble(span, out var readCount);

            if (readCount == span.Length)
            {
                return answer;
            }

            throw new JsonParsingException(ExpectedFirst + "Number Token" + ExpectedLast);
        }

        public static decimal ReadDecimal(ref this JsonReader reader)
        {
            reader.SkipWhiteSpace();
            var span = reader.Reader.ReadToAnyOfDelimitersOrEndDoNotPastDelimiter(JsonReader.NumberBreaks);
            var answer = StringToDoubleConverter.ToDecimal(span, out var readCount);

            if (readCount == span.Length)
            {
                return answer;
            }

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