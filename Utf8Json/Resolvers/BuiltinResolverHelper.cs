// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utf8Json.Formatters;
using Utf8Json.Internal;

#if UNITY_2018_4_OR_NEWER
using Unity.Collections;
#endif

#if !ENABLE_IL2CPP
using System.Dynamic;
#endif

namespace Utf8Json.Resolvers
{
    public sealed partial class BuiltinResolver
    {
        internal static class BuiltinResolverGetFormatterHelper
        {
            private static readonly ThreadSafeTypeKeyFormatterHashTable formatterHashTable
#region table
            = new ThreadSafeTypeKeyFormatterHashTable(new[]
            {
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(string[]), StaticHelper.GetSerializeStatic<string?[]?, NullableStringArrayFormatter>(), StaticHelper.GetDeserializeStatic<string?[]?, NullableStringArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(string[]), StaticHelper.GetSerializeStatic<string[], NullableStringArrayFormatter>(), StaticHelper.GetDeserializeStatic<string[], NullableStringArrayFormatter>()),
#endif
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  byte), StaticHelper.GetSerializeStatic<  byte, ByteFormatter>(), StaticHelper.GetDeserializeStatic<  byte, ByteFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  char), StaticHelper.GetSerializeStatic<  char, CharFormatter>(), StaticHelper.GetDeserializeStatic<  char, CharFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( short), StaticHelper.GetSerializeStatic< short, Int16Formatter>(), StaticHelper.GetDeserializeStatic< short, Int16Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(   int), StaticHelper.GetSerializeStatic<   int, Int32Formatter>(), StaticHelper.GetDeserializeStatic<   int, Int32Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  long), StaticHelper.GetSerializeStatic<  long, Int64Formatter>(), StaticHelper.GetDeserializeStatic<  long, Int64Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( sbyte), StaticHelper.GetSerializeStatic< sbyte, SByteFormatter>(), StaticHelper.GetDeserializeStatic< sbyte, SByteFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ushort), StaticHelper.GetSerializeStatic<ushort, UInt16Formatter>(), StaticHelper.GetDeserializeStatic<ushort, UInt16Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  uint), StaticHelper.GetSerializeStatic<  uint, UInt32Formatter>(), StaticHelper.GetDeserializeStatic<  uint, UInt32Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( ulong), StaticHelper.GetSerializeStatic< ulong, UInt64Formatter>(), StaticHelper.GetDeserializeStatic< ulong, UInt64Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( float), StaticHelper.GetSerializeStatic< float, SingleFormatter>(), StaticHelper.GetDeserializeStatic< float, SingleFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(double), StaticHelper.GetSerializeStatic<double, DoubleFormatter>(), StaticHelper.GetDeserializeStatic<double, DoubleFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  bool), StaticHelper.GetSerializeStatic<  bool, BooleanFormatter>(), StaticHelper.GetDeserializeStatic<  bool, BooleanFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( short?), StaticHelper.GetSerializeStatic< short?, NullableInt16Formatter>(), StaticHelper.GetDeserializeStatic< short?, NullableInt16Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( int?  ), StaticHelper.GetSerializeStatic< int?  , NullableInt32Formatter>(), StaticHelper.GetDeserializeStatic< int?  , NullableInt32Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( long?  ), StaticHelper.GetSerializeStatic< long?  , NullableInt64Formatter>(), StaticHelper.GetDeserializeStatic< long?  , NullableInt64Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( ushort?), StaticHelper.GetSerializeStatic< ushort?, NullableUInt16Formatter>(), StaticHelper.GetDeserializeStatic< ushort?, NullableUInt16Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(   uint?), StaticHelper.GetSerializeStatic<   uint?, NullableUInt32Formatter>(), StaticHelper.GetDeserializeStatic<   uint?, NullableUInt32Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  ulong?), StaticHelper.GetSerializeStatic<  ulong?, NullableUInt64Formatter>(), StaticHelper.GetDeserializeStatic<  ulong?, NullableUInt64Formatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  byte?), StaticHelper.GetSerializeStatic<  byte?, NullableByteFormatter>(), StaticHelper.GetDeserializeStatic<  byte?, NullableByteFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( sbyte?), StaticHelper.GetSerializeStatic< sbyte?, NullableSByteFormatter>(), StaticHelper.GetDeserializeStatic< sbyte?, NullableSByteFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  bool?), StaticHelper.GetSerializeStatic<  bool?, NullableBooleanFormatter>(), StaticHelper.GetDeserializeStatic<  bool?, NullableBooleanFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  char?), StaticHelper.GetSerializeStatic<  char?, NullableCharFormatter>(), StaticHelper.GetDeserializeStatic<  char?, NullableCharFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( float?), StaticHelper.GetSerializeStatic< float?, NullableSingleFormatter>(), StaticHelper.GetDeserializeStatic< float?, NullableSingleFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(double?), StaticHelper.GetSerializeStatic<double?, NullableDoubleFormatter>(), StaticHelper.GetDeserializeStatic<double?, NullableDoubleFormatter>()),
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( short[]), StaticHelper.GetSerializeStatic< short[]?, Int16ArrayFormatter>(), StaticHelper.GetDeserializeStatic< short[]?, Int16ArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( short[]), StaticHelper.GetSerializeStatic< short[], Int16ArrayFormatter>(), StaticHelper.GetDeserializeStatic< short[], Int16ArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(   int[]), StaticHelper.GetSerializeStatic<   int[]?, Int32ArrayFormatter>(), StaticHelper.GetDeserializeStatic<   int[]?, Int32ArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(   int[]), StaticHelper.GetSerializeStatic<   int[], Int32ArrayFormatter>(), StaticHelper.GetDeserializeStatic<   int[], Int32ArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  long[]), StaticHelper.GetSerializeStatic<  long[]?, Int64ArrayFormatter>(), StaticHelper.GetDeserializeStatic<  long[]?, Int64ArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  long[]), StaticHelper.GetSerializeStatic<  long[], Int64ArrayFormatter>(), StaticHelper.GetDeserializeStatic<  long[], Int64ArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ushort[]), StaticHelper.GetSerializeStatic<ushort[]?, UInt16ArrayFormatter>(), StaticHelper.GetDeserializeStatic<ushort[]?, UInt16ArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ushort[]), StaticHelper.GetSerializeStatic<ushort[], UInt16ArrayFormatter>(), StaticHelper.GetDeserializeStatic<ushort[], UInt16ArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  uint[]), StaticHelper.GetSerializeStatic<  uint[]?, UInt32ArrayFormatter>(), StaticHelper.GetDeserializeStatic<  uint[]?, UInt32ArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  uint[]), StaticHelper.GetSerializeStatic<  uint[], UInt32ArrayFormatter>(), StaticHelper.GetDeserializeStatic<  uint[], UInt32ArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( ulong[]), StaticHelper.GetSerializeStatic< ulong[]?, UInt64ArrayFormatter>(), StaticHelper.GetDeserializeStatic< ulong[]?, UInt64ArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( ulong[]), StaticHelper.GetSerializeStatic< ulong[], UInt64ArrayFormatter>(), StaticHelper.GetDeserializeStatic< ulong[], UInt64ArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  byte[]), StaticHelper.GetSerializeStatic<  byte[]?, ByteArrayFormatter>(), StaticHelper.GetDeserializeStatic<  byte[]?, ByteArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  byte[]), StaticHelper.GetSerializeStatic<  byte[], ByteArrayFormatter>(), StaticHelper.GetDeserializeStatic<  byte[], ByteArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( sbyte[]), StaticHelper.GetSerializeStatic< sbyte[]?, SByteArrayFormatter>(), StaticHelper.GetDeserializeStatic< sbyte[]?, SByteArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( sbyte[]), StaticHelper.GetSerializeStatic< sbyte[], SByteArrayFormatter>(), StaticHelper.GetDeserializeStatic< sbyte[], SByteArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  bool[]), StaticHelper.GetSerializeStatic<  bool[]?, BooleanArrayFormatter>(), StaticHelper.GetDeserializeStatic<  bool[]?, BooleanArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  bool[]), StaticHelper.GetSerializeStatic<  bool[], BooleanArrayFormatter>(), StaticHelper.GetDeserializeStatic<  bool[], BooleanArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  char[]), StaticHelper.GetSerializeStatic<  char[]?, CharArrayFormatter>(), StaticHelper.GetDeserializeStatic<  char[]?, CharArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(  char[]), StaticHelper.GetSerializeStatic<  char[], CharArrayFormatter>(), StaticHelper.GetDeserializeStatic<  char[], CharArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( float[]), StaticHelper.GetSerializeStatic< float[]?, SingleArrayFormatter>(), StaticHelper.GetDeserializeStatic< float[]?, SingleArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof( float[]), StaticHelper.GetSerializeStatic< float[], SingleArrayFormatter>(), StaticHelper.GetDeserializeStatic< float[], SingleArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(double[]), StaticHelper.GetSerializeStatic<double[]?, DoubleArrayFormatter>(), StaticHelper.GetDeserializeStatic<double[]?, DoubleArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(double[]), StaticHelper.GetSerializeStatic<double[], DoubleArrayFormatter>(), StaticHelper.GetDeserializeStatic<double[], DoubleArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  byte>), StaticHelper.GetSerializeStatic<List<  byte>?, ByteListFormatter>(), StaticHelper.GetDeserializeStatic<List<  byte>?, ByteListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  byte>), StaticHelper.GetSerializeStatic<List<  byte>, ByteListFormatter>(), StaticHelper.GetDeserializeStatic<List<  byte>, ByteListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  char>), StaticHelper.GetSerializeStatic<List<  char>?, CharListFormatter>(), StaticHelper.GetDeserializeStatic<List<  char>?, CharListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  char>), StaticHelper.GetSerializeStatic<List<  char>, CharListFormatter>(), StaticHelper.GetDeserializeStatic<List<  char>, CharListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< short>), StaticHelper.GetSerializeStatic<List< short>?, Int16ListFormatter>(), StaticHelper.GetDeserializeStatic<List< short>?, Int16ListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< short>), StaticHelper.GetSerializeStatic<List< short>, Int16ListFormatter>(), StaticHelper.GetDeserializeStatic<List< short>, Int16ListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<   int>), StaticHelper.GetSerializeStatic<List<   int>?, Int32ListFormatter>(), StaticHelper.GetDeserializeStatic<List<   int>?, Int32ListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<   int>), StaticHelper.GetSerializeStatic<List<   int>, Int32ListFormatter>(), StaticHelper.GetDeserializeStatic<List<   int>, Int32ListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  long>), StaticHelper.GetSerializeStatic<List<  long>?, Int64ListFormatter>(), StaticHelper.GetDeserializeStatic<List<  long>?, Int64ListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  long>), StaticHelper.GetSerializeStatic<List<  long>, Int64ListFormatter>(), StaticHelper.GetDeserializeStatic<List<  long>, Int64ListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< sbyte>), StaticHelper.GetSerializeStatic<List< sbyte>?, SByteListFormatter>(), StaticHelper.GetDeserializeStatic<List< sbyte>?, SByteListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< sbyte>), StaticHelper.GetSerializeStatic<List< sbyte>, SByteListFormatter>(), StaticHelper.GetDeserializeStatic<List< sbyte>, SByteListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<ushort>), StaticHelper.GetSerializeStatic<List<ushort>?, UInt16ListFormatter>(), StaticHelper.GetDeserializeStatic<List<ushort>?, UInt16ListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<ushort>), StaticHelper.GetSerializeStatic<List<ushort>, UInt16ListFormatter>(), StaticHelper.GetDeserializeStatic<List<ushort>, UInt16ListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  uint>), StaticHelper.GetSerializeStatic<List<  uint>?, UInt32ListFormatter>(), StaticHelper.GetDeserializeStatic<List<  uint>?, UInt32ListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  uint>), StaticHelper.GetSerializeStatic<List<  uint>, UInt32ListFormatter>(), StaticHelper.GetDeserializeStatic<List<  uint>, UInt32ListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< ulong>), StaticHelper.GetSerializeStatic<List< ulong>?, UInt64ListFormatter>(), StaticHelper.GetDeserializeStatic<List< ulong>?, UInt64ListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< ulong>), StaticHelper.GetSerializeStatic<List< ulong>, UInt64ListFormatter>(), StaticHelper.GetDeserializeStatic<List< ulong>, UInt64ListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< float>), StaticHelper.GetSerializeStatic<List< float>?, SingleListFormatter>(), StaticHelper.GetDeserializeStatic<List< float>?, SingleListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List< float>), StaticHelper.GetSerializeStatic<List< float>, SingleListFormatter>(), StaticHelper.GetDeserializeStatic<List< float>, SingleListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<double>), StaticHelper.GetSerializeStatic<List<double>?, DoubleListFormatter>(), StaticHelper.GetDeserializeStatic<List<double>?, DoubleListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<double>), StaticHelper.GetSerializeStatic<List<double>, DoubleListFormatter>(), StaticHelper.GetDeserializeStatic<List<double>, DoubleListFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  bool>), StaticHelper.GetSerializeStatic<List<  bool>?, BooleanListFormatter>(), StaticHelper.GetDeserializeStatic<List<  bool>?, BooleanListFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<  bool>), StaticHelper.GetSerializeStatic<List<  bool>, BooleanListFormatter>(), StaticHelper.GetDeserializeStatic<List<  bool>, BooleanListFormatter>()),
#endif
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<  byte>), StaticHelper.GetSerializeStatic<Memory<  byte>, ByteMemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<  byte>, ByteMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<  char>), StaticHelper.GetSerializeStatic<Memory<  char>, CharMemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<  char>, CharMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory< short>), StaticHelper.GetSerializeStatic<Memory< short>, Int16MemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory< short>, Int16MemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<   int>), StaticHelper.GetSerializeStatic<Memory<   int>, Int32MemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<   int>, Int32MemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<  long>), StaticHelper.GetSerializeStatic<Memory<  long>, Int64MemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<  long>, Int64MemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory< sbyte>), StaticHelper.GetSerializeStatic<Memory< sbyte>, SByteMemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory< sbyte>, SByteMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<ushort>), StaticHelper.GetSerializeStatic<Memory<ushort>, UInt16MemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<ushort>, UInt16MemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<  uint>), StaticHelper.GetSerializeStatic<Memory<  uint>, UInt32MemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<  uint>, UInt32MemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory< ulong>), StaticHelper.GetSerializeStatic<Memory< ulong>, UInt64MemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory< ulong>, UInt64MemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory< float>), StaticHelper.GetSerializeStatic<Memory< float>, SingleMemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory< float>, SingleMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<double>), StaticHelper.GetSerializeStatic<Memory<double>, DoubleMemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<double>, DoubleMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Memory<  bool>), StaticHelper.GetSerializeStatic<Memory<  bool>, BooleanMemoryFormatter>(), StaticHelper.GetDeserializeStatic<Memory<  bool>, BooleanMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<  byte>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<  byte>, ByteReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<  byte>, ByteReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<  char>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<  char>, CharReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<  char>, CharReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory< short>), StaticHelper.GetSerializeStatic<ReadOnlyMemory< short>, Int16ReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory< short>, Int16ReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<   int>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<   int>, Int32ReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<   int>, Int32ReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<  long>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<  long>, Int64ReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<  long>, Int64ReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory< sbyte>), StaticHelper.GetSerializeStatic<ReadOnlyMemory< sbyte>, SByteReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory< sbyte>, SByteReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<ushort>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<ushort>, UInt16ReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<ushort>, UInt16ReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<  uint>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<  uint>, UInt32ReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<  uint>, UInt32ReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory< ulong>), StaticHelper.GetSerializeStatic<ReadOnlyMemory< ulong>, UInt64ReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory< ulong>, UInt64ReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory< float>), StaticHelper.GetSerializeStatic<ReadOnlyMemory< float>, SingleReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory< float>, SingleReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<double>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<double>, DoubleReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<double>, DoubleReadOnlyMemoryFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ReadOnlyMemory<  bool>), StaticHelper.GetSerializeStatic<ReadOnlyMemory<  bool>, BooleanReadOnlyMemoryFormatter>(), StaticHelper.GetDeserializeStatic<ReadOnlyMemory<  bool>, BooleanReadOnlyMemoryFormatter>()),
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Dictionary<string, string>), StaticHelper.GetSerializeStatic<Dictionary<string, string>?, DictionaryFormatter<string, string>>(), StaticHelper.GetDeserializeStatic<Dictionary<string, string>?, DictionaryFormatter<string, string>>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Dictionary<string, string>), StaticHelper.GetSerializeStatic<Dictionary<string, string>, DictionaryFormatter<string, string>>(), StaticHelper.GetDeserializeStatic<Dictionary<string, string>, DictionaryFormatter<string, string>>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(IDictionary<string, string>), StaticHelper.GetSerializeStatic<IDictionary<string, string>?, InterfaceDictionaryFormatter<string, string>>(), StaticHelper.GetDeserializeStatic<IDictionary<string, string>?, InterfaceDictionaryFormatter<string, string>>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(IDictionary<string, string>), StaticHelper.GetSerializeStatic<IDictionary<string, string>, InterfaceDictionaryFormatter<string, string>>(), StaticHelper.GetDeserializeStatic<IDictionary<string, string>, InterfaceDictionaryFormatter<string, string>>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(IDictionary<string, object>), StaticHelper.GetSerializeStatic<IDictionary<string, object>?, InterfaceDictionaryFormatter<string, object>>(), StaticHelper.GetDeserializeStatic<IDictionary<string, object>?, InterfaceDictionaryFormatter<string, object>>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(IDictionary<string, object>), StaticHelper.GetSerializeStatic<IDictionary<string, object>, InterfaceDictionaryFormatter<string, object>>(), StaticHelper.GetDeserializeStatic<IDictionary<string, object>, InterfaceDictionaryFormatter<string, object>>()),
#endif
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(BigInteger), StaticHelper.GetSerializeStatic<BigInteger, BigIntegerFormatter>(), StaticHelper.GetDeserializeStatic<BigInteger, BigIntegerFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(BigInteger?), StaticHelper.GetSerializeStatic<BigInteger?, NullableBigIntegerFormatter>(), StaticHelper.GetDeserializeStatic<BigInteger?, NullableBigIntegerFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Complex), StaticHelper.GetSerializeStatic<Complex, ComplexFormatter>(), StaticHelper.GetDeserializeStatic<Complex, ComplexFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Complex?), StaticHelper.GetSerializeStatic<Complex?, NullableComplexFormatter>(), StaticHelper.GetDeserializeStatic<Complex?, NullableComplexFormatter>()),
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Task), StaticHelper.GetSerializeStatic<Task?, TaskUnitFormatter>(), StaticHelper.GetDeserializeStatic<Task?, TaskUnitFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Task), StaticHelper.GetSerializeStatic<Task, TaskUnitFormatter>(), StaticHelper.GetDeserializeStatic<Task, TaskUnitFormatter>()),
#endif
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(      DateTime), StaticHelper.GetSerializeStatic<      DateTime, ISO8601DateTimeFormatter>(), StaticHelper.GetDeserializeStatic<      DateTime, ISO8601DateTimeFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(DateTimeOffset), StaticHelper.GetSerializeStatic<DateTimeOffset, ISO8601DateTimeOffsetFormatter>(), StaticHelper.GetDeserializeStatic<DateTimeOffset, ISO8601DateTimeOffsetFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(      TimeSpan), StaticHelper.GetSerializeStatic<      TimeSpan, ISO8601TimeSpanFormatter>(), StaticHelper.GetDeserializeStatic<      TimeSpan, ISO8601TimeSpanFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(      DateTime?), StaticHelper.GetSerializeStatic<      DateTime?, NullableISO8601DateTimeFormatter>(), StaticHelper.GetDeserializeStatic<      DateTime?, NullableISO8601DateTimeFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(DateTimeOffset?), StaticHelper.GetSerializeStatic<DateTimeOffset?, NullableISO8601DateTimeOffsetFormatter>(), StaticHelper.GetDeserializeStatic<DateTimeOffset?, NullableISO8601DateTimeOffsetFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(      TimeSpan?), StaticHelper.GetSerializeStatic<      TimeSpan?, NullableISO8601TimeSpanFormatter>(), StaticHelper.GetDeserializeStatic<      TimeSpan?, NullableISO8601TimeSpanFormatter>()),
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(string), StaticHelper.GetSerializeStatic<string?, NullableStringFormatter>(), StaticHelper.GetDeserializeStatic<string?, NullableStringFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(string), StaticHelper.GetSerializeStatic<string, NullableStringFormatter>(), StaticHelper.GetDeserializeStatic<string, NullableStringFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<string>), StaticHelper.GetSerializeStatic<List<string>?, ListFormatter<string>>(), StaticHelper.GetDeserializeStatic<List<string>?, ListFormatter<string>>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(List<string>), StaticHelper.GetSerializeStatic<List<string>, ListFormatter<string>>(), StaticHelper.GetDeserializeStatic<List<string>, ListFormatter<string>>()),
#endif
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(              Guid), StaticHelper.GetSerializeStatic<              Guid, GuidFormatter>(), StaticHelper.GetDeserializeStatic<              Guid, GuidFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ArraySegment<byte>), StaticHelper.GetSerializeStatic<ArraySegment<byte>, ArraySegmentFormatter<byte>>(), StaticHelper.GetDeserializeStatic<ArraySegment<byte>, ArraySegmentFormatter<byte>>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(decimal), StaticHelper.GetSerializeStatic<decimal, DecimalFormatter>(), StaticHelper.GetDeserializeStatic<decimal, DecimalFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(              Guid?), StaticHelper.GetSerializeStatic<              Guid?, NullableGuidFormatter>(), StaticHelper.GetDeserializeStatic<              Guid?, NullableGuidFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ArraySegment<byte>?), StaticHelper.GetSerializeStatic<ArraySegment<byte>?, NullableArraySegmentFormatter<byte>>(), StaticHelper.GetDeserializeStatic<ArraySegment<byte>?, NullableArraySegmentFormatter<byte>>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(           decimal?), StaticHelper.GetSerializeStatic<           decimal?, NullableDecimalFormatter>(), StaticHelper.GetDeserializeStatic<           decimal?, NullableDecimalFormatter>()),
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(StringBuilder), StaticHelper.GetSerializeStatic<StringBuilder?, StringBuilderFormatter>(), StaticHelper.GetDeserializeStatic<StringBuilder?, StringBuilderFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(StringBuilder), StaticHelper.GetSerializeStatic<StringBuilder, StringBuilderFormatter>(), StaticHelper.GetDeserializeStatic<StringBuilder, StringBuilderFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(BitArray), StaticHelper.GetSerializeStatic<BitArray?, BitArrayFormatter>(), StaticHelper.GetDeserializeStatic<BitArray?, BitArrayFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(BitArray), StaticHelper.GetSerializeStatic<BitArray, BitArrayFormatter>(), StaticHelper.GetDeserializeStatic<BitArray, BitArrayFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Type), StaticHelper.GetSerializeStatic<Type?, TypeFormatter>(), StaticHelper.GetDeserializeStatic<Type?, TypeFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Type), StaticHelper.GetSerializeStatic<Type, TypeFormatter>(), StaticHelper.GetDeserializeStatic<Type, TypeFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Uri), StaticHelper.GetSerializeStatic<Uri?, UriFormatter>(), StaticHelper.GetDeserializeStatic<Uri?, UriFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Uri), StaticHelper.GetSerializeStatic<Uri, UriFormatter>(), StaticHelper.GetDeserializeStatic<Uri, UriFormatter>()),
#endif
#if CSHARP_8_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Version), StaticHelper.GetSerializeStatic<Version?, VersionFormatter>(), StaticHelper.GetDeserializeStatic<Version?, VersionFormatter>()),
#else
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(Version), StaticHelper.GetSerializeStatic<Version, VersionFormatter>(), StaticHelper.GetDeserializeStatic<Version, VersionFormatter>()),
#endif


