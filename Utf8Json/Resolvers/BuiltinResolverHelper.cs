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
                FromTypeToMethodHandles.GetEntry<  byte, ByteFormatter>(),
                FromTypeToMethodHandles.GetEntry<  char, CharFormatter>(),
                FromTypeToMethodHandles.GetEntry< short, Int16Formatter>(),
                FromTypeToMethodHandles.GetEntry<   int, Int32Formatter>(),
                FromTypeToMethodHandles.GetEntry<  long, Int64Formatter>(),
                FromTypeToMethodHandles.GetEntry< sbyte, SByteFormatter>(),
                FromTypeToMethodHandles.GetEntry<ushort, UInt16Formatter>(),
                FromTypeToMethodHandles.GetEntry<  uint, UInt32Formatter>(),
                FromTypeToMethodHandles.GetEntry< ulong, UInt64Formatter>(),
                FromTypeToMethodHandles.GetEntry< float, SingleFormatter>(),
                FromTypeToMethodHandles.GetEntry<double, DoubleFormatter>(),
                FromTypeToMethodHandles.GetEntry<  bool, BooleanFormatter>(),
                FromTypeToMethodHandles.GetEntry<  char?, NullableCharFormatter>(),
                FromTypeToMethodHandles.GetEntry<  byte?, NullableByteFormatter>(),
                FromTypeToMethodHandles.GetEntry< short?, NullableInt16Formatter>(),
                FromTypeToMethodHandles.GetEntry< int?  , NullableInt32Formatter>(),
                FromTypeToMethodHandles.GetEntry< long?  , NullableInt64Formatter>(),
                FromTypeToMethodHandles.GetEntry< ushort?, NullableUInt16Formatter>(),
                FromTypeToMethodHandles.GetEntry<   uint?, NullableUInt32Formatter>(),
                FromTypeToMethodHandles.GetEntry<  ulong?, NullableUInt64Formatter>(),
                FromTypeToMethodHandles.GetEntry< sbyte?, NullableSByteFormatter>(),
                FromTypeToMethodHandles.GetEntry< float?, NullableSingleFormatter>(),
                FromTypeToMethodHandles.GetEntry<double?, NullableDoubleFormatter>(),
                FromTypeToMethodHandles.GetEntry<  bool?, NullableBooleanFormatter>(),
                FromTypeToMethodHandles.GetEntry<  byte[], ByteArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<  char[], CharArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry< sbyte[], SByteArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry< short[], Int16ArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<   int[], Int32ArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<  long[], Int64ArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<ushort[], UInt16ArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<  uint[], UInt32ArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry< ulong[], UInt64ArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry< float[], SingleArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<double[], DoubleArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<  bool[], BooleanArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<  byte>, ByteListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<  char>, CharListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List< short>, Int16ListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<   int>, Int32ListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<  long>, Int64ListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List< sbyte>, SByteListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<ushort>, UInt16ListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<  uint>, UInt32ListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List< ulong>, UInt64ListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List< float>, SingleListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<double>, DoubleListFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<  bool>, BooleanListFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<  byte>, ByteMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<  char>, CharMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory< short>, Int16MemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<   int>, Int32MemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<  long>, Int64MemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory< sbyte>, SByteMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<ushort>, UInt16MemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<  uint>, UInt32MemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory< ulong>, UInt64MemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory< float>, SingleMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<double>, DoubleMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Memory<  bool>, BooleanMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<  byte>, ByteReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<  char>, CharReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory< short>, Int16ReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<   int>, Int32ReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<  long>, Int64ReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory< sbyte>, SByteReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<ushort>, UInt16ReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<  uint>, UInt32ReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory< ulong>, UInt64ReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory< float>, SingleReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<double>, DoubleReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<ReadOnlyMemory<  bool>, BooleanReadOnlyMemoryFormatter>(),
                FromTypeToMethodHandles.GetEntry<Dictionary<string, string>, DictionaryFormatter<string, string>>(),
                FromTypeToMethodHandles.GetEntry<IDictionary<string, string>, InterfaceDictionaryFormatter<string, string>>(),
                FromTypeToMethodHandles.GetEntry<IDictionary<string, object>, InterfaceDictionaryFormatter<string, object>>(),
                FromTypeToMethodHandles.GetEntry<BigInteger, BigIntegerFormatter>(),
                FromTypeToMethodHandles.GetEntry<BigInteger?, NullableBigIntegerFormatter>(),
                FromTypeToMethodHandles.GetEntry<Complex, ComplexFormatter>(),
                FromTypeToMethodHandles.GetEntry<Complex?, NullableComplexFormatter>(),
                FromTypeToMethodHandles.GetEntry<Task, TaskUnitFormatter>(),
                FromTypeToMethodHandles.GetEntry<      DateTime, ISO8601DateTimeFormatter>(),
                FromTypeToMethodHandles.GetEntry<DateTimeOffset, ISO8601DateTimeOffsetFormatter>(),
                FromTypeToMethodHandles.GetEntry<      TimeSpan, ISO8601TimeSpanFormatter>(),
                FromTypeToMethodHandles.GetEntry<      DateTime?, NullableISO8601DateTimeFormatter>(),
                FromTypeToMethodHandles.GetEntry<DateTimeOffset?, NullableISO8601DateTimeOffsetFormatter>(),
                FromTypeToMethodHandles.GetEntry<      TimeSpan?, NullableISO8601TimeSpanFormatter>(),
                FromTypeToMethodHandles.GetEntry<string, NullableStringFormatter>(),
                FromTypeToMethodHandles.GetEntry<string[], NullableStringArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<List<string>, ListFormatter<string>>(),
                FromTypeToMethodHandles.GetEntry<              Guid, GuidFormatter>(),
                FromTypeToMethodHandles.GetEntry<ArraySegment<byte>, ArraySegmentFormatter<byte>>(),
                FromTypeToMethodHandles.GetEntry<decimal, DecimalFormatter>(),
                FromTypeToMethodHandles.GetEntry<              Guid?, NullableGuidFormatter>(),
                FromTypeToMethodHandles.GetEntry<ArraySegment<byte>?, NullableArraySegmentFormatter<byte>>(),
                FromTypeToMethodHandles.GetEntry<           decimal?, NullableDecimalFormatter>(),
                FromTypeToMethodHandles.GetEntry<StringBuilder, StringBuilderFormatter>(),
                FromTypeToMethodHandles.GetEntry<BitArray, BitArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<Type, TypeFormatter>(),
                FromTypeToMethodHandles.GetEntry<Uri, UriFormatter>(),
                FromTypeToMethodHandles.GetEntry<Version, VersionFormatter>(),

#if UNITY_2018_4_OR_NEWER
                FromTypeToMethodHandles.GetEntry<NativeArray<  byte>, ByteNativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<  char>, CharNativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray< short>, Int16NativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<   int>, Int32NativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<  long>, Int64NativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray< sbyte>, SByteNativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<ushort>, UInt16NativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<  uint>, UInt32NativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray< ulong>, UInt64NativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray< float>, SingleNativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<double>, DoubleNativeArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<NativeArray<  bool>, BooleanNativeArrayFormatter>(),
#endif

#if !ENABLE_IL2CPP
                FromTypeToMethodHandles.GetEntry<ExpandoObject, ExpandoObjectFormatter>(),
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
