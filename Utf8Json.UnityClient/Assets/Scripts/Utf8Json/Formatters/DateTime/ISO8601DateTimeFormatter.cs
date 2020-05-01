// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    // ReSharper disable once InconsistentNaming
    public sealed class ISO8601DateTimeFormatter : IJsonFormatter<DateTime>
    {
        public void Serialize(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var year = value.Year;
            var month = value.Month;
            var day = value.Day;
            var hour = value.Hour;
            var minute = value.Minute;
            var second = value.Second;
            var nanoSec = value.Ticks % TimeSpan.TicksPerSecond;

            const int baseLength = 19 + 2; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation

            Span<byte> span;
            int sizeHint;
            switch (value.Kind)
            {
                case DateTimeKind.Local:
                    sizeHint = baseLength + 6 + (nanoSec == 0 ? 0 : 8);
                    span = writer.Writer.GetSpan(sizeHint);
                    break;
                case DateTimeKind.Utc:
                    sizeHint = baseLength + 1 + (nanoSec == 0 ? 0 : 8);
                    span = writer.Writer.GetSpan(sizeHint);
                    break;
                // ReSharper disable once RedundantCaseLabel
                case DateTimeKind.Unspecified:
                default:
                    sizeHint = baseLength + (nanoSec == 0 ? 0 : 8);
                    span = writer.Writer.GetSpan(sizeHint);
                    break;
            }

            span[0] = (byte)'\"';
            span = span.Slice(1);

            if (year < 10)
            {
                span[0] = (byte)'0';
                span[1] = (byte)'0';
                span[2] = (byte)'0';
                span[3] = (byte)('0' + year);
            }
            else if (year < 100)
            {
                span[0] = (byte)'0';
                span[1] = (byte)'0';
                var t = year / 10;
                span[2] = (byte)('0' + t);
                span[3] = (byte)('0' + year - t * 10);
            }
            else if (year < 1000)
            {
                span[0] = (byte)'0';
                var t = year / 100;
                span[1] = (byte)('0' + t);
                year -= t * 100;
                t = year / 10;
                span[2] = (byte)('0' + t);
                span[3] = (byte)('0' + year - t * 10);
            }
            else
            {
                var t = year / 1000;
                span[0] = (byte)('0' + t);
                year -= t * 1000;
                t = year / 100;
                span[1] = (byte)('0' + t);
                year -= t * 100;
                t = year / 10;
                span[2] = (byte)('0' + t);
                span[3] = (byte)('0' + year - t * 10);
            }
            span[4] = ((byte)'-');
            span = span.Slice(5);

            if (month < 10)
            {
                span[0] = (byte)'0';
                span[1] = (byte)('0' + month);
            }
            else
            {
                var t = month / 10;
                span[0] = (byte)('0' + t);
                span[1] = (byte)('0' + month - t * 10);
            }
            span[2] = (byte)'-';
            span = span.Slice(3);

            if (day < 10)
            {
                span[0] = (byte)'0';
                span[1] = (byte)('0' + day);
            }
            else
            {
                var t = day / 10;
                span[0] = (byte)('0' + t);
                span[1] = (byte)('0' + day - t * 10);
            }
            span[2] = ((byte)'T');
            span = span.Slice(3);

            if (hour < 10)
            {
                span[0] = (byte)'0';
                span[1] = (byte)('0' + hour);
            }
            else
            {
                var t = hour / 10;
                span[0] = (byte)('0' + t);
                span[1] = (byte)('0' + hour - t * 10);
            }
            span[2] = ((byte)':');
            span = span.Slice(3);

            if (minute < 10)
            {
                span[0] = (byte)'0';
                span[1] = (byte)('0' + minute);
            }
            else
            {
                var t = minute / 10;
                span[0] = (byte)('0' + t);
                span[1] = (byte)('0' + minute - t * 10);
            }
            span[2] = (byte)':';
            span = span.Slice(3);

            if (second < 10)
            {
                span[0] = (byte)'0';
                span[1] = (byte)('0' + second);
            }
            else
            {
                var t = second / 10;
                span[0] = (byte)('0' + t);
                span[1] = (byte)('0' + second - t * 10);
            }
            span = span.Slice(2);

            if (nanoSec != 0)
            {
                span[0] = ((byte)'.');
                span = span.Slice(1);

                if (nanoSec < 10)
                {
                    span[0] = (byte)'0';
                    span[1] = (byte)'0';
                    span[2] = (byte)'0';
                    span[3] = (byte)'0';
                    span[4] = (byte)'0';
                    span[5] = (byte)'0';
                    span[6] = (byte)('0' + nanoSec);
                }
                else if (nanoSec < 100)
                {
                    span[0] = (byte)'0';
                    span[1] = (byte)'0';
                    span[2] = (byte)'0';
                    span[3] = (byte)'0';
                    span[4] = (byte)'0';
                    var t = nanoSec / 10;
                    span[5] = (byte)('0' + t);
                    nanoSec -= t * 10;
                    span[6] = (byte)('0' + nanoSec);
                }
                else if (nanoSec < 1000)
                {
                    span[0] = (byte)'0';
                    span[1] = (byte)'0';
                    span[2] = (byte)'0';
                    span[3] = (byte)'0';
                    var t = nanoSec / 100;
                    span[4] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[5] = (byte)('0' + t);
                    nanoSec -= t * 10;
                    span[6] = (byte)('0' + nanoSec);
                }
                else if (nanoSec < 10000)
                {
                    span[0] = (byte)'0';
                    span[1] = (byte)'0';
                    span[2] = (byte)'0';
                    var t = nanoSec / 1000;
                    span[3] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[4] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[5] = (byte)('0' + t);
                    nanoSec -= t * 10;
                    span[6] = (byte)('0' + nanoSec);
                }
                else if (nanoSec < 100000)
                {
                    span[0] = (byte)'0';
                    span[1] = (byte)'0';
                    var t = nanoSec / 10000;
                    span[2] = (byte)('0' + t);
                    nanoSec -= t * 10000;
                    t = nanoSec / 1000;
                    span[3] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[4] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[5] = (byte)('0' + t);
                    nanoSec -= t * 10;
                    span[6] = (byte)('0' + nanoSec);
                }
                else if (nanoSec < 1000000)
                {
                    span[0] = (byte)'0';
                    var t = nanoSec / 100000;
                    span[1] = (byte)('0' + t);
                    nanoSec -= t * 100000;
                    t = nanoSec / 10000;
                    span[2] = (byte)('0' + t);
                    nanoSec -= t * 10000;
                    t = nanoSec / 1000;
                    span[3] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[4] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[5] = (byte)('0' + t);
                    nanoSec -= t * 10;
                    span[6] = (byte)('0' + nanoSec);
                }
                else
                {
                    var t = nanoSec / 1000000;
                    span[0] = (byte)('0' + t);
                    nanoSec -= t * 1000000;
                    t = nanoSec / 100000;
                    span[1] = (byte)('0' + t);
                    nanoSec -= t * 100000;
                    t = nanoSec / 10000;
                    span[2] = (byte)('0' + t);
                    nanoSec -= t * 10000;
                    t = nanoSec / 1000;
                    span[3] = (byte)('0' + t);
                    nanoSec -= t * 1000;
                    t = nanoSec / 100;
                    span[4] = (byte)('0' + t);
                    nanoSec -= t * 100;
                    t = nanoSec / 10;
                    span[5] = (byte)('0' + t);
                    nanoSec -= t * 10;
                    span[6] = (byte)('0' + nanoSec);
                }

                span = span.Slice(7);
            }

            switch (value.Kind)
            {
                case DateTimeKind.Local:
                    // should not use `BaseUtcOffset` - https://stackoverflow.com/questions/10019267/is-there-a-generic-timezoneinfo-for-central-europe
                    var localOffset = TimeZoneInfo.Local.GetUtcOffset(value);
                    var minus = (localOffset < TimeSpan.Zero);
                    if (minus) localOffset = localOffset.Negate();
                    var h = localOffset.Hours;
                    var m = localOffset.Minutes;
                    span[0] = (minus ? (byte)'-' : (byte)'+');
                    span = span.Slice(1);
                    if (h < 10)
                    {
                        span[0] = (byte)'0';
                        span[1] = (byte)('0' + h);
                    }
                    else
                    {
                        var t = h / 10;
                        span[0] = (byte)('0' + t);
                        span[1] = (byte)('0' + h - t * 10);
                    }
                    span[2] = ((byte)':');
                    span = span.Slice(3);
                    if (m < 10)
                    {
                        span[0] = (byte)'0';
                        span[1] = (byte)('0' + m);
                    }
                    else
                    {
                        var t = m / 10;
                        span[0] = (byte)('0' + t);
                        span[1] = (byte)('0' + m - t * 10);
                    }
                    span = span.Slice(2);
                    break;
                case DateTimeKind.Utc:
                    span[0] = (byte)'Z';
                    span = span.Slice(1);
                    break;
                // ReSharper disable once RedundantCaseLabel
                case DateTimeKind.Unspecified:
                default:
                    break;
            }

            span[0] = ((byte)'\"');

            writer.Writer.Advance(sizeHint);
        }

        public DateTime Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static DateTime DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var str = reader.ReadNotNullStringSegmentRaw();
            var i = 0;

            switch (str.Length)
            {
                // YYYY
                case 4:
                    {
                        var y = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');
                        return new DateTime(y, 1, 1);
                    }
                // YYYY-MM
                case 7:
                    {
                        var y = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                        if (str[i++] != (byte)'-') goto ERROR;
                        var m = (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');
                        return new DateTime(y, m, 1);
                    }
                // YYYY-MM-DD
                case 10:
                    {
                        var y = (str[i++] - (byte)'0') * 1000 + (str[i++] - (byte)'0') * 100 + (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                        if (str[i++] != (byte)'-') goto ERROR;
                        var m = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                        if (str[i++] != (byte)'-') goto ERROR;
                        var d = (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');
                        return new DateTime(y, m, d);
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
                while (i < str.Length && IntegerConverter.IsNumber(str[i]))
                {
                    i++;
                }
            }

        END_TICKS:
            var kind = DateTimeKind.Unspecified;
            if (i < str.Length && str[i] == 'Z')
            {
                kind = DateTimeKind.Utc;
            }
            else if (i < str.Length && str[i] == '-' || str[i] == '+')
            {
                if (!(i + 5 < str.Length)) goto ERROR;

                var minus = str[i++] == '-';

                var h = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
                i++;
                var m = (str[i++] - (byte)'0') * 10 + (str[i] - (byte)'0');

                var offset = new TimeSpan(h, m, 0);
                if (minus) offset = offset.Negate();

                return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc).AddTicks(ticks).Subtract(offset).ToLocalTime();
            }

            return new DateTime(year, month, day, hour, minute, second, kind).AddTicks(ticks);

        ERROR:
#if SPAN_BUILTIN
            throw new InvalidOperationException("invalid datetime format. value:" + StringEncoding.Utf8.GetString(str));
#else
            throw new InvalidOperationException("invalid datetime format.");
#endif
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is DateTime innerValue))
            {
                throw new ArgumentNullException();
            }

            SerializeStatic(ref writer, innerValue, options);
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
