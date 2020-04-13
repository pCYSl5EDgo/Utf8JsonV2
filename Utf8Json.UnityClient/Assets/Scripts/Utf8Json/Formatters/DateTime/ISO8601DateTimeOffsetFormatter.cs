// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;
// ReSharper disable UseIndexFromEndExpression

namespace Utf8Json.Formatters
{
    // ReSharper disable once InconsistentNaming
    public sealed class ISO8601DateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        public void Serialize(ref JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            var year = value.Year;
            var month = value.Month;
            var day = value.Day;
            var hour = value.Hour;
            var minute = value.Minute;
            var second = value.Second;
            var nanoSec = value.Ticks % TimeSpan.TicksPerSecond;

            const int baseLength = 19 + 2; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation
            const int nanoSecLength = 8; // .{nanoseconds}

            // +{Hour}:{Minute}
            var length = baseLength + (nanoSec == 0 ? 0 : nanoSecLength) + 6;
            var localOffset = value.Offset;
            var minus = localOffset < TimeSpan.Zero;
            if (minus) localOffset = localOffset.Negate();
            var h = localOffset.Hours;
            var m = localOffset.Minutes;

            var span = writer.Writer.GetSpan(length);

            span[0] = (byte)'\"';
            if (year < 10)
            {
                span[1] = (byte)'0';
                span[2] = (byte)'0';
                span[3] = (byte)'0';
                span[4] = (byte)('0' + year);
            }
            else if (year < 100)
            {
                span[1] = (byte)'0';
                span[2] = (byte)'0';
                var t = year / 10;
                span[3] = (byte)('0' + t);
                year -= t * 10;
                span[4] = (byte)('0' + year);
            }
            else if (year < 1000)
            {
                span[1] = (byte)'0';
                var t = year / 100;
                span[2] = (byte)('0' + t);
                year -= t * 100;
                t = year / 10;
                span[3] = (byte)('0' + t);
                year -= t * 10;
                span[4] = (byte)('0' + year);
            }
            else
            {
                var t = year / 1000;
                span[1] = (byte)('0' + t);
                year -= t * 1000;
                t = year / 100;
                span[2] = (byte)('0' + t);
                year -= t * 100;
                t = year / 10;
                span[3] = (byte)('0' + t);
                year -= t * 10;
                span[4] = (byte)('0' + year);
            }

            span[5] = (byte)'-';

            if (month < 10)
            {
                span[6] = (byte)'0';
                span[7] = (byte)('0' + month);
            }
            else
            {
                span[6] = (byte)'1';
                span[7] = (byte)('0' - 10 + month);
            }
            span[8] = (byte)'-';

            if (day < 10)
            {
                span[9] = (byte)'0';
                span[10] = (byte)('0' + day);
            }
            else
            {
                var t = day / 10;
                span[9] = (byte)('0' + t);
                span[10] = (byte)('0' + day - 10 * t);
            }
            span[11] = (byte)'T';

            if (hour < 10)
            {
                span[12] = (byte)'0';
                span[13] = (byte)('0' + hour);
            }
            else
            {
                var t = hour / 10;
                span[12] = (byte)('0' + t);
                span[13] = (byte)('0' + hour - 10 * t);
            }
            span[14] = (byte)':';

            if (minute < 10)
            {
                span[15] = (byte)'0';
                span[16] = (byte)('0' + minute);
            }
            else
            {
                var t = minute / 10;
                span[15] = (byte)('0' + t);
                span[16] = (byte)('0' + minute - 10 * t);
            }
            span[17] = (byte)':';

            if (second < 10)
            {
                span[15] = (byte)'0';
                span[16] = (byte)('0' + second);
            }
            else
            {
                var t = second / 10;
                span[15] = (byte)('0' + t);
                span[16] = (byte)('0' + second - 10 * t);
            }

            if (nanoSec != 0)
            {
                span[17] = (byte)'.';

                if (nanoSec < 10)
                {
                    span[18] = (byte)'0';
                    span[19] = (byte)'0';
                    span[20] = (byte)'0';
                    span[21] = (byte)'0';
                    span[22] = (byte)'0';
                    span[23] = (byte)'0';
                    span[24] = (byte)('0' + nanoSec);
                }
                else if (nanoSec < 100)
                {
                    span[18] = (byte)'0';
                    span[19] = (byte)'0';
                    span[20] = (byte)'0';
                    span[21] = (byte)'0';
                    span[22] = (byte)'0';
                    var t = nanoSec / 10;
                    span[23] = (byte)('0' + t);
                    span[24] = (byte)('0' + nanoSec - 10 * t);
                }
                else if (nanoSec < 1000)
                {
                    span[18] = (byte)'0';
                    span[19] = (byte)'0';
                    span[20] = (byte)'0';
                    span[21] = (byte)'0';
                    var t = nanoSec / 100;
                    span[22] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[23] = (byte)('0' + t);
                    span[24] = (byte)('0' + nanoSec - 10 * t);
                }
                else if (nanoSec < 10000)
                {
                    span[18] = (byte)'0';
                    span[19] = (byte)'0';
                    span[20] = (byte)'0';
                    var t = nanoSec / 1000;
                    span[21] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[22] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[23] = (byte)('0' + t);
                    span[24] = (byte)('0' + nanoSec - 10 * t);
                }
                else if (nanoSec < 100000)
                {
                    span[18] = (byte)'0';
                    span[19] = (byte)'0';
                    var t = nanoSec / 10000;
                    span[20] = (byte)('0' + t);
                    nanoSec -= t * 10000;
                    t = nanoSec / 1000;
                    span[21] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[22] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[23] = (byte)('0' + t);
                    span[24] = (byte)('0' + nanoSec - 10 * t);
                }
                else if (nanoSec < 1000000)
                {
                    span[18] = (byte)'0';
                    var t = nanoSec / 100000;
                    span[19] = (byte)('0' + t);
                    nanoSec -= t * 100000;
                    t = nanoSec / 10000;
                    span[20] = (byte)('0' + t);
                    nanoSec -= t * 10000;
                    t = nanoSec / 1000;
                    span[21] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[22] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[23] = (byte)('0' + t);
                    span[24] = (byte)('0' + nanoSec - 10 * t);
                }
                else
                {
                    var t = nanoSec / 1000000;
                    span[18] = (byte)('0' + t);
                    nanoSec -= t * 1000000;
                    t = nanoSec / 100000;
                    span[19] = (byte)('0' + t);
                    nanoSec -= t * 100000;
                    t = nanoSec / 10000;
                    span[20] = (byte)('0' + t);
                    nanoSec -= t * 10000;
                    t = nanoSec / 1000;
                    span[21] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[22] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[23] = (byte)('0' + t);
                    span[24] = (byte)('0' + nanoSec - 10 * t);
                }

                span[25] = minus ? (byte)'-' : (byte)'+';
                if (h < 10)
                {
                    span[26] = (byte)'0';
                    span[27] = (byte)('0' + h);
                }
                else
                {
                    var t = h / 10;
                    span[26] = (byte)('0' + t);
                    span[27] = (byte)('0' + h - 10 * t);
                }
                span[28] = (byte)':';
                if (m < 10)
                {
                    span[29] = (byte)'0';
                    span[30] = (byte)('0' + m);
                }
                else
                {
                    var t = m / 10;
                    span[29] = (byte)('0' + t);
                    span[30] = (byte)('0' + m - 10 * t);
                }

                span[31] = (byte)'\"';
            }
            else
            {
                span[17] = minus ? (byte)'-' : (byte)'+';
                if (h < 10)
                {
                    span[18] = (byte)'0';
                    span[19] = (byte)('0' + h);
                }
                else
                {
                    var t = h / 10;
                    span[18] = (byte)('0' + t);
                    span[19] = (byte)('0' + h - 10 * t);
                }
                span[20] = (byte)':';
                if (m < 10)
                {
                    span[21] = (byte)'0';
                    span[22] = (byte)('0' + m);
                }
                else
                {
                    var t = m / 10;
                    span[21] = (byte)('0' + t);
                    span[22] = (byte)('0' + m - 10 * t);
                }

                span[23] = (byte)'\"';
            }

            writer.Writer.Advance(length);
        }

