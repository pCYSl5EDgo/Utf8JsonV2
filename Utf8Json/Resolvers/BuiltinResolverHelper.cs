// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Utf8Json.Formatters;
using Utf8Json.Internal;

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
                FromTypeToMethodHandles.GetEntry<System.Numerics.BigInteger, BigIntegerFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Numerics.Complex, ComplexFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Threading.Tasks.Task, TaskUnitFormatter>(),
                FromTypeToMethodHandles.GetEntry<      System.DateTime, ISO8601DateTimeFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.DateTimeOffset, ISO8601DateTimeOffsetFormatter>(),
                FromTypeToMethodHandles.GetEntry<      System.TimeSpan, ISO8601TimeSpanFormatter>(),
                FromTypeToMethodHandles.GetEntry<string, NullableStringFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Guid, GuidFormatter>(),
                FromTypeToMethodHandles.GetEntry<decimal, DecimalFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Text.StringBuilder, StringBuilderFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Collections.BitArray, BitArrayFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Type, TypeFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Uri, UriFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Version, VersionFormatter>(),

#if UNITY_2018_4_OR_NEWER
                FromTypeToMethodHandles.GetEntry<      UnityEngine.Rect, RectFormatter>(),
                FromTypeToMethodHandles.GetEntry<     UnityEngine.Color, ColorFormatter>(),
                FromTypeToMethodHandles.GetEntry<    UnityEngine.Bounds, BoundsFormatter>(),
                FromTypeToMethodHandles.GetEntry<   UnityEngine.Vector2, Vector2Formatter>(),
                FromTypeToMethodHandles.GetEntry<   UnityEngine.Vector3, Vector3Formatter>(),
                FromTypeToMethodHandles.GetEntry<   UnityEngine.Vector4, Vector4Formatter>(),
                FromTypeToMethodHandles.GetEntry<   UnityEngine.RectInt, RectIntFormatter>(),
                FromTypeToMethodHandles.GetEntry<   UnityEngine.Color32, Color32Formatter>(),
                FromTypeToMethodHandles.GetEntry< UnityEngine.Matrix4x4, Matrix4x4Formatter>(),
                FromTypeToMethodHandles.GetEntry<UnityEngine.Vector2Int, Vector2IntFormatter>(),
                FromTypeToMethodHandles.GetEntry<UnityEngine.Vector3Int, Vector3IntFormatter>(),
                FromTypeToMethodHandles.GetEntry<UnityEngine.Quaternion, QuaternionFormatter>(),
#endif

#if !ENABLE_IL2CPP
                FromTypeToMethodHandles.GetEntry<System.Dynamic.ExpandoObject, ExpandoObjectFormatter>(),
#endif
            }, 0.5d);
#endregion

            internal static ThreadSafeTypeKeyFormatterHashTable.FunctionPair GetFunctionPointers(System.Type t)
            {
                return formatterHashTable[t];
            }
        }
    }
}
