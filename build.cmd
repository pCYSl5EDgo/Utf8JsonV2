dotnet build -c Release
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release direct -directory "./" -patterns "Utf8Json*.dll"
dotnet test --no-build