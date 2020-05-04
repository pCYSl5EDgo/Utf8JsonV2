extern alias V2A;
extern alias V2;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Text;
using Utf8JsonBenchmark;

[assembly:V2A::Utf8Json.RegisterTargetType(typeof(JsonCompoundObjectSerializeTester.EightMemberStruct), 0)]
[assembly:V2A::Utf8Json.RegisterTargetType(typeof(JsonCompoundObjectSerializeTester.SixMemberClass), 1)]
[assembly:V2A::Utf8Json.RegisterTargetType(typeof(JsonCompoundObjectSerializeTester.CompoundClass), 2)]
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
            BenchmarkRunner.Run<Json8IntegerFieldObjectTypelessSerializeTester>();
            BenchmarkRunner.Run<Json8IntegerFieldObjectDeserializeTester>();
            BenchmarkRunner.Run<JsonCompoundObjectSerializeTester>();
        }
    }

    [MemoryDiagnoser]
    public class JsonCompoundObjectSerializeTester
    {
        public struct EightMemberStruct
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

        public class SixMemberClass
        {
            public string 第一の文章 { get; set; }
            public string 第2の文章 { get; set; }
            public string 第３の文章 { get; set; }
            public string 第よんの文章 { get; set; }
            public string 第Vの文章 { get; set; }
            public string 第Ⅵの文章 { get; set; }
        }

        public class CompoundClass
        {
            public EightMemberStruct S0 { get; set; }
            public SixMemberClass S1 { get; set; }
        }

        public CompoundClass Value;

        [GlobalSetup]
        public void GlobalSetUp()
        {
            V2::Utf8Json.JsonSerializerOptionsExtensions.PrepareJsonFormatter(V2::Utf8Json.JsonSerializer.DefaultOptions);
            Value = new CompoundClass
            {
                S0 = new EightMemberStruct
                {
                    Second = 113413,
                    EighthNumber = 121741,
                    Third = -32,
                    FourthInt = int.MinValue,
                    FifthNum = int.MaxValue,
                    SixthInteger = 0,
                    SeventhDigit = -9715725,
                    FirstInt32 = 1,
                },
                S1 = new SixMemberClass
                {
                    第2の文章 = "",
                    //"春は、あけぼの。やうやう白くなりゆく山ぎは少し明りて紫だちたる雲の細くたなびきたる。\n夏は、夜。月の頃はさらなり。闇もなほ。螢の多く飛び違ひたる。また、ただ一つ二つなど、ほのかにうち光りて行くもをかし。雨など降るもをかし。\n秋は、夕暮。夕日のさして、山の端（は）いと近うなりたるに、烏の寝どころへ行くとて、三つ四つ、二つ三つなど、飛び急ぐさへあはれなり。まいて雁などの連ねたるがいと小さく見ゆるは、いとをかし。日入り果てて、風の音、虫の音など、はたいふべきにあらず。\n冬は、つとめて。雪の降りたるはいふべきにもあらず。霜のいと白きも、またさらでも、いと寒きに、火など急ぎ熾して、炭もて渡るも、いとつきづきし。昼になりて、ぬるくゆるびもていけば、火桶の火も、白き灰がちになりて、わろし。"
                    第Vの文章 = "春は、あけぼの。",
                    //"ルイズ！ルイズ！ルイズ！ルイズぅぅうううわぁああああああああああああああああああああああん！！！あぁああああ…ああ…あっあっー！あぁああああああ！！！ルイズルイズルイズぅううぁわぁああああ！！！あぁクンカクンカ！クンカクンカ！スーハースーハー！スーハースーハー！いい匂いだなぁ…くんくん んはぁっ！ルイズ・フランソワーズたんの桃色ブロンドの髪をクンカクンカしたいお！クンカクンカ！あぁあ！！間違えた！モフモフしたいお！モフモフ！モフモフ！髪髪モフモフ！カリカリモフモフ…きゅんきゅんきゅい！！小説12巻のルイズたんかわいかったよぅ！！あぁぁああ…あああ…あっあぁああああ！！ふぁぁあああんんっ！！アニメ2期放送されて良かったねルイズたん！あぁあああああ！かわいい！ルイズたん！かわいい！あっああぁああ！コミック2巻も発売されて嬉し…いやぁああああああ！！！にゃああああああああん！！ぎゃああああああああ！！ぐあああああああああああ！！！コミックなんて現実じゃない！！！！あ…小説もアニメもよく考えたら…ル イ ズ ち ゃ ん は 現実 じ ゃ な い？にゃあああああああああああああん！！うぁああああああああああ！！そんなぁああああああ！！いやぁぁぁあああああああああ！！はぁああああああん！！ハルケギニアぁああああ！！この！ちきしょー！やめてやる！！現実なんかやめ…て…え！？見…てる？表紙絵のルイズちゃんが僕を見てる？表紙絵のルイズちゃんが僕を見てるぞ！ルイズちゃんが僕を見てるぞ！挿絵のルイズちゃんが僕を見てるぞ！！アニメのルイズちゃんが僕に話しかけてるぞ！！！よかった…世の中まだまだ捨てたモンじゃないんだねっ！いやっほぉおおおおおおお！！！僕にはルイズちゃんがいる！！やったよケティ！！ひとりでできるもん！！！あ、コミックのルイズちゃああああああああああああああん！！いやぁあああああああああああああああ！！！！あっあんああっああんあアン様ぁあ！！シ、シエスター！！アンリエッタぁああああああ！！！タバサｧぁあああ！！ううっうぅうう！！俺の想いよルイズへ届け！！ハルケギニアのルイズへ届け！"
                    第Ⅵの文章 = "ルイズ！",
                    第よんの文章 = "..fdko",
                    第一の文章 = "日本語!",
                    第３の文章 = null,
                },
            };
        }

        [Benchmark] public byte[] SerializeUtf8JsonV1() => global::Utf8Json.JsonSerializer.Serialize(Value);
        [Benchmark] public byte[] SerializeUtf8JsonV2() => V2::Utf8Json.JsonSerializer.Serialize(Value);
        [Benchmark] public byte[] SerializeSystemTextJson() => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
    }

    [MemoryDiagnoser]
    public class Json8IntegerFieldObjectSerializeTester
    {
        public struct EightMemberStruct
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

        public EightMemberStruct Value;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Value = new EightMemberStruct
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

        [Benchmark] public byte[] SerializeUtf8JsonV1() => global::Utf8Json.JsonSerializer.Serialize(Value);
        [Benchmark] public byte[] SerializeUtf8JsonV2() => V2::Utf8Json.JsonSerializer.Serialize(Value);
        [Benchmark] public byte[] SerializeSystemTextJson() => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
    }

    [MemoryDiagnoser]
    public class Json8IntegerFieldObjectTypelessSerializeTester
    {
        public struct EightMemberStruct
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

        public object Value;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Value = new EightMemberStruct
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

        [Benchmark] public byte[] SerializeUtf8JsonV1() => global::Utf8Json.JsonSerializer.Serialize(Value);
        [Benchmark] public byte[] SerializeUtf8JsonV2() => V2::Utf8Json.JsonSerializer.Serialize(Value);
        [Benchmark] public byte[] SerializeSystemTextJson() => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Value);
    }

    [MemoryDiagnoser]
    public class Json8IntegerFieldObjectDeserializeTester
    {
        public struct EightMemberStruct
        {
            public int 第一の数値型 { get; set; }
            public int 第二の整数型 { get; set; }
            public int 第三の数の型 { get; set; }
            public int FourthInt { get; set; }
            public int FifthNum { get; set; }
            public int SixthInteger { get; set; }
            public int SeventhDigit { get; set; }
            public int EighthNumber { get; set; }
        }

        public byte[] V1Bytes;
        public byte[] V2Bytes;
        public byte[] TextBytes;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var value = new EightMemberStruct
            {
                第一の数値型 = int.MinValue,
                第二の整数型 = int.MaxValue,
                第三の数の型 = 0,
                FourthInt = -1,
                FifthNum = 114514,
                SixthInteger = -334810,
                SeventhDigit = new Random().Next(int.MinValue, int.MaxValue),
                EighthNumber = new Random().Next(int.MinValue, int.MaxValue),
            };
            V1Bytes = global::Utf8Json.JsonSerializer.Serialize(value);
            V2Bytes = V2::Utf8Json.JsonSerializer.Serialize(value);
            TextBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
        }

        [Benchmark] public EightMemberStruct SerializeUtf8JsonV1() => global::Utf8Json.JsonSerializer.Deserialize<EightMemberStruct>(V1Bytes);
        [Benchmark] public EightMemberStruct SerializeUtf8JsonV2() => V2::Utf8Json.JsonSerializer.Deserialize<EightMemberStruct>(V2Bytes);
        [Benchmark] public EightMemberStruct SerializeSystemTextJson() => System.Text.Json.JsonSerializer.Deserialize<EightMemberStruct>(TextBytes);
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
