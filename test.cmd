dotnet build --nologo --verbosity quiet -c Release
IF %ERRORLEVEL% NEQ 0 EXIT 1
dotnet run -p "StaticFunctionPointerImplementor" --no-build -c Release helper -directory "./"
dotnet test --no-restore --no-build --nologo --verbosity minimal -c Release -f netcoreapp3.1