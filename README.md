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

