// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utf8Json.Test
{
    [TestFixture]
    public class ReflectionTests
    {
        [TestCase(typeof(int))]
        [TestCase(typeof(IEnumerable<Dictionary<string, DateTime>>))]
        [TestCase(typeof(void))]
        public void TypeTest(Type value)
        {
            var bytes = JsonSerializer.Serialize(value);
            var deserialize = JsonSerializer.Deserialize<Type>(bytes);
            Assert.IsTrue(value == deserialize);
        }

        [Test]
        public void MethodTest()
        {
            var value = typeof(Console).GetMethod("WriteLine", new[] { typeof(int) });
            var bytes = JsonSerializer.Serialize(value);
            TestContext.WriteLine(Encoding.UTF8.GetString(bytes));
            var deserialize = JsonSerializer.Deserialize<MethodInfo>(bytes);
            Assert.IsTrue(value == deserialize);
        }
    }
}
