// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Formatters
{
    internal static class ExceptionFormatterHelper
    {
        public static readonly byte[] BytesMessage = {
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
        };

        public static readonly byte[] BytesHResult = {
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
        };

        public static readonly byte[] BytesStackTrace = {
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
        };

        public static readonly byte[] BytesHelpLink = {
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
        };

        public static readonly byte[] BytesSource = {
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
        };

        public static readonly byte[] BytesTargetSite = {
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
        };
        
        public static readonly byte[] BytesInnerException = {
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
        };

        public static void SerializeStaticWithoutWritingEndObject(ref JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            writer.WriteRaw(BytesMessage);
            writer.Write(value.Message);
            writer.WriteRaw(BytesHResult);
            writer.Write(value.HResult);

            var stackTrace = value.StackTrace;
            if (stackTrace != null)
            {
                writer.WriteRaw(BytesStackTrace);
                writer.Write(stackTrace);
            }

            var helpLink = value.HelpLink;
            if (helpLink != null)
            {
                writer.WriteRaw(BytesHelpLink);
                writer.Write(helpLink);
            }

            var source = value.Source;
            if (source != null)
            {
                writer.WriteRaw(BytesSource);
                writer.Write(source);
            }

            var targetSite = value.TargetSite;
            if (targetSite != null)
            {
                writer.WriteRaw(BytesTargetSite);
                MethodBaseFormatter.SerializeStatic(ref writer, targetSite, options);
            }

            var innerException = value.InnerException;
            if (innerException == null)
            {
                return;
            }

            writer.WriteRaw(BytesInnerException);
            SerializeStaticWithoutWritingEndObject(ref writer, innerException, options);
            writer.WriteEndObject();
        }
    }
}
