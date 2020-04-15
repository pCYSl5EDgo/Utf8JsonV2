// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using NUnit.Framework;

namespace Utf8Json.Test
{
    public class BasicGenericTypeTests
    {
        [TestCase("", "")]
        [TestCase("fuga", "hoge")]
        [TestCase("\r\n", "\t\\\"")]
        [TestCase("つきかげ", "たいよう")]
        public void ValueTuple2Test(string value0, string value1)
        {
            var value = (value0, value1);
            var bytes = JsonSerializer.Serialize(value);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));
            var deserialize = JsonSerializer.Deserialize<(string, string)>(bytes);
            Console.WriteLine(deserialize);
            Assert.IsTrue(value == deserialize);
        }
    }
}
