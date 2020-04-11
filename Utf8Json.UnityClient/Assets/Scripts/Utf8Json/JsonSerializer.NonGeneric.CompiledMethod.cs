// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

#if SPAN_BUILTIN
using System.IO;
using System.Threading.Tasks;
#endif

// ReSharper disable InconsistentNaming

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        private sealed class CompiledMethods
        {
            internal delegate void JsonWriterSerialize(ref JsonWriter writer, object value, JsonSerializerOptions options);

            internal delegate void JsonWriterSerializeWithoutOptions(ref JsonWriter writer, object value);

            internal delegate object JsonReaderDeserialize(ref JsonReader reader, JsonSerializerOptions options);

            internal delegate object JsonReaderDeserializeWithoutOptions(ref JsonReader reader);

            internal readonly Func<object, JsonSerializerOptions, CancellationToken, byte[]> Serialize_T_Options;
            internal readonly Func<object, CancellationToken, byte[]> Serialize_T;
            internal readonly JsonWriterSerialize Serialize_JsonWriter_T_Options;
            internal readonly JsonWriterSerializeWithoutOptions Serialize_JsonWriter_T;
            internal readonly Action<IBufferWriter<byte>, object, JsonSerializerOptions, CancellationToken> Serialize_IBufferWriter_T_Options_CancellationToken;
            internal readonly Action<IBufferWriter<byte>, object, CancellationToken> Serialize_IBufferWriter_T_CancellationToken;

#if SPAN_BUILTIN
            internal readonly Action<Stream, object, JsonSerializerOptions, CancellationToken> Serialize_Stream_T_Options_CancellationToken;
            internal readonly Action<Stream, object, CancellationToken> Serialize_Stream_T_CancellationToken;
            internal readonly Func<Stream, object, JsonSerializerOptions, CancellationToken, Task> SerializeAsync_Stream_T_Options_CancellationToken;
            internal readonly Func<Stream, object, CancellationToken, Task> SerializeAsync_Stream_T_CancellationToken;
#endif

            internal readonly JsonReaderDeserialize Deserialize_JsonReader_Options;
            internal readonly JsonReaderDeserializeWithoutOptions Deserialize_JsonReader;
            internal readonly Func<ReadOnlyMemory<byte>, JsonSerializerOptions, CancellationToken, object> Deserialize_ReadOnlyMemory_Options;
            internal readonly Func<ReadOnlyMemory<byte>, CancellationToken, object> Deserialize_ReadOnlyMemory;
            internal readonly Func<ReadOnlySequence<byte>, JsonSerializerOptions, CancellationToken, object> Deserialize_ReadOnlySequence_Options_CancellationToken;
            internal readonly Func<ReadOnlySequence<byte>, CancellationToken, object> Deserialize_ReadOnlySequence_CancellationToken;

#if SPAN_BUILTIN
            internal readonly Func<Stream, JsonSerializerOptions, CancellationToken, object> Deserialize_Stream_Options_CancellationToken;
            internal readonly Func<Stream, CancellationToken, object> Deserialize_Stream_CancellationToken;
            internal readonly Func<Stream, JsonSerializerOptions, CancellationToken, ValueTask<object>> DeserializeAsync_Stream_Options_CancellationToken;
            internal readonly Func<Stream, CancellationToken, ValueTask<object>> DeserializeAsync_Stream_CancellationToken;
#endif

            internal CompiledMethods(Type type)
            {
                var ti = type.GetTypeInfo();
                {
                    // public static byte[] Serialize<T>(T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { null, typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(object), "obj");
                    var param2 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        ti.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type),
                        param2,
                        param3);
                    var lambda = Expression.Lambda<Func<object, JsonSerializerOptions, CancellationToken, byte[]>>(body, param1, param2, param3).Compile();

                    this.Serialize_T_Options = lambda;
                }

                {
                    // public static byte[] Serialize<T>(T obj, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { null, typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(object), "obj");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        ti.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type),
                        param3);
                    var lambda = Expression.Lambda<Func<object, CancellationToken, byte[]>>(body, param1, param3).Compile();

                    this.Serialize_T = lambda;
                }