        public DateTimeOffset Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static DateTimeOffset DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var str = reader.ReadNotNullStringSegmentRaw();
            var i = 0;
            switch (str.Length)
            {
                // YYYY
                case 4:
                    {
                        var y = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');
                        return new DateTimeOffset(y, 1, 1, 0, 0, 0, TimeSpan.Zero);
                    }
                // YYYY-MM
                case 7:
                    {
                        var y = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                        if (str[i++] != (byte)'-') goto ERROR;
                        var m = (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');
                        return new DateTimeOffset(y, m, 1, 0, 0, 0, TimeSpan.Zero);
                    }
                // YYYY-MM-DD
                case 10:
                    {
                        var y = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                        if (str[i++] != (byte)'-') goto ERROR;
                        var m = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                        if (str[i++] != (byte)'-') goto ERROR;
                        var d = (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');
                        return new DateTimeOffset(y, m, d, 0, 0, 0, TimeSpan.Zero);
                    }
            }

            // range-first section requires 19
            if (str.Length < 19) goto ERROR;

            var year = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
            if (str[i++] != (byte)'-') goto ERROR;
            var month = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
            if (str[i++] != (byte)'-') goto ERROR;
            var day = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');

            if (str[i++] != (byte)'T') goto ERROR;

            var hour = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
            if (str[i++] != (byte)':') goto ERROR;
            var minute = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
            if (str[i++] != (byte)':') goto ERROR;
            var second = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');

            var ticks = 0;
            if (i < str.Length && str[i] == '.')
            {
                i++;

                // *7.
                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 1000000;
                i++;

                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 100000;
                i++;

                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 10000;
                i++;

                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 1000;
                i++;

                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 100;
                i++;

                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 10;
                i++;

                if (!(i < str.Length) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 1;
                i++;

                // others, lack of precision
                while (i < +str.Length && IntegerConverter.IsNumber(str[i]))
                {
                    i++;
                }
            }

        END_TICKS:

            if ((i >= str.Length || str[i] != '-') && str[i] != '+')
            {
                return new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero).AddTicks(ticks);
            }

            {
                if (!(i + 5 < str.Length))
                {
                    goto ERROR;
                }

                var minus = str[i++] == '-';

                var h = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                i++;
                var m = (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');

                var offset = new TimeSpan(h, m, 0);
                if (minus)
                {
                    offset = offset.Negate();
                }

                return new DateTimeOffset(year, month, day, hour, minute, second, offset).AddTicks(ticks);
            }


        ERROR:
#if SPAN_BUILTIN
            throw new InvalidOperationException("invalid datetime format. value:" + StringEncoding.Utf8.GetString(str));
#else
            unsafe
            {
                fixed (byte* ptr = &str[0])
                {
                    throw new InvalidOperationException("invalid datetime format. value:" + StringEncoding.Utf8.GetString(ptr, str.Length));
                }
            }
#endif
        }
    }

    // ReSharper disable once InconsistentNaming
    public sealed class NullableISO8601DateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset?>
    {
        public void Serialize(ref JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ISO8601DateTimeOffsetFormatter.SerializeStatic(ref writer, value.Value, options);
                return;
            }

            var span = writer.Writer.GetSpan(4);
            span[0] = (byte)'n';
            span[1] = (byte)'u';
            span[2] = (byte)'l';
            span[3] = (byte)'l';
            writer.Writer.Advance(4);
        }

        public DateTimeOffset? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default : ISO8601DateTimeOffsetFormatter.DeserializeStatic(ref reader, options);
        }
    }
}