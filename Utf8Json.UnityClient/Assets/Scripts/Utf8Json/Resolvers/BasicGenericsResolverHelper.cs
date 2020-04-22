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
                        arrayFormatterBase = typeof(ArrayFormatter<>);
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
                var definitionFullName = definition.FullName;
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

                Type formatterType;
                switch (definitionFullName)
                {
                    case "System.Collections.ObjectModel.ReadOnlyDictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeyReadOnlyDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(ReadOnlyDictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
                    case "System.Collections.Generic.IDictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeyInterfaceDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(InterfaceDictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
                    case "System.Collections.Generic.Dictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeyDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(DictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
                    case "System.Collections.Generic.SortedDictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeySortedDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(SortedDictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
                    case "System.Collections.Generic.SortedList`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeySortedListFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(SortedListFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
                    case "System.Collections.Concurrent.ConcurrentDictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeyConcurrentDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(ConcurrentDictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
#if IMMUTABLE
                    case "System.Collections.Immutable.ImmutableDictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeyImmutableDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(ImmutableDictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
                    case "System.Collections.Immutable.ImmutableSortedDictionary`2":
                        if (genericArguments[0] == typeof(string))
                        {
                            formatterType = typeof(StringKeyImmutableSortedDictionaryFormatter<>).MakeGenericType(genericArguments[1]);
                        }
                        else
                        {
                            formatterType = typeof(ImmutableSortedDictionaryFormatter<,>).MakeGenericType(genericArguments);
                        }
                        break;
#endif
                    case "System.Collections.Generic.IEnumerable`1":
                        formatterType = typeof(InterfaceEnumerableFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.IList`1":
                        formatterType = typeof(InterfaceListFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.ICollection`1":
                        formatterType = typeof(InterfaceCollectionFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.ILookup`2":
                        formatterType = typeof(InterfaceLookupFormatter<,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.IGrouping`2":
                        formatterType = typeof(InterfaceGroupingFormatter<,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.KeyValuePair`2":
                        formatterType = typeof(KeyValuePairFormatter<,>).MakeGenericType(genericArguments);
                        break;
                    case "System.ArraySegment`1":
                        formatterType = typeof(ArraySegmentFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Threading.Tasks.ValueTask`1":
                        formatterType = typeof(ValueTaskValueFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Threading.Task`1":
                        formatterType = typeof(TaskValueFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Nullable`1":
                        formatterType = typeof(NullableFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.Queue`1":
                        formatterType = typeof(QueueFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.Stack`1":
                        formatterType = typeof(StackFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.HashSet`1":
                        formatterType = typeof(HashSetFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.LinkedList`1":
                        formatterType = typeof(LinkedListFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.Generic.List`1":
                        formatterType = typeof(ListFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.Collections.ObjectModel.ReadOnlyCollection`1":
                        formatterType = typeof(ReadOnlyCollectionFormatter<>).MakeGenericType(genericArguments);
                        break;
                    case "System.ValueTuple`2":
                        formatterType = typeof(ValueTupleFormatter<,>).MakeGenericType(genericArguments);
                        break;
                    case "System.ValueTuple`3":
                        formatterType = typeof(ValueTupleFormatter<,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.ValueTuple`4":
                        formatterType = typeof(ValueTupleFormatter<,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.ValueTuple`5":
                        formatterType = typeof(ValueTupleFormatter<,,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.ValueTuple`6":
                        formatterType = typeof(ValueTupleFormatter<,,,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.ValueTuple`7":
                        formatterType = typeof(ValueTupleFormatter<,,,,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Tuple`2":
                        formatterType = typeof(TupleFormatter<,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Tuple`3":
                        formatterType = typeof(TupleFormatter<,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Tuple`4":
                        formatterType = typeof(TupleFormatter<,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Tuple`5":
                        formatterType = typeof(TupleFormatter<,,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Tuple`6":
                        formatterType = typeof(TupleFormatter<,,,,,>).MakeGenericType(genericArguments);
                        break;
                    case "System.Tuple`7":
                        formatterType = typeof(TupleFormatter<,,,,,,>).MakeGenericType(genericArguments);
                        break;
#if UNITY_2018_4_OR_NEWER
                    case "Unity.Collections.NativeArray`1":
                        formatterType = typeof(NativeArrayFormatter<>).MakeGenericType(genericArguments);
                        break;
#endif
                    default:
                        return MatchInterface(targetType);
                }

                return formatterType;
            }

#if CSHARP_8_OR_NEWER
            private static Type? MatchInterface(Type targetType)
#else
            private static Type MatchInterface(Type targetType)
#endif
            {
                var interfaces = targetType.GetInterfaces();
                if (interfaces.Length == 0)
                {
                    return default;
                }

                var interfaceDefinitions = new Type[interfaces.Length];
                for (var index = 0; index < interfaces.Length; index++)
                {
                    var @interface = interfaces[index];
                    if (@interface.IsGenericType && !@interface.IsGenericTypeDefinition)
                    {
                        interfaceDefinitions[index] = @interface.GetGenericTypeDefinition();
                    }
                    else
                    {
                        interfaceDefinitions[index] = @interface;
                    }
                }

                var answer = TryMatchInterfaceDictionary(targetType, interfaceDefinitions, interfaces);
                if (answer != null)
                {
                    return answer;
                }

                return default;
            }

#if CSHARP_8_OR_NEWER
            private static Type? TryMatchInterfaceDictionary(Type targetType, Type[] interfaceDefinitions, Type[] interfaces)
#else
            private static Type TryMatchInterfaceDictionary(Type targetType, Type[] interfaceDefinitions, Type[] interfaces)
#endif
            {
                if (targetType.IsInterface)
                {
                    return default;
                }

                var defaultConstructor = targetType.GetConstructor(Array.Empty<Type>());
                if (defaultConstructor == null)
                {
                    return default;
                }

                for (var index = 0; index < interfaceDefinitions.Length; index++)
                {
                    var genericTypeDefinition = interfaceDefinitions[index];
                    if (genericTypeDefinition.FullName != "System.Collections.Generic.IDictionary`2")
                    {
                        continue;
                    }

                    var arguments = interfaces[index].GetGenericArguments();
                    return typeof(GenericDictionaryFormatter<,,>).MakeGenericType(targetType, arguments[0], arguments[1]);
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
        }
    }
}
