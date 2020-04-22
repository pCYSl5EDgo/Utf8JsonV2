extern alias V2;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace Utf8JsonBenchmark
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<JsonBooleanTester>();
            BenchmarkRunner.Run<JsonUInt32ArrayTester>();
            BenchmarkRunner.Run<JsonInt32Tester>();
            BenchmarkRunner.Run<JsonInt32DeserializeTester>();
            BenchmarkRunner.Run<JsonInt64Tester>();
            BenchmarkRunner.Run<JsonInt64DeserializeTester>();
            BenchmarkRunner.Run<JsonStringTester>();
            BenchmarkRunner.Run<JsonStringDeserializeTester>();
            BenchmarkRunner.Run<JsonCharTester>();
            BenchmarkRunner.Run<JsonEnumInt32Tester>();
            BenchmarkRunner.Run<JsonEnumByteTester>();
            BenchmarkRunner.Run<JsonEnumUInt64Tester>();
            BenchmarkRunner.Run<JsonEnumUInt16Tester>();
            BenchmarkRunner.Run<Json8IntegerFieldObjectSerializeTester>();
        }
    }

    [MemoryDiagnoser]
    public class Json8IntegerFieldObjectSerializeTester
    {
        public class MyClass
        {
            public int FirstInt32 { get; set; }
            public int Second { get; set; }
            public int Third { get; set; }
            public int FourthInt { get; set; }
            public int FifthNum { get; set; }
            public int SixthInteger { get; set; }
            public int SeventhDigit { get; set; }
            public int EighthNumber { get; set; }
        }

        public MyClass Value;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Value = new MyClass
            {
                FirstInt32 = int.MinValue,
                Second = int.MaxValue,
                Third = 0,
                FourthInt = -1,
                FifthNum = 114514,
                SixthInteger = -334810,
                SeventhDigit = new Random().Next(int.MinValue, int.MaxValue),
                EighthNumber = new Random().Next(int.MinValue, int.MaxValue),
            };
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonEnumUInt16Tester
    {
        public enum MyEnum : ushort
        {
            None,
            Alice,
            Bob,
            Charles,
            Django,
        }

        [Params(MyEnum.Alice, MyEnum.Bob, MyEnum.Charles, MyEnum.Django, MyEnum.None)]
        public MyEnum Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonEnumUInt64Tester
    {
        public enum MyEnum : ulong
        {
            None,
            Alice,
            Bob,
            Charles,
            Django,
        }

        [Params(MyEnum.Alice, MyEnum.Bob, MyEnum.Charles, MyEnum.Django, MyEnum.None)]
        public MyEnum Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonEnumByteTester
    {
        public enum MyEnum : byte
        {
            None,
            Alice,
            Bob,
            Charles,
            Django,
        }

        [Params(MyEnum.Alice, MyEnum.Bob, MyEnum.Charles, MyEnum.Django, MyEnum.None)]
        public MyEnum Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonEnumInt32Tester
    {
        public enum MyEnum
        {
            None,
            Alice,
            Bob,
            Charles,
            Django,
        }

        [Params(MyEnum.Alice, MyEnum.Bob, MyEnum.Charles, MyEnum.Django, MyEnum.None)]
        public MyEnum Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonInt32DeserializeTester
    {
        [Params(1, 32, 334810, -1145141, -1, 0, 10245, int.MinValue, int.MaxValue)]
        public int Value;

        public byte[] Bytes;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            Bytes = V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public int DeserializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Deserialize<int>(Bytes);
        }

        [Benchmark]
        public int DeserializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Deserialize<int>(Bytes);
        }

        [Benchmark]
        public int DeserializeSystemJsonText()
        {
            return System.Text.Json.JsonSerializer.Deserialize<int>(Bytes);
        }
    }

    [MemoryDiagnoser]
    public class JsonInt64DeserializeTester
    {
        [Params(1L, 32L, 334810L, -1145141L, -1L, 0L, 10245L, (long)int.MinValue, (long)int.MaxValue << 7, long.MinValue, long.MaxValue)]
        public long Value;

        public byte[] Bytes;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            Bytes = V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public long DeserializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Deserialize<long>(Bytes);
        }

        [Benchmark]
        public long DeserializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Deserialize<long>(Bytes);
        }

        [Benchmark]
        public long DeserializeSystemJsonText()
        {
            return System.Text.Json.JsonSerializer.Deserialize<long>(Bytes);
        }
    }

    [MemoryDiagnoser]
    public class JsonUInt32ArrayTester
    {
        [Params(0, 1, 16, 256, 1024)]
        public int Length;

        public uint[] Value;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            Value = Length == 0 ? Array.Empty<uint>() : new uint[Length];
            for (uint i = 0; i < Value.Length; i++)
            {
                Value[i] = i;
            }
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemJsonText()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonStringTester
    {
        [Params("", "1145141919810334", "どげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかん", "文章として崩壊しているのでそのあたり\\\"勘弁してクレメンス\"\\")]
        public string Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemJsonText()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonStringDeserializeTester
    {
        [Params("", "1145141919810334", "どげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかんどげんかせんといかんいかん", "文章として崩壊して\t\tいるのでそのあたり\\\"勘弁してクレメンス\"\\")]
        public string Value;

        public byte[] Bytes;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            Bytes = V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public string DeserializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Deserialize<string>(Bytes);
        }

        [Benchmark]
        public string DeserializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Deserialize<string>(Bytes);
        }

        [Benchmark]
        public string DeserializeSystemJsonText()
        {
            return System.Text.Json.JsonSerializer.Deserialize<string>(Bytes);
        }
    }

    [MemoryDiagnoser]
    public class JsonBooleanTester
    {
        [Benchmark]
        public byte[] SerializeUtf8JsonV1True()
        {
            return global::Utf8Json.JsonSerializer.Serialize(true);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2True()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(true);
        }

        [Benchmark]
        public byte[] SerializeSystemJsonTextTrue()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(true);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV1False()
        {
            return global::Utf8Json.JsonSerializer.Serialize(false);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2False()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(false);
        }

        [Benchmark]
        public byte[] SerializeSystemJsonTextFalse()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(false);
        }

        [Benchmark]
        public bool DeserializeUtf8JsonV1True()
        {
            return global::Utf8Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e', });
        }

        [Benchmark]
        public bool DeserializeUtf8JsonV2True()
        {
            return V2::Utf8Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e', });
        }

        [Benchmark]
        public bool DeserializeSystemJsonTextTrue()
        {
            return System.Text.Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e', });
        }

        [Benchmark]
        public bool DeserializeUtf8JsonV1False()
        {
            return global::Utf8Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', });
        }

        [Benchmark]
        public bool DeserializeUtf8JsonV2False()
        {
            return V2::Utf8Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', });
        }

        [Benchmark]
        public bool DeserializeSystemJsonTextFalse()
        {
            return System.Text.Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', });
        }
    }

    [MemoryDiagnoser]
    public class JsonInt64Tester
    {
        [Params(1L, 32L, -11451419191810334L, -1L, 0, long.MinValue, long.MaxValue)]
        public long Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonInt32Tester
    {
        [Params(1, 32, -1145141, -1, 0, 10245, int.MinValue, int.MaxValue)]
        public int Value;

        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize(Value);
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
        }
    }

    [MemoryDiagnoser]
    public class JsonCharTester
    {
        [Benchmark]
        public byte[] SerializeUtf8JsonV1()
        {
            return global::Utf8Json.JsonSerializer.Serialize('\r');
        }

        [Benchmark]
        public byte[] SerializeUtf8JsonV2()
        {
            return V2::Utf8Json.JsonSerializer.Serialize('\r');
        }

        [Benchmark]
        public byte[] SerializeSystemTextJson()
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes('\r');
        }
    }
}
