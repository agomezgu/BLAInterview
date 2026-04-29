$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet test "$root\BLAInterview.Backend.sln"
