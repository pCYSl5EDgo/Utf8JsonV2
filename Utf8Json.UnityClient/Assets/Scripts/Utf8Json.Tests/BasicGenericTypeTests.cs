// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Collections.Generic;

namespace Utf8Json.Test
{
    [TestFixture]
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

        [Test]
        public void DictionaryTest()
        {
            var key0 = "ワレワレハウチュウジンダ";
            var key1 = "友だちになろう\r\n敵は彼にしよう。";
            var value = new Dictionary<string, int>()
            {
                { key0, 1145141919 },
                { key1, -19 },
            };
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<Dictionary<string, int>>(bytes);
            Assert.AreEqual(2, deserialize.Count);
            Assert.AreEqual(value[key0], deserialize[key0]);
            Assert.AreEqual(value[key1], deserialize[key1]);
        }

        [Test]
        public void InterfaceDictionaryTest()
        {
            var key0 = "ワレワレハウチュウジンダ";
            var key1 = "友だちになろう\r\n敵は彼にしよう。";
            var value = new Dictionary<string, int>()
            {
                { key0, 1145141919 },
                { key1, -19 },
            };
            var bytes = JsonSerializer.Serialize<IDictionary<string, int>>(value);
            var deserialize = JsonSerializer.Deserialize<IDictionary<string, int>>(bytes);
            Assert.AreEqual(2, deserialize.Count);
            Assert.AreEqual(value[key0], deserialize[key0]);
            Assert.AreEqual(value[key1], deserialize[key1]);
        }
    }
}
