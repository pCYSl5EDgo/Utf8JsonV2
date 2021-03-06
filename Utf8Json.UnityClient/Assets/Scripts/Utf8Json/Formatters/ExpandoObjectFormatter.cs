// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using StaticFunctionPointerHelper;
using System.Collections.Generic;
using System.Dynamic;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ExpandoObjectFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ExpandoObject?>
#else
        : IJsonFormatter<ExpandoObject>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ExpandoObject? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ExpandoObject? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
#endif
        {
            InterfaceDictionaryFormatter<string, object>.SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static ExpandoObject? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ExpandoObject DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            var result = new ExpandoObject();

            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<object>();
            if (deserializer.ToPointer() != null)
            {
                while (reader.ReadIsInObject(ref count))
                {
                    var propName = reader.ReadPropertyName();
                    var value = reader.Deserialize<object>(options, deserializer);
                    ((IDictionary<string, object>)result).Add(propName, value);
                }
            }
            else
            {
                var formatter = options.Resolver.GetFormatterWithVerify<object>();
                while (reader.ReadIsInObject(ref count))
                {
                    var propName = reader.ReadPropertyName();
                    var value = formatter.Deserialize(ref reader, options);
                    ((IDictionary<string, object>)result).Add(propName, value);
                }
            }

            return result;
        }

#if CSHARP_8_OR_NEWER
        public ExpandoObject? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ExpandoObject Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ExpandoObject, options);
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
#endif
