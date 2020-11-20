cd ../src/NbSites.Web

@REM call dotnet nuget list source
call dotnet restore -r win-x64
call dotnet publish -c Release -r win-x64 --self-contained false --no-restore /p:EnvironmentName=Development

pause