#if SPAN_BUILTIN
                {
                    // public static void Serialize<T>(Stream stream, T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { typeof(Stream), null, typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param3 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param4 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param3,
                        param4);
                    var lambda = Expression.Lambda<Action<Stream, object, JsonSerializerOptions, CancellationToken>>(body, param1, param2, param3, param4).Compile();

                    this.Serialize_Stream_T_Options_CancellationToken = lambda;
                }

                {
                    // public static void Serialize<T>(Stream stream, T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { typeof(Stream), null, typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param4 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param4);
                    var lambda = Expression.Lambda<Action<Stream, object, CancellationToken>>(body, param1, param2, param4).Compile();

                    this.Serialize_Stream_T_CancellationToken = lambda;
                }

                {
                    // public static Task SerializeAsync<T>(Stream stream, T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(SerializeAsync), type, new[] { typeof(Stream), null, typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param3 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param4 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param3,
                        param4);
                    var lambda = Expression.Lambda<Func<Stream, object, JsonSerializerOptions, CancellationToken, Task>>(body, param1, param2, param3, param4).Compile();

                    this.SerializeAsync_Stream_T_Options_CancellationToken = lambda;
                }

                {
                    // public static Task SerializeAsync<T>(Stream stream, T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(SerializeAsync), type, new[] { typeof(Stream), null, typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param4 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param4);
                    var lambda = Expression.Lambda<Func<Stream, object, CancellationToken, Task>>(body, param1, param2, param4).Compile();

                    this.SerializeAsync_Stream_T_CancellationToken = lambda;
                }
#endif

                {
                    // public static Task Serialize<T>(IBufferWriter<byte> writer, T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { typeof(IBufferWriter<byte>), null, typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(IBufferWriter<byte>), "writer");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param3 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param4 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param3,
                        param4);
                    var lambda = Expression.Lambda<Action<IBufferWriter<byte>, object, JsonSerializerOptions, CancellationToken>>(body, param1, param2, param3, param4).Compile();

                    this.Serialize_IBufferWriter_T_Options_CancellationToken = lambda;
                }

                {
                    // public static Task Serialize<T>(IBufferWriter<byte> writer, T obj, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { typeof(IBufferWriter<byte>), null, typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(IBufferWriter<byte>), "writer");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param4 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param4);
                    var lambda = Expression.Lambda<Action<IBufferWriter<byte>, object, CancellationToken>>(body, param1, param2, param4).Compile();

                    this.Serialize_IBufferWriter_T_CancellationToken = lambda;
                }

                {
                    // public static void Serialize<T>(ref JsonWriter writer, T obj, JsonSerializerOptions options)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { typeof(JsonWriter).MakeByRefType(), null, typeof(JsonSerializerOptions) });

                    var param1 = Expression.Parameter(typeof(JsonWriter).MakeByRefType(), "writer");
                    var param2 = Expression.Parameter(typeof(object), "obj");
                    var param3 = Expression.Parameter(typeof(JsonSerializerOptions), "options");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                        param3);
                    var lambda = Expression.Lambda<JsonWriterSerialize>(body, param1, param2, param3).Compile();

                    this.Serialize_JsonWriter_T_Options = lambda;
                }

                {
                    // public static void Serialize<T>(ref JsonWriter writer, T obj, JsonSerializerOptions options)
                    var serialize = GetMethod(nameof(Serialize), type, new[] { typeof(JsonWriter).MakeByRefType(), null });

                    var param1 = Expression.Parameter(typeof(JsonWriter).MakeByRefType(), "writer");
                    var param2 = Expression.Parameter(typeof(object), "obj");

                    var body = Expression.Call(
                        null,
                        serialize,
                        param1,
                        ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type));
                    var lambda = Expression.Lambda<JsonWriterSerializeWithoutOptions>(body, param1, param2).Compile();

                    this.Serialize_JsonWriter_T = lambda;
                }

                {
                    // public static T Deserialize<T>(ref JsonReader reader, JsonSerializerOptions options)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(JsonReader).MakeByRefType(), typeof(JsonSerializerOptions) });

                    var param1 = Expression.Parameter(typeof(JsonReader).MakeByRefType(), "reader");
                    var param2 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param2), typeof(object));
                    var lambda = Expression.Lambda<JsonReaderDeserialize>(body, param1, param2).Compile();

                    this.Deserialize_JsonReader_Options = lambda;
                }

#if SPAN_BUILTIN
                {
                    // public static T Deserialize<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(Stream), typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param2 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param2, param3), typeof(object));
                    var lambda = Expression.Lambda<Func<Stream, JsonSerializerOptions, CancellationToken, object>>(body, param1, param2, param3).Compile();

                    this.Deserialize_Stream_Options_CancellationToken = lambda;
                }

                {
                    // public static ValueTask<object> DeserializeObjectAsync<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(DeserializeObjectAsync), type, new[] { typeof(Stream), typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param2 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param2, param3), typeof(ValueTask<object>));
                    var lambda = Expression.Lambda<Func<Stream, JsonSerializerOptions, CancellationToken, ValueTask<object>>>(body, param1, param2, param3).Compile();

                    this.DeserializeAsync_Stream_Options_CancellationToken = lambda;
                }
