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
                FromTypeToMethodHandles.GetEntry<System.Exception, ExceptionFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.MethodBase, MethodBaseFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.MethodInfo, MethodInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.ConstructorInfo, ConstructorInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.TypeInfo, TypeInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.FieldInfo, FieldInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.PropertyInfo, PropertyInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.MemberInfo, MemberInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.CustomAttributeData, CustomAttributeDataFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.CustomAttributeNamedArgument, CustomAttributeNamedArgumentFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.CustomAttributeTypedArgument, CustomAttributeTypedArgumentFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Reflection.ParameterInfo, ParameterInfoFormatter>(),
                FromTypeToMethodHandles.GetEntry<System.Globalization.CultureInfo, CultureInfoFormatter>(),

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

            private static readonly ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> formatters =  new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>(new[]
            {
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(  byte), new ByteFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(  char), new CharFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof( short), new Int16Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(   int), new Int32Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(  long), new Int64Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof( sbyte), new SByteFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(ushort), new UInt16Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(  uint), new UInt32Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof( ulong), new UInt64Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof( float), new SingleFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(double), new DoubleFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(  bool), new BooleanFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Numerics.BigInteger), new BigIntegerFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Numerics.Complex), new ComplexFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Threading.Tasks.Task), new TaskUnitFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(      System.DateTime), new ISO8601DateTimeFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.DateTimeOffset), new ISO8601DateTimeOffsetFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(      System.TimeSpan), new ISO8601TimeSpanFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(string), new NullableStringFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Guid), new GuidFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(decimal), new DecimalFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Text.StringBuilder), new StringBuilderFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Collections.BitArray), new BitArrayFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Type), new TypeFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Uri), new UriFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Version), new VersionFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Exception), new ExceptionFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.MethodBase), new MethodBaseFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.MethodInfo), new MethodInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.ConstructorInfo), new ConstructorInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.TypeInfo), new TypeInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.FieldInfo), new FieldInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.PropertyInfo), new PropertyInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.MemberInfo), new MemberInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.CustomAttributeData), new CustomAttributeDataFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.CustomAttributeNamedArgument), new CustomAttributeNamedArgumentFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.CustomAttributeTypedArgument), new CustomAttributeTypedArgumentFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Reflection.ParameterInfo), new ParameterInfoFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Globalization.CultureInfo), new CultureInfoFormatter()),

#if UNITY_2018_4_OR_NEWER
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(      UnityEngine.Rect), new RectFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(     UnityEngine.Color), new ColorFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(    UnityEngine.Bounds), new BoundsFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(   UnityEngine.Vector2), new Vector2Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(   UnityEngine.Vector3), new Vector3Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(   UnityEngine.Vector4), new Vector4Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(   UnityEngine.RectInt), new RectIntFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(   UnityEngine.Color32), new Color32Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof( UnityEngine.Matrix4x4), new Matrix4x4Formatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(UnityEngine.Vector2Int), new Vector2IntFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(UnityEngine.Vector3Int), new Vector3IntFormatter()),
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(UnityEngine.Quaternion), new QuaternionFormatter()),
#endif

#if !ENABLE_IL2CPP
                new ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter>.Entry(typeof(System.Dynamic.ExpandoObject), new ExpandoObjectFormatter()),
#endif
            }, 0.5d);
            
            internal static ThreadSafeTypeKeyReferenceHashTable<IJsonFormatter> GetFormatterCache() => formatters;
        }
    }
}
