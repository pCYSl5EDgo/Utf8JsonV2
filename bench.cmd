REM dotnet build --nologo --verbosity quiet -c Release
devenv.exe Utf8JsonV2.sln /Build release
IF %ERRORLEVEL% NEQ 0 EXIT 1
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"
dotnet run -p "Utf8Json.Benchmark" --no-build -c Release