$root = Split-Path -Parent $MyInvocation.MyCommand.Path

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root'; dotnet run --project .\src\BLAInterview.Idp\BLAInterview.Idp.csproj --launch-profile https"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root'; dotnet run --project .\src\BLAInterview.WebApi\BLAInterview.WebApi.csproj --launch-profile https"