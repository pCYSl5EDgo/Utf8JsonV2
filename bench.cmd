dotnet build -c Release
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"
dotnet run -p "Utf8Json.Benchmark" --no-build -c Release