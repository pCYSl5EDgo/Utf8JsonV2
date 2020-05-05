REM dotnet build --nologo --verbosity quiet -c Release
devenv.exe Utf8JsonV2.sln /Build release
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"