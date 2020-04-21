// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace Utf8Json
{
    public static class JsonSerializerOptionsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeWithVerify<T>(this JsonSerializerOptions options, ref JsonWriter writer, in T value)
        {
#if !ENABLE_IL2CPP
            var serializer = options.Resolver.GetSerializeStatic<T>();
            unsafe
            {
                if (serializer.ToPointer() != null)
                {
                    StaticFunctionPointerHelper.CallHelper.Serialize(ref writer, value, options, serializer);
                    return;
                }
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeWithVerify<T>(this JsonSerializerOptions options, ref JsonReader reader)
        {
#if !ENABLE_IL2CPP
            var deserializer = options.Resolver.GetDeserializeStatic<T>();
            unsafe
            {
                if (deserializer.ToPointer() != null)
                {
                    return StaticFunctionPointerHelper.CallHelper.Deserialize<T>(ref reader, options, deserializer);
                }
            }
#endif
            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            return formatter.Deserialize(ref reader, options);
        }
    }
}
