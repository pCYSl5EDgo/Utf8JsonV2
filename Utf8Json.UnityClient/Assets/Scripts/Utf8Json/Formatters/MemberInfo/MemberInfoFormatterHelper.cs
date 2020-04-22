// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Formatters
{
    internal static class MemberInfoFormatterHelper
    {
        public static readonly byte[] BytesName =
        {
            (byte)'{',
            (byte)'"',
            (byte)'N',
            (byte)'a',
            (byte)'m',
            (byte)'e',
            (byte)'"',
            (byte)':',
        };

        public static readonly byte[] BytesMemberType =
        {
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
        };

        public static readonly byte[] BytesMetadataToken =
        {
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
        };

        public static readonly byte[] BytesDeclaringType =
        {
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
        };

        public static readonly byte[] BytesCustomAttributes =
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
        };

        public static void SerializeStaticWithoutEndObject(ref JsonWriter writer, MemberInfo value, JsonSerializerOptions options)
        {
            writer.WriteRaw(BytesName);
            writer.Write(value.Name);
            writer.WriteRaw(BytesMetadataToken);
            writer.Write(value.MetadataToken);
            writer.WriteRaw(BytesMemberType);
            writer.Write((int)value.MemberType);

            var declaringType = value.DeclaringType;
            if (!options.IgnoreNullValues || declaringType != null)
            {
                writer.WriteRaw(BytesDeclaringType);
                TypeFormatter.SerializeStatic(ref writer, declaringType, options);
            }

            writer.WriteRaw(BytesCustomAttributes);
            var customAttributes = value.CustomAttributes;
            var enumerator = customAttributes.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    goto END_ARRAY;
                }

                var customAttribute = enumerator.Current;
                options.SerializeWithVerify(ref writer, customAttribute);

                while (enumerator.MoveNext())
                {
                    writer.WriteValueSeparator();
                    customAttribute = enumerator.Current;
                    options.SerializeWithVerify(ref writer, customAttribute);
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
