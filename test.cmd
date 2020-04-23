dotnet build -c Release
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"
dotnet test --no-restore --no-build --nologo -v m -c Release -f netcoreapp3.1