// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if UNITY_2018_4_OR_NEWER

using System;
using StaticFunctionPointerHelper;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Utf8Json.Formatters
{
    public sealed unsafe class NativeArrayFormatter<T>
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
            if (!value.IsCreated || value.Length == 0)
            {
                var span = writer.Writer.GetSpan(2);
                span[0] = (byte)'[';
                span[1] = (byte)']';
                writer.Writer.Advance(2);
                return;
            }

            writer.WriteBeginArray();

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                SerializeStaticWithFormatter(ref writer, value, options);
            }
            else
            {
                writer.Serialize(value[0], options, serializer);

                for (var index = 1; index < value.Length; index++)
                {
                    writer.WriteValueSeparator();
                    writer.Serialize(value[index], options, serializer);
                }
            }

            writer.WriteEndArray();
        }

        private static void SerializeStaticWithFormatter(ref JsonWriter writer, NativeArray<T> value, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();

            formatter.Serialize(ref writer, value[0], options);

            for (var index = 1; index < value.Length; index++)
            {
                writer.WriteValueSeparator();
                formatter.Serialize(ref writer, value[index], options);
            }
        }

        public static NativeArray<T> DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginArrayWithVerify();
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            if (deserializer.ToPointer() == null)
            {
                return DeserializeStaticWithFormatter(ref reader, options);
            }

            var ptr = (T*)UnsafeUtility.Malloc(sizeof(T) << 5, 4, Allocator.Temp);
            var capacity = 32;
            var count = 0;
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

            var answer = new NativeArray<T>(count, Allocator.Persistent);
            UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer), ptr, sizeof(T) * count);
            UnsafeUtility.Free(ptr, sizeof(T) * capacity < 1024 ? Allocator.Temp : Allocator.Persistent);
            return answer;
        }

        private static NativeArray<T> DeserializeStaticWithFormatter(ref JsonReader reader, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var ptr = (T*)UnsafeUtility.Malloc(sizeof(T) << 5, 4, Allocator.Temp);
            var capacity = 32;
            var count = 0;
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

            var answer = new NativeArray<T>(count, Allocator.Persistent);
            UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer), ptr, sizeof(T) * count);
            UnsafeUtility.Free(ptr, sizeof(T) * capacity < 1024 ? Allocator.Temp : Allocator.Persistent);
            return answer;
        }
    }
}
#endif
