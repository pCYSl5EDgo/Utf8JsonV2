// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using UnityEngine;

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

        public class Z
        {
            public X FFF;

            public Y ときはきた { get; set; }
        }

        [Test]
        public void LayoutKindTest()
        {
            var dictionary = new Dictionary<LayoutKind, BindingFlags>
            {
                { LayoutKind.Explicit, BindingFlags.Static },
            };
            var bytes = JsonSerializer.Serialize(dictionary);
            var deserialize = JsonSerializer.Deserialize<Dictionary<LayoutKind, BindingFlags>>(bytes);
            Assert.IsTrue(deserialize.ContainsKey(LayoutKind.Explicit));
            Assert.IsTrue((deserialize[LayoutKind.Explicit] & BindingFlags.Static) != 0);
            Assert.AreEqual(1, deserialize.Count);
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
            TestContext.WriteLine(Encoding.UTF8.GetString(bytes));
            var deserialize = JsonSerializer.Deserialize<Y>(bytes);
            Assert.AreEqual(value, deserialize.A);
        }

        [TestCase(0, 0, default(string))]
        [TestCase(int.MinValue, int.MaxValue, "")]
        [TestCase(33, 4, "TNOK")]
        public void ZTest(int xc, int xb, string ya)
        {
            var value = new Z
            {
                FFF = new X(xc, xb),
                ときはきた = new Y
                {
                    A = ya,
                }
            };
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<Z>(bytes);
            Assert.AreEqual(deserialize.ときはきた.A, ya);
            Assert.AreEqual(deserialize.FFF.c, xc);
            Assert.IsTrue(deserialize.FFF.SameB(xb));
        }
    }
}

#if !UNITY_2018_4_OR_NEWER
namespace UnityEngine
{
    public sealed class SerializeField : System.Attribute
    {
    }
}
#endif
