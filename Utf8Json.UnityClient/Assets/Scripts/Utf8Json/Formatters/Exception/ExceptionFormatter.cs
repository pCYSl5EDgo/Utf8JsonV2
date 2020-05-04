// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Formatters
{
    public sealed class ExceptionFormatter : IJsonFormatter<Exception>
    {
        public Exception Deserialize(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public static Exception DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.ReadIsNull())
            {
                throw new NullReferenceException();
            }

            reader.ReadIsBeginObjectWithVerify();
            var ignoreCase = options.IgnoreCase;

            var message = default(string);
            var source = default(string);
            var helpLink = default(string);
            var innerException = default(Exception);

            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var name = reader.ReadPropertyNameSegmentRaw();
                switch (name.Length)
                {
                    case 6: // Source
                        if (ignoreCase)
                        {
                            if (name[0] != 'S' && name[0] != 's'
                                || name[1] != 'o' && name[1] != 'O'
                                || name[2] != 'u' && name[2] != 'U'
                                || name[3] != 'r' && name[3] != 'R'
                                || name[4] != 'c' && name[4] != 'C'
                                || name[5] != 'e' && name[5] != 'E')
                            {
                                goto default;
                            }
                        }
                        else
                        {
                            if (name[0] != 'S'
                                || name[1] != 'o'
                                || name[2] != 'u'
                                || name[3] != 'r'
                                || name[4] != 'c'
                                || name[5] != 'e')
                            {
                                goto default;
                            }
                        }

                        source = reader.ReadString();
                        break;
                    case 7: // Message, HResult
                        if (ignoreCase)
                        {
                            switch (name[0])
                            {
                                case (byte)'M':
                                case (byte)'m':
                                    if (name[1] != 'e' && name[1] != 'E'
                                        || name[2] != 's' && name[2] != 'S'
                                        || name[3] != 's' && name[3] != 'S'
                                        || name[4] != 'a' && name[4] != 'A'
                                        || name[5] != 'g' && name[5] != 'G'
                                        || name[6] != 'e' && name[6] != 'E'
                                    )
                                    {
                                        goto SKIP;
                                    }
                                    break;
                                default:
                                    goto SKIP;
                            }
                        }
                        else
                        {
                            switch (name[0])
                            {
                                case (byte)'M':
                                    if (name[1] != 'e'
                                        || name[2] != 's'
                                        || name[3] != 's'
                                        || name[4] != 'a'
                                        || name[5] != 'g'
                                        || name[6] != 'e'
                                    )
                                    {
                                        goto SKIP;
                                    }
                                    break;
                                default:
                                    goto SKIP;
                            }
                        }

                        message = reader.ReadString();
                        break;
                    case 8: // HelpLink
                        if (ignoreCase)
                        {
                            if (name[0] != 'H' && name[0] != 'h'
                                || name[1] != 'e' && name[1] != 'E'
                                || name[2] != 'l' && name[2] != 'L'
                                || name[3] != 'p' && name[3] != 'P'
                                || name[4] != 'L' && name[4] != 'l'
                                || name[5] != 'i' && name[5] != 'I'
                                || name[6] != 'n' && name[6] != 'N'
                                || name[7] != 'k' && name[7] != 'K'
                            )
                            {
                                goto default;
                            }
                        }
                        else
                        {
                            if (name[0] != 'H'
                                || name[1] != 'e'
                                || name[2] != 'l'
                                || name[3] != 'p'
                                || name[4] != 'L'
                                || name[5] != 'i'
                                || name[6] != 'n'
                                || name[7] != 'k'
                            )
                            {
                                goto default;
                            }
                        }

                        helpLink = reader.ReadString();
                        break;
                    case 14: // InnerException
                        if (ignoreCase)
                        {
                            if (name[0] != 'I' && name[0] != 'i'
                                || name[1] != 'n' && name[1] != 'N'
                                || name[2] != 'n' && name[2] != 'N'
                                || name[3] != 'e' && name[3] != 'E'
                                || name[4] != 'r' && name[4] != 'R'
                                || name[5] != 'E' && name[5] != 'e'
                                || name[6] != 'x' && name[6] != 'X'
                                || name[7] != 'c' && name[7] != 'C'
                                || name[8] != 'e' && name[8] != 'E'
                                || name[9] != 'p' && name[9] != 'P'
                                || name[10] != 't' && name[10] != 'T'
                                || name[11] != 'i' && name[11] != 'I'
                                || name[12] != 'o' && name[12] != 'O'
                                || name[13] != 'n' && name[13] != 'N'
                            )
                            {
                                goto default;
                            }
                        }
                        else
                        {
                            if (name[0] != 'I'
                                || name[1] != 'n'
                                || name[2] != 'n'
                                || name[3] != 'e'
                                || name[4] != 'r'
                                || name[5] != 'E'
                                || name[6] != 'x'
                                || name[7] != 'c'
                                || name[8] != 'e'
                                || name[9] != 'p'
                                || name[10] != 't'
                                || name[11] != 'i'
                                || name[12] != 'o'
                                || name[13] != 'n'
                            )
                            {
                                goto default;
                            }
                        }

                        innerException = reader.ReadIsNull() ? default : DeserializeStatic(ref reader, options);
                        break;
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 9:
                    case 10: // StackTrace, TargetSite
                    case 11:
                    case 12:
                    case 13:
                    default:
                    SKIP:
                        reader.ReadNextBlock();
                        break;
                }
            }

            var answer = new Exception(message, innerException)
            {
                HelpLink = helpLink,
                Source = source,
            };
            return answer;
        }

        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
        {
            return DeserializeStatic(ref reader, options);
        }

        public void Serialize(ref JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            SerializeStatic(ref writer, value, options);
        }

        public static void SerializeStatic(ref JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            if (writer.Depth >= options.MaxDepth)
            {
                writer.Writer.WriteEmptyObject();
                return;
            }

            ++writer.Depth;
            ExceptionFormatterHelper.SerializeStaticWithoutWritingEndObject(ref writer, value, options);
            writer.WriteEndObject();
            --writer.Depth;
        }

#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (!(value is Exception innerValue))
            {
                throw new ArgumentNullException(nameof(value));
            }

            SerializeStatic(ref writer, innerValue, options);
        }
    }
}
