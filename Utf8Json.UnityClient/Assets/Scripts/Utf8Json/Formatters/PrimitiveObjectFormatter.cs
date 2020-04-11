// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if ENABLE_IL2CPP
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Utf8Json.Internal;
// ReSharper disable RedundantCaseLabel

namespace Utf8Json.Formatters
{
    public sealed class PrimitiveObjectFormatter : IJsonFormatter<object>
    {
        private static readonly ThreadSafeTypeKeyIntValueHashTable table = new ThreadSafeTypeKeyIntValueHashTable(0.75f,
            typeof(bool),
            typeof(char),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(string)
        );

#region Serialize
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        => SerializeStatic(ref writer, value, options);

#if CSHARP_8_OR_NEWER
        public statoc void SerializeStatic(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var t = value.GetType();

            if (table.TryGetValue(t, out var code))
            {
                switch (code)
                {
                    case 0:
                        writer.Write((bool)value);
                        return;
                    case 1:
                        writer.Write((char)value);
                        return;
                    case 2:
                        writer.Write((sbyte)value);
                        return;
                    case 3:
                        writer.Write((byte)value);
                        return;
                    case 4:
                        writer.Write((short)value);
                        return;
                    case 5:
                        writer.Write((ushort)value);
                        return;
                    case 6:
                        writer.Write((int)value);
                        return;
                    case 7:
                        writer.Write((uint)value);
                        return;
                    case 8:
                        writer.Write((long)value);
                        return;
                    case 9:
                        writer.Write((ulong)value);
                        return;
                    case 10:
                        writer.Write((float)value);
                        return;
                    case 11:
                        writer.Write((double)value);
                        return;
                    case 12:
                        writer.Write(Unsafe.As<string>(value));
                        return;
                    default:
                        goto ERROR;
                }
            }

            if (t.IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(t);
                var code2 = table[underlyingType];
                switch (code2)
                {
                    case 2:
                        writer.Write((sbyte)value);
                        return;
                    case 3:
                        writer.Write((byte)value);
                        return;
                    case 4:
                        writer.Write((short)value);
                        return;
                    case 5:
                        writer.Write((ushort)value);
                        return;
                    case 6:
                        writer.Write((int)value);
                        return;
                    case 7:
                        writer.Write((uint)value);
                        return;
                    case 8:
                        writer.Write((long)value);
                        return;
                    case 9:
                        writer.Write((ulong)value);
                        return;
                    default:
                        goto ERROR;
                }
            }

            if (t.IsArray)
            {
                SerializeArray(ref writer, options, Unsafe.As<Array>(value), t);
                return;
            }

            if (value is IDictionary dictionary)
            {
                SerializeDictionary(ref writer, options, dictionary, t);
                return;
            }

            if (value is ICollection collection)
            {
                SerializeCollection(ref writer, options, collection);
                return;
            }

        ERROR:
            throw new JsonSerializationException("Not supported primitive object resolver. type:" + t.Name);
        }

#region Serialize Array
        private void SerializeArray(ref JsonWriter writer, JsonSerializerOptions options, Array value, Type type)
        {
            var elementType = type.GetElementType();
            if (value.Rank == 1)
            {
                SerializeArrayRank1(ref writer, options, value, elementType);
                return;
            }

            SerializeArrayMultiDimension(ref writer, options, value, elementType);
        }

        private static readonly byte[] arrayMultiDimensionBeginBytes = { 0x7B, 0x22, 0x6C, 0x65, 0x6E, 0x67, 0x74, 0x68, 0x73, 0x22, 0x3A, 0x5B, }; // {"lengths":[
        private static readonly byte[] arrayMultiDimensionMiddleBytes = { 0x5D, 0x2C, 0x22, 0x69, 0x74, 0x65, 0x6D, 0x73, 0x22, 0x3A, 0x5B, }; // ],"items":[
        private static readonly byte[] arrayMultiDimensionEndBytes = { 0x5D, 0x7D, }; // ]}