#endif

                {
                    // public static T Deserialize<T>(ReadOnlyMemory<byte> bytes, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(ReadOnlyMemory<byte>), typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(ReadOnlyMemory<byte>), "bytes");
                    var param2 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param2, param3), typeof(object));
                    var lambda = Expression.Lambda<Func<ReadOnlyMemory<byte>, JsonSerializerOptions, CancellationToken, object>>(body, param1, param2, param3).Compile();

                    this.Deserialize_ReadOnlyMemory_Options = lambda;
                }

                {
                    // public static T Deserialize<T>(ReadOnlySequence<byte> bytes, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(ReadOnlySequence<byte>).MakeByRefType(), typeof(JsonSerializerOptions), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "bytes");
                    var param2 = Expression.Parameter(typeof(JsonSerializerOptions), "options");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param2, param3), typeof(object));
                    var lambda = Expression.Lambda<Func<ReadOnlySequence<byte>, JsonSerializerOptions, CancellationToken, object>>(body, param1, param2, param3).Compile();

                    this.Deserialize_ReadOnlySequence_Options_CancellationToken = lambda;
                }

                {
                    // public static T Deserialize<T>(ref JsonReader reader, JsonSerializerOptions options)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(JsonReader).MakeByRefType() });

                    var param1 = Expression.Parameter(typeof(JsonReader).MakeByRefType(), "reader");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1), typeof(object));
                    var lambda = Expression.Lambda<JsonReaderDeserializeWithoutOptions>(body, param1).Compile();

                    this.Deserialize_JsonReader = lambda;
                }

#if SPAN_BUILTIN
                {
                    // public static T Deserialize<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(Stream), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param3), typeof(object));
                    var lambda = Expression.Lambda<Func<Stream, CancellationToken, object>>(body, param1, param3).Compile();

                    this.Deserialize_Stream_CancellationToken = lambda;
                }

                {
                    // public static ValueTask<object> DeserializeObjectAsync<T>(Stream stream, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(DeserializeObjectAsync), type, new[] { typeof(Stream), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(Stream), "stream");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param3), typeof(ValueTask<object>));
                    var lambda = Expression.Lambda<Func<Stream, CancellationToken, ValueTask<object>>>(body, param1, param3).Compile();

                    this.DeserializeAsync_Stream_CancellationToken = lambda;
                }
#endif

                {
                    // public static T Deserialize<T>(ReadOnlyMemory<byte> bytes, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(ReadOnlyMemory<byte>), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(ReadOnlyMemory<byte>), "bytes");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param3), typeof(object));
                    var lambda = Expression.Lambda<Func<ReadOnlyMemory<byte>, CancellationToken, object>>(body, param1, param3).Compile();

                    this.Deserialize_ReadOnlyMemory = lambda;
                }

                {
                    // public static T Deserialize<T>(ReadOnlySequence<byte> bytes, JsonSerializerOptions options, CancellationToken cancellationToken)
                    var deserialize = GetMethod(nameof(Deserialize), type, new[] { typeof(ReadOnlySequence<byte>).MakeByRefType(), typeof(CancellationToken) });

                    var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "bytes");
                    var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var body = Expression.Convert(Expression.Call(null, deserialize, param1, param3), typeof(object));
                    var lambda = Expression.Lambda<Func<ReadOnlySequence<byte>, CancellationToken, object>>(body, param1, param3).Compile();

                    this.Deserialize_ReadOnlySequence_CancellationToken = lambda;
                }
            }

            // null is generic type marker.
#if CSHARP_8_OR_NEWER
            private static MethodInfo GetMethod(string methodName, Type type, Type?[] parameters)
#else
            private static MethodInfo GetMethod(string methodName, Type type, Type[] parameters)
#endif
            {
                return typeof(JsonSerializer).GetRuntimeMethods().Single(x =>
                    {
                        if (methodName != x.Name)
                        {
                            return false;
                        }

                        var ps = x.GetParameters();
                        if (ps.Length != parameters.Length)
                        {
                            return false;
                        }

                        for (var i = 0; i < ps.Length; i++)
                        {
                            if (parameters[i] == null && ps[i].ParameterType.IsGenericParameter)
                            {
                                continue;
                            }

                            if (ps[i].ParameterType != parameters[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    })
                    .MakeGenericMethod(type);
            }
        }
    }
}