// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Formatters
{
    public sealed class JsonObjectFormatter : IJsonFormatter<JsonObject>
    {
        public JsonObject Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public void Serialize(ref JsonWriter writer, JsonObject value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static JsonObject DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.SkipWhiteSpace();
            var jsonToken = reader.GetCurrentJsonToken();
            switch (jsonToken)
            {
                case JsonToken.BeginObject:
                    reader.ReadNextBlock();
                    reader.SkipWhiteSpace();
                    if (reader.GetCurrentJsonToken() != JsonToken.EndObject)
                    {
                        return DeserializeStaticObject(ref reader, options);
                    }

                    reader.ReadNextBlock();
                    return new JsonObject
                    {
                        Token = JsonObject.Kind.EmptyObject,
                    };
                case JsonToken.BeginArray:
                    reader.ReadNextBlock();
                    reader.SkipWhiteSpace();
                    if (reader.GetCurrentJsonToken() != JsonToken.EndArray)
                    {
                        return DeserializeStaticArray(ref reader, options);
                    }

                    reader.ReadNextBlock();
                    return new JsonObject
                    {
                        Token = JsonObject.Kind.EmptyArray,
                    };
                case JsonToken.Number:
                    return new JsonObject
                    {
                        Number = reader.ReadDouble(),
                        Token = JsonObject.Kind.Number,
                    };
                case JsonToken.String:
                    return new JsonObject
                    {
                        String = reader.ReadString(),
                        Token = JsonObject.Kind.String,
                    };
                case JsonToken.True:
                    reader.ReadNextBlock();
                    return new JsonObject
                    {
                        Token = JsonObject.Kind.True,
                    };
                case JsonToken.False:
                    reader.ReadNextBlock();
                    return new JsonObject
                    {
                        Token = JsonObject.Kind.False,
                    };
                case JsonToken.Null:
                    reader.ReadNextBlock();
                    return new JsonObject
                    {
                        Token = JsonObject.Kind.Null,
                    };
                case JsonToken.ValueSeparator:
                case JsonToken.NameSeparator:
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                case JsonToken.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static JsonObject DeserializeStaticArray(ref JsonReader reader, JsonSerializerOptions options)
        {
            var pool = ArrayPool<JsonObject>.Shared;
            var array = pool.Rent(256);
            try
            {
                array[0] = DeserializeStatic(ref reader, options);
                var count = 1;
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (array.Length < count)
                    {
                        var tmp = pool.Rent(count << 1);
                        Array.Copy(array, tmp, array.Length);
                        pool.Return(array);
                        array = tmp;
                    }

                    array[count - 1] = DeserializeStatic(ref reader, options);
                }

                var answer = new JsonObject
                {
                    ObjectArray = new JsonObject[count],
                    Token = JsonObject.Kind.Object,
                };
                Array.Copy(array, answer.ObjectArray, count);

                answer.ReCalc();
                return answer;
            }
            finally
            {
                pool.Return(array);
            }
        }

        private static JsonObject DeserializeStaticObject(ref JsonReader reader, JsonSerializerOptions options)
        {
            var answer = new JsonObject
            {
                Token = JsonObject.Kind.Object,
                ObjectDictionary = new Dictionary<string, JsonObject>(),
            };

            var propertyName = reader.ReadPropertyName();
            var jsonObject = DeserializeStatic(ref reader, options);
            answer.ObjectDictionary.Add(propertyName, jsonObject);
            var count = 1;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                propertyName = reader.ReadPropertyName();
                jsonObject = DeserializeStatic(ref reader, options);
                answer.ObjectDictionary.Add(propertyName, jsonObject);
            }

            return answer;
        }

        public static void SerializeStatic(ref JsonWriter writer, JsonObject value, JsonSerializerOptions options)
        {
            switch (value.Token)
            {
                case JsonObject.Kind.EmptyObject:
                    {
                        var span = writer.Writer.GetSpan(2);
                        span[0] = (byte)'{';
                        span[1] = (byte)'}';
                        writer.Writer.Advance(2);
                    }
                    break;
                case JsonObject.Kind.EmptyArray:
                    {
                        var span = writer.Writer.GetSpan(2);
                        span[0] = (byte)'[';
                        span[1] = (byte)']';
                        writer.Writer.Advance(2);
                    }
                    break;
                case JsonObject.Kind.Object:
                    writer.WriteBeginObject();
                    var objectDictionary = value.ObjectDictionary;
                    Debug.Assert(objectDictionary != null);
                    var enumerator = objectDictionary.GetEnumerator();
                    try
                    {
                        if (!enumerator.MoveNext())
                        {
                            goto END_OBJECT;
                        }

                        var pair = enumerator.Current;
                        writer.WritePropertyName(pair.Key);
                        SerializeStatic(ref writer, pair.Value, options);

                        while (enumerator.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            pair = enumerator.Current;
                            writer.WritePropertyName(pair.Key);
                            SerializeStatic(ref writer, pair.Value, options);
                        }
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }

                END_OBJECT:
                    writer.WriteEndObject();
                    break;
                case JsonObject.Kind.Array:
                    writer.WriteBeginArray();
                    var objectArray = value.ObjectArray;
                    Debug.Assert(objectArray != null);
                    SerializeStatic(ref writer, objectArray[0], options);
                    for (var index = 1; index < objectArray.Length; index++)
                    {
                        writer.WriteValueSeparator();
                        SerializeStatic(ref writer, objectArray[index], options);
                    }

                    writer.WriteEndArray();
                    break;
                case JsonObject.Kind.BooleanArray:
                    writer.WriteBeginArray();
                    var booleanArray = value.BooleanArray;
                    Debug.Assert(booleanArray != null);
                    writer.Write(booleanArray[0]);
                    for (var index = 1; index < booleanArray.Length; index++)
                    {
                        writer.WriteValueSeparator();
                        writer.Write(booleanArray[index]);
                    }

                    writer.WriteEndArray();
                    break;
                case JsonObject.Kind.StringArray:
                    writer.WriteBeginArray();
                    var stringArray = value.StringArray;
                    Debug.Assert(stringArray != null);
                    writer.Write(stringArray[0]);
                    for (var index = 1; index < stringArray.Length; index++)
                    {
                        writer.WriteValueSeparator();
                        writer.Write(stringArray[index]);
                    }

                    writer.WriteEndArray();
                    break;
                case JsonObject.Kind.NumberArray:
                    writer.WriteBeginArray();
                    var numberArray = value.NumberArray;
                    Debug.Assert(numberArray != null);
                    writer.Write(numberArray[0]);
                    for (var index = 1; index < numberArray.Length; index++)
                    {
                        writer.WriteValueSeparator();
                        writer.Write(numberArray[index]);
                    }

                    writer.WriteEndArray();
                    break;
                case JsonObject.Kind.String:
                    writer.Write(value.String);
                    break;
                case JsonObject.Kind.Number:
                    writer.Write(value.Number);
                    break;
                case JsonObject.Kind.True:
                    {
                        var span = writer.Writer.GetSpan(4);
                        span[0] = (byte)'t';
                        span[1] = (byte)'r';
                        span[2] = (byte)'u';
                        span[3] = (byte)'e';
                        writer.Writer.Advance(4);
                    }
                    break;
                case JsonObject.Kind.False:
                    {
                        var span = writer.Writer.GetSpan(5);
                        span[0] = (byte)'f';
                        span[1] = (byte)'a';
                        span[2] = (byte)'l';
                        span[3] = (byte)'s';
                        span[4] = (byte)'e';
                        writer.Writer.Advance(5);
                    }
                    break;
                case JsonObject.Kind.Null:
                    {
                        var span = writer.Writer.GetSpan(4);
                        span[0] = (byte)'n';
                        span[1] = (byte)'u';
                        span[2] = (byte)'l';
                        span[3] = (byte)'l';
                        writer.Writer.Advance(4);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            switch (value)
            {
                case JsonObject jsonObject:
                    SerializeStatic(ref writer, jsonObject, options);
                    return;
                default:
                    var formatter = options.Resolver.GetFormatterWithVerify(value.GetType());
                    formatter.SerializeTypeless(ref writer, value, options);
                    return;
            }
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
