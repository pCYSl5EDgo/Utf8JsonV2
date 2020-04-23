// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    internal static class ExceptionFormatterHelper
    {
        public static void SerializeStaticWithoutWritingEndObject(ref JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            writer.WriteRaw(new[]
            {
                (byte)'{',
                (byte)'"',
                (byte)'M',
                (byte)'e',
                (byte)'s',
                (byte)'s',
                (byte)'a',
                (byte)'g',
                (byte)'e',
                (byte)'"',
                (byte)':',
            });
            writer.Write(value.Message);
            writer.WriteRaw(new[]
            {
                (byte) ',',
                (byte) '"',
                (byte) 'H',
                (byte) 'R',
                (byte) 'e',
                (byte) 's',
                (byte) 'u',
                (byte) 'l',
                (byte) 't',
                (byte) '"',
                (byte) ':',
            });
            writer.Write(value.HResult);

            var stackTrace = value.StackTrace;
            if (stackTrace != null)
            {
                writer.WriteRaw(new[]{
                    (byte) ',',
                    (byte) '"',
                    (byte) 'S',
                    (byte) 't',
                    (byte) 'a',
                    (byte) 'c',
                    (byte) 'k',
                    (byte) 'T',
                    (byte) 'r',
                    (byte) 'a',
                    (byte) 'c',
                    (byte) 'e',
                    (byte) '"',
                    (byte) ':',
                });
                writer.Write(stackTrace);
            }

            var helpLink = value.HelpLink;
            if (helpLink != null)
            {
                writer.WriteRaw(new[]
                {
                    (byte) ',',
                    (byte) '"',
                    (byte) 'H',
                    (byte) 'e',
                    (byte) 'l',
                    (byte) 'p',
                    (byte) 'L',
                    (byte) 'i',
                    (byte) 'n',
                    (byte) 'k',
                    (byte) '"',
                    (byte) ':',
                });
                writer.Write(helpLink);
            }

            var source = value.Source;
            if (source != null)
            {
                writer.WriteRaw(new[]
                {
                    (byte) ',',
                    (byte) '"',
                    (byte) 'S',
                    (byte) 'o',
                    (byte) 'u',
                    (byte) 'r',
                    (byte) 'c',
                    (byte) 'e',
                    (byte) '"',
                    (byte) ':',
                });
                writer.Write(source);
            }

            var targetSite = value.TargetSite;
            if (targetSite != null)
            {
                writer.WriteRaw(new[]
                {
                    (byte) ',',
                    (byte) '"',
                    (byte) 'T',
                    (byte) 'a',
                    (byte) 'r',
                    (byte) 'g',
                    (byte) 'e',
                    (byte) 't',
                    (byte) 'S',
                    (byte) 'i',
                    (byte) 't',
                    (byte) 'e',
                    (byte) '"',
                    (byte) ':',
                });
                MethodBaseFormatter.SerializeStatic(ref writer, targetSite, options);
            }

            var innerException = value.InnerException;
            if (innerException == null) return;
            writer.WriteRaw(new[]
            {
                (byte) ',',
                (byte) '"',
                (byte) 'I',
                (byte) 'n',
                (byte) 'n',
                (byte) 'e',
                (byte) 'r',
                (byte) 'E',
                (byte) 'x',
                (byte) 'c',
                (byte) 'e',
                (byte) 'p',
                (byte) 't',
                (byte) 'i',
                (byte) 'o',
                (byte) 'n',
                (byte) '"',
                (byte) ':',
            });
            SerializeStaticWithoutWritingEndObject(ref writer, innerException, options);
            writer.WriteEndObject();
        }
    }
}
