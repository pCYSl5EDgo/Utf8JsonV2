// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public static class DateTimeConverter
    {
        /// <summary>
        /// Write Invariant Culture.
        /// "Month(2)/Day(2)/Year(4) Hour(2):Minute(2):Second(2)"
        /// Total 32 bytes.
        /// </summary>
        public static void Write(ref BufferWriter writer, in DateTime value)
        {
            const int hint = 25;
            var span = writer.GetSpan(hint);

            span[0] = (byte)'"';
            {
                var month = value.Month;
                if (month < 10)
                {
                    span[1] = (byte)'0';
                    span[2] = (byte)('0' + month);
                }
                else
                {
                    span[1] = (byte)'1';
                    span[2] = (byte)('0' - 10 + month);
                }
            }
            span[3] = (byte)'/';
            {
                var day = value.Day;
                if (day < 10)
                {
                    span[4] = (byte)'0';
                    span[5] = (byte)('0' + day);
                }
                else if (day < 20)
                {
                    span[4] = (byte)'1';
                    span[5] = (byte)('0' - 10 + day);
                }
                else if (day < 30)
                {
                    span[4] = (byte)'2';
                    span[5] = (byte)('0' - 20 + day);
                }
                else
                {
                    span[4] = (byte)'3';
                    span[5] = (byte)('0' - 30 + day);
                }
            }
            span[6] = (byte)'/';
            {
                var year = value.Year;
                var thousand = year / 1000;
                span[7] = (byte)('0' + thousand);
                year -= thousand * 1000;
                var hundred = year / 100;
                span[8] = (byte)('0' + hundred);
                year -= hundred * 100;
                var ten = year / 10;
                span[9] = (byte)('0' + ten);
                span[10] = (byte)('0' + year - ten * 10);
            }
            span[11] = (byte)' ';
            {
                var hour = value.Hour;
                if (hour < 10)
                {
                    span[12] = (byte)'0';
                    span[13] = (byte)('0' + hour);
                }
                else if (hour < 20)
                {
                    span[12] = (byte)'1';
                    span[13] = (byte)('0' - 10 + hour);
                }
                else
                {
                    span[12] = (byte)'2';
                    span[13] = (byte)('0' - 20 + hour);
                }
            }
            span[14] = (byte)':';
            {
                var minute = value.Minute;
                var ten = minute / 10;
                span[15] = (byte)('0' + ten);
                span[16] = (byte)('0' + minute - ten * 10);
            }
            span[17] = (byte)':';
            {
                var second = value.Second;
                var ten = second / 10;
                span[18] = (byte)('0' + ten);
                span[19] = (byte)('0' + second - ten * 10);
            }
            span[20] = (byte)'.';
            {
                var millisecond = value.Millisecond;
                var hundred = millisecond / 100;
                span[21] = (byte)('0' + hundred);
                millisecond -= hundred * 100;
                var ten = millisecond / 10;
                span[22] = (byte)('0' + ten);
                span[23] = (byte)('0' + millisecond - ten * 10);
            }
            span[24] = (byte)'"';

            writer.Advance(hint);
        }

        public static bool TryRead(ReadOnlySpan<byte> rawBytes, out DateTime value)
        {
            switch (rawBytes.Length)
            {
                case 19:
                    {
                        var isExact = rawBytes[2] == '/' && rawBytes[5] == '/' && rawBytes[10] == ' ' && rawBytes[13] == ':' && rawBytes[16] == ':';
                        if (isExact)
                        {
                            value = ReadExact19InvariantCulture(rawBytes);
                            return true;
                        }
                        break;
                    }
                case 23:
                    {
                        var isExact = rawBytes[2] == '/' && rawBytes[5] == '/' && rawBytes[10] == ' ' && rawBytes[13] == ':' && rawBytes[16] == ':' && rawBytes[19] == '.';
                        if (isExact)
                        {
                            value = ReadExact23InvariantCulture(rawBytes);
                            return true;
                        }
                        break;
                    }
            }
#if SPAN_BUILTIN
            var text = StringEncoding.Utf8.GetString(rawBytes);
#else
            string text;
            unsafe
            {
                fixed (byte* ptr = &rawBytes[0])
                {
                    text = StringEncoding.Utf8.GetString(ptr, rawBytes.Length);
                }
            }
#endif
            return DateTime.TryParse(text, out value);
        }

        private static DateTime ReadExact19InvariantCulture(in ReadOnlySpan<byte> rawBytes)
        {
            // '0' is 48
            var month = rawBytes[0] * 10 + rawBytes[1] - '0' * 11;
            var day = rawBytes[3] * 10 + rawBytes[4] - '0' * 11;
            var year = rawBytes[6] * 1000 + rawBytes[7] * 100 + rawBytes[8] * 10 + rawBytes[9] - '0' * 1111;
            var hour = rawBytes[11] * 10 + rawBytes[12] - '0' * 11;
            var minute = rawBytes[14] * 10 + rawBytes[15] - '0' * 11;
            var second = rawBytes[17] * 10 + rawBytes[18] - '0' * 11;
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }

        private static DateTime ReadExact23InvariantCulture(in ReadOnlySpan<byte> rawBytes)
        {
            // '0' is 48
            var month = rawBytes[0] * 10 + rawBytes[1] - '0' * 11;
            var day = rawBytes[3] * 10 + rawBytes[4] - '0' * 11;
            var year = rawBytes[6] * 1000 + rawBytes[7] * 100 + rawBytes[8] * 10 + rawBytes[9] - '0' * 1111;
            var hour = rawBytes[11] * 10 + rawBytes[12] - '0' * 11;
            var minute = rawBytes[14] * 10 + rawBytes[15] - '0' * 11;
            var second = rawBytes[17] * 10 + rawBytes[18] - '0' * 11;
            var millisecond = rawBytes[20] * 100 + rawBytes[21] * 10 + rawBytes[22] - '0' * 111;
            return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
        }
    }
}