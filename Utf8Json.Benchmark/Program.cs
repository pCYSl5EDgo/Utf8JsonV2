extern alias V2;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Utf8JsonBenchmark
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<JsonStringTester>();
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
}
