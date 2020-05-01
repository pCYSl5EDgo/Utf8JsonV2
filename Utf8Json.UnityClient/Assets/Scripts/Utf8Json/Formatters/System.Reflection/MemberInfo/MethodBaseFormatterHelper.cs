// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Utf8Json.Formatters
{
    internal static class MethodBaseFormatterHelper
    {
        public static void SerializeStaticWithoutEndObject(ref JsonWriter writer, MethodBase value, JsonSerializerOptions options)
        {
            MemberInfoFormatterHelper.SerializeStaticWithoutEndObject(ref writer, value, options);

            writer.WriteRaw(new[]
            {
                (byte) ',',
                (byte) '"',
                (byte) 'I',
                (byte) 's',
                (byte) 'S',
                (byte) 't',
                (byte) 'a',
                (byte) 't',
                (byte) 'i',
                (byte) 'c',
                (byte) '"',
                (byte) ':',
            });
            writer.Write(value.IsStatic);

            writer.WriteRaw(new[]
            {
                (byte) ',',
                (byte) '"',
                (byte) 'I',
                (byte) 's',
                (byte) 'P',
                (byte) 'u',
                (byte) 'b',
                (byte) 'l',
                (byte) 'i',
                (byte) 'c',
                (byte) '"',
                (byte) ':',
            });
            writer.Write(value.IsStatic);

            var parameters = value.GetParameters();
            writer.Writer.Write(new[]
            {
                (byte) ',',
                (byte) '"',
                (byte) 'P',
                (byte) 'a',
                (byte) 'r',
                (byte) 'a',
                (byte) 'm',
                (byte) 'e',
                (byte) 't',
                (byte) 'e',
                (byte) 'r',
                (byte) 's',
                (byte) '"',
                (byte) ':',
                (byte) '[',
            });

            if (parameters.Length != 0)
            {
                ParameterInfoFormatter.SerializeStatic(ref writer, parameters[0], options);
                for (var index = 1; index < parameters.Length; index++)
                {
                    writer.WriteValueSeparator();
                    ParameterInfoFormatter.SerializeStatic(ref writer, parameters[index], options);
                }
            }

            writer.WriteEndArray();
        }
    }
}
