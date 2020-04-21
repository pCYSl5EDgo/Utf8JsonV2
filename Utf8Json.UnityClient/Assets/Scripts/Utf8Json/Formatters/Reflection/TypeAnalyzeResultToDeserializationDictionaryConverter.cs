// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public static class TypeAnalyzeResultToDeserializationDictionaryConverter
    {
        public static DeserializationDictionary Convert(in TypeAnalyzeResult result)
        {
            var length = result.FieldReferenceTypeArray.Length
                                + result.FieldValueTypeArray.Length
                                + result.FieldReferenceTypeShouldSerializeArray.Length
                                + result.FieldValueTypeShouldSerializeArray.Length
                                + result.PropertyReferenceTypeArray.Length
                                + result.PropertyValueTypeArray.Length
                                + result.PropertyReferenceTypeShouldSerializeArray.Length
                                + result.PropertyValueTypeShouldSerializeArray.Length;
            var array = ArrayPool<DeserializationDictionary.Setter>.Shared.Rent(length);
            var count = 0;
            try
            {
                foreach (ref readonly var info in result.FieldReferenceTypeArray.AsSpan())
                {
                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.FieldValueTypeArray.AsSpan())
                {
                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.FieldReferenceTypeShouldSerializeArray.AsSpan())
                {
                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.FieldValueTypeShouldSerializeArray.AsSpan())
                {
                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.PropertyReferenceTypeArray.AsSpan())
                {
                    if (!info.Info.CanWrite)
                    {
                        continue;
                    }

                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.PropertyValueTypeArray.AsSpan())
                {
                    if (!info.Info.CanWrite)
                    {
                        continue;
                    }

                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.PropertyReferenceTypeShouldSerializeArray.AsSpan())
                {
                    if (!info.Info.CanWrite)
                    {
                        continue;
                    }

                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                foreach (ref readonly var info in result.PropertyValueTypeShouldSerializeArray.AsSpan())
                {
                    if (!info.Info.CanWrite)
                    {
                        continue;
                    }

                    array[count++] = new DeserializationDictionary.Setter(info.Info, info.GetPropertyNameRaw().ToArray(), info.Formatter);
                }

                var answer = new DeserializationDictionary(array.AsSpan(0, count));
                return answer;
            }
            finally
            {
                ArrayPool<DeserializationDictionary.Setter>.Shared.Return(array);
            }
        }
    }
}
