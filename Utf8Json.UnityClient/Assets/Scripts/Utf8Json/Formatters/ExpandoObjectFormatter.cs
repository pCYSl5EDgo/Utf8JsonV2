// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System.Collections.Generic;
using System.Dynamic;
using StaticFunctionPointerHelper;

namespace Utf8Json.Formatters
{
    public sealed unsafe class ExpandoObjectFormatter : IJsonFormatter<ExpandoObject>
    {
        public void Serialize(ref JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
            var serializer = options.Resolver.GetSerializeStatic<IDictionary<string, object>>();
            if (serializer.ToPointer() == null)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<IDictionary<string, object>>();
                formatter.Serialize(ref writer, value, options);
            }
            else
            {
                writer.Serialize(value, options, serializer);
            }
        }

        public static ExpandoObject DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var result = new ExpandoObject();

            var count = 0;
            var deserializer = options.Resolver.GetDeserializeStatic<object>();
            if (deserializer.ToPointer() == null)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<object>();
                while (reader.ReadIsInObject(ref count))
                {
                    var propName = reader.ReadPropertyName();
                    var value = formatter.Deserialize(ref reader, options);
                    ((IDictionary<string, object>)result).Add(propName, value);
                }
            }
            else
            {
                while (reader.ReadIsInObject(ref count))
                {
                    var propName = reader.ReadPropertyName();
                    var value = reader.Deserialize<object>(options, deserializer);
                    ((IDictionary<string, object>)result).Add(propName, value);
                }
            }

            return result;
        }

        public ExpandoObject Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
#endif
