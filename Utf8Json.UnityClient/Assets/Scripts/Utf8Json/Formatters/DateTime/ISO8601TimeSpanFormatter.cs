// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    // ReSharper disable once InconsistentNaming
    public sealed class ISO8601TimeSpanFormatter : IJsonFormatter<TimeSpan>
    {
        // StringEncoding.UTF8.GetBytes("\"" + TimeSpan.MinValue + "\"");
        private static readonly byte[] minValue = { 0x22, 0x2D, 0x31, 0x30, 0x36, 0x37, 0x35, 0x31, 0x39, 0x39, 0x2E, 0x30, 0x32, 0x3A, 0x34, 0x38, 0x3A, 0x30, 0x35, 0x2E, 0x34, 0x37, 0x37, 0x35, 0x38, 0x30, 0x38, 0x22, };

        public void Serialize(ref JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            // can not negate, use cache
            if (value == TimeSpan.MinValue)
            {
                writer.WriteRaw(minValue);
                return;
            }


            var minus = value < TimeSpan.Zero;
            if (minus) value = value.Negate();
            var span = writer.Writer.GetSpan(minus ? 2 : 1);
            span[0] = (byte)'\"';
            if (minus)
            {
                span[1] = (byte)'-';
            }
            writer.Writer.Advance(minus ? 2 : 1);

            var day = value.Days;
            if (day != 0)
            {
                writer.Write(day);
                writer.Writer.GetPointer(1) = (byte)'.';
                writer.Writer.Advance(1);
            }

            var hour = value.Hours;
            span = writer.Writer.GetSpan(8);
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
            span[2] = (byte)':';
            span = span.Slice(3);

            var minute = value.Minutes;
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

            var second = value.Seconds;
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
            writer.Writer.Advance(8);

            var nanosecond = value.Ticks % TimeSpan.TicksPerSecond;
            if (nanosecond != 0)
            {
                writer.Writer.GetPointer(1) = (byte)'.';
                writer.Writer.Advance(1);
                writer.Write(nanosecond);
            }

            writer.Writer.GetPointer(1) = (byte)'"';
            writer.Writer.Advance(1);
        }

        public TimeSpan Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static TimeSpan DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var str = reader.ReadNotNullStringSegmentRaw();
            var i = 0;
            var to = str.Length;

            // check day exists
            var hasDay = false;
            {
                var foundDot = false;
                var foundColon = false;
                for (var j = i; j < str.Length; j++)
                {
                    if (str[j] == '.')
                    {
                        if (foundColon)
                        {
                            break;
                        }
                        foundDot = true;
                    }
                    else if (str[j] == ':')
                    {
                        if (foundDot)
                        {
                            hasDay = true;
                        }
                        foundColon = true;
                    }
                }
            }

            // check sign
            var minus = false;
            if (str[i] == '-')
            {
                minus = true;
                i++;
            }

            var day = 0;
            if (hasDay)
            {
                var poolArray = ArrayPool<byte>.Shared.Rent(256);
                try
                {
                    for (; str[i] != '.'; i++)
                    {
                        poolArray[day++] = str[i];
                    }

                    var jsonReader = new JsonReader(poolArray);
                    day = jsonReader.ReadInt32();
                    i++; // skip '.'
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(poolArray);
                }
            }

            var hour = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
            if (str[i++] != (byte)':') goto ERROR;
            var minute = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');
            if (str[i++] != (byte)':') goto ERROR;
            var second = (str[i++] - (byte)'0') * 10 + (str[i++] - (byte)'0');

            var ticks = 0;
            if (i < to && str[i] == '.')
            {
                i++;

                // *7.
                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 1000000;
                i++;

                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 100000;
                i++;

                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 10000;
                i++;

                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 1000;
                i++;

                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 100;
                i++;

                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 10;
                i++;

                if (!(i < to) || !IntegerConverter.IsNumber(str[i])) goto END_TICKS;
                ticks += (str[i] - (byte)'0') * 1;
                i++;

                // others, lack of precision
                while (i < to && IntegerConverter.IsNumber(str[i]))
                {
                    i++;
                }
            }

        END_TICKS:

            // be careful to overflow
            var ts = new TimeSpan(day, hour, minute, second);
            var tk = TimeSpan.FromTicks(ticks);
            return (minus)
                ? ts.Negate().Subtract(tk)
                : ts.Add(tk);

        ERROR:
#if SPAN_BUILTIN
            throw new InvalidOperationException("invalid datetime format. value:" + StringEncoding.Utf8.GetString(str));
#else
            throw new InvalidOperationException("invalid datetime format.");
#endif
        }
    }

    // ReSharper disable once InconsistentNaming
    public sealed class NullableISO8601TimeSpanFormatter : IJsonFormatter<TimeSpan?>
    {
        public void Serialize(ref JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ISO8601TimeSpanFormatter.SerializeStatic(ref writer, value.Value, options);
                return;
            }

            writer.WriteNull();
        }

        public static void SerializeStatic(ref JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                ISO8601TimeSpanFormatter.SerializeStatic(ref writer, value.Value, options);
                return;
            }

            writer.WriteNull();
        }

        public TimeSpan? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default(TimeSpan?) : ISO8601TimeSpanFormatter.DeserializeStatic(ref reader, options);
        }

        public static TimeSpan? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            return reader.ReadIsNull() ? default(TimeSpan?) : ISO8601TimeSpanFormatter.DeserializeStatic(ref reader, options);
        }
    }
}