#if UNITY_2018_4_OR_NEWER
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<  byte>), StaticHelper.GetSerializeStatic<NativeArray<  byte>, ByteNativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<  byte>, ByteNativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<  char>), StaticHelper.GetSerializeStatic<NativeArray<  char>, CharNativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<  char>, CharNativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray< short>), StaticHelper.GetSerializeStatic<NativeArray< short>, Int16NativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray< short>, Int16NativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<   int>), StaticHelper.GetSerializeStatic<NativeArray<   int>, Int32NativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<   int>, Int32NativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<  long>), StaticHelper.GetSerializeStatic<NativeArray<  long>, Int64NativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<  long>, Int64NativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray< sbyte>), StaticHelper.GetSerializeStatic<NativeArray< sbyte>, SByteNativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray< sbyte>, SByteNativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<ushort>), StaticHelper.GetSerializeStatic<NativeArray<ushort>, UInt16NativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<ushort>, UInt16NativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<  uint>), StaticHelper.GetSerializeStatic<NativeArray<  uint>, UInt32NativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<  uint>, UInt32NativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray< ulong>), StaticHelper.GetSerializeStatic<NativeArray< ulong>, UInt64NativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray< ulong>, UInt64NativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray< float>), StaticHelper.GetSerializeStatic<NativeArray< float>, SingleNativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray< float>, SingleNativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<double>), StaticHelper.GetSerializeStatic<NativeArray<double>, DoubleNativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<double>, DoubleNativeArrayFormatter>()),
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(NativeArray<  bool>), StaticHelper.GetSerializeStatic<NativeArray<  bool>, BooleanNativeArrayFormatter>(), StaticHelper.GetDeserializeStatic<NativeArray<  bool>, BooleanNativeArrayFormatter>()),
#endif

#if !ENABLE_IL2CPP
                new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(ExpandoObject), StaticHelper.GetSerializeStatic<ExpandoObject, ExpandoObjectFormatter>(), StaticHelper.GetDeserializeStatic<ExpandoObject, ExpandoObjectFormatter>()),
#endif
            }, 0.5d);
#endregion

            internal static ThreadSafeTypeKeyFormatterHashTable.FunctionPair GetFunctionPointers(Type t)
            {
                return formatterHashTable[t];
            }
        }
    }
}
