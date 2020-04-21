// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
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

        [TestCase(true)]
        [TestCase(false)]
        public void SameBoolean(bool value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<bool>(bytes);
            Assert.AreEqual(value, deserialize);
        }

        [Test]
        public void SameByte()
        {
            for (byte value = 0; value < byte.MaxValue; value++)
            {
                var bytes = JsonSerializer.Serialize<byte>(value);
                var deserialize = JsonSerializer.Deserialize<byte>(bytes);
                Assert.IsTrue(value == deserialize);
            }

            {
                var bytes = JsonSerializer.Serialize<byte>(byte.MaxValue);
                var deserialize = JsonSerializer.Deserialize<byte>(bytes);
                Assert.IsTrue(byte.MaxValue == deserialize);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(16)]
        [TestCase(256)]
        public void SameByteArray(int length)
        {
            var value = length == 0 ? Array.Empty<byte>() : new byte[length];
            for (var i = 0; i < value.Length; i++)
            {
                value[i] = (byte)i;
            }
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<byte[]>(bytes);
            Assert.IsTrue(value.SequenceEqual(deserialize));
        }

        [Test]
        public void SameSByte()
        {
            for (sbyte value = sbyte.MinValue; value < sbyte.MaxValue; value++)
            {
                var bytes = JsonSerializer.Serialize<sbyte>(value);
                var deserialize = JsonSerializer.Deserialize<sbyte>(bytes);
                Assert.IsTrue(value == deserialize);
            }

            {
                var bytes = JsonSerializer.Serialize<sbyte>(sbyte.MaxValue);
                Console.WriteLine(bytes.ToArrayString());
                var deserialize = JsonSerializer.Deserialize<sbyte>(bytes);
                Assert.IsTrue(sbyte.MaxValue == deserialize);
            }
        }

        [Test]
        public void SameUInt16()
        {
            for (ushort value = 0; value < ushort.MaxValue; value++)
            {
                var bytes = JsonSerializer.Serialize<ushort>(value);
                var deserialize = JsonSerializer.Deserialize<ushort>(bytes);
                Assert.AreEqual(value, deserialize);
            }

            {
                var bytes = JsonSerializer.Serialize(ushort.MaxValue);
                var deserialize = JsonSerializer.Deserialize<ushort>(bytes);
                Assert.AreEqual(ushort.MaxValue, deserialize);
            }
        }

        [Test]
        public void SameInt16()
        {
            for (short value = short.MinValue; value < short.MaxValue; value++)
            {
                var bytes = JsonSerializer.Serialize<short>(value);
                var deserialize = JsonSerializer.Deserialize<short>(bytes);
                Assert.AreEqual(value, deserialize);
            }

            {
                var bytes = JsonSerializer.Serialize<short>(short.MaxValue);
                var deserialize = JsonSerializer.Deserialize<short>(bytes);
                Assert.AreEqual(short.MaxValue, deserialize);
            }
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

        [TestCase(0U)]
        [TestCase(uint.MaxValue)]
        [TestCase(1U)]
        [TestCase(334U)]
        [TestCase(1145141919U)]
        [TestCase(810U)]
        [TestCase(unchecked((uint)-34))]
        public void SameUInt32(uint value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<uint>(bytes);
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

        [TestCase(0UL)]
        [TestCase((ulong)int.MaxValue)]
        [TestCase(unchecked((ulong)int.MinValue))]
        [TestCase((ulong)long.MaxValue)]
        [TestCase(unchecked((ulong)long.MinValue))]
        [TestCase(ulong.MaxValue)]
        [TestCase(1UL)]
        [TestCase(334UL)]
        [TestCase(unchecked((ulong)-1145141919L))]
        [TestCase(810UL)]
        public void SameUInt64(ulong value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<ulong>(bytes);
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
        [TestCase('\u0000')]
        [TestCase('\u0001')]
        [TestCase('\u0002')]
        [TestCase('\u0003')]
        [TestCase('\u0013')]
        [TestCase('\u0023')]
        [TestCase('\u0023')]
        [TestCase('\u0123')]
        [TestCase('\u0123')]
        [TestCase('\u0f23')]
        [TestCase('\u2f23')]
        [TestCase('\b')]
        [TestCase('\t')]
        [TestCase('\f')]
        [TestCase('/')]
        [TestCase('あ')]
        [TestCase('ｲ')]
        [TestCase('凸')]
        public void SameChar(char value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<char>(bytes);
            Assert.AreEqual(value, deserialize);
        }

        [TestCase("")]
        [TestCase(default(string))]
        [TestCase("とてつもなく大きな力が我々の前に立ちはだかるだろう。心せよ。探索者たちよ。")]
        [TestCase("\"\u0002\r\n\0\0\0\0\\")]
        [TestCase("とてつもな\\\\\\\\\r\nく大きな力が我々の前に立ちはだかるだろう。心せよ。探索者たちよ。")]
        public void SameString(string value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<string>(bytes);
            Assert.True(value == deserialize);
        }

        [Test]
        public void CanReadNull()
        {
            var bytes = JsonSerializer.Serialize<string>(null);
            var deserialize = JsonSerializer.Deserialize<string>(bytes);
            Assert.IsNull(deserialize);
        }
    }
}
