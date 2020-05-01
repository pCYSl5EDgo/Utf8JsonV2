// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using UnityEngine;
using Utf8Json.Formatters;

namespace Utf8Json.Test
{
    public class NewlyDefinedValueTypeTests
    {
        private enum ByteEnum : byte
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum SByteEnum : sbyte
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum Int16Enum : short
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum UInt16Enum : ushort
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum Int32Enum : int
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum UInt32Enum : uint
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum Int64Enum : long
        {
            None,
            Help,
            Want,
            Queue,
        }

        private enum UInt64Enum : ulong
        {
            None,
            Help,
            Want,
            Queue,
        }

        public struct X
        {
            public int C { get; private set; }
            [SerializeField] private readonly int b;

            public X(int c, int b)
            {
                this.C = c;
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

        public class W
        {
            [JsonExtensionData]
            public Dictionary<string, object> Ext { get; } = new Dictionary<string, object>();

            [DataMember(Name = "MyName")]
            public int A { get; set; }

            [JsonFormatter(typeof(DateTimeFormatter))]
            public DateTime DateTimeNow { get; set; } = DateTime.Now;
        }

        [Test]
        public void SimpleEnumTest()
        {
            {
                const ByteEnum a = ByteEnum.Help;
                Assert.AreEqual(JsonSerializer.Deserialize<ByteEnum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const SByteEnum a = SByteEnum.None;
                Assert.AreEqual(JsonSerializer.Deserialize<SByteEnum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const Int16Enum a = Int16Enum.Queue;
                Assert.AreEqual(JsonSerializer.Deserialize<Int16Enum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const UInt16Enum a = UInt16Enum.Queue;
                Assert.AreEqual(JsonSerializer.Deserialize<UInt16Enum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const Int32Enum a = Int32Enum.Want;
                Assert.AreEqual(JsonSerializer.Deserialize<Int32Enum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const UInt32Enum a = UInt32Enum.Want;
                Assert.AreEqual(JsonSerializer.Deserialize<UInt32Enum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const Int64Enum a = (Int64Enum)long.MaxValue;
                Assert.AreEqual(JsonSerializer.Deserialize<Int64Enum>(JsonSerializer.Serialize(a)), a);
            }
            {
                const UInt64Enum a = (UInt64Enum)ulong.MaxValue;
                Assert.AreEqual(JsonSerializer.Deserialize<UInt64Enum>(JsonSerializer.Serialize(a)), a);
            }
        }

        //[Test]
        public void ExtensionDataTest()
        {
            var value = new W
            {
                A = 114514,
                Ext = { { "Hoge", LayoutKind.Auto }, { "Foo", 33 } }
            };
            var bytes = JsonSerializer.Serialize(value);
            TestContext.WriteLine(Encoding.UTF8.GetString(bytes));
            var deserialize = JsonSerializer.Deserialize<W>(bytes);
            Assert.AreEqual(value.A, deserialize.A);
        }

        [TestCase(LayoutKind.Explicit)]
        [TestCase(LayoutKind.Sequential)]
        [TestCase(LayoutKind.Auto)]
        public void SimpleEnumTest(LayoutKind value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<LayoutKind>(bytes);
            Assert.IsTrue(value == deserialize);
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

        private struct EmptyStruct
        {
        }

        private struct Callback : IBeforeSerializationCallback
        {
            public string A;

            [SerializeField] private string B;

            public string C { get; private set; }

            [NonSerialized] public int D;

            public bool ShouldSerializeB()
            {
                if (B is null) return false;
                return B.Length < 14;
            }

            [JsonExtensionData]
            public Dictionary<string, object> Dictionary { get; private set; }

            public Callback(string b)
            {
                A = "aaaa";
                B = b;
                Dictionary = new Dictionary<string, object>
                {
                    {"たいよう", 1},
                    {"つきかげ", "このみちをゆく"},
                    {"きたかぜ", new byte[] { 11, 12, 0, 255 }},
                };
                C = b + b;
                D = 9;
            }

            public void OnSerializing()
            {
                TestContext.WriteLine(B);
            }
        }

        [TestCase("どれだけしんじてもしんじてもうらぎられるんだ")]
        [TestCase("こんなに！")]
        public void CallbackTest(string v)
        {
            var value = new Callback(v);
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<Callback>(bytes);
            Assert.AreEqual(value.C, deserialize.C);
            Assert.AreEqual(value.A, deserialize.A);
            Assert.AreEqual(0, deserialize.D);
            Assert.AreEqual(value.Dictionary.Count, deserialize.Dictionary.Count);
        }

        [Test]
        public void EmptyStructTest()
        {
            var value = new EmptyStruct();
            var bytes = JsonSerializer.Serialize(value);
            JsonSerializer.Deserialize<EmptyStruct>(bytes);
        }

        [TestCase(0, 0)]
        [TestCase(1, 2)]
        [TestCase(33, -4)]
        public void XTest(int a, int b)
        {
            var value = new X(a, b);
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<X>(bytes);
            Assert.AreEqual(a, deserialize.C);
            Assert.IsTrue(deserialize.SameB(b));
        }

        [TestCase("")]
        [TestCase("null")]
        [TestCase(default(string))]
        [TestCase("大好きなのはあなた")]
        public void YTest(string value)
        {
            var bytes = JsonSerializer.Serialize(new Y { A = value });
            var deserialize = JsonSerializer.Deserialize<Y>(bytes);
            Assert.AreEqual(value, deserialize.A);
        }

        //[TestCase(0, 0, default(string))]
        //[TestCase(int.MinValue, int.MaxValue, "")]
        //[TestCase(33, 4, "TNOK")]
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
            Assert.AreEqual(deserialize.FFF.C, xc);
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
