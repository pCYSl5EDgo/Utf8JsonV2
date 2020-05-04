// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class ConstructorInfoFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<ConstructorInfo?>
#else
        : IJsonFormatter<ConstructorInfo>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as ConstructorInfo, options);
        }

        public object
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, ConstructorInfo? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, ConstructorInfo value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyObject();
                return;
            }

            ++writer.Depth;
            MethodBaseFormatterHelper.SerializeStaticWithoutEndObject(ref writer, value, options);
            writer.WriteEndObject();
            --writer.Depth;
        }

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, ConstructorInfo? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, ConstructorInfo value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

        public ConstructorInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static ConstructorInfo
#if CSHARP_8_OR_NEWER
            ?
#endif
            DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                return default;
            }

            reader.ReadIsBeginObjectWithVerify();
            var count = 0;
            var declaringType = default(Type);
            var isStatic = false;
            var isPublic = false;
            var parameters = default(Type[]);
            var parameterCount = -1;
            try
            {
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var propertyName = reader.ReadPropertyNameSegmentRaw();
                    switch (propertyName.Length)
                    {
                        case 8: // isStatic, isPublic
                            if (options.IgnoreNullValues)
                            {
                                if (
                                       propertyName[0] != 'I' && propertyName[0] != 'i'
                                    || propertyName[1] != 's' && propertyName[1] != 'S'
                                    || propertyName[6] != 'i' && propertyName[6] != 'I'
                                    || propertyName[7] != 'c' && propertyName[7] != 'C'
                                )
                                    goto default;
                                if (propertyName[2] == 'S' || propertyName[2] == 's')
                                {
                                    if (
                                           propertyName[3] != 't' && propertyName[3] != 'T'
                                        || propertyName[4] != 'a' && propertyName[4] != 'A'
                                        || propertyName[5] != 't' && propertyName[5] != 'T'
                                    )
                                        goto default;
                                    isStatic = reader.ReadBoolean();
                                }
                                else if (propertyName[2] == 'P' || propertyName[2] == 'p')
                                {
                                    if (
                                           propertyName[3] != 'u' && propertyName[3] != 'U'
                                        || propertyName[4] != 'b' && propertyName[4] != 'B'
                                        || propertyName[5] != 'l' && propertyName[5] != 'L'
                                    )
                                        goto default;
                                    isPublic = reader.ReadBoolean();
                                }
                                else
                                {
                                    goto default;
                                }
                            }
                            else
                            {
                                if (
                                       propertyName[0] != 'I'
                                    || propertyName[1] != 's'
                                    || propertyName[6] != 'i'
                                    || propertyName[7] != 'c'
                                )
                                    goto default;
                                if (propertyName[2] == 'S')
                                {
                                    if (
                                        propertyName[3] != 't'
                                     || propertyName[4] != 'a'
                                     || propertyName[5] != 't'
                                    )
                                        goto default;
                                    isStatic = reader.ReadBoolean();
                                }
                                else if (propertyName[2] == 'P')
                                {
                                    if (
                                           propertyName[3] != 'u'
                                        || propertyName[4] != 'b'
                                        || propertyName[5] != 'l'
                                    )
                                        goto default;
                                    isPublic = reader.ReadBoolean();
                                }
                                else
                                {
                                    goto default;
                                }
                            }
                            break;
                        case 10: // Parameters
                            if (options.IgnoreNullValues)
                            {
                                if (
                                       propertyName[0] != 'P' && propertyName[0] != 'p'
                                    || propertyName[1] != 'a' && propertyName[1] != 'A'
                                    || propertyName[2] != 'r' && propertyName[2] != 'R'
                                    || propertyName[3] != 'a' && propertyName[3] != 'A'
                                    || propertyName[4] != 'm' && propertyName[4] != 'M'
                                    || propertyName[5] != 'e' && propertyName[5] != 'E'
                                    || propertyName[6] != 't' && propertyName[6] != 'T'
                                    || propertyName[7] != 'e' && propertyName[7] != 'E'
                                    || propertyName[8] != 'r' && propertyName[8] != 'R'
                                    || propertyName[9] != 's' && propertyName[9] != 'S'
                                )
                                    goto default;
                            }
                            else
                            {
                                if (
                                       propertyName[0] != 'P'
                                    || propertyName[1] != 'a'
                                    || propertyName[2] != 'r'
                                    || propertyName[3] != 'a'
                                    || propertyName[4] != 'm'
                                    || propertyName[5] != 'e'
                                    || propertyName[6] != 't'
                                    || propertyName[7] != 'e'
                                    || propertyName[8] != 'r'
                                    || propertyName[9] != 's'
                                )
                                    goto default;
                            }
                            (parameters, parameterCount) = ParameterInfoFormatter.AlmostDeserializeArray(ref reader, options);
                            break;
                        case 13: // DeclaringType
                            if (options.IgnoreNullValues)
                            {
                                if (
                                       propertyName[0] != 'D' && propertyName[0] != 'd'
                                    || propertyName[1] != 'e' && propertyName[1] != 'E'
                                    || propertyName[2] != 'c' && propertyName[2] != 'C'
                                    || propertyName[3] != 'l' && propertyName[3] != 'L'
                                    || propertyName[4] != 'a' && propertyName[4] != 'A'
                                    || propertyName[5] != 'r' && propertyName[5] != 'R'
                                    || propertyName[6] != 'i' && propertyName[6] != 'I'
                                    || propertyName[7] != 'n' && propertyName[7] != 'N'
                                    || propertyName[8] != 'g' && propertyName[8] != 'G'
                                    || propertyName[9] != 'T' && propertyName[9] != 't'
                                    || propertyName[10] != 'y' && propertyName[10] != 'Y'
                                    || propertyName[11] != 'p' && propertyName[11] != 'P'
                                    || propertyName[12] != 'e' && propertyName[12] != 'E'
                                )
                                    goto default;
                            }
                            else
                            {
                                if (
                                       propertyName[0] != 'D'
                                    || propertyName[1] != 'e'
                                    || propertyName[2] != 'c'
                                    || propertyName[3] != 'l'
                                    || propertyName[4] != 'a'
                                    || propertyName[5] != 'r'
                                    || propertyName[6] != 'i'
                                    || propertyName[7] != 'n'
                                    || propertyName[8] != 'g'
                                    || propertyName[9] != 'T'
                                    || propertyName[10] != 'y'
                                    || propertyName[11] != 'p'
                                    || propertyName[12] != 'e'
                                )
                                    goto default;
                            }
                            declaringType = TypeFormatter.DeserializeStatic(ref reader, options);
                            break;
                        default:
                            reader.ReadNextBlock();
                            break;
                    }
                }
                if (declaringType is null || parameters is null)
                {
                    throw new NullReferenceException();
                }

                var flag = isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
                flag |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
                var parameterArray = parameterCount == -1
                    ? Array.Empty<Type>()
                    : parameterCount == parameters.Length
                        ? parameters
                        : parameters.AsSpan(0, parameterCount).ToArray();
                var answer = declaringType.GetConstructor(flag, null, parameterArray, null);
                return answer;
            }
            finally
            {
                if (parameterCount != -1 && parameters != null)
                {
                    ArrayPool<Type>.Shared.Return(parameters);
                }
            }
        }
    }
}
