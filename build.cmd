dotnet build -c Debug
dotnet build -c Release
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"
echo dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release direct -directory "./" -patterns "Utf8Json*.dll"