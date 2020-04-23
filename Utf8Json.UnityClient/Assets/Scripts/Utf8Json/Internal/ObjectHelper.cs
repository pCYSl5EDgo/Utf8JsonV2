// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable RedundantExplicitArraySize

using System;

namespace Utf8Json.Internal
{
    public static class ObjectHelper
    {
        public static readonly object Object;
        public static readonly object True;
        public static readonly object False;
        public static readonly object Char;
        public static readonly object Single;
        public static readonly object Double;
        public static readonly object IntPtr;
        public static readonly object UIntPtr;

        public static readonly object[] ByteArray;
        public static readonly object[] SByteArray;
        public static readonly object[] Int16Array;
        public static readonly object[] UInt16Array;
        public static readonly object[] UInt32Array;
        public static readonly object[] Int32Array;
        public static readonly object[] UInt64Array;
        public static readonly object[] Int64Array;

        public static readonly ThreadSafeTypeKeyReferenceHashTable<object> ValueTypeDefaultValueHashTable;

        static ObjectHelper()
        {
            Object = new object();
            True = true;
            False = false;
            Char = '\0';
            Single = 0f;
            Double = 0d;
            IntPtr = System.IntPtr.Zero;
            UIntPtr = System.UIntPtr.Zero;
            ByteArray = new object[256];
            SByteArray = new object[256];
            Int16Array = new object[257];
            UInt16Array = new object[256];
            Int32Array = new object[257];
            UInt32Array = new object[256];
            Int64Array = new object[257];
            UInt64Array = new object[256];
            for (short i = 0; i < 256; i++)
            {
                ByteArray[i] = (byte)i;
                SByteArray[i] = (sbyte)(byte)i;
                Int16Array[i] = i;
                UInt16Array[i] = (ushort)i;
                Int32Array[i] = (int)i;
                UInt32Array[i] = (uint)i;
                Int64Array[i] = (long)i;
                UInt64Array[i] = (ulong)i;
            }
            Int16Array[256] = (short)-1;
            Int32Array[256] = -1;
            Int64Array[256] = -1L;

            ValueTypeDefaultValueHashTable = new ThreadSafeTypeKeyReferenceHashTable<object>(new[]
            {
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(object), Object),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(sbyte), SByteArray[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(byte), ByteArray[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(short), Int16Array[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(ushort), UInt16Array[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(int), Int32Array[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(uint), UInt32Array[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(long), Int64Array[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(ulong), UInt64Array[0]),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(char), Char),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(bool), False),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(float), Single),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(double), Double),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(IntPtr), IntPtr),
                new ThreadSafeTypeKeyReferenceHashTable<object>.Entry(typeof(UIntPtr), UIntPtr),
            }, 0.5D);
        }

        public static object
#if CSHARP_8_OR_NEWER
            ?
#endif
            DefaultValueFactory(Type arg)
        {
            return Activator.CreateInstance(arg);
        }
    }
}