// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class ValueTupleFormatter<T1, T2> : IJsonFormatter<ValueTuple<T1, T2>>
    {
        public void Serialize(ref JsonWriter writer, ValueTuple<T1, T2> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTuple<T1, T2> value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'1';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item1);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'2';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item2);

            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTuple<T1, T2> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default(ValueTuple<T1, T2>);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answer.Item1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answer.Item2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ValueTuple<T1, T2> innerValue))
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

    public sealed class ValueTupleFormatter<T1, T2, T3> : IJsonFormatter<ValueTuple<T1, T2, T3>>
    {
        public void Serialize(ref JsonWriter writer, ValueTuple<T1, T2, T3> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTuple<T1, T2, T3> value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'1';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item1);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'2';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item2);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'3';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item3);

            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTuple<T1, T2, T3> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default(ValueTuple<T1, T2, T3>);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answer.Item1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answer.Item2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answer.Item3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ValueTuple<T1, T2, T3> innerValue))
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

    public sealed class ValueTupleFormatter<T1, T2, T3, T4> : IJsonFormatter<ValueTuple<T1, T2, T3, T4>>
    {
        public void Serialize(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4> value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'1';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item1);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'2';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item2);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'3';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item3);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'4';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item4);

            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTuple<T1, T2, T3, T4> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default(ValueTuple<T1, T2, T3, T4>);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answer.Item1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answer.Item2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answer.Item3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answer.Item4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ValueTuple<T1, T2, T3, T4> innerValue))
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

    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public void Serialize(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'1';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item1);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'2';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item2);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'3';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item3);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'4';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item4);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'5';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item5);

            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTuple<T1, T2, T3, T4, T5> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default(ValueTuple<T1, T2, T3, T4, T5>);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answer.Item1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answer.Item2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answer.Item3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answer.Item4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                        case (byte)'5':
                            answer.Item5 = options.DeserializeWithVerify<T5>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ValueTuple<T1, T2, T3, T4, T5> innerValue))
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

    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public void Serialize(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'1';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item1);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'2';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item2);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'3';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item3);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'4';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item4);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'5';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item5);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'6';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item6);

            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default(ValueTuple<T1, T2, T3, T4, T5, T6>);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answer.Item1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answer.Item2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answer.Item3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answer.Item4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                        case (byte)'5':
                            answer.Item5 = options.DeserializeWithVerify<T5>(ref reader);
                            continue;
                        case (byte)'6':
                            answer.Item6 = options.DeserializeWithVerify<T6>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ValueTuple<T1, T2, T3, T4, T5, T6> innerValue))
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

    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public void Serialize(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options)
        {
            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)'{';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'1';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item1);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'2';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item2);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'3';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item3);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'4';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item4);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'5';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item5);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'6';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item6);

            {
                const int sizeHint = 9;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)',';
                span[1] = (byte)'"';
                span[2] = (byte)'I';
                span[3] = (byte)'t';
                span[4] = (byte)'e';
                span[5] = (byte)'m';
                span[6] = (byte)'7';
                span[7] = (byte)'"';
                span[8] = (byte)':';
                writer.Writer.Advance(sizeHint);
            }

            options.SerializeWithVerify(ref writer, value.Item7);

            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default(ValueTuple<T1, T2, T3, T4, T5, T6, T7>);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answer.Item1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answer.Item2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answer.Item3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answer.Item4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                        case (byte)'5':
                            answer.Item5 = options.DeserializeWithVerify<T5>(ref reader);
                            continue;
                        case (byte)'6':
                            answer.Item6 = options.DeserializeWithVerify<T6>(ref reader);
                            continue;
                        case (byte)'7':
                            answer.Item7 = options.DeserializeWithVerify<T7>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is ValueTuple<T1, T2, T3, T4, T5, T6, T7> innerValue))
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
