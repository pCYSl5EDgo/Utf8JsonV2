// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Utf8Json.Internal;
// ReSharper disable ConvertSwitchStatementToSwitchExpression

// ReSharper disable RedundantCaseLabel
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Utf8Json
{
    public struct JsonObject
        : IEquatable<JsonObject>
#if CSHARP_8_OR_NEWER
            , IEquatable<object?>
#else
            , IEquatable<object>
#endif
    {
        public enum Kind
        {
            EmptyObject,
            EmptyArray,
            Object,
            Array,
            BooleanArray,
            StringArray,
            NumberArray,
            String,
            Number,
            True,
            False,
            Null,
        }

#if CSHARP_8_OR_NEWER
        public object? ToObject()
#else
        public object ToObject()
#endif
        {
            switch (Token)
            {
                case Kind.Object: return ObjectDictionary;
                case Kind.BooleanArray: return BooleanArray;
                case Kind.Array: return ObjectArray;
                case Kind.StringArray: return StringArray;
                case Kind.NumberArray: return NumberArray;
                case Kind.Number: return Number;
                case Kind.String: return String;
                case Kind.True: return ObjectHelper.True;
                case Kind.False: return ObjectHelper.False;
                case Kind.Null: return null;
                case Kind.EmptyObject: return ObjectHelper.Object;
                case Kind.EmptyArray: return Array.Empty<object>();
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public Kind Token;
        public double Number;
#if CSHARP_8_OR_NEWER
        private object? union;
#else
        private object union;
#endif

#if CSHARP_8_OR_NEWER
        public Dictionary<string, JsonObject>? ObjectDictionary
        {
            get => union as Dictionary<string, JsonObject>;
            set => union = value;
        }

        public JsonObject[]? ObjectArray
        {
            get => union as JsonObject[];
            set => union = value;
        }

        public bool[]? BooleanArray
        {
            get => union as bool[];
            set => union = value;
        }

        public string?[]? StringArray
        {
            get => union as string[];
            set => union = value;
        }

        public double[]? NumberArray
        {
            get => union as double[];
            set => union = value;
        }

        public string? String
        {
            get => union as string;
            set => union = value;
        }
#else
        public Dictionary<string, JsonObject> ObjectDictionary
        {
            get => union as Dictionary<string, JsonObject>;
            set => union = value;
        }

        public JsonObject[] ObjectArray
        {
            get => union as JsonObject[];
            set => union = value;
        }

        public bool[] BooleanArray
        {
            get => union as bool[];
            set => union = value;
        }

        public string[] StringArray
        {
            get => union as string[];
            set => union = value;
        }

        public double[] NumberArray
        {
            get => union as double[];
            set => union = value;
        }

        public string String
        {
            get => union as string;
            set => union = value;
        }
#endif

        public void ReCalc()
        {
            if (Token == Kind.Array)
            {
                ReCalcArray();
            }
        }

        private void ReCalcArray()
        {
            if (this.ObjectArray == null)
            {
                this.Token = Kind.Null;
                return;
            }

            if (this.ObjectArray.Length == 0)
            {
                this.Token = Kind.EmptyArray;
                return;
            }

            for (var index = 0; index < this.ObjectArray.Length; index++)
            {
                this.ObjectArray[index].ReCalc();
            }

            var firstKind = this.ObjectArray[0].Token;
            if (firstKind != Kind.Number && firstKind != Kind.String && firstKind != Kind.True && firstKind != Kind.False)
            {
                return;
            }

            if (firstKind == Kind.True || firstKind == Kind.False)
            {
                for (var index = 1; index < this.ObjectArray.Length; index++)
                {
                    var token = this.ObjectArray[index].Token;
                    if (token != Kind.True && token != Kind.False)
                    {
                        return;
                    }
                }
            }
            else
            {
                for (var index = 1; index < this.ObjectArray.Length; index++)
                {
                    if (firstKind != this.ObjectArray[index].Token)
                    {
                        return;
                    }
                }
            }

            switch (firstKind)
            {
                case Kind.Number:
                    var numbers = new double[this.ObjectArray.Length];
                    for (var index = 0; index < this.ObjectArray.Length; index++)
                    {
                        numbers[index] = this.ObjectArray[index].Number;
                    }

                    this.Token = Kind.NumberArray;
                    this.union = numbers;
                    break;
                case Kind.String:
#if CSHARP_8_OR_NEWER
                    var strings = new string?[this.ObjectArray.Length];
#else
                    var strings = new string[this.ObjectArray.Length];
#endif
                    for (var index = 0; index < this.ObjectArray.Length; index++)
                    {
                        strings[index] = this.ObjectArray[index].String;
                    }

                    this.Token = Kind.StringArray;
                    this.union = strings;
                    break;
                case Kind.True:
                case Kind.False:
                    var booleans = new bool[this.ObjectArray.Length];
                    for (var index = 0; index < this.ObjectArray.Length; index++)
                    {
                        var token = this.ObjectArray[index].Token;
                        booleans[index] = token == Kind.True;
                    }

                    this.Token = Kind.BooleanArray;
                    this.union = booleans;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public bool Equals(JsonObject other)
        {
            if (Token != other.Token)
            {
                return false;
            }

            switch (Token)
            {
                case Kind.Object:
                    return ObjectDictionary.SequenceEqual(other.ObjectDictionary);
                case Kind.Array:
                    return ObjectArray.SequenceEqual(other.ObjectArray);
                case Kind.Number:
                    return Number == other.Number;
                case Kind.String:
                    return String == other.String;
                case Kind.BooleanArray:
                    return BooleanArray.SequenceEqual(other.BooleanArray);
                case Kind.StringArray:
                    return StringArray.SequenceEqual(other.StringArray);
                case Kind.NumberArray:
                    return NumberArray.SequenceEqual(other.NumberArray);
                case Kind.EmptyObject:
                case Kind.EmptyArray:
                case Kind.True:
                case Kind.False:
                case Kind.Null:
                default:
                    return true;
            }
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return (int)Token;
        }

        public override bool Equals(object
#if CSHARP_8_OR_NEWER
            ?
#endif
            other)
        {
            switch (Token)
            {
                case Kind.Object: return (other as Dictionary<string, JsonObject>)?.SequenceEqual(ObjectDictionary) ?? false;
                case Kind.Array: return (other as JsonObject[])?.SequenceEqual(ObjectArray) ?? false;
                case Kind.Number: return other is double dOther && Number == dOther;
                case Kind.String: return String == other as string;
                case Kind.True: return other is bool xOther && xOther;
                case Kind.False: return other is bool yOther && !yOther;
                case Kind.BooleanArray: return (other as bool[])?.SequenceEqual(BooleanArray) ?? false;
                case Kind.StringArray: return (other as string[])?.SequenceEqual(StringArray) ?? false;
                case Kind.NumberArray: return (other as double[])?.SequenceEqual(NumberArray) ?? false;
                case Kind.Null: return other == null;
                case Kind.EmptyObject: return other != null && other.GetType() == typeof(object);
                case Kind.EmptyArray: return other != null && other is Array array && array.Length == 0;
                default: return false;
            }
        }
    }
}
