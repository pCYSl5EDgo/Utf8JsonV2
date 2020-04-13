extern alias V2;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Utf8JsonBenchmark
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<JsonBooleanTester>();
            /*BenchmarkRunner.Run<JsonInt32Tester>();
            BenchmarkRunner.Run<JsonInt64Tester>();
            BenchmarkRunner.Run<JsonStringTester>();
            BenchmarkRunner.Run<JsonCharTester>();*/
        }
    }

    [MemoryDiagnoser]
    public class JsonStringTester
    {
        [Params("", "1145141919810334", "どげんかせんといかんいかん", "文章として崩壊しているのでそのあたり\\\"勘弁してクレメンス\"\\")]
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
        public bool DeserializeUtf8JsonV1False()
        {
            return global::Utf8Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', });
        }

        [Benchmark]
        public bool DeserializeUtf8JsonV2False()
        {
            return V2::Utf8Json.JsonSerializer.Deserialize<bool>(new[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', });
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
            return V2::Utf8Json.JsonSerializer.Serialize<int>(Value);
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
    }
}
