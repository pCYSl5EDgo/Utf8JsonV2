// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System.Collections.Generic;
using System.Dynamic;

namespace Utf8Json.Formatters
{
    public sealed class ExpandoObjectFormatter : IJsonFormatter<ExpandoObject>
    {
        public void Serialize(ref JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<IDictionary<string, object>>();
            formatter.Serialize(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
            var formatter = options.Resolver.GetFormatterWithVerify<IDictionary<string, object>>();
            formatter.Serialize(ref writer, value, options);
        }

        public static ExpandoObject DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;

            var objectFormatter = options.Resolver.GetFormatterWithVerify<object>();
            var c = 0;
            while (reader.ReadIsInObject(ref c))
            {
                var propName = reader.ReadPropertyName();
                var value = objectFormatter.Deserialize(ref reader, options);
                result.Add(propName, value);
            }

            return (ExpandoObject)result;
        }

        public ExpandoObject Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;

            var objectFormatter = options.Resolver.GetFormatterWithVerify<object>();
            var c = 0;
            while (reader.ReadIsInObject(ref c))
            {
                var propName = reader.ReadPropertyName();
                var value = objectFormatter.Deserialize(ref reader, options);
                result.Add(propName, value);
            }

            return (ExpandoObject)result;
        }
    }
}
#endif
