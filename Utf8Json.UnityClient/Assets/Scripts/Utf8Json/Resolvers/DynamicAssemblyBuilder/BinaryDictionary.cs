// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Utf8Json.Internal;

namespace Utf8Json.Resolvers.DynamicAssemblyBuilder
{
    public sealed class BinaryDictionary
    {
        private readonly object lockObject = new object();
        private Entry[][] table;

        public BinaryDictionary()
        {
            table = new Entry[256][];
            for (var i = 0; i < table.Length; i++)
            {
                table[i] = Array.Empty<Entry>();
            }
        }

        public (FieldInfo field, int offset) GetOrAdd(ReadOnlySpan<byte> key, TypeBuilder typeBuilder)
        {
            if (key.IsEmpty)
            {
                throw new ArgumentException(typeBuilder.FullName, nameof(key));
            }

            var keyLength = key.Length;
            if (keyLength - 1 >= table.Length)
            {
                return Add(key, typeBuilder);
            }

            var entries = table[keyLength - 1];
            foreach (var entry in entries)
            {
                if (key.SequenceEqual(entry.Key))
                {
                    return (entry.Value, 0);
                }
            }

            return Add(key, typeBuilder);
        }

        private (FieldInfo field, int offset) Add(ReadOnlySpan<byte> key, TypeBuilder typeBuilder)
        {
            var hash = FarmHash.Hash64(key);
            var keyArray = key.ToArray();
            var field = typeBuilder.DefineInitializedData("<>" + hash.ToString("X16", CultureInfo.InvariantCulture) + "<>" + key.Length.ToString(CultureInfo.InvariantCulture), keyArray, FieldAttributes.Public | FieldAttributes.HasFieldRVA);
            var entry = new Entry(keyArray, field);
            lock (lockObject)
            {
                var keyLengthMinus1 = key.Length - 1;
                if (keyLengthMinus1 >= table.Length)
                {
                    var oldLength = table.Length;
                    Array.Resize(ref table, keyLengthMinus1 + 1);
                    for (var i = oldLength; i < keyLengthMinus1; i++)
                    {
                        table[i] = Array.Empty<Entry>();
                    }
                }

                HashTableHelper.Add(ref table[keyLengthMinus1], entry);
            }

            return (entry.Value, 0);
        }

        private readonly struct Entry
        {
            public readonly byte[] Key;
            public readonly FieldInfo Value;

            public Entry(byte[] key, FieldInfo value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
