// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Formatters
{
    internal static class MemberInfoFormatterHelper
    {
        public static (string name, Type declaringType) ReadNameAndDeclaringType(ref JsonReader reader, JsonSerializerOptions options)
        {
            reader.ReadIsBeginObjectWithVerify();
            var count = 0;
            var name = default(string);
            var declaringType = default(Type);
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var propertyName = reader.ReadPropertyNameSegmentRaw();
                switch (propertyName.Length)
                {
                    case 4: // Name
                        if (options.IgnoreNullValues)
                        {
                            if (
                                propertyName[0] != 'N' && propertyName[0] != 'n'
                                || propertyName[1] != 'a' && propertyName[1] != 'A'
                                || propertyName[2] != 'm' && propertyName[2] != 'M'
                                || propertyName[3] != 'e' && propertyName[3] != 'E'
                            )
                                goto default;
                        }
                        else
                        {
                            if (
                                propertyName[0] != 'N'
                                || propertyName[1] != 'a'
                                || propertyName[2] != 'm'
                                || propertyName[3] != 'e'
                            )
                                goto default;
                        }
                        name = reader.ReadString();
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

            if (name == null || declaringType is null)
            {
                throw new NullReferenceException();
            }

            return (name, declaringType);
        }

        public static void SerializeStaticWithoutEndObject(ref JsonWriter writer, MemberInfo value, JsonSerializerOptions options)
        {
            writer.WriteRaw(new[] {
                (byte)'{',
                (byte)'"',
                (byte)'N',
                (byte)'a',
                (byte)'m',
                (byte)'e',
                (byte)'"',
                (byte)':',
            });
            writer.Write(value.Name);
            writer.WriteRaw(new[] {
                (byte)',',
                (byte)'"',
                (byte)'M',
                (byte)'e',
                (byte)'t',
                (byte)'a',
                (byte)'d',
                (byte)'a',
                (byte)'t',
                (byte)'a',
                (byte)'T',
                (byte)'o',
                (byte)'k',
                (byte)'e',
                (byte)'n',
                (byte)'"',
                (byte)':',
            });
            writer.Write(value.MetadataToken);
            writer.WriteRaw(new[] {
                (byte)',',
                (byte)'"',
                (byte)'M',
                (byte)'e',
                (byte)'m',
                (byte)'b',
                (byte)'e',
                (byte)'r',
                (byte)'T',
                (byte)'y',
                (byte)'p',
                (byte)'e',
                (byte)'"',
                (byte)':',
            });
            writer.Write((int)value.MemberType);

            var declaringType = value.DeclaringType;
            if (!options.IgnoreNullValues || declaringType != null)
            {
                writer.WriteRaw(new[] {
                    (byte)',',
                    (byte)'"',
                    (byte)'D',
                    (byte)'e',
                    (byte)'c',
                    (byte)'l',
                    (byte)'a',
                    (byte)'r',
                    (byte)'i',
                    (byte)'n',
                    (byte)'g',
                    (byte)'T',
                    (byte)'y',
                    (byte)'p',
                    (byte)'e',
                    (byte)'"',
                    (byte)':',
                });
                TypeFormatter.SerializeStatic(ref writer, declaringType, options);
            }

            writer.WriteRaw(new[] {
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
            var customAttributes = value.CustomAttributes;
            var enumerator = customAttributes.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END_ARRAY;
                }

                var customAttribute = enumerator.Current;
                CustomAttributeDataFormatter.SerializeStatic(ref writer, customAttribute, options);

                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    customAttribute = enumerator.Current;
                    CustomAttributeDataFormatter.SerializeStatic(ref writer, customAttribute, options);
                }
            }
            finally
            {
                enumerator.Dispose();
            }

        END_ARRAY:
            writer.WriteEndArray();
        }
    }
}
