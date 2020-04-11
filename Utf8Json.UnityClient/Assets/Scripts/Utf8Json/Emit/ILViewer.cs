// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace Utf8Json.Internal
{
    internal class IlStreamReader : BinaryReader
    {
        private static readonly OpCode[] oneByteOpCodes = new OpCode[0x100];
        private static readonly OpCode[] twoByteOpCodes = new OpCode[0x100];

        private readonly int endPosition;

        public int CurrentPosition => (int)BaseStream.Position;

        public bool EndOfStream => !((int)BaseStream.Position < endPosition);

        static IlStreamReader()
        {
            foreach (var fi in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var opCode = (OpCode)fi.GetValue(null);
                var value = unchecked((ushort)opCode.Value);

                if (value < 0x100)
                {
                    oneByteOpCodes[value] = opCode;
                }
                else if ((value & 0xff00) == 0xfe00)
                {
                    twoByteOpCodes[value & 0xff] = opCode;
                }
            }
        }

        public IlStreamReader(byte[] ilByteArray)
            : base(new MemoryStream(ilByteArray))
        {
            this.endPosition = ilByteArray.Length;
        }

        public OpCode ReadOpCode()
        {
            var code = ReadByte();
            if (code != 0xFE)
            {
                return oneByteOpCodes[code];
            }
            code = ReadByte();
            return twoByteOpCodes[code];
        }

        public int ReadMetadataToken()
        {
            return ReadInt32();
        }
    }

#if DEBUG

    // not yet completed so only for debug.
    public static class ILViewer
    {
        public static string ToPrettyPrintInstruction(MethodBase method)
        {
            var sb = new StringBuilder();

            foreach (var item in ToOpCodes(method))
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }

        public static IEnumerable<Instruction> ToOpCodes(MethodBase method)
        {
            var body = method.GetMethodBody();
            if (body == null) yield break;

            var il = body.GetILAsByteArray();

            using (var reader = new IlStreamReader(il))
            {
                while (!reader.EndOfStream)
                {

                    var data = default(object);
                    var position = reader.CurrentPosition;

                    var opCode = reader.ReadOpCode();
                    switch (opCode.OperandType)
                    {
                        case OperandType.ShortInlineI:
                            data = reader.ReadByte();
                            break;
                        case OperandType.ShortInlineBrTarget:
                        case OperandType.ShortInlineVar:
                            // data =
                            reader.ReadByte();
                            break;
                        case OperandType.InlineVar:
                            reader.ReadUInt16();
                            break;
                        case OperandType.InlineSig:
                        case OperandType.InlineType:
                            reader.ReadUInt32();
                            break;
                        case OperandType.InlineNone:
                            break;
                        case OperandType.InlineBrTarget:
                            // data = 
                            reader.ReadUInt32();
                            break;
                        case OperandType.InlineSwitch:
                            {
                                var count = reader.ReadUInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    // data =...
                                    reader.ReadInt32();
                                }
                            }
                            break;
                        case OperandType.InlineTok:
                        case OperandType.InlineI:
                        case OperandType.InlineString:
                        case OperandType.InlineMethod:
                        case OperandType.InlineField:
                            var metaDataToken = reader.ReadMetadataToken();
                            var genericMethodArguments = default(Type[]);
                            if (method.IsGenericMethod)
                            {
                                genericMethodArguments = method.GetGenericArguments();
                            }

                            if (opCode.OperandType == OperandType.InlineMethod)
                            {
                                data = method.Module.ResolveMethod(metaDataToken, method.DeclaringType.GetGenericArguments(), genericMethodArguments);
                            }
                            else if (opCode.OperandType == OperandType.InlineField)
                            {
                                data = method.Module.ResolveField(metaDataToken, method.DeclaringType.GetGenericArguments(), genericMethodArguments);
                            }
                            else if (opCode.OperandType == OperandType.InlineString)
                            {
                                data = method.Module.ResolveString(metaDataToken);
                            }
                            else if (opCode.OperandType == OperandType.InlineI)
                            {
                                data = metaDataToken;
                            }
                            else if (opCode.OperandType == OperandType.InlineTok)
                            {
                                data = method.Module.ResolveType(metaDataToken);
                            }
                            break;
                        case OperandType.ShortInlineR:
                            data = reader.ReadSingle();
                            break;
                        case OperandType.InlineI8: // more dump?
                        case OperandType.InlineR:
                            if (opCode.OperandType == OperandType.InlineI8)
                            {
                                data = reader.ReadInt64();
                            }
                            else
                            {
                                data = reader.ReadDouble();
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    Debug.Assert(data != null, nameof(data) + " != null");
                    yield return new Instruction(position, opCode, data);
                }
            }
        }
        public struct Instruction
        {
            public readonly int Offset;
            public readonly OpCode OpCode;
            public readonly object Data;

            private static readonly Regex trimVersion = new Regex(", Version=.+, Culture=.+, PublicKeyToken=[0-9a-z]+", RegexOptions.Compiled);

            public Instruction(int offset, OpCode opCode, object data)
            {
                Offset = offset;
                OpCode = opCode;
                Data = data;
            }

            public override string ToString()
            {
                // format like LINQPad IL
                var addition = "";
                if (Data is int && OpCode == OpCodes.Switch) // switch
                {
                    // var offset = Offset;
                    // addition = "(" + string.Join(", ", Enumerable.Range(0, (int)Data).Select(x => "IL_" + (offset + x * 4).ToString("X4")).ToArray()) + ")";
                }
                else if ((OpCode.OperandType == OperandType.InlineBrTarget) && (Data is int))
                {
                    // note:jump position
                    // addition = "IL_" + (Offset + (int)Data).ToString("X4");
                }
                else if ((OpCode.OperandType == OperandType.ShortInlineBrTarget) && (Data is byte))
                {
                    // addition = "IL_" + (Offset + (byte)Data).ToString("X4");
                }
                else
                {
                    switch (Data)
                    {
                        case byte b:
                            addition = b.ToString(CultureInfo.InvariantCulture); // I don't like hex format:)
                            break;
                        case int i:
                            addition = i.ToString(CultureInfo.InvariantCulture);
                            break;
                        case long l:
                            addition = l.ToString(CultureInfo.InvariantCulture);
                            break;
                        case double d:
                            addition = d.ToString(CultureInfo.InvariantCulture);
                            break;
                        case float f:
                            addition = f.ToString(CultureInfo.InvariantCulture);
                            break;
                        case MethodInfo methodInfo:
                            addition = trimVersion.Replace(methodInfo.DeclaringType?.FullName ?? throw new InvalidOperationException(), "") + "." + methodInfo.Name;
                            break;
                        case ConstructorInfo constructorInfo:
                            addition = trimVersion.Replace(constructorInfo.DeclaringType?.FullName ?? throw new InvalidOperationException(), "") + ".ctor";
                            break;
                        case FieldInfo fieldInfo:
                            addition = trimVersion.Replace(fieldInfo.DeclaringType?.FullName ?? throw new InvalidOperationException(), "") + "." + fieldInfo.Name;
                            break;
                        case string str:
                            addition = str;
                            break;
                        case Type t:
                            Debug.Assert(t.FullName != null, "t.FullName != null");
                            addition = trimVersion.Replace(t.FullName, "");
                            break;
                    }
                }

                return $"IL_{Offset,4:X4}:  {OpCode,-11} {addition}";
            }
        }
    }

#endif
}
#endif
