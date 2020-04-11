// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json
{
    public ref struct JsonReader
    {
        private const byte Bom0 = 0xEF;
        private const byte Bom1 = 0xBB;
        private const byte Bom2 = 0xBF;
        private const string ExpectedFirst = "expected:'";
        private const string ExpectedLast = "'";

        /// <summary>
        /// The reader over the sequence.
        /// </summary>
        internal SequenceReader<byte> Reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> struct.
        /// </summary>
        /// <param name="memory">The buffer to read from.</param>
        public JsonReader(ReadOnlyMemory<byte> memory)
        {
            if (memory.Length > 3)
            {
                var span = memory.Span;
                if (span[0] == Bom0 && span[1] == Bom1 && span[2] == Bom2)
                {
                    memory = memory.Slice(3);
                }
            }

            this.Reader = new SequenceReader<byte>(new ReadOnlySequence<byte>(memory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> struct.
        /// </summary>
        /// <param name="readOnlySequence">The sequence to read from.</param>
        public JsonReader(in ReadOnlySequence<byte> readOnlySequence)
        {
            if (readOnlySequence.Length < 3)
            {
                goto DEFAULT;
            }

            var span = readOnlySequence.First.Span;
            if (span.Length >= 3)
            {
                if (span[0] == Bom0 && span[1] == Bom1 && span[2] == Bom2)
                {
                    this.Reader = new SequenceReader<byte>(readOnlySequence.Slice(3));
                    return;
                }

                goto DEFAULT;
            }

            if (readOnlySequence.IsSingleSegment)
            {
                goto DEFAULT;
            }

            Span<byte> header = stackalloc byte[3];
            readOnlySequence.CopyTo(header);

            if (header[0] == Bom0 && header[1] == Bom1 && header[2] == Bom2)
            {
                this.Reader = new SequenceReader<byte>(readOnlySequence.Slice(3));
                return;
            }

        DEFAULT:
            this.Reader = new SequenceReader<byte>(readOnlySequence);
        }

        /// <summary>
        /// Gets or sets the cancellation token for this deserialization operation.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets the <see cref="ReadOnlySequence{T}"/> originally supplied to the constructor.
        /// </summary>
        public ReadOnlySequence<byte> Sequence => this.Reader.Sequence;

        /// <summary>
        /// Gets the current position of the reader within <see cref="Sequence"/>.
        /// </summary>
        public SequencePosition Position => this.Reader.Position;

        /// <summary>
        /// Gets the number of bytes consumed by the reader.
        /// </summary>
        public long Consumed => this.Reader.Consumed;

        /// <summary>
        /// Gets a value indicating whether the reader is at the end of the sequence.
        /// </summary>
        public bool End => this.Reader.End;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> struct,
        /// with the same settings as this one, but with its own buffer to read from.
        /// </summary>
        /// <param name="readOnlySequence">The sequence to read from.</param>
        /// <returns>The new reader.</returns>
        public JsonReader Clone(in ReadOnlySequence<byte> readOnlySequence) => new JsonReader(readOnlySequence)
        {
            CancellationToken = this.CancellationToken
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(long length)
        {
            Reader.Advance(length);
        }

        /// <summary>
        /// Advances past consecutive spaces.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipWhiteSpace()
        {
            const byte space = 0x20;
            const byte tab = 0x09;
            const byte lineFeed = 0x0a;
            const byte carriageReturn = 0x0d;

            Reader.AdvancePastAny(space, tab, lineFeed, carriageReturn);
        }

        private static readonly byte[] nullBytesSkipFirst = { (byte)'u', (byte)'l', (byte)'l' };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsNull()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != 'n')
            {
                return false;
            }

            Advance(1);

            if (Reader.IsNext(nullBytesSkipFirst, true))
            {
                return true;
            }

            throw new JsonParsingException(ExpectedFirst + "null" + ExpectedLast);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsBeginArray()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != '[')
            {
                return false;
            }

            Advance(1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadIsBeginArrayWithVerify()
        {
            if (!ReadIsBeginArray())
            {
                throw new JsonParsingException(ExpectedFirst + "[" + ExpectedLast);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsEndArray()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != ']')
            {
                return false;
            }

            Advance(1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadIsEndArrayWithVerify()
        {
            if (!ReadIsEndArray())
            {
                throw new JsonParsingException(ExpectedFirst + "]" + ExpectedLast);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsValueSeparator()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != ',')
            {
                return false;
            }

            Advance(1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadIsValueSeparatorWithVerify()
        {
            if (!ReadIsValueSeparator())
            {
                throw new JsonParsingException(ExpectedFirst + "," + ExpectedLast);
            }
        }

        public bool ReadIsEndArrayWithSkipValueSeparator(ref int count)
        {
            SkipWhiteSpace();

            if (Reader.TryPeek(out var b) && b == ']')
            {
                Advance(1);
                return true;
            }

            if (count++ != 0)
            {
                ReadIsValueSeparatorWithVerify();
            }

            return false;
        }

        /// <summary>
        /// Convenient pattern of ReadIsBeginArrayWithVerify + while(!ReadIsEndArrayWithSkipValueSeparator)
        /// </summary>
        public bool ReadIsInArray(ref int count)
        {
            if (count == 0)
            {
                ReadIsBeginArrayWithVerify();
                if (ReadIsEndArray())
                {
                    return false;
                }
            }
            else
            {
                if (ReadIsEndArray())
                {
                    return false;
                }

                ReadIsValueSeparatorWithVerify();
            }

            count++;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsNameSeparator()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != ':')
            {
                return false;
            }

            Advance(1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadIsNameSeparatorWithVerify()
        {
            if (!ReadIsNameSeparator())
            {
                throw new JsonParsingException(ExpectedFirst + ":" + ExpectedLast);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsBeginObject()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != '{')
            {
                return false;
            }

            Advance(1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadIsBeginObjectWithVerify()
        {
            if (!ReadIsBeginObject())
            {
                throw new JsonParsingException(ExpectedFirst + "{" + ExpectedLast);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadIsEndObject()
        {
            SkipWhiteSpace();
            if (!Reader.TryPeek(out var b) || b != '}')
            {
                return false;
            }

            Advance(1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadIsEndObjectWithVerify()
        {
            if (!ReadIsEndObject())
            {
                throw new JsonParsingException(ExpectedFirst + "}" + ExpectedLast);
            }
        }

        public bool ReadIsEndObjectWithSkipValueSeparator(ref int count)
        {
            SkipWhiteSpace();
            if (Reader.TryPeek(out var b) && b == '}')
            {
                Advance(1);
                return true;
            }

            if (count++ != 0)
            {
                ReadIsValueSeparatorWithVerify();
            }

            return false;
        }

        /// <summary>
        /// Convenient pattern of ReadIsBeginObjectWithVerify + while(!ReadIsEndObjectWithSkipValueSeparator)
        /// </summary>
        public bool ReadIsInObject(ref int count)
        {
            if (count == 0)
            {
                ReadIsBeginObjectWithVerify();
                if (ReadIsEndObject())
                {
                    return false;
                }
            }
            else
            {
                if (ReadIsEndObject())
                {
                    return false;
                }

                ReadIsValueSeparatorWithVerify();
            }

            count++;
            return true;
        }

        /// <summary>Get raw string-span(do not unescape)</summary>
        public ReadOnlySpan<byte> ReadNotNullStringSegmentRaw()
        {
            // SkipWhiteSpace is already called from IsNull
            if (!Reader.TryRead(out var b) && b != '\"')
            {
                throw new JsonParsingException("" + ExpectedFirst + "\"'");
            }

            if (!Reader.TryReadTo(out ReadOnlySpan<byte> answer, (byte)'"', (byte)'\\'))
            {
                throw new JsonParsingException(ExpectedFirst + "not found end string." + ExpectedLast);
            }

            return answer;
        }

        /// <summary>Get raw string-span(do not unescape) + ReadIsNameSeparatorWithVerify</summary>
        public ReadOnlySpan<byte> ReadPropertyNameSegmentRaw()
        {
            SkipWhiteSpace();
            var key = ReadNotNullStringSegmentRaw();
            ReadIsNameSeparatorWithVerify();
            return key;
        }

        internal static readonly byte[] NumberBreaks =
        {
            (byte)' ',
            (byte)'\t',
            (byte)'\r',
            (byte)'\n',
            (byte)',',
            (byte)']',
            (byte)'}',
            (byte)'"',
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadNext()
        {
            var token = GetCurrentJsonToken();
            ReadNextCore(token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadNextCore(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.BeginObject:
                case JsonToken.BeginArray:
                case JsonToken.ValueSeparator:
                case JsonToken.NameSeparator:
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                    Advance(1);
                    break;
                case JsonToken.True:
                case JsonToken.Null:
                    Advance(4);
                    break;
                case JsonToken.False:
                    Advance(5);
                    break;
                case JsonToken.String:
                    Advance(1);
                    if (!Reader.TryReadTo(out ReadOnlySpan<byte> _, (byte)'"', (byte)'\\'))
                    {
                        throw new JsonParsingException(ExpectedFirst + "not found end string." + ExpectedLast);
                    }
                    break;
                case JsonToken.Number:
                    if (!Reader.TryAdvanceToAny(NumberBreaks))
                    {
                        Advance(Reader.Remaining);
                    }
                    break;
                case JsonToken.None:
                default:
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadNextBlock()
        {
            var token = GetCurrentJsonToken();
            switch (token)
            {
                case JsonToken.BeginObject:
                case JsonToken.BeginArray:
                    Advance(1);
                    ReadNextBlockCore(1);
                    break;
                case JsonToken.EndObject:
                    throw new JsonParsingException("Invalid Json Token : End Of Object");
                case JsonToken.EndArray:
                    throw new JsonParsingException("Invalid Json Token : End Of Array");
                case JsonToken.True:
                case JsonToken.False:
                case JsonToken.Null:
                case JsonToken.String:
                case JsonToken.Number:
                case JsonToken.NameSeparator:
                case JsonToken.ValueSeparator:
                    ReadNextCore(token);
                    break;
                case JsonToken.None:
                default:
                    break;
            }
        }

        private void ReadNextBlockCore(int stack)
        {
            while (true)
            {
                var token = GetCurrentJsonToken();
                switch (token)
                {
                    case JsonToken.BeginObject:
                    case JsonToken.BeginArray:
                        Advance(1);
                        stack++;
                        continue;
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        Advance(1);
                        if (stack != 1)
                        {
                            stack--;
                            continue;
                        }
                        break;
                    case JsonToken.True:
                    case JsonToken.False:
                    case JsonToken.Null:
                    case JsonToken.String:
                    case JsonToken.Number:
                    case JsonToken.NameSeparator:
                    case JsonToken.ValueSeparator:
                        do
                        {
                            ReadNextCore(token);
                            token = GetCurrentJsonToken();
                        } while (stack != 0 && !((int)token < 5)); // !(None, Begin/EndObject, Begin/EndArray)

                        if (stack != 0)
                        {
                            continue;
                        }
                        break;
                    case JsonToken.None:
                    default:
                        break;
                }
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar()
        {
            SkipWhiteSpace();
            var span = ReadNotNullStringSegmentRaw();
            if (span.IsEmpty)
            {
                goto ERROR;
            }

            var first = (uint)span[0];
            if (first == '\\')
            {
                return ReadCharEscaped(span);
            }

            switch (span.Length)
            {
                case 1:
                    if (first < 0x80)
                    {
                        return (char)first;
                    }

                    goto ERROR;
                case 2:
                    if (first >> 5 == 0b110)
                    {
                        return (char)(((first & 0b11111) << 6) | ((uint)span[1] & 0b111111));
                    }

                    goto ERROR;
                case 3:
                    if (first >> 4 == 0b1110)
                    {
                        var answer = (char)(first & 0b1111U);
                        answer <<= 6;
                        answer |= (char)(span[1] & 0b111111U);
                        answer <<= 6;
                        answer |= (char)(span[2] & 0b111111U);
                        return answer;
                    }

                    goto ERROR;

                default:
                    goto ERROR;
            }

        ERROR:
            throw new JsonParsingException("invalid char.");
        }

        private static char ReadCharEscaped(in ReadOnlySpan<byte> span)
        {
            switch (span[1])
            {
                case (byte)'u':
                    if (span.Length != 6)
                    {
                        goto default;
                    }

                    char ret;
                    uint b = span[2];
                    switch (b)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            ret = (char)(b - 48);
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            ret = (char)(b - 55);
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            ret = (char)(b - 87);
                            break;
                        default:
                            throw new JsonParsingException("Invalid Hex Character.");
                    }
                    ret <<= 4;
                    b = span[3];
                    switch (b)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            ret |= (char)(b - 48);
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            ret |= (char)(b - 55);
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            ret |= (char)(b - 87);
                            break;
                        default:
                            throw new JsonParsingException("Invalid Hex Character.");
                    }
                    ret <<= 4;
                    b = span[4];
                    switch (b)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            ret |= (char)(b - 48);
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            ret |= (char)(b - 55);
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            ret |= (char)(b - 87);
                            break;
                        default:
                            throw new JsonParsingException("Invalid Hex Character.");
                    }
                    ret <<= 4;
                    b = span[5];
                    switch (b)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            ret |= (char)(b - 48);
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            ret |= (char)(b - 55);
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            ret |= (char)(b - 87);
                            break;
                        default:
                            throw new JsonParsingException("Invalid Hex Character.");
                    }
                    return ret;
                case (byte)'b':
                    if (span.Length != 2)
                    {
                        goto default;
                    }

                    return '\b';
                case (byte)'f':
                    if (span.Length != 2)
                    {
                        goto default;
                    }

                    return '\f';
                case (byte)'n':
                    if (span.Length != 2)
                    {
                        goto default;
                    }

                    return '\n';
                case (byte)'r':
                    if (span.Length != 2)
                    {
                        goto default;
                    }

                    return '\r';
                case (byte)'t':
                    if (span.Length != 2)
                    {
                        goto default;
                    }

                    return '\t';
                case (byte)'\\':
                case (byte)'/':
                    if (span.Length != 2)
                    {
                        goto default;
                    }

                    return (char)span[1];
                default:
                    throw new JsonParsingException("invalid char.");
            }
        }

        public JsonToken GetCurrentJsonToken()
        {
            SkipWhiteSpace();

            if (!Reader.TryPeek(out var b))
            {
                return JsonToken.None;
            }

            switch (b)
            {
                case (byte)'{': return JsonToken.BeginObject;
                case (byte)'}': return JsonToken.EndObject;
                case (byte)'[': return JsonToken.BeginArray;
                case (byte)']': return JsonToken.EndArray;
                case (byte)'t': return JsonToken.True;
                case (byte)'f': return JsonToken.False;
                case (byte)'n': return JsonToken.Null;
                case (byte)',': return JsonToken.ValueSeparator;
                case (byte)':': return JsonToken.NameSeparator;
                case (byte)'-': return JsonToken.Number;
                case (byte)'0': return JsonToken.Number;
                case (byte)'1': return JsonToken.Number;
                case (byte)'2': return JsonToken.Number;
                case (byte)'3': return JsonToken.Number;
                case (byte)'4': return JsonToken.Number;
                case (byte)'5': return JsonToken.Number;
                case (byte)'6': return JsonToken.Number;
                case (byte)'7': return JsonToken.Number;
                case (byte)'8': return JsonToken.Number;
                case (byte)'9': return JsonToken.Number;
                case (byte)'\"': return JsonToken.String;
                #region Other cases
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 46:
                case 47:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 92:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 103:
                case 104:
                case 105:
                case 106:
                case 107:
                case 108:
                case 109:
                case 111:
                case 112:
                case 113:
                case 114:
                case 115:
                case 117:
                case 118:
                case 119:
                case 120:
                case 121:
                case 122:
                #endregion
                default:
                    return JsonToken.None;
            }
        }
    }
}
