// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ValueTupleFormatter<T1, T2> : IJsonFormatter<(T1, T2)>
    {
        public void Serialize(ref JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
            var resolver = options.Resolver;
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

            {
                var serializer = resolver.GetSerializeStatic<T1>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T1>();
                    formatter.Serialize(ref writer, value.Item1, options);
                }
                else
                {
                    writer.Serialize(value.Item1, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T2>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T2>();
                    formatter.Serialize(ref writer, value.Item2, options);
                }
                else
                {
                    writer.Serialize(value.Item2, options, serializer);
                }
            }
        }

        public (T1, T2) Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static (T1, T2) DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default((T1, T2));
            var resolver = options.Resolver;
            var count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadNotNullStringSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T1>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T1>();
                                    answer.Item1 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item1 = reader.Deserialize<T1>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'2':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T2>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T2>();
                                    answer.Item2 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item2 = reader.Deserialize<T2>(options, deserializer);
                                }
                            }
                            continue;
                    }
                }
                
                reader.ReadNextBlock();
            }

            return answer;
        }
    }

    public sealed unsafe class ValueTupleFormatter<T1, T2, T3> : IJsonFormatter<(T1, T2, T3)>
    {
        public void Serialize(ref JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options)
        {
            var resolver = options.Resolver;
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

            {
                var serializer = resolver.GetSerializeStatic<T1>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T1>();
                    formatter.Serialize(ref writer, value.Item1, options);
                }
                else
                {
                    writer.Serialize(value.Item1, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T2>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T2>();
                    formatter.Serialize(ref writer, value.Item2, options);
                }
                else
                {
                    writer.Serialize(value.Item2, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T3>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T3>();
                    formatter.Serialize(ref writer, value.Item3, options);
                }
                else
                {
                    writer.Serialize(value.Item3, options, serializer);
                }
            }
        }

        public (T1, T2, T3) Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static (T1, T2, T3) DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default((T1, T2, T3));
            var resolver = options.Resolver;
            var count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadNotNullStringSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T1>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T1>();
                                    answer.Item1 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item1 = reader.Deserialize<T1>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'2':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T2>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T2>();
                                    answer.Item2 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item2 = reader.Deserialize<T2>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'3':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T3>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T3>();
                                    answer.Item3 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item3 = reader.Deserialize<T3>(options, deserializer);
                                }
                            }
                            continue;
                    }
                }
                
                reader.ReadNextBlock();
            }

            return answer;
        }
    }

    public sealed unsafe class ValueTupleFormatter<T1, T2, T3, T4> : IJsonFormatter<(T1, T2, T3, T4)>
    {
        public void Serialize(ref JsonWriter writer, (T1, T2, T3, T4) value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, (T1, T2, T3, T4) value, JsonSerializerOptions options)
        {
            var resolver = options.Resolver;
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

            {
                var serializer = resolver.GetSerializeStatic<T1>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T1>();
                    formatter.Serialize(ref writer, value.Item1, options);
                }
                else
                {
                    writer.Serialize(value.Item1, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T2>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T2>();
                    formatter.Serialize(ref writer, value.Item2, options);
                }
                else
                {
                    writer.Serialize(value.Item2, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T3>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T3>();
                    formatter.Serialize(ref writer, value.Item3, options);
                }
                else
                {
                    writer.Serialize(value.Item3, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T4>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T4>();
                    formatter.Serialize(ref writer, value.Item4, options);
                }
                else
                {
                    writer.Serialize(value.Item4, options, serializer);
                }
            }
        }

        public (T1, T2, T3, T4) Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static (T1, T2, T3, T4) DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default((T1, T2, T3, T4));
            var resolver = options.Resolver;
            var count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadNotNullStringSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T1>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T1>();
                                    answer.Item1 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item1 = reader.Deserialize<T1>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'2':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T2>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T2>();
                                    answer.Item2 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item2 = reader.Deserialize<T2>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'3':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T3>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T3>();
                                    answer.Item3 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item3 = reader.Deserialize<T3>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'4':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T4>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T4>();
                                    answer.Item4 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item4 = reader.Deserialize<T4>(options, deserializer);
                                }
                            }
                            continue;
                    }
                }
                
                reader.ReadNextBlock();
            }

            return answer;
        }
    }

    public sealed unsafe class ValueTupleFormatter<T1, T2, T3, T4, T5> : IJsonFormatter<(T1, T2, T3, T4, T5)>
    {
        public void Serialize(ref JsonWriter writer, (T1, T2, T3, T4, T5) value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, (T1, T2, T3, T4, T5) value, JsonSerializerOptions options)
        {
            var resolver = options.Resolver;
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

            {
                var serializer = resolver.GetSerializeStatic<T1>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T1>();
                    formatter.Serialize(ref writer, value.Item1, options);
                }
                else
                {
                    writer.Serialize(value.Item1, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T2>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T2>();
                    formatter.Serialize(ref writer, value.Item2, options);
                }
                else
                {
                    writer.Serialize(value.Item2, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T3>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T3>();
                    formatter.Serialize(ref writer, value.Item3, options);
                }
                else
                {
                    writer.Serialize(value.Item3, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T4>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T4>();
                    formatter.Serialize(ref writer, value.Item4, options);
                }
                else
                {
                    writer.Serialize(value.Item4, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T5>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T5>();
                    formatter.Serialize(ref writer, value.Item5, options);
                }
                else
                {
                    writer.Serialize(value.Item5, options, serializer);
                }
            }
        }

        public (T1, T2, T3, T4, T5) Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static (T1, T2, T3, T4, T5) DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default((T1, T2, T3, T4, T5));
            var resolver = options.Resolver;
            var count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadNotNullStringSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T1>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T1>();
                                    answer.Item1 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item1 = reader.Deserialize<T1>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'2':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T2>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T2>();
                                    answer.Item2 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item2 = reader.Deserialize<T2>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'3':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T3>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T3>();
                                    answer.Item3 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item3 = reader.Deserialize<T3>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'4':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T4>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T4>();
                                    answer.Item4 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item4 = reader.Deserialize<T4>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'5':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T5>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T5>();
                                    answer.Item5 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item5 = reader.Deserialize<T5>(options, deserializer);
                                }
                            }
                            continue;
                    }
                }
                
                reader.ReadNextBlock();
            }

            return answer;
        }
    }

    public sealed unsafe class ValueTupleFormatter<T1, T2, T3, T4, T5, T6> : IJsonFormatter<(T1, T2, T3, T4, T5, T6)>
    {
        public void Serialize(ref JsonWriter writer, (T1, T2, T3, T4, T5, T6) value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, (T1, T2, T3, T4, T5, T6) value, JsonSerializerOptions options)
        {
            var resolver = options.Resolver;
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

            {
                var serializer = resolver.GetSerializeStatic<T1>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T1>();
                    formatter.Serialize(ref writer, value.Item1, options);
                }
                else
                {
                    writer.Serialize(value.Item1, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T2>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T2>();
                    formatter.Serialize(ref writer, value.Item2, options);
                }
                else
                {
                    writer.Serialize(value.Item2, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T3>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T3>();
                    formatter.Serialize(ref writer, value.Item3, options);
                }
                else
                {
                    writer.Serialize(value.Item3, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T4>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T4>();
                    formatter.Serialize(ref writer, value.Item4, options);
                }
                else
                {
                    writer.Serialize(value.Item4, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T5>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T5>();
                    formatter.Serialize(ref writer, value.Item5, options);
                }
                else
                {
                    writer.Serialize(value.Item5, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T6>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T6>();
                    formatter.Serialize(ref writer, value.Item6, options);
                }
                else
                {
                    writer.Serialize(value.Item6, options, serializer);
                }
            }
        }

        public (T1, T2, T3, T4, T5, T6) Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static (T1, T2, T3, T4, T5, T6) DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default((T1, T2, T3, T4, T5, T6));
            var resolver = options.Resolver;
            var count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadNotNullStringSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T1>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T1>();
                                    answer.Item1 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item1 = reader.Deserialize<T1>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'2':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T2>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T2>();
                                    answer.Item2 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item2 = reader.Deserialize<T2>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'3':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T3>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T3>();
                                    answer.Item3 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item3 = reader.Deserialize<T3>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'4':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T4>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T4>();
                                    answer.Item4 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item4 = reader.Deserialize<T4>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'5':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T5>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T5>();
                                    answer.Item5 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item5 = reader.Deserialize<T5>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'6':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T6>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T6>();
                                    answer.Item6 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item6 = reader.Deserialize<T6>(options, deserializer);
                                }
                            }
                            continue;
                    }
                }
                
                reader.ReadNextBlock();
            }

            return answer;
        }
    }

    public sealed unsafe class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7> : IJsonFormatter<(T1, T2, T3, T4, T5, T6, T7)>
    {
        public void Serialize(ref JsonWriter writer, (T1, T2, T3, T4, T5, T6, T7) value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, (T1, T2, T3, T4, T5, T6, T7) value, JsonSerializerOptions options)
        {
            var resolver = options.Resolver;
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

            {
                var serializer = resolver.GetSerializeStatic<T1>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T1>();
                    formatter.Serialize(ref writer, value.Item1, options);
                }
                else
                {
                    writer.Serialize(value.Item1, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T2>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T2>();
                    formatter.Serialize(ref writer, value.Item2, options);
                }
                else
                {
                    writer.Serialize(value.Item2, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T3>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T3>();
                    formatter.Serialize(ref writer, value.Item3, options);
                }
                else
                {
                    writer.Serialize(value.Item3, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T4>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T4>();
                    formatter.Serialize(ref writer, value.Item4, options);
                }
                else
                {
                    writer.Serialize(value.Item4, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T5>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T5>();
                    formatter.Serialize(ref writer, value.Item5, options);
                }
                else
                {
                    writer.Serialize(value.Item5, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T6>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T6>();
                    formatter.Serialize(ref writer, value.Item6, options);
                }
                else
                {
                    writer.Serialize(value.Item6, options, serializer);
                }
            }

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

            
            {
                var serializer = resolver.GetSerializeStatic<T7>();
                if (serializer.ToPointer() == null)
                {
                    var formatter = resolver.GetFormatterWithVerify<T7>();
                    formatter.Serialize(ref writer, value.Item7, options);
                }
                else
                {
                    writer.Serialize(value.Item7, options, serializer);
                }
            }
        }

        public (T1, T2, T3, T4, T5, T6, T7) Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static (T1, T2, T3, T4, T5, T6, T7) DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var answer = default((T1, T2, T3, T4, T5, T6, T7));
            var resolver = options.Resolver;
            var count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadNotNullStringSegmentRaw();
                if (keySpan.Length == 5 && keySpan[0] == 'I' && keySpan[1] == 't' && keySpan[2] == 'e' && keySpan[3] == 'm')
                {
                    switch (keySpan[4])
                    {
                        case (byte)'1':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T1>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T1>();
                                    answer.Item1 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item1 = reader.Deserialize<T1>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'2':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T2>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T2>();
                                    answer.Item2 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item2 = reader.Deserialize<T2>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'3':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T3>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T3>();
                                    answer.Item3 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item3 = reader.Deserialize<T3>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'4':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T4>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T4>();
                                    answer.Item4 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item4 = reader.Deserialize<T4>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'5':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T5>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T5>();
                                    answer.Item5 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item5 = reader.Deserialize<T5>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'6':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T6>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T6>();
                                    answer.Item6 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item6 = reader.Deserialize<T6>(options, deserializer);
                                }
                            }
                            continue;
                        case (byte)'7':
                            {
                                var deserializer = resolver.GetDeserializeStatic<T7>();
                                if (deserializer.ToPointer() == null)
                                {
                                    var formatter = resolver.GetFormatterWithVerify<T7>();
                                    answer.Item7 = formatter.Deserialize(ref reader, options);
                                }
                                else
                                {
                                    answer.Item7 = reader.Deserialize<T7>(options, deserializer);
                                }
                            }
                            continue;
                    }
                }
                
                reader.ReadNextBlock();
            }

            return answer;
        }
    }

}
