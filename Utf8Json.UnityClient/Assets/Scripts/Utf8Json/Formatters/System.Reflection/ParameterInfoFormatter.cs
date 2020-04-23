// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class ParameterInfoFormatter
#if CSHARP_8_OR_NEWER
    : IJsonFormatter<ParameterInfo?>
#else
    : IJsonFormatter<ParameterInfo>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ParameterInfo, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ParameterInfo? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ParameterInfo value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public ParameterInfo? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public ParameterInfo Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ParameterInfo? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ParameterInfo value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteRaw(new[]
            {
                (byte)'{',
                (byte)'"',
                (byte)'P',
                (byte)'a',
                (byte)'r',
                (byte)'a',
                (byte)'m',
                (byte)'e',
                (byte)'t',
                (byte)'e',
                (byte)'r',
                (byte)'T',
                (byte)'y',
                (byte)'p',
                (byte)'e',
                (byte)'"',
                (byte)':',
            });

            TypeFormatter.SerializeStatic(ref writer, value.ParameterType, options);

            var enumerator = value.CustomAttributes.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END;
                }

                writer.WriteRaw(new[]
                {
                    (byte)',',
                    (byte)'"',
                    (byte)'C',
                    (byte)'u',
                    (byte)'s',
                    (byte)'t',
                    (byte)'o',
                    (byte)'m',
                    (byte)'A',
                    (byte)'t',
                    (byte)'t',
                    (byte)'r',
                    (byte)'i',
                    (byte)'b',
                    (byte)'u',
                    (byte)'t',
                    (byte)'e',
                    (byte)'s',
                    (byte)'"',
                    (byte)':',
                    (byte)'[',
                });
                CustomAttributeDataFormatter.SerializeStatic(ref writer, enumerator.Current, options);

                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    CustomAttributeDataFormatter.SerializeStatic(ref writer, enumerator.Current, options);
                }

                writer.WriteEndArray();
            }
            finally
            {
                enumerator.Dispose();
            }

        END:
            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        public static ParameterInfo? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static ParameterInfo DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.ReadNextBlock();
            return default;
        }

#if CSHARP_8_OR_NEWER
        public static (Type? parameterType, CustomAttributeData[] customAttributeDataArray) AlmostDeserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static (Type parameterType, CustomAttributeData[] customAttributeDataArray) AlmostDeserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            reader.ReadIsBeginObjectWithVerify();
            var count = 0;
            var parameterType = default(Type);
            var customAttributes = default(CustomAttributeData[]);
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var propertyName = reader.ReadPropertyNameSegmentRaw();
                switch (propertyName.Length)
                {
                    case 13: // ParameterType
                        if (
                            propertyName[0] != (byte)'P'
                            && propertyName[1] != (byte)'a'
                            && propertyName[2] != (byte)'r'
                            && propertyName[3] != (byte)'a'
                            && propertyName[4] != (byte)'m'
                            && propertyName[5] != (byte)'e'
                            && propertyName[6] != (byte)'t'
                            && propertyName[7] != (byte)'e'
                            && propertyName[8] != (byte)'r'
                            && propertyName[9] != (byte)'T'
                            && propertyName[10] != (byte)'y'
                            && propertyName[11] != (byte)'p'
                            && propertyName[12] != (byte)'e'
                        )
                            goto default;
                        parameterType = TypeFormatter.DeserializeStatic(ref reader, options);
                        break;
                    case 16: // CustomAttributes
                        if (
                            propertyName[0] != (byte)'C'
                            && propertyName[1] != (byte)'u'
                            && propertyName[2] != (byte)'s'
                            && propertyName[3] != (byte)'t'
                            && propertyName[4] != (byte)'o'
                            && propertyName[5] != (byte)'m'
                            && propertyName[6] != (byte)'A'
                            && propertyName[7] != (byte)'t'
                            && propertyName[8] != (byte)'t'
                            && propertyName[9] != (byte)'r'
                            && propertyName[10] != (byte)'i'
                            && propertyName[11] != (byte)'b'
                            && propertyName[12] != (byte)'u'
                            && propertyName[13] != (byte)'t'
                            && propertyName[14] != (byte)'e'
                            && propertyName[15] != (byte)'s'
                        )
                            goto default;
                        customAttributes = ArrayFormatter<CustomAttributeData>.DeserializeStatic(ref reader, options);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            return (parameterType, customAttributes ?? Array.Empty<CustomAttributeData>());
        }

        public static (Type[], int) AlmostDeserializeArray(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return (Array.Empty<Type>(), -1);
            }

            reader.ReadIsBeginArrayWithVerify();
            var array = ArrayPool<Type>.Shared.Rent(32);
            var count = 0;
            while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                if (array.Length < count)
                {
                    var tmp = ArrayPool<Type>.Shared.Rent(count << 1);
                    Array.Copy(array, tmp, array.Length);
                    ArrayPool<Type>.Shared.Return(array);
                    array = tmp;
                }

                var (parameterType, _) = AlmostDeserialize(ref reader, options);
                array[count - 1] = parameterType ?? throw new NullReferenceException();
            }

            return (array, count);
        }
    }
}
