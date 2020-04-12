// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using Utf8Json.Resolvers;

namespace Utf8Json.Test
{
    public class BaseTypeTests
    {
        [Test]
        public void DefaultOptionIsNotNull()
        {
            Assert.IsNotNull(JsonSerializer.DefaultOptions);
        }

        [Test]
        public void DefaultOptionResolverIsStandardResolver()
        {
            Assert.IsTrue(JsonSerializer.DefaultOptions.Resolver is StandardResolver);
        }

        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(1)]
        [TestCase(334)]
        [TestCase(-1145141919)]
        [TestCase(810)]
        public void SameInt32(int value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<int>(bytes);
            Assert.AreEqual(value, deserialize);
        }

        [TestCase(0L)]
        [TestCase((long)int.MaxValue)]
        [TestCase((long)int.MinValue)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        [TestCase(1L)]
        [TestCase(334L)]
        [TestCase(-1145141919L)]
        [TestCase(810L)]
        public void SameInt64(long value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<long>(bytes);
            Assert.AreEqual(value, deserialize);
        }

        [TestCase(' ')]
        [TestCase('A')]
        [TestCase('a')]
        [TestCase('a')]
        [TestCase('1')]
        [TestCase('\r')]
        [TestCase('\n')]
        [TestCase('\\')]
        [TestCase('\b')]
        [TestCase('\t')]
        [TestCase('\f')]
        [TestCase('/')]
        [TestCase('‚ ')]
        [TestCase('²')]
        [TestCase('“Ê')]
        public void SameChar(char value)
        {
            var bytes = JsonSerializer.Serialize(value);
            Console.WriteLine(bytes.ToArrayString());
            var deserialize = JsonSerializer.Deserialize<char>(bytes);
            Assert.AreEqual(value, deserialize);
        }
    }
}
