// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace Utf8Json.Test
{
    public class BasicGenericTypeTests
    {
        [TestCase("", null)]
        [TestCase("", "")]
        [TestCase("fuga", "hoge")]
        [TestCase("\r\n", "\t\\\"")]
        [TestCase("つきかげ", "たいよう")]
        public void ValueTuple2Test(string value0, string value1)
        {
            var value = (value0, value1);
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<(string, string)>(bytes);
            Assert.IsTrue(value == deserialize);
        }

        [TestCase("", null)]
        [TestCase("", "")]
        [TestCase("fuga", "hoge")]
        [TestCase("\r\n", "\t\\\"")]
        [TestCase("つきかげ", "たいよう")]
        public void Dimension2ArrayTest(string value0, string value1)
        {
            var value = new[,] { { value0 }, { value1 } };
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<string[,]>(bytes);
            Assert.AreEqual(value.Rank, deserialize.Rank);
            Assert.AreEqual(value.GetLength(0), deserialize.GetLength(0));
            Assert.AreEqual(value.GetLength(1), deserialize.GetLength(1));
            Assert.AreEqual(value[0, 0], deserialize[0, 0]);
            Assert.AreEqual(value[1, 0], deserialize[1, 0]);
        }
    }
}
