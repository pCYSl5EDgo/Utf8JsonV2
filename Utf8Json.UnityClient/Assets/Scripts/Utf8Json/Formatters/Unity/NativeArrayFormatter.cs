// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_2018_4_OR_NEWER

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

#if !ENABLE_IL2CPP
using StaticFunctionPointerHelper;
#endif

namespace Utf8Json.Formatters
{
    public sealed class NativeArrayFormatter<T>
        : IJsonFormatter<NativeArray<T>>
        where T : unmanaged
    {
        public void Serialize(ref JsonWriter writer, NativeArray<T> value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public NativeArray<T> Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, NativeArray<T> value, JsonSerializerOptions options)
        {
            if (!value.IsCreated || value.Length == 0 || writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyArray();
                return;
            }

            writer.Depth++;
            writer.WriteBeginArray();
#if !ENABLE_IL2CPP
            var serializer = options.Resolver.GetSerializeStatic<T>();
            unsafe
            {
                if (serializer.ToPointer() != null)
                {
                    writer.Serialize(value[0], options, serializer);

                    for (var index = 1; index < value.Length; index++)
                    {
                        writer.WriteValueSeparator();
                        writer.Serialize(value[index], options, serializer);
                    }
                    goto END;
                }
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();

            formatter.Serialize(ref writer, value[0], options);

            for (var index = 1; index < value.Length; index++)
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, value[index], options);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            writer.WriteEndArray();
            writer.Depth--;
        }

        public static unsafe NativeArray<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var capacity = 32;
            var count = 0;
            var ptr = (T*)UnsafeUtility.Malloc(sizeof(T) << 5, 4, Allocator.Temp);
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() != null)
            {
                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    if (capacity < count)
                    {
                        var oldSize = sizeof(T) * capacity;
                        capacity = count * 2;
                        var size = sizeof(T) * capacity;
                        var tmp = (T*)UnsafeUtility.Malloc(size, 4, size < 1024 ? Allocator.Temp : Allocator.Persistent);
                        UnsafeUtility.MemCpy(tmp, ptr, oldSize);
                        UnsafeUtility.Free(ptr, oldSize < 1024 ? Allocator.Temp : Allocator.Persistent);
                        ptr = tmp;
                    }

                    ptr[count - 1] = reader.Deserialize<T>(options, deserializer);
                }
                goto END;
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (capacity < count)
                {
                    var oldSize = sizeof(T) * capacity;
                    capacity = count * 2;
                    var size = sizeof(T) * capacity;
                    var tmp = (T*)UnsafeUtility.Malloc(size, 4, size < 1024 ? Allocator.Temp : Allocator.Persistent);
                    UnsafeUtility.MemCpy(tmp, ptr, oldSize);
                    UnsafeUtility.Free(ptr, oldSize < 1024 ? Allocator.Temp : Allocator.Persistent);
                    ptr = tmp;
                }

                ptr[count - 1] = formatter.Deserialize(ref reader, options);
            }
#if !ENABLE_IL2CPP
        END:
#endif
            var answer = new NativeArray<T>(count, Allocator.Persistent);
            UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer), ptr, sizeof(T) * count);
            UnsafeUtility.Free(ptr, sizeof(T) * capacity < 1024 ? Allocator.Temp : Allocator.Persistent);
            return answer;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is NativeArray<T> innerValue))
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
#endif
