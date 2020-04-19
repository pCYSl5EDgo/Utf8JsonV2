// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using Utf8Json.Resolvers;

namespace Utf8Json.Test
{
    public class NewlyDefinedValueTypeTests
    {
        public struct X
        {
            public int c { get; set; }
            [SerializeField] private int b;

            [SerializationConstructor]
            public X(int C, int b)
            {
                this.c = C;
                this.b = b;
            }

            public bool SameB(int b) => this.b == b;
        }

        public struct Y
        {
            public string A { get; set; }
        }

        [TestCase(0, 0)]
        [TestCase(1, 2)]
        [TestCase(33, -4)]
        public void XTest(int a, int b)
        {
            var value = new X(a, b);
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<X>(bytes);
            Assert.AreEqual(a, deserialize.c);
            Assert.IsTrue(deserialize.SameB(b));
        }

        [TestCase("")]
        [TestCase("null")]
        [TestCase(default(string))]
        [TestCase("大好きなのはあなた")]
        public void YTest(string value)
        {
            var bytes = JsonSerializer.Serialize(new Y { A = value });
            Console.WriteLine(Encoding.UTF8.GetString(bytes));
            var deserialize = JsonSerializer.Deserialize<Y>(bytes);
            Assert.AreEqual(value, deserialize.A);
        }
    }
}

#if !UNITY_2018_4_OR_NEWER
namespace UnityEngine
{
    public sealed class SerializeFieldAttribute : System.Attribute
    {
    }
}
#endif
