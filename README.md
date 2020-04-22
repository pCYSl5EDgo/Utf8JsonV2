# Utf8JsonV2

Utf8JsonのV2化を試しているリポジトリ

# 試すには

git cloneしてWindowsなら./test.cmd回してテストコードが全て成功したらOK。
./bench.cmdでベンチマークが走るのでそれを使ってV1との性能比較が出来る。

# 依存

netstandard2.0以上で動作することを想定。
Unity2018.4以上を対象としているので基本的にC#7.3で記述。
ただし個人的にC#8のnullableが無いと耐えられないので`#if CSHARP_8_OR_NEWER`とプリプロセッサディレクティブでC#8を導入している。
多分Unity2021とかでこのDefineConstant定義されるでしょ（希望的観測）。

netstandard2.0, netcoreapp2.1, netcoreapp3.1で動作を検証している。
IL2CPPビルドで検証してくれる有志が欲しい……

# デフォルトでシリアライズ可能な型

- プリミティブ型
    - byte
    - sbyte
    - short
    - ushort
    - int
    - uint
    - long
    - ulong
    - IntPtr
    - UIntPtr
    - char
    - bool
    - float
    - double
- string
- 配列
- 32次元までの多次元配列
- ArraySegment&lt;&gt;
- BitArray
- IEnumerable&lt;&gt;
- IGrouping&lt;,&gt;
- ILookup&lt;,&gt;
- ReadOnlyCollection&lt;&gt;
- List&lt;&gt;
- IList&lt;&gt;
- ICollection&lt;&gt;
- LinkedList&lt;&gt;
- Queue&lt;&gt;
- Stack&lt;&gt;
- HashSet&lt;&gt;
- DateTime
- DateTimeOffset
- TimeSpan
- Dictionary&lt;,&gt;
- ReadOnlyDictionary&lt;,&gt;
- SortedDictionary&lt;,&gt;
- IDictionary&lt;,&gt;
- ConcurrentDictionary&lt;,&gt;
- ImmutableDictionary&lt;,&gt;
- ImmutableSortedDictionary&lt;,&gt;
- KeyValuePair&lt;,&gt;
- SortedList&lt;,&gt;
- Exception
- BigInteger
- Complex
- ValueTuple
    - ValueTuple&lt;,&gt;
    - ValueTuple&lt;,,&gt;
    - ValueTuple&lt;,,,&gt;
    - ValueTuple&lt;,,,,&gt;
    - ValueTuple&lt;,,,,,&gt;
    - ValueTuple&lt;,,,,,,&gt;
- Tuple
    - Tuple&lt;,&gt;
    - Tuple&lt;,,&gt;
    - Tuple&lt;,,,&gt;
    - Tuple&lt;,,,,&gt;
    - Tuple&lt;,,,,,&gt;
    - Tuple&lt;,,,,,,&gt;
- ExpandoObject
- Task
- Task&lt;&gt;
- ValueTask&lt;&gt;
- CultureInfo
- DBNull
- Guid
- Nullable&lt;&gt;
- StringBuilder
- Uri
- Version
- MemberInfo
    - MethodBase
        - MethodInfo
        - ConstructorInfo
    - FieldInfo
    - PropertyInfo
    - Type
        - TypeInfo
- CustomAttributeData
    - CustomAttributeNamedArgument
    - CustomAttributeTypedArgument

## Unity環境

- Bounds
- Color
- Color32
- Matrix4x4
- NativeArray&lt;&gt;
- Quaternion
- Rect
- RectInt
- Vecto2
- Vecto2Int
- Vecto3
- Vecto3Int
- Vecto4