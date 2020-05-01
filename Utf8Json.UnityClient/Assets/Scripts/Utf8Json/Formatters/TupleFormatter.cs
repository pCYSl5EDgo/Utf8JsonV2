// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    public sealed class TupleFormatter<T1, T2>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Tuple<T1, T2>?>
#else
    : IJsonFormatter<Tuple<T1, T2>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public Tuple<T1, T2>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Tuple<T1, T2> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Tuple<T1, T2>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Tuple<T1, T2> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var answerItem1 = default(T1);
            var answerItem2 = default(T2);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answerItem1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answerItem2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

#if CSHARP_8_OR_NEWER
            var answer = new Tuple<T1, T2>(answerItem1!, answerItem2!);
#else
            var answer = new Tuple<T1, T2>(answerItem1, answerItem2);
#endif
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Tuple<T1, T2>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class TupleFormatter<T1, T2, T3>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Tuple<T1, T2, T3>?>
#else
    : IJsonFormatter<Tuple<T1, T2, T3>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public Tuple<T1, T2, T3>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Tuple<T1, T2, T3> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Tuple<T1, T2, T3>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Tuple<T1, T2, T3> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var answerItem1 = default(T1);
            var answerItem2 = default(T2);
            var answerItem3 = default(T3);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answerItem1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answerItem2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answerItem3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

#if CSHARP_8_OR_NEWER
            var answer = new Tuple<T1, T2, T3>(answerItem1!, answerItem2!, answerItem3!);
#else
            var answer = new Tuple<T1, T2, T3>(answerItem1, answerItem2, answerItem3);
#endif
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Tuple<T1, T2, T3>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class TupleFormatter<T1, T2, T3, T4>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Tuple<T1, T2, T3, T4>?>
#else
    : IJsonFormatter<Tuple<T1, T2, T3, T4>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public Tuple<T1, T2, T3, T4>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Tuple<T1, T2, T3, T4> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Tuple<T1, T2, T3, T4>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Tuple<T1, T2, T3, T4> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var answerItem1 = default(T1);
            var answerItem2 = default(T2);
            var answerItem3 = default(T3);
            var answerItem4 = default(T4);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answerItem1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answerItem2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answerItem3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answerItem4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

#if CSHARP_8_OR_NEWER
            var answer = new Tuple<T1, T2, T3, T4>(answerItem1!, answerItem2!, answerItem3!, answerItem4!);
#else
            var answer = new Tuple<T1, T2, T3, T4>(answerItem1, answerItem2, answerItem3, answerItem4);
#endif
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Tuple<T1, T2, T3, T4>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class TupleFormatter<T1, T2, T3, T4, T5>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Tuple<T1, T2, T3, T4, T5>?>
#else
    : IJsonFormatter<Tuple<T1, T2, T3, T4, T5>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public Tuple<T1, T2, T3, T4, T5>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Tuple<T1, T2, T3, T4, T5> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Tuple<T1, T2, T3, T4, T5>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Tuple<T1, T2, T3, T4, T5> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var answerItem1 = default(T1);
            var answerItem2 = default(T2);
            var answerItem3 = default(T3);
            var answerItem4 = default(T4);
            var answerItem5 = default(T5);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answerItem1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answerItem2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answerItem3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answerItem4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                        case (byte)'5':
                            answerItem5 = options.DeserializeWithVerify<T5>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

#if CSHARP_8_OR_NEWER
            var answer = new Tuple<T1, T2, T3, T4, T5>(answerItem1!, answerItem2!, answerItem3!, answerItem4!, answerItem5!);
#else
            var answer = new Tuple<T1, T2, T3, T4, T5>(answerItem1, answerItem2, answerItem3, answerItem4, answerItem5);
#endif
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Tuple<T1, T2, T3, T4, T5>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class TupleFormatter<T1, T2, T3, T4, T5, T6>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Tuple<T1, T2, T3, T4, T5, T6>?>
#else
    : IJsonFormatter<Tuple<T1, T2, T3, T4, T5, T6>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public Tuple<T1, T2, T3, T4, T5, T6>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Tuple<T1, T2, T3, T4, T5, T6> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Tuple<T1, T2, T3, T4, T5, T6>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Tuple<T1, T2, T3, T4, T5, T6> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var answerItem1 = default(T1);
            var answerItem2 = default(T2);
            var answerItem3 = default(T3);
            var answerItem4 = default(T4);
            var answerItem5 = default(T5);
            var answerItem6 = default(T6);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answerItem1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answerItem2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answerItem3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answerItem4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                        case (byte)'5':
                            answerItem5 = options.DeserializeWithVerify<T5>(ref reader);
                            continue;
                        case (byte)'6':
                            answerItem6 = options.DeserializeWithVerify<T6>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

#if CSHARP_8_OR_NEWER
            var answer = new Tuple<T1, T2, T3, T4, T5, T6>(answerItem1!, answerItem2!, answerItem3!, answerItem4!, answerItem5!, answerItem6!);
#else
            var answer = new Tuple<T1, T2, T3, T4, T5, T6>(answerItem1, answerItem2, answerItem3, answerItem4, answerItem5, answerItem6);
#endif
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Tuple<T1, T2, T3, T4, T5, T6>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

    public sealed class TupleFormatter<T1, T2, T3, T4, T5, T6, T7>
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<Tuple<T1, T2, T3, T4, T5, T6, T7>?>
#else
    : IJsonFormatter<Tuple<T1, T2, T3, T4, T5, T6, T7>>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7>? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7>? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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

#if CSHARP_8_OR_NEWER
        public Tuple<T1, T2, T3, T4, T5, T6, T7>? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Tuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Tuple<T1, T2, T3, T4, T5, T6, T7>? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var answerItem1 = default(T1);
            var answerItem2 = default(T2);
            var answerItem3 = default(T3);
            var answerItem4 = default(T4);
            var answerItem5 = default(T5);
            var answerItem6 = default(T6);
            var answerItem7 = default(T7);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            answerItem1 = options.DeserializeWithVerify<T1>(ref reader);
                            continue;
                        case (byte)'2':
                            answerItem2 = options.DeserializeWithVerify<T2>(ref reader);
                            continue;
                        case (byte)'3':
                            answerItem3 = options.DeserializeWithVerify<T3>(ref reader);
                            continue;
                        case (byte)'4':
                            answerItem4 = options.DeserializeWithVerify<T4>(ref reader);
                            continue;
                        case (byte)'5':
                            answerItem5 = options.DeserializeWithVerify<T5>(ref reader);
                            continue;
                        case (byte)'6':
                            answerItem6 = options.DeserializeWithVerify<T6>(ref reader);
                            continue;
                        case (byte)'7':
                            answerItem7 = options.DeserializeWithVerify<T7>(ref reader);
                            continue;
                    }
                }

                reader.ReadNextBlock();
            }

#if CSHARP_8_OR_NEWER
            var answer = new Tuple<T1, T2, T3, T4, T5, T6, T7>(answerItem1!, answerItem2!, answerItem3!, answerItem4!, answerItem5!, answerItem6!, answerItem7!);
#else
            var answer = new Tuple<T1, T2, T3, T4, T5, T6, T7>(answerItem1, answerItem2, answerItem3, answerItem4, answerItem5, answerItem6, answerItem7);
#endif
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Tuple<T1, T2, T3, T4, T5, T6, T7>, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }

}
