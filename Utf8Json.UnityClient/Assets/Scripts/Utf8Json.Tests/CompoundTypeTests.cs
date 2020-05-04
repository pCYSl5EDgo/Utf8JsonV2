// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
[assembly: Utf8Json.RegisterTargetType(typeof(Utf8Json.Test.CompoundTypeTests.Int32Int32Tuple), 0)]
[assembly: Utf8Json.RegisterTargetType(typeof(Utf8Json.Test.CompoundTypeTests.Int64Int64Tuple), 1)]
[assembly: Utf8Json.RegisterTargetType(typeof(Utf8Json.Test.CompoundTypeTests.CompoundType), 2)]

namespace Utf8Json.Test
{
    [TestFixture]
    public class CompoundTypeTests
    {
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            JsonSerializer.DefaultOptions.PrepareJsonFormatter();
        }

        [TestCase(0, 0, 0L, 0L)]
        [TestCase(10, -100, 1024L, -556L)]
        [TestCase(int.MaxValue, int.MinValue, long.MaxValue, long.MinValue)]
        public void CompoundTest(int x, int y, long a, long b)
        {
            var value = new CompoundType(x, y, a, b);
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<CompoundType>(bytes);
            TestContext.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
            Assert.AreEqual(x, deserialize.t32.X);
            Assert.AreEqual(y, deserialize.t32.Y);
            Assert.AreEqual(a, deserialize.t64.A);
            Assert.AreEqual(b, deserialize.t64.B);
        }

        public struct Int32Int32Tuple
        {
            public int X;
            public int Y;
        }

        public class Int64Int64Tuple
        {
            public long A { get; set; }
            public long B { get; }

            [SerializationConstructor]
            public Int64Int64Tuple(long a, long b)
            {
                A = a;
                B = b;
            }
        }

        public sealed class CompoundType
        {
            public readonly Int32Int32Tuple t32;
            public readonly Int64Int64Tuple t64;
            private CompoundType() { }

            public CompoundType(int f, int g, long x, long y)
            {
                t32 = new Int32Int32Tuple { X = f, Y = g };
                t64 = new Int64Int64Tuple(x, y);
            }
        }
    }
}