        private void SerializeArrayMultiDimension(ref JsonWriter writer, JsonSerializerOptions options, Array value, Type elementType)
        {
            writer.WriteRaw(arrayMultiDimensionBeginBytes);
            writer.Write(value.GetLongLength(0));
            for (int dimension = 1, rank = value.Rank; dimension < rank; dimension++)
            {
                writer.WriteValueSeparator();
                writer.Write(value.GetLongLength(dimension));
            }
            writer.WriteRaw(arrayMultiDimensionMiddleBytes);
            var first = true;
            if (table.TryGetValue(elementType, out var code))
            {
                switch (code)
                {
                    case 0:
                        foreach (bool item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 1:
                        foreach (char item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 2:
                        foreach (sbyte item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 3:
                        foreach (byte item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 4:
                        foreach (short item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 5:
                        foreach (ushort item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 6:
                        foreach (int item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 7:
                        foreach (uint item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 8:
                        foreach (long item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 9:
                        foreach (ulong item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 10:
                        foreach (float item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 11:
                        foreach (double item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            writer.Write(item);
                        }
                        goto END;
                    case 12:
                        foreach (var item in value)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.WriteValueSeparator();
                            }

                            if (item == null)
                            {
                                writer.WriteNull();
                            }
                            else
                            {
                                writer.Write(Unsafe.As<string>(item));
                            }
                        }
                        goto END;
                }
            }

            foreach (var item in value)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                Serialize(ref writer, item, options);
            }

        END:
            writer.WriteRaw(arrayMultiDimensionEndBytes);
        }

        private void SerializeArrayRank1(ref JsonWriter writer, JsonSerializerOptions options, Array value, Type elementType)
        {
            if (table.TryGetValue(elementType, out var code))
            {
                switch (code)
                {
                    case 0:
                        SerializeArray(ref writer, Unsafe.As<bool[]>(value));
                        return;
                    case 1:
                        SerializeArray(ref writer, Unsafe.As<char[]>(value));
                        return;
                    case 2:
                        SerializeArray(ref writer, Unsafe.As<sbyte[]>(value));
                        return;
                    case 3:
                        SerializeArray(ref writer, Unsafe.As<byte[]>(value));
                        return;
                    case 4:
                        SerializeArray(ref writer, Unsafe.As<short[]>(value));
                        return;
                    case 5:
                        SerializeArray(ref writer, Unsafe.As<ushort[]>(value));
                        return;
                    case 6:
                        SerializeArray(ref writer, Unsafe.As<int[]>(value));
                        return;
                    case 7:
                        SerializeArray(ref writer, Unsafe.As<uint[]>(value));
                        return;
                    case 8:
                        SerializeArray(ref writer, Unsafe.As<long[]>(value));
                        return;
                    case 9:
                        SerializeArray(ref writer, Unsafe.As<ulong[]>(value));
                        return;
                    case 10:
                        SerializeArray(ref writer, Unsafe.As<float[]>(value));
                        return;
                    case 11:
                        SerializeArray(ref writer, Unsafe.As<double[]>(value));
                        return;
                    case 12:
#if CSHARP_8_OR_NEWER
                        SerializeArray(ref writer, Unsafe.As<string?[]>(value));
#else
                        SerializeArray(ref writer, Unsafe.As<string[]>(value));
#endif
                        return;
                }
            }

            writer.WriteBeginArray();

            var first = true;
            foreach (var item in value)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                Serialize(ref writer, item, options);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, int[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, uint[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, byte[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, sbyte[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, short[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, ushort[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, long[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, ulong[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, bool[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, float[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, double[] array)
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                writer.Write(array[i]);
            }

            writer.WriteEndArray();
        }

        private static void SerializeArray(ref JsonWriter writer, char[] array)
        {
            writer.Write(array);
        }

#if CSHARP_8_OR_NEWER
        private static void SerializeArray(ref JsonWriter writer, string?[] array)
#else
        private static void SerializeArray(ref JsonWriter writer, string[] array)
#endif
        {
            writer.WriteBeginArray();
            if (array.Length != 0)
            {
                writer.Write(array[0]);
            }

            for (var i = 1; i < array.Length; i++)
            {
                writer.WriteValueSeparator();
                if (array[i] == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Write(array[i]);
                }
            }

            writer.WriteEndArray();
        }

#endregion

#region Serialize Collection
        private void SerializeCollection(ref JsonWriter writer, JsonSerializerOptions options, ICollection collection)
        {
            switch (collection)
            {
                case ICollection<int> intCollection:
                    SerializeCollection(ref writer, intCollection);
                    return;
                case ICollection<uint> uintCollection:
                    SerializeCollection(ref writer, uintCollection);
                    return;
#if CSHARP_8_OR_NEWER
                case ICollection<string?> stringCollection:
#else
                case ICollection<string> stringCollection:
#endif
                    SerializeCollection(ref writer, stringCollection);
                    return;
                case ICollection<bool> boolCollection:
                    SerializeCollection(ref writer, boolCollection);
                    return;
                case ICollection<char> charCollection:
                    SerializeCollection(ref writer, charCollection);
                    return;
                case ICollection<long> longCollection:
                    SerializeCollection(ref writer, longCollection);
                    return;
                case ICollection<ulong> ulongCollection:
                    SerializeCollection(ref writer, ulongCollection);
                    return;
                case ICollection<byte> byteCollection:
                    SerializeCollection(ref writer, byteCollection);
                    return;
                case ICollection<float> floatCollection:
                    SerializeCollection(ref writer, floatCollection);
                    return;
                case ICollection<double> doubleCollection:
                    SerializeCollection(ref writer, doubleCollection);
                    return;
                case ICollection<short> shortCollection:
                    SerializeCollection(ref writer, shortCollection);
                    return;
                case ICollection<ushort> ushortCollection:
                    SerializeCollection(ref writer, ushortCollection);
                    return;
                case ICollection<sbyte> sbyteCollection:
                    SerializeCollection(ref writer, sbyteCollection);
                    return;
                case ICollection<object> objectCollection:
                    SerializeCollection(ref writer, objectCollection, options);
                    return;
            }

            var first = true;
            writer.WriteBeginArray();
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                Serialize(ref writer, item, options);
            }
            writer.WriteEndArray();
        }

        private void SerializeCollection(ref JsonWriter writer, ICollection<object> collection, JsonSerializerOptions options)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                Serialize(ref writer, item, options);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<int> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<uint> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<short> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<ushort> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<byte> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<sbyte> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<bool> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<char> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<long> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<ulong> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<float> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

        private static void SerializeCollection(ref JsonWriter writer, ICollection<double> collection)
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(item);
            }

            writer.WriteEndArray();
        }

#if CSHARP_8_OR_NEWER
        private static void SerializeCollection(ref JsonWriter writer, ICollection<string?> collection)
#else
        private static void SerializeCollection(ref JsonWriter writer, ICollection<string> collection)
#endif
        {
            writer.WriteBeginArray();
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                if (item == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Write(item);
                }
            }

            writer.WriteEndArray();
        }
#endregion

#region Serialize Dictionary
        private void SerializeDictionary(ref JsonWriter writer, JsonSerializerOptions options, IDictionary dictionary, Type type)
        {
            switch (dictionary)
            {
                case IDictionary<string, int> intDictionary:
                    SerializeDictionary(ref writer, intDictionary);
                    return;
                case IDictionary<string, uint> uintDictionary:
                    SerializeDictionary(ref writer, uintDictionary);
                    return;
                case IDictionary<string, long> longDictionary:
                    SerializeDictionary(ref writer, longDictionary);
                    return;
                case IDictionary<string, ulong> ulongDictionary:
                    SerializeDictionary(ref writer, ulongDictionary);
                    return;
                case IDictionary<string, bool> boolDictionary:
                    SerializeDictionary(ref writer, boolDictionary);
                    return;
#if CSHARP_8_OR_NEWER
                case IDictionary<string, string?> stringDictionary:
#else
                case IDictionary<string, string> stringDictionary:
#endif
                    SerializeDictionary(ref writer, stringDictionary);
                    return;
                case IDictionary<string, float> floatDictionary:
                    SerializeDictionary(ref writer, floatDictionary);
                    return;
                case IDictionary<string, double> doubleDictionary:
                    SerializeDictionary(ref writer, doubleDictionary);
                    return;
                case IDictionary<string, char> charDictionary:
                    SerializeDictionary(ref writer, charDictionary);
                    return;
                case IDictionary<string, sbyte> sbyteDictionary:
                    SerializeDictionary(ref writer, sbyteDictionary);
                    return;
                case IDictionary<string, byte> byteDictionary:
                    SerializeDictionary(ref writer, byteDictionary);
                    return;
                case IDictionary<string, short> shortDictionary:
                    SerializeDictionary(ref writer, shortDictionary);
                    return;
                case IDictionary<string, ushort> ushortDictionary:
                    SerializeDictionary(ref writer, ushortDictionary);
                    return;
                case IDictionary<string, object> objectDictionary:
                    SerializeDictionary(ref writer, objectDictionary, options);
                    return;
            }

            var first = true;
            writer.WriteBeginObject();
            foreach (DictionaryEntry entry in dictionary)
            {
                if (!(entry.Key is string str))
                {
                    throw new JsonSerializationException("Not supported primitive object resolver. type:" + type.FullName);
                }

                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(str);
                writer.WriteNameSeparator();
                Serialize(ref writer, entry.Value, options);
            }

            writer.WriteEndObject();
        }

        private void SerializeDictionary(ref JsonWriter writer, IDictionary<string, object> dictionary, JsonSerializerOptions options)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                Serialize(ref writer, obj, options);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, bool> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, int> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, char> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, uint> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, long> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, ulong> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, byte> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, sbyte> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, short> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, ushort> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

#if CSHARP_8_OR_NEWER
        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, string?> dictionary)
#else
        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, string> dictionary)
#endif
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                if (obj == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Write(obj);
                }
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, float> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }

        private static void SerializeDictionary(ref JsonWriter writer, IDictionary<string, double> dictionary)
        {
            writer.WriteBeginObject();
            var first = true;
            foreach (var (key, obj) in dictionary)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteValueSeparator();
                }

                writer.Write(key);
                writer.WriteNameSeparator();
                writer.Write(obj);
            }

            writer.WriteEndObject();
        }
#endregion
#endregion

#region Deserialize
#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public object? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public object Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        => DeserializeStatic(ref reader, options);

#if CSHARP_8_OR_NEWER
#pragma warning disable 8613
        public static object? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#pragma warning restore 8613
#else
        public static object DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            var token = reader.GetCurrentJsonToken();
            switch (token)
            {
                case JsonToken.BeginObject:
                    reader.Advance(1);
                    return DeserializeObject(ref reader, options);
                case JsonToken.BeginArray:
                    reader.Advance(1);
                    return DeserializeArray(ref reader, options);
                case JsonToken.Number:
                    return reader.ReadDouble();
                case JsonToken.String:
                    return reader.ReadString();
                case JsonToken.True:
                    if (!reader.AdvanceTrue())
                    {
                        goto default;
                    }

                    return true;
                case JsonToken.False:
                    if (!reader.AdvanceFalse())
                    {
                        goto default;
                    }

                    return false;
                case JsonToken.Null:
                    reader.Advance(4);
                    return default;
                case JsonToken.EndArray:
                case JsonToken.ValueSeparator:
                case JsonToken.NameSeparator:
                case JsonToken.EndObject:
                case JsonToken.None:
                default:
                    throw new JsonParsingException("Invalid JSON");
            }
        }

        private Dictionary<string, object?> DeserializeObject(ref JsonReader reader, JsonSerializerOptions options)
        {
#if CSHARP_8_OR_NEWER
            var dictionary = new Dictionary<string, object?>();
#else
            var dictionary = new Dictionary<string, object>();
#endif

            do
            {
                var token = reader.GetCurrentJsonToken();
                if (token == JsonToken.EndObject)
                {
                    break;
                }

                if (token == JsonToken.ValueSeparator)
                {
                    reader.Advance(1);
                }

                var key = reader.ReadString();
                if (key == null)
                {
                    throw new JsonParsingException("Property name should not be null.");
                }

                reader.ReadIsNameSeparatorWithVerify();
                var value = Deserialize(ref reader, options);

                dictionary.Add(key, value);
            } while (true);

            return dictionary;
        }

        private object DeserializeArray(ref JsonReader reader, JsonSerializerOptions options)
        {
#if CSHARP_8_OR_NEWER
            var list = new List<object?>();
#else
            var list = new List<object>();
#endif
            do
            {
                var token = reader.GetCurrentJsonToken();
                if (token == JsonToken.EndArray)
                {
                    break;
                }

                if (token == JsonToken.ValueSeparator)
                {
                    reader.Advance(1);
                }

                list.Add(Deserialize(ref reader, options));
            } while (true);

            reader.ReadNextCore(JsonToken.EndArray);
            if (list.Count == 0)
            {
                return Array.Empty<object>();
            }

            return list;
        }

#endregion
    }
}
#endif
