// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Utf8Json.Formatters;
using Utf8Json.Internal;
using FP = Utf8Json.Internal.ThreadSafeTypeKeyFormatterHashTable.FunctionPair;

namespace Utf8Json.Resolvers
{
    /// <summary>
    /// Unity2018というかUnityのMono基盤はcalliについてジェネリクスをサポートしていない
    /// </summary>
    public sealed partial class BasicGenericsResolver
    {
        internal static class BasicGenericsResolverGetFormatterHelper
        {
            private static FP CreateFunctionPair(Type formatterType)
            {
                return new FP(StaticHelper.GetSerializeStatic(formatterType), StaticHelper.GetDeserializeStatic(formatterType), default, default);
            }

#if CSHARP_8_OR_NEWER
            private static Type? FindFormatterType(Type targetType, bool excludeReferenceType)
#else
            private static Type FindFormatterType(Type targetType, bool excludeReferenceType)
#endif
            {
                if (targetType.IsArray)
                {
                    return FindWhenArray(targetType, excludeReferenceType);
                }

                if (targetType.IsConstructedGenericType)
                {
                    return FindWhenConstructedGenericType(targetType, excludeReferenceType);
                }

                return default;
            }


#if CSHARP_8_OR_NEWER
            private static Type? FindWhenArray(Type targetType, bool excludeReferenceType)
#else
            private static Type FindWhenArray(Type targetType, bool excludeReferenceType)
#endif
            {
                Type arrayFormatterBase;
                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (targetType.GetArrayRank())
                {
                    case 1:
                        arrayFormatterBase = typeof(AddArrayFormatter<>);
                        break;
                    case 2:
                        arrayFormatterBase = typeof(Dimension2ArrayFormatter<>);
                        break;
                    case 3:
                        arrayFormatterBase = typeof(Dimension3ArrayFormatter<>);
                        break;
                    case 4:
                        arrayFormatterBase = typeof(Dimension4ArrayFormatter<>);
                        break;
                    case 5:
                        arrayFormatterBase = typeof(Dimension5ArrayFormatter<>);
                        break;
                    case 6:
                        arrayFormatterBase = typeof(Dimension6ArrayFormatter<>);
                        break;
                    case 7:
                        arrayFormatterBase = typeof(Dimension7ArrayFormatter<>);
                        break;
                    case 8:
                        arrayFormatterBase = typeof(Dimension8ArrayFormatter<>);
                        break;
                    case 9:
                        arrayFormatterBase = typeof(Dimension9ArrayFormatter<>);
                        break;
                    case 10:
                        arrayFormatterBase = typeof(Dimension10ArrayFormatter<>);
                        break;
                    case 11:
                        arrayFormatterBase = typeof(Dimension11ArrayFormatter<>);
                        break;
                    case 12:
                        arrayFormatterBase = typeof(Dimension12ArrayFormatter<>);
                        break;
                    case 13:
                        arrayFormatterBase = typeof(Dimension13ArrayFormatter<>);
                        break;
                    case 14:
                        arrayFormatterBase = typeof(Dimension14ArrayFormatter<>);
                        break;
                    case 15:
                        arrayFormatterBase = typeof(Dimension15ArrayFormatter<>);
                        break;
                    case 16:
                        arrayFormatterBase = typeof(Dimension16ArrayFormatter<>);
                        break;
                    case 17:
                        arrayFormatterBase = typeof(Dimension17ArrayFormatter<>);
                        break;
                    case 18:
                        arrayFormatterBase = typeof(Dimension18ArrayFormatter<>);
                        break;
                    case 19:
                        arrayFormatterBase = typeof(Dimension19ArrayFormatter<>);
                        break;
                    case 20:
                        arrayFormatterBase = typeof(Dimension20ArrayFormatter<>);
                        break;
                    case 21:
                        arrayFormatterBase = typeof(Dimension21ArrayFormatter<>);
                        break;
                    case 22:
                        arrayFormatterBase = typeof(Dimension22ArrayFormatter<>);
                        break;
                    case 23:
                        arrayFormatterBase = typeof(Dimension23ArrayFormatter<>);
                        break;
                    case 24:
                        arrayFormatterBase = typeof(Dimension24ArrayFormatter<>);
                        break;
                    case 25:
                        arrayFormatterBase = typeof(Dimension25ArrayFormatter<>);
                        break;
                    case 26:
                        arrayFormatterBase = typeof(Dimension26ArrayFormatter<>);
                        break;
                    case 27:
                        arrayFormatterBase = typeof(Dimension27ArrayFormatter<>);
                        break;
                    case 28:
                        arrayFormatterBase = typeof(Dimension28ArrayFormatter<>);
                        break;
                    case 29:
                        arrayFormatterBase = typeof(Dimension29ArrayFormatter<>);
                        break;
                    case 30:
                        arrayFormatterBase = typeof(Dimension30ArrayFormatter<>);
                        break;
                    case 31:
                        arrayFormatterBase = typeof(Dimension31ArrayFormatter<>);
                        break;
                    case 32:
                        arrayFormatterBase = typeof(Dimension32ArrayFormatter<>);
                        break;
                    default: throw new ArgumentOutOfRangeException(targetType.GetArrayRank().ToString());
                }

                var elementType = targetType.GetElementType();
                if (elementType == null)
                {
                    return default;
                }

                if (excludeReferenceType && !elementType.IsValueType)
                {
                    return default;
                }

                var formatterType = arrayFormatterBase.MakeGenericType(elementType);
                return formatterType;
            }

#if CSHARP_8_OR_NEWER
            private static Type? FindWhenConstructedGenericType(Type targetType, bool excludeReferenceType)
#else
            private static Type FindWhenConstructedGenericType(Type targetType, bool excludeReferenceType)
#endif
            {
                var definition = targetType.GetGenericTypeDefinition();
                var definitionFullName = definition?.FullName;
                if (definitionFullName == null)
                {
                    return default;
                }

                var genericArguments = targetType.GetGenericArguments();


                if (excludeReferenceType)
                {
                    foreach (var genericArgument in genericArguments)
                    {
                        if (!genericArgument.IsValueType)
                        {
                            return default;
                        }
                    }
                }

                if (definitionFullName.Length == 19)
                {
                    Type baseFormatterType;
                    switch (definitionFullName)
                    {
                        case "System.ValueTuple`2":
                            System.Diagnostics.Debug.Assert(genericArguments.Length == 2);
                            baseFormatterType = typeof(ValueTupleFormatter<,>);
                            break;
                        case "System.ValueTuple`3":
                            System.Diagnostics.Debug.Assert(genericArguments.Length == 3);
                            baseFormatterType = typeof(ValueTupleFormatter<,,>);
                            break;
                        case "System.ValueTuple`4":
                            System.Diagnostics.Debug.Assert(genericArguments.Length == 4);
                            baseFormatterType = typeof(ValueTupleFormatter<,,,>);
                            break;
                        case "System.ValueTuple`5":
                            System.Diagnostics.Debug.Assert(genericArguments.Length == 5);
                            baseFormatterType = typeof(ValueTupleFormatter<,,,,>);
                            break;
                        case "System.ValueTuple`6":
                            System.Diagnostics.Debug.Assert(genericArguments.Length == 6);
                            baseFormatterType = typeof(ValueTupleFormatter<,,,,,>);
                            break;
                        case "System.ValueTuple`7":
                            System.Diagnostics.Debug.Assert(genericArguments.Length == 7);
                            baseFormatterType = typeof(ValueTupleFormatter<,,,,,,>);
                            break;
                        default: return default;
                    }

                    var formatterType = baseFormatterType.MakeGenericType(genericArguments);
                    return formatterType;
                }

                return default;
            }

#if UNITY_2018_4_OR_NEWER
            private const bool ExcludeReferenceType = true;
#else
            private const bool ExcludeReferenceType = false;
#endif

            public static FP GetFunctionPointers(Type targetType)
            {
                var firstFormatter = FindFormatterType(targetType, ExcludeReferenceType);
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (firstFormatter != null)
                {
                    return CreateFunctionPair(firstFormatter);
                }

                return default;
            }

#if UNITY_2018_4_OR_NEWER
#if CSHARP_8_OR_NEWER
            public static object? CreateFormatter(Type targetType)
#else
            public static object CreateFormatter(Type targetType)
#endif
            {
                var formatterType = FindFormatterType(targetType, false);
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (formatterType == null)
                {
                    return default;
                }

                return Activator.CreateInstance(formatterType);
            }
#endif
        }
    }
